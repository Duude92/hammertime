using Sledge.DataStructures.Geometric;
using Sledge.Formats.Model.Source;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Viewports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Sledge.Providers.Model.Mdl44
{
	public class MdlModel : IModel
	{
		public MdlFile Model { get; }

		private Guid _guid;
		private Rendering.Resources.Buffer _buffer;
		private VertexModel3[] _vertices;
		private uint[] _indices;
		private Rendering.Resources.Texture[] _textureResources;
		private string[] _materials;
		private uint _numWireframeIndices;
		private uint _numTexturedIndices;
		private List<(int vertexStart, int vertexCount, int materialNum)> Meshes = new();

		public MdlModel(MdlFile model)
		{
			Model = model;
			_guid = Guid.NewGuid();
		}

		public void CreateResources(EngineInterface engine, RenderContext context)
		{
			_buffer = engine.CreateBuffer();
			_vertices = Model.GetVertices().Select(v => new VertexModel3
			{
				Position = v.Vertex,
				Normal = v.Normal,
				Texture = new Vector3(v.Texture, 0),
				Bone = (uint)v.VertexBone,
				Flags = VertexFlags.None
			}).ToArray();

			var wireframeIndices = new List<uint>();
			var indicesList = new List<ushort>();
			var indicesCount = 0;

			for (var meshIndex = 0; meshIndex < Model.GetMeshCount(0, 0); meshIndex++)
			{
				var indices = Model.GetIndices(meshIndex);
				indicesList.AddRange(indices);
				var thisIndicesCount = indices.Length;
				Meshes.Add((indicesCount, thisIndicesCount, Model.GetMaterialIndex(meshIndex, 0, 0)));
				indicesCount += thisIndicesCount;
			}

			var initIndices = indicesList.ToArray();

			for (int i = 0; i < initIndices.Length; i++)
			{
				var vi = initIndices[i];
				wireframeIndices.Add(vi);
				//wireframeIndices.Add((uint)(i % 3 == 2 ? vi - 2 : vi + 1));
			}

			_indices = new uint[initIndices.Length + wireframeIndices.Count];
			Array.Copy(initIndices, 0, _indices, 0, initIndices.Length);
			Array.Copy(wireframeIndices.ToArray(), 0, _indices, initIndices.Length, wireframeIndices.Count);
			_numWireframeIndices = (uint)wireframeIndices.Count;
			_numTexturedIndices = (uint)initIndices.Length;
			_buffer.Update(_vertices, _indices);
			_materials = Model.Materials.Select(m => Path.Combine(Model.MaterialDirectory, m ?? "").Replace('\\', '/')).ToArray(); // Ensure forward slashes for consistency
		}

		public string[] GetTextureName()
		{
			return _materials;
		}
		public void SetTexture(Rendering.Resources.Texture[] textures)
		{
			_textureResources = textures;
		}

		public (Vector3, Vector3) GetBoundingBox(int sequence, int frame, float subframe)
		{
			var xxx = Model.Bones[0].Data.quat;
			var x = xxx.X;
			xxx.X = xxx.Z;
			xxx.Z = x;
			var tr1 = Matrix4x4.CreateFromQuaternion(xxx);
			var transforms = new Matrix4x4[1] { tr1 };

			var list =
	from part in Model.VtxFile.BodyParts
	from mesh in part.Models[0].LOD[0].Meshes
	from stripG in mesh.StripGroups
	from strip in stripG.Strips
	from vertex in strip.Verts
	let transform = transforms[0]
	select Vector3.Transform(Model.VvdFile.Vertices[vertex.origMeshVertID].m_vecPosition, transform);

			//var list = Model.VtxFile.BodyParts[0].Models[0].LOD[0].Meshes[0].StripGroups[0].Strips[0].Verts.Select(x => Vector3.Transform( Model.VvdFile.Vertices[x.origMeshVertID].m_vecPosition, transforms[0])).ToList();


			//var list =
			//	from vertex in Model.VvdFile.Vertices
			//	select vertex.m_vecPosition;

			var box = new Box(list);
			return (box.Start, box.End); // FIXME: Placeholder, implement actual bounding box calculation
		}

		public void DestroyResources()
		{
			_buffer?.Dispose();
		}

		public void Dispose()
		{
			_buffer?.Dispose();
		}

		public List<string> GetSequences()
		{
			return new List<string>();
		}

		internal void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
		{
			_buffer.Bind(cl, 0);

			if (pipeline.Type == PipelineType.TexturedModel)
			{
				foreach (var mesh in Meshes)
				{
					if (mesh.materialNum < 0 || mesh.materialNum >= _textureResources.Length) continue;
					_textureResources[mesh.materialNum].BindTo(cl, 1);
					cl.DrawIndexed((uint)mesh.vertexCount, 1, (uint)(mesh.vertexStart), 0, 0);
				}
				//_textureResources[0].BindTo(cl, 1);
				//uint ci = 0;
				//cl.DrawIndexed(_numTexturedIndices, 1, 0, 0, 0);

				//foreach (var bpi in _bodyPartIndices)
				//{
				//	const int model = 0;
				//	for (var j = 0; j < bpi.Length; j++)
				//	{
				//		if (j == model) cl.DrawIndexed(bpi[j], 1, ci, 0, 0);
				//		ci += bpi[j];
				//	}
				//}
			}
			else if (pipeline.Type == PipelineType.WireframeModel)
			{
				//cl.DrawIndexed((uint)_indices.Length, 1, 0, 0, 0);

				cl.DrawIndexed(_numWireframeIndices, 1, _numTexturedIndices, 0, 0);
			}
		}


	}
}
