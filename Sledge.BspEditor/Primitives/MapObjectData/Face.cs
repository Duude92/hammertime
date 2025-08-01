﻿using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Vortice.Mathematics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
	public class Face : IMapObjectData, ITransformable, ITextured
	{
		public long ID { get; }
		public Texture Texture { get; set; }
		public VertexCollection Vertices { get; }
		public Veldrid.Texture LightMap { get; set; }
		/// <summary>
		/// Used for lightmap UVs
		/// </summary>
		public Vector2[] Uv1 { get; set; }

		public Plane Plane
		{
			get => Vertices.Plane;
			set => Vertices.Plane = value;
		}

		public Vector3 Origin => Vertices.Origin;

		public Face(long id)
		{
			ID = id;
			Texture = new Texture();
			Vertices = new VertexCollection();
		}

		public Face(SerialisedObject obj)
		{
			ID = obj.Get("ID", ID);

			var t = obj.Children.FirstOrDefault(x => x.Name == "Texture");
			Texture = new Texture();

			if (t != null)
			{
				Texture.Name = t.Get("Name", "");
				Texture.Rotation = t.Get("Rotation", 0f);
				Texture.UAxis = t.Get("UAxis", -Vector3.UnitZ);
				Texture.VAxis = t.Get("VAxis", Vector3.UnitX);
				Texture.XScale = t.Get("XScale", 1f);
				Texture.XShift = t.Get("XShift", 0f);
				Texture.YScale = t.Get("YScale", 1f);
				Texture.YShift = t.Get("YShift", 0f);
			}

			Vertices = new VertexCollection();
			Vertices.AddRange(obj.Children.Where(x => x.Name == "Vertex").Select(x => x.Get<Vector3>("Position")));
		}

		[Export(typeof(IMapElementFormatter))]
		public class ActiveTextureFormatter : StandardMapElementFormatter<Face> { }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ID", ID);
			info.AddValue("Plane", Plane);
			info.AddValue("Texture", Texture);
			info.AddValue("Vertices", Vertices.ToArray());
		}

		private void CopyBase(Face face)
		{
			face.Texture = Texture.Clone();
			face.Vertices.Reset(Vertices.Select(x => x));
		}

		public IMapElement Clone()
		{
			var face = new Face(ID);
			CopyBase(face);
			return face;
		}

		public IMapElement Copy(UniqueNumberGenerator numberGenerator)
		{
			var face = new Face(numberGenerator.Next("Face"));
			CopyBase(face);
			return face;
		}

		public SerialisedObject ToSerialisedObject()
		{
			var so = new SerialisedObject("Face");
			so.Set("ID", ID);

			var p = new SerialisedObject("Plane");
			p.Set("Normal", Plane.Normal);
			p.Set("DistanceFromOrigin", Plane.DistanceFromOrigin);
			so.Children.Add(p);

			if (Texture != null)
			{
				var t = new SerialisedObject("Texture");
				t.Set("Name", Texture.Name);
				t.Set("Rotation", Texture.Rotation);
				t.Set("UAxis", Texture.UAxis);
				t.Set("VAxis", Texture.VAxis);
				t.Set("XScale", Texture.XScale);
				t.Set("XShift", Texture.XShift);
				t.Set("YScale", Texture.YScale);
				t.Set("YShift", Texture.YShift);
				so.Children.Add(t);
			}
			foreach (var c in Vertices)
			{
				var v = new SerialisedObject("Vertex");
				v.Set("Position", c);
				so.Children.Add(v);
			}
			return so;
		}
		/// <summary>
		/// Projects relative value [0..1] onto surface world coordinates
		/// </summary>
		/// <param name="x">Horizontal value</param>
		/// <param name="y">Vertical value</param>
		/// <returns>World position of projected relative coordinates onto surface</returns>
		public Vector3 ProjectedUVtoWorld(float uWorld, float vWorld)
		{

			Vector3 worldPos = Vertices[0]
							 + Texture.UAxis * uWorld * Texture.YScale
							 + Texture.VAxis * vWorld * Texture.XScale;

			if (Plane.C != 0) // If is not verticall (wall)
				worldPos.Z = -(Plane.A * worldPos.X + Plane.B * worldPos.Y + Plane.D) / Plane.C;

			return worldPos;
		}
		/// <summary>
		/// Compute surface texture size with set PPU, from polygon size
		/// </summary>
		/// <param name="pixelsPerUnit">How much pixels per unit should texture include</param>
		/// <returns>Size of surface texture</returns>
		public Size GetTextureResolution(float pixelsPerUnit)
		{
			var normal = Plane.Normal;

			Vector3 temp = Vector3.UnitX;
			if (Math.Abs(Vector3.Dot(temp, normal)) > 0.99f)
				temp = Vector3.UnitY;

			Vector3 uAxis = Vector3.Normalize(Vector3.Cross(normal, temp));
			Vector3 vAxis = Vector3.Normalize(Vector3.Cross(normal, uAxis));

			float minU = float.MaxValue, maxU = float.MinValue;
			float minV = float.MaxValue, maxV = float.MinValue;

			foreach (var vertex in Vertices)
			{
				float u = Vector3.Dot(vertex, Texture.UAxis) / Texture.YScale;
				float v = Vector3.Dot(vertex, Texture.VAxis) / Texture.XScale;

				minU = Math.Min(minU, u);
				maxU = Math.Max(maxU, u);
				minV = Math.Min(minV, v);
				maxV = Math.Max(maxV, v);
			}

			float worldWidth = maxU - minU;
			float worldHeight = maxV - minV;

			int rawWidth = (int)MathF.Ceiling(worldWidth * pixelsPerUnit);
			int rawHeight = (int)MathF.Ceiling(worldHeight * pixelsPerUnit);

			var width = (int)BitOperations.RoundUpToPowerOf2((uint)Math.Max(1, rawWidth));
			var height = (int)BitOperations.RoundUpToPowerOf2((uint)Math.Max(1, rawHeight));
			var size = new Size(width, height);
			return size;
		}

		public virtual IEnumerable<Tuple<Vector3, float, float>> GetTextureCoordinates(int width, int height)
		{
			if (width <= 0 || height <= 0 || Texture.XScale == 0 || Texture.YScale == 0)
			{
				return Vertices.Select(x => Tuple.Create(x, 0f, 0f));
			}

			var udiv = width * Texture.XScale;
			var uadd = Texture.XShift / width;
			var vdiv = height * Texture.YScale;
			var vadd = Texture.YShift / height;

			return Vertices.Select(x => Tuple.Create(x, x.Dot(Texture.UAxis) / udiv + uadd, x.Dot(Texture.VAxis) / vdiv + vadd));
		}

		public void Transform(Matrix4x4 matrix)
		{
			Vertices.Transform(x => Vector3.Transform(x, matrix));
		}
		public Vector3? GetIntersectionPoint(Line line, bool ignoreDirection = false)
		{
			if (Vertices.Count < 3) return null;

			var plane = Plane;
			var isect = plane.GetIntersectionPoint(line, ignoreDirection);
			if (isect == null) return null;

			var intersect = isect.Value;

			var vectors = Vertices;

			// http://paulbourke.net/geometry/insidepoly/

			// The angle sum will be 2 * PI if the point is inside the face
			double sum = 0;
			for (var i = 0; i < vectors.Count; i++)
			{
				var i1 = i;
				var i2 = (i + 1) % vectors.Count;

				// Translate the vertices so that the intersect point is on the origin
				var v1 = vectors[i1] - intersect;
				var v2 = vectors[i2] - intersect;

				var m1 = v1.Length();
				var m2 = v2.Length();
				var nom = m1 * m2;
				if (nom < 0.001f)
				{
					// intersection is at a vertex
					return intersect;
				}
				sum += Math.Acos(v1.Dot(v2) / nom);
			}

			var delta = Math.Abs(sum - Math.PI * 2);
			return (delta < 0.001d) ? intersect : (Vector3?)null;
		}
		public Polygon ToPolygon()
		{
			return new Polygon(Vertices);
		}

		public virtual IEnumerable<Line> GetEdges()
		{
			for (var i = 0; i < Vertices.Count; i++)
			{
				yield return new Line(Vertices[i], Vertices[(i + 1) % Vertices.Count]);
			}
		}

		public class VertexCollection : IList<Vector3>
		{
			private readonly List<Vector3> _list = new List<Vector3>();
			private Plane _plane = new Plane(Vector3.UnitZ, Vector3.Zero);

			internal Plane Plane
			{
				get => _plane;
				set
				{
					if (_list.Count < 3) _plane = value;
				}
			}

			public int Count => _list.Count;
			public bool IsReadOnly => false;

			internal Vector3 Origin => !_list.Any()
				? Vector3.Zero
				: _list.Aggregate(Vector3.Zero, (a, b) => a + b) / _list.Count;

			public Vector3 this[int index]
			{
				get => _list[index];
				set
				{
					_list[index] = value;
					UpdatePlane();
				}
			}

			public void Add(Vector3 item)
			{
				_list.Add(item);
				UpdatePlane();
			}

			public void AddRange(IEnumerable<Vector3> items)
			{
				_list.AddRange(items);
				UpdatePlane();
			}

			public void Flip()
			{
				_list.Reverse();
				UpdatePlane();
			}

			public void Clear()
			{
				_list.Clear();
				UpdatePlane();
			}

			public bool Remove(Vector3 item)
			{
				var r = _list.Remove(item);
				UpdatePlane();
				return r;
			}

			public void Insert(int index, Vector3 item)
			{
				_list.Insert(index, item);
				UpdatePlane();
			}

			public void RemoveAt(int index)
			{
				_list.RemoveAt(index);
				UpdatePlane();
			}

			public void Transform(Func<Vector3, Vector3> tranform)
			{
				for (var i = 0; i < _list.Count; i++)
				{
					_list[i] = tranform(_list[i]);
				}
				UpdatePlane();
			}

			public void Reset(IEnumerable<Vector3> values)
			{
				_list.Clear();
				_list.AddRange(values);
				UpdatePlane();
			}

			public void UpdatePlane()
			{
				_plane = _list.Count < 3 ? _plane : new Plane(_list[0], _list[1], _list[2]);
			}

			public IEnumerator<Vector3> GetEnumerator() => _list.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
			public bool Contains(Vector3 item) => _list.Contains(item);
			public void CopyTo(Vector3[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
			public int IndexOf(Vector3 item) => _list.IndexOf(item);
			public void ForEach(Action<Vector3> action) => _list.ForEach(action);
		}
	}
}