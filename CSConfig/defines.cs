using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
namespace CsConfig
{

    public enum Tristate
    {
        N,
        Y, 
        M,
    }

    public class SelectableList<T> where T: IEquatable<T>
    {
        public T SelectedItem { get; set; }
        [JsonIgnore]
        public List<T> Items { get; set; }
        public SelectableList(List<T> items,T selectedItem) 
        {
            Items=items;
            SelectedItem=selectedItem;
        } 
    }
    public interface IMenu 
    {
        public void ItemValueChanged(string propertyName);
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class ItemAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string MarcoName { get; set; }
        public ItemAttribute(string displayName, string description, string marcoName)
        {
            DisplayName = displayName;
            Description = description;
            MarcoName = marcoName;
        }
    }
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class HexAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class MainMenuAttribute : Attribute
    {

    }
    
}
