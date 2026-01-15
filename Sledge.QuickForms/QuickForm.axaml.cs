using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Sledge.QuickForms.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Sledge.QuickForms;

public partial class QuickForm : Window, IDisposable
{
	private readonly WrapPanel _flpLayout;
	private readonly Panel _layoutSizerPanel;
	private readonly List<QuickFormItem> _items;

	public DialogResult DialogResult { get; private set; }
	/// <summary>
	/// True to use shortcut keys - enter will press an ok button, escape will press a cancel button.
	/// </summary>
	public bool UseShortcutKeys { get; set; }

	/// <summary>
	/// Create a form with the specified title.
	/// </summary>
	/// <param name="title">The title of the form</param>
	public QuickForm(string title)
	{
#if DEBUG
		this.AttachDevTools();
#endif
		_items = new List<QuickFormItem>();
		Title = title;
		//FormBorderStyle = FormBorderStyle.FixedDialog;
		ShowInTaskbar = false;
		//MinimizeBox = false;
		//MaximizeBox = false;
		UseShortcutKeys = false;
		//KeyPreview = true;
		CanResize = false;

		_flpLayout = new WrapPanel
		{
			//Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
			//AutoSize = true,
			//AutoSizeMode = AutoSizeMode.GrowOnly,
			//FlowDirection = FlowDirection.TopDown,
			//Location = new Point(5, 5),
			//Size = new Size(ClientSize.Width, 10)
			
			Orientation = Avalonia.Layout.Orientation.Vertical,
			FlowDirection = Avalonia.Media.FlowDirection.LeftToRight,
			MaxWidth = ClientSize.Width,
			MaxHeight = ClientSize.Height
		};
		_layoutSizerPanel = new Panel
		{
			//AutoSize = false,
			Height = 2,
			Width = ClientSize.Width - 12,
			//Margin = Padding.Empty,
			//Padding = Padding.Empty
		};
		_flpLayout.Children.Add(_layoutSizerPanel);
		InitializeComponent();

		this.Content = _flpLayout;
	}
	public Task<DialogResult> ShowDialog()
	{
		var wind = Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
		return ShowDialog<DialogResult>(wind);
	}
	protected override void OnResized(WindowResizedEventArgs e)
	{
		_layoutSizerPanel.Width = ClientSize.Width - 12;
		base.OnResized(e);
	}
	protected override void OnKeyDown(KeyEventArgs e)
	{
		//if (UseShortcutKeys)
		//{
		//	Func<Button, bool> searchFunc = null;
		//	if (e.KeyCode == Keys.Enter)
		//	{
		//		searchFunc = value => value.DialogResult == DialogResult.OK || value.DialogResult == DialogResult.Yes;
		//	}
		//	else if (e.KeyCode == Keys.Escape)
		//	{
		//		searchFunc = value => value.DialogResult == DialogResult.Cancel || value.DialogResult == DialogResult.No;
		//	}
		//	if (searchFunc != null)
		//	{
		//		var btn = ContentPanel.Children.OfType<Button>().FirstOrDefault(searchFunc);
		//		if (btn == null)
		//		{
		//			btn = _flpLayout.Controls.OfType<QuickFormButtonSet>().FirstOrDefault()?.Controls.OfType<Button>().FirstOrDefault(searchFunc);
		//		}
		//		btn?.PerformClick();
		//	}
		//}
		base.OnKeyDown(e);
	}
	public void SetClientSize(Size size)
	{
		ClientSize = size;
	}
	protected override void OnLoaded(RoutedEventArgs e)
	{
		var ps = _flpLayout.DesiredSize;
		//var ps = _flpLayout.GetPreferredSize(new Size(ClientSize.Width, 100000));
		ClientSize = new Size(ClientSize.Width, ps.Height + 10);
		var nonlabel = ContentPanel.Children.OfType<Control>().FirstOrDefault(x => !(x is Label));
		nonlabel?.Focus();

		_items.ForEach(item => item.OnResize(e));
		base.OnLoaded(e);
	}

	public async Task<DialogResult> ShowDialogAsync()
	{
		await Task.Yield();
		var dialog = ShowDialog(null);
		return DialogResult;
	}

	/// <summary>
	/// Add an item to the form.
	/// </summary>
	/// <param name="name">The name of the item</param>
	/// <param name="item">The item to add</param>
	private void AddItem(string name, QuickFormItem item)
	{
		item.Name = name;
		_items.Add(item);
		_flpLayout.Children.Add(item);
	}

	/// <summary>
	/// Add an item to the form.
	/// </summary>
	/// <param name="item">The item to add</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm Item(QuickFormItem item)
	{
		AddItem(item.Name, item);
		return this;
	}

	/// <summary>
	/// Add a textbox to the form.
	/// </summary>
	/// <param name="name">The name of the textbox</param>
	/// <param name="text">The display text for the textbox</param>
	/// <param name="value">The default value of the textbox</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm TextBox(string name, string text, string value = "")
	{
		AddItem(name, new QuickFormTextBox(text, value));
		return this;
	}

	/// <summary>
	/// Add a browse textbox to the form.
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <param name="text">The display text for the control</param>
	/// <param name="filter">The filter for the open file dialog</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm Browse(string name, string text, string filter)
	{
		AddItem(name, new QuickFormBrowse(text, "Browse...", filter));
		return this;
	}

