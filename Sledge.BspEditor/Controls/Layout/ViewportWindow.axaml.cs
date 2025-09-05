using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Controls.Layout;

namespace Sledge.BspEditor.Controls.Layout;

public partial class ViewportWindow : Window
{
	public MapDocumentContainer MapDocumentContainer { get; }

	public ViewportWindow(MapDocumentControlWindowConfiguration config)
	{
		InitializeComponent();

		MapDocumentContainer = new MapDocumentContainer(config.WindowID)
		{
			//Dock = DockStyle.Fill
		};
		MainPanel.Children.Add(MapDocumentContainer);

		SetConfiguration(config);
	}

	public void SetConfiguration(MapDocumentControlWindowConfiguration config)
	{
		MapDocumentContainer.Table.Configuration = config.Configuration;
		MapDocumentContainer.Table.RowSizes = config.RowSizes;
		MapDocumentContainer.Table.ColumnSizes = config.ColumnSizes;

		Position = new PixelPoint(config.Size.Location.X, config.Size.Location.Y);
		Width = config.Size.Width;
		Height = config.Size.Height;
		//Size = config.Size.Size;
		WindowState = config.Maximised ? WindowState.Maximized : WindowState.Normal;
	}
}