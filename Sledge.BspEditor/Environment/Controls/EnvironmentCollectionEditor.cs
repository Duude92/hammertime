using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Environment.Controls
{
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

		public EnvironmentCollectionEditor(IEnumerable<IEnvironmentFactory> factories)
		{
			_factories = factories.ToList();
			InitializeComponent();
			Anchor = AnchorStyles.Top | AnchorStyles.Bottom;

            _nameLabel = new Label {Text = "Name", Padding = new Padding(0, 6, 0, 0), AutoSize = true};
            _nameBox = new TextBox{Width = 250};
			_nameBox.TextChanged += UpdateEnvironment;

			if (_factories.Any())
			{
				ctxEnvironmentMenu.Items.Clear();
				foreach (var ef in _factories)
				{
					var mi = new ToolStripMenuItem(ef.Description) { Tag = ef };
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
			btnAdd.Text = strings.GetString(prefix, "Add");
			btnRemove.Text = strings.GetString(prefix, "Remove");
			_nameLabel.Text = strings.GetString(prefix, "Name");
		}

		private void UpdateTreeNodes()
		{
			treEnvironments.Nodes.Clear();
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
				treEnvironments.Nodes.Add(groupNode);
			}
			treEnvironments.ExpandAll();
		}

		private void AddEnvironment(object sender, EventArgs e)
		{
			var factory = (sender as ToolStripItem)?.Tag as IEnvironmentFactory;
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

				var nodeToSelect = treEnvironments.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).FirstOrDefault(x => x.Tag == newEnv);
				if (nodeToSelect != null) treEnvironments.SelectedNode = nodeToSelect;

				OnValueChanged?.Invoke(this, Key);
			}
		}

		private void RemoveEnvironment(object sender, EventArgs e)
		{
			var node = treEnvironments.SelectedNode?.Tag as SerialisedEnvironment;
			if (node != null && _value != null)
			{
				_value.Remove(node);
				UpdateTreeNodes();
				OnValueChanged?.Invoke(this, Key);
				EnvironmentSelected(null, null);
			}
		}

		private IEnvironmentEditor _currentEditor = null;

		private void EnvironmentSelected(object sender, TreeViewEventArgs e)
		{
			if (_currentEditor != null) _currentEditor.EnvironmentChanged -= UpdateEnvironment;

			var translate = Common.Container.Get<ITranslationStringProvider>();

			_currentEditor = null;
			pnlSettings.Controls.Clear();

			var node = e?.Node?.Tag as SerialisedEnvironment;
			if (node != null)
			{
				var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
				if (factory != null)
				{
					var fp = new FlowLayoutPanel
					{
						Height = 30,
						Width = 400,
						FlowDirection = FlowDirection.LeftToRight,
						Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
					};
					fp.Controls.Add(_nameLabel);
					fp.Controls.Add(_nameBox);
					pnlSettings.Controls.Add(fp);

					_nameBox.Text = node.Name;

					var des = factory.Deserialise(node);
					_currentEditor = factory.CreateEditor();
					translate.Translate(_currentEditor);
					pnlSettings.Controls.Add(_currentEditor.Control);
					_currentEditor.Environment = des;
					_currentEditor.EnvironmentChanged += UpdateEnvironment;
				}
			}
		}

		private void UpdateEnvironment(object sender, EventArgs e)
		{
			var node = treEnvironments.SelectedNode?.Tag as SerialisedEnvironment;
			if (node != null && _currentEditor != null)
			{
				treEnvironments.SelectedNode.Text = _nameBox.Text;
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

		private void importProfileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog
			{
				AddExtension = true,
				DefaultExt = "json",
				Filter = "JSON file (*.json)|*.json"
			};
			if (fileDialog.ShowDialog() != DialogResult.OK) return;

			FolderBrowserDialog folder = new FolderBrowserDialog
			{
				UseDescriptionForTitle = true,
				Description = "Select game base folder"
			};
			if (folder.ShowDialog() != DialogResult.OK) return;


			using (var file = new StreamReader(fileDialog.FileName))
			{
				var invSer = file.ReadToEnd();
				JsonSettingsStore store = new JsonSettingsStore(invSer);
				var ser = store.Get<SerialisedEnvironment>("Environment");
				if (_value.Any(x => x.Name == ser.Name)) ser.Name += "_copy";
				if (_value.Any(x => x.ID == ser.ID)) ser.ID = Guid.NewGuid().ToString();
				var factory = _factories.FirstOrDefault(x => x.TypeName == ser.Type);
				if (factory is GoldsourceEnvironmentFactory gsFactory)
				{
					ser.Properties.Add("BaseDirectory", folder.SelectedPath);
					var env = gsFactory.InverseDeserialise(ser) as GoldsourceEnvironment;
					_value.Add(gsFactory.Serialise(env));

					UpdateTreeNodes();

					var nodeToSelect = treEnvironments.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>()).FirstOrDefault(x => x.Tag == ser);
					if (nodeToSelect != null) treEnvironments.SelectedNode = nodeToSelect;

					OnValueChanged?.Invoke(this, Key);
				}
			}

		}

		private void exportProfileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var node = treEnvironments.SelectedNode?.Tag as SerialisedEnvironment;
			if (node != null && _currentEditor != null)
			{
				treEnvironments.SelectedNode.Text = _nameBox.Text;
				var factory = _factories.FirstOrDefault(x => x.TypeName == node.Type);
				if (factory != null && factory is GoldsourceEnvironmentFactory gsFactory)
				{
					var ser = gsFactory.InverseSerilise(_currentEditor.Environment);
					ser.Name = node.Name;
					ser.ID = node.ID;

					SaveFileDialog fileDialog = new SaveFileDialog
					{
						AddExtension = true,
						DefaultExt = "json",
						Filter = "JSON file (*.json)|*.json"
					};
					if (fileDialog.ShowDialog() == DialogResult.OK)
					{
						using (var file = new StreamWriter(fileDialog.FileName))
						{
							JsonSettingsStore store = new JsonSettingsStore();
							store.Set("Environment", ser);
							file.Write(store.ToJson());
						}
					}
				}
			}
		}
	}
}
