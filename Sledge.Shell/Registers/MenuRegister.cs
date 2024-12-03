using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Menu;
using Sledge.Shell.Components;

namespace Sledge.Shell.Registers
{
	/// <summary>
	/// The menu register registers and handles menu and toolbar items
	/// </summary>
	[Export(typeof(IStartupHook))]
	[Export(typeof(IInitialiseHook))]
	internal class MenuRegister : IStartupHook, IInitialiseHook
	{
		// The menu register needs direct access to the shell
		[Import] private Forms.Shell _shell;

		// Store the context (the menu register is one of the few things that should need static access to the context)
		[Import] private IContext _context;

		[ImportMany] private IEnumerable<Lazy<IMenuItemProvider>> _itemProviders;
		[ImportMany] private IEnumerable<Lazy<IMenuMetadataProvider>> _metaDataProviders;

		public Task OnStartup()
		{
			// Register commands as menu items
			foreach (var export in _itemProviders)
			{
				export.Value.MenuItemsChanged += UpdateMenus;
				foreach (var menuItem in export.Value.GetMenuItems())
				{
					Add(menuItem);
				}
			}

			return Task.FromResult(0);
		}

		public async Task OnInitialise()
		{
			// Register metadata providers
			foreach (var md in _metaDataProviders)
			{
				_declaredGroups.AddRange(md.Value.GetMenuGroups());
				_declaredSections.AddRange(md.Value.GetMenuSections());
			}

			_shell.InvokeSync(() =>
			{
				_tree = new VirtualMenuTree(_context, _shell.MenuStrip, _shell.ToolStrip, _declaredSections, _declaredGroups);
				_tree.ResetItems(_menuItems.Values);
			});

			Oy.Subscribe<IContext>("Context:Changed", ContextChanged);
			Oy.Subscribe<object>("Menu:Update", UpdateMenu);
			Oy.Subscribe<bool>("Theme:Changed", (useDark) => _tree.UseDarkTheme = useDark);
		}

		private Task ContextChanged(IContext context)
		{
			return UpdateMenu(context);
		}

		private async Task UpdateMenu(object obj)
		{
			_shell.InvokeLater(_tree.Update);
		}

		// Clear all menus and repopulate them from the menu item providers
		private void UpdateMenus(object sender, EventArgs e)
		{
			_shell.InvokeLater(() =>
			{
				_menuItems.Clear();

				foreach (var export in _itemProviders)
				{
					foreach (var menuItem in export.Value.GetMenuItems())
					{
						Add(menuItem);
					}
				}

				_tree.ResetItems(_menuItems.Values);
			});
		}

		/// <summary>
		/// The list of all menu items by ID
		/// </summary>
		private readonly Dictionary<string, IMenuItem> _menuItems;

		/// <summary>
		/// Sections declared by menu metadata providers. A section is a top-level menu or toolbar.
		/// </summary>
		private readonly List<MenuSection> _declaredSections;

		/// <summary>
		/// Groups declared by menu metadata providers. Groups within a section are separated with lines.
		/// </summary>
		private readonly List<MenuGroup> _declaredGroups;

		private VirtualMenuTree _tree;

		public MenuRegister()
		{
			_menuItems = new Dictionary<string, IMenuItem>();
			_declaredSections = new List<MenuSection>();
			_declaredGroups = new List<MenuGroup>();
		}

		/// <summary>
		/// Add a menu item to the list
		/// </summary>
		/// <param name="menuItem">The menu item to add</param>
		private void Add(IMenuItem menuItem)
		{
			_menuItems[menuItem.ID] = menuItem;
		}

		/// <summary>
		/// A class that handles the tree of menu items
		/// and inserts them into the correct positions.
		/// </summary>
		private class VirtualMenuTree
		{
			private readonly IContext _context;
			private readonly List<MenuSection> _declaredSections;
			private readonly List<MenuGroup> _declaredGroups;
			private bool _useDarkTheme = false;
			public bool UseDarkTheme
			{
				get => _useDarkTheme; set
				{
					_useDarkTheme = value;
					if (value)
					{
						MenuStrip.Renderer = new CustomToolStripRenderer(SystemColors.ControlDarkDark, _backColor);
						return;
					}
					MenuStrip.Renderer = null;
				}
			}
			private Color _systemDarkBackColor = Color.FromArgb(50, 50, 50);
			private Color _backColor = Color.FromArgb(70, 70, 70);

