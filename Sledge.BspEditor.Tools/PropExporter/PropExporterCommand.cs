using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;
using Sledge.Providers.Model.Mdl10.Format;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Primitives.MapObjects;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using Version = Sledge.Providers.Model.Mdl10.Format.Version;
using Sledge.DataStructures.Geometric;
using SixLabors.ImageSharp.ColorSpaces;

namespace Sledge.BspEditor.Tools.PropExporter
{
	[Export(typeof(ICommand))]
	[CommandID("Tools:CreateProp")]
	[MenuItem("Tools", "", "CreateProp", "L")]
	public class PropExporterCommand : BaseCommand
	{
		public override string Name { get; set; } = "Create prop";
		public override string Details { get; set; } = "Create prop from selection";
		private string[] _filterTextures = new string[]
		{
			"null",
			"sky",
		};

		protected async override Task Invoke(MapDocument document, CommandParameters parameters)
		{
			var selection = document.Selection;
			MdlFile model = new MdlFile();
			var bb = selection.GetSelectionBoundingBox();
			model.Header = new Header
			{
				ID = ID.Idst,
				Version = Version.Goldsource,
				Name = "brushSolidMdl",
				Size = 0x0,
				EyePosition = Vector3.Zero,
				HullMin = Vector3.Zero,
				HullMax = Vector3.Zero,
				BoundingBoxMin = bb.Start,
				BoundingBoxMax = bb.End,
				Flags = 0,

			};
			model.Bones = new List<Bone> { new Bone {
				Name = "bone0",
				Parent = -1,
				Flags = 0 ,
				Controllers = new int[6]{-1,-1,-1,-1,-1,-1},
				Position = Vector3.Zero,
				PositionScale = Vector3.One,
				Rotation = Vector3.Zero,
				RotationScale = Vector3.One,
			} };
			model.BoneControllers = new List<BoneController>(0);
			model.Hitboxes = new List<Hitbox>
			{
				new Hitbox {
					Bone = 0,
					Group = 0,
					Min = bb.Start,
					Max = bb.End,
				}
			};
			model.Sequences = new List<Sequence>(1) { new Sequence
			{
				Header = new SequenceHeader{
					Name = "idle",
					Framerate = 30,
					Flags = 1,
					Activity = 1,
					ActivityWeight = 1,
					NumEvents = 0,
					EventIndex = 0,//offset to event, Zero as we dont have anims
					NumFrames = 1,
					NumPivots = 0,
					PivotIndex = 0,
					MotionType = 0,
					MotionBone = 0,
					LinearMovement = Vector3.Zero,
					AutoMoveAngleIndex = 0,
					AutoMovePositionIndex = 0,
					Min = bb.Start,
					Max = bb.End,
					NumBlends = 1,
					AnimationIndex = 0x0,
					BlendType = new int[2] {0,0},
					BlendStart = new float[2] {0,0},
					BlendEnd = new float[2] {1,0},
					BlendParent = 0,
				}
			} };
			model.SequenceGroups = new List<SequenceGroup>(1)
			{
				new SequenceGroup
				{
					Name = "",
					Label = "default",
				}
			};
			var solids = selection.OfType<Solid>();

			{
				var childSolids = new List<Solid>();
				CollectSolids(childSolids, mapObject);
			//foreach (var mapObject in selection)
			//{
			//	var childSolids = CollectSolids(mapObject);
			//	solids.AddRange(childSolids);
			//}
			solids = solids.Distinct().ToList();

			var textures = solids.SelectMany(x => x.Faces).Select(f => f.Texture).DistinctBy(t => t.Name).ToList();
			var textureCollection = await document.Environment.GetTextureCollection();
			var streamsource = textureCollection.GetStreamSource();
			var texturesCollection = await textureCollection.GetTextureItems(textures.Select(x => x.Name));
			var imageConverter = new ImageConverter();

			var textures1 = await Task.WhenAll(textures.Select(async x =>
			{
				var texFile = texturesCollection.FirstOrDefault(t => t.Name == x.Name);
				using (var image = await streamsource.GetImage(x.Name, texFile.Width, texFile.Height))
				{
					return new Sledge.Providers.Model.Mdl10.Format.Texture(GetBitmapDataWithPalette(image, texFile.Height, texFile.Width), new TextureHeader
					{
						Name = x.Name,
						Flags = TextureFlags.Flatshade,
						Height = texFile.Height,
						Width = texFile.Width,
						Index = 0x0
					});
				}
			}
			));
			model.Textures = textures1.ToList();
			model.Skins = new List<SkinFamily> { new SkinFamily {
				Textures = new short[] {0,0,0,0,0,0,0},
			}};
			model.Attachments = new List<Attachment>(0);


			var meshVertices = solids.SelectMany(x => x.Faces).SelectMany(f => f.Vertices).Distinct().Select(v => new MeshVertex
			{
				Normal = Vector3.Zero,
				NormalBone = 0,
				Texture = Vector2.Zero,
				Vertex = v,
				VertexBone = 0,
			}).ToList();
			var rand = new Random(0);
			var meshes = solids
				.SelectMany(s => s.Faces)
				.GroupBy(f => f.Texture.Name)
				.Where(g => !_filterTextures.Contains(g.First().Texture.Name.ToLower()))
				.Select(g => new Mesh
				{
					Header = new MeshHeader
					{
						NormalIndex = 0x0,
						NumNormals = g.Sum(f => f.Vertices.Count),
						NumTriangles = g.Sum(f => f.Vertices.Count - 2), //TODO: count right (x2 is for quads)
						SkinRef = Array.IndexOf(textures1, textures1.First(t => t.Header.Name == g.First().Texture.Name)),
						TriangleIndex = 0x0
					},
					Vertices = g.SelectMany(f => f.Vertices).Distinct().Select(v => meshVertices.FirstOrDefault(mv=>mv.Vertex == v)).ToArray(),
					Sequences = g.Select(f =>
					{
						var tex = textures1.FirstOrDefault(t => t.Header.Name.Equals(f.Texture.Name));
						var uv = f.GetTextureCoordinates(tex.Header.Width, tex.Header.Height).ToArray();
						var i = 0;
						var triseq = new TriSequence
						{
							TriCountDir = (short)(f.Vertices.Count * -1),
							TriVerts = f.Vertices.Select(v =>
							{

								var triv = new Trivert
								{
									normindex = 0,
									vertindex = (short)meshVertices.IndexOf(meshVertices.FirstOrDefault(mv => mv.Vertex == v)),
									s = (short)(uv[i].Item2 * tex.Header.Width),
									t = (short)(uv[i].Item3 * tex.Header.Height),
								};
								i++;

								return triv;
							}).ToArray()

						};
						return triseq;
					}).ToArray()
				}).ToArray();
			var tIndex = 0;
			model.Skins = new List<SkinFamily>(1)
			{
				new SkinFamily
				{
					Textures = textures1.Select(t=>(short)tIndex++).ToArray(),
				}
			};

			for (var i = 0; i < meshes.Length; i++)
			{
				var seq = meshes[i].Sequences.ToList();
				seq.Add(new TriSequence { TriCountDir = 0, TriVerts = new Trivert[0] });
				meshes[i].Sequences = seq.ToArray();
			}
			model.BodyParts = new List<BodyPart>
			{
				new BodyPart
				{
					Header = new BodyPartHeader{
						Name = "body",
						NumModels = 1,
						Base = 1,
						ModelIndex = 0x0
					},
					Models = new Model[]
					{
						new Model
						{
							Header = new ModelHeader
							{
								Name = "solidBrush",
								Type = 0,
								Radius = 0,
								NumMesh = meshes.Length,
								MeshIndex = 0x0,
								NumVerts = meshVertices.Count,
								VertInfoIndex = 0x0,
								VertIndex = 0x0,
								NumNormals = meshVertices.Count,
								NormalInfoIndex = 0x0,
								NormalIndex = 0x0,
								NumGroups = 0,
								GroupIndex = 0
							},
							Meshes = meshes,
							Vertices = meshVertices.ToArray(),
						}
					}
				}
			};
			void CollectSolids(List<Solid> solids, IMapObject parent)
			{
				if (parent is Solid) solids.Add(parent as Solid);

				foreach (var obj in parent.Hierarchy)
				{
					if (obj is Solid s) solids.Add(s);
					else if (obj is Group) CollectSolids(solids, obj);
				}
			}


			(byte[] pixelData, byte[] paletteData) GetBitmapDataWithPalette(Bitmap bitmap, int height, int width)
			{
				// Get the pixel data
				BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				int pixelDataSize = Math.Abs(bmpData.Stride) * bitmap.Height;
				byte[] pixelData = new byte[pixelDataSize];
				System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelData, 0, pixelDataSize);
				bitmap.UnlockBits(bmpData);

				// Get the palette data
				ColorPalette palette = bitmap.Palette;
				byte[] paletteData = new byte[palette.Entries.Length * 3]; // Assuming ARGB format
				for (int i = 0; i < palette.Entries.Length; i++)
				{
					var argb = palette.Entries[i].ToVector4();
					paletteData[(i * 3) + 0] = (byte)(palette.Entries[i].R);
					paletteData[(i * 3) + 1] = (byte)(palette.Entries[i].G);
					paletteData[(i * 3) + 2] = (byte)(palette.Entries[i].B);


				}

				return (pixelData, paletteData);
			}
			model.Write("D:\\1.mdl");

			return;
		}
	}

}
