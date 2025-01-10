using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Extensions;
using Sledge.DataStructures;
using Sledge.FileSystem;
using static System.Collections.Specialized.BitVector32;


namespace Sledge.Providers.Model.Mdl10.Format
{
	public class MdlFile
	{
		private static readonly int MESH_HEADER_SIZE = Marshal.SizeOf<MeshHeader>();
		private static readonly int TRIVERT_SIZE = Marshal.SizeOf<Trivert>();


		public Header Header { get; set; }
		public List<Bone> Bones { get; set; }
		public List<BoneController> BoneControllers { get; set; }
		public List<Hitbox> Hitboxes { get; set; }
		public List<Sequence> Sequences { get; set; }
		public List<SequenceGroup> SequenceGroups { get; set; }
		public List<Texture> Textures { get; set; }
		public List<SkinFamily> Skins { get; set; }
		public List<BodyPart> BodyParts { get; set; }
		public List<Attachment> Attachments { get; set; }

		public MdlFile(IEnumerable<Stream> streams)
		{
			Bones = new List<Bone>();
			BoneControllers = new List<BoneController>();
			Hitboxes = new List<Hitbox>();
			Sequences = new List<Sequence>();
			SequenceGroups = new List<SequenceGroup>();
			Textures = new List<Texture>();
			Skins = new List<SkinFamily>();
			BodyParts = new List<BodyPart>();
			Attachments = new List<Attachment>();

			var readers = streams.Select(x => new BinaryReader(x, Encoding.ASCII)).ToList();
			try
			{
				Read(readers);
			}
			finally
			{
				readers.ForEach(x => x.Dispose());
			}
		}
		public MdlFile()
		{

		}

		public static MdlFile FromFile(string filename)
		{
			var dir = Path.GetDirectoryName(filename);
			var fname = Path.GetFileNameWithoutExtension(filename);

			var streams = new List<Stream>();
			try
			{
				streams.Add(File.OpenRead(filename));
				var tfile = Path.Combine(dir, fname + "t.mdl");
				if (File.Exists(tfile)) streams.Add(File.OpenRead(tfile));
				for (var i = 1; i < 32; i++)
				{
					var sfile = Path.Combine(dir, fname + i.ToString("00") + ".mdl");
					if (File.Exists(sfile)) streams.Add(File.OpenRead(sfile));
					else break;
				}

				return new MdlFile(streams);
			}
			finally
			{
				foreach (var s in streams) s.Dispose();
			}
		}

		public static MdlFile FromFile(IFile file)
		{
			var dir = file.Parent;
			var fname = file.NameWithoutExtension;

			var streams = new List<Stream>();
			try
			{
				streams.Add(file.Open());
				var tfile = dir.GetFile(fname + "t.mdl");
				if (tfile?.Exists == true) streams.Add(tfile.Open());
				for (var i = 1; i < 32; i++)
				{
					var sfile = dir.GetFile(fname + i.ToString("00") + ".mdl");
					if (sfile?.Exists == true) streams.Add(sfile.Open());
					else break;
				}

				return new MdlFile(streams);
			}
			finally
			{
				foreach (var s in streams) s.Dispose();
			}
		}

		private static readonly HashSet<Version> KnownVersions = Enum.GetValues(typeof(Version)).OfType<Version>().ToHashSet();

