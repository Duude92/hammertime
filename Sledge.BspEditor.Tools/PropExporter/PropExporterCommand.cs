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
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;
using System.Windows.Forms;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.Common.Shell.Settings;
using System.IO;
using Sledge.QuickForms;
using System.Threading.Channels;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Tools.PropExporter
{
	[AutoTranslate]
	[Export(typeof(ICommand))]
	[Export(typeof(ISettingsContainer))]
	[CommandID("Tools:CreateProp")]
	[MenuItem("Tools", "", "CreateProp", "L")]
	public class PropExporterCommand : BaseCommand, ISettingsContainer
	{
		public override string Name { get; set; } = "Create mdl prop";
		public override string Details { get; set; } = "Create prop from selection";

		public bool ValuesLoaded => true;
		public string lastPath = null;

		private string[] _filterTextures = new string[]
		{
			"null",
			"sky",
		};

		protected async override Task Invoke(MapDocument document, CommandParameters parameters)
		{
			if (document.Selection.IsEmpty || !document.Selection.OfType<Solid>().Any())
			{
				MessageBox.Show("Nothing to build.\nTry to select objects to build prop.", "Build error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			var defaultPath = string.IsNullOrEmpty(lastPath) ? (document.Environment as GoldsourceEnvironment).BaseDirectory : lastPath;
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = defaultPath;
			dialog.Filter = "Models (*.mdl)|*.mdl";
			if (dialog.ShowDialog() != DialogResult.OK) return;
			var path = dialog.FileName;
			lastPath = Path.GetDirectoryName(path);
			var selection = document.Selection;
			MdlFile model = new MdlFile();
			var bb = selection.GetSelectionBoundingBox();
			var matrix = Matrix4x4.CreateTranslation(-bb.Center);
			var transaction = new Transaction(new Transform(matrix, selection.GetSelectedParents()));
			transaction.Add(new TransformTexturesUniform(matrix, selection.GetSelectedParents().SelectMany(p => p.FindAll())));
			await MapDocumentOperation.Perform(document, transaction);
			var bbmin = bb.Center - bb.End;
			var bbmax = bb.Center - bb.Start;
			bbmin = new Vector3(bbmin.Y, bbmin.X, bbmin.Z);
			bbmax = new Vector3(bbmax.Y, bbmax.X, bbmax.Z);
			model.Header = new Header
			{
				ID = ID.Idst,
				Version = Version.Goldsource,
				Name = "brushSolidMdl",
				Size = 0x0,
				EyePosition = Vector3.Zero,
				HullMin = Vector3.Zero,
				HullMax = Vector3.Zero,
				BoundingBoxMin = bbmin,
				BoundingBoxMax = bbmax,
				Flags = 0,

			};
			model.Bones = new List<Bone> { new Bone {
				Name = "bone0",
				Parent = -1,
				Flags = 0 ,
				Controllers = new int[6]{-1,-1,-1,-1,-1,-1},
				Position = Vector3.Zero,
				PositionScale = new Vector3(0.00390637f,0.00390637f,0.00390637f),
				Rotation = Vector3.Zero,
				RotationScale = new Vector3(0.00001198f,0.00001198f,0.00004794f),
			} };
			model.BoneControllers = new List<BoneController>(0);
			model.Hitboxes = new List<Hitbox>
			{
				new Hitbox {
					Bone = 0,
					Group = 0,
					Min = bb.Center - bb.End,
					Max = bb.Center - bb.Start,
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
					EventIndex = 580,//offset to event, Zero as we dont have anims
					NumFrames = 1,
					NumPivots = 0,
					PivotIndex = 580,
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

			solids = solids.Distinct().ToList();

			var entities = selection.OfType<Primitives.MapObjects.Entity>().Where(e => e.EntityData.Get<int>("renderamt", 255) < 255);
			var transparentTexture = entities.SelectMany(e => e.FindAll().OfType<Solid>()).SelectMany(s => s.Faces).Select(f => f.Texture.Name).Distinct().ToList();

			var textures = solids.SelectMany(x => x.Faces).Select(f => f.Texture).DistinctBy(t => t.Name).ToList();
			var textureCollection = await document.Environment.GetTextureCollection();
			var streamsource = textureCollection.GetStreamSource();
			var texturesCollection = await textureCollection.GetTextureItems(textures.Select(x => x.Name));
			var imageConverter = new ImageConverter();
			var faces = solids.SelectMany(s => s.Faces);

			var textures1 = await Task.WhenAll(textures.Select(async x =>
			{
				var texFile = texturesCollection.FirstOrDefault(t => t.Name.ToLower().Equals(x.Name.ToLower()));
				using (var image = await streamsource.GetRawImage(x.Name, texFile.Width, texFile.Height))
				{
					return new Sledge.Providers.Model.Mdl10.Format.Texture(GetBitmapDataWithPalette(image, texFile.Height, texFile.Width), new TextureHeader
					{
						Name = x.Name,
						Flags = transparentTexture.Contains(x.Name) ? TextureFlags.Additive : x.Name.StartsWith('{') ? TextureFlags.Masked : 0,
						Height = texFile.Height,
						Width = texFile.Width,
						Index = 0x0
					});
				}
			}
			));
			textures1 = textures1.Where(t => !_filterTextures.Contains(t.Header.Name)).ToArray();
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
			var filteredMeshes = faces
				.GroupBy(f => f.Texture.Name)
				.Where(g => !_filterTextures.Contains(g.First().Texture.Name.ToLower()));
			var allMeshVertices = filteredMeshes.SelectMany(g => g.SelectMany(f => f.Vertices)).Distinct();
			meshVertices = meshVertices.Where(v => allMeshVertices.Contains(v.Vertex)).ToList();

			var meshes = filteredMeshes.Select(g => new Mesh
			{
				Header = new MeshHeader
				{
					NormalIndex = 0x0,
					NumNormals = g.Sum(f => f.Vertices.Count),
					NumTriangles = g.Sum(f => f.Vertices.Count - 2), //TODO: count right (x2 is for quads)
					SkinRef = Array.IndexOf(textures1, textures1.First(t => t.Header.Name == g.First().Texture.Name)),
					TriangleIndex = 0x0
				},
				Vertices = g.SelectMany(f => f.Vertices).Distinct().Select(v => meshVertices.FirstOrDefault(mv => mv.Vertex == v)).ToArray(),
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
			matrix = Matrix4x4.CreateTranslation(bb.Center);
			transaction = new Transaction(new Transform(matrix, selection.GetSelectedParents()));
			transaction.Add(new TransformTexturesUniform(matrix, selection.GetSelectedParents().SelectMany(p => p.FindAll())));
			await MapDocumentOperation.Perform(document, transaction);
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
			model.Write(path);

			return;
		}

		public IEnumerable<SettingKey> GetKeys()
		{
			yield break;
		}

		public void LoadValues(ISettingsStore store)
		{
			lastPath = store.Get<string>("lastPath");
		}

		public void StoreValues(ISettingsStore store)
		{
			store.Set<string>("lastPath", lastPath);
		}
	}

}
