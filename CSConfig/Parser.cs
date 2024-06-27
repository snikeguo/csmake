
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace CSConfig
{
    public static class Parser
    {
        public static void ConfigParsing(Assembly CsConfigAssembly, IMenu descriptionMenu,IMenu userConfigMenu, Assembly UserScriptDescriptionAssembly)
        {
            var properties = descriptionMenu.GetType().GetProperties();
            foreach (var property in properties)
            {
                var itemAttr = property.GetCustomAttribute<ItemConfigAttribute>();
                if (itemAttr != null)
                {
                    object oguiv = property.GetValue(descriptionMenu);
                    object ocfgv = null;
                    if (userConfigMenu != null)
                    {
                        ocfgv = property.GetValue(userConfigMenu);
                    }
                    if (property.PropertyType.IsGenericType)
                    {
                        var configType=property.PropertyType.GetGenericTypeDefinition();
                        if(configType==typeof(Config<>))
                        {
                            if (ocfgv != null)
                            {
                                property.SetValue(descriptionMenu, ocfgv);
                            }

                            descriptionMenu.ItemValueChanged((IItem)oguiv);
                        }
                    }
                    else if (property.PropertyType == typeof(Choice))
                    {
                        var guiv = (Choice)oguiv;
                        var cfgv = (Choice)(ocfgv);
                        if (cfgv != null)
                        {
                            guiv.SelectedItem = cfgv.SelectedItem;
                        }
#if true
                        //这里注释的原因是,如果是IMenu类型，JSON文件会实例化SelectedItem的类型，直接将JSON实例化出来的对象给到guiv.SelectedItem就可以了
                        //但是我觉得又可以打开，万一描述文件(cs文件)逻辑发生变化，举个例子:
                        //现在有三项a  b c ，旧的描述文件为c=a+b,那么配置出来的结果就c=a+b 比如c=3,a=1,b=2
                        //然后后来发现用户这个逻辑写错了,不应该是c=a+b,而是c=b-a;
                        //如果把这个注释后,就会出现c=3,a=1,b=2显然不符合新的逻辑
                        //按道理来说c需要更新,如果把这个宏打开，并且反射遍历property的时候,c在前面 a和b在后面,那么c的值就有机会变成1,因为执行了c=b-a
                        //当反射遍历property时,如果c在后面,那么c的值就无法变成1,仍然保持3,除非你在ItemValueChanged回调函数里去写b=c-a或者a=c-b
                        //也就是说双向校验了：a,b变则c变,c变则a变或者b变,我相信很多人不会写这个反向校验,因为预计牵扯的量太多。
                        //所以开启这个宏并不能解决数据之间的关系,但是开启宏后会调用ItemValueChanged
                        if (guiv.SelectedItem!=null)
                        {
                            
                            if(guiv.SelectedItem is IMenu menu)
                            {
                                ConfigParsing(CsConfigAssembly, menu, cfgv ==null? null: (IMenu)cfgv.SelectedItem, UserScriptDescriptionAssembly);
                            }
                        }
#endif
                        descriptionMenu.ItemValueChanged(guiv);
                    }
                    else if (property.PropertyType.GetInterfaces().Contains(typeof(IMenu)))
                    {
                        var guiv = (IMenu)oguiv;
                        var cfgv = (IMenu)(ocfgv);
                        if(guiv==null)
                        {
                            guiv = (IMenu)UserScriptDescriptionAssembly.CreateInstance(property.PropertyType.FullName);
                            property.SetValue(descriptionMenu, guiv);
                        }
                        //Debug.WriteLine($"Menu:{subMenu.Name}");
                        ConfigParsing(CsConfigAssembly, guiv, cfgv, UserScriptDescriptionAssembly);
                    }
                }
            }
        }
        public static (IMenu, IMenu) Parse(Assembly UserScriptDescriptionAssembly, string UserConfigFilePath)
        {
            var CsConfigAssembly=Assembly.GetExecutingAssembly();
            var allTypes = UserScriptDescriptionAssembly.GetTypes();
            IMenu mainMenu = null;
            Type mainMenuType = null;
            IMenu userConfig = null;

            foreach (var type in allTypes)
            {
                var mainMenuAttr = type.GetCustomAttribute<MainMenuAttribute>();
                if (mainMenuAttr != null)
                {
                    mainMenuType = type;
                    try
                    {
                        var userConfigText = File.ReadAllText(UserConfigFilePath);
                        var settings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects // 必须与序列化时一致  
                        };
                        userConfig = (IMenu)JsonConvert.DeserializeObject(userConfigText, type, settings);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("cs config type != user config type!");
                    }

                    var mainMenuProperties = type.GetProperties();
                    foreach (var p in mainMenuProperties)
                    {
                        if ( p.PropertyType == type )
                        {
                            mainMenu = (IMenu)(p.GetValue(null));
                            if (mainMenu == null)
                            {
                                Console.WriteLine("please define static menu object!!");
                                return (null, userConfig);
                            }
                            ConfigParsing(CsConfigAssembly, mainMenu, userConfig,UserScriptDescriptionAssembly);
                            return (mainMenu, userConfig);
                        }
                    }
                    break;
                }
            }

            return (null, null);
        }
    
        public static void RecursiveMenu(IMenu descriptionMenu,Action<IItem> action)
        {
            var properties = descriptionMenu.GetType().GetProperties();
            foreach (var property in properties)
            {
                var itemAttr = property.GetCustomAttribute<ItemConfigAttribute>();
                if (itemAttr != null)
                {
                    var value = (IItem)property.GetValue(descriptionMenu);

                    if (property.PropertyType.IsGenericType)
                    {
                        var configType = property.PropertyType.GetGenericTypeDefinition();
                        if (configType == typeof(Config<>))
                        {
                            action(value);
                        }
                    }
                    else if (property.PropertyType == typeof(Choice))
                    {
                        if (value is IMenu menu)
                        {
                            RecursiveMenu(menu, action);
                        }
                        else
                        {
                            action(value);
                        }
                        
                    }
                    else if (property.PropertyType.GetInterfaces().Contains(typeof(IMenu)))
                    {
                        IMenu subMenu = (IMenu)value;
                        action(value);
                        Debug.WriteLine($"Menu:{subMenu.DisplayName}");
                        RecursiveMenu(subMenu, action);
                    }
                }
            }
        }
#if false
        public static void UpdateMenuValue(IMenu descriptionMenu, IMenu userCfgMainMenu)
        {
            var properties = descriptionMenu.GetType().GetProperties();
            foreach (var property in properties)
            {
                var itemAttr = property.GetCustomAttribute<ItemConfigAttribute>();
                if (itemAttr != null)
                {
                    if (property.PropertyType == typeof(Config))
                    {
                        Config cfgv = (Config)property.GetValue(userCfgMainMenu);
                        Config guiv = (Config)property.GetValue(descriptionMenu);
                        if (guiv.ConfigType == ConfigType.Tristate)
                        {
                            guiv.Value = (Tristate)(Convert.ToInt32(cfgv.Value));
                        }
                        else
                        {
                            guiv.Value = cfgv.Value;
                        }
                        descriptionMenu.ItemValueChanged(guiv);
                    }
                    else if (property.PropertyType == typeof(Choice))
                    {
                        Choice cfgv = (Choice)property.GetValue(userCfgMainMenu);
                        Choice guiv = (Choice)property.GetValue(descriptionMenu);
                        guiv.SelectedConfig = cfgv.SelectedConfig;
                        descriptionMenu.ItemValueChanged(guiv);
                    }
                    else if (property.PropertyType.GetInterfaces().Contains(typeof(IMenu)))
                    {
                        IMenu cfgv = (IMenu) property.GetValue(userCfgMainMenu);
                        IMenu guiv = (IMenu)property.GetValue(descriptionMenu);
                        UpdateMenuValue(guiv, cfgv);
                    }
                }
            }
        }
#endif
    }
}
