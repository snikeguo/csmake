using CommandLine;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
namespace csmake
{

    public interface ICommand
    {
        int Execute();
    }
    internal class ComponentManager
    {
        public static int CommandExecute(string[] args)
        {
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(assemblyCatalog);
            var container = new CompositionContainer(aggregateCatalog);
            var exports = container.GetExportedValues<ICommand>();
            var parseResult = Parser.Default.ParseArguments(args, exports.Select(x => x.GetType()).ToArray())
                .MapResult<ICommand, int>(
                x =>
                {
                    var handler = exports.First(h => h.GetType()== x.GetType());
                    var properties=handler.GetType().GetProperties();
                    foreach (var property in properties)
                    { 
                        var xv=property.GetValue(x);
                        property.SetValue(handler, xv);
                    }
                    return (int)handler.Execute();
                },
                NotParsed
                );
            return parseResult;
        }
        public static int NotParsed(IEnumerable<Error> es)
        {
            foreach (var e in es)
            {
                Console.WriteLine(e.ToString());
            }
            return 0;
        }
    }
}
