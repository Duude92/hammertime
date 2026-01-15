using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Sledge.BspEditor.Environment.Controls;

public partial class EnvironmentCollectionEditor : UserControl, ISettingEditor, IManualTranslate
{
	private readonly List<IEnvironmentFactory> _factories;
	private EnvironmentCollection _value;
	public event EventHandler<SettingKey> OnValueChanged;

	public string Label { get; set; }

	public object Value
	{
		get => _value;
		set
		{
			_value = value as EnvironmentCollection;
			UpdateTreeNodes();
		}
	}

	public object Control => this;

	public SettingKey Key { get; set; }

	private Label _nameLabel;
	private TextBox _nameBox;
	private ContextMenu ctxEnvironmentMenu =>btnAdd.ContextMenu;


	public EnvironmentCollectionEditor(IEnumerable<IEnvironmentFactory> factories)
	{
		_factories = factories.ToList();
		InitializeComponent();
		//Anchor = AnchorStyles.Top | AnchorStyles.Bottom;

		_nameLabel = new Label { Content = "Name", Padding = new Thickness(0, 6, 0, 0), /*AutoSize = true*/ };
		_nameBox = new TextBox { Width = 250 };
		_nameBox.TextChanged += UpdateEnvironment;

		if (_factories.Any())
		{
			btnAdd.ContextMenu = new ContextMenu();
			ctxEnvironmentMenu.Items.Clear();
			foreach (var ef in _factories)
			{
				var mi = new MenuItem() { Tag = ef, Name = ef.Description };
				mi.Click += AddEnvironment;
				ctxEnvironmentMenu.Items.Add(mi);
			}
		}

		var translate = Common.Container.Get<ITranslationStringProvider>();
		translate.Translate(this);
	}

	public void Translate(ITranslationStringProvider strings)
	{
		var prefix = GetType().FullName;
		btnAdd.Content = strings.GetString(prefix, "Add");
		btnRemove.Content = strings.GetString(prefix, "Remove");
		_nameLabel.Content = strings.GetString(prefix, "Name");
	}

	private void UpdateTreeNodes()
	{
		treEnvironments.Items.Clear();
		if (_value == null) return;

		foreach (var g in _value.GroupBy(x => x.Type))
		{
			var ef = _factories.FirstOrDefault(x => x.TypeName == g.Key)?.Description ?? g.Key;

			var groupNode = new TreeNode(ef);
			foreach (var se in g)
			{
				var envNode = new TreeNode(se.Name) { Tag = se };
				groupNode.Nodes.Add(envNode);
			}
			treEnvironments.Items.Add(groupNode);
		}
		//treEnvironments.ExpandAll();
	}

