using Avalonia.Controls;
using Avalonia.Input;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Shell.Settings.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Sledge.Shell.Forms;

[Export(typeof(IDialog))]
[AutoTranslate]
public partial class SettingsForm : Window, IDialog
{
	private readonly IEnumerable<Lazy<ISettingEditorFactory>> _editorFactories;
	private readonly IEnumerable<Lazy<ISettingsContainer>> _settingsContainers;
	private readonly Lazy<ITranslationStringProvider> _translations;
	private readonly Lazy<Window> _parent;

	private Dictionary<ISettingsContainer, List<SettingKey>> _keys;
	private Dictionary<ISettingsContainer, JsonSettingsStore> _values;
	private bool _darktheme;

	//public string Title
	//{
	//	get => Text;
	//	set => this.InvokeLater(() => Text = value);
	//}

	public string OK
	{
		get => OKButton.Content as string;
		set => this.InvokeLater(() => OKButton.Content = value);
	}

	public string Cancel
	{
		get => CancelButton.Content as string;
		set => this.InvokeLater(() => CancelButton.Content = value);
	}

	public bool Visible => IsVisible;

	[ImportingConstructor]
	public SettingsForm(
		[ImportMany] IEnumerable<Lazy<ISettingEditorFactory>> editorFactories,
		[ImportMany] IEnumerable<Lazy<ISettingsContainer>> settingsContainers,
		[Import] Lazy<ITranslationStringProvider> translations,
		[Import("Shell")] Lazy<Window> parent
	)
	{
		_editorFactories = editorFactories;
		_settingsContainers = settingsContainers;
		_translations = translations;
		_parent = parent;

		InitializeComponent();
		//Icon = Icon.FromHandle(Resources.Menu_Options.GetHicon());
	}
	protected void OnVisibleChanged(EventArgs e)
	{
		if (Visible)
		{
			_keys = _settingsContainers.ToDictionary(x => x.Value, x =>
			{
				var keys = x.Value.GetKeys().ToList();
				keys.ForEach(k => k.Container = x.Value.Name);
				return keys;
			});
			_values = _settingsContainers.ToDictionary(x => x.Value, x =>
			{
				var fss = new JsonSettingsStore();
				x.Value.StoreValues(fss);
				return fss;
			});
			LoadGroupList();
		}
		//base.OnVisibleChanged(e);
	}
	protected override void OnClosing(WindowClosingEventArgs e)
	{
		e.Cancel = true;
		Oy.Publish("Context:Remove", new ContextInfo("SettingsForm"));
		this.Owner.Focus();
	}

	private void LoadGroupList()
	{
		var Items = new Dictionary<string, TreeNode>();

		GroupList.Items.Clear();
		foreach (var k in _keys.SelectMany(x => x.Value).GroupBy(x => x.Group).OrderBy(x => x.Key))
		{
			var gh = new GroupHolder(k.Key, _translations.Value.GetSetting("@Group." + k.Key) ?? k.Key);

			TreeNode parentNode = null;
			var par = k.Key.LastIndexOf('/');
			if (par > 0)
			{
				var sub = k.Key.Substring(0, par);
				if (Items.ContainsKey(sub))
				{
					parentNode = Items[sub];
				}
				else
				{
					var pgh = new GroupHolder(sub, _translations.Value.GetSetting("@Group." + sub) ?? sub);
					parentNode = new TreeNode(pgh.Label) { Tag = pgh };
					GroupList.Items.Add(parentNode);
					Items.Add(sub, parentNode);
				}
			}
			var node = new TreeNode(gh.Label) { Tag = gh };
			if (parentNode != null) parentNode.Nodes.Add(node);
			else GroupList.Items.Add(node);
			Items.Add(k.Key, node);
		}

		//GroupList.ExpandAll();

	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		Focus();
		base.OnPointerEntered(e);
	}

	private readonly List<ISettingEditor> _editors = new List<ISettingEditor>();

