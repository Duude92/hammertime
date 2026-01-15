
using Avalonia.Controls;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// A control that shows a checkbox.
	/// </summary>
	public class QuickFormCheckBox : QuickFormItem
	{
	    public override object Value => _checkBox.IsChecked;

        private readonly CheckBox _checkBox;

	    public QuickFormCheckBox(string text, bool isChecked)
		{
	        _checkBox = new CheckBox
	        {
	            Content = text,
	            IsChecked = isChecked
	        };

		    var margin = _checkBox.Margin + new Avalonia.Thickness(LabelWidth+6,0,0,0);
		    _checkBox.Margin = margin;

            Controls.Add(_checkBox);
		}
	}
}
