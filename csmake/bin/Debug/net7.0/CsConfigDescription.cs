using CSConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using csmake;
//子菜单
public class Menu1 : IMenu
{
    public Menu1()
    {
        Menu1Config = new Config() { ConfigType = ConfigType.Int,IsHexShow=true,Value = 0x0, Depends = null, EnableControl = null, Help = "ram start address!", Name = "Ram Menu1Config" };
        Menu1StringConfig = new Config() { ConfigType = ConfigType.String, Value = "rh850", Depends = null, EnableControl = null, Help = "chip", Name = "chipname" };
        var tc397 = new Config() { ConfigType = ConfigType.Bool, Value = false, Depends = null, EnableControl = null, Help = "英飞凌的板子 中文测试", Name = "Tc397" };
        var s32k = new Config() { ConfigType = ConfigType.Bool, Value = false, Depends = null, EnableControl = null, Name = "s32k" };
        Menu1Choice = new Choice();
        Menu1Choice.Configs = new List<Config>();
        Menu1Choice.Configs.Add(tc397);
        Menu1Choice.Configs.Add(s32k);
        Menu1Choice.SelectedConfig = s32k;
        Menu1Choice.Help = "board select!";
        Menu1Choice.EnableControl = null;
        Menu1Choice.Name = "选择开发板";
        Host.WriteLine("FUCK222");
    }
    public string Name => "menu1";
    public string Help => "menu1 help";
    [ItemConfig]
    public Config Menu1Config { get; set; }
    
    [ItemConfig]
    public Config Menu1StringConfig{get;set;}
    public void ItemValueChanged(IItem cfg)
    {
        if(cfg==Menu1StringConfig)
        {
            Host.WriteLine("fuck!");
            string v = (string)(((Config)cfg).Value);
            if (v == "stm32")
            {
                var stm32 = new Config() { ConfigType = ConfigType.Bool, Value = false, Depends = null, EnableControl = null, Help = "stm32 help", Name = "stm32" };
                Menu1Choice.Configs.Add(stm32);
            }
            Menu1StringConfig.Value = v;
            MyMainMenu.gInstnace.MainMenuConfig.Value=v;
        }
        else if(cfg==Menu1Choice)
        {
            Host.WriteLine($"Menu1Choice 's Selected Config = {Menu1Choice.SelectedConfig}");
           
        }
        
    }

    [ItemConfig]
    public Choice Menu1Choice { get; set; }
}

//主菜单
[MainMenu]
public class MyMainMenu : IMenu
{
    public static MyMainMenu gInstnace{get;set;}=new MyMainMenu();
    public string Name =>"MyMainMenu";
    public MyMainMenu()
    {
        MainMenuConfig = new Config() { ConfigType = ConfigType.String, Name = "XXX1", Value = "linux kernel", Help = "xxx1 help" };
    }
    public void ItemValueChanged(IItem cfg)
    {
        if(cfg==MainMenuConfig)
        {
            menu1.Menu1StringConfig.Value=MainMenuConfig.Value;
        }
    }
    [ItemConfig]
    public Config MainMenuConfig { get; set; }
    [ItemConfig]
    public Menu1 menu1 { get; set; }
}