		public static bool CanRead(IFile file)
		{
			if (!file.Exists || file.Extension != "mdl") return false;

			try
			{
				using (var s = file.Open())
				{
					using (var br = new BinaryReader(s))
					{
						var id = (ID)br.ReadInt32();
						var version = (Version)br.ReadInt32();
						return id == ID.Idst && KnownVersions.Contains(version);
					}
				}

			}
			catch
			{
				return false;
			}
		}
		public void Write(string filename)
		{
			using (var stream = new BinaryWriter(new StreamWriter(filename).BaseStream))
			{
				char[] name = new char[64];
				int numskinref = 2;
				int numskinfamilies = 1;

				var headstruSize = Marshal.SizeOf<Header>();
				var headstruBuffer = new byte[headstruSize];
				IntPtr headbuffer = Marshal.AllocHGlobal(headstruSize);
				Marshal.StructureToPtr(Header, headbuffer, false);
				Marshal.Copy(headbuffer, headstruBuffer, 0, headstruSize);
				Marshal.FreeHGlobal(headbuffer);
				stream.Write(headstruBuffer);


				//stream.Write((int)ID.Idst);
				//stream.Write((int)Version.Goldsource);
				//stream.Write(name);
				//stream.Write(0);// file size
				//stream.WriteVector3(Vector3.Zero); // eye position
				//stream.WriteVector3(Vector3.Zero); // hullmin
				//stream.WriteVector3(Vector3.Zero); //hullmax
				//stream.WriteVector3(Vector3.Zero); //bbmin
				//stream.WriteVector3(Vector3.Zero); //bbmax
				//stream.Write(0);// flags
				int bonesOffset = headstruSize + (11 * 2 * 4) + 2 * 4 + 2 * 4; //section count + offset by sizeof(int) + add tex field + add skin field 
				int boneControllerOffset = bonesOffset + Marshal.SizeOf<Bone>() * Bones.Count;
				int hitboxOffset = boneControllerOffset + Marshal.SizeOf<BoneController>() * BoneControllers.Count;
				int unknownOffset = hitboxOffset + 16;
				int sequencesOffset = unknownOffset + Marshal.SizeOf<Hitbox>() * Hitboxes.Count;
				int seqGroupOffset = sequencesOffset + Marshal.SizeOf<SequenceHeader>() * Sequences.Count;
				int bodypartOffset = seqGroupOffset + Marshal.SizeOf<SequenceGroup>() * SequenceGroups.Count;

				stream.Write(Bones.Count);// numbones
				stream.Write(bonesOffset);// boneoffset (0xf4 static mesh)
				stream.Write(BoneControllers.Count);// numbonecontrls
				stream.Write(boneControllerOffset);// bonecontrloffset 1.
				stream.Write(Hitboxes.Count);// numhitbox
				stream.Write(hitboxOffset);// hitboxoffset 2. (skip bonectloffset since bonectl is 0)
				stream.Write(Sequences.Count);// numseq
				stream.Write(sequencesOffset);// seqoffset (static mesh)
				stream.Write(SequenceGroups.Count);// numseqgrp
				stream.Write(seqGroupOffset);// seqgrpoffset
				stream.Write(Textures.Count);// numtextures
				var textureOffset = bonesOffset + Marshal.SizeOf<Texture>() * Textures.Count;
				stream.Write(textureOffset);// textureindex (offset)
				stream.Write(0);// texdataindex (offset)
				stream.Write(Skins[0].Textures.Length);// numskinref
				stream.Write(Skins.Count);// numskinfamilies
				stream.Write(0x0);// skinindex ??? (offset)
				stream.Write(BodyParts.Count);// numbodyparts
				stream.Write(bodypartOffset);// bodypartindex				
				bonesOffset += Marshal.SizeOf<BodyPartHeader>() * BodyParts.Count;
				stream.Write(Attachments.Count);// numattachments
				stream.Write(356);// attachmentindex
				stream.Write(0);// soundtable
				stream.Write(0);// soundindex
				stream.Write(0);// soundgroups
				stream.Write(0);// soundgroupindex
				stream.Write(0);// numtransitions
				stream.Write(bodypartOffset);// transitionindex



				// Bones
				for (var i = 0; i < Bones.Count; i++)
				{
					var struSize = Marshal.SizeOf<Bone>();
					var struBuffer = new byte[struSize];
					IntPtr buffer = Marshal.AllocHGlobal(struSize);
					Marshal.StructureToPtr(Bones[i], buffer, false);
					Marshal.Copy(buffer, struBuffer, 0, struSize);
					Marshal.FreeHGlobal(buffer);
					stream.Write(struBuffer);


				}
				//SKIP BONECONTROLLERS


				// Attachments
				for (var i = 0; i < 0; i++)
				{
					//var attachment = new Attachment
					//{
					//	Name = br.ReadFixedLengthString(Encoding.ASCII, 32),
					//	Type = br.ReadInt32(),
					//	Bone = br.ReadInt32(),
					//	Origin = br.ReadVector3(),
					//	Vectors = br.ReadVector3Array(3)
					//};
					//Attachments.Add(attachment
				}

				// Hitboxes
				for (var i = 0; i < Hitboxes.Count; i++)
				{
					var struSize = Marshal.SizeOf<Hitbox>();
					var struBuffer = new byte[struSize];
					IntPtr buffer = Marshal.AllocHGlobal(struSize);
					Marshal.StructureToPtr(Hitboxes[i], buffer, false);
					Marshal.Copy(buffer, struBuffer, 0, struSize);
					Marshal.FreeHGlobal(buffer);
					stream.Write(struBuffer);
				}
				var animationIndex = (int)stream.BaseStream.Position;
				byte[] animation = new byte[16]
				{
					0x00, 0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x0c ,0x00 ,0x01 ,0x01 ,0xff ,0x7f

				};
				stream.Write(animation);


				// Sequences
				for (var i = 0; i < Sequences.Count; i++)
				{
					var sequence = Sequences[i];
					sequence.Header.AnimationIndex = animationIndex;
					Sequences[i] = sequence;
					var struSize = Marshal.SizeOf<SequenceHeader>();
					byte[] struBuffer = new byte[struSize];

					IntPtr buffer = Marshal.AllocHGlobal(struSize);
					Marshal.StructureToPtr(Sequences[i].Header, buffer, false);
					Marshal.Copy(buffer, struBuffer, 0, struSize);
					Marshal.FreeHGlobal(buffer);
					stream.Write(struBuffer);

				}

				// Sequence groups
				for (var i = 0; i < SequenceGroups.Count; i++)
				{
					var struSize = Marshal.SizeOf<SequenceGroup>();
					var struBuffer = new byte[struSize];
					IntPtr buffer = Marshal.AllocHGlobal(struSize);
					Marshal.StructureToPtr(SequenceGroups[i], buffer, false);
					Marshal.Copy(buffer, struBuffer, 0, struSize);
					Marshal.FreeHGlobal(buffer);
					stream.Write(struBuffer);
				}

				var bodyPartDataOffset = bodypartOffset + Marshal.SizeOf<BodyPartHeader>() * BodyParts.Count;

				// Body parts
				for (var i = 0; i < BodyParts.Count; i++)
				{
					var bodyPart = BodyParts[i];
					bodyPart.Header.ModelIndex = bodyPartDataOffset;

					var struSize = Marshal.SizeOf<BodyPartHeader>();
					bodyPartDataOffset += Marshal.SizeOf<ModelHeader>();
					var struBuffer = new byte[struSize];
					IntPtr buffer = Marshal.AllocHGlobal(struSize);
					Marshal.StructureToPtr(bodyPart.Header, buffer, false);
					Marshal.Copy(buffer, struBuffer, 0, struSize);
					Marshal.FreeHGlobal(buffer);
					stream.Write(struBuffer);

				}

				var modelsStartOffset = bodyPartDataOffset;

				var vertInfoOffset = modelsStartOffset;
				var v3Size = Marshal.SizeOf<Vector3>();

				var (verticesoffset, verticesData) = GetVerticesInfo();
				foreach (var part in BodyParts)
				{
					for (int i = 0; i < part.Models.Length; i++)
					{
						var localModel = part.Models[i];
						var normalCount = localModel.Meshes.Sum(m => m.Header.NumTriangles);
						localModel.Header.VertInfoIndex = vertInfoOffset;
						localModel.Header.NormalInfoIndex = vertInfoOffset + verticesoffset[0];
						localModel.Header.VertIndex = vertInfoOffset + verticesoffset[1];
						localModel.Header.NormalIndex = vertInfoOffset + verticesoffset[2];
						localModel.Header.MeshIndex = vertInfoOffset + verticesoffset[3];
						localModel.Header.NumNormals = normalCount * 3;
						part.Models[i] = localModel;

						var mdlStruSize = Marshal.SizeOf<ModelHeader>();
						var mdlStruBuffer = new byte[mdlStruSize];
						IntPtr mdlbuffer = Marshal.AllocHGlobal(mdlStruSize);
						Marshal.StructureToPtr(localModel.Header, mdlbuffer, false);
						Marshal.Copy(mdlbuffer, mdlStruBuffer, 0, mdlStruSize);
						Marshal.FreeHGlobal(mdlbuffer);
						stream.Write(mdlStruBuffer);
					}
				}

				stream.Write(verticesData);

				var (meshDataOffsets, meshData) = GetMeshesData();


				foreach (var part in BodyParts)
				{
					foreach (var model in part.Models)
					{
						var triangleIndex = model.Header.MeshIndex + MESH_HEADER_SIZE * model.Header.NumMesh;
						var meshesOffset = (int)stream.BaseStream.Position + MESH_HEADER_SIZE * model.Header.NumMesh;
						for (int i = 0; i < model.Header.NumMesh; i++)
						{
							var mesh = model.Meshes[i];
							mesh.Header.TriangleIndex = meshesOffset + meshDataOffsets[i];
							var struBuffer = new byte[MESH_HEADER_SIZE];
							IntPtr buffer = Marshal.AllocHGlobal(MESH_HEADER_SIZE);
							Marshal.StructureToPtr(mesh.Header, buffer, false);
							Marshal.Copy(buffer, struBuffer, 0, MESH_HEADER_SIZE);
							Marshal.FreeHGlobal(buffer);
							stream.Write(struBuffer);
						}

					}
				}
				stream.Write(meshData);
				stream.Write(new byte[2]); //align

				var (texturesOffset, textureData) = GetTexturesData();


				// Textures
				int textureIndex = (int)stream.BaseStream.Position;
				for (var i = 0; i < Textures.Count; i++)
				{
					var tex = Textures[i];
					var struSize = Marshal.SizeOf<TextureHeader>();
					tex.Header.Index = ((textureIndex + struSize * Textures.Count) + Skins.Count * Skins[0].Textures.Length * sizeof(short)) + texturesOffset[i];
					tex.Header.Name += ".bmp";
					Textures[i] = tex;
					var struBuffer = new byte[struSize];
					IntPtr buffer = Marshal.AllocHGlobal(struSize);
					Marshal.StructureToPtr(Textures[i].Header, buffer, false);
					Marshal.Copy(buffer, struBuffer, 0, struSize);
					Marshal.FreeHGlobal(buffer);
					stream.Write(struBuffer);

				}

				var skins = new byte[Skins.Count * Skins[0].Textures.Length * sizeof(short)];
				var skinIndex = (int)stream.BaseStream.Position;
				foreach (var skinFamily in Skins)
				{
					foreach (var skin in skinFamily.Textures)
					{
						stream.Write(skin);
					}
				}


				int textureDataIndex = (int)stream.BaseStream.Position;
				// Texture data
				stream.Write(textureData);

				stream.Seek(headstruSize + 11 * 4, SeekOrigin.Begin);
				stream.Write(textureIndex);
				stream.Write(textureDataIndex);
				stream.Seek(8, SeekOrigin.Current);
				stream.Write(skinIndex);

				stream.Seek(72, SeekOrigin.Begin);
				stream.Write(stream.BaseStream.Length);
			}

		}