			/// <summary>
			/// The toolstrip containing the toolbars
			/// </summary>
			private ToolStripPanel ToolStrip { get; set; }

			/// <summary>
			/// The menu strip for the top level menus
			/// </summary>
			private MenuStrip MenuStrip { get; set; }

			/// <summary>
			/// The root nodes of the virtual tree
			/// </summary>
			private Dictionary<string, MenuTreeRoot> RootNodes { get; set; }

			public VirtualMenuTree(IContext context, MenuStrip menuStrip, ToolStripPanel toolStrip, List<MenuSection> declaredSections, List<MenuGroup> declaredGroups)
			{
				_context = context;
				_declaredSections = declaredSections;
				_declaredGroups = declaredGroups;
				MenuStrip = menuStrip;
				ToolStrip = toolStrip;
				RootNodes = new Dictionary<string, MenuTreeRoot>();
				Clear();

			}

			public void ResetItems(IEnumerable<IMenuItem> items)
			{
				Clear();

				foreach (var mi in items)
				{
					Add(mi);
				}

				Render();
			}

			private void Render()
			{
				MenuStrip.SuspendLayout();
				MenuStrip.Items.Clear();
				MenuStrip.Items.AddRange(RootNodes.Values.OrderBy(x => x.OrderHint).Select(x => x.MenuMenuItem).OfType<ToolStripItem>().ToArray());
				MenuStrip.ResumeLayout();
				ToolStrip.BeginInit();
				ToolStrip.Controls.Clear();
				foreach (var ts in RootNodes.Values.OrderByDescending(x => x.OrderHint))
				{
					if (ts.ToolStrip.Items.Count > 0) ToolStrip.Join(ts.ToolStrip);
				}
				ToolStrip.EndInit();
				if (UseDarkTheme)
				{
					ToolStrip.BackColor = _systemDarkBackColor;
					ToolStrip.ForeColor = System.Drawing.Color.White;
					foreach (Control item in ToolStrip.Controls)
					{
						item.BackColor = _systemDarkBackColor;
						item.ForeColor = System.Drawing.Color.White;
					}

					MenuStrip.BackColor = _systemDarkBackColor;
					MenuStrip.ForeColor = System.Drawing.Color.White;

					foreach (ToolStripItem item in MenuStrip.Items)
					{
						item.BackColor = _systemDarkBackColor;
						item.ForeColor = Color.White;

						if (item is ToolStripDropDownItem dropDownItem)
						{

							foreach (ToolStripItem dropdownItem in dropDownItem.DropDownItems)
							{
								dropdownItem.BackColor = _backColor;
								if (!dropdownItem.Enabled)
								{
									dropdownItem.ForeColor = Color.DimGray;
								}
							}
						}
					}
				}
			}

			/// <summary>
			/// Add a section to the tree. This will create a top-level menu as well as a toolbar.
			/// </summary>
			private void AddSection(MenuSection ds)
			{
				// Create the root
				var rtn = new MenuTreeRoot(_context, ds.Description, ds);

				// When the menu is closed, push an empty string to the status bar
				rtn.MenuMenuItem.DropDownClosed += (s, a) => { Oy.Publish("Status:Information", ""); };

				// When the menu is opened, update the state of all the menu items in this section
				rtn.MenuMenuItem.DropDownOpening += (s, a) => { rtn.Update(); };

				// Add the node, menu, and toolbar
				RootNodes.Add(ds.Name, rtn);
			}

			/// <summary>
			/// Add an item to the tree. This will add the menu item and the toolbar button if required.
			/// </summary>
			private void Add(IMenuItem item)
			{
				// If the section isn't known, add it to the end
				if (!RootNodes.ContainsKey(item.Section)) AddSection(new MenuSection(item.Section, item.Section, "Z"));

				var root = RootNodes[item.Section];

				root.AddDescendant(item, _declaredGroups);
			}

			public void Clear()
			{
				MenuStrip.Items.Clear();
				ToolStrip.Controls.Clear();
				RootNodes.Clear();

				// Add known sections straight away
				foreach (var ds in _declaredSections.OrderBy(x => x.OrderHint)) AddSection(ds);
			}

			public void Update()
			{
				foreach (var node in RootNodes.Values)
				{
					node.Update();
				}
			}
		}
		public class CustomToolStripRenderer : ToolStripProfessionalRenderer
		{
			private readonly Color _pressedBackColor;
			private readonly Color _backColor;
			private readonly Brush _dropDownEnabledColor = new SolidBrush(Color.Black);
			private readonly Brush _dropDownDisabledColor = new SolidBrush(Color.Gray);

			public CustomToolStripRenderer(Color pressedBackColor, Color backColor)
			{
				_pressedBackColor = pressedBackColor;
				_backColor = backColor;
			}
			protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
			{
				//base.OnRenderItemBackground(e);
				using (var brush = new SolidBrush(Color.Black)) // Example color
				{
					e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
				}
			}
			protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
			{
				//using (var brush = new SolidBrush(Color.Black))
				//{
				//	e.Graphics.FillRectangle(brush, e.ToolStripPanel.ClientRectangle);
				//	e.Graphics.DrawRectangle(SystemPens.Highlight, e.ToolStripPanel.ClientRectangle);
				//}
				base.OnRenderToolStripPanelBackground(e);

			}
			protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
			{
				if (e.ToolStrip is ToolStripDropDown)
				{
					// Custom border for the drop-down menu
					using (var pen = new Pen(ControlPaint.Dark(_backColor), 1)) // Border color
					{
						e.Graphics.DrawRectangle(pen, new Rectangle(Point.Empty, e.ToolStrip.ClientSize - new Size(1, 1)));
					}
				}
				else
				{
					base.OnRenderToolStripBorder(e);
				}
			}
			protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
			{
				return;
				//using (var pen = new Pen(Color.Gold, 1)) // Separator color
				//{
				//	int y = e.ToolStrip.ClientRectangle.Top + e.ToolStrip.ClientRectangle.Height / 2;
				//	e.Graphics.DrawLine(pen, e.ToolStrip.ClientRectangle.Left, y, e.ToolStrip.ClientRectangle.Right, y);
				//}
				//base.OnRenderImageMargin(e);
			}
			protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
			{
				//using (var pen = new Pen(Color.Purple, 1)) // Separator color
				//{
				//	int y = e.ToolStrip.ClientRectangle.Top + e.ToolStrip.ClientRectangle.Height / 2;
				//	e.Graphics.DrawLine(pen, e.ToolStrip.ClientRectangle.Left, y, e.ToolStrip.ClientRectangle.Right, y);
				//}
				base.OnRenderItemImage(e);
			}
			protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
			{

				// Custom separator rendering
				using (var pen = new Pen(ControlPaint.Dark(_backColor), 1)) // Separator color
				{
					int y = e.Item.ContentRectangle.Top + e.Item.ContentRectangle.Height / 2;
					e.Graphics.DrawLine(pen, e.Item.ContentRectangle.Left, y, e.Item.ContentRectangle.Right, y);
				}
			}
			protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
			{
				// Custom background for the entire drop-down
				if (e.ToolStrip is ToolStripDropDown)
				{
					e.ToolStrip.BackColor = _backColor;
					using (var brush = new SolidBrush(_backColor))
					{
						e.Graphics.FillRectangle(brush, e.ToolStrip.DisplayRectangle);
						e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, e.ToolStrip.ClientRectangle);
					}
				}
				//else if (e.ToolStrip is ToolStripDropDownItem)
				//{
				//	using (var brush = new SolidBrush(Color.Yellow))
				//	{
				//		e.Graphics.FillRectangle(brush, e.AffectedBounds);
				//	}
				//}
				else
				{
					base.OnRenderToolStripBackground(e);
				}
			}
			protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
			{
				if (e.Item.Owner is ToolStripDropDown) //Item is drop-down item
				{
					if (e.Item is ToolStripMenuItem menuItem)
					{
						if (!e.Item.Enabled)
						{

							e.Graphics.DrawString(menuItem.Text, e.TextFont, _dropDownDisabledColor, e.TextRectangle);
						}
						else
						{
							e.Graphics.DrawString(menuItem.Text, e.TextFont, _dropDownEnabledColor, e.TextRectangle);
						}
						if (!string.IsNullOrEmpty(menuItem.ShortcutKeyDisplayString)) //Item has shortcut
						{
							SizeF shortcutTextSize = e.Graphics.MeasureString(menuItem.ShortcutKeyDisplayString, e.TextFont);
							float shortcutX = e.Item.ContentRectangle.Right - shortcutTextSize.Width - 10; // 10px padding from the right

							RectangleF shortcutRect = new RectangleF(shortcutX, e.Item.ContentRectangle.Top, shortcutTextSize.Width, shortcutTextSize.Height);
							if (e.Item.Enabled)
							{

								e.Graphics.DrawString(menuItem.ShortcutKeyDisplayString, e.TextFont, _dropDownEnabledColor, shortcutRect);
								return;
							}

							e.Graphics.DrawString(menuItem.ShortcutKeyDisplayString, e.TextFont, _dropDownDisabledColor, shortcutRect);
							return;
						}
					}

				}
				else if (e.Item.Owner is MenuStrip)
				{
					using (Brush textBrush = new SolidBrush(Color.White))
					{
						e.Graphics.DrawString(e.Text, e.TextFont, textBrush, e.TextRectangle);
					}
				}
				else
				{

					base.OnRenderItemText(e);
				}

			}

			protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
			{
				// Check if the item is pressed or selected
				if (e.Item.Pressed)
				{
					// Draw custom background for pressed state
					using (var brush = new SolidBrush(_pressedBackColor))
					{
						e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
					}
				}
				else if (e.Item.Selected)
				{
					// Draw custom background for selected state
					using (var brush = new SolidBrush(Color.DimGray)) // Example color
					{
						e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
					}
				}
				else
				{
					// Draw default background
					base.OnRenderMenuItemBackground(e);
				}
			}
		}

		/// <summary>
		/// A root node of a menu tree
		/// </summary>
		private class MenuTreeRoot : BaseMenuTreeNode
		{
			private MenuSection Section { get; }
			public ToolStrip ToolStrip { get; }

			public override string OrderHint => Section.OrderHint;

			private List<MenuTreeGroup> _toolbarGroups;

			public MenuTreeRoot(IContext context, string text, MenuSection section)
			{
				Section = section;
				MenuMenuItem = new ToolStripMenuItem(text) { Tag = this };
				ToolStrip = new ToolStrip { Tag = this, LayoutStyle = ToolStripLayoutStyle.Flow };
				Context = context;
				_toolbarGroups = new List<MenuTreeGroup>();
			}

			/// <summary>
			/// Add a descendant to this root node. Searches down the path until we find the correct parent
			/// </summary>
			public void AddDescendant(IMenuItem item, List<MenuGroup> declaredGroups)
			{
				// Find the parent node for this item
				// Start at the section root node
				BaseMenuTreeNode node = this;

				// Traverse the path until we get to the target
				var path = (item.Path ?? "").Split('/').Where(x => x.Length > 0).ToList();
				var currentPath = new List<string>();
				foreach (var p in path)
				{
					currentPath.Add(p);

					// If the current node isn't found, add it in
					if (!node.Children.ContainsKey(p))
					{
						var gr = declaredGroups.FirstOrDefault(x => x.Name == p && x.Path == String.Join("/", currentPath) && x.Section == item.Section);
						node.AddChild(p, new MenuTreeTextNode(Context, gr?.Description ?? p, gr));
					}

					node = node.Children[p];
				}

				// Add the node to the parent node
				var group = declaredGroups.FirstOrDefault(x => x.Name == item.Group && x.Path == item.Path && x.Section == item.Section);
				var itemNode = new MenuTreeNode(Context, item, group);
				node.AddChild(item.ID, itemNode);

				// Add to the toolbar as well
				// Items with no icon are never allowed
				if (item.AllowedInToolbar && item.Icon != null)
				{
					AddToolbarItem(itemNode);
				}
			}

			private void AddToolbarItem(BaseMenuTreeNode menuTreeNode)
			{
				if (_toolbarGroups.All(x => x.Group.Name != menuTreeNode.Group.Name))
				{
					_toolbarGroups.Add(new MenuTreeGroup(menuTreeNode.Group));
					_toolbarGroups = _toolbarGroups.OrderBy(x => x.Group.OrderHint).ToList();
				}

				// Insert the item into the correct index
				var groupIndex = _toolbarGroups.FindIndex(x => x.Group.Name == menuTreeNode.Group.Name);

				// Skip to the start of the group
				var groupStart = 0;
				for (var i = 0; i < groupIndex; i++)
				{
					var g = _toolbarGroups[i];
					groupStart += g.Nodes.Count + (g.HasSplitter ? 1 : 0);
				}

				// Add the node to the list and sort
				var group = _toolbarGroups[groupIndex];
				group.Nodes = group.Nodes.Union(new[] { menuTreeNode }).OrderBy(x => x.OrderHint ?? "").ToList();

				// Skip to the start of the node and insert
				var idx = group.Nodes.IndexOf(menuTreeNode);
				ToolStrip.Items.Insert(groupStart + idx, menuTreeNode.ToolbarButton);

				// Check groups for splitters
				groupStart = 0;
				for (var i = 0; i < _toolbarGroups.Count - 1; i++)
				{
					var g = _toolbarGroups[i];
					groupStart += g.Nodes.Count;

					// Add a splitter to the group if needed
					if (!g.HasSplitter && g.Nodes.Count > 0)
					{
						ToolStrip.Items.Insert(groupStart, new ToolStripSeparator());
						g.HasSplitter = true;
					}

					groupStart++;
				}
			}
		}

		/// <summary>
		/// A dummy node of the virtual menu tree. This node is text only, always enabled, and does nothing.
		/// </summary>
		private class MenuTreeTextNode : BaseMenuTreeNode
		{
			public override string OrderHint => Group.OrderHint;

			public MenuTreeTextNode(IContext context, string text, MenuGroup group)
			{
				Context = context;
				Group = group ?? new MenuGroup("", "", "", "T");
				MenuMenuItem = new ToolStripMenuItem(text) { Tag = this };
			}
		}

		/// <summary>
		/// A normal node of the virtual tree. This node has text, an icon, and will do something when activated.
		/// </summary>
		private class MenuTreeNode : BaseMenuTreeNode
		{
			private IMenuItem MenuItem { get; set; }

			public override string OrderHint => MenuItem.OrderHint;

			public MenuTreeNode(IContext context, IMenuItem menuItem, MenuGroup group)
			{
				var en = menuItem.IsInContext(context);

				Group = group ?? new MenuGroup("", "", "", "T");
				Context = context;

				MenuItem = menuItem;

				MenuMenuItem = new ToolStripMenuItem(menuItem.Name, menuItem.Icon)
				{
					Tag = this,
					ShortcutKeyDisplayString = menuItem.ShortcutText,
					Enabled = en
				};
				MenuMenuItem.Click += Fire;
				MenuMenuItem.MouseEnter += (s, a) => { Oy.Publish("Status:Information", menuItem.Description); };
				MenuMenuItem.MouseLeave += (s, a) => { Oy.Publish("Status:Information", ""); };

				if (menuItem.AllowedInToolbar)
				{
					ToolbarButton = new ToolStripButton(menuItem.Name, menuItem.Icon)
					{
						Tag = this,
						DisplayStyle = ToolStripItemDisplayStyle.Image,
						Enabled = en
					};
					ToolbarButton.Click += Fire;
					ToolbarButton.MouseEnter += (s, a) => { Oy.Publish("Status:Information", menuItem.Description); };
					ToolbarButton.MouseLeave += (s, a) => { Oy.Publish("Status:Information", ""); };
				}

				if (menuItem.IsToggle)
				{
					MenuMenuItem.CheckState = menuItem.GetToggleState(context) ? CheckState.Checked : CheckState.Unchecked;
					if (ToolbarButton != null) ToolbarButton.CheckState = MenuMenuItem.CheckState;
				}
			}

			private void Fire(object sender, EventArgs e)
			{
				MenuItem?.Invoke(Context).ContinueWith(t => MenuMenuItem.GetCurrentParent()?.InvokeLater(Update));
			}

			public override void Update()
			{
				var en = MenuItem.IsInContext(Context);
				MenuMenuItem.Enabled = en;
				if (ToolbarButton != null) ToolbarButton.Enabled = en;
				if (MenuItem.IsToggle && en)
				{
					var ts = MenuItem.GetToggleState(Context);
					MenuMenuItem.CheckState = ts ? CheckState.Checked : CheckState.Unchecked;
					if (ToolbarButton != null) ToolbarButton.CheckState = MenuMenuItem.CheckState;
				}
				base.Update();
			}
		}

		private abstract class BaseMenuTreeNode
		{
			public MenuGroup Group { get; set; }
			public IContext Context { get; set; }

			public ToolStripMenuItem MenuMenuItem { get; set; }
			public ToolStripButton ToolbarButton { get; set; }

			public List<MenuTreeGroup> Groups { get; protected set; }
			public Dictionary<string, BaseMenuTreeNode> Children { get; private set; }

			public abstract string OrderHint { get; }

			protected BaseMenuTreeNode()
			{
				Groups = new List<MenuTreeGroup>();
				Children = new Dictionary<string, BaseMenuTreeNode>();
			}

			public void AddChild(string name, BaseMenuTreeNode menuTreeNode)
			{
				Children.Add(name, menuTreeNode);
				if (Groups.All(x => x.Group.Name != menuTreeNode.Group.Name))
				{
					Groups.Add(new MenuTreeGroup(menuTreeNode.Group));
					Groups = Groups.OrderBy(x => x.Group.OrderHint).ToList();
				}

				// Insert the item into the correct index
				var groupIndex = Groups.FindIndex(x => x.Group.Name == menuTreeNode.Group.Name);

				// Skip to the start of the group
				var groupStart = 0;
				for (var i = 0; i < groupIndex; i++)
				{
					var g = Groups[i];
					groupStart += g.Nodes.Count + (g.HasSplitter ? 1 : 0);
				}

				// Add the node to the list and sort
				var group = Groups[groupIndex];
				group.Nodes = group.Nodes.Union(new[] { menuTreeNode }).OrderBy(x => x.OrderHint ?? "").ToList();

				// Skip to the start of the node and insert
				var idx = group.Nodes.IndexOf(menuTreeNode);
				MenuMenuItem.DropDownItems.Insert(groupStart + idx, menuTreeNode.MenuMenuItem);

				// Check groups for splitters
				groupStart = 0;
				for (var i = 0; i < Groups.Count - 1; i++)
				{
					var g = Groups[i];
					groupStart += g.Nodes.Count;

					// Add a splitter to the group if needed
					if (!g.HasSplitter && g.Nodes.Count > 0)
					{
						MenuMenuItem.DropDownItems.Insert(groupStart, new ToolStripSeparator());
						g.HasSplitter = true;
					}

					groupStart++;
				}
			}

			public virtual void Update()
			{
				foreach (var c in Children)
				{
					c.Value.Update();
				}
			}
		}

		private class MenuTreeGroup
		{
			public MenuGroup Group { get; set; }
			public List<BaseMenuTreeNode> Nodes { get; set; }

			public bool HasSplitter { get; set; }

			public MenuTreeGroup(MenuGroup group)
			{
				Group = group;
				Nodes = new List<BaseMenuTreeNode>();
			}
		}
	}
}
