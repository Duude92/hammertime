using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
	[Serializable]
	public class Path : BaseMapObject
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public PathDirection Direction { get; set; }
		public List<PathNode> Nodes { get; private set; }

		protected override string SerialisedName => "Path";

		public Path(long id) : base(id)
		{

			Nodes = new List<PathNode>();
		}

		public Path(SerialisedObject obj) : base(obj)
		{
			Name = obj.Get("Name", "");
			Type = obj.Get("Type", "");
			Direction = obj.Get("Direction", PathDirection.OneWay);

			var children = obj.Children.Where(x => x.Name == "Node");
			Nodes = children.Select(x => new PathNode(x)).ToList();
		}

		[Export(typeof(IMapElementFormatter))]
		public class EntityFormatter : StandardMapElementFormatter<Path> { }

		//protected Path(SerializationInfo info, StreamingContext context)
		//{
		//	Name = info.GetString("Name");
		//	Type = info.GetString("Type");
		//	Direction = (PathDirection)info.GetValue("Direction", typeof(PathDirection));
		//	Nodes = (List<PathNode>)info.GetValue("Nodes", typeof(List<PathNode>));
		//}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", Name);
			info.AddValue("Type", Type);
			info.AddValue("Direction", Direction);
			info.AddValue("Nodes", Nodes);
		}

		public override IMapElement Clone()
		{
			var clone = base.Clone() as Path;
			clone.Nodes = Nodes.Select(x => x.Clone()).ToList();
			return clone;
			//return new Path
			//{
			//	Name = Name,
			//	Type = Type,
			//	Direction = Direction,
			//	Nodes = Nodes.Select(x => x.Clone()).ToList()
			//};
		}

		public override IMapElement Copy(UniqueNumberGenerator numberGenerator)
		{
			return Clone();
		}

		public override SerialisedObject ToSerialisedObject()
		{
			var so = new SerialisedObject("Path");
			so.Set("Name", Name);
			so.Set("Type", Type);
			so.Set("Direction", Direction);
			so.Children.AddRange(Nodes.Select(x => x.ToSerialisedObject()));
			return so;
		}

		protected override Box GetBoundingBox()
		{
			return Box.Empty;
		}

		public override IEnumerable<Polygon> GetPolygons()
		{
			return new Polygon[0];
		}

		public override IEnumerable<IMapObject> Decompose(IEnumerable<Type> allowedTypes)
		{
			throw new NotImplementedException();
		}

		public enum PathDirection
		{
			OneWay,
			Circular,
			PingPong
		}

		[Serializable]
		public class PathNode : BaseMapObject, ISerializable
		{
			public Vector3 Position { get; set; }
			public int ID { get; set; }
			public string Name { get; set; }
			public Dictionary<string, string> Properties { get; private set; }

			protected override string SerialisedName => "PathNode";

			public PathNode(long id) : base(id)
			{
				Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			}

			public PathNode(SerialisedObject obj) : base(obj)
			{
				ID = obj.Get("ID", 0);
				Name = obj.Get("Name", "");
				Position = obj.Get<Vector3>("Position");

				Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
				foreach (var prop in obj.Properties)
				{
					if (prop.Key == "ID" || prop.Key == "Name" || prop.Key == "Position") continue;
					Properties[prop.Key] = prop.Value;
				}
			}

			//protected PathNode(SerializationInfo info, StreamingContext context)
			//{
			//	ID = info.GetInt32("ID");
			//	Name = info.GetString("Name");
			//	Position = (Vector3)info.GetValue("Position", typeof(Vector3));
			//	Properties = (Dictionary<string, string>)info.GetValue("Properties", typeof(Dictionary<string, string>));
			//}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("ID", ID);
				info.AddValue("Name", Name);
				info.AddValue("Position", Position);
				info.AddValue("Properties", Properties);
			}

			public override SerialisedObject ToSerialisedObject()
			{
				var so = new SerialisedObject("PathNode");

				foreach (var kv in Properties) so.Set(kv.Key, kv.Value);

				so.Set("ID", ID);
				so.Set("Name", Name);
				so.Set("Position", Position);

				return so;
			}

			public override PathNode Clone()
			{
				var clone = base.Clone() as PathNode;
				//var node = new PathNode
				//{
				//	Position = Position,
				//	ID = ID,
				//	Name = Name
				//};
				clone.Position = Position;
				clone.Name = Name;
				foreach (var kv in Properties) clone.Properties[kv.Key] = kv.Value;
				return clone;
			}

			protected override Box GetBoundingBox()
			{
				return new Box(Position - Vector3.One * 20, Position + Vector3.One * 20);
			}

			public override IEnumerable<Polygon> GetPolygons()
			{
				throw new NotImplementedException();
			}

			public override IEnumerable<IMapObject> Decompose(IEnumerable<Type> allowedTypes)
			{
				throw new NotImplementedException();
			}
		}
	}
}