	private void LoadEditorList()
	{
		_editors.ForEach(x => x.OnValueChanged -= OnValueChanged);
		_editors.Clear();

		SettingsPanel.Children.Clear();

		//SettingsPanel.RowStyles.Clear();

		if ((GroupList?.SelectedItem as TreeNode)?.Tag is GroupHolder gh)
		{
			var group = gh.Key;
			foreach (var kv in _keys)
			{
				var container = kv.Key;
				var keys = kv.Value.Where(x => x.Group == group);
				var values = _values[container];
				foreach (var key in keys)
				{
					var editor = GetEditor(key);
					editor.UseDarkTheme(_darktheme);
					editor.Key = key;
					editor.Label = _translations.Value.GetSetting($"{kv.Key.Name}.{key.Key}") ?? key.Key;
					editor.Value = values.Get(key.Type, key.Key);

					if (SettingsPanel.Children.Count > 0)
					{
						// Add a separator
						var line = new Label
						{
							Height = 1,
							//BackColor = Color.FromArgb(128, Color.Black),
							//Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
						};
						SettingsPanel.Children.Add(line);
					}

					var ctrl = (Control)editor.Control;
					//ctrl.Anchor |= AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
					SettingsPanel.Children.Add(ctrl);

					//if (ctrl.Anchor.HasFlag(AnchorStyles.Bottom))
					//{
					//	SettingsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
					//}

					_editors.Add(editor);
				}
			}
		}


		_editors.ForEach(x => x.OnValueChanged += OnValueChanged);
	}

	private void OnValueChanged(object sender, SettingKey key)
	{
		var se = sender as ISettingEditor;
		var store = _values.Where(x => x.Key.Name == key.Container).Select(x => x.Value).FirstOrDefault();
		if (store != null && se != null)
		{
			store.Set(key.Key, se.Value);
		}
	}

	private ISettingEditor GetEditor(SettingKey key)
	{
		foreach (var ef in _editorFactories.OrderBy(x => x.Value.OrderHint))
		{
			if (ef.Value.Supports(key)) return ef.Value.CreateEditorFor(key);
		}
		return new DefaultSettingEditor();
	}

	public bool IsInContext(IContext context)
	{
		return context.HasAny("SettingsForm");
	}

	public void SetVisible(IContext context, bool visible)
	{
		this.InvokeLater(() =>
		{
			if (visible)
			{
				if (!Visible) Show(_parent.Value);
			}
			else
			{
				if (Visible) Hide();
			}
			OnVisibleChanged(null);
		});
	}

	private void GroupList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		LoadEditorList();
	}

	private void OkClicked(object sender, EventArgs e)
	{
		foreach (var kv in _values)
		{
			kv.Key.LoadValues(kv.Value);
		}
		Oy.Publish("SettingPreChanged");
		Oy.Publish("Settings:Save");
		Oy.Publish("SettingsChanged", new object());
		Close();
	}

	private void CancelClicked(object sender, EventArgs e)
	{
		Close();
	}

	public void UseDarkTheme(bool dark)
	{
		_darktheme = dark;
	}
	private class GroupHolder
	{
		public string Key { get; set; }
		public string Label { get; set; }

		public GroupHolder(string key, string label)
		{
			Key = key;
			Label = label;
		}

		public override string ToString()
		{
			return Label;
		}
	}

}

public class TreeNode
{
	public string Name { get; set; }
	public object Tag { get; set; }
	public NodeList Nodes { get; set; } = new NodeList();
	public bool IsExpanded { get; set; }
	public TreeNode(string name)
	{
		Name = name;
	}
	public class NodeList : ObservableCollection<TreeNode>
	{
		public TreeNode Add(string name)
		{
			var node = new TreeNode(name);
			base.Add(node);
			return node;
		}
		public void AddRange(TreeNode[] nodes)
		{
			foreach (var n in nodes) base.Add(n);
		}
	}
}
