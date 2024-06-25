using CSConfig;
using System;
using csmake;
using System.Collections.Generic;
//子菜单
public class Menu1 : IMenu
{
    public string DisplayName => "menu1";
    public string Help => "menu1 help";
    [ItemConfig]
    public Config Menu1Config { get; set; } = new Config() { Key = "CONFIG_RAM_ADDRESS", ConfigType = typeof(int), IsHexShow = true, Value = 0x0, Depends = null, EnableControl = null, Help = "ram start address!", DisplayName = "Ram Menu1Config" };
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
//主菜单
[MainMenu]
public class MyMainMenu : IMenu
{
    public static MyMainMenu gInstnace { get; set; } = new MyMainMenu();
    public string DisplayName => "MyMainMenu";
    [ItemConfig]
    public Config BoolConfig { get; set; } = new Config() { Key = "CONFIG_Bool_VALUE", ConfigType = typeof(bool), DisplayName = "BoolType", Value = true };

    [ItemConfig]
    public Config TristateConfig { get; set; } = new Config() { Key = "CONFIG_Tristate_VALUE", ConfigType = typeof(Tristate), DisplayName = "TristateType", Value = Tristate.Y };

    [ItemConfig]
    public Config StringConfig { get; set; } = new Config() { Key = "CONFIG_String_VALUE", ConfigType = typeof(string), DisplayName = "StringType", Value = "123" };

    [ItemConfig]
    public Config IntConfig { get; set; } = new Config() { Key = "CONFIG_Int_VALUE", ConfigType = typeof(int), DisplayName = "IntType", Value = (UInt64)1 };

    [ItemConfig]
    public Config IntHexConfig { get; set; } = new Config() { Key = "CONFIG_IntHex_VALUE", ConfigType = typeof(int), DisplayName = "IntHexType", Value = (UInt64)1, IsHexShow = true };

    [ItemConfig]
    public Config UIntConfig { get; set; } = new Config() { Key = "CONFIG_UInt_VALUE", ConfigType = typeof(uint), DisplayName = "UIntType", Value = (UInt64)1 };


    [ItemConfig]
    public Config UIntHexConfig { get; set; } = new Config() { Key = "CONFIG_UIntHex_VALUE", ConfigType = typeof(uint), DisplayName = "UIntHexType", Value = (UInt64)1, IsHexShow = true };

    [ItemConfig]
    public Config DoubleConfig { get; set; } = new Config() { Key = "CONFIG_Double_VALUE", ConfigType = typeof(double), DisplayName = "DoubleType", Value = (double)1.3 };

    [ItemConfig]
    public Choice ChoiceConfig { get; set; }

    [ItemConfig]
    public Menu1 menu1 { get; set; }

    public MyMainMenu()
    {
        ChoiceConfig = new Choice()
        {
            Configs = new List<Config>()
            {
            new Config() {Key = "CONFIG_CHIP_TC397", ConfigType = typeof(bool), Value = false, Depends = null, EnableControl = null, Help = "英飞凌的板子 中文测试", DisplayName = "Tc397" },
            new Config() {Key = "CONFIG_CHIP_S32K", ConfigType = typeof(bool), Value = false, Depends = null, EnableControl = null, DisplayName = "s32k" },

            },
            Help = "choice help",
            DisplayName = "choice DisplayName",
        };
        ChoiceConfig.SelectedConfig = ChoiceConfig.Configs[0];
    }
    public void ItemValueChanged(IItem cfg)
    {
        if (cfg == IntConfig)
        {
            gInstnace.IntHexConfig.Value = IntConfig.Value;
        }
    }
}
