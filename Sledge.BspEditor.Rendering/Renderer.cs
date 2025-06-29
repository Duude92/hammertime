﻿using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.ChangeHandlers;
using Sledge.Common.Shell.Settings;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;

namespace Sledge.BspEditor.Rendering
{
	/// <summary>
	/// Bootstraps a renderer instance
	/// </summary>
	[Export(typeof(ISettingsContainer))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class Renderer : ISettingsContainer
	{
		[Import] private Lazy<EngineInterface> _engine;

		// Renderer settings
		[Setting] public static Color PerspectiveBackgroundColour { get; set; } = Color.Black;
		[Setting] public static Color OrthographicBackgroundColour { get; set; } = Color.Black;

		[Setting] public static Color FractionalGridLineColour { get; set; } = Color.FromArgb(32, 32, 32);
		[Setting] public static Color StandardGridLineColour { get; set; } = Color.FromArgb(75, 75, 75);
		[Setting] public static Color PrimaryGridLineColour { get; set; } = Color.FromArgb(115, 115, 115);
		[Setting] public static Color SecondaryGridLineColour { get; set; } = Color.FromArgb(100, 46, 0);
		[Setting] public static Color AxisGridLineColour { get; set; } = Color.FromArgb(0, 100, 100);
		[Setting] public static Color BoundaryGridLineColour { get; set; } = Color.Red;
		[Setting("UnfocusedViewportTargetFps")] private int _targetFps { get; set; } = 10;
		[Setting] private static MSAA_OPTION MSAAoption { get; set; } = MSAA_OPTION.MSAA_1X;
		[Setting] public static float GizmoScale { get; set; } = 1.0f;

		// Settings container

		public string Name => "Sledge.BspEditor.Rendering.Renderer";
		public bool ValuesLoaded { get; private set; } = false;

		public IEnumerable<SettingKey> GetKeys()
		{
			yield return new SettingKey("Rendering", "PerspectiveBackgroundColour", typeof(Color));
			yield return new SettingKey("Rendering", "OrthographicBackgroundColour", typeof(Color));
			yield return new SettingKey("Rendering", "UnfocusedViewportTargetFps", typeof(int));
			yield return new SettingKey("Rendering", "MSAAoption", typeof(MSAA_OPTION));
			yield return new SettingKey("Rendering", "GizmoScale", typeof(decimal));


			yield return new SettingKey("Rendering/Grid", "FractionalGridLineColour", typeof(Color));
			yield return new SettingKey("Rendering/Grid", "StandardGridLineColour", typeof(Color));
			yield return new SettingKey("Rendering/Grid", "PrimaryGridLineColour", typeof(Color));
			yield return new SettingKey("Rendering/Grid", "SecondaryGridLineColour", typeof(Color));
			yield return new SettingKey("Rendering/Grid", "AxisGridLineColour", typeof(Color));
			yield return new SettingKey("Rendering/Grid", "BoundaryGridLineColour", typeof(Color));
		}

		public void LoadValues(ISettingsStore store)
		{
			store.LoadInstance(this);
			_engine.Value.SetClearColour(CameraType.Perspective, PerspectiveBackgroundColour);
			_engine.Value.SetClearColour(CameraType.Orthographic, OrthographicBackgroundColour);
			_engine.Value.InactiveTargetFps = Math.Max(_targetFps, 1);
			_engine.Value.SetMSAA((int)MSAAoption);
			if (GizmoScale < .1f) GizmoScale = .1f;
			Oy.Publish("Render:Gizmos:ScaleChanged", GizmoScale);

			ValuesLoaded = true;
		}

		public void StoreValues(ISettingsStore store)
		{
			store.StoreInstance(this);
		}
		private enum MSAA_OPTION
		{
			MSAA_1X ,
			MSAA_2X ,
			MSAA_4X ,
			MSAA_8X ,
		}
	}
}
