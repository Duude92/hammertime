using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// A control that shows a number of buttons.
	/// </summary>
	public class QuickFormButtonSet : QuickFormItem
	{
	    public override object Value => null;

	    public QuickFormButtonSet(IEnumerable<(string, DialogResult, Action)> buttons)
		{
		    FlowDirection = Avalonia.Media.FlowDirection.RightToLeft;
			LastChildFill = false;

		    // Add in reverse since the direction is RTL
		    foreach (var b in buttons.Reverse())
		    {
		        var btn = new Button
		        {
		            Content = b.Item1,
					MinHeight = 0,
					MinWidth = 80,
					FlowDirection = Avalonia.Media.FlowDirection.RightToLeft,
					
					//DialogResult = b.Item2
		        };

		        btn.Click += (s, e) => b.Item3();
		        Controls.Add(btn);
		    }
		}
	}
}
