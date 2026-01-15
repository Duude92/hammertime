using Avalonia.Controls;
using System;

namespace Sledge.QuickForms.Items
{
    /// <summary>
    /// A control that shows a text box.
    /// </summary>
    public class QuickFormTextBox : QuickFormItem
    {
        public override object Value => _textBox.Text;

        private readonly Label _label;
        private readonly TextBox _textBox;

        public QuickFormTextBox(string text, string value)
        {
            _label = new Label
            {
                Content = text,
                //AutoSize = true,
                //MinimumSize = new Size(LabelWidth, 0),
                //MaximumSize = new Size(LabelWidth, 1000),
                //TextAlign = ContentAlignment.MiddleRight,
                //Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Width = LabelWidth,
                MinHeight = 0,
                MaxHeight = 1000
            };
            _textBox = new TextBox
            {
                Text = value,
                IsEnabled = true

            };

            Controls.Add(_label);
            Controls.Add(_textBox);
        }
        public override void OnResize(EventArgs eventargs)
        {
            var width = (Parent as Panel).DesiredSize.Width;
            _textBox.Width = width - _label.Width - _label.Margin.Left - _textBox.Margin.Left;
            base.OnResize(eventargs);
        }
    }
}
