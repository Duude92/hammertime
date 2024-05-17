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
using Sledge.Common.Shell.Menu;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Shell.Properties;

namespace Sledge.Shell.Commands
{
	[AutoTranslate]
	[Export(typeof(ISettingsContainer))]
	[Export(typeof(ICommand))]
	[CommandID("File:Export")]
	[MenuItem("File", "", "File", "L")]
	[MenuImage(typeof(Resources), nameof(Resources.Menu_Export))]
	public class Export : ICommand, ISettingsContainer
	{
		private readonly IEnumerable<Lazy<IDocumentLoader>> _loaders;

		public string Name { get; set; } = "Export...";
		public string Details { get; set; } = "Export...";
		string ISettingsContainer.Name => "Sledge.Shell.Commands.Export";

		private string _lastExtension = null;
		public bool ValuesLoaded { get; private set; } = false;

		[ImportingConstructor]
		public Export([ImportMany] IEnumerable<Lazy<IDocumentLoader>> loaders)
		{
			_loaders = loaders;
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
				var loaders = _loaders.Select(x => x.Value).Where(x => x.CanSave(doc)).ToList();

				var filter = loaders.SelectMany(x => x.SupportedFileExtensions).Select(x => x.Description + "|" + String.Join(";", x.Extensions.Select(e => "*" + e))).ToList();

				var filterIndex = 0;
				if (!(string.IsNullOrEmpty(_lastExtension) || string.IsNullOrWhiteSpace(_lastExtension)))
				{
					filterIndex = filter.FindIndex(f => f.Contains(_lastExtension)) + 1;
				}


				using (var sfd = new SaveFileDialog { Filter = String.Join("|", filter), FilterIndex = filterIndex })
				{
					if (sfd.ShowDialog() == DialogResult.OK)
					{
						var loader = loaders.FirstOrDefault(x => x.CanLoad(doc.FileName));
						if (loader != null)
						{
							await Oy.Publish("Document:BeforeSave", doc);
							await loader.Save(doc, sfd.FileName);

						}
						_lastExtension = Path.GetExtension(sfd.FileName);
					}
				}
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