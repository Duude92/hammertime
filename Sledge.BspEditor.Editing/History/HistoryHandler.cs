using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Settings;

namespace Sledge.BspEditor.Editing.History
{
	/// <summary>
	/// Deals with storing history and undo/redo on map documents
	/// </summary>
	[Export(typeof(IInitialiseHook))]
	[Export(typeof(ISettingsContainer))]
	public class HistoryHandler : IInitialiseHook, ISettingsContainer
	{
		public string Name => "Sledge.BspEditor.Editing.History";

		public bool ValuesLoaded { get; set; } = false;

		[Setting] private int MaximumSize { get; set; } = 50;

		public IEnumerable<SettingKey> GetKeys()
		{
			yield return new SettingKey("History", "MaximumSize", typeof(int));
		}

		public void LoadValues(ISettingsStore store)
		{
			store.LoadInstance(this);
		}

		public async Task OnInitialise()
		{
			Oy.Subscribe<MapDocument>("Document:Opened", Opened);
			Oy.Subscribe<MapDocumentOperation>("MapDocument:Perform", Performed);
			Oy.Subscribe<MapDocumentOperation>("MapDocument:Reverse", Reversed);
		}

		public void StoreValues(ISettingsStore store)
		{
			MaximumSize = Math.Max(1, MaximumSize);
			store.StoreInstance(this);
			ValuesLoaded = true;
		}

		private async Task Opened(MapDocument doc)
		{
			doc.Map.Data.Replace(new HistoryStack(MaximumSize));
		}

		private async Task Performed(MapDocumentOperation operation)
		{
			if (operation.Operation.Trivial) return;

			var stack = operation.Document.Map.Data.GetOne<HistoryStack>();
			stack?.Add(operation.Operation);

			Oy.Publish("MapDocument:HistoryChanged", operation.Document);
		}

		private async Task Reversed(MapDocumentOperation operation)
		{
			var stack = operation.Document.Map.Data.GetOne<HistoryStack>();
			stack?.Remove(operation.Operation);

			Oy.Publish("MapDocument:HistoryChanged", operation.Document);
		}
	}
}
