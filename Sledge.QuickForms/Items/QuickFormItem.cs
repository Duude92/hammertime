using Avalonia;
using Avalonia.Controls;
using System;

namespace Sledge.QuickForms.Items
{
	/// <summary>
	/// Abstract base class for all form items.
	/// </summary>
	public abstract class QuickFormItem : WrapPanel
	{
		protected const int LabelWidth = 80;

		public abstract object Value { get; }
		public Controls Controls => this.Children;
		protected QuickFormItem()
		{
			FlowDirection = Avalonia.Media.FlowDirection.LeftToRight;
			//AutoSize = true;
			//AutoSizeMode = AutoSizeMode.GrowAndShrink;
			//Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			Margin = new Thickness { };
		}
		public virtual void OnResize(EventArgs eventargs) { }
	}
}
