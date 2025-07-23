using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.ShadowBake.BVH;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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

		var solids = doc.Map.Root.Collect(x => true, x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && x is Solid solid && solid.Faces.Count() != solid.Faces.Where(face => face.Texture.Name.ToLower().Equals("sky", StringComparison.InvariantCulture)).Count()).OfType<Solid>();
		var textureCollection = await doc.Environment.GetTextureCollection();
		var textures = solids.SelectMany(x => x.Faces).Select(f => f.Texture).DistinctBy(t => t.Name).ToList();


		var texturesCollection = await textureCollection.GetTextureItems(textures.Select(x => x.Name));

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

		var resources = new ConcurrentStack<EngineInterface.DepthResource>();
		var i = 0;
		var rand = new Random(DateTime.Now.Millisecond);
		var bvhRoot = new BVH.BVHNode.BVHBuilder().BuildBVHIterative(solids.ToList());

		var startTime = DateTime.Now;
		var chunkCount = System.Environment.ProcessorCount;
		var nestLevel = MathF.Ceiling(MathF.Sqrt(chunkCount));
		var solidChunks = new List<List<Solid>>();
		BVHAbstract.GroupId = 0;
		bvhRoot.GetLeafs((int)nestLevel, 0, solidChunks);
		var facesChunks = solidChunks.Select(chunk => chunk.SelectMany(x => x.Faces).Where(x => !x.Texture.Name.ToLower().Equals("sky", StringComparison.InvariantCulture)).ToList());

		var stringStack = new ConcurrentStack<string>();
		Parallel.ForEach(facesChunks, (faceChunk) =>
		{
			var chunkData = new List<(uint, uint, float[])>();
			foreach (var face in faceChunk)
			{
				var texFile = texturesCollection.FirstOrDefault(t => t.Name.ToLower().Equals(face.Texture.Name.ToLower()));

				uint width, height;

				var size = face.GetTextureResolution(0.25f);
				width = (uint)size.Width;
				height = (uint)size.Height;


				// IMPORTANT: Normalize UVs
				var uvs = face.GetTextureCoordinates((int)width, (int)height);
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

				var resource = Engine.Interface.CreateDepthTexture(width, height);
				face.LightMap = resource.Texture;
				var w = 0;
				var cachedSolids = new LinkedList<Solid>();
				var maxLightDistance = (lightDirection.Normalise() * LightMaxDistance);
				var lines = new Line[width * height];
				var data = new float[width * height];

				for (var x = 0; x < width; x++)
				{
					for (var y = 0; y < height; y++)
					{
						w = (int)(y * width + x);
						data[w] = 1f;

						var projection = face.ProjectedUVtoWorld((float)y / height, (float)x / width);
						lines[w] = new Line(projection, projection - maxLightDistance);
					}
				}
				var firstLine = lines[0];
				var onPlane = face.Plane.OnPlane(firstLine.End);
				if (onPlane < 0)
				{
					for (var i = 0; i < data.Length; i++)
					{
						data[i] = 0.5f;
					}
				}
				else
				{
					w = 0;
					for (var x = 0; x < width; x++)
					{
						for (var y = 0; y < height; y++)
						{
							w = (int)(y * width + x);

							var found = false;
							var line = lines[w];

							foreach (var solid in cachedSolids)
							{
								if (solid.BoundingBox.IntersectsWith(line))
								{
									var intersect = solid.GetIntersectionPoint(line);
									if (intersect.HasValue)
									{
										data[w] = 0.5f;
										found = true;
										break;
									}
								}
							}
							if (!found)
							{
								var intersect = TraverseBVH(bvhRoot, line, face);
								if (intersect.Item1)
								{
									cachedSolids.AddFirst((intersect.Item2 as BVHLeaf).Solid);
									data[w] = 0.5f;
								}

							}
						}
					}
				}
				resources.Push(resource);
				i++;

				for (int y = 0; y < height; y++)
				{
					IntPtr dest = resource.MappedResource.MappedResource.Data + (int)(y * resource.MappedResource.MappedResource.RowPitch);
					Marshal.Copy(data, (int)(y * width), dest, (int)width);
				}
				chunkData.Add((width, height, data));
			}
			var textureSize = BitOperations.RoundUpToPowerOf2((uint)MathF.Ceiling(MathF.Sqrt(chunkData.Count)));
			var resource = Engine.Interface.CreateDepthTexture(textureSize, textureSize);


			stringStack.Push($"Chunk data is {chunkData.Count}, texture size is {textureSize}");
		}
		);

		while (stringStack.TryPop(out string result))
		{
			Log.Debug(result, "ShadowBakeTool");
		}
		Log.Info($"Bake Light took {DateTime.Now - startTime} seconds", "ShadowBakeTool");
		Engine.Interface.CopyDepthTexture(resources.ToArray());
		var tr = new Transaction();
		tr.Add(new TrivialOperation(x => { }, x =>
		{
			x.UpdateRange(x.Document.Map.Root.Find(s => s is Solid));
		}));
		await MapDocumentOperation.Perform(doc, tr);
	}
	private (bool, BVHAbstract) TraverseBVH(BVHAbstract bvhNode, Line line, Face solidToIgnore = null)
	{
		if (bvhNode.Bounds == null)
		{
			return (false, null);
		}
		if (!bvhNode.Bounds.IntersectsWith(line))
			return (false, null);

		if (bvhNode is BVHLeaf leaf)
		{
			var intersect = leaf.Solid.GetIntersectionPoint(line);
			if (intersect.HasValue && (leaf.Solid.Faces.Contains(solidToIgnore))) return (false, null);
			return (intersect.HasValue, leaf);
		}
		var node = bvhNode as BVHNode;
		var left = TraverseBVH(node.Left, line, solidToIgnore);
		var right = TraverseBVH(node.Right, line, solidToIgnore);
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