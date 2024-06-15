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
namespace AvaloniaFrontEnd.Views;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
    }
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
       await KConfigRendering(App.CsScriptDescriptionAssembly, App.CsConfigAssembly, App.UserConfigString);
    }
    public async Task KConfigRendering(Assembly cssriptAssembly, Assembly csConfigAssembly, string userConfigStr)
    {
        var allTypes = cssriptAssembly.GetTypes();
        CSConfig.Menu mainMenu = null;
        foreach (var type in allTypes)
        {
            var mainMenuAttr = type.GetCustomAttribute<MainMenuAttribute>();
            if (mainMenuAttr != null)
            {
                bool isCreate = false;

                if(string.IsNullOrEmpty(userConfigStr))
                {
                    isCreate = true;    
                }
                else
                {
                    try
                    {
                        var temp = JsonConvert.DeserializeObject(userConfigStr, type);
                        mainMenu = (CSConfig.Menu)temp;
                    }
                    catch (Exception)
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Caption", "cs config type != user config type!", ButtonEnum.Ok);
                        await box.ShowAsync();
                        isCreate = true;
                    }
                }
                if (isCreate)
                {
                    mainMenu = (CSConfig.Menu)cssriptAssembly.CreateInstance(type.FullName);
                }
                break;
            }
        }
        var mainGuiMenu = new MenuNode();
        KConfigRenderingInternal(mainGuiMenu, mainMenu, cssriptAssembly, csConfigAssembly);
        
        KconfigTreeView.Items.Add(mainGuiMenu);
    }
    public void KConfigRenderingInternal(MenuNode guiMenu, CSConfig.Menu menu, Assembly cssriptAssembly,Assembly csConfigAssembly)
    {
        var properties = menu.GetType().GetProperties();
        guiMenu.Source = menu;
        foreach (var property in properties)
        {
            var itemAttr = property.GetCustomAttribute<ItemConfigAttribute>();
            if (itemAttr != null)
            {
                var value = property.GetValue(menu);

                if (property.PropertyType == typeof(Config))
                {
                    if (value == null)
                    {
                        value = csConfigAssembly.CreateInstance(property.PropertyType.FullName);
                        property.SetValue(menu, value);
                    }

                    Config config = (Config)value;
                    var configNode = new MenuNode();
                    configNode.Source = config;
                    guiMenu.Items.Add(configNode);
                }
                else if (property.PropertyType == typeof(Choice))
                {
                    if (value == null)
                    {
                        value = csConfigAssembly.CreateInstance(property.PropertyType.FullName);
                        property.SetValue(menu, value);
                    }

                    Choice choice = (Choice)value;
                    var choiceNode=new MenuNode();
                    choiceNode.Source = choice;
                    guiMenu.Items.Add(choiceNode);
                }
                else if (property.PropertyType.BaseType== typeof(CSConfig.Menu))
                {
                    if (value == null)
                    {
                        value = cssriptAssembly.CreateInstance(property.PropertyType.FullName);
                        property.SetValue(menu, value);
                    }

                    CSConfig.Menu subMenu = (CSConfig.Menu)value;
                    Debug.WriteLine($"Menu:{subMenu.Name}");
                    var subGuiMenu=new MenuNode();
                    KConfigRenderingInternal(subGuiMenu, subMenu, cssriptAssembly, csConfigAssembly);
                    guiMenu.Items.Add(subGuiMenu);
                }
            }
        }
        return ;
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
            if (config.ConfigType == ConfigType.Bool)
            {
                SelectedItemTextBox.Text = "";
                SelectedItemTextBox.IsEnabled = false;
                SelectedItemComboBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null;
                SelectedItemComboBox.ItemsSource = new List<bool> { false, true };
                var value = false;
                if (config.Value == null)
                {
                    value = (bool)config.DefalutValue;
                }
                else
                { 
                    value= (bool)config.Value;
                }
                if (value == false)
                {
                    SelectedItemComboBox.SelectedIndex = 0;
                }
                else
                {
                    SelectedItemComboBox.SelectedIndex = 1;
                }
            }
            else if (config.ConfigType == ConfigType.Tristate)
            {
                SelectedItemTextBox.Text = "";
                SelectedItemTextBox.IsEnabled = false;
                SelectedItemComboBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null;
                SelectedItemComboBox.ItemsSource = new List<Tristate> { Tristate.N, Tristate.Y, Tristate.M };

                var value = Tristate.N;
                if(config.Value == null)
                {
                    value = (Tristate)config.DefalutValue;
                }
                else
                {
                    value=(Tristate)config.Value;
                }
                if (value == Tristate.N)
                {
                    SelectedItemComboBox.SelectedIndex = 0;
                }
                else if (value == Tristate.Y)
                {
                    SelectedItemComboBox.SelectedIndex = 1;
                }
                else if (value == Tristate.M)
                {
                    SelectedItemComboBox.SelectedIndex = 2;
                }
            }
            else if (config.ConfigType == ConfigType.String)
            {
                if(config.Value==null)
                {
                    SelectedItemTextBox.Text = (string)config.DefalutValue;
                }
                else
                {
                    SelectedItemTextBox.Text = (string)config.Value;
                }
                
                SelectedItemTextBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null; 
                SelectedItemComboBox.IsEnabled = false;
            }
            else if(config.ConfigType == ConfigType.Int)
            {
                if (config.Value == null)
                {
                    SelectedItemTextBox.Text = ((int)config.DefalutValue).ToString();
                }
                else
                {
                    SelectedItemTextBox.Text = ((int)config.Value).ToString();
                }
                SelectedItemTextBox.IsEnabled = true;
                SelectedItemComboBox.ItemsSource = null;
                SelectedItemComboBox.IsEnabled = false;
            }
            else if (config.ConfigType == ConfigType.Hex)
            {
                Int64 v = 0;
                if (config.Value == null)
                {
                    v =Convert.ToInt64(config.DefalutValue);
                    
                }
                else
                {
                    v = (Int64)config.Value;
                }
                SelectedItemTextBox.Text = $"0x{v:X}";
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
            SelectedItemComboBox.Items.Clear();
            SelectedItemComboBox.ItemsSource = choice.Configs;
            if(choice.SelectedConfig==null)
            {
                SelectedItemComboBox.SelectedItem = choice.DefalutConfig;
            }
            else
            {
                foreach (var lc in choice.Configs)
                {
                    if(lc.Name==choice.SelectedConfig.Name)
                    {
                        SelectedItemComboBox.SelectedItem = lc;
                        break;
                    }
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

    private async void SaveConfigButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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

    private async void SelectedItemTextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(CurrentMenuNode!=null)
        {
            Config config = CurrentMenuNode.Source as Config;
            if(config.ConfigType== ConfigType.String|| config.ConfigType== ConfigType.String)
            {
                config.Value = SelectedItemTextBox.Text;
            }
            else if(config.ConfigType == ConfigType.Int || config.ConfigType == ConfigType.Hex)
            {
                try
                {
                    config.Value = Convert.ToInt64(SelectedItemTextBox.Text, 16);
                }
                catch (Exception)
                {

                    var box = MessageBoxManager.GetMessageBoxStandard("Caption", "input number error", ButtonEnum.Ok);
                    var result = await box.ShowAsync();
                }
                
            }
        }
    }
    private void SelectedItemComboBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CurrentMenuNode != null)
        {
            Config config = CurrentMenuNode.Source as Config;
            if (config != null)
            {
                if (config.ConfigType == ConfigType.Bool || config.ConfigType == ConfigType.Tristate)
                {
                    config.Value = SelectedItemComboBox.SelectedItem;
                }
            }
            else
            {
                Choice choice=CurrentMenuNode.Source as Choice;
                if(choice!=null)
                {
                    choice.SelectedConfig = (Config)SelectedItemComboBox.SelectedItem;
                }
            }
              
        }
    }

}
public class MenuNode
{
    public IItem Source { get; set; }
    public List<MenuNode> Items { get; set; } = new List<MenuNode>();

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