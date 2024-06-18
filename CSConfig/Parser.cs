
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace CSConfig
{
#if false
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
                    if (property.PropertyType == typeof(Config))
                    {
                        var guiv = (Config)oguiv;
                        var cfgv = (Config)(ocfgv);
                        if (cfgv != null)
                        {
                            if (guiv.ConfigType == ConfigType.Tristate)
                            {
                                guiv.Value = (Tristate)(Convert.ToInt32(cfgv.Value));
                            }
                            else
                            {
                                guiv.Value = cfgv.Value;
                            }
                        }
                        
                        descriptionMenu.ItemValueChanged(guiv);

                    }
                    else if (property.PropertyType == typeof(Choice))
                    {
                        var guiv = (Choice)oguiv;
                        var cfgv = (Choice)(ocfgv);
                        if (cfgv != null)
                        {
                            guiv.SelectedConfig = cfgv.SelectedConfig;
                        }   
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
                        userConfig = (IMenu)JsonConvert.DeserializeObject(userConfigText, type);
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

                    if (property.PropertyType == typeof(Config))
                    {
                        action(value);
                    }
                    else if (property.PropertyType == typeof(Choice))
                    {
                        action(value);
                    }
                    else if (property.PropertyType.GetInterfaces().Contains(typeof(IMenu)))
                    {
                        IMenu subMenu = (IMenu)value;
                        action(value);
                        Debug.WriteLine($"Menu:{subMenu.Name}");
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
#endif
}
