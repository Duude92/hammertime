using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Formats.Map.Formats;
using static Sledge.Shell.ControlExtensions;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using System.Drawing;
using System.Xml.Linq;
using Sledge.BspEditor.Modification.Operations.Mutation;
using System.Numerics;
using Newtonsoft.Json.Linq;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Primitives;
using System.Reflection;

namespace Sledge.BspEditor.Tools.Prefab
{
    [AutoTranslate]
	[Export(typeof(ISidebarComponent))]
	[Export(typeof(IInitialiseHook))]
	[OrderHint("A")]


	public partial class PrefabSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
	{
		private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

		public string Title => "Prefabs";

		public object Control => this;
		private string[] _files = null;
		private WorldcraftPrefabLibrary _activeWorldcraftPrefabLibrary;
		#region Translations
		public string CreatePrefabButton { set => this.InvokeLater(() => this.CreateButton.Text = value); }
		public string NewPrefabButton { set=>this.InvokeLater(()=>this.NewPrefab.Text = value); }
		public string CreateLibButton { set=>this.InvokeLater(()=>this.CreateLib.Text = value); }
		public string NewPrefabPlaceholder { set => this.InvokeLater(() => this.NewPrefabName.Text = value); }
		public string NewLibPlaceholder { set=> this.InvokeLater(()=>this.NewLibName.Text = value); }
		#endregion
		public PrefabSidebarPanel()
		{
			InitializeComponent();
			CreateHandle();

			InitPrefabLibraries();

		}

		private void InitPrefabLibraries()
		{
			if (!Directory.Exists("./prefabs/")) return;
			_files = Directory.GetFiles("./prefabs/");

			FileContainer.Items.Clear();

			FileContainer.Items.AddRange(_files.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray());
			FileContainer.SelectedIndex = 0;

			UpdatePrefabList();

			Task.Delay(3000).ContinueWith(t =>
			{
				Oy.Publish("Context:Add", new ContextInfo("PrefabTool:ActiveLibrary", _files[0])); //Should be delayed until PrefabTool is created
			});
		}

		private void UpdatePrefabList(int index = 0)
		{
			PrefabList.Items.Clear();
			_activeWorldcraftPrefabLibrary = WorldcraftPrefabLibrary.FromFile(_files[index]);

			PrefabList.Text = null;

			PrefabList.Items.AddRange(_activeWorldcraftPrefabLibrary.Prefabs.Select(x => x.Name).ToArray());
			if (PrefabList.Items.Count > 0)
				PrefabList.SelectedIndex = 0;


		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveTool", out PrefabTool _);
		}

		public Task OnInitialise()
		{
			Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
			Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
			return Task.FromResult(0);
		}

		private async Task DocumentActivated(IDocument doc)
		{
			var md = doc as MapDocument;

			_activeDocument = new WeakReference<MapDocument>(md);
			Update(md);
		}

		private async Task DocumentChanged(Change change)
		{
			if (_activeDocument.TryGetTarget(out MapDocument t) && change.Document == t)
			{
				if (change.HasObjectChanges)
				{
					Update(change.Document);
				}
			}
		}

		private void Update(MapDocument document)
		{
			Task.Factory.StartNew(() =>
			{

			});
		}


		private async void CreateButton_Click(object sender, EventArgs e)
		{
			await Oy.Publish("PrefabTool:CreatePrefab", PrefabList.SelectedIndex);
		}

		private void FileContainer_SelectionChangeCommitted(object sender, EventArgs e)
		{
			UpdatePrefabList(FileContainer.SelectedIndex);
		}

		private void NewPrefab_Click(object sender, EventArgs e)
		{
			var name = NewPrefabName.Text.Trim();
			if (String.IsNullOrEmpty(name)) throw new Exception($"Prefab name cannot be empty.\r\nPrefab name: {name}");
			if (_activeDocument.TryGetTarget(out var mapDocument))
			{
				var selection = mapDocument.Selection;
				HammerTime.Formats.Map.Prefab.WriteObjects(_activeWorldcraftPrefabLibrary, selection, name);

				_activeWorldcraftPrefabLibrary.WriteToFile(_files[FileContainer.SelectedIndex]);

				UpdatePrefabList(FileContainer.SelectedIndex);


			}
		}

		private void PrefabList_SelectedValueChanged(object sender, EventArgs e)
		{
			Oy.Publish("Context:Add", new ContextInfo("PrefabTool:PrefabIndex", PrefabList.SelectedIndex));
		}

		private void FileContainer_SelectedIndexChanged(object sender, EventArgs e)
		{
			Oy.Publish("Context:Add", new ContextInfo("PrefabTool:ActiveLibrary", _files[FileContainer.SelectedIndex]));
		}

		private void CreateLib_Click(object sender, EventArgs e)
		{
			if(!Directory.Exists("./prefabs/")) Directory.CreateDirectory("./prefabs/");
			var name = NewLibName.Text.Trim();
			if (String.IsNullOrEmpty(name)) throw new Exception($"Prefab name cannot be empty.\r\nPrefab name: {name}");
			var lib = new WorldcraftPrefabLibrary() { Description = name };
			lib.WriteToFile($"./prefabs/{name}.ol");
			InitPrefabLibraries();
		}
	}
}
