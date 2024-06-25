using Avalonia.Controls;
using System.Reflection;
using CSConfig;
using System.Linq;
using System;
using Microsoft.VisualBasic;
using System.Diagnostics;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls.Templates;
using System.Collections.Generic;
using DynamicData;
using Avalonia.Platform.Storage;
using System.IO;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Avalonia.Data.Converters;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Newtonsoft.Json;
using ReactiveUI;
namespace AvaloniaFrontEnd.Views;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
       KConfigRendering(App.UserScriptDescriptionMenuInstance);
    }
    public void KConfigRendering(IMenu userScriptDescriptionMenu)
    {  
        var mainGuiMenu = new MenuNode();
        mainGuiMenu.Parent = null;
        KConfigRenderingInternal(mainGuiMenu, userScriptDescriptionMenu);
        KconfigTreeView.Items.Add(mainGuiMenu);
    }
    private void CallItemValueChanrged(object guiObj,object property)
    {
        IMenu menu = (IMenu)guiObj;
        menu.ItemValueChanged(property as IItem);
    }
    
    public void KConfigRenderingInternal(MenuNode guiMenu, IMenu userMenu)
    {
        var properties = userMenu.GetType().GetProperties();
        guiMenu.Source = userMenu;
        foreach (var property in properties)
        {
            var itemAttr = property.GetCustomAttribute<ItemConfigAttribute>();
            if (itemAttr != null)
            {
                var value = property.GetValue(userMenu);

                if (property.PropertyType == typeof(Config))
                {
                    Config config = (Config)value;
                    var configNode = new MenuNode();
                    configNode.Source = config;
                    configNode.Parent = guiMenu;
                    guiMenu.Items.Add(configNode);
                }
                else if (property.PropertyType == typeof(Choice))
                { 
                    Choice choice = (Choice)value;
                    var choiceNode=new MenuNode();
                    choiceNode.Source = choice;
                    choiceNode.Parent = guiMenu;
                    guiMenu.Items.Add(choiceNode);
                }
                else if (property.PropertyType.GetInterfaces().Contains(typeof(IMenu)))
                {
                    IMenu subMenu = (IMenu)value;
                    Debug.WriteLine($"Menu:{subMenu.Name}");
                    var subGuiMenu=new MenuNode();
                    subGuiMenu.Parent = guiMenu;
                    KConfigRenderingInternal(subGuiMenu, subMenu);
                    guiMenu.Items.Add(subGuiMenu);
                }
            }
        }
    }

    private MenuNode CurrentMenuNode = null;
    private void KconfigTreeView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        KconfigTreeView.Focus();

        var tv = sender as TreeView;
        var tvItem = tv.SelectedItem as MenuNode;
        HelpTextBlock.Text = tvItem.Source.Help;
        CurrentMenuNode = tvItem;
        if (tvItem.Source is Config config)
        {
            if (config.Value.GetType() == typeof(bool))
            {
                SelectedItemTextBox.Text = "";
                SelectedItemTextBox.IsEnabled = false;
                SelectedItemComboBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null;
                SelectedItemComboBox.Items.Clear();
                SelectedItemComboBox.ItemsSource = new List<bool> { false, true };
                var value = false;
                value = (bool)config.Value;
                if (value == false)
                {
                    SelectedItemComboBox.SelectedIndex = 0;
                }
                else
                {
                    SelectedItemComboBox.SelectedIndex = 1;
                }
            }
            else if (config.Value.GetType() == typeof(Enum))
            {
                SelectedItemTextBox.Text = "";
                SelectedItemTextBox.IsEnabled = false;
                SelectedItemComboBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null;
                var enumValues = Enum.GetValues(config.Value.GetType());
                SelectedItemComboBox.ItemsSource=enumValues;

                foreach (var enumval in enumValues)
                {
                    if(enumval==config.Value)
                    {
                        SelectedItemComboBox.SelectedItem=config.Value;
                    }
                }
            }
            else if (config.Value.GetType().IsValueType)
            {
                object v = config.Value;
                if (config.IsHexShow)
                {
                    SelectedItemTextBox.Text = $"0x{v:X}";
                }
                else
                {
                    SelectedItemTextBox.Text = v.ToString(); ;
                }
                SelectedItemTextBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null;
                SelectedItemComboBox.IsEnabled = false;
            }
            else if (config.Value.GetType() == typeof(string))
            {
                SelectedItemTextBox.Text = (string)config.Value;

                SelectedItemTextBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null; 
                SelectedItemComboBox.IsEnabled = false;
            }
            
            
        }
        else if(tvItem.Source is Choice choice)
        {
            SelectedItemTextBox.Text = "";
            SelectedItemTextBox.IsEnabled = false;
            SelectedItemComboBox.ItemsSource= null;
            //SelectedItemComboBox.Items.Clear();
            List<string> configNamesOfChoice= new List<string>();   
            foreach (var item in choice.Configs)
            {
                configNamesOfChoice.Add(item.Name);
            }
            SelectedItemComboBox.ItemsSource=configNamesOfChoice;
            foreach (var lc in choice.Configs)
            {
                if (lc.Name == choice.SelectedConfig.Name)
                {
                    SelectedItemComboBox.SelectedItem = lc.Name;
                    break;
                }
            }

            SelectedItemComboBox.IsEnabled = true;
        }
        else
        {
            SelectedItemTextBox.Text = "";
            SelectedItemTextBox.IsEnabled = false;
            SelectedItemComboBox.ItemsSource = null;
            SelectedItemComboBox.IsEnabled = false;
        }
    }

    private async void SaveConfigButton_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);

        // Start async operation to open the dialog.
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Text File",
        });

        if (file is not null)
        {
            var manGuiMenu = ((MenuNode)KconfigTreeView.Items[0]).Source;
            var json= Newtonsoft.Json.JsonConvert.SerializeObject(manGuiMenu, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(file.Path.LocalPath, json);
            var box = MessageBoxManager.GetMessageBoxStandard("Caption", "save ok",ButtonEnum.Ok); 
            var result = await box.ShowAsync();
        }
    }

    private async void SelectedItemTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
