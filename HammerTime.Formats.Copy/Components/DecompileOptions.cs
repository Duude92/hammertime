using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.Formats.Bsp.Objects;
using Sledge.Shell;

namespace HammerTime.Formats.Components
{
	[Export(typeof(IDialog))]
	[Export(typeof(ISettingsContainer))]
	[AutoTranslate]
	public partial class DecompileOptions : Form, IDialog, ISettingsContainer
	{
		[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
		[Import] private IContext _context;

		private List<Subscription> _subscriptions;

		public string OpenButton { get => openBsp.Text; set => openBsp.Text = value; }
		public string BrushOptimizationText { get => BrushOptimizationLabel.Text; set => BrushOptimizationLabel.Text = value; }
		public string DecompileStrategyText { get => DecompileStrategyLabel.Text; set => DecompileStrategyLabel.Text = value; }
		public string GenerateOriginBrush { get => originBoxCheckBox.Text; set => originBoxCheckBox.Text = value; }
		public string MergeBrushes { get => mergeBrushesCheckBox.Text; set => mergeBrushesCheckBox.Text = value; }
		public string ApplyNull { get => applyNullCheckBox.Text; set => applyNullCheckBox.Text = value; }
		public string IncludeLiquids { get => includeLiquidsCheckBox.Text; set => includeLiquidsCheckBox.Text = value; }
		public string WildcardTex { get => WildcardLabel.Text; set=> WildcardLabel.Text = value; }
		public new string CancelButton { get => cancelButton.Text; set => cancelButton.Text = value; }

		public DecompileOptions()
		{
			InitializeComponent();
		}

		public bool IsInContext(IContext context)
		{
			return context.HasAny("BspEditor:DecompileOptions");
		}

		public void Translate(ITranslationStringProvider strings)
		{
			CreateHandle();
			var prefix = GetType().FullName;
			this.InvokeLater(() =>
			{
				Text = strings.GetString(prefix, "Title");
			});
		}


		public async Task DocumentActivated(MapDocument document)
		{
			this.InvokeLater(() =>
			{
			});
		}

		private async Task DocumentChanged(Change change)
		{
			this.InvokeLater(() =>
			{
			});
		}

		public void SetVisible(IContext context, bool visible)
		{
			if (visible)
			{
				if (!Visible) Show(_parent.Value);
			}
			else
			{
				Hide();
			}

		}
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			Oy.Publish("Context:Remove", new ContextInfo("BspEditor:DecompileOptions"));
		}
		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void openBsp_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				if (ofd.ShowDialog() != DialogResult.OK) return;



				Oy.Publish("Command:Run", new CommandMessage("Tool:DecompileTool", new
				{
					Path = ofd.FileName,
					Optimization = brushOptimizationBox.SelectedIndex,
					Strategy = strategyComboBox.SelectedIndex,
					GenerateOrigin = originBoxCheckBox.Checked,
					MergeBrushes = mergeBrushesCheckBox.Checked,
					ApplyNull = applyNullCheckBox.Checked,
					IncludeLiquids = includeLiquidsCheckBox.Checked,
					TriggerEntityWildcards = textBox1.Text,
				}));
			}
		}
		[Setting] public int BrushOptimizationSetting { get; set; } = 0;
		[Setting] public int StrategySetting { get; set; }
		[Setting] public bool GenerateOriginSetting { get; set; }
		[Setting] public bool MergeBrushesSetting { get; set; }
		[Setting] public bool ApplyNullSetting { get; set; }
		[Setting] public bool IncludeLiquidsSetting { get; set; }
		[Setting] public string TriggerEntityWildcardsSetting {  get; set; }

		public IEnumerable<SettingKey> GetKeys()
		{
			yield break;
		}

		public void LoadValues(ISettingsStore store)
		{
			store.LoadInstance(this);
			brushOptimizationBox.SelectedIndex = BrushOptimizationSetting;
			strategyComboBox.SelectedIndex = StrategySetting;
			originBoxCheckBox.Checked = GenerateOriginSetting;
			mergeBrushesCheckBox.Checked = MergeBrushesSetting;
			applyNullCheckBox.Checked = ApplyNullSetting;
			includeLiquidsCheckBox.Checked = IncludeLiquidsSetting;
			textBox1.Text = TriggerEntityWildcardsSetting;
		}

		public void StoreValues(ISettingsStore store)
		{
			BrushOptimizationSetting				= brushOptimizationBox.SelectedIndex;
			StrategySetting							= strategyComboBox.SelectedIndex	;
			GenerateOriginSetting					= originBoxCheckBox.Checked			;
			MergeBrushesSetting						= mergeBrushesCheckBox.Checked		;
			ApplyNullSetting						= applyNullCheckBox.Checked			;
			IncludeLiquidsSetting					= includeLiquidsCheckBox.Checked	;
			TriggerEntityWildcardsSetting			= textBox1.Text						;
			store.StoreInstance(this);
		}
	}
}