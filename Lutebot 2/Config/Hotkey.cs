using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lutebot.Config
{
    /// <summary>
    /// A Hotkey with a name and its associated key in the winforms Keys enum
    /// </summary>
    public class Hotkey
    {
        private HotkeyItem item;
        private Keys key;

        /// <summary>
        /// The name of the Hotkey
        /// </summary>
        public HotkeyItem Item { get => item; set => item = value; }

        /// <summary>
        /// The item in the winforms Keys enum
        /// </summary>
        public Keys Key { get => key; set => key = value; }

        public static List<Hotkey> GetDefaultValues()
        {
            int i = 0;
            Keys currentKey;
            HotkeyItem[] hotkeyItems = (HotkeyItem[])Enum.GetValues(typeof(HotkeyItem));
            List<Hotkey> defaultHotkey = new List<Hotkey>();
            Keys[] hotkeyDefaultValues =
            {
               Keys.None,
               Keys.Add,
               Keys.Subtract,
               Keys.Multiply,
               Keys.Divide,
               Keys.Next

            };
            foreach (HotkeyItem hotkeyItem in hotkeyItems)
            {
                if (i < hotkeyDefaultValues.Length)
                {
                    currentKey = hotkeyDefaultValues[i];
                }
                else
                {
                    currentKey = Keys.None;
                }
                defaultHotkey.Add(new Hotkey() { Item = hotkeyItem, Key = currentKey });
                i++;
            }
            defaultHotkey.RemoveAt(0);
            return defaultHotkey;
        }

        public static HotkeyItem FromString(string name)
        {
            HotkeyItem result;
            if (Enum.TryParse(name, out result))
            {
                return result;
            }
            else
            {
                return HotkeyItem.None;
            }
        }

        public static Keys KeyFromString(string name)
        {
            Keys result;
            if (Enum.TryParse(name, out result))
            {
                return result;
            }
            else
            {
                return Keys.None;
            }
        }
    }
}