		private (int[], byte[]) GetTexturesData()
		{
			using (var stream = new BinaryWriter(new MemoryStream()))
			{
				var offsets = new List<int>(Textures.Count);
				foreach (var texture in Textures)
				{
					offsets.Add((int)stream.BaseStream.Position);
					stream.Write(texture.Data);
					stream.Write(texture.Palette);

				}
				return (offsets.ToArray(), ((MemoryStream)stream.BaseStream).ToArray());
			}
		}

		private (int[],byte[]) GetVerticesInfo()
		{
			var offsets = new List<int>();
			using (var stream = new BinaryWriter(new MemoryStream()))
			{
				foreach (var part in BodyParts)
				{
					foreach (var model in part.Models)
					{
						var normalCount = model.Meshes.Sum(m => m.Header.NumTriangles) * 3;
						var vmdl = model.Meshes.SelectMany(mesh => mesh.Vertices);
						var modelVertices = model.Vertices;
						var vBones = modelVertices.Select(v => (byte)v.VertexBone).ToArray(); //reoder to every v, every n, every ve, no
						stream.Write(vBones);
						var bytestoadd = stream.BaseStream.Position % 4;
						stream.Write(new byte[bytestoadd]);

						offsets.Add((int)stream.BaseStream.Position);
						//var nBones = modelVertices.Select(v => (byte)v.NormalBone).ToArray();
						var nBones = new byte[normalCount];
						stream.Write(nBones);
						bytestoadd = stream.BaseStream.Position % 4;
						stream.Write(new byte[bytestoadd]);

						offsets.Add((int)stream.BaseStream.Position);
						var vertices = modelVertices.Select(v => v.Vertex).ToArray();
						foreach (var vertex in vertices)
							stream.WriteVector3(vertex);
						bytestoadd = stream.BaseStream.Position % 4;
						stream.Write(new byte[bytestoadd]);

						offsets.Add((int)stream.BaseStream.Position);
						//var normals = modelVertices.Select(v => v.Normal).ToArray();
						var normals = new Vector3[normalCount];
						foreach (var normal in normals)
							stream.WriteVector3(normal);
						bytestoadd = stream.BaseStream.Position % 4;
						stream.Write(new byte[bytestoadd]);
						offsets.Add((int)stream.BaseStream.Position);

					}
				}
				return (offsets.ToArray(),((MemoryStream)stream.BaseStream).ToArray());
			}
		}

