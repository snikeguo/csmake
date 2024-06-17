using Avalonia;
using AvaloniaFrontEnd;
using CommandLine;
using CSConfig;
using CSScriptLib;
using Microsoft.CodeAnalysis;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace csmake
{
    public static class Host
    {
        public static void WriteLine(string format, params string[]d)
        {
            Debug.WriteLine(format, d);
        }
    }
    [Verb("CsConfig", HelpText = "show cs config gui ")]
    [Export(typeof(ICommand))]
    public class CsConfigCommand : ICommand
    {
        [Option('d', "CsConfigDescriptionFile", HelpText = "CsConfig Description file")]
        public string CsConfigDescriptionFile { get; set; } = "CsConfigDescription.csx";

        [Option('u', "UserConfigFile", HelpText = "user config file")]
        public string UserConfigFile { get; set; } = "CsConfig.json";

        private Assembly CsScriptDescriptionAssembly;

        // Avalonia configuration, don't remove; also used by visual designer.
        public int Execute()
        {
            try
            {

               

                var content = File.ReadAllText(CsConfigDescriptionFile);
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;
                var eva = CSScript.Evaluator;
                var csConfigAssembly = typeof(IItem).Assembly;
                CSScript.RoslynEvaluator.ReferenceAssembly(csConfigAssembly);
                CSScript.RoslynEvaluator.ReferenceDomainAssemblies();
                CsScriptDescriptionAssembly = CSScript.RoslynEvaluator.CompileCode(content, new CompileInfo { CodeKind = SourceCodeKind.Script });

                App.CsScriptDescriptionAssembly= CsScriptDescriptionAssembly;
                App.CsConfigAssembly = csConfigAssembly;
                if(File.Exists(UserConfigFile))
                {
                    var userConfigText = File.ReadAllText(UserConfigFile);
                    App.UserConfigString = userConfigText;
                }
                
                var appBuilder = AppBuilder.Configure<App>()
               .UsePlatformDetect()
               .WithInterFont()
               .LogToTrace()
               .UseReactiveUI().StartWithClassicDesktopLifetime(null);

                Console.ReadLine();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生异常:{ex.ToString()}");
            }
            return 1;
        }


    }

    
}