	/// <summary>
	/// Add a label to the form.
	/// </summary>
	/// <param name="text">The text of the label</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm Label(string text)
	{
		AddItem(string.Empty, new QuickFormLabel(text));
		return this;
	}

	/// <summary>
	/// Add a NumericUpDown to the form.
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <param name="text">The display text of the control</param>
	/// <param name="min">The minimum value of the control</param>
	/// <param name="max">The maximum value of the control</param>
	/// <param name="decimals">The number of decimals for the control</param>
	/// <param name="value">The default value of the control</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm NumericUpDown(string name, string text, int min, int max, int decimals, decimal value = 0)
	{
		AddItem(name, new QuickFormNumericUpDown(text, min, max, decimals, value));
		return this;
	}

	/// <summary>
	/// Add a ComboBox to the form.
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <param name="text">The display text of the control</param>
	/// <param name="items">The items for the control</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm ComboBox(string name, string text, IEnumerable<object> items)
	{
		AddItem(name, new QuickFormComboBox(text, items));
		return this;
	}

	/// <summary>
	/// Add a checkbox to the form.
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <param name="text">The display text for the checkbox</param>
	/// <param name="value">The initial value of the checkbox</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm CheckBox(string name, string text, bool value = false)
	{
		AddItem(name, new QuickFormCheckBox(text, value));
		return this;
	}

	/// <summary>
	/// Add OK and Cancel buttons to the control
	/// </summary>
	/// <param name="okText"></param>
	/// <param name="cancelText"></param>
	/// <param name="ok">The action to perform when OK is clicked</param>
	/// <param name="cancel">The action to perform when cancel is clicked</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm OkCancel(string okText, string cancelText, Action<QuickForm> ok = null, Action<QuickForm> cancel = null)
	{
		AddItem(string.Empty, new QuickFormButtonSet(new (string, DialogResult, Action)[]
		{
				(okText, DialogResult.OK, OKAction),
				(cancelText, DialogResult.Cancel, CancelAction)
		}));

		return this;

		void OKAction()
		{
			ok?.Invoke(this);
			DialogResult = DialogResult.OK;
			Close(DialogResult);
		}

		void CancelAction()
		{
			cancel?.Invoke(this);
			DialogResult = DialogResult.Cancel;
			Close(DialogResult);
		}
	}

	/// <summary>
	/// Add a button to the control
	/// </summary>
	/// <param name="text">The button text</param>
	/// <param name="action">The action to perform when the button is clicked</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm Button(string text, Action<QuickForm> action)
	{
		AddItem(string.Empty, new QuickFormButtonSet(new[]
		{
				(text, DialogResult.None, new Action(() => action(this)))
			}));
		return this;
	}

	/// <summary>
	/// Add a set of buttons to the control, each one closing the form
	/// with a specific dialog result.
	/// </summary>
	/// <param name="buttons">The button texts and dialog results</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm DialogButtons(params (string, DialogResult)[] buttons)
	{
		AddItem(string.Empty, new QuickFormButtonSet(
			buttons.Select(x => (x.Item1, x.Item2, new Action(() => { })))
		));
		return this;
	}

	/// <summary>
	/// Add a set of buttons to the control, each one performing a custom action.
	/// </summary>
	/// <param name="buttons">The button texts and dialog results</param>
	/// <returns>This object, for method chaining</returns>
	public QuickForm ActionButtons(params (string, Action<QuickForm>)[] buttons)
	{
		AddItem(string.Empty, new QuickFormButtonSet(
			buttons.Select(x => (x.Item1, DialogResult.None, new Action(() => x.Item2(this))))
		));
		return this;
	}

	/// <summary>
	/// Get a control by name
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <returns>The control, or null if it was not found</returns>
	public QuickFormItem GetItem(string name)
	{
		return _items.FirstOrDefault(x => x.Name == name);
	}

	/// <summary>
	/// Get a string value from a control
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <returns>The string value</returns>
	public string String(string name)
	{
		var c = GetItem(name);
		if (c != null) return c.Value as string;
		throw new Exception("Control " + name + " not found!");
	}

	/// <summary>
	/// Get a decimal value from a control
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <returns>The decimal value</returns>
	public decimal Decimal(string name)
	{
		var c = GetItem(name);
		if (c != null) return (decimal)c.Value;
		throw new Exception("Control " + name + " not found!");
	}

	/// <summary>
	/// Get a boolean value from a control
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <returns>The boolean value</returns>
	public bool Bool(string name)
	{
		var c = GetItem(name);
		if (c != null) return (bool)c.Value;
		throw new Exception("Control " + name + " not found!");
	}

	/// <summary>
	/// Get an object from a control
	/// </summary>
	/// <param name="name">The name of the control</param>
	/// <returns>The object</returns>
	public object Object(string name)
	{
		var c = GetItem(name);
		if (c != null) return c.Value;
		throw new Exception("Control " + name + " not found!");
	}

	/// <summary>
	/// Get all the named keys and values for this form
	/// </summary>
	/// <returns>Key/value collection</returns>
	public Dictionary<string, object> GetValues()
	{
		return _items.Where(x => !string.IsNullOrWhiteSpace(x.Name))
			.GroupBy(x => x.Name)
			.ToDictionary(x => x.Key, x => x.First().Value);
	}

	public void Dispose()
	{
	}
}
