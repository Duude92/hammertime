using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Sledge.Shell.Forms;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;

namespace Sledge.Shell;

public partial class SingleInstanceAv : Application
{
	private CompositionContainer _container;

	internal ShellAv Shell { get; set; }
	public SingleInstanceAv()
	{
		
	}
	public SingleInstanceAv(CompositionContainer container) :this()
	{
		_container = container;
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		// Line below is needed to remove Avalonia data validation.
		// Without this line you will get duplicate validations from both Avalonia and CT
		BindingPlugins.DataValidators.RemoveAt(0);
		if (_container is null)
		{
			base.OnFrameworkInitializationCompleted();

			return;
		}


		Shell = _container.GetExport<Forms.ShellAv>().Value;


		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = Shell;
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		{
			singleViewPlatform.MainView = Shell;
		}

		base.OnFrameworkInitializationCompleted();
	}
}