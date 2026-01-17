using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json.Linq;
using Sledge.Common.Shell;
using Sledge.Common.Translations;
using Sledge.Shell.Forms;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Sledge.Shell.Forms;

public partial class TranslationForm : Window
{
	private readonly TranslationStringsCatalog _catalog;
	private readonly IApplicationInfo _appInfo;
	private string _appTranslationsFolder;
	private string _userTranslationsFolder;
	private ObservableCollection<DataTranslation> Items { get; set; } = new();

	/// <summary>
	/// Construct the translator form
	/// </summary>
	public TranslationForm()
	{
		InitializeComponent();
		DataContext = this;

		_catalog = Common.Container.Get<TranslationStringsCatalog>();
		_appInfo = Common.Container.Get<IApplicationInfo>();

		_appTranslationsFolder = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "Translations");
		_userTranslationsFolder = _appInfo.GetApplicationSettingsFolder("Translations");

		PopulateLanguageList();
		PopulateFileList();


	}
	protected override void OnPointerEntered(PointerEventArgs e)
	{
		Focus();
		base.OnPointerEntered(e);
	}

	private void PopulateLanguageList()
	{
		cmbLanguage.Items.Clear();

		foreach (var lang in _catalog.Languages.Values)
		{
			if (lang.Code.Contains("debug")) continue;
			if (lang.Code == "en") continue;

			var w = new Wrapper<Language>(lang, lang.Description);
			cmbLanguage.Items.Add(w);
		}
	}

	private void PopulateFileList()
	{
		cmbFile.Items.Clear();

		var enFiles = Directory.GetFiles(_appTranslationsFolder, "*.en.json");
		foreach (var file in enFiles)
		{
			var w = new Wrapper<string>(file, Path.GetFileName(file.Substring(0, file.Length - 8)));
			cmbFile.Items.Add(w);
		}
		if (cmbFile.Items.Count > 0) cmbFile.SelectedIndex = 0;
	}

	private void LanguageChanged(object sender, SelectionChangedEventArgs e)
	{
		PopulateFileList();
	}

	private void FileChanged(object sender, SelectionChangedEventArgs e)
	{
		PopulateDataTable();
	}

	private void PopulateDataTable()
	{
		var lang = cmbLanguage.SelectedItem as Wrapper<Language>;
		var file = cmbFile.SelectedItem as Wrapper<string>;

		// Making the headers invisible during update speeds up rendering a lot

		//dataGridView.ColumnHeadersVisible = false;
		//dataGridView.RowHeadersVisible = false;

		//var source = (DataTable)dataGridView.DataSource;
		Items.Clear();
		if (lang != null && file != null)
		{
			PopulateTable(lang.Object, file.Object);
		}

		//dataGridView.ColumnHeadersVisible = true;
		//dataGridView.RowHeadersVisible = true;
	}

	private TranslationStringsCollection LoadLanguageFile(Language lang, string enFile)
	{
		var prefix = Path.GetFileName(enFile.Substring(0, enFile.Length - 8));
		var langFile = $"{prefix}.{lang.Code}.json";

		var appLang = TranslationStringsCatalog.LoadLanguageFromFile(Path.Combine(_appTranslationsFolder, langFile));
		var userLang = TranslationStringsCatalog.LoadLanguageFromFile(Path.Combine(_userTranslationsFolder, langFile));

		var tsc = new TranslationStringsCollection();
		if (appLang != null)
		{
			foreach (var kv in appLang.Collection.Settings) tsc.Settings[kv.Key] = kv.Value;
			foreach (var kv in appLang.Collection.Strings) tsc.Strings[kv.Key] = kv.Value;
		}
		if (userLang != null)
		{
			foreach (var kv in userLang.Collection.Settings) tsc.Settings[kv.Key] = kv.Value;
			foreach (var kv in userLang.Collection.Strings) tsc.Strings[kv.Key] = kv.Value;
		}
		return tsc;
	}

	private void PopulateTable(Language targetLang, string file)
	{
		var en = LoadLanguageFile(_catalog.Languages["en"], file);
		var collection = LoadLanguageFile(targetLang, file);
		var prefix = Path.GetFileName(file.Substring(0, file.Length - 8));

		foreach (var settingKv in en.Settings)
		{
			if (!collection.Settings.TryGetValue(settingKv.Key, out var translated)) translated = "";
			var niceKey = settingKv.Key[0] == '@' ? settingKv.Key : settingKv.Key.Substring(prefix.Length + 1);
			Items.Add(new DataTranslation(niceKey, "Setting", "Setting: " + niceKey, settingKv.Value, translated));
		}

		foreach (var stringKv in en.Strings)
		{
			if (!collection.Strings.TryGetValue(stringKv.Key, out var translated)) translated = "";
			var niceKey = stringKv.Key.Substring(prefix.Length + 1);
			Items.Add(new DataTranslation(niceKey, "String", niceKey, stringKv.Value, translated));
		}
	}

	private void AddLanguageClicked(object sender, RoutedEventArgs e)
	{
		using (var alf = new AddLanguageForm())
		{
			if (alf.ShowDialog() == DialogResult.OK)
			{
				var lang = new Language(alf.Code) { Description = alf.Description, Inherit = "en" };

				var w = new Wrapper<Language>(lang, lang.Description);
				cmbLanguage.Items.Add(w);
				cmbLanguage.SelectedItem = w;
			}
		}
	}

	private void SaveClicked(object sender, RoutedEventArgs e)
	{
		var langW = cmbLanguage.SelectedItem as Wrapper<Language>;
		var fileW = cmbFile.SelectedItem as Wrapper<string>;
		if (langW == null || fileW == null) return;

		var enFile = fileW.Object;
		var targetLang = langW.Object;

		var prefix = Path.GetFileName(enFile.Substring(0, enFile.Length - 8));
		var langFile = $"{prefix}.{targetLang.Code.ToLower()}.json";

		var file = new JObject();

		file.Add("@Meta", new JObject
		{
			["Base"] = prefix,
			["Language"] = targetLang.Code ?? "",
			["LanguageDescription"] = targetLang.Description ?? "",
			["Inherit"] = targetLang.Inherit ?? ""
		});

		var settings = Items.Where(x => x.Type == "Setting").ToList();
		if (settings.Any())
		{
			var settingNode = new JObject();
			foreach (var ss in settings)
			{
				var k = Convert.ToString(ss.ID);
				var v = Convert.ToString(ss.Translation);
				settingNode[k] = v;
			}
			file.Add("@Settings", settingNode);
		}

		var strings = Items.Where(x => x.Type == "String").ToList();
		foreach (var ss in strings)
		{
			var k = Convert.ToString(ss.ID);
			var v = Convert.ToString(ss.Translation);
			file[k] = v;
		}


		var userLang = Path.Combine(_userTranslationsFolder, langFile);
		var output = file.ToString(Newtonsoft.Json.Formatting.Indented);

		if (!Directory.Exists(_userTranslationsFolder)) Directory.CreateDirectory(_userTranslationsFolder);
		File.WriteAllText(userLang, output);


		//MessageBox.Show("Save complete! Reset Sledge to test the changes.", "Changes saved");
	}

	private class Wrapper<T>
	{
		public T Object { get; set; }
		public string Text { get; set; }

		public Wrapper(T o, string text)
		{
			Object = o;
			Text = text;
		}

		public override string ToString() => Text;
	}
	public class DataTranslation
	{
		public string ID { get; set; }
		public string Type { get; set; }
		public string FriendlyID { get; set; }
		public string English { get; set; }
		public string Translation { get; set; }
		public DataTranslation(string id, string type, string friend, string english, string translation)
		{
			ID = id;
			Type = type;
			FriendlyID = friend;
			English = english;
			Translation = translation;
		}
	}
}
