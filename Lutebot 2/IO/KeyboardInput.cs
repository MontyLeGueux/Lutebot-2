using Lutebot.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot.IO
{
    /// <summary>
    /// Trigger events when specific keys are pressed by the user.
    /// </summary>
    public class KeyboardInput
    {
        private ConfigManager configManager;

        public event EventHandler PlayKeyPressed;
        public event EventHandler NextKeyPressed;
        public event EventHandler PreviousKeyPressed;
        public event EventHandler ConsoleKeyPressed;
        public event EventHandler ReadyPressed;

        public KeyboardInput(ConfigManager configManager)
        {
            this.configManager = configManager;
        }

        public void HotkeyPressed(int keyCode)
        {
            Keys tempKey = (Keys)keyCode;
            Hotkey performedAction = configManager.IdentifyKey(tempKey);
            if (performedAction != null) {
                if (performedAction.Item == HotkeyItem.Play)
                {
                    EventHandler handler = PlayKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Item == HotkeyItem.Next)
                {
                    EventHandler handler = NextKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Item == HotkeyItem.Previous)
                {
                    EventHandler handler = PreviousKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Item == HotkeyItem.OpenConsole)
                {
                    EventHandler handler = ConsoleKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Item == HotkeyItem.Ready)
                {
                    EventHandler handler = ReadyPressed;
                    handler?.Invoke(this, null);
                }
            }
        }
    }
}
