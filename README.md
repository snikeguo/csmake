# 使用C#做脚本代替KConfig、makefile,持续开发中,原型验证是可以的。
![](./doc/index.jpg)


编译:
* 1.VS2022安装Avalonia for VS2022 扩展。
* 2.编译csmake仓库
* 3.F5调试。他会使用$root\CSConfigSln\csmake\bin\Debug\net7.0\CsConfigDescription.cs作为脚本(类似于kconfig脚本)输入，渲染一个tree
* 4.非调试运行：
    
    .\csmake.exe menuconfig -d CsConfigDescription.cs -u 33.json
    .\csmake.exe cs2head -d CsConfigDescription.cs -u 33.json -o myheader.h

编写一个demo:主菜单名为demo,demo中有三个项a,b,c,其中c=a+b,c使用16进制表示
```
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
public struct TestStruct
{
    int a;
    byte b;

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
        Host.WriteLine(cfg.ToString());
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
```
保存为test.cs
输入命令.\csmake.exe menuconfig -d test.cs   这里没有-u xxx.json 是因为我们没有默认的用户配置文件。



这个等价于kconfig,可以将kconfig转成cs脚本。

接下来做的事：
csmake 代替make :初步设计: 
1.csmake build -i XXX1.cs其中XXX1.cs类似于makefile，
2.如果不写-i ，也就是csmake build 他将默认找csmake.cs作为输入。
3.csmake.cs描述了又多少个.c /cpp 以及每个头文件路径，还有排除项，还有用什么编译器编译，比如GNU？
    比如预定义宏(-D)通过这个csmake.cs就能获取这些。
4.获取到之后,如果命令行是csmake  build 没有其他的，name将csmake.cs的工具链作为编译工具链.
    如果命令行是csmake build --toolchain "MSVC",那么将忽略csmake.cs的关于工具链的描述内容。

总体思想是
# 1.csmake.cs描述了要编译的所有资源，如源码文件，头文件 工具链，链接脚本，输出目录，输出文件名，BuildAfter事件等等。
# 2.命令行这边，主要是覆盖csmake.cs的内容，比如增加预定义宏，替换工具链。
还有什么 大家想想？
主要要设计好这个模型！



