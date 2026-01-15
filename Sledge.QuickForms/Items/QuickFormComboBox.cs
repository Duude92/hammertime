using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a ComboBox control.
    /// </summary>
    public class QuickFormComboBox : QuickFormItem
    {
        public override object Value => _comboBox.SelectedItem;

        private readonly Label _label;
        private readonly ComboBox _comboBox;

        public QuickFormComboBox(string text, IEnumerable<object> items)
        {
            _label = new Label
            {
                Content = text,
                //AutoSize = true,
                //MinimumSize = new Size(LabelWidth, 0),
                //MaximumSize = new Size(LabelWidth, 1000),
                //TextAlign = ContentAlignment.MiddleRight,
                //Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            };
            _comboBox = new ComboBox
            {
                //DropDownStyle = ComboBoxStyle.DropDownList
            };
            _comboBox.Items.Append(items.ToArray());
            _comboBox.SelectedIndex = 0;

            Controls.Add(_label);
            Controls.Add(_comboBox);
        }
        public override void OnResize(EventArgs eventargs)
        {
            _comboBox.Width = Width - _label.Width - _label.Margin.Left - _comboBox.Margin.Left;
            base.OnResize(eventargs);
        }
    }
}