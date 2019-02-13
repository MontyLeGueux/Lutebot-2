using LuteBot.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Lutebot.Config
{

    /// <summary>
    /// Save, load and store the configuration.
    /// </summary>
    public class ConfigManager
    {

        private Config config;
        private const string autoSavePath = "Config";

        public ConfigManager()
        {
            Refresh();
        }

        /// <summary>
        /// Re-Load the config from the file system
        /// </summary>
        public void Refresh()
        {
            config = LoadConfig();
            //if the config file cannot be found, load the default values.
            if (config == null)
            {
                config = new Config();
                config.Init();
            }
        }

        public static HotkeyItem[] GetAllConfigItems()
        {
            return (HotkeyItem[])Enum.GetValues(typeof(HotkeyItem));
        }

        public Keys GetHotkey(HotkeyItem item)
        {
            return config.Get(item);
        }

        public string GetProperty(PropertyItem item)
        {
            return config.Get(item);
        }

        public void SetProperty(PropertyItem item, string value)
        {
            config.Set(item, value);
        }

        public void SetHotkey(HotkeyItem item, Keys value)
        {
            config.Set(item, value);
        }

        public List<Hotkey> GetAll()
        {
            return config.GetAllHotkeys();
        }

        public void SaveConfig()
        {
            FileIO.SaveNoDialog<Config>(config, "Settings");
        }

        private Config LoadConfig()
        {
            return FileIO.LoadNoDialog<Config>("Settings");
        }

        public Hotkey IdentifyKey(Keys key)
        {
            return config.IdentifyKey(key);
        }

        public bool GetBooleanProperty(PropertyItem item)
        {
            string temp = config.Get(item);
            bool result;
            if (bool.TryParse(temp, out result))
            {
                return result;
            }
            else return false;
        }

        public Point GetWindowCoordinates(PropertyItem item)
        {
            string rawCoords = config.Get(item);
            string[] splitCoords = rawCoords.Split('|');
            if (splitCoords.Length > 1)
            {
                return new Point() { X = int.Parse(splitCoords[0]), Y = int.Parse(splitCoords[1]) };
            }
            else
            {
                return new Point() {X = 0, Y = 0 };
            }
        }

        public void SetWindowCoordinates(PropertyItem item, Point point)
        {
            config.Set(item, point.X+"|"+point.Y);
        }
    }
}
