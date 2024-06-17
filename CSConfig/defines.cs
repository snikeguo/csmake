using Newtonsoft.Json;

namespace CSConfig
{
    public enum ConfigType
    {
        Bool,
        Tristate,//y/n/m
        String,
        Int,
        UInt,
        Double,
    }
    public enum Tristate
    {
        Y,
        N,
        M,
    }


    public interface IItem
    {

        string Help  => null;

        string Name => null;

        public List<Config> Depends => null;//kconfig 遗留项
        public List<Config> Select => null;
        public Config EnableControl => null;


    }

    public class Config: IItem
    {
        [JsonIgnore]
        public ConfigType ConfigType { get; set; }
        public object Value { get;set; }
        public bool IsHexShow { get; set; }
        [JsonIgnore]
        public string Help { get; set; }
        //[JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public List<Config> Depends { get; set; }//kconfig 遗留项
        [JsonIgnore]
        public List<Config> Select { get; set; }
        [JsonIgnore]
        public Config EnableControl { get; set; }
        public override string ToString()
        {
            return Name;
        }

    }

    public class Choice: IItem
    {
        [JsonIgnore]
        public List<Config> Configs { get; set; }

        public Config SelectedConfig { get; set; }

        [JsonIgnore]
        public string Help { get; set; }
        //[JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public List<Config> Depends { get; set; }//kconfig 遗留项
        [JsonIgnore]
        public List<Config> Select { get; set; }
        [JsonIgnore]
        public Config EnableControl { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public interface IMenu : IItem
    {
        public string ToString()
        {
            return Name;
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
