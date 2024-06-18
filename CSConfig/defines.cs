using Newtonsoft.Json;
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

    
    public interface IMenu 
    {
        public void ItemValueChanged(object item);
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class ConfigNameAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class MainMenuAttribute : Attribute
    {

    }
    
}
