using System;
using System.Drawing;
using Avalonia.Controls;
using Sledge.Common.Shell.Settings;

namespace Sledge.Shell.Settings.Editors
{
	public class DefaultSettingEditor : UserControl, ISettingEditor
	{
		public event EventHandler<SettingKey> OnValueChanged;
		public string Label { get; set; }

		public object Value
		{
			get => _box.Text;
			set => _box.Text = Convert.ToString(value);
		}

		public object Control => this;
		public SettingKey Key { get; set; }

		private readonly TextBox _box;

		public DefaultSettingEditor()
		{
			Width = 400; Height = 30;
			_box = new TextBox();
			Content = _box;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
