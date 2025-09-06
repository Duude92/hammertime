using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Tools.Vertex;
using Sledge.BspEditor.Tools.Vertex.Errors;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Sledge.BspEditor.Tools;

[Export(typeof(ISidebarComponent))]
[OrderHint("G")]
[AutoTranslate]
public partial class VertexErrorsSidebarPanel : UserControl, ISidebarComponent
{
	private readonly IEnumerable<Lazy<IVertexErrorCheck>> _errorChecks;
	private readonly Lazy<ITranslationStringProvider> _translator;

	public string Title { get; set; } = "Vertex Errors";
	public object Control => this;
	public VertexErrorsSidebarPanel()
	{
		InitializeComponent();
	}

	[ImportingConstructor]
	public VertexErrorsSidebarPanel(
		[ImportMany] IEnumerable<Lazy<IVertexErrorCheck>> errorChecks,
		[Import] Lazy<ITranslationStringProvider> translator
	)
	{
		_errorChecks = errorChecks;
		_translator = translator;

		InitializeComponent();

		Oy.Subscribe<VertexSelection>("VertexTool:Updated", t => UpdateErrorList(t));
	}

	private void UpdateErrorList(VertexSelection selection)
	{
		this.InvokeLater(() =>
		{
			var errors = _errorChecks.SelectMany(ec => selection.SelectMany(s => ec.Value.GetErrors(s)));
			ErrorList.Items.Clear();
			foreach (var error in errors.Select(e => new ErrorWrapper(_translator.Value.GetString(e.Key) ?? e.Key, e)).OfType<object>().ToArray())
			{
				ErrorList.Items.Add(error);
			}
		});
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out VertexTool _);
	}

	private void ErrorListSelectionChanged(object sender, EventArgs e)
	{
		var error = ErrorList.SelectedItem as ErrorWrapper;
		OnSelectError(error?.Error);
	}

	protected virtual void OnSelectError(VertexError error)
	{
		// todo post-beta: vertex errors
	}

	protected virtual void OnFixError(VertexError error)
	{
		// todo post-beta: vertex errors
	}

	protected virtual void OnFixAllErrors()
	{
		// todo post-beta: vertex errors
	}

	private class ErrorWrapper
	{
		private readonly string _message;
		public VertexError Error { get; }

		public ErrorWrapper(string message, VertexError error)
		{
			_message = message;
			Error = error;
		}

		public override string ToString()
		{
			return _message;
		}
	}
}