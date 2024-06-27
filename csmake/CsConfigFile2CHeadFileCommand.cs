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
using static System.Collections.Specialized.BitVector32;

namespace csmake
{
    
    [Verb("cs2header", HelpText = "show menu config gui ")]
    [Export(typeof(ICommand))]
    public class CsConfigFile2CHeadFileCommand : ICommand
    {
        [Option('d', "UserConfigDescriptionFile", HelpText = "User Config Description file")]
        public string UserConfigDescriptionFile { get; set; } = "UserConfigDescriptionFile.csx";

        [Option('u', "UserConfigFile", HelpText = "user config file")]
        public string UserConfigFile { get; set; } = "CsConfig.json";

        [Option('o', "OutPutFile", HelpText = "OutPutFile")]
        public string OutPutFile { get; set; } = "config.h";

        private Assembly UserScriptDescriptionAssembly;

        private Assembly CsConfigAssembly;
        StreamWriter sw;
        private void CodeGen_Config(IItem item)
        {
            var valuePI = item.GetType().GetProperty("Value");
            Type vt = valuePI.PropertyType;

            var isHexShowPi = item.GetType().GetProperty("IsHexShow");
            var val = valuePI.GetValue(item);
            bool isHexShow = (bool)isHexShowPi.GetValue(item);
            var macroName = (string)(item.GetType().GetProperty("MacroName").GetValue(item));

            if (vt == typeof(byte)
            || vt == typeof(sbyte)
            || vt == typeof(UInt16)
            || vt == typeof(Int16)
            || vt == typeof(UInt32)
            || vt == typeof(Int32)
            || vt == typeof(UInt64)
            || vt == typeof(Int64))
            {

                if (isHexShow)
                {
                    sw.WriteLine($"#define {macroName} 0x{val:X}");
                }
                else
                {
                    sw.WriteLine($"#define {macroName} {val}");
                }
            }
            else if (vt == typeof(string))
            {
                sw.WriteLine($"#define {macroName} \"{val}\"");
            }
            else
            {
                sw.WriteLine($"#define {macroName} {val}");
            }
        }
        private void RecursiveMenuAction(IItem item)
        {
            var t = item.GetType();

            if (t.IsGenericType)
            {
                var configType = t.GetGenericTypeDefinition();
                if (configType == typeof(Config<>))
                {
                    CodeGen_Config(item);
                }
                    
            }
            else if (item is Choice)
            {
                var c = ((item as Choice)).SelectedItem;
                if (c is IMenu menu)
                {
                    CSConfig.Parser.RecursiveMenu(menu, RecursiveMenuAction);
                }
                else
                {
                    CodeGen_Config(c);
                }
            }
            else if(item is IMenu menu)
            {
                CSConfig.Parser.RecursiveMenu(menu, RecursiveMenuAction);
            }
        }
        public int Execute()
        {
            try
            {
                var content = File.ReadAllText(UserConfigDescriptionFile);
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;
                var eva = CSScript.Evaluator;
                CsConfigAssembly = typeof(IItem).Assembly;
                CSScript.RoslynEvaluator.ReferenceAssembly(CsConfigAssembly);
                CSScript.RoslynEvaluator.ReferenceDomainAssemblies();
                UserScriptDescriptionAssembly = CSScript.RoslynEvaluator.CompileCode(content, new CompileInfo { CodeKind = SourceCodeKind.Script, AssemblyName = "csmake.userscript" });
                var  ( UserScriptDescriptionMenuInstance,UserConfig) = CSConfig.Parser.Parse(UserScriptDescriptionAssembly,UserConfigFile);
                FileStream fs = new FileStream(OutPutFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                sw=new StreamWriter(fs);
                string fnWithOutExtension = Path.GetFileNameWithoutExtension(OutPutFile);
                if(UserConfig!=null)
                {
                    sw.WriteLine($"#ifndef {fnWithOutExtension}_H");
                    sw.WriteLine($"#define {fnWithOutExtension}_H");
                    
                    sw.WriteLine();
                    sw.WriteLine("#include \"stdbool.h\"");

                    sw.WriteLine();
                    //todo codegen user define struct 
                    //todo codegen user define enum

                    sw.WriteLine();
                    sw.WriteLine("//User Config:");
                    CSConfig.Parser.RecursiveMenu(UserScriptDescriptionMenuInstance,RecursiveMenuAction);
                    sw.WriteLine();
                    sw.WriteLine($"#endif //{fnWithOutExtension}_H");
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                
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