		private (int[], byte[]) GetMeshesData()
		{
			using (var memo = new BinaryWriter(new MemoryStream()))
			{
				var offsets = new List<int>();
				foreach (var part in BodyParts)
				{
					foreach (var model in part.Models)
					{
						foreach (var mesh in model.Meshes)
						{
							offsets.Add((int)memo.BaseStream.Position);
							foreach (var sequence in mesh.Sequences)
							{
								memo.Write(sequence.TriCountDir);
								foreach (var trivert in sequence.TriVerts)
								{
									var struBuffer = new byte[TRIVERT_SIZE];
									IntPtr buffer = Marshal.AllocHGlobal(TRIVERT_SIZE);
									Marshal.StructureToPtr(trivert, buffer, false);
									Marshal.Copy(buffer, struBuffer, 0, TRIVERT_SIZE);
									Marshal.FreeHGlobal(buffer);
									memo.Write(struBuffer);
								}
							}
							var bytestoadd = memo.BaseStream.Position % 4;
							memo.Write(new byte[bytestoadd]);
						}
					}
				}
				var bytes = ((MemoryStream)memo.BaseStream).ToArray();


				return (offsets.ToArray(), bytes);
			}
		}

		private void Read(IEnumerable<BinaryReader> readers)
		{
			var main = new List<BinaryReader>();
			var sequenceGroups = new Dictionary<string, BinaryReader>();

			foreach (var br in readers)
			{
				var id = (ID)br.ReadInt32();
				var version = (Version)br.ReadInt32();

				if (version != Version.Goldsource)
				{
					throw new NotSupportedException("Only Goldsource (v10) MDL files are supported.");
				}

				if (id != ID.Idsq && id != ID.Idst)
				{
					throw new NotSupportedException("Only Goldsource (v10) MDL files are supported.");
				}

				if (id == ID.Idst)
				{
					main.Add(br);
				}
				else
				{
					var name = br.ReadFixedLengthString(Encoding.ASCII, 64);
					sequenceGroups[name] = br;
				}
			}

			foreach (var br in main)
			{
				Read(br, sequenceGroups);
			}
		}

