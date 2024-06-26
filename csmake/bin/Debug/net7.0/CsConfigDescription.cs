using CSConfig;
using System;
using csmake;
using System.Collections.Generic;
//子菜单
public class RamMenu : IMenu
{
    public string DisplayName => "RamMenu";
    public string Help => "RamMenu help";
    [ItemConfig]
    public Config<int> RamConfig { get; set; } = new Config<int> { MacroName = "CONFIG_RAM_ADDRESS", IsHexShow = true, Value = 0x0, Help = "ram start address!", DisplayName = "Ram Menu1Config" };
    public void ItemValueChanged(IItem cfg)
    {

    }
}
public enum Tristate
{
    N,
    Y,
    M,
}

public class STM32F103Menu : IMenu
{
    public string DisplayName => "STM32F103Menu";
    [ItemConfig]
    public Config<bool> BoardNameConfig { get; set; } = new Config<bool> { MacroName = "CONFIG_STM32F103_BOARD", IsReadOnly = true, DisplayName = "BoolType", Value = true };

    [ItemConfig]
    public Config<Tristate> TristateConfig { get; set; } = new Config<Tristate> { MacroName = "CONFIG_Tristate_VALUE", DisplayName = "TristateType", Value = Tristate.Y };

    [ItemConfig]
    public Config<string> StringConfig { get; set; } = new Config<string> { MacroName = "CONFIG_STRING_VALUE", DisplayName = "StringType", Value = "123" };

    [ItemConfig]
    public Config<int> IntConfig { get; set; } = new Config<int> { MacroName = "CONFIG_Int_VALUE", DisplayName = "IntType", Value = 1 };

    [ItemConfig]
    public Config<int> IntHexConfig { get; set; } = new Config<int> { MacroName = "CONFIG_IntHex_VALUE", DisplayName = "IntHexType", Value = 1, IsHexShow = true };

    [ItemConfig]
    public Config<uint> UIntConfig { get; set; } = new Config<uint> { MacroName = "CONFIG_UInt_VALUE", DisplayName = "UIntType", Value = 1 };


    [ItemConfig]
    public Config<uint> UIntHexConfig { get; set; } = new Config<uint> { MacroName = "CONFIG_UIntHex_VALUE", DisplayName = "UIntHexType", Value = 1, IsHexShow = true };

    [ItemConfig]
    public Config<double> DoubleConfig { get; set; } = new Config<double> { MacroName = "CONFIG_Double_VALUE", DisplayName = "DoubleType", Value = 1.3 };

    [ItemConfig]
    public Choice ChoiceConfig { get; set; }

    [ItemConfig]
    public RamMenu RamMenu { get; set; }

    public STM32F103Menu()
    {
        var items = new List<IItem>()
        {
            new Config<bool>() {MacroName = "CONFIG_CHIP_TC397",  Value = false,  Help = "英飞凌的板子 中文测试", DisplayName = "Tc397" },
            new Config<bool>()  {MacroName = "CONFIG_CHIP_S32K", Value = false,  DisplayName = "s32k" },
        };
        ChoiceConfig = new Choice()
        {
            Help = "choice help",
            DisplayName = "choice DisplayName",
        };
        ChoiceConfig.Items = items as List<IItem>;
        ChoiceConfig.SelectedItem = ChoiceConfig.Items[0];
    }
    public void ItemValueChanged(IItem cfg)
    {
        if (cfg == IntConfig)
        {
            //gInstnace.IntHexConfig.Value = IntConfig.Value;
        }
    }
}
public class S32KMenu : IMenu
{
    public string DisplayName => "S32KMenu";
    [ItemConfig]
    public Config<bool> BoardNameConfig { get; set; } = new Config<bool> { MacroName = "CONFIG_S32K_BOARD", IsReadOnly = true, DisplayName = "BoolType", Value = true };

    [ItemConfig]
    public RamMenu RamMenu { get; set; }
    public void ItemValueChanged(IItem cfg)
    {

    }
}
//主菜单
[MainMenu]
public class MyMainMenu : IMenu
{
    public static MyMainMenu gInstnace { get; set; } = new MyMainMenu();
    [ItemConfig]
    public Choice ChipChoice { get; set; }
    public MyMainMenu()
    {
        var stm32f103menu = new STM32F103Menu();
        var s32kmenu = new S32KMenu();
        ChipChoice = new Choice()
        {
            Items = new List<IItem>() { stm32f103menu, s32kmenu },
            SelectedItem = stm32f103menu,
            Help = "choice a chip",
            DisplayName = "CHOICE CHIP",
        };
    }
    public void ItemValueChanged(IItem cfg)
    {

    }
}