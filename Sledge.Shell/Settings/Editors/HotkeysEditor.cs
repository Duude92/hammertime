using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Forms;
using Sledge.Shell.Input;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Settings.Editors
{
	public partial class HotkeysEditor : UserControl, ISettingEditor
	{
		public event EventHandler<SettingKey> OnValueChanged;

		string ISettingEditor.Label { get; set; }

		private HotkeyRegister.HotkeyBindings _bindings;
		private static bool _useDarkMode;

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
			Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
			Oy.Subscribe<bool>("Theme:Changed", (useDark) => UseDarkTheme(useDark));

		}

		private void UpdateHotkeyList()
		{
			// Technically a hack, but meh we're going to be using a singleton here anyway because we're lazy.
			// The whole thing's internal so stop judging me
			var register = BaseForm.HotkeyRegister;

			HotkeyList.BeginUpdate();
			var idx = HotkeyList.SelectedIndices.Count == 0 ? 0 : HotkeyList.SelectedIndices[0];

			HotkeyList.Items.Clear();
			foreach (var hotkey in FilterHotkeys(register.GetHotkeys(), FilterBox.Text).OrderBy(x => x.Name))
			{
				var binding = _bindings.ContainsKey(hotkey.ID) ? _bindings[hotkey.ID] : hotkey.DefaultHotkey;
				HotkeyList.Items.Add(new ListViewItem(new[] { hotkey.Name, hotkey.Description, binding }) { Tag = hotkey });
			}

			HotkeyList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

			if (idx >= 0 && idx < HotkeyList.Items.Count) HotkeyList.Items[idx].Selected = true;
			HotkeyList.EndUpdate();

			HotkeyActionList.BeginUpdate();
			idx = HotkeyActionList.SelectedIndex;
			HotkeyActionList.Items.Clear();
			foreach (var hotkey in register.GetHotkeys().OrderBy(x => x.Name))
			{
				HotkeyActionList.Items.Add(new HotkeyWrapper(hotkey));
			}
			if (idx < 0 || idx >= HotkeyActionList.Items.Count) idx = HotkeyActionList.Items.Count - 1;
			HotkeyActionList.SelectedIndex = idx;
			HotkeyActionList.EndUpdate();
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
			e.SuppressKeyPress = true;
			e.Handled = true;
			HotkeyCombination.Text = KeyboardState.KeysToString(e.KeyData);
		}

		private void HotkeySetButtonClicked(object sender, EventArgs e)
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

		private void HotkeyUnsetButtonClicked(object sender, EventArgs e)
		{
			if (HotkeyActionList.SelectedIndex < 0) return;

			var def = ((HotkeyWrapper)HotkeyActionList.SelectedItem).Hotkey;
			_bindings[def.ID] = "";
			HotkeyCombination.Text = "";

			OnValueChanged?.Invoke(this, Key);
			UpdateHotkeyList();
		}

		private void HotkeyResetButtonClicked(object sender, EventArgs e)
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

		private void HotkeyListSelectionChanged(object sender, EventArgs e)
		{
			if (HotkeyList.SelectedItems.Count == 1)
			{
				var hk = (IHotkey)HotkeyList.SelectedItems[0].Tag;
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
			if (e.KeyCode == Keys.Delete && HotkeyList.SelectedItems.Count == 1)
			{
				DeleteHotkey((IHotkey)HotkeyList.SelectedItems[0].Tag);
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

		private void UpdateFilter(object sender, EventArgs e)
		{
			UpdateHotkeyList();
		}
		private void UseDarkTheme(bool dark)
		{
			_useDarkMode = dark;
			BackColor = dark ? Color.DimGray : SystemColors.Control;
			ForeColor = Color.Black;
			HotkeyList.BackColor = BackColor;
			HotkeyList.ForeColor = ForeColor;

			foreach (Control c in HotkeyList.Controls)
			{
				c.BackColor = BackColor;
				c.ForeColor = ForeColor;
			}
		}

		private void HotkeyList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			if (_useDarkMode)
			{
				e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);
				e.Graphics.DrawRectangle(Pens.DimGray, e.Bounds);
			}
			e.DrawText();
		}

		private void HotkeyList_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			return;
		}

		private void HotkeyList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			if (_useDarkMode)
			{
				e.Graphics.FillRectangle(Brushes.Gray, e.Bounds);
				e.Graphics.DrawRectangle(Pens.DarkGray, e.Bounds);

			}
			if (e.Item.Selected)
			{
				if (_useDarkMode)
				{
					e.Graphics.FillRectangle(Brushes.DimGray, e.Bounds);
				}
				else
				{
					e.Graphics.FillRectangle(Brushes.DarkGray, e.Bounds);
				}
			}
			e.DrawText();
		}
	}
}