		#region Reading

		private void Read(BinaryReader br, Dictionary<string, BinaryReader> sequenceGroups)
		{
			Header = new Header
			{
				ID = ID.Idst,
				Version = Version.Goldsource,
				Name = br.ReadFixedLengthString(Encoding.ASCII, 64),
				Size = br.ReadInt32(),
				EyePosition = br.ReadVector3(),
				HullMin = br.ReadVector3(),
				HullMax = br.ReadVector3(),
				BoundingBoxMin = br.ReadVector3(),
				BoundingBoxMax = br.ReadVector3(),
				Flags = br.ReadInt32()
			};

			// Read all the nums/offsets from the header
			var sections = new int[(int)Section.NumSections][];
			for (var i = 0; i < (int)Section.NumSections; i++)
			{
				var sec = (Section)i;

				int indexNum;
				if (sec == Section.Texture || sec == Section.Skin) indexNum = 3;
				else indexNum = 2;

				sections[i] = new int[indexNum];
				for (var j = 0; j < indexNum; j++)
				{
					sections[i][j] = br.ReadInt32();
				}
			}

			// Bones
			var num = SeekToSection(br, Section.Bone, sections);
			var numBones = num;
			for (var i = 0; i < num; i++)
			{
				var bone = new Bone
				{
					Name = br.ReadFixedLengthString(Encoding.ASCII, 32),
					Parent = br.ReadInt32(),
					Flags = br.ReadInt32(),
					Controllers = br.ReadIntArray(6),
					Position = br.ReadVector3(),
					Rotation = br.ReadVector3(),
					PositionScale = br.ReadVector3(),
					RotationScale = br.ReadVector3()
				};
				Bones.Add(bone);
			}

			// Bone controllers
			num = SeekToSection(br, Section.BoneController, sections);
			for (var i = 0; i < num; i++)
			{
				var boneController = new BoneController
				{
					Bone = br.ReadInt32(),
					Type = br.ReadInt32(),
					Start = br.ReadSingle(),
					End = br.ReadSingle(),
					Rest = br.ReadInt32(),
					Index = br.ReadInt32()
				};
				BoneControllers.Add(boneController);
			}

			// Hitboxes
			num = SeekToSection(br, Section.Hitbox, sections);
			for (var i = 0; i < num; i++)
			{
				var hitbox = new Hitbox
				{
					Bone = br.ReadInt32(),
					Group = br.ReadInt32(),
					Min = br.ReadVector3(),
					Max = br.ReadVector3()
				};
				Hitboxes.Add(hitbox);
			}

			// Sequence groups
			num = SeekToSection(br, Section.SequenceGroup, sections);
			for (var i = 0; i < num; i++)
			{
				var group = new SequenceGroup
				{
					Label = br.ReadFixedLengthString(Encoding.ASCII, 32),
					Name = br.ReadFixedLengthString(Encoding.ASCII, 64)
				};
				br.ReadBytes(8); // unused
				SequenceGroups.Add(group);
			}

			// Sequences
			num = SeekToSection(br, Section.Sequence, sections);
			for (var i = 0; i < num; i++)
			{
				var sequence = new Sequence
				{
					Header = new SequenceHeader
					{
						Name = br.ReadFixedLengthString(Encoding.ASCII, 32),
						Framerate = br.ReadSingle(),
						Flags = br.ReadInt32(),
						Activity = br.ReadInt32(),
						ActivityWeight = br.ReadInt32(),
						NumEvents = br.ReadInt32(),
						EventIndex = br.ReadInt32(),
						NumFrames = br.ReadInt32(),
						NumPivots = br.ReadInt32(),
						PivotIndex = br.ReadInt32(),
						MotionType = br.ReadInt32(),
						MotionBone = br.ReadInt32(),
						LinearMovement = br.ReadVector3(),
						AutoMovePositionIndex = br.ReadInt32(),
						AutoMoveAngleIndex = br.ReadInt32(),
						Min = br.ReadVector3(),
						Max = br.ReadVector3(),
						NumBlends = br.ReadInt32(),
						AnimationIndex = br.ReadInt32(),
						BlendType = br.ReadIntArray(2),
						BlendStart = br.ReadSingleArray(2),
						BlendEnd = br.ReadSingleArray(2),
						BlendParent = br.ReadInt32(),
						SequenceGroup = br.ReadInt32(),
						EntryNode = br.ReadInt32(),
						ExitNode = br.ReadInt32(),
						NodeFlags = br.ReadInt32(),
						NextSequence = br.ReadInt32()
					}
				};

				var seqGroup = SequenceGroups[sequence.Header.SequenceGroup];

				// Only load seqence group 0 for now (others are in other files)
				if (sequence.Header.SequenceGroup == 0)
				{
					var pos = br.BaseStream.Position;
					sequence.Blends = LoadAnimationBlends(br, sequence, numBones);
					br.BaseStream.Position = pos;
				}
				else if (sequenceGroups.ContainsKey(seqGroup.Name))
				{
					var reader = sequenceGroups[seqGroup.Name];
					sequence.Blends = LoadAnimationBlends(reader, sequence, numBones);
				}

				Sequences.Add(sequence);
			}

			// Textures
			num = SeekToSection(br, Section.Texture, sections);
			var firstTextureIndex = Textures.Count;
			for (var i = 0; i < num; i++)
			{
				var texture = new Texture
				{
					Header = new TextureHeader
					{
						Name = br.ReadFixedLengthString(Encoding.ASCII, 64),
						Flags = (TextureFlags)br.ReadInt32(),
						Width = br.ReadInt32(),
						Height = br.ReadInt32(),
						Index = br.ReadInt32()
					}
				};
				Textures.Add(texture);
			}

			// Texture data
			for (var i = firstTextureIndex; i < firstTextureIndex + num; i++)
			{
				var t = Textures[i];
				br.BaseStream.Position = t.Header.Index;
				t.Data = br.ReadBytes(t.Header.Width * t.Header.Height);
				t.Palette = br.ReadBytes(256 * 3);
				Textures[i] = t;
			}

			// Skins
			var skinSection = sections[(int)Section.Skin];
			var numSkinRefs = skinSection[0];
			var numSkinFamilies = skinSection[1];
			br.BaseStream.Seek(skinSection[2], SeekOrigin.Begin);
			for (var i = 0; i < numSkinFamilies; i++)
			{
				var skin = new SkinFamily
				{
					Textures = br.ReadShortArray(numSkinRefs)
				};
				Skins.Add(skin);
			}

			// Body parts
			num = SeekToSection(br, Section.BodyPart, sections);
			for (var i = 0; i < num; i++)
			{
				var part = new BodyPart
				{
					Header = new BodyPartHeader
					{
						Name = br.ReadFixedLengthString(Encoding.ASCII, 64),
						NumModels = br.ReadInt32(),
						Base = br.ReadInt32(),
						ModelIndex = br.ReadInt32()
					}
				};
				var pos = br.BaseStream.Position;
				part.Models = LoadModels(br, part);
				br.BaseStream.Position = pos;
				BodyParts.Add(part);
			}

			// Attachments
			num = SeekToSection(br, Section.Attachment, sections);
			for (var i = 0; i < num; i++)
			{
				var attachment = new Attachment
				{
					Name = br.ReadFixedLengthString(Encoding.ASCII, 32),
					Type = br.ReadInt32(),
					Bone = br.ReadInt32(),
					Origin = br.ReadVector3(),
					Vectors = br.ReadVector3Array(3)
				};
				Attachments.Add(attachment);
			}

			// Transitions

			// Sounds & Sound groups aren't used
		}

