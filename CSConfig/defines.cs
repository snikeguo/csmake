using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
namespace CSConfig
{
    public interface IItem
    {
        string Help  => null;
        string DisplayName => null;
        bool IsVisible => true;
        bool IsReadOnly => false;
    }

    public class Config: IItem
    {
        [JsonIgnore]
        public Type ConfigType { get; private set; }
        [JsonIgnore]
        public string MacroName { get; set; }
        public object Value { get; set; }
        [JsonIgnore]
        public bool IsHexShow { get; set; }
        [JsonIgnore]
        public string Help { get; set; }
        [JsonIgnore]
        public string DisplayName { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }

    }

    public class Choice: IItem
    {
        [JsonIgnore]
        public List<Config> Configs { get; set; }
        public Config SelectedConfig { get; set; }

        [JsonIgnore]
        public string Help { get; set; }
        [JsonIgnore]
        public string DisplayName { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }
    }

    
    public interface IMenu : IItem
    {
        public string ToString()
        {
            return DisplayName;
        }
        public void ItemValueChanged(IItem item);
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class ItemConfigAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class MainMenuAttribute : Attribute
    {

    }
    
}
