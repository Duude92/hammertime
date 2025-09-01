using Avalonia.Controls;
using Avalonia.Input;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.BspEditor.Editing;
[Export(typeof(IDialog))]
[AutoTranslate]
public partial class EntityReportDialog : Window, IDialog, IManualTranslate
{
	[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
	[Import] private IContext _context;

	private class ColumnComparer : IComparer
	{
		public int Column { get; set; }
		public SortOrder SortOrder { get; set; }

		public ColumnComparer(int column)
		{
			Column = column;
			SortOrder = SortOrder.Ascending;
		}

		public int Compare(object x, object y)
		{
			var i1 = (ListViewItem)x;
			var i2 = (ListViewItem)y;
			var compare = String.CompareOrdinal(i1.SubItems[Column].Text, i2.SubItems[Column].Text);
			return SortOrder == SortOrder.Descending ? -compare : compare;
		}
	}

	private readonly ColumnComparer _sorter;

	private List<Subscription> _subscriptions;
	public bool Visible { get; private set; }
	public EntityReportDialog()
	{
		InitializeComponent();

		_sorter = new ColumnComparer(0);
		//EntityList.ListViewItemSorter = _sorter;
	}

	public void Translate(ITranslationStringProvider strings)
	{
		//TODO: This
		return;
		//if (Handle == null) CreateHandle();
		var prefix = GetType().FullName;
		//this.InvokeLater(() =>
		//{
		Title = strings.GetString(prefix, "Title");
		//ClassNameHeader.Text = strings.GetString(prefix, "ClassHeader");
		//EntityNameHeader.Text = strings.GetString(prefix, "NameHeader");
		GoToButton.Content = strings.GetString(prefix, "GoTo");
		DeleteButton.Content = strings.GetString(prefix, "Delete");
		PropertiesButton.Content = strings.GetString(prefix, "Properties");
		FollowSelection.Content = strings.GetString(prefix, "FollowSelection");
		FilterGroup.Content = strings.GetString(prefix, "Filter");
		TypeAll.Content = strings.GetString(prefix, "ShowAll");
		TypePoint.Content = strings.GetString(prefix, "ShowPoint");
		TypeBrush.Content = strings.GetString(prefix, "ShowBrush");
		IncludeHidden.Content = strings.GetString(prefix, "IncludeHidden");
		FilterByKeyValueLabel.Text = strings.GetString(prefix, "FilterByKeyValue");
		FilterByClassLabel.Text = strings.GetString(prefix, "FilterByClass");
		FilterClassExact.Content = strings.GetString(prefix, "Exact");
		FilterKeyValueExact.Content = strings.GetString(prefix, "Exact");
		ResetFiltersButton.Content = strings.GetString(prefix, "ResetFilters");
		CloseButton.Content = strings.GetString(prefix, "Close");
		//});
	}

	protected override void OnClosing(WindowClosingEventArgs e)
	{
		e.Cancel = true;
		Oy.Publish("Context:Remove", new ContextInfo("BspEditor:EntityReport"));
	}

	private void PointerEntered(object sender, PointerPressedEventArgs args)
	{
		Focus();
		//base.OnMouseEnter(e);
	}

	public bool IsInContext(IContext context)
	{
		return context.HasAny("BspEditor:EntityReport");
	}

	public void SetVisible(IContext context, bool visible)
	{
		//this.InvokeLater(() =>
		{
			if (visible)
			{
				Visible = true;
				//FIXME: Parent is not Avalonia form
				//if (!Visible) Show(_parent.Value);
				Show();
				Subscribe();
				ResetFilters(null, null);
			}
			else
			{
				Visible = false;
				Hide();
				Unsubscribe();
			}
		}
		//);
	}

	private void Subscribe()
	{
		if (_subscriptions != null) return;
		_subscriptions = new List<Subscription>
			{
				Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
				Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated),
				Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", SelectionChanged)
			};
	}

	private void Unsubscribe()
	{
		if (_subscriptions == null) return;
		_subscriptions.ForEach(x => x.Dispose());
		_subscriptions = null;
	}

	public async Task DocumentActivated(MapDocument document)
	{
		FiltersChanged(null, null);
	}

	public async Task SelectionChanged(MapDocument document)
	{
		if (!FollowSelection.IsChecked.Value) return;

		var doc = _context.Get<MapDocument>("ActiveDocument");
		if (doc == null) return;

		var selection = doc.Selection.OfType<Entity>();
		SetSelected(selection);
	}

	private async Task DocumentChanged(Change change)
	{
		if (!change.HasObjectChanges) return;

		if (change.Added.Any(x => x is Entity) || change.Updated.Any(x => x is Entity) || change.Removed.Any(x => x is Entity))
		{
			FiltersChanged(null, null);
		}
	}

