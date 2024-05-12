using Sledge.Common.Shell.Settings;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Sledge.BspEditor.Tools.Plugins
{
	[Export(typeof(ISettingsContainer))]
	public class Plugins : ISettingsContainer
	{
		public string Name => "Sledge.BspEditor.Tools.Plugins";

		public bool ValuesLoaded => true;
		[Setting("IncludeDecompilerTool")]
		private bool _includeDecompiler = false;
		private bool _shouldUpdatePlugin = false;
		public Plugins()
		{
			Application.ApplicationExit += (handle, args) =>
			{
				if (!_shouldUpdatePlugin) return;
				string arguments = "";

				var files = Directory.GetFiles("./plugins/decompiler/");
				if (_includeDecompiler)
				{
					arguments = "/C copy .\\plugins\\decompiler\\* .\\";
				}
				else
				{
					arguments = "/C timeout 2 & del";
					foreach (var file in files)
					{
						arguments += " " + Path.GetFileName(file);
					}
				}
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = "cmd.exe",
					Arguments = arguments,
				};
				Process.Start(startInfo);
			};
		}

		public IEnumerable<SettingKey> GetKeys()
		{
			yield return new SettingKey("Tools/Plugins", "IncludeDecompilerTool", typeof(bool));
		}

		public void LoadValues(ISettingsStore store)
		{
			_includeDecompiler = store.Get("IncludeDecompilerTool", false);
		}

		public void StoreValues(ISettingsStore store)
		{
			var previousState = store.Get("IncludeDecompilerTool", false);
			store.Set("IncludeDecompilerTool", _includeDecompiler);
			_shouldUpdatePlugin = File.Exists("./HammerTime.Decompiler.dll") ? !_includeDecompiler : _includeDecompiler;

		}

	}
}
