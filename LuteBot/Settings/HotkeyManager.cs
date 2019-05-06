using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot
{
    public class HotkeyManager
    {
        private ConfigManager configManager;

        public event EventHandler PlayKeyPressed;
        public event EventHandler NextKeyPressed;
        public event EventHandler PreviousKeyPressed;
        public event EventHandler ConsoleKeyPressed;
        public event EventHandler ReadyPressed;

        public HotkeyManager(ConfigManager configManager)
        {
            this.configManager = configManager;
        }

        public void HotkeyPressed(int keyCode)
        {
            Keys tempKey = (Keys)keyCode;
            ActionKey performedAction = configManager.GetHotkey(tempKey.ToString());
            if (performedAction != null) {
                if (performedAction.Name == "Play")
                {
                    EventHandler handler = PlayKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Name == "Next")
                {
                    EventHandler handler = NextKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Name == "Previous")
                {
                    EventHandler handler = PreviousKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Name == "OpenConsole")
                {
                    EventHandler handler = ConsoleKeyPressed;
                    handler?.Invoke(this, null);
                }
                if (performedAction.Name == "Ready")
                {
                    EventHandler handler = ReadyPressed;
                    handler?.Invoke(this, null);
                }
            }
        }
    }
}
