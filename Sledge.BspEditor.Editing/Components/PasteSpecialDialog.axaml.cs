using Avalonia.Controls;
using Avalonia.Interactivity;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using System;
using System.Numerics;
using System.Windows.Forms;

namespace Sledge.BspEditor.Editing;

public partial class PasteSpecialDialog : Window, IManualTranslate
{
	public enum PasteSpecialStartPoint
	{
		Origin,
		CenterOriginal,
		CenterSelection
	}

	public enum PasteSpecialGrouping
	{
		None,
		Individual,
		All
	}

	private readonly Box _source;

	public int NumberOfCopies
	{
		get => (int)NumCopies.Value;
		set => NumCopies.Value = value;
	}

	public PasteSpecialStartPoint StartPoint
	{
		get
		{
			if (StartOrigin.IsChecked.Value) return PasteSpecialStartPoint.Origin;
			if (StartOriginal.IsChecked.Value) return PasteSpecialStartPoint.CenterOriginal;
			return PasteSpecialStartPoint.CenterSelection;
		}
		set
		{
			switch (value)
			{
				case PasteSpecialStartPoint.Origin:
					StartOrigin.IsChecked = true;
					break;
				case PasteSpecialStartPoint.CenterOriginal:
					StartOriginal.IsChecked = true;
					break;
				case PasteSpecialStartPoint.CenterSelection:
					StartSelection.IsChecked = true;
					break;
			}
		}
	}

	public PasteSpecialGrouping Grouping
	{
		get
		{
			if (GroupNone.IsChecked.Value) return PasteSpecialGrouping.None;
			if (GroupIndividual.IsChecked.Value) return PasteSpecialGrouping.Individual;
			return PasteSpecialGrouping.All;
		}
		set
		{
			switch (value)
			{
				case PasteSpecialGrouping.None:
					GroupNone.IsChecked = true;
					break;
				case PasteSpecialGrouping.Individual:
					GroupIndividual.IsChecked = true;
					break;
				case PasteSpecialGrouping.All:
					GroupAll.IsChecked = true;
					break;
			}
		}
	}

	public Vector3 AccumulativeOffset
	{
		get => new Vector3((float)OffsetX.Value, (float)OffsetY.Value, (float)OffsetZ.Value);
		set
		{
			OffsetX.Value = (decimal)value.X;
			OffsetY.Value = (decimal)value.Y;
			OffsetZ.Value = (decimal)value.Z;
		}
	}

	public Vector3 AccumulativeRotation
	{
		get => new Vector3((float)RotationX.Value, (float)RotationY.Value, (float)RotationZ.Value);
		set
		{
			RotationX.Value = (decimal)value.X;
			RotationY.Value = (decimal)value.Y;
			RotationZ.Value = (decimal)value.Z;
		}
	}

	public bool MakeEntitiesUnique
	{
		get => UniqueEntityNames.IsChecked.Value;
		set => UniqueEntityNames.IsChecked = value;
	}

	public bool PrefixEntityNames
	{
		get => PrefixEntityNamesCheckbox.IsChecked.Value;
		set
		{
			PrefixEntityNamesCheckbox.IsChecked = value;
			EntityPrefix.IsEnabled = PrefixEntityNamesCheckbox.IsChecked.Value;
		}
	}

	public string EntityNamePrefix
	{
		get => EntityPrefix.Text;
		set => EntityPrefix.Text = value;
	}

	private static decimal _lastNumCopies = 1;
	private static decimal _lastXOffset = 0;
	private static decimal _lastYOffset = 0;
	private static decimal _lastZOffset = 0;
	private static decimal _lastXRotation = 0;
	private static decimal _lastYRotation = 0;
	private static decimal _lastZRotation = 0;

	public PasteSpecialDialog(Box source)
	{
		_source = source;
		InitializeComponent();
		EntityPrefix.IsEnabled = PrefixEntityNamesCheckbox.IsChecked.Value;

		ZeroOffsetXButton.Click += (sender, e) => OffsetX.Value = 0;
		ZeroOffsetYButton.Click += (sender, e) => OffsetY.Value = 0;
		ZeroOffsetZButton.Click += (sender, e) => OffsetZ.Value = 0;

		SourceOffsetXButton.Click += (sender, e) => OffsetX.Value = (decimal)_source.Width;
		SourceOffsetYButton.Click += (sender, e) => OffsetY.Value = (decimal)_source.Length;
		SourceOffsetZButton.Click += (sender, e) => OffsetZ.Value = (decimal)_source.Height;

		ZeroRotationXButton.Click += (sender, e) => RotationX.Value = 0;
		ZeroRotationYButton.Click += (sender, e) => RotationY.Value = 0;
		ZeroRotationZButton.Click += (sender, e) => RotationZ.Value = 0;

		PrefixEntityNamesCheckbox.IsCheckedChanged += (sender, e) => EntityPrefix.IsEnabled = PrefixEntityNamesCheckbox.IsChecked.Value;

		NumCopies.Value = _lastNumCopies;
		OffsetX.Value = _lastXOffset;
		OffsetY.Value = _lastYOffset;
		OffsetZ.Value = _lastZOffset;
		RotationX.Value = _lastXRotation;
		RotationY.Value = _lastYRotation;
		RotationZ.Value = _lastZRotation;
	}

	public void Translate(ITranslationStringProvider strings)
	{
		return;
		//var prefix = GetType().FullName;
		//this.InvokeLater(() =>
		//{
		//	Text = strings.GetString(prefix, "Title");
		//	lblCopies.Text = strings.GetString(prefix, "NumberOfCopies");
		//	grpStartPoint.Text = strings.GetString(prefix, "StartPoint");
		//	StartOrigin.Text = strings.GetString(prefix, "StartAtOrigin");
		//	StartOriginal.Text = strings.GetString(prefix, "StartAtCenterOfOriginal");
		//	StartSelection.Text = strings.GetString(prefix, "StartAtCenterOfSelection");
		//	grpGrouping.Text = strings.GetString(prefix, "Grouping");
		//	GroupNone.Text = strings.GetString(prefix, "NoGrouping");
		//	GroupIndividual.Text = strings.GetString(prefix, "GroupIndividual");
		//	GroupAll.Text = strings.GetString(prefix, "GroupAll");
		//	grpOffset.Text = strings.GetString(prefix, "Offset");
		//	grpRotation.Text = strings.GetString(prefix, "Rotation");
		//	UniqueEntityNames.Text = strings.GetString(prefix, "MakeEntityNamesUnique");
		//	PrefixEntityNamesCheckbox.Text = strings.GetString(prefix, "PrevixEntityNames");
		//	OkButton.Text = strings.GetString(prefix, "OK");
		//	CancelButton.Text = strings.GetString(prefix, "Cancel");
		//});
	}
	protected override void OnLoaded(RoutedEventArgs e)
	{
		NumCopies.Focus();
		//NumCopies.Select(0, NumCopies.Text.Length);

		base.OnLoaded(e);
	}

	private void OkButtonClicked(object? sender, RoutedEventArgs e)
	{
		_lastNumCopies = NumCopies.Value.GetValueOrDefault();
		_lastXOffset = OffsetX.Value.GetValueOrDefault();
		_lastYOffset = OffsetY.Value.GetValueOrDefault();
		_lastZOffset = OffsetZ.Value.GetValueOrDefault();
		_lastXRotation = RotationX.Value.GetValueOrDefault();
		_lastYRotation = RotationY.Value.GetValueOrDefault();
		_lastZRotation = RotationZ.Value.GetValueOrDefault();
		Close(DialogResult.OK);
	}
}