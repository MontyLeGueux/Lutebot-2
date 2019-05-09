using LuteBot.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot.IO.KB
{
    public static class ActionManager
    {
        public enum AutoConsoleMode
        {
            Off = 0, // to make this the default value for TryParse
            Old,
            New,
        }

        public const UInt32 WM_KEYDOWN = 0x0100;
        public const UInt32 WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        private const string CONSOLE_COMMAND = "EQUIPMENTCOMMAND";
        private const string MORDHAU_PROCESS_NAME = "Mordhau-Win64-Shipping";

        private static bool consoleOn = false;

        public static void ToggleConsole(bool consoleOpen)
        {
            if (consoleOpen != consoleOn)
            {
                Process[] processes = Process.GetProcessesByName(MORDHAU_PROCESS_NAME);
                foreach (Process proc in processes)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)ConfigManager.GetKeybindProperty(PropertyItem.OpenConsole), 0);
                    if (!consoleOn)
                    {
                        PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)ConfigManager.GetKeybindProperty(PropertyItem.OpenConsole), 0);
                    }
                    consoleOn = !consoleOn;
                }
            }
        }

        private static void InputCommand(int noteId)
        {
            Process[] processes = Process.GetProcessesByName(MORDHAU_PROCESS_NAME);
            Enum.TryParse<AutoConsoleMode>(ConfigManager.GetProperty(PropertyItem.ConsoleOpenMode), out AutoConsoleMode consoleMode);

            foreach (Process proc in processes)
            {
                if (consoleMode == AutoConsoleMode.New)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)ConfigManager.GetKeybindProperty(PropertyItem.OpenConsole), 0);
                }
                foreach (char key in CONSOLE_COMMAND)
                {
                    Enum.TryParse<Keys>(key.ToString(), out Keys tempKey);
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)tempKey, 0);
                }
                PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.Space, 0);
                foreach (char key in noteId.ToString())
                {
                    Enum.TryParse<Keys>("NumPad" + key, out Keys tempKey);
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)tempKey, 0);
                }
                Thread.Sleep(ConfigManager.GetIntegerProperty(PropertyItem.NoteCooldown));
                PostMessage(proc.MainWindowHandle, WM_KEYUP, (int)Keys.Enter, 0);
            }
        }

        public static void PlayNote(int noteId)
        {
            InputCommand(noteId);
        }

        private static void InputAngle()
        {

        }
    }
}
