using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaFrontEnd.ViewModels;
using CSConfig;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AvaloniaFrontEnd.Views;
namespace AvaloniaFrontEnd.DataTemplates
{
    public class ItemTemplateSelector : IDataTemplate
    {
        // This Dictionary should store our shapes. We mark this as [Content], so we can directly add elements to it later.
        [Content]
        public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new Dictionary<string, IDataTemplate>();

        // Build the DataTemplate here
        public Control Build(object? data)
        {
            var key = "Config";
            var menuNode = data as MenuNode;
            if (menuNode.Source.GetType()==typeof(Config<>))
            {
                key = "Config";
            }
            else if (menuNode.Source is Choice)
            {
                key = "Choice";
            }
            else if (menuNode.Source is IMenu)
            {
                key = "Menu";
            }
            return AvailableTemplates[key].Build(data); // finally we look up the provided key and let the System build the DataTemplate for us
        }

        // Check if we can accept the provided data
        public bool Match(object? data)
        {
            // Our Keys in the dictionary are strings, so we call .ToString() to get the key to look up
            var key = "Config";
            var menuNode=data as MenuNode;
            if (menuNode.Source.GetType() == typeof(Config<>))
            {
                key = "Config";
            }
            else if (menuNode.Source is Choice)
            {
                key = "Choice";
            }
            else if (menuNode.Source is CSConfig.IMenu)
            {
                key = "Menu";
            }
            else
            {
                return false;
            }
            var x= AvailableTemplates.ContainsKey(key); // and the key must be found in our Dictionary
            return x;
        }
    }
}
