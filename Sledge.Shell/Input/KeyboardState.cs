using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Input;

namespace Sledge.Shell.Input
{
    /// <summary>
    /// Performs polling on the current keyboard state.
    /// </summary>
    /// Most of this is adapted from:
    /// http://www.switchonthecode.com/tutorials/winforms-accessing-mouse-and-keyboard-state
    public static class KeyboardState
    {
		public static KeyModifiers CurrentModifiers { get; set; } = KeyModifiers.None;
		public static List<Key> CurrentKeys { get; } = new List<Key>();
        private static readonly Dictionary<string, string> KeyStringReplacements;

        static KeyboardState()
        {
            KeyStringReplacements = new Dictionary<string, string>
                                        {
                                            {"Add", "+"},
                                            {"Oemplus", "+"},
                                            {"Subtract", "-"},
                                            {"OemMinus", "-"},
                                            {"Separator", "-"},
                                            {"Decimal", "."},
                                            {"OemPeriod", "."},
                                            {"Divide", "/"},
                                            {"OemQuestion", "/"},
                                            {"Multiply", "*"},
                                            {"OemBackslash", "\\"},
                                            {"Oem5", "\\"},
                                            {"OemCloseBrackets", "]"},
                                            {"Oem6", "]"},
                                            {"OemOpenBrackets", "["},
                                            {"OemPipe", "|"},
                                            {"OemQuotes", "'"},
                                            {"Oem7", "'"},
                                            {"OemSemicolon", ";"},
                                            {"Oem1", ";"},
                                            {"Oemcomma", ","},
                                            {"Oemtilde", "`"},
                                            {"Back", "Backspace"},
                                            {"Return", "Enter"},
                                            {"Next", "PageDown"},
                                            {"Prior", "PageUp"},
                                            {"D1", "1"},
                                            {"D2", "2"},
                                            {"D3", "3"},
                                            {"D4", "4"},
                                            {"D5", "5"},
                                            {"D6", "6"},
                                            {"D7", "7"},
                                            {"D8", "8"},
                                            {"D9", "9"},
                                            {"D0", "0"},
                                            {"Delete", "Del"}
                                        };
        }

		public static bool Ctrl => IsModifierKeyDown(KeyModifiers.Control);
		public static bool Shift => IsModifierKeyDown(KeyModifiers.Shift);
		public static bool Alt => IsModifierKeyDown(KeyModifiers.Alt);
		public static bool CapsLocked => IsKeyToggled(Key.CapsLock);
		public static bool ScrollLocked => IsKeyToggled(Key.Scroll);
		public static bool NumLocked => IsKeyToggled(Key.NumLock);

		private static bool IsModifierKeyDown(KeyModifiers k)
        {
			return (CurrentModifiers & k) == k;
        }


		public static bool IsKeyDown(Key key)
        {
			return CurrentKeys.Contains(key);
            // Key is down if the high bit is 1
        }

		public static bool IsAnyKeyDown(params Key[] keys)
        {
            return keys.Any(IsKeyDown);
        }
		public static bool IsAnyKeyDown(params KeyModifiers[] keys)
		{
			return keys.Any(x => CurrentModifiers.HasFlag(x));
		}

		private static bool IsKeyToggled(Key key)
        {
			return CurrentKeys.Contains(key);

            // Key is toggled if the low bit is 1
        }

        public static string KeysToString(Keys key)
        {
            // KeysConverter seems to ignore the invariant culture, manually replicate the results
            var mods = key & Keys.Modifiers;
            var keycode = key & Keys.KeyCode;
            if (keycode == Keys.None) return "";

            var str = keycode.ToString();
            if (KeyStringReplacements.ContainsKey(str)) str = KeyStringReplacements[str];

            // Modifier order: Ctrl+Alt+Shift+Key
            return (mods.HasFlag(Keys.Control) ? "Ctrl+" : "")
                   + (mods.HasFlag(Keys.Alt) ? "Alt+" : "")
                   + (mods.HasFlag(Keys.Shift) ? "Shift+" : "")
                   + str;
        }
    }
}
