using Sledge.Common.Shell.Settings;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Sledge.BspEditor.Rendering.Viewport
{
	[Export(typeof(ISettingsContainer))]
	internal class ViewportSettingContainer : ISettingsContainer
	{
		public string Name => "Sledge.BspEditor.Rendering.Viewport.ViewportSettingContainer";

		public bool ValuesLoaded { get; private set; } = false;

		[Setting("ForwardKey")] public Keys ForwardKey = Keys.W;
		[Setting("BaskwardKey")] public Keys BackwardKey = Keys.S;
		[Setting("LeftMoveKey")] public Keys LeftKey = Keys.A;
		[Setting("RightMoveKey")] public Keys RightKey = Keys.D;
		[Setting("UpMoveKey")] public Keys UpKey = Keys.Q;
		[Setting("DownMoveKey")] public Keys DownKey = Keys.E;
		[Setting("PanRightKey")] public Keys PanRightKey = Keys.Right;
		[Setting("PanLeftKey")] public Keys PanLeftKey = Keys.Left;
		[Setting("TiltUpKey")] public Keys TiltUpKey = Keys.Up;
		[Setting("TiltDownKey")] public Keys TiltDownKey = Keys.Down;
		[Setting("FreeLookKey")] public Keys FreeLookKey = Keys.Z;

		public IEnumerable<SettingKey> GetKeys()
		{
			yield return new SettingKey("Controls", "ForwardKey", typeof(Keys));
			yield return new SettingKey("Controls", "BaskwardKey", typeof(Keys));
			yield return new SettingKey("Controls", "LeftMoveKey", typeof(Keys));
			yield return new SettingKey("Controls", "RightMoveKey", typeof(Keys));
			yield return new SettingKey("Controls", "UpMoveKey", typeof(Keys));
			yield return new SettingKey("Controls", "DownMoveKey", typeof(Keys));
			yield return new SettingKey("Controls", "PanRightKey", typeof(Keys));
			yield return new SettingKey("Controls", "PanLeftKey", typeof(Keys));
			yield return new SettingKey("Controls", "TiltUpKey", typeof(Keys));
			yield return new SettingKey("Controls", "TiltDownKey", typeof(Keys));
			yield return new SettingKey("Controls", "FreeLookKey", typeof(Keys));
		}

		public void LoadValues(ISettingsStore store)
		{
			store.LoadInstance(this);
			ValuesLoaded = true;
		}

		public void StoreValues(ISettingsStore store)
		{
			store.StoreInstance(this);
		}
	}

}