	private IEnumerable<Entity> GetSelected()
	{
		return EntityList.SelectedItems.Count == 0 ? Array.Empty<Entity>() : (EntityList.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag as Entity));
	}

	private void SetSelected(IEnumerable<IMapObject> selection)
	{
		//this.InvokeLater(() =>
		{
			if (selection == null) return;
			var items = EntityList.ItemsSource.OfType<ListItem>().Where(x => selection.Contains(x.Entity));//.FirstOrDefault(x => x.Tag == selection);
			if (!items.Any()) return;

			//EntityList.SelectedItems.Clear();
			//foreach (var item in items)
			//{
			//		item.Selected = true;
			//}
			//EntityList.EnsureVisible(EntityList.Items.IndexOf(items.First()));
		}
		//);
	}
	private IEnumerable<ListItem> _items = new List<ListItem>();

	private void FiltersChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		//this.InvokeLater(() =>
		{
			//EntityList.BeginUpdate();
			var selected = GetSelected().ToArray();
			//EntityList.ListViewItemSorter = null;
			//EntityList.Items.Clear();

			var doc = _context.Get<MapDocument>("ActiveDocument");
			if (doc != null)
			{
				_items = (doc.Map.Root
					.Find(x => x is Entity)
					.OfType<Entity>()
					.Where(DoFilters)
					.Select(GetListItem));
					//.ToArray();
				EntityList.ItemsSource = _items;
				//EntityList.Items.AddRange(items);

				//EntityList.ListViewItemSorter = _sorter;
				//EntityList.Sort();
				SetSelected(selected);
				totalEntityCount.Text = $"{_items.Count()}";
			}

			//EntityList.EndUpdate();
		}
		//);
	}
	private class ListItem
	{
		public string ClassName { get; private set; }
		public string TargetName { get; private set; }
		public Entity Entity { get; private set; }
		public ListItem(string name, string target, Entity entity)
		{
			ClassName = name;
			TargetName = target;
			Entity = entity;
		}
	}

	private ListItem GetListItem(Entity entity)
	{
		var targetname = entity.EntityData.Properties.FirstOrDefault(x => x.Key.ToLower() == "targetname");
		return new ListItem(entity.EntityData.Name, targetname.Value ?? "", entity);
	}

	private bool DoFilters(Entity ent)
	{
		var hasChildren = ent.Hierarchy.HasChildren;

		if (hasChildren && TypePoint.IsChecked.Value) return false;
		if (!hasChildren && TypeBrush.IsChecked.Value) return false;
		if (!IncludeHidden.IsChecked.Value)
		{
			if (ent.Data.OfType<IObjectVisibility>().Any(x => x.IsHidden)) return false;
		}

		var classFilter = FilterClass.Text.ToUpperInvariant();
		var exactClass = FilterClassExact.IsChecked.Value;
		var keyFilter = FilterKey.Text.ToUpperInvariant();
		var valueFilter = FilterValue.Text.ToUpperInvariant();
		var exactKeyValue = FilterKeyValueExact.IsChecked.Value;

		if (!String.IsNullOrWhiteSpace(classFilter))
		{
			var name = (ent.EntityData.Name ?? "").ToUpperInvariant();
			if (exactClass && name != classFilter) return false;
			if (!exactClass && !name.Contains(classFilter)) return false;
		}

		if (!String.IsNullOrWhiteSpace(keyFilter))
		{
			if (ent.EntityData.Properties.All(x => x.Key.ToUpperInvariant() != keyFilter)) return false;
			var prop = ent.EntityData.Properties.FirstOrDefault(x => x.Key.ToUpperInvariant() == keyFilter);
			var val = prop.Value.ToUpperInvariant();
			if (exactKeyValue && val != valueFilter) return false;
			if (!exactKeyValue && !val.Contains(valueFilter)) return false;
		}

		return true;
	}

	private void ResetFilters(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		TypeAll.IsChecked = true;
		IncludeHidden.IsChecked = true;
		FilterKeyValueExact.IsChecked = false;
		FilterClassExact.IsChecked = false;
		FilterKey.Text = "";
		FilterValue.Text = "";
		FilterClass.Text = "";
		FiltersChanged(null, null);
	}

	//private void SortByColumn(object sender, ColumnClickEventArgs e)
	//{
	//	if (_sorter.Column == e.Column)
	//	{
	//		_sorter.SortOrder = _sorter.SortOrder == SortOrder.Descending
	//								? SortOrder.Ascending
	//								: SortOrder.Descending;
	//	}
	//	else
	//	{
	//		_sorter.Column = e.Column;
	//		_sorter.SortOrder = SortOrder.Ascending;
	//	}
	//	//EntityList.Sort();
	//	SetSelected(GetSelected()); // Reset the scroll value
	//}

	private async Task SelectEntity(IEnumerable<Entity> sel)
	{
		var doc = _context.Get<MapDocument>("ActiveDocument");
		if (doc == null) return;

		var currentSelection = doc.Selection.Except(sel).ToList();
		var tran = new Transaction(
			new Deselect(currentSelection),
			new Select(sel)
		);
		await MapDocumentOperation.Perform(doc, tran);
	}
	public Box GetSelectionBoundingBox(IEnumerable<Entity> entities)
	{
		return !entities.Any() ? Box.Empty : new Box(entities.Select(x => x.BoundingBox).Where(x => x != null).DefaultIfEmpty(Box.Empty));
	}
	private void GoToSelectedEntity(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var selected = GetSelected();
		if (selected == null) return;
		SelectEntity(selected);
		var bb = GetSelectionBoundingBox(selected);
		Oy.Publish("MapDocument:Viewport:Focus2D", bb);
		Oy.Publish("MapDocument:Viewport:Focus3D", bb);
	}

	private void DeleteSelectedEntity(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var doc = _context.Get<MapDocument>("ActiveDocument");
		if (doc == null) return;

		var selected = GetSelected();
		if (selected == null) return;
		MapDocumentOperation.Perform(doc, new Detatch(selected.First().Hierarchy.Parent.ID, selected.ToArray()));
	}

	private void OpenEntityProperties(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var selected = GetSelected();
		if (selected == null) return;
		SelectEntity(selected).ContinueWith(_ => Oy.Publish("Command:Run", new CommandMessage("BspEditor:Map:Properties")));
	}

	private void CloseButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		Close();
	}
	private void EntityList_SelectedIndexChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
	{
		selectedEntityCount.Text = $"{EntityList.SelectedItems.Count}";
	}
}