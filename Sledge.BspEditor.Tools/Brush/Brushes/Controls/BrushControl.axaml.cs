using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Tools.Brush;

namespace Sledge.BspEditor.Tools;

public partial class BrushControl : UserControl
{
	public delegate void ValuesChangedEventHandler(object sender, IBrush brush);

	public event ValuesChangedEventHandler ValuesChanged;

	protected virtual void OnValuesChanged(IBrush brush)
	{
		ValuesChanged?.Invoke(this, brush);
	}

	protected readonly IBrush Brush;

	private BrushControl()
	{
	}

	protected BrushControl(IBrush brush)
	{
		Brush = brush;
	}
}
