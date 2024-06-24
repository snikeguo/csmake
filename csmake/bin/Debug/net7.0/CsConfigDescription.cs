using CsConfig;
using System;
using csmake;
using System.Collections.Generic;

public class CanController : IEquatable<CanController>, IMenu
{
    public int BaudRate { get; set; }
    public bool Equals(CanController other)
    {
        if (other.BaudRate == BaudRate)
            return true;
        return false;
    }
    public void ItemValueChanged(string propertyName)
    {

    }
}

//子菜单
public class Menu1 : IMenu
{

    [Item("ChoiceCanController", "desc", "CONFIG_ChoiceCanController_VALUE")]

    public SelectableList<CanController> CanControllerConfig { get; set; }

    public Menu1()
    {
        var br250k = new CanController() { BaudRate = 250 };
        var br125k = new CanController() { BaudRate = 125 };
        CanControllerConfig = new SelectableList<CanController>(new List<CanController>() { br125k, br250k }, br250k);
    }
    public void ItemValueChanged(string propertyName)
    {
       if(propertyName==nameof(CanControllerConfig))
        {
            Host.WriteLine("CanControllerConfig is changed!");
        }
    }
}

//主菜单
[MainMenu]
public class MyMainMenu : IMenu
{
    public static MyMainMenu gInstnace { get; set; } = new MyMainMenu();
    public string Name => "MyMainMenu";
    [Item("BoolConfig", "desc", "CONFIG_Bool_VALUE")]
    public bool BoolInstance { get; set; }

    [Item("TristateConfig", "desc", "CONFIG_Bool_VALUE")]
    public Tristate TristateInstance { get; set; } = Tristate.Y;

    [Item("StringConfig", "desc", "CONFIG_String_VALUE")]
    public string StringConfig { get; set; } = "123";

    [Item("ByteConfig", "desc", "CONFIG_Byte_VALUE")]
    [Hex]
    public byte ByteInstance { get; set; } = 1;

    [Item("ChoiceIntConfig", "desc", "CONFIG_ChoiceInt_VALUE")]
    public SelectableList<int> ChoiceIntConfig { get; set; } = new SelectableList<int>(new List<int>() { 1, 2, 3, 4, 5 }, 2);

    [Item("Menu1Config", "desc", "")]
    public Menu1 menu1 { get; set; }

    public void ItemValueChanged(string propertyName)
    {
        Host.WriteLine(propertyName.ToString());
        if(propertyName==nameof(BoolInstance))
        {
            Host.WriteLine("222");
            StringConfig=BoolInstance.ToString();
        }
        else if(propertyName==nameof(ByteInstance))
        {
            ChoiceIntConfig.SelectedItem=(int)ByteInstance;
            Host.WriteLine("333");
        }
    }
}
