using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Problems;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.BspEditor.Editing;
[Export(typeof(IDialog))]
[AutoTranslate]
public partial class CheckForProblemsDialog : Window, IDialog
{
	[Import("Shell", typeof(Form))] private Lazy<Form> _parent;
	[Import] private IContext _context;
	[ImportMany] private IEnumerable<Lazy<IProblemCheck>> _problemCheckers;

	private List<Subscription> _subscriptions;
	private List<ProblemWrapper> _problems;

	private bool _visibleOnly;
	private bool _selectedOnly;

	public bool Visible { get; set; }

	public CheckForProblemsDialog()
	{
		InitializeComponent();
		_problems = new List<ProblemWrapper>();
		_visibleOnly = true;
		_selectedOnly = false;

		//using (var icon = new Bitmap(Resources.Menu_CheckForProblems))
		//{
		//	var ptr = icon.GetHicon();
		//	var ico = Icon.FromHandle(ptr);
		//	Icon = ico;
		//	ico.Dispose();
		//}
	}

	public void Translate(ITranslationStringProvider strings)
	{
		//if (Handle == null) CreateHandle();
		var prefix = GetType().FullName;
		//this.InvokeLater(() =>
		//{
		Title = strings.GetString(prefix, "Title");
		grpDetails.Header = strings.GetString(prefix, "Details");
		btnGoToError.Content = strings.GetString(prefix, "GoToError");
		btnFix.Content = strings.GetString(prefix, "FixError");
		btnFixAllOfType.Content = strings.GetString(prefix, "FixAllOfType");
		btnFixAll.Content = strings.GetString(prefix, "FixAll");
		chkVisibleOnly.Content = strings.GetString(prefix, "VisibleObjectsOnly");
		chkSelectedOnly.Content = strings.GetString(prefix, "SelectedObjectsOnly");
		lnkExtraDetails.Content = strings.GetString(prefix, "ClickForAdditionalDetails");
		btnClose.Content = strings.GetString(prefix, "CloseButton");
		//});
	}

	protected override void OnClosing(WindowClosingEventArgs e)
	{
		e.Cancel = true;
		Oy.Publish("Context:Remove", new ContextInfo("BspEditor:CheckForProblems"));
	}

	private void PointerEntered(object sender, PointerPressedEventArgs args)
	{
		Focus();
		//base.OnMouseEnter(e);
	}

	public bool IsInContext(IContext context)
	{
		return context.HasAny("BspEditor:CheckForProblems");
	}

	public void SetVisible(IContext context, bool visible)
	{
		//this.InvokeLater(() =>
		{
			if (visible)
			{
				//FIXME: This is broken because the parent form is not Avalonia
				//if (!Visible) Show(_parent.Value);
				if (!Visible)
				{
					Visible = !Visible;
					Show();
					Subscribe();
					DoCheck();
				}
			}
			else
			{
				if (Visible)
				{
					Visible = !Visible;
					Hide();
					Unsubscribe();
				}
			}
		}
		//);
	}

