using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Forms;
using Sledge.Shell.Input;
using Sledge.Shell.Registers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sledge.Shell.Settings.Editors;

public partial class HotkeysEditor : UserControl, ISettingEditor
{
	public event EventHandler<SettingKey> OnValueChanged;

	string ISettingEditor.Label { get; set; }

	private HotkeyRegister.HotkeyBindings _bindings;
	public ObservableCollection<HotKeyListItem> Items { get; set; } = new ();

	public object Value
	{
		get => _bindings;
		set
		{
			_bindings = ((HotkeyRegister.HotkeyBindings)value).Clone();
			UpdateHotkeyList();
		}
	}

	public object Control => this;
	public SettingKey Key { get; set; }

	public HotkeysEditor()
	{
		InitializeComponent();
		//Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
		DataContext = this;
	}

	private void UpdateHotkeyList()
	{
		// Technically a hack, but meh we're going to be using a singleton here anyway because we're lazy.
		// The whole thing's internal so stop judging me
		var register = BaseForm.HotkeyRegister;

		var idx = HotkeyList.SelectedItems.Count == 0 ? 0 : HotkeyList.SelectedIndex;

		Items.Clear();
		foreach (var hotkey in FilterHotkeys(register.GetHotkeys(), FilterBox.Text).OrderBy(x => x.Name))
		{
			var binding = _bindings.ContainsKey(hotkey.ID) ? _bindings[hotkey.ID] : hotkey.DefaultHotkey;
			Items.Add(
				new HotKeyListItem()
			{
				Action = hotkey.Name,
				Description = hotkey.Description,
				Hotkey = binding,
				Tag = hotkey
			});
		}

		//HotkeyList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

		if (idx >= 0 && idx < Items.Count)
			HotkeyList.SelectedIndex = idx;
		HotkeyList.SelectedIndex = idx;

		idx = HotkeyActionList.SelectedIndex;
		HotkeyActionList.Items.Clear();
		foreach (var hotkey in register.GetHotkeys().OrderBy(x => x.Name))
		{
			HotkeyActionList.Items.Add(new HotkeyWrapper(hotkey));
		}
		if (idx < 0 || idx >= HotkeyActionList.Items.Count) idx = HotkeyActionList.Items.Count - 1;
		HotkeyActionList.SelectedIndex = idx;
	}

	private IEnumerable<IHotkey> FilterHotkeys(IEnumerable<IHotkey> hotkeys, string filter)
	{
		return String.IsNullOrWhiteSpace(filter) ? hotkeys : hotkeys.Where(IsMatch);

		bool IsMatch(IHotkey h) => h.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
								   h.Description.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0;
	}

	private void DeleteHotkey(IHotkey hk)
	{
		_bindings[hk.ID] = "";
		OnValueChanged?.Invoke(this, Key);
		UpdateHotkeyList();
	}

	private void HotkeyCombinationKeyDown(object sender, KeyEventArgs e)
	{
		//e.SuppressKeyPress = true;
		e.Handled = true;
		HotkeyCombination.Text = KeyboardState.KeysToString(e.Key);
	}

	private void HotkeySetButtonClicked(object sender, RoutedEventArgs e)
	{
		var key = HotkeyCombination.Text;
		if (HotkeyActionList.SelectedIndex < 0 || String.IsNullOrWhiteSpace(key)) return;

		if (_bindings.ContainsValue(key))
		{
			// if (MessageBox.Show(key + " is already assigned to \"" + Hotkeys.GetHotkeyDefinition(conflict.ID) + "\".\n" +
			//                     "Continue anyway?", "Conflict Detected", MessageBoxButtons.YesNo) == DialogResult.No)
			// {
			//     return;
			// }
		}

		var def = ((HotkeyWrapper)HotkeyActionList.SelectedItem).Hotkey;
		_bindings[def.ID] = key;
		HotkeyCombination.Text = "";

		OnValueChanged?.Invoke(this, Key);
		UpdateHotkeyList();
	}

	private void HotkeyUnsetButtonClicked(object sender, RoutedEventArgs e)
	{
		if (HotkeyActionList.SelectedIndex < 0) return;

		var def = ((HotkeyWrapper)HotkeyActionList.SelectedItem).Hotkey;
		_bindings[def.ID] = "";
		HotkeyCombination.Text = "";

		OnValueChanged?.Invoke(this, Key);
		UpdateHotkeyList();
	}

	private void HotkeyResetButtonClicked(object sender, RoutedEventArgs e)
	{
		_bindings.Clear();
		foreach (var hk in BaseForm.HotkeyRegister.GetHotkeys())
		{
			if (!String.IsNullOrWhiteSpace(hk.DefaultHotkey))
			{
				_bindings[hk.ID] = hk.DefaultHotkey;
			}
		}
		OnValueChanged?.Invoke(this, Key);
		UpdateHotkeyList();
	}

	private void HotkeyListSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (HotkeyList.SelectedItems.Count == 1)
		{
			var hk = (IHotkey)(HotkeyList.SelectedItems[0] as HotKeyListItem).Tag;
			var str = _bindings.ContainsKey(hk.ID) ? _bindings[hk.ID] : "";

			HotkeyActionList.SelectedItem = new HotkeyWrapper(hk);
			HotkeyCombination.Text = str;
		}
		else
		{
			HotkeyActionList.SelectedIndex = -1;
			HotkeyCombination.Text = "";
		}
	}

	private void HotkeyListKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Avalonia.Input.Key.Delete && HotkeyList.SelectedItems.Count == 1)
		{
			DeleteHotkey((IHotkey)(HotkeyList.SelectedItems[0] as HotKeyListItem).Tag);
		}
	}

	private class HotkeyWrapper
	{
		public IHotkey Hotkey { get; }

		public HotkeyWrapper(IHotkey hotkey)
		{
			Hotkey = hotkey;
		}

		private bool Equals(HotkeyWrapper other)
		{
			return Equals(Hotkey, other.Hotkey);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((HotkeyWrapper)obj);
		}

		public override int GetHashCode()
		{
			return (Hotkey != null ? Hotkey.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return Hotkey.Name;
		}
	}

	private void UpdateFilter(object sender, TextChangedEventArgs e)
	{
		UpdateHotkeyList();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
	public class HotKeyListItem 
	{
		public string Description { get; set; }
		public string Hotkey { get; set; }
		public string Action { get; set; }
		public IHotkey Tag { get; set; }
	}
}
