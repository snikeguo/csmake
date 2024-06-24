using Avalonia.Controls;
using System.Reflection;
using CsConfig;
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
using System.ComponentModel;
using PropertyModels.Collections;
namespace AvaloniaFrontEnd.Views;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        
    }
    private void SaveConfigButton_Click(object? sender, RoutedEventArgs e)
    {

    }
}
#if false
[TypeConverter(typeof(ExpandableObjectConverter))]
public class Menu2 : IMenu
{
    [DisplayName("boolValue")]
    public bool boolValue { get; set; }

    [DisplayName("intValue")]
    public int intValue { get; set; }

    [DisplayName("stringValue")]
    public string stringValue { get; set; }

    [DisplayName("floatValue")]
    public float floatValue { get; set; }

    [DisplayName("stringList")]
    public SelectableList<string> stringList { get; set; } = new SelectableList<string>() { "1", "2" };

    [DisplayName("classList")]
    public SelectableList<Board> classList { get; set; } = new SelectableList<Board>(){
        new Board(){ Name="TC397"},
    };


    [DisplayName("enumValue")]
    public Tristate enumValue { get; set; }

    public void ItemValueChanged(object item)
    {
        throw new NotImplementedException();
    }
}
[TypeConverter(typeof(ExpandableObjectConverter))]
public class Menu1:IMenu
{
    [DisplayName("boolValue")]
    public bool boolValue { get; set; }

    [DisplayName("intValue")]
    public int intValue { get; set; }

    [DisplayName("stringValue")]
    public string stringValue { get; set; }

    [DisplayName("floatValue")]
    public float floatValue { get; set; }

    [DisplayName("stringList")]
    public SelectableList<string> stringList { get; set; } = new SelectableList<string>() { "1", "2" };

    [DisplayName("classList")]
    public SelectableList<Board> classList { get; set; } = new SelectableList<Board>(){
        new Board(){ Name="TC397"},
    };


    [DisplayName("enumValue")]
    public Tristate enumValue { get; set; }

    [DisplayName("menu2")]
    public Menu2 Menu2 { get; set; } = new Menu2();

    public void ItemValueChanged(object item)
    {
        throw new NotImplementedException();
    }
}


public class Board
{
    [DisplayName("Name")]
    public string Name { get; set; }
    public override string ToString()
    {
        return Name;
    }
}
[MainMenu]
public class MyMainMenu : IMenu
{
    public static MyMainMenu Instance { get; set; }=new MyMainMenu();

    [DisplayName("boolValue")]
    public bool boolValue { get; set; }

    [DisplayName("intValue")]
    public int intValue { get; set; }

    [DisplayName("stringValue")]
    public string stringValue { get; set; }

    [DisplayName("floatValue")]
    public float floatValue { get; set; }

    [DisplayName("stringList")]
    public SelectableList<string> stringList { get; set; } = new SelectableList<string>() { "1", "2" };

    [DisplayName("classList")]
    public SelectableList<Board> classList { get; set; }=new SelectableList<Board>(){
        new Board(){ Name="TC397"},
    };

    [DisplayName("menu1")]
    public Menu1 Menu1 { get; set; }=new Menu1();

    [DisplayName("enumValue")]
    public Tristate enumValue { get; set; }

    public void ItemValueChanged(object item)
    {
        throw new NotImplementedException();    
    }
}
#endif