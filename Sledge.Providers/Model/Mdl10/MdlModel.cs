using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Model.Mdl10.Format;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Veldrid;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Buffer = Sledge.Rendering.Resources.Buffer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Sledge.Providers.Model.Mdl10
{
	public class MdlModel : IModel
	{
		private const int TEXTURE_MARGIN = 2;
		public MdlFile Model { get; }

		private readonly Guid _guid;
		private uint[][] _bodyPartIndices;

		private Rendering.Resources.Texture _textureResource;
		private Buffer _buffer;
		private uint _numTexturedIndices;
		private uint _numWireframeIndices;

		private string TextureName => $"{nameof(MdlModel)}::{_guid}";

		public MdlModel(MdlFile model)
		{
			Model = model;
			_guid = Guid.NewGuid();
		}

		public List<string> GetSequences()
		{
			return Model.Sequences.Select(x => x.Header.Name).ToList();
		}

		public (Vector3, Vector3) GetBoundingBox(int sequence, int frame, float subframe)
		{
			var transforms = new Matrix4x4[Model.Bones.Count];
			Model.GetTransforms(sequence, frame, subframe, ref transforms);

			var list =
				from part in Model.BodyParts
				from mesh in part.Models[0].Meshes
				from vertex in mesh.Vertices
				let transform = transforms[vertex.VertexBone]
				select Vector3.Transform(vertex.Vertex, transform);

			var box = new Box(list);
			return (box.Start, box.End);
		}

		private static Bitmap CreateBitmap(int width, int height, byte[] data, byte[] palette, bool lastTextureIsTransparent)
		{
			var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

			// Set palette
			var pal = bmp.Palette;
			for (var j = 0; j <= byte.MaxValue; j++)
			{
				var k = j * 3;
				pal.Entries[j] = Color.FromArgb(255, palette[k], palette[k + 1], palette[k + 2]);
			}

			if (lastTextureIsTransparent)
			{
				pal.Entries[pal.Entries.Length - 1] = Color.Transparent;
			}
			bmp.Palette = pal;

			// Write entries
			var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
			Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
			bmp.UnlockBits(bmpData);

			return bmp;
		}
		List<Rectangle> _originalRectangles;
		private List<Rectangle> CreateTexuture(EngineInterface engine, RenderContext context)
		{
			if (!Model.Textures.Any()) return new List<Rectangle>();

			// Combine all the textures into one long texture
			var textures = Model.Textures.Select(x => CreateBitmap(x.Header.Width, x.Header.Height, x.Data, x.Palette, x.Header.Flags.HasFlag(TextureFlags.Masked))).ToList();

			var width = textures.Max(x => x.Width);
			var height = textures.Max(x => x.Height);

			var rectangles = new List<Rectangle>();
			_originalRectangles = new List<Rectangle>(textures.Count);

			var data = new byte[textures.Count][];
			var i = 0;
			foreach (var bitmap in textures)
			{
				var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
				using (var g = Graphics.FromImage(bmp))
				{
					g.DrawImage(bitmap, new Rectangle(0, 0, width, height));

					_originalRectangles.Add(new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					var lb = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
					data[i] = new byte[lb.Stride * lb.Height];
					Marshal.Copy(lb.Scan0, data[i], 0, data[i].Length);
					bmp.UnlockBits(lb);
				}

				i++;
				bmp.Dispose();
			}

			rectangles = _originalRectangles;

			_textureResource = engine.UploadTexture(TextureName, width, height, data, TextureSampleType.Standard, (uint)textures.Count);

			return rectangles;
		}
		private List<Rectangle> _rectangles;
		public void CreateResources(EngineInterface engine, RenderContext context)
		{
			_rectangles = CreateTexuture(engine, context);
			_buffer = engine.CreateBuffer();

			ReInitResources();
		}
		public void ReInitResources(int skinId = 0, int bodyPartId = 0)
		{
			var texHeight = _rectangles.Max(x => x.Bottom);
			var texWidth = _rectangles.Max(x => x.Right);
			var maxTexSize = new Vector2(texWidth, texHeight);
			var vertices = new List<VertexModel3>();
			var indices = new Dictionary<short, List<uint>>();
			for (short i = 0; i < Model.Textures.Count; i++) indices[i] = new List<uint>();

			var wireframeIndices = new List<uint>();


			var _bodyPartIndices1 = new uint[Model.BodyParts.Count][];

			uint vi = 0;
			var skinMax = Math.Max(0, Math.Min(skinId, Model.Skins.Count - 1));
			bodyPartId = Math.Max(0, bodyPartId);

			var skin = Model.Skins[skinMax].Textures;
			for (var bpi = 0; bpi < Model.BodyParts.Count; bpi++)
			{
				var part = Model.BodyParts[bpi];
				_bodyPartIndices1[bpi] = new uint[part.Models.Length];

				var body = bodyPartId % part.Models.Length;
				bodyPartId /= part.Models.Length;
				var model = part.Models[body];
				_bodyPartIndices1[bpi][0] = (uint)model.Meshes.Sum(x => x.Vertices.Length);
				foreach (var mesh in model.Meshes)
				{
					var texId = skin[mesh.Header.SkinRef];
					var rec = _rectangles.Count > texId ? _rectangles[texId] : Rectangle.Empty;
					for (var i = 0; i < mesh.Vertices.Length; i++)
					{
						var x = mesh.Vertices[i];
						var origRect = _originalRectangles.Count > texId ? _originalRectangles[texId] : Rectangle.Empty;

						var coeff = maxTexSize / new Vector2(origRect.Width, origRect.Height);
						var texturePosition = new Vector3((x.Texture.X / maxTexSize.X) * coeff.X, (x.Texture.Y / maxTexSize.Y) * coeff.Y, mesh.Header.SkinRef);
						vertices.Add(new VertexModel3
						{
							Position = x.Vertex,
							Normal = x.Normal,
							Texture = texturePosition,
							Bone = (uint)x.VertexBone
						});
						indices[texId].Add(vi);
						wireframeIndices.Add(vi);
						wireframeIndices.Add(i % 3 == 2 ? vi - 2 : vi + 1);
						vi++;
					}
				}
			}
			_bodyPartIndices = _bodyPartIndices1;

			var flatIndices = new uint[vi + wireframeIndices.Count];
			var currentIndexCount = 0;
			foreach (var kv in indices.OrderBy(x => x.Key))
			{
				var num = kv.Value.Count;
				Array.Copy(kv.Value.ToArray(), 0, flatIndices, currentIndexCount, num);
				currentIndexCount += num;
			}
			Array.Copy(wireframeIndices.ToArray(), 0, flatIndices, currentIndexCount, wireframeIndices.Count);

			_buffer.Update(vertices, flatIndices);

			_numTexturedIndices = (uint)(flatIndices.Length - wireframeIndices.Count);
			_numWireframeIndices = (uint)wireframeIndices.Count;
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
		{
			_buffer.Bind(cl, 0);

			if (pipeline.Type == PipelineType.TexturedModel)
			{
				_textureResource.BindTo(cl, 1);
				uint ci = 0;

				foreach (var bpi in _bodyPartIndices)
				{
					const int model = 0;
					for (var j = 0; j < bpi.Length; j++)
					{
						if (j == model) cl.DrawIndexed(bpi[j], 1, ci, 0, 0);
						ci += bpi[j];
					}
				}
			}
			else if (pipeline.Type == PipelineType.WireframeModel)
			{
				cl.DrawIndexed(_numWireframeIndices, 1, _numTexturedIndices, 0, 0);
			}
		}

		public void DestroyResources()
		{
			_buffer?.Dispose();
			_textureResource?.Dispose();
		}

		public void Dispose()
		{
			//
		}
	}
}