		private static int SeekToSection(BinaryReader br, Section section, int[][] sections)
		{
			var s = sections[(int)section];
			br.BaseStream.Seek(s[1], SeekOrigin.Begin);
			return s[0];
		}

		#endregion

		#region Animations

		private static Blend[] LoadAnimationBlends(BinaryReader br, Sequence sequence, int numBones)
		{
			var blends = new Blend[sequence.Header.NumBlends];
			var blendLength = 6 * numBones;

			br.BaseStream.Seek(sequence.Header.AnimationIndex, SeekOrigin.Begin);

			var animPosition = br.BaseStream.Position;
			var offsets = br.ReadUshortArray(blendLength * sequence.Header.NumBlends);
			for (var i = 0; i < sequence.Header.NumBlends; i++)
			{
				var blendOffsets = new ushort[blendLength];
				Array.Copy(offsets, blendLength * i, blendOffsets, 0, blendLength);

				var startPosition = animPosition + i * blendLength * 2;
				blends[i].Frames = LoadAnimationFrames(br, sequence, numBones, startPosition, blendOffsets);
			}

			return blends;
		}

		private static AnimationFrame[] LoadAnimationFrames(BinaryReader br, Sequence sequence, int numBones, long startPosition, ushort[] boneOffsets)
		{
			var frames = new AnimationFrame[sequence.Header.NumFrames];
			for (var i = 0; i < frames.Length; i++)
			{
				frames[i].Positions = new Vector3[numBones];
				frames[i].Rotations = new Vector3[numBones];
			}

			for (var i = 0; i < numBones; i++)
			{
				var boneValues = new short[6][];
				for (var j = 0; j < 6; j++)
				{
					var offset = boneOffsets[i * 6 + j];
					if (offset <= 0)
					{
						boneValues[j] = new short[sequence.Header.NumFrames];
						continue;
					}

					br.BaseStream.Seek(startPosition + i * 6 * 2 + offset, SeekOrigin.Begin);
					boneValues[j] = ReadAnimationFrameValues(br, sequence.Header.NumFrames);
				}

				for (var j = 0; j < sequence.Header.NumFrames; j++)
				{
					frames[j].Positions[i] = new Vector3(boneValues[0][j], boneValues[1][j], boneValues[2][j]);
					frames[j].Rotations[i] = new Vector3(boneValues[3][j], boneValues[4][j], boneValues[5][j]);
				}
			}

			return frames;
		}

