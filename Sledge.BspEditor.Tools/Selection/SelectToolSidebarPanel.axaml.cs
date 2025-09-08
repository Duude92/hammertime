using Avalonia.Controls;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Tools.Selection;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using System;
using System.ComponentModel.Composition;
using Sledge.Shell;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;
[Export(typeof(ISidebarComponent))]
[OrderHint("F")]
[AutoTranslate]
public partial class SelectToolSidebarPanel : UserControl, ISidebarComponent, IManualTranslate
{
	public string Title { get; set; } = "Selection Tool";
	public object Control => this;

	private WeakReference<MapDocument> _activeDocument;

	public SelectToolSidebarPanel()
	{
		InitializeComponent();

		Oy.Subscribe<String>("SelectTool:TransformationModeChanged", x =>
		{
			if (Enum.TryParse(x, out SelectionBoxDraggableState.TransformationMode mode))
			{
				TransformationToolChanged(mode);
			}
		});

		Oy.Subscribe<String>("SelectTool:SetShow3DWidgets", x =>
		{
			Show3DWidgetsCheckbox.IsChecked = x == "1";
		});
		Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
		TranslateModeCheckbox.IsCheckedChanged += TranslateModeChecked;
		RotateModeCheckbox.IsCheckedChanged += RotateModeChecked;
		SkewModeCheckbox.IsCheckedChanged += SkewModeChecked;
	}

	public void Translate(ITranslationStringProvider strings)
	{
		return;
		var prefix = GetType().FullName;
		this.InvokeLater(() =>
		{
			Title = strings.GetString(prefix, "Title");
			lblMode.Content = strings.GetString(prefix, "Mode");
			TranslateModeCheckbox.Content = strings.GetString(prefix, "Translate");
			RotateModeCheckbox.Content = strings.GetString(prefix, "Rotate");
			SkewModeCheckbox.Content = strings.GetString(prefix, "Skew");
			Show3DWidgetsCheckbox.Content = strings.GetString(prefix, "Show3DWidgets");
			lblActions.Content = strings.GetString(prefix, "Actions");
			MoveToWorldButton.Content = strings.GetString(prefix, "MoveToWorld");
			MoveToEntityButton.Content = strings.GetString(prefix, "TieToEntity");
		});
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out SelectTool _);
	}

	private SelectionBoxDraggableState.TransformationMode _selectedType;

	public void TransformationToolChanged(SelectionBoxDraggableState.TransformationMode tt)
	{
		_selectedType = tt;
		SetCheckState();
	}
	private delegate void SetCheckStateCallback(CheckBox checkBox, bool state);
	private void SetCheckState(CheckBox checkBox, bool state)
	{
		//if (checkBox.InvokeRequired)
		//{
		//	SetCheckStateCallback callback = new SetCheckStateCallback(SetCheckState);
		//	this.Invoke(callback, new object[] { checkBox, state });
		//}
		//else
		//{
		//	checkBox.IsChecked = state;
		//}
		this.Invoke(() => checkBox.IsChecked = state);
	}

	private void SetCheckState()
	{
		if (_selectedType == SelectionBoxDraggableState.TransformationMode.Resize)
		{
			SetCheckState(RotateModeCheckbox, false);
			SetCheckState(SkewModeCheckbox, false);
			SetCheckState(TranslateModeCheckbox, true);
		}
		else if (_selectedType == SelectionBoxDraggableState.TransformationMode.Rotate)
		{
			SetCheckState(TranslateModeCheckbox, false);
			SetCheckState(SkewModeCheckbox, false);
			SetCheckState(RotateModeCheckbox, true);
		}
		else if (_selectedType == SelectionBoxDraggableState.TransformationMode.Skew)
		{
			SetCheckState(RotateModeCheckbox, false);
			SetCheckState(TranslateModeCheckbox, false);
			SetCheckState(SkewModeCheckbox, true);
		}
	}

	private void TranslateModeChecked(object sender, RoutedEventArgs e)
	{
		if (TranslateModeCheckbox.IsChecked.Value && _selectedType != SelectionBoxDraggableState.TransformationMode.Resize)
			Oy.Publish("SelectTool:TransformationModeChanged", "Resize");
		else
			SetCheckState();
	}

	private void RotateModeChecked(object sender, RoutedEventArgs e)
	{
		if (RotateModeCheckbox.IsChecked.Value && _selectedType != SelectionBoxDraggableState.TransformationMode.Rotate)
			Oy.Publish("SelectTool:TransformationModeChanged", "Rotate");
		else
			SetCheckState();
	}

	private void SkewModeChecked(object sender, RoutedEventArgs e)
	{
		if (SkewModeCheckbox.IsChecked.Value && _selectedType != SelectionBoxDraggableState.TransformationMode.Skew)
			Oy.Publish("SelectTool:TransformationModeChanged", "Skew");
		else
			SetCheckState();
	}

	private void Show3DWidgetsChecked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("SelectTool:Show3DWidgetsChanged", Show3DWidgetsCheckbox.IsChecked.Value ? "1" : "0");
	}

	private void MoveToWorldButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Command:Run", new CommandMessage("BspEditor:Tools:MoveToWorld"));
	}

	private void TieToEntityButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Command:Run", new CommandMessage("BspEditor:Tools:TieToEntity"));
	}
	private void KeepEntityAngleChecked(object sender, RoutedEventArgs e)
	{
		_activeDocument.TryGetTarget(out var doc);
		var flags = doc.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
		flags.KeepRotationAngle = keepEntityAngle.IsChecked.Value;
		doc.Map.Data.Replace(flags);
	}
	private void DocumentActivated(IDocument document)
	{
		var md = document as MapDocument;

		_activeDocument = new WeakReference<MapDocument>(md);
	}
}
