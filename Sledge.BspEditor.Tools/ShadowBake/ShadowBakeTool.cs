using LogicAndTrick.Oy;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.ShadowBake.BVH;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Engine;
using Sledge.Shell;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.ShadowBake;
[Export(typeof(ISidebarComponent))]
[Export(typeof(IInitialiseHook))]
public partial class ShadowBakeTool : UserControl, ISidebarComponent, IInitialiseHook
{
	private WeakReference<MapDocument> _document = new WeakReference<MapDocument>(null);
	private const int LightMaxDistance = 5000;

	public string Title => "Bake Light";

	public object Control => this;

	public ShadowBakeTool()
	{
		CreateHandle();

		InitializeComponent();
	}

	private void SetDocument(IDocument document)
	{
		var md = document as MapDocument;
		_document = new WeakReference<MapDocument>(md);
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out CameraTool _);
	}

	private async void BakeButton_Click(object sender, System.EventArgs e)
	{
		_document.TryGetTarget(out var doc);

		//var solids = doc.Map.Root.Hierarchy.OfType<Solid>().Where(solid => solid.Faces.Count() != solid.Faces.Where(face => face.Texture.Name.Equals("sky", StringComparison.InvariantCulture)).Count()).ToList();
		//var slds = doc.Map.Root.Collect(x => true, obj => obj is Solid solid && solid.Faces.Count() != solid.Faces.Where(face => face.Texture.Name.Equals("sky", StringComparison.InvariantCulture)).Count());
		var solids = doc.Map.Root.Collect(x => true, x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && x is Solid solid && solid.Faces.Count() != solid.Faces.Where(face => face.Texture.Name.Equals("sky", StringComparison.InvariantCulture)).Count()).OfType<Solid>();
		var textureCollection = await doc.Environment.GetTextureCollection();
		var textures = solids.SelectMany(x => x.Faces).Select(f => f.Texture).DistinctBy(t => t.Name).ToList();


		var texturesCollection = await textureCollection.GetTextureItems(textures.Select(x => x.Name));

		var faces = solids.SelectMany(x => x.Faces).Where(x => !x.Texture.Name.Equals("sky", StringComparison.InvariantCulture)).ToList();
		var lightVectorRotation = Engine.Interface.GetLightAnglesRadians();

		var mat_x = Matrix4x4.CreateRotationX(lightVectorRotation.Z);
		var mat_y = Matrix4x4.CreateRotationY(lightVectorRotation.X);
		var mat_z = Matrix4x4.CreateRotationZ(lightVectorRotation.Y);
		var rot = mat_x * mat_y * mat_z;
		var lightRotation = Quaternion.CreateFromRotationMatrix(rot);
		Vector3 lightDirection = Vector3.Transform(Vector3.UnitX, lightRotation);

		lightDirection.X *= 1;
		lightDirection.Y *= 1;
		lightDirection.Z *= -1;

		var iopt = MapObjectExtensions.IgnoreOptions.IgnoreClip | MapObjectExtensions.IgnoreOptions.IgnoreNull;
		//var resources = new EngineInterface.DepthResource[faces.Count];
		var resources = new ConcurrentStack<EngineInterface.DepthResource>();
		var i = 0;
		var rand = new Random(DateTime.Now.Millisecond);
		var bvhRoot = new BVH.BVHNode.BVHBuilder().BuildBVHIterative(solids.ToList());
#if DEBUG
		var graph = new Graph("BVH");

		Node TraverseGraph(BVHAbstract bvhNode)
		{
			if (bvhNode == null) return null;
			if (bvhNode is BVHLeaf leaf)
			{
				var nd = new Node($"{leaf.GetHashCode().ToString()}\n{leaf.Bounds.Center.ToFormattedString()}");
				nd.Attr.Shape = Shape.Diamond;
				graph.AddNode(nd);
				return nd;
			}
			var node = new Node($"{bvhNode.GetHashCode().ToString()}\n{bvhNode.Bounds.Center.ToFormattedString()}");

			graph.AddEdge(node.LabelText, TraverseGraph((bvhNode as BVHNode)?.Left)?.LabelText ?? "null");
			graph.AddEdge(node.LabelText, TraverseGraph((bvhNode as BVHNode)?.Right)?.LabelText ?? "null");
			return node;
		}
		graph.AddNode(TraverseGraph(bvhRoot));

		var graphControl = new GViewer();
		var renderer = new GraphRenderer(graph);
		renderer.CalculateLayout();
		graphControl.Graph = graph;
		var form = new Form();
		form.SuspendLayout();
		graphControl.AutoSize = true;
		form.Controls.Add(graphControl);
		form.ResumeLayout();
		form.ShowDialogAsync();
#endif
		Parallel.ForEach(faces, (face) =>
		//foreach (var face in faces)
		{
			var texFile = texturesCollection.FirstOrDefault(t => t.Name.ToLower().Equals(face.Texture.Name.ToLower()));

			// IMPORTANT: Normalize UVs
			var uvs = face.GetTextureCoordinates(64, 64);
			float minU = uvs.Min(x => x.Item2);
			float maxU = uvs.Max(x => x.Item2);
			float minV = uvs.Min(x => x.Item3);
			float maxV = uvs.Max(x => x.Item3);

			var normalizedUvs = uvs.Select(x =>
			{
				float normU = (x.Item2 - minU) / (maxU - minU);
				float normV = (x.Item3 - minV) / (maxV - minV);
				return new Vector2(normU, normV);
			}).ToArray();
			face.Uv1 = normalizedUvs;

			uint width = (uint)MathF.Ceiling((texFile?.Width ?? 256));// * face.Texture.XScale);
			uint height = (uint)MathF.Ceiling((texFile?.Height ?? 256));// * face.Texture.YScale);
			width = width < 8 ? 8 : width;
			height = height < 8 ? 8 : height;
			width = 64;
			height = 64;
			var resource = Engine.Interface.CreateDepthTexture(width, height);
			face.LightMap = resource.Texture;
			var w = 0;
			var cachedSolids = new LinkedList<Solid>();
			var maxLightDistance = (lightDirection.Normalise() * LightMaxDistance);
			var lines = new Line[width * height];
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var projection = face.ProjectedUVtoWorld((float)x / width, (float)y / height);
					lines[w] = new Line(projection, projection - maxLightDistance);
					w++;
				}
			}
			w = 0;
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					resource.MappedResource[w] = 1f;
					var found = false;
					var line = lines[w];
					foreach (var solid in cachedSolids)
					{
						if (solid.BoundingBox.IntersectsWith(line))
						{
							var intersect = solid.GetIntersectionPoint(line);
							if (intersect.HasValue)
							{
								resource.MappedResource[w] = 0.5f;
								found = true;
								break;
							}
						}
					}
					if (!found)
					{
						//var currentSolid = perFaceSolids.First;
						var intersect = TraverseBVH(bvhRoot, line);
						if (intersect.Item1)
						{
							cachedSolids.AddFirst((intersect.Item2 as BVHLeaf).Solid);
							resource.MappedResource[w] = 0.5f;
						}

					}
					w++;
				}
			}
			resources.Push(resource);

			i++;
		}
		);

		Engine.Interface.CopyDepthTexture(resources.ToArray());
		var tr = new Transaction();
		tr.Add(new TrivialOperation(x=> { }, x =>
		{
			x.UpdateRange(x.Document.Map.Root.Find(s => s is Solid));
		}));
		await MapDocumentOperation.Perform(doc, tr);
	}
	private (bool, BVHAbstract) TraverseBVH(BVHAbstract bvhNode, Line line)
	{
		if (!bvhNode.Bounds.IntersectsWith(line))
			return (false, null);

		if (bvhNode is BVHLeaf leaf)
			return (leaf.Solid.GetIntersectionPoint(line).HasValue, leaf);
		var node = bvhNode as BVHNode;
		var left = TraverseBVH(node.Left, line);
		var right = TraverseBVH(node.Right, line);
		return left.Item1 ? left : right;
	}

	public Task OnInitialise()
	{
		Oy.Subscribe<IDocument>("Document:Activated", SetDocument);
		return Task.FromResult(0);
	}

}
public static class Vector3Extensions
{
	public static string ToFormattedString(this Vector3 v)
	{
		return $"({v.X}, {v.Y}, {v.Z})";
	}
}