		private static short[] ReadAnimationFrameValues(BinaryReader br, int count)
		{
			/*
             * RLE data:
             * byte compressed_length - compressed number of values in the data
             * byte uncompressed_length - uncompressed number of values in run
             * short values[compressed_length] - values in the run, the last value is repeated to reach the uncompressed length
             */
			var values = new short[count];

			for (var i = 0; i < count; /* i = i */)
			{
				var run = br.ReadBytes(2); // read the compressed and uncompressed lengths
				var vals = br.ReadShortArray(run[0]); // read the compressed data
				for (var j = 0; j < run[1] && i < count; i++, j++)
				{
					var idx = Math.Min(run[0] - 1, j); // value in the data or the last value if we're past the end
					values[i] = vals[idx];
				}
			}

			return values;
		}

		#endregion

		#region Models

		private static Model[] LoadModels(BinaryReader br, BodyPart part)
		{
			br.BaseStream.Seek(part.Header.ModelIndex, SeekOrigin.Begin);

			var models = new Model[part.Header.NumModels];
			for (var i = 0; i < part.Header.NumModels; i++)
			{
				var model = new Model
				{
					Header = new ModelHeader
					{
						Name = br.ReadFixedLengthString(Encoding.ASCII, 64),
						Type = br.ReadInt32(),
						Radius = br.ReadSingle(),
						NumMesh = br.ReadInt32(),
						MeshIndex = br.ReadInt32(),
						NumVerts = br.ReadInt32(),
						VertInfoIndex = br.ReadInt32(),
						VertIndex = br.ReadInt32(),
						NumNormals = br.ReadInt32(),
						NormalInfoIndex = br.ReadInt32(),
						NormalIndex = br.ReadInt32(),
						NumGroups = br.ReadInt32(),
						GroupIndex = br.ReadInt32()
					}
				};

				var pos = br.BaseStream.Position;
				model.Meshes = ReadMeshes(br, model);
				br.BaseStream.Position = pos;

				models[i] = model;
			}

			return models;
		}

		private static Mesh[] ReadMeshes(BinaryReader br, Model model)
		{
			var meshes = new Mesh[model.Header.NumMesh];

			// Read all the vertex data
			br.BaseStream.Position = model.Header.VertInfoIndex;
			var vertexBones = br.ReadBytes(model.Header.NumVerts);

			br.BaseStream.Position = model.Header.NormalInfoIndex;
			var normalBones = br.ReadBytes(model.Header.NumNormals);

			br.BaseStream.Position = model.Header.VertIndex;
			var vertices = br.ReadVector3Array(model.Header.NumVerts);

			br.BaseStream.Position = model.Header.NormalIndex;
			var normals = br.ReadVector3Array(model.Header.NumNormals);

			// Read the meshes
			br.BaseStream.Position = model.Header.MeshIndex;
			for (var i = 0; i < model.Header.NumMesh; i++)
			{
				var mesh = new Mesh
				{
					Header = new MeshHeader
					{
						NumTriangles = br.ReadInt32(),
						TriangleIndex = br.ReadInt32(),
						SkinRef = br.ReadInt32(),
						NumNormals = br.ReadInt32(),
						NormalIndex = br.ReadInt32()
					}
				};
				meshes[i] = mesh;
			}

			// Read the triangle data
			for (var i = 0; i < model.Header.NumMesh; i++)
			{
				meshes[i].Vertices = ReadTriangles(br, meshes[i], vertices, vertexBones, normals, normalBones);
			}

			return meshes;
		}

		private static MeshVertex[] ReadTriangles(BinaryReader br, Mesh mesh, Vector3[] vertices, byte[] vertexBones, Vector3[] normals, byte[] normalBones)
		{
			/*
             * Mesh data
             * short type - abs(type) is the length of the run
             *   - < 0 = triangle fan,
             *   - > 0 = triangle strip
             *   - 0 = end of list
             * short vertex - vertex index
             * short normal - normal index
             * short u, short v - texture coordinates
             */

			var meshVerts = new MeshVertex[mesh.Header.NumTriangles * 3];
			var vi = 0;

			br.BaseStream.Position = mesh.Header.TriangleIndex;

			short type;
			while ((type = br.ReadInt16()) != 0)
			{
				var fan = type < 0;
				var length = Math.Abs(type);
				var pointData = br.ReadShortArray(4 * length);
				for (var i = 0; i < length - 2; i++)
				{
					//                    | TRIANGLE FAN    |                       | TRIANGLE STRIP (ODD) |         | TRIANGLE STRIP (EVEN) |
					var add = fan ? new[] { 0, i + 1, i + 2 } : (i % 2 == 1 ? new[] { i + 1, i, i + 2 } : new[] { i, i + 1, i + 2 });
					foreach (var idx in add)
					{
						var vert = pointData[idx * 4 + 0];
						var norm = pointData[idx * 4 + 1];
						var s = pointData[idx * 4 + 2];
						var t = pointData[idx * 4 + 3];

						meshVerts[vi++] = new MeshVertex
						{
							VertexBone = vertexBones[vert],
							NormalBone = normalBones[norm],
							Vertex = vertices[vert],
							Normal = normals[norm],
							Texture = new Vector2(s, t)
						};
					}
				}
			}

			return meshVerts;
		}

