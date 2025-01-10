using System;
using System.Windows.Forms;

namespace Sledge.BspEditor.Environment.Controls
{
	public class GridUpDown : NumericUpDown
	{
		private int MyIncrement { get => (int)Math.Log((double)this.Value, 2); set => SetNewValue(value); }
		public override void UpButton()
		{
			MyIncrement++;
		}
		public override void DownButton()
		{
			MyIncrement--;
		}
		private void SetNewValue(decimal inputValue)
		{
			var value = Math.Max(Math.Min(inputValue, 10), -2);
			value = (decimal)Math.Pow(2, (double)value);
			if (this.Maximum < value || value < this.Minimum) return;

			this.Value = value;
		}
	}
}
