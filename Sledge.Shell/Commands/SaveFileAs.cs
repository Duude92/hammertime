using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Shell.Properties;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Commands
{
	[AutoTranslate]
	[Export(typeof(ISettingsContainer))]
	[Export(typeof(ICommand))]
	[CommandID("File:SaveAs")]
	[DefaultHotkey("Ctrl+Shift+S")]
	[MenuItem("File", "", "File", "J")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_SaveAs))]
	public class SaveFileAs : ICommand, ISettingsContainer
	{
		private readonly Lazy<DocumentRegister> _documentRegister;

		public string Name { get; set; } = "Save As...";
		public string Details { get; set; } = "Save As...";
		string ISettingsContainer.Name => "Sledge.Shell.Commands.SaveFileAs";

		private string _lastExtension = null;

		public bool ValuesLoaded { get; private set; }

		[ImportingConstructor]
		public SaveFileAs(
			[Import] Lazy<DocumentRegister> documentRegister
		)
		{
			_documentRegister = documentRegister;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out IDocument _);
		}

		public async Task Invoke(IContext context, CommandParameters parameters)
		{
			var doc = context.Get<IDocument>("ActiveDocument");
			if (doc != null)
			{
				string filename;

				var filter = _documentRegister.Value.GetSupportedFileExtensions(doc)
					.Select(x => x.Description + "|" + String.Join(";", x.Extensions.Select(ex => "*" + ex)))
					.ToList();
				var filterIndex = 0;
				if (!(string.IsNullOrEmpty(_lastExtension) || string.IsNullOrWhiteSpace(_lastExtension)))
				{
					filterIndex = filter.FindIndex(f => f.Contains(_lastExtension)) + 1;
				}

				using (var sfd = new SaveFileDialog { Filter = String.Join("|", filter), FilterIndex = filterIndex })
				{
					if (sfd.ShowDialog() != DialogResult.OK) return;
					filename = sfd.FileName;
					_lastExtension = Path.GetExtension(filename);
				}

				await _documentRegister.Value.SaveDocument(doc, filename);
			}
		}

		public IEnumerable<SettingKey> GetKeys()
		{
			yield break;
		}

		public void LoadValues(ISettingsStore store)
		{
			_lastExtension = store.Get<string>("extension");
			ValuesLoaded = true;
		}

		public void StoreValues(ISettingsStore store)
		{
			store.Set("extension", _lastExtension);
		}
	}
}