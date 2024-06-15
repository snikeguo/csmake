using Avalonia;
using Avalonia.ReactiveUI;
using AvaloniaFrontEnd;
using System;
namespace csmake;
class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var componentExecuteResult = ComponentManager.CommandExecute(args);
        //BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
               .UsePlatformDetect()
               .WithInterFont()
               .LogToTrace()
               .UseReactiveUI();
    }
}