		#endregion

		/// <summary>
		/// Get the transforms for the bones of this model
		/// </summary>
		/// <param name="sequence">The sequence id to use</param>
		/// <param name="frame">The frame number to use</param>
		/// <param name="subframe">The subframe between the given frame and the next frame, as a percentage between 0 and 1</param>
		/// <param name="transforms">The array of transforms to set values into. Must be at least the size of the <see cref="Bones"/> array.</param>
		public void GetTransforms(int sequence, int frame, float subframe, ref Matrix4x4[] transforms)
		{
			var seq = Sequences[sequence];
			var blend = seq.Blends[0];
			var cFrame = blend.Frames[frame % seq.Header.NumFrames];
			var nFrame = blend.Frames[(frame + 1) % seq.Header.NumFrames];

			var indivTransforms = new Matrix4x4[128];
			for (var i = 0; i < Bones.Count; i++)
			{
				var bone = Bones[i];
				var cPos = bone.Position + cFrame.Positions[i] * bone.PositionScale;
				var nPos = bone.Position + nFrame.Positions[i] * bone.PositionScale;
				var cRot = bone.Rotation + cFrame.Rotations[i] * bone.RotationScale;
				var nRot = bone.Rotation + nFrame.Rotations[i] * bone.RotationScale;

				var cQtn = Quaternion.CreateFromYawPitchRoll(cRot.X, cRot.Y, cRot.Z);
				var nQtn = Quaternion.CreateFromYawPitchRoll(nRot.X, nRot.Y, nRot.Z);

				// MDL angles have Y as the up direction
				cQtn = new Quaternion(cQtn.Y, cQtn.X, cQtn.Z, cQtn.W);
				nQtn = new Quaternion(nQtn.Y, nQtn.X, nQtn.Z, nQtn.W);

				var mat = Matrix4x4.CreateFromQuaternion(Quaternion.Slerp(cQtn, nQtn, subframe));
				mat.Translation = cPos * (1 - subframe) + nPos * subframe;

				indivTransforms[i] = mat;
			}

			for (var i = 0; i < Bones.Count; i++)
			{
				var mat = indivTransforms[i];
				var parent = Bones[i].Parent;
				while (parent >= 0)
				{
					var parMat = indivTransforms[parent];
					mat = mat * parMat;
					parent = Bones[parent].Parent;
				}
				transforms[i] = mat;
			}
		}

		/// <summary>
		/// Pre-calculate some bogus chrome values that look "ok" for a cheap effect.
		/// This method will modify the original vertices.
		/// </summary>
		public void WriteFakePrecalculatedChromeCoordinates()
		{
			var transforms = new Matrix4x4[Bones.Count];
			GetTransforms(0, 0, 0, ref transforms);
			for (var bp = 0; bp < BodyParts.Count; bp++)
			{
				var part = BodyParts[bp];
				for (var m = 0; m < part.Models.Length; m++)
				{
					var model = part.Models[m];
					for (var me = 0; me < model.Meshes.Length; me++)
					{
						var mesh = model.Meshes[me];
						var skin = Textures[mesh.Header.SkinRef];
						if (!skin.Header.Flags.HasFlag(TextureFlags.Chrome)) continue;

						for (var vi = 0; vi < mesh.Vertices.Length; vi++)
						{
							var v = mesh.Vertices[vi];
							var transform = transforms[v.VertexBone];

							// Borrowed from HLMV's StudioModel::Chrome function
							var tmp = Vector3.Normalize(transform.Translation);

							// Using unitx for the "player right" vector
							var up = Vector3.Normalize(Vector3.Cross(tmp, Vector3.UnitX));
							var right = Vector3.Normalize(Vector3.Cross(tmp, up));

							// HLMV is doing an inverse rotate (no translation),
							// so we set the shift values to zero after inverting
							var inv = Matrix4x4.Invert(transform, out var i) ? i : transform;
							inv.Translation = Vector3.Zero;
							up = Vector3.Transform(up, inv);
							right = Vector3.Transform(right, inv);

							BodyParts[bp].Models[m].Meshes[me].Vertices[vi].Texture = new Vector2(
								(Vector3.Dot(v.Normal, right) + 1) * 32,
								(Vector3.Dot(v.Normal, up) + 1) * 32
							);
						}
					}
				}
			}
		}

		private enum Section : int
		{
			Bone,
			BoneController,
			Hitbox,
			Sequence,
			SequenceGroup,
			Texture,
			Skin,
			BodyPart,
			Attachment,
			Sound,      // Unused
			SoundGroup, // Unused
			Transition,
			NumSections = 11
		}
	}
}
