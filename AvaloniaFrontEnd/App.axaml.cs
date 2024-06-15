using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using AvaloniaFrontEnd.ViewModels;
using AvaloniaFrontEnd.Views;
using CSConfig;
using System.Reflection;

namespace AvaloniaFrontEnd;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                //DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            //singleViewPlatform.MainView = new MainView
            {
                //DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    public static Assembly CsScriptDescriptionAssembly { get; set; }
    public static Assembly CsConfigAssembly { get; set; }
    public static string UserConfigString { get; set; }
}
