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
                UserScriptDescriptionAssembly = CSScript.RoslynEvaluator.CompileCode(content, new CompileInfo { CodeKind = SourceCodeKind.Script });
                var  ( UserScriptDescriptionMenuInstance,UserConfig) = CSConfig.Parser.Parse(UserScriptDescriptionAssembly,UserConfigFile);
                FileStream fs = new FileStream(OutPutFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter sw=new StreamWriter(fs);
                string fnWithOutExtension = Path.GetFileNameWithoutExtension(OutPutFile);
                if(UserConfig!=null)
                {
                    sw.WriteLine($"#ifndef {fnWithOutExtension}_H");
                    sw.WriteLine($"#define {fnWithOutExtension}_H");
                    
                    sw.WriteLine();
                    sw.WriteLine("#ifndef True");
                    sw.WriteLine("#define True 1");
                    sw.WriteLine("#define False 0");
                    sw.WriteLine("#endif");

                    sw.WriteLine();
                    sw.WriteLine("#ifndef Build_Tristate_Type");
                    sw.WriteLine("#define Build_Tristate_Type");
                    sw.WriteLine("#define Build_Tristate_Type_N 0");
                    sw.WriteLine("#define Build_Tristate_Type_Y 1");
                    sw.WriteLine("#define Build_Tristate_Type_M 2");
                    sw.WriteLine("#endif");

                    sw.WriteLine();
                    sw.WriteLine("//User Config:");
                    CSConfig.Parser.RecursiveMenu(UserScriptDescriptionMenuInstance, (item) =>
                    {
                        Config c = null;
                        if(item is Config)
                        {
                            c= (Config)item;
                        }
                        else if(item is Choice)
                        {
                            c = ((item as Choice)).SelectedConfig;
                        }
                        if (c != null)
                        {
                            if (c.IsHexShow)
                            {
                                sw.WriteLine($"#define {c.Key} 0x{c.Value:X}");
                            }
                            else
                            {
                                if (c.ConfigType == ConfigType.Tristate)
                                {
                                    sw.WriteLine($"#define {c.Key} {c.Value.ToString()}");
                                }
                                else if (c.ConfigType == ConfigType.String)
                                {
                                    sw.WriteLine($"#define {c.Key} \"{c.Value.ToString()}\"");
                                }
                                else
                                {
                                    sw.WriteLine($"#define {c.Key} {c.Value.ToString()}");
                                }

                            }
                        }
                        
                    });
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
