using Newtonsoft.Json;

namespace CSConfig
{
    public enum ConfigType
    {
        Bool,
        Tristate,//y/n/m
        String,
        Int,
        Hex,
    }
    public enum Tristate
    {
        Y,
        N,
        M,
    }

    public interface IItem
    {
        
        string Help { get; set; }
        string Name { get; set; }

        public List<Config> Depends { get; set; }//kconfig 遗留项
        public List<Config> Select { get; set; }
        public Config EnableControl { get; set; }
    }

    public class Config: IItem
    {
        [JsonIgnore]
        public ConfigType ConfigType { get; set; }
        public object Value { get;set; }
        [JsonIgnore]
        public object DefalutValue { get;set; }
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
        public Config DefalutConfig { get; set; }
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

    public class Menu : IItem
    {
        [JsonIgnore]
        public string Help { get; set; }
        //[JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public List<Config> Depends { get; set; }
        [JsonIgnore]
        public List<Config> Select { get; set; }
        [JsonIgnore]
        public Config EnableControl { get; set; }
        public override string ToString()
        {
            return Name;
        }
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
