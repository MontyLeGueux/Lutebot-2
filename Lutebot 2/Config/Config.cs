using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lutebot.Config
{
    /// <summary>
    /// The class used by the ConfigManager to store the configuration.
    /// </summary>
    public class Config
    {
        
        public Config()
        {
            hotkeys = new List<Hotkey>();
            properties = new List<Property>();
        }
        
        /// <summary>
        /// Creates a new config with default values.
        /// </summary>
        public void Init()
        {
            hotkeys = Hotkey.GetDefaultValues();
            properties = Property.GetDefaultValues();
        }

        private List<Hotkey> hotkeys;
        private List<Property> properties;
        public List<Hotkey> Hotkeys { get => hotkeys; set => hotkeys = value; }
        public List<Property> Properties { get => properties; set => properties = value; }

        public Keys Get(HotkeyItem item)
        {
            return hotkeys.Find(x => x.Item == item).Key;
        }

        public string Get(PropertyItem item)
        {
            return properties.Find(x => x.Item == item).Value;
        }

        public void Set(HotkeyItem item, Keys value)
        {
            hotkeys.Find(x => x.Item == item).Key = value;
        }

        public void Set(PropertyItem item, string value)
        {
            properties.Find(x => x.Item == item).Value = value;
        }

        public Hotkey IdentifyKey(Keys key)
        {
            return hotkeys.Find(x => x.Key== key);
        }

        public List<Hotkey> GetAllHotkeys()
        {
            return hotkeys;
        }

        public List<Property> GetAllProperties()
        {
            return properties;
        }
    }
}
