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

                // -u choice_stm32.json

                var content = File.ReadAllText(UserConfigDescriptionFile);
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;
                var eva = CSScript.Evaluator;
                CsConfigAssembly = typeof(IItem).Assembly;
                CSScript.RoslynEvaluator.ReferenceAssembly(CsConfigAssembly);
                CSScript.RoslynEvaluator.ReferenceDomainAssemblies();
                UserScriptDescriptionAssembly = CSScript.RoslynEvaluator.CompileCode(content, new CompileInfo { CodeKind = SourceCodeKind.Script, AssemblyName="aaa"});
                IMenu userConfig = null;
                (App.UserScriptDescriptionMenuInstance,userConfig) = CSConfig.Parser.Parse(UserScriptDescriptionAssembly,UserConfigFile);
                if(true)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects // 必须与序列化时一致  
                    };

                    var str=JsonConvert.SerializeObject(App.UserScriptDescriptionMenuInstance,Newtonsoft.Json.Formatting.Indented, settings);
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
