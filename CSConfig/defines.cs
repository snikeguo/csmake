using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
namespace CSConfig
{

    public interface IItem
    {
        [JsonIgnore]
        string Help  => null;
        [JsonIgnore]
        string DisplayName => null;
        [JsonIgnore]
        bool IsVisible => true;
        [JsonIgnore]
        bool IsReadOnly => false;
    }
    
    public class Config<T>: IItem
    {
        [JsonIgnore]
        public string MacroName { get; set; }
        public T Value { get; set; }
        [JsonIgnore]
        public bool IsHexShow { get; set; }
        [JsonIgnore]
        public string Help { get; set; }
        [JsonIgnore]
        public string DisplayName { get; set; }
        [JsonIgnore]
        public bool IsVisible { get; set; }
        [JsonIgnore]
        public bool IsReadOnly { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }

    }
    
    public class Choice: IItem
    {
        [JsonIgnore]
        public List<IItem> Items { get; set; }
        public IItem SelectedItem { get; set; }

        [JsonIgnore]
        public string Help { get; set; }
        [JsonIgnore]
        public string DisplayName { get; set; }
        [JsonIgnore]
        public bool IsVisible { get; set; }
        [JsonIgnore]
        public bool IsReadOnly { get; set; }
        [JsonIgnore]
        public bool CanAddByUser { get; set; } = false;
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