	private void Subscribe()
	{
		if (_subscriptions != null) return;
		_subscriptions = new List<Subscription>
			{
				Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
				Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated)
			};
	}

	private void Unsubscribe()
	{
		if (_subscriptions == null) return;
		_subscriptions.ForEach(x => x.Dispose());
		_subscriptions = null;
	}

	private async Task DocumentActivated(MapDocument document)
	{
		await DoCheck(document);
	}

	private async Task DocumentChanged(Change change)
	{
		await DoCheck(change.Document);
	}

	private Task DoCheck()
	{
		return DoCheck(_context.Get<MapDocument>("ActiveDocument"));
	}

	private async Task DoCheck(MapDocument doc)
	{
		_problems = doc == null ? new List<ProblemWrapper>() : await Check(doc, GetFilter(_visibleOnly, _selectedOnly));
		//this.InvokeLater(() =>
		//{
		var si = ProblemsList.SelectedIndex;
		//ProblemsList.BeginUpdate();
		ProblemsList.Items.Clear();
		foreach (var item in _problems.OfType<object>())
		{
			ProblemsList.Items.Add(item);
		}
		//ProblemsList.Items.AddRange(_problems.OfType<object>().ToArray());
		if (si < 0 || si >= ProblemsList.Items.Count) si = 0;
		if (si < ProblemsList.Items.Count) ProblemsList.SelectedIndex = si;
		//ProblemsList.EndUpdate();
		UpdateSelectedProblem(null, null);
		//});
	}

	private Predicate<IMapObject> GetFilter(bool visibleOnly, bool selectedOnly)
	{
		return x =>
		{
			if (selectedOnly && !x.IsSelected) return false;
			if (visibleOnly && x.Data.OfType<IObjectVisibility>().Any(v => v.IsHidden)) return false;
			return true;
		};
	}

	private async Task<List<ProblemWrapper>> Check(MapDocument map, Predicate<IMapObject> filter)
	{
		var list = new List<ProblemWrapper>();

		var index = 1;
		foreach (var checker in _problemCheckers)
		{
			var probs = await checker.Value.Check(map, filter);
			foreach (var p in probs)
			{
				var w = new ProblemWrapper(index++, map, p, checker.Value);
				list.Add(w);
			}
		}

		return list;
	}

	private void UpdateSelectedProblem(object? sender, SelectionChangedEventArgs e)
	{
		var sel = ProblemsList.SelectedItem as ProblemWrapper;
		DescriptionTextBox.Text = sel?.Details ?? "";
		btnGoToError.IsEnabled = sel?.Problem.Objects.Any() == true;
		btnFix.IsEnabled = btnFixAllOfType.IsEnabled = sel?.CanFix == true;
		lnkExtraDetails.IsVisible = sel?.Url != null;
		btnFixAll.IsEnabled = _problems.Any(x => x.CanFix);
	}

	private void VisibleOnlyCheckboxChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		_selectedOnly = chkSelectedOnly.IsChecked.Value;
		_visibleOnly = chkVisibleOnly.IsChecked.Value;
		DoCheck();
	}

	private void OpenUrl(object sender, LinkLabelLinkClickedEventArgs e)
	{
		var sel = ProblemsList.SelectedItem as ProblemWrapper;
		var uri = sel?.Url;
		if (uri != null)
		{
			Process.Start(uri.ToString());
		}
	}

	private async void GoToError(object sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var doc = _context.Get<MapDocument>("ActiveDocument");
		if (doc == null) return;

		var sel = ProblemsList.SelectedItem as ProblemWrapper;
		if (sel == null || !sel.Problem.Objects.Any()) return;

		var op = new Transaction(
			new Deselect(doc.Selection.Except(sel.Problem.Objects)),
			new Select(sel.Problem.Objects)
		);
		await MapDocumentOperation.Perform(doc, op);

		var bb = doc.Selection.GetSelectionBoundingBox();
		await Oy.Publish("MapDocument:Viewport:Focus2D", bb);
		await Oy.Publish("MapDocument:Viewport:Focus3D", bb);
	}

	private async Task FixErrors(IEnumerable<ProblemWrapper> problems)
	{
		var doc = _context.Get<MapDocument>("ActiveDocument");
		if (doc == null) return;

		var fixes = problems.Where(x => x.CanFix && x.Document == doc).ToList();
		foreach (var pw in fixes)
		{
			await pw.Checker.Fix(doc, pw.Problem);
		}
	}

	private async void FixError(object sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		if (ProblemsList.SelectedItem is ProblemWrapper sel) await FixErrors(new[] { sel });
	}

	private async void FixAllOfType(object sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var sel = ProblemsList.SelectedItem as ProblemWrapper;
		if (sel == null) return;
		await FixErrors(_problems.Where(x => x.Checker == sel.Checker));
	}

	private async void FixAll(object sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		await FixErrors(_problems);
	}

	private class ProblemWrapper
	{
		public int Index { get; }
		public MapDocument Document { get; set; }
		public Problem Problem { get; }
		public IProblemCheck Checker { get; }

		public string Name => Checker.Name;
		public string Details => Checker.Details;
		public Uri Url => Checker.Url;
		public bool CanFix => Checker.CanFix;

		public ProblemWrapper(int index, MapDocument document, Problem problem, IProblemCheck checker)
		{
			Index = index;
			Document = document;
			Problem = problem;
			Checker = checker;
		}

		public override string ToString()
		{
			return Index + ": " +
				   (Name ?? Checker.GetType().Name) +
				   (String.IsNullOrWhiteSpace(Problem.Text) ? "" : " - " + Problem.Text);
		}
	}

	private void Close(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		this.Close();
	}

}