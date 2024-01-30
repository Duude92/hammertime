﻿using LogicAndTrick.Oy;
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

namespace Sledge.BspEditor.Editing.Components.Prefabs
{
	[AutoTranslate]
	[Export(typeof(ISidebarComponent))]
	[Export(typeof(IInitialiseHook))]
	[OrderHint("H")]


	public partial class PrefabSidebarPanel : UserControl, ISidebarComponent, IInitialiseHook
	{
		private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

		public string Title => "Prefabs";

		public object Control => this;
		private string[] _files = null;
		private WorldcraftPrefabLibrary _activeWorldcraftPrefabLibrary;

		public PrefabSidebarPanel()
		{
			InitializeComponent();
			CreateHandle();

			_files = Directory.GetFiles("./prefabs/");

			FileContainer.Items.AddRange(_files.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray());
			FileContainer.SelectedIndex = 0;

			UpdatePrefabList();

		}

		private void UpdatePrefabList(int index = 0)
		{
			PrefabList.Items.Clear();
			_activeWorldcraftPrefabLibrary = WorldcraftPrefabLibrary.FromFile(_files[index]);

			PrefabList.Items.AddRange(_activeWorldcraftPrefabLibrary.Prefabs.Select(x => x.Name).ToArray());
			PrefabList.SelectedIndex = 0;
		}

		public bool IsInContext(IContext context)
		{
			return context.TryGet("ActiveDocument", out MapDocument _);
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

		private void CreateButton_Click(object sender, EventArgs e)
		{
			_activeDocument.TryGetTarget(out var mapDocument);
			var ung = mapDocument.Map.NumberGenerator;
			
			var contents = HammerTime.Formats.Prefab.GetPrefab(_activeWorldcraftPrefabLibrary.Prefabs[PrefabList.SelectedIndex].Map, ung, mapDocument.Map);

			var transaction = new Transaction();
			transaction.Add(new Attach(mapDocument.Map.Root.ID, contents));
			transaction.Add(new Deselect(mapDocument.Selection));
			transaction.Add(new Select(contents));


			MapDocumentOperation.Perform(mapDocument, transaction);

		}

		private void FileContainer_SelectionChangeCommitted(object sender, EventArgs e)
		{
			UpdatePrefabList(FileContainer.SelectedIndex);
		}
	}
}