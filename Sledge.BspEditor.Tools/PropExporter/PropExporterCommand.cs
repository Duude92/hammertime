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

namespace Sledge.BspEditor.Tools.PropExporter
{
	[Export(typeof(ICommand))]
	[CommandID("Tools:CreateProp")]
	[MenuItem("Tools", "", "CreateProp", "L")]
	public class PropExporterCommand : BaseCommand
	{
		public override string Name { get; set; } = "Create prop";
		public override string Details { get; set; } = "Create prop from selection";

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
				Position = bb.Center,
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
				Name = "idle",
				Framerate = 30,
				Flags = 1,
				Activity = 1,
				ActivityWeight = 1,
				NumEvents = 0,
				EventIndex = 0x0,
				NumFrames = 1,
				NumPivots = 0,
				PivotIndex = 0x0,
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
			} };
			model.SequenceGroups = new List<SequenceGroup>(1)
			{
				new SequenceGroup
				{
					Name = "default",
					Label = "",
				}
			};
			var solids = new List<Solid>();
			foreach (var mapObject in selection)
			{
				var childSolids = new List<Solid>();
				CollectSolids(childSolids, mapObject);
				solids.AddRange(childSolids);
			}
			var textures = solids.SelectMany(x => x.Faces).Select(f => f.Texture).DistinctBy(t => t.Name).ToList();
			var textureCollection = await document.Environment.GetTextureCollection();
			var streamsource = textureCollection.GetStreamSource();
			var texturesCollection = await textureCollection.GetTextureItems(textures.Select(x => x.Name));
			var imageConverter = new ImageConverter();

			var textures1 = await Task.WhenAll(textures.Select(async x =>
			{
				var texFile = texturesCollection.FirstOrDefault(t => t.Name == x.Name);
				Sledge.Providers.Model.Mdl10.Format.Texture ret = default;
				//streamsource.GetImage(x.Name, texFile.Width, texFile.Height).ContinueWith(image=>
				using (var image = await streamsource.GetImage(x.Name, texFile.Width, texFile.Height))
				{
					var (data, palette) = GetBitmapDataWithPalette(image);

					ret = new Sledge.Providers.Model.Mdl10.Format.Texture
					{
						Name = x.Name,
						Flags = TextureFlags.Flatshade,
						Height = texFile.Height,
						Width = texFile.Width,
						Data = data,
						Palette = palette,
						Index = 0x0
					};
					return ret;
				}
			}
			));
			model.Textures = textures1.ToList();
			model.Skins = new List<SkinFamily> { new SkinFamily {
				Textures = new short[] {0,0,0,0,0,0,0},
			} };
			model.Attachments = new List<Attachment>(0);
			var vertices = solids.SelectMany(x => x.Faces).SelectMany(f => f.Vertices).ToList();
			var meshes = solids.Select(s => new Mesh
			{
				Header = new MeshHeader
				{
					NormalIndex = 0x0,
					NumNormals = 0,
					NumTriangles = 0,
					SkinRef = 0,
					TriangleIndex = 0x0,
				},
				Vertices = s.Faces.SelectMany(f => f.Vertices).Select(v => new MeshVertex
				{
					Normal = Vector3.Zero,
					NormalBone = 0,
					Texture = Vector2.Zero,
					Vertex = v,
					VertexBone = 0,
				}).ToArray(),
			}).ToArray();
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
								NumMesh = solids.Count,
								MeshIndex = 0x0,
								NumVerts = vertices.Count,
								VertInfoIndex = 0x0,
								VertIndex = 0x0,
								NumNormals = 0,
								NormalInfoIndex = 0x0,
								NormalIndex = 0x0,
								NumGroups = 0,
								GroupIndex = 0
							},
							Meshes = meshes,
						}
					}
				}
			};
			void CollectSolids(List<Solid> solids, IMapObject parent)
			{
				foreach (var obj in parent.Hierarchy)
				{
					if (obj is Solid s) solids.Add(s);
					else if (obj is Group) CollectSolids(solids, obj);
				}
			}

			(byte[] pixelData, byte[] paletteData) GetBitmapDataWithPalette(Bitmap bitmap)
			{
				// Get the pixel data
				BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				int pixelDataSize = Math.Abs(bmpData.Stride) * bitmap.Height;
				byte[] pixelData = new byte[pixelDataSize];
				System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelData, 0, pixelDataSize);
				bitmap.UnlockBits(bmpData);

				// Get the palette data
				ColorPalette palette = bitmap.Palette;
				byte[] paletteData = new byte[palette.Entries.Length * 4]; // Assuming ARGB format
				for (int i = 0; i < palette.Entries.Length; i++)
				{
					BitConverter.GetBytes(palette.Entries[i].ToArgb()).CopyTo(paletteData, i * 4);
				}

				return (pixelData, paletteData);
			}
			model.Write("D:\\1.mdl");

			return;
		}
	}
}
