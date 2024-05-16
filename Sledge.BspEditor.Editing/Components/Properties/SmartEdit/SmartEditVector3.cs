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
	public class SmartEditVector3 : SmartEditControl
	{
		private readonly NumericUpDown _xField;
		private readonly NumericUpDown _yField;
		private readonly NumericUpDown _zField;
		public SmartEditVector3()
		{
			_xField = new NumericUpDown() { Width = 80, Minimum = -131072, Maximum = 131072, Increment = 0.25M, DecimalPlaces = 2, Left = 0 };
			_yField = new NumericUpDown() { Width = 80, Minimum = -131072, Maximum = 131072, Increment = 0.25M, DecimalPlaces = 2, Left = 82 };
			_zField = new NumericUpDown() { Width = 80, Minimum = -131072, Maximum = 131072, Increment = 0.25M, DecimalPlaces = 2, Left = 164 };
			_xField.TextChanged += (sender, e) => OnValueChanged();
			_yField.TextChanged += (sender, e) => OnValueChanged();
			_zField.TextChanged += (sender, e) => OnValueChanged();
			Controls.Add(_xField);
			Controls.Add(_yField);
			Controls.Add(_zField);
		}

		public override string PriorityHint => "H";

		public override bool SupportsType(VariableType type)
		{
			return type == VariableType.Vector;
		}

		protected override string GetName()
		{
			return OriginalName;
		}

		protected override string GetValue()
		{
			return $"{_xField.Text} {_yField.Text} {_zField.Text}";
		}

		protected override void OnSetProperty(MapDocument document)
		{
			var split = PropertyValue.Split(' ');
			var x = decimal.Parse(split[0]);
			var y = decimal.Parse(split[1]);
			var z = decimal.Parse(split[2]);
			_xField.Value = x;
			_yField.Value = y;
			_zField.Value = z;
		}
	}
}