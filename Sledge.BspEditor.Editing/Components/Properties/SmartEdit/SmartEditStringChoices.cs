using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.DataStructures.GameData;

namespace Sledge.BspEditor.Editing.Components.Properties.SmartEdit
{
	[Export(typeof(IObjectPropertyEditor))]
	public class SmartEditStringChoices : SmartEditControl
	{
		private readonly ComboBox _comboBox;
		public SmartEditStringChoices()
		{
			_comboBox = new ComboBox { Width = 250 };
			_comboBox.TextChanged += (sender, e) => OnValueChanged();
			Controls.Add(_comboBox);
		}

		public override string PriorityHint => "H";

		public override bool SupportsType(VariableType type)
		{
			return type == VariableType.StringChoices;
		}

		protected override string GetName()
		{
			return OriginalName;
		}

		protected override string GetValue()
		{
			return _comboBox.Text;
		}

		private IEnumerable<Option> GetSortedOptions()
		{
			int key;
			if (Property.Options.All(x => int.TryParse(x.Key, out key)))
			{
				return Property.Options.OrderBy(x => int.Parse(x.Key));
			}
			return Property.Options.OrderBy(x => x.Key.ToLowerInvariant());
		}

		protected override void OnSetProperty(MapDocument document)
		{
			_comboBox.Items.Clear();
			if (Property != null)
			{
				var options = GetSortedOptions().ToList();
				_comboBox.Items.AddRange(options.Select(x => x.DisplayText()).OfType<object>().ToArray());
				var index = options.FindIndex(x => String.Equals(x.Key, PropertyValue, StringComparison.InvariantCultureIgnoreCase));
				if (index >= 0)
				{
					_comboBox.SelectedIndex = index;
					return;
				}
			}
			_comboBox.Text = PropertyValue;
		}
	}
}