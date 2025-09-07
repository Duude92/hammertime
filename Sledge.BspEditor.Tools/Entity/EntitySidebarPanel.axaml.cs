using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Tools.Entity;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.Shell;
using System.Linq;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;

[AutoTranslate]
[Export(typeof(ISidebarComponent))]
[OrderHint("F")]
public partial class EntitySidebarPanel : UserControl, ISidebarComponent
{
	public string Title { get; set; } = "Entities";
	public object Control => this;

	public string EntityTypeLabelText
	{
		get => EntityTypeLabel.Content as string;
		set { EntityTypeLabel.InvokeLater(() => { EntityTypeLabel.Content = value; }); }
	}

	public EntitySidebarPanel()
	{
		InitializeComponent();

		Oy.Subscribe<MapDocument>("Document:Activated", d => { this.InvokeLater(() => RefreshEntities(d)); });
		Oy.Subscribe<EntityTool>("EntityTool:ResetEntityType", t => { this.InvokeLater(() => ResetEntityType(t)); });
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out EntityTool _);
	}

	private async Task RefreshEntities(MapDocument doc)
	{
		if (doc == null) return;

		var sel = GetSelectedEntity()?.Name;
		var gameData = await doc.Environment.GetGameData();
		var defaultName = doc.Environment.DefaultPointEntity ?? "";

		EntityTypeLabel.InvokeLater(() =>
		{
			//EntityTypeList.BeginUpdate();
			EntityTypeList.Items.Clear();

			var def = doc.Environment.DefaultPointEntity;
			GameDataObject reselect = null, redef = null;
			foreach (var gdo in gameData.Classes.Where(x => x.ClassType == ClassType.Point).OrderBy(x => x.Name.ToLowerInvariant()))
			{
				EntityTypeList.Items.Add(gdo);
				if (String.Equals(sel, gdo.Name, StringComparison.InvariantCultureIgnoreCase)) reselect = gdo;
				if (String.Equals(def, gdo.Name, StringComparison.InvariantCultureIgnoreCase)) redef = gdo;
			}

			if (reselect == null && redef == null)
			{
				redef = gameData.Classes
					.Where(x => x.ClassType == ClassType.Point)
					.OrderBy(x => x.Name == defaultName ? 0 : (x.Name.StartsWith("info_player_start") ? 1 : 2))
					.FirstOrDefault();
			}

			EntityTypeList.SelectedItem = reselect ?? redef;
			//EntityTypeList.EndUpdate();
		});
		EntityTypeList_SelectedIndexChanged(null, null);
	}

	private async Task ResetEntityType(EntityTool tool)
	{
		var doc = tool.GetDocument();
		if (doc == null) return;

		var gameData = await doc.Environment.GetGameData();
		var defaultName = doc.Environment.DefaultPointEntity ?? "";

		var redef = gameData.Classes
			.Where(x => x.ClassType == ClassType.Point)
			.OrderBy(x => x.Name == defaultName ? 0 : (x.Name.StartsWith("info_player_start") ? 1 : 2))
			.FirstOrDefault();
		if (redef != null)
		{
			EntityTypeList.SelectedItem = redef;
		}
	}

	private GameDataObject GetSelectedEntity()
	{
		return EntityTypeList.SelectedItem as GameDataObject;
	}

	private void EntityTypeList_SelectedIndexChanged(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Context:Add", new ContextInfo("EntityTool:ActiveEntity", GetSelectedEntity()?.Name));
	}
}