	private void AddEnvironment(object sender, RoutedEventArgs e)
	{
		var factory = (sender as MenuItem)?.Tag as IEnvironmentFactory;
		if (factory != null && _value != null)
		{
			var newEnv = new SerialisedEnvironment
			{
				ID = Guid.NewGuid().ToString("N"),
				Name = "New Environment",
				Type = factory.TypeName
			};
			_value.Add(newEnv);
			UpdateTreeNodes();

			var nodeToSelect = treEnvironments.Items.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).FirstOrDefault(x => x.Tag == newEnv);
			if (nodeToSelect != null) treEnvironments.SelectedItem = nodeToSelect;

			OnValueChanged?.Invoke(this, Key);
		}
	}

	private void RemoveEnvironment(object sender, EventArgs e)
	{
		var node = (treEnvironments.SelectedItem as TreeNode)?.Tag as SerialisedEnvironment;
		if (node != null && _value != null)
		{
			_value.Remove(node);
			UpdateTreeNodes();
			OnValueChanged?.Invoke(this, Key);
			EnvironmentSelected(null, null);
		}
	}

	private IEnvironmentEditor _currentEditor = null;

	private void EnvironmentSelected(object sender, SelectionChangedEventArgs e)
	{
		if (_currentEditor != null) _currentEditor.EnvironmentChanged -= UpdateEnvironment;

		var translate = Common.Container.Get<ITranslationStringProvider>();

		_currentEditor = null;
		pnlSettings.Children.Clear();

		var node = (treEnvironments.SelectedItem as TreeNode)?.Tag as SerialisedEnvironment;
		if (node != null)
		{
			var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
			if (factory != null)
			{
				var fp = new WrapPanel
				{
					Height = 30,
					Width = 400,
					FlowDirection = FlowDirection.LeftToRight,
					//Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
				};
				fp.Children.Add(_nameLabel);
				fp.Children.Add(_nameBox);
				pnlSettings.Children.Add(fp);

				_nameBox.Text = node.Name;

				var des = factory.Deserialise(node);
				_currentEditor = factory.CreateEditor();
				translate.Translate(_currentEditor);
				//pnlSettings.Children.Add(_currentEditor.Control);
				_currentEditor.Environment = des;
				_currentEditor.EnvironmentChanged += UpdateEnvironment;
			}
		}
	}

	private void UpdateEnvironment(object sender, EventArgs e)
	{
		var node = (treEnvironments.SelectedItem as TreeNode)?.Tag as SerialisedEnvironment;
		if (node != null && _currentEditor != null)
		{
			(treEnvironments.SelectedItem as TreeNode).Name = _nameBox.Text;
			var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
			if (factory != null)
			{
				var ser = factory.Serialise(_currentEditor.Environment);
				node.Name = _nameBox.Text;
				node.Properties = ser.Properties;
			}
			OnValueChanged?.Invoke(this, Key);
		}
	}

	private async void importProfileToolStripMenuItem_Click(object sender, EventArgs e)
	{
		var importFile = await (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow.StorageProvider.
			OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions { FileTypeFilter = new[] { new Avalonia.Platform.Storage.FilePickerFileType("JSON file") { Patterns = new[] { "*.json" } } }, AllowMultiple = false });

		if (importFile == null || !importFile.Any()) return;
		var fileDialog = importFile.First();

		var gameFolder = await (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow.StorageProvider.
			OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions { Title = "Select game base folder" });

		if (gameFolder == null || !gameFolder.Any()) return;
		var folder = gameFolder.First();


		using (var file = new StreamReader(fileDialog.Path.ToString()))
		{
			var invSer = file.ReadToEnd();
			JsonSettingsStore store = new JsonSettingsStore(invSer);
			var ser = store.Get<SerialisedEnvironment>("Environment");
			if (_value.Any(x => x.Name == ser.Name)) ser.Name += "_copy";
			if (_value.Any(x => x.ID == ser.ID)) ser.ID = Guid.NewGuid().ToString();
			var factory = _factories.FirstOrDefault(x => x.TypeName == ser.Type);
			if (factory is GoldsourceEnvironmentFactory gsFactory)
			{
				ser.Properties.Add("BaseDirectory", folder.Path.ToString());
				var env = gsFactory.InverseDeserialise(ser) as GoldsourceEnvironment;
				_value.Add(gsFactory.Serialise(env));

				UpdateTreeNodes();

				var nodeToSelect = treEnvironments.Items.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).FirstOrDefault(x => x.Tag == ser);
				if (nodeToSelect != null) treEnvironments.SelectedItem = nodeToSelect;

				OnValueChanged?.Invoke(this, Key);
			}
		}

	}

	private async void exportProfileToolStripMenuItem_Click(object sender, EventArgs e)
	{
		var node = (treEnvironments.SelectedItem as TreeNode).Tag as SerialisedEnvironment;
		if (node != null && _currentEditor != null)
		{
			(treEnvironments.SelectedItem as TreeNode).Name = _nameBox.Text;
			var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
			if (factory != null && factory is GoldsourceEnvironmentFactory gsFactory)
			{
				var ser = gsFactory.InverseSerialise(_currentEditor.Environment);
				ser.Name = node.Name;
				ser.ID = node.ID;

				var importFile = await (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?.MainWindow.StorageProvider.
					OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions { FileTypeFilter = new[] { new Avalonia.Platform.Storage.FilePickerFileType("JSON file") { Patterns = new[] { "*.json" } } }, AllowMultiple = false });

				if (importFile != null && importFile.Any())
				{
					var fileDialog = importFile.First();
					using (var file = new StreamWriter(fileDialog.Path.ToString()))
					{
						JsonSettingsStore store = new JsonSettingsStore();
						store.Set("Environment", ser);
						file.Write(store.ToJson());
					}
				}
			}
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
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
