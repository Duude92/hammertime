using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Sledge.Common.Shell.Settings;
using System;

namespace Sledge.Shell.Settings.Editors;

public partial class SliderEditor : UserControl, ISettingEditor
{
	private decimal _actualToSliderMultiplier = 1;
	private SettingKey _key;
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label
	{
		get => Label.Content as string;
		set => Label.Content = value;
	}

	public object Value
	{
		get => NumericBox.Value;
		set
		{
			var d = Convert.ToDecimal(value);
			Slider.Value = Math.Min(Slider.Maximum, Math.Max(Slider.Minimum, ActualToSlider(d)));
			NumericBox.Value = Math.Min(NumericBox.Maximum, Math.Max(NumericBox.Minimum, d));
		}
	}

	public object Control => this;

	public SettingKey Key
	{
		get => _key;
		set
		{
			_key = value;
			SetHint(value?.EditorHint);
			SetType(value?.Type);
		}
	}

	public SliderEditor()
	{
		InitializeComponent();
	}

	private void SetType(Type type)
	{
		if (type == null) type = typeof(decimal);
		NumericBox.FormatString = type == typeof(int) ? "0" : "0.00";
	}

	private void SetHint(string hint)
	{
		var spl = (hint ?? "").Split(',');

		if (spl.Length > 4 && decimal.TryParse(spl[4], out var mul)) _actualToSliderMultiplier = mul;

		if (spl.Length > 0 && decimal.TryParse(spl[0], out var min))
		{
			Slider.Minimum = ActualToSlider(min);
			NumericBox.Minimum = min;
		}

		if (spl.Length > 1 && decimal.TryParse(spl[1], out var max))
		{
			Slider.Maximum = ActualToSlider(max);
			NumericBox.Maximum = max;
		}

		if (spl.Length > 2 && decimal.TryParse(spl[2], out var step))
		{
			Slider.SmallChange = ActualToSlider(step);
			NumericBox.Increment = step;
		}

		if (spl.Length > 3 && decimal.TryParse(spl[3], out var step2))
		{
			Slider.LargeChange = ActualToSlider(step2);
		}
	}

	private void NumberChanged(object sender, NumericUpDownValueChangedEventArgs e)
	{
		Slider.Value = Math.Min(Slider.Maximum, Math.Max(Slider.Minimum, ActualToSlider(NumericBox.Value ?? 0)));
		OnValueChanged?.Invoke(this, Key);
	}

	private void SliderChanged(object sender, RangeBaseValueChangedEventArgs e)
	{
		NumericBox.Value = Math.Min(NumericBox.Maximum, Math.Max(NumericBox.Minimum, SliderToActual((int)Slider.Value)));
		OnValueChanged?.Invoke(this, Key);
	}

	private int ActualToSlider(decimal val) => (int)(val * _actualToSliderMultiplier);
	private decimal SliderToActual(int val) => val / _actualToSliderMultiplier;

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
