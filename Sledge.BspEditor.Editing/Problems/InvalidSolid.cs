using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Editing.Problems
{
	[Export(typeof(IProblemCheck))]
	[AutoTranslate]
	public class InvalidSolid : IProblemCheck
	{
		public string Name { get; set; }
		public string Details { get; set; }
		public Uri Url => null;
		public bool CanFix => true;

		public Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
		{
			var solids = document.Map.Root.FindAll()
				.Where(x => filter(x))
				.OfType<Solid>()
				.Where(x => !x.IsValid())
				.Select(x => new Problem().Add(x))
				.ToList();

			return Task.FromResult(solids);
		}

		public Task Fix(MapDocument document, Problem problem)
		{
			var fixTransaction = new Transaction();
			foreach (var g in problem.Objects.GroupBy(x => x.Hierarchy.Parent.ID))
			{
				fixTransaction.Add(new Detatch(g.Key, g));
				var newSolid = FixSolid(document, g.First() as Solid);
				if(newSolid.IsValid())
					fixTransaction.Add(new Attach(g.First().Hierarchy.Parent.ID, newSolid));
			}
			return MapDocumentOperation.Perform(document, fixTransaction);
		}
		private Solid FixSolid(MapDocument document, Solid solid)
		{
			var polyhedron = new Polyhedron(solid.GetPolygons().Select(p=>p.Plane));
			var newSolid = new Solid(document.Map.NumberGenerator.Next("MapObject"));

			newSolid.Data.AddRange(solid.Data.Where(data => data is not Face));
			foreach (var poly in polyhedron.Polygons)
			{
				var oldFace = solid.Faces.FirstOrDefault(x => x.Plane.EquivalentTo(poly.Plane, 0.0075f));
				var face = new Face(document.Map.NumberGenerator.Next("Face"))
				{
					Plane = poly.Plane,
					Texture = oldFace.Texture,
				};
				face.Vertices.AddRange(poly.Vertices);
				newSolid.Data.Add(face);
			}
			newSolid.DescendantsChanged();

			return newSolid;
		}
	}
}