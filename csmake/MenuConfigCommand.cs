using Avalonia;
using Avalonia.ReactiveUI;
using AvaloniaFrontEnd;
using CommandLine;
using CSConfig;
using CSScriptLib;
using Microsoft.CodeAnalysis;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json;
namespace csmake
{
    
    [Verb("menuconfig", HelpText = "show menu config gui ")]
    [Export(typeof(ICommand))]
    public class MenuConfigCommand : ICommand
    {
        [Option('d', "MainMenuDescriptionFile", HelpText = "Main Menu Description File")]
        public string MainMenuDescriptionFile { get; set; } = "MainMenuDescription.csx";

        [Option('u', "UserConfigFile", HelpText = "user config file")]
        public string UserConfigFile { get; set; } = "CsConfig.json";

        private Assembly MenuAssembly;

        private Assembly UserConfigAssembly;

        
        public int Execute()
        {
            try
            {

                // -u choice_stm32.json
                Stopwatch sw = Stopwatch.StartNew();
                var content = File.ReadAllText(MainMenuDescriptionFile);
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;
                var eva = CSScript.Evaluator;
                UserConfigAssembly = typeof(IItem).Assembly;
                CSScript.RoslynEvaluator.ReferenceAssembly(UserConfigAssembly);
                CSScript.RoslynEvaluator.ReferenceDomainAssemblies();
                
                Console.WriteLine($"compiling {MainMenuDescriptionFile}.");
                
                MenuAssembly = CSScript.RoslynEvaluator.CompileCode(content, new CompileInfo { CodeKind = SourceCodeKind.Script, AssemblyName="csmake.userscript"});
                
                IMenu userConfig = null;
                
                (App.MenuInstance,userConfig) = CSConfig.Parser.Parse(MenuAssembly,UserConfigFile);

                sw.Stop();
                Console.WriteLine($"Parse time span:{sw.Elapsed}");

                if (false)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects // 必须与序列化时一致  
                    };

                    var str=JsonConvert.SerializeObject(App.MenuInstance,Newtonsoft.Json.Formatting.Indented, settings);
                    File.WriteAllText("choice_stm32.json", str);
                }

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
