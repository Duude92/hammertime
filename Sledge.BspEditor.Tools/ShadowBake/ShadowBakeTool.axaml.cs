using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Tools.ShadowBake.BVH;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Engine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Primitives.MapObjects;
using System.Linq;
using RectpackSharp;
using Sledge.DataStructures.Geometric;
using Vortice.Mathematics;
using Sledge.Common.Logging;
using Sledge.Shell;
using Sledge.BspEditor.Primitives.MapObjectData;
using System.Runtime.InteropServices;
using Avalonia.Interactivity;


namespace Sledge.BspEditor.Tools;

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

		InitializeComponent();
		progressBar1.IsVisible = false;
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

	private async void BakeButton_Click(object sender, RoutedEventArgs e)
	{
		progressBar1.Value = 0;
		progressBar1.IsVisible = true;
		_document.TryGetTarget(out var doc);

		var nonRenderableTextures = doc.Environment.NonRenderableTextures;
		var solids = doc.Map.Root.Collect(x => true, x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && x is Solid solid && solid.Faces.Count() != solid.Faces.Where(face => nonRenderableTextures.Contains(face.Texture.Name.ToLower())).Count()).OfType<Solid>();

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
		var rand = new Random(DateTime.Now.Millisecond);
		var bvhRoot = new ShadowBake.BVH.BVHNode.BVHBuilder().BuildBVHIterative(solids.ToList());

		var startTime = DateTime.Now;
		var chunkCount = System.Environment.ProcessorCount;
		var nestLevel = MathF.Ceiling(MathF.Sqrt(chunkCount));
		var solidChunks = new List<List<Solid>>();
		BVHAbstract.GroupId = 0;
		bvhRoot.GetLeafs(solidChunks, (int)nestLevel, 0);
		var facesChunks = solidChunks.Select(chunk => chunk.SelectMany(x => x.Faces).Where(x => !x.Texture.Name.ToLower().Equals("sky", StringComparison.InvariantCulture)).ToList());
		progressBar1.Maximum = facesChunks.Select(facesChunk => facesChunk.Count()).Aggregate(0, (cur, next) => cur += next);
		//progressBar1.Step = 1;
		progressBar1.Minimum = 0;
		await Parallel.ForEachAsync(facesChunks, async (faceChunk, _) =>
		{
			var chunkData = new List<(uint, uint, float[])>();
			var chunkDataId = 0;
			List<PackingRectangle> rectangles = new();

			foreach (var face in faceChunk)
			{

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

				var w = 0;
				var cachedSolids = new LinkedList<Solid>();
				var maxLightDistance = (lightDirection.Normalise() * LightMaxDistance);
				var lines = new Line[width * height];
				var data = new float[width * height];

				//TODO: Move min/max uv calculations, so it wouldn't recalculate those values each pixel again
				minU = float.MaxValue; maxU = float.MinValue;
				minV = float.MaxValue; maxV = float.MinValue;

				foreach (var vertex in face.Vertices)
				{
					Vector3 local = vertex - face.Vertices[0];

					float u = Vector3.Dot(local, face.Texture.UAxis) / face.Texture.YScale;
					float v = Vector3.Dot(local, face.Texture.VAxis) / face.Texture.XScale;

					minU = MathF.Min(minU, u);
					maxU = MathF.Max(maxU, u);
					minV = MathF.Min(minV, v);
					maxV = MathF.Max(maxV, v);
				}

				for (var x = 0; x < width; x++)
				{
					for (var y = 0; y < height; y++)
					{
						w = (int)(y * width + x);
						data[w] = 1f;

						float relY = (float)y / height;
						float relX = (float)x / width;
						float uWorld = MathHelper.Lerp(minU, maxU, relX);
						float vWorld = MathHelper.Lerp(minV, maxV, relY);


						var projection = face.ProjectedUVtoWorld(uWorld, vWorld);
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
								var intersect = await TraverseBVH(bvhRoot, line, face);
								if (intersect.Item1)
								{
									cachedSolids.AddFirst((intersect.Item2 as BVHLeaf).Solid);
									data[w] = 0.5f;
								}

							}
						}
					}
				}

				chunkData.Add((width, height, data));
				var rect = new PackingRectangle
				{
					X = 0,
					Y = 0,
					Width = width,
					Height = height,
					Id = chunkDataId
				};
				rectangles.Add(rect);

				chunkDataId++;
				UpdateProgress();
			}

			var rects = rectangles.ToArray();

			RectanglePacker.Pack(rects, out PackingRectangle bounds);
			var boundsSize = BitOperations.RoundUpToPowerOf2((uint)MathF.Max(bounds.Width, bounds.Height));

			var packedData = new float[boundsSize * boundsSize];

			for (int i = 0; i < chunkData.Count; i++)
			{
				var rect = rects[i];
				var dataId = rect.Id;

				for (int iy = 0; iy < rect.Height; iy++)
				{
					for (int ix = 0; ix < rect.Width; ix++)
					{
						int srcIndex = (int)(iy * rect.Width + ix);
						int dstIndex = (int)((rect.Y + iy) * boundsSize + (rect.X + ix));

						packedData[dstIndex] = chunkData[dataId].Item3[srcIndex];

					}
				}
			}
			var resource = Engine.Interface.CreateDepthTexture(boundsSize, boundsSize);


			IntPtr dest = resource.MappedResource.MappedResource.Data;
			Marshal.Copy(packedData, 0, dest, packedData.Length);
			for (int i = 0; i < chunkData.Count; i++)
			{
				var rect = rects[i];

				faceChunk[rect.Id].LightMap = resource.Texture;
				for (int k = 0; k < faceChunk[rect.Id].Uv1.Length; k++)
				{
					faceChunk[rect.Id].Uv1[k] = new Vector2(
						(faceChunk[rect.Id].Uv1[k].X * rect.Width + rect.X) / boundsSize,
						(faceChunk[rect.Id].Uv1[k].Y * rect.Height + rect.Y) / boundsSize
					);
				}
			}
			resources.Push(resource);

		}
		);

		Log.Info("ShadowBakeTool", $"Bake Light took {DateTime.Now - startTime} seconds");
		Engine.Interface.CopyDepthTexture(resources.ToArray());
		var tr = new Transaction();
		tr.Add(new TrivialOperation(x => { }, x =>
		{
			x.UpdateRange(solids);
		}));
		await MapDocumentOperation.Perform(doc, tr);
		await ((Func<Task>)(async () =>
		{
			await Task.Delay(1000);
			progressBar1.IsVisible = false;
		}))();
	}
	private void UpdateProgress()
	{
		Avalonia.Threading.Dispatcher.UIThread.Post(()=> progressBar1.Value++ );
		//if (progressBar1.InvokeRequired)
		//{
		//	progressBar1.Invoke(() => progressBar1.PerformStep());
		//}
		//else
		//{
		//	progressBar1.PerformStep();
		//}
	}

	private async Task<(bool, BVHAbstract)> TraverseBVH(BVHAbstract bvhNode, Line line, Face solidToIgnore = null)
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
		var left = await TraverseBVH(node.Left, line, solidToIgnore);
		var right = await TraverseBVH(node.Right, line, solidToIgnore);
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