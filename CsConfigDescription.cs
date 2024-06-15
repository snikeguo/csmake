using CSConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using csmake;
//子菜单
public class Menu1 : Menu
{
    public Menu1()
    {
        Menu1Config = new Config() { ConfigType = ConfigType.Hex, DefalutValue = 0x0, Depends = null, EnableControl = null, Help = "ram start address!",Name= "Ram Menu1Config" };
        _Menu1StringConfig = new Config() { ConfigType = ConfigType.String, DefalutValue = "rh850", Depends = null, EnableControl = null, Help = "chip",Name= "chipname" };
        var tc397 = new Config() { ConfigType = ConfigType.Bool, DefalutValue = false, Depends = null, EnableControl = null,Help="英飞凌的板子 中文测试",Name="Tc397" };
        var s32k = new Config() { ConfigType = ConfigType.Bool, DefalutValue = false, Depends = null, EnableControl = null, Name="s32k"};
        Menu1Choice = new Choice();
        Menu1Choice.Configs=new List<Config>();
        Menu1Choice.Configs.Add(tc397);
        Menu1Choice.Configs.Add(s32k);
        Menu1Choice.DefalutConfig = s32k;
        Menu1Choice.Help = "board select!";
        Menu1Choice.EnableControl = null;
        Menu1Choice.Name = "选择开发板";
        Name = "menu1";
        EnableControl = null;
        Help = "menu1 help";
        Host.WriteLine("FUCK222");
    }
    [ItemConfig]
    public Config Menu1Config{get;set;}
    private Config _Menu1StringConfig;
    [ItemConfig]
    public Config Menu1StringConfig
    {
        get
        {
            return _Menu1StringConfig;
        }
        set
        {
            Host.WriteLine("fuck!");
            string v=(string)value.Value;
            if(v=="stm32")
            {
                var stm32 = new Config() { ConfigType = ConfigType.Bool, DefalutValue = false, Depends = null, EnableControl = null, Help = "stm32 help" ,Name="stm32"};
                Menu1Choice.Configs.Add(stm32);
            }
            _Menu1StringConfig.Value=value.Value;
        }
    }
    [ItemConfig]
    public Choice Menu1Choice { get; set; }
}

//主菜单
[MainMenu]
public class MyMainMenu : Menu
{
    public MyMainMenu()
    {
        MainMenuConfig = new Config() { ConfigType = ConfigType.String, Name = "XXX1", DefalutValue = "linux kernel", Help = "xxx1 help" };
        Name="MainMenu";
    }
    [ItemConfig]
    public Config MainMenuConfig { get; set; }
    [ItemConfig]
    public Menu1 menu1 { get; set; }
}