using Avalonia;
using Avalonia.ReactiveUI;
using AvaloniaFrontEnd;
using CommandLine;
using CsConfig;
using CSScriptLib;
using Microsoft.CodeAnalysis;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace csmake
{

    [Verb("menuconfig", HelpText = "show menu config gui ")]
    [Export(typeof(ICommand))]
    public class MenuConfigCommand : ICommand
    {
        [Option('d', "UserConfigDescriptionFile", HelpText = "User Config Description file")]
        public string UserConfigDescriptionFile { get; set; } = "UserConfigDescriptionFile.csx";

        [Option('u', "UserConfigFile", HelpText = "user config file")]
        public string UserConfigFile { get; set; } = "CsConfig.json";

        private Assembly UserScriptDescriptionAssembly;

        private Assembly CsConfigAssembly;

        public int Execute()
        {
            try
            {
                var content = File.ReadAllText(UserConfigDescriptionFile);
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;
                var eva = CSScript.Evaluator;
                CsConfigAssembly = typeof(IMenu).Assembly;
                CSScript.RoslynEvaluator.ReferenceAssembly(CsConfigAssembly);
                CSScript.RoslynEvaluator.ReferenceDomainAssemblies();
                UserScriptDescriptionAssembly = CSScript.RoslynEvaluator.CompileCode(content, new CompileInfo { CodeKind = SourceCodeKind.Script });
                IMenu userConfig = null;
                (App.UserScriptDescriptionMenuInstance,userConfig) = CsConfig.Parser.Parse(UserScriptDescriptionAssembly, UserConfigFile);

                var appBuilder = AppBuilder.Configure<App>()
               .UsePlatformDetect()
               .WithInterFont()
               .LogToTrace()
               .UseReactiveUI().StartWithClassicDesktopLifetime(null);

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
