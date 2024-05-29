using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Selection
{
	public class Select : IOperation
	{
		private readonly List<IMapObject> _idsToSelect;
		public bool Trivial => false;

		public Select(params IMapObject[] objectsToSelect)
		{
			_idsToSelect = objectsToSelect.Where(x => !x.IsSelected).ToList();
		}

		public Select(IEnumerable<IMapObject> objectsToSelect)
		{
			_idsToSelect = objectsToSelect.Where(x => !x.IsSelected).ToList();
		}

		public async Task<Change> Perform(MapDocument document)
		{
			var ch = new Change(document);
			var updateList = new List<IMapObject>();

			foreach (var id in _idsToSelect)
			{
				//var o = document.Map.Root.FindByID(id);
				var o = id;
				if (o != null)
				{
					o.IsSelected = true;
					updateList.Add(o);
				}
			}

			ch.UpdateRange(updateList);
			return ch;
		}

		public async Task<Change> Reverse(MapDocument document)
		{
			var ch = new Change(document);

			foreach (var id in _idsToSelect)
			{
				//var o = document.Map.Root.FindByID(id);
				var o = id;
				if (o != null)
				{
					o.IsSelected = false;
					ch.Update(o);
				}
			}

			return ch;
		}
	}
}