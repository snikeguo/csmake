using Avalonia;
using Avalonia.ReactiveUI;
using AvaloniaFrontEnd;
using System;
using System.Diagnostics;
using System.Reflection;
namespace csmake;
public static class Host
{
    public static void WriteLine(string format, params string[] d)
    {
        Debug.WriteLine(format, d);
    }
}
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
