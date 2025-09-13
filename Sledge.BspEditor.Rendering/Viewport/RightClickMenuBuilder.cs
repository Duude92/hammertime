using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Rendering.Cameras;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sledge.BspEditor.Rendering.Viewport
{
	public class RightClickMenuBuilder
	{
		public ViewportEvent Event { get; }
		public MapViewport Viewport { get; }
		public bool Intercepted { get; set; }
		private List<Control> Items { get; }
		public bool IsEmpty => Items.Count == 0;

		public RightClickMenuBuilder(MapViewport viewport, ViewportEvent viewportEvent)
		{
			Event = viewportEvent;
			Viewport = viewport;
			Items = new List<Control>
			{
				CommandItem("BspEditor:Edit:Paste", new {AxisLock = (viewport.Viewport.Camera is OrthographicCamera camera)?
				camera.ViewType == OrthographicCamera.OrthographicType.Top?"Z":
				camera.ViewType == OrthographicCamera.OrthographicType.Front?"X":"Y":"3D" }),
				CommandItem("BspEditor:Edit:PasteSpecial"),
				new MenuItem{Header = "-"},
				CommandItem("BspEditor:Edit:Undo"),
				CommandItem("BspEditor:Edit:Redo")
			};
		}

		public MenuItem CreateCommandItem(string commandId, object parameters = null)
		{
			return CommandItem(commandId, parameters);
		}

		public MenuItem AddCommand(string commandId, object parameters = null)
		{
			var mi = CreateCommandItem(commandId, parameters);
			Items.Add(mi);
			return mi;
		}

		public MenuItem AddCallback(string description, Action callback)
		{
			var mi = new MenuItem { Header = description };
			mi.Click += (s, e) => callback();
			Items.Add(mi);
			return mi;
		}

		public MenuItem AddSeparator()
		{
			var mi = new MenuItem { Header = "-" };
			Items.Add(mi);
			return mi;
		}

		public MenuItem AddGroup(string description)
		{
			var g = new MenuItem { Header = description };
			Items.Add(g);
			return g;
		}

		public void Add(params MenuItem[] items)
		{
			Items.AddRange(items);
		}

		public void Clear()
		{
			Items.Clear();
		}

		public void Populate(MenuFlyout menu)
		{
			menu.Items.Clear();
			foreach (var command in Items)
			{
				menu.Items.Add(command);
			}
		}

		private MenuItem CommandItem(string commandID, object parameters = null)
		{

			var register = Common.Container.Get<Shell.Registers.CommandRegister>();
			var cmd = register.Get(commandID);
			var mi = new MenuItem
			{

				Header = cmd == null ? commandID : cmd.Name,
			};
			mi.Click += (object sender, RoutedEventArgs e) => Oy.Publish("Command:Run", new CommandMessage(commandID, parameters));
			return mi;
		}
	}
}