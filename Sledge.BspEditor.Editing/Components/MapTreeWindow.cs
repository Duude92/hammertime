﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
	[Export(typeof(IDialog))]
	[AutoTranslate]
	public partial class MapTreeWindow : Form, IDialog, IManualTranslate
	{
		[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
		[Import] private IContext _context;

		private List<Subscription> _subscriptions;
		private IMapObject _selection;

		public MapTreeWindow()
		{
			InitializeComponent();
			MapTree.BeforeExpand += MapTree_BeforeExpand;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			Oy.Publish("Context:Remove", new ContextInfo("BspEditor:MapTree"));
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			Focus();
			base.OnMouseEnter(e);
		}

		public bool IsInContext(IContext context)
		{
			return context.HasAny("BspEditor:MapTree");
		}

		public void SetVisible(IContext context, bool visible)
		{
			this.InvokeLater(() =>
			{
				if (visible)
				{
					if (!Visible) Show(_parent.Value);
					Subscribe();
					RefreshNodes();
				}
				else
				{
					Hide();
					Unsubscribe();
				}
			});
		}

		public void Translate(ITranslationStringProvider strings)
		{
			if (Handle == null) CreateHandle();
			var prefix = GetType().FullName;
			this.InvokeLater(() =>
			{
				Text = strings.GetString(prefix, "Title");
			});
		}

		private void Subscribe()
		{
			if (_subscriptions != null) return;
			_subscriptions = new List<Subscription>
			{
				Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
				Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated),
				Oy.Subscribe<MapDocument>("MapDocument:SelectionChanged", SelectionChanged)
			};
		}

		private void Unsubscribe()
		{
			if (_subscriptions == null) return;
			_subscriptions.ForEach(x => x.Dispose());
			_subscriptions = null;
		}

		private async Task SelectionChanged(MapDocument doc)
		{
			this.InvokeLater(() =>
			{
				if (doc == null || doc.Selection.IsEmpty) return;
				_selection = doc.Selection.GetSelectedParents().First();
				var node = FindNodeWithTag(MapTree.Nodes.OfType<TreeNode>(), _selection);
				if (node != null) MapTree.SelectedNode = node;
			});
		}

		public async Task DocumentActivated(MapDocument document)
		{
			this.InvokeLater(() =>
			{
				RefreshNodes(document);
			});
		}

		private async Task DocumentChanged(Change change)
		{
			await this.InvokeAsync(async () =>
			{
				await RefreshNodes(change.Document);
				var node = FindNodeWithTag(MapTree.Nodes.OfType<TreeNode>(), _selection);
				if (node != null) MapTree.SelectedNode = node;
			});
		}

		private TreeNode FindNodeWithTag(IEnumerable<TreeNode> nodes, object tag)
		{
			foreach (var tn in nodes)
			{
				if (tn.Tag == tag) return tn;
				var recurse = FindNodeWithTag(tn.Nodes.OfType<TreeNode>(), tag);
				if (recurse != null) return recurse;
			}
			return null;
		}

		private async Task RefreshNodes()
		{
			var doc = _context.Get<MapDocument>("ActiveDocument");
			await RefreshNodes(doc);
		}

		private async Task RefreshNodes(MapDocument doc)
		{
			MapTree.BeginUpdate();
			MapTree.Nodes.Clear();
			if (doc != null)
			{
				await LoadMapNodeAsync(null, doc.Map.Root);
			}
			MapTree.EndUpdate();
		}

		private async Task LoadMapNodeAsync(TreeNode parent, IMapObject obj)
		{
			var text = GetNodeText(obj);
			var node = new TreeNode(obj.GetType().Name + $" {obj.ID}" + text) { Tag = obj };

			// Add a dummy node to indicate it has children (this forces expand button)
			node.Nodes.Add(new TreeNode("Loading..."));

			if (parent == null)
				MapTree.Nodes.Add(node);
			else
				parent.Nodes.Add(node);

			// Load child nodes asynchronously
			await Task.Run(() => LoadChildNodes(node, obj));
			MapTree.Invoke((MethodInvoker)(() => node.Expand()));
		}
		private async void MapTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			var obj = e.Node.Tag as IMapObject;
			if (obj != null && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Loading...")
			{
				await Task.Run(() => LoadChildNodes(e.Node, obj));
			}
		}
		private void LoadChildNodes(TreeNode node, IMapObject obj)
		{
			// Prepare list of child nodes
			List<TreeNode> childNodes = new();

			if (obj is Root w)
			{
				childNodes.AddRange(GetEntityNodes(w.Data.GetOne<EntityData>()));
			}
			else if (obj is Entity e)
			{
				childNodes.AddRange(GetEntityNodes(e.EntityData));
			}
			else if (obj is Solid s)
			{
				childNodes.AddRange(GetFaceNodes(s.Faces));
			}

			foreach (var mo in obj.Hierarchy)
			{
				TreeNode childNode = new TreeNode(mo.GetType().Name + $" {mo.ID}" + GetNodeText(mo)) { Tag = mo };
				childNode.Nodes.Add(new TreeNode("Loading...")); // Placeholder for expandability
				childNodes.Add(childNode);
			}

			// Update UI on the main thread
			MapTree.Invoke((MethodInvoker)(() =>
			{
				node.Nodes.Clear(); // Remove "Loading..." node
				node.Nodes.AddRange(childNodes.ToArray()); // Add real children
			}));
		}
		private string GetNodeText(IMapObject mo)
		{
			if (mo is Solid solid)
			{
				return " (" + solid.Faces.Count() + " faces)";
			}
			if (mo is Group)
			{
				return " (" + mo.Hierarchy.HasChildren + " children)";
			}
			var ed = mo.Data.GetOne<EntityData>();
			if (ed != null)
			{
				var targetName = ed.Get("targetname", "");
				return ": " + ed.Name + (String.IsNullOrWhiteSpace(targetName) ? "" : " (" + targetName + ")");
			}
			return "";
		}

		private IEnumerable<TreeNode> GetEntityNodes(EntityData data)
		{
			if (data == null) yield break;

			yield return new TreeNode("Flags: " + data.Flags);
		}

		private IEnumerable<TreeNode> GetFaceNodes(IEnumerable<Face> faces)
		{
			var c = 0;
			foreach (var face in faces)
			{
				var fnode = new TreeNode("Face " + c);
				c++;
				var pnode = fnode.Nodes.Add($"Plane: {face.Plane.Normal} * {face.Plane.DistanceFromOrigin}");
				pnode.Nodes.Add($"Normal: {face.Plane.Normal}");
				pnode.Nodes.Add($"Distance: {face.Plane.DistanceFromOrigin}");
				pnode.Nodes.Add($"A: {face.Plane.A}");
				pnode.Nodes.Add($"B: {face.Plane.B}");
				pnode.Nodes.Add($"C: {face.Plane.C}");
				pnode.Nodes.Add($"D: {face.Plane.D}");
				var tnode = fnode.Nodes.Add("Texture: " + face.Texture.Name);
				tnode.Nodes.Add($"U Axis: {face.Texture.UAxis}");
				tnode.Nodes.Add($"V Axis: {face.Texture.VAxis}");
				tnode.Nodes.Add($"Scale: X = {face.Texture.XScale}, Y = {face.Texture.YScale}");
				tnode.Nodes.Add($"Offset: X = {face.Texture.XShift}, Y = {face.Texture.YShift}");
				tnode.Nodes.Add("Rotation: " + face.Texture.Rotation);
				var vnode = fnode.Nodes.Add($"Vertices: {face.Vertices.Count}");
				var d = 0;
				foreach (var vertex in face.Vertices)
				{
					var cnode = vnode.Nodes.Add("Vertex " + d + ": " + vertex);
					d++;
				}
				yield return fnode;
			}
		}

		private async void TreeSelectionChanged(object sender, TreeViewEventArgs e)
		{
			//var document = _context.Get<MapDocument>("ActiveDocument"); 
			await RefreshSelectionProperties();
			//if (MapTree.SelectedNode != null && MapTree.SelectedNode.Tag is IMapObject && !(MapTree.SelectedNode.Tag is Root) && document != null)
			//{
			//    var transaction = new Transaction(
			//        new Deselect(document.Selection),
			//        new Select((IMapObject)MapTree.SelectedNode.Tag)
			//        );
			//    await MapDocumentOperation.Perform(document, transaction);
			//}
		}

		private async Task RefreshSelectionProperties()
		{
			Properties.Items.Clear();
			if (MapTree.SelectedNode != null && MapTree.SelectedNode.Tag != null)
			{
				var list = await GetTagProperties((IMapObject)MapTree.SelectedNode.Tag);
				foreach (var kv in list)
				{
					Properties.Items.Add(new ListViewItem(new[] { kv.Item1, kv.Item2 }));
				}
				Properties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		private async Task<IEnumerable<Tuple<string, string>>> GetTagProperties(IMapObject tag)
		{
			var list = new List<Tuple<string, string>>();
			if (!(tag.ID is long id)) return list;

			var doc = _context.Get<MapDocument>("ActiveDocument");
			if (doc == null) return list;

			var mo = doc.Map.Root.FindByID(id);
			if (mo == null) return list;

			var ed = mo.Data.GetOne<EntityData>();
			if (ed != null)
			{

				var gameData = await doc.Environment.GetGameData();

				var gd = gameData.GetClass(ed.Name);
				foreach (var prop in ed.Properties)
				{
					var gdp = gd?.Properties.FirstOrDefault(x => String.Equals(x.Name, prop.Key, StringComparison.InvariantCultureIgnoreCase));
					var key = gdp != null && !String.IsNullOrWhiteSpace(gdp.ShortDescription) ? gdp.ShortDescription : prop.Key;
					list.Add(Tuple.Create(key, prop.Value));
				}
				return list;
			}
			var dd = mo.Data;
			if (dd != null)
			{
				list.Add(Tuple.Create("Color", dd.Get<ObjectColor>().First().Color.ToString()));
				var hidden = dd.Get<QuickHidden>().FirstOrDefault()?.IsHidden;

				list.Add(Tuple.Create("IsVisgroupHidden", dd.Get<VisgroupHidden>().First().IsHidden.ToString()));
				list.Add(Tuple.Create("IsHidden", hidden.HasValue ? hidden.ToString() : "False"));
				return list;
			}
			return list;
		}
	}
}