#if false
        if(CurrentMenuNode!=null)
        {
            Config config = CurrentMenuNode.Source as Config;
            if(config.ConfigType== ConfigType.String|| config.ConfigType== ConfigType.String)
            {
                config.Value = SelectedItemTextBox.Text;
                CallItemValueChanrged(CurrentMenuNode.Parent.Source, config);
            }
            else if(config.ConfigType == ConfigType.Int)
            {
                if (config.IsHexShow)
                {
                    config.Value = Convert.ToInt64(SelectedItemTextBox.Text, 16);
                }
                else
                {
                    config.Value = Convert.ToInt64(SelectedItemTextBox.Text, 10);
                }   
            }
            else if (config.ConfigType == ConfigType.UInt)
            {
                if (config.IsHexShow)
                {
                    config.Value = Convert.ToUInt64(SelectedItemTextBox.Text, 16);
                }
                else
                {
                    config.Value = Convert.ToUInt64(SelectedItemTextBox.Text, 10);
                }
            }
            else if (config.ConfigType == ConfigType.Double)
            {
                config.Value = Convert.ToDouble(SelectedItemTextBox.Text);
            }
            try
            {
                CallItemValueChanrged(CurrentMenuNode.Parent.Source, config);
            }
            catch (Exception)
            {

                var box = MessageBoxManager.GetMessageBoxStandard("Caption", "input number error", ButtonEnum.Ok);
                var result = await box.ShowAsync();
            }
        }
#endif
    }
    private void SelectedItemComboBox_LostFocus(object? sender, RoutedEventArgs e)
    {
#if false
        if (CurrentMenuNode != null)
        {
            Config config = CurrentMenuNode.Source as Config;
            if (config != null)
            {
                if (config.ConfigType == ConfigType.Bool || config.ConfigType == ConfigType.Tristate)
                {
                    config.Value = SelectedItemComboBox.SelectedItem;
                    CallItemValueChanrged(CurrentMenuNode.Parent.Source, config);
                }
            }
            else
            {
                Choice choice=CurrentMenuNode.Source as Choice;
                if(choice!=null && SelectedItemComboBox.SelectedItem!=null)
                {
                    foreach (var lc in choice.Configs)
                    {
                        if (lc.Name == SelectedItemComboBox.SelectedItem.ToString())
                        {
                            choice.SelectedConfig = lc;
                            break;
                        }
                    }
                    CallItemValueChanrged(CurrentMenuNode.Parent.Source, choice);
                }
            }
              
        }
#endif
    }

}
public class MenuNode
{
    public IItem Source { get; set; }
    public List<MenuNode> Items { get; set; } = new List<MenuNode>();
    public MenuNode Parent { get; set; }
    public string Header => Source.Name;
  
}
#if false
public class IsEnableOfTextBox : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        MenuNode menuNode = value as MenuNode;
        if (menuNode == null) return false;
        if (menuNode.Source is IMenu)
            return false;
        if (menuNode.Source is Choice)
            return false;
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class IsEnableOfComboBox : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class SelectedItemChanged : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class TextChanged:IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        MenuNode menuNode = value as MenuNode;
        if (menuNode == null) return null;
        if (menuNode.Source is Config config)
        {
            if (config.ConfigType == ConfigType.String)
            {
                return (string)config.DefalutValue;
            }
            else if (config.ConfigType == ConfigType.Int)
            {
                return ((int)config.DefalutValue).ToString();
            }
            else if (config.ConfigType == ConfigType.Hex)
            {
                int v = (int)config.DefalutValue;
                return $"0x{v:X}";
            }
        }
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;
        return value;
    }
}
#endif