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
using Sledge.Common.Translations;
using Sledge.Formats.Bsp.Objects;
using Sledge.Shell;

namespace HammerTime.Formats.Components
{
	[Export(typeof(IDialog))]
	[AutoTranslate]
	public partial class DecompileOptions : Form, IDialog, IManualTranslate
	{
		[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
		[Import] private IContext _context;

		private List<Subscription> _subscriptions;

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
			this.InvokeLater(() =>
			{
				if (visible)
				{
					if (!Visible) Show(_parent.Value);
				}
				else
				{
					Hide();
				}
			});
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
	}
}