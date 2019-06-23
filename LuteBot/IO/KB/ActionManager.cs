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
using System.Windows.Input;




namespace LuteBot.IO.KB
{

    public static class ActionManager
    {
        public enum AutoConsoleMode
        {
            Old,
            New,
            Off
        }

        public const UInt32 WM_KEYDOWN = 0x0100;
        public const UInt32 WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        private static string consoleCommand = "EQUIPMENTCOMMAND";

        private static bool consoleOn = false;

        public static string AutoConsoleModeToString(AutoConsoleMode mode)
        {
            switch (mode)
            {
                case AutoConsoleMode.Old:
                    return "Old";
                case AutoConsoleMode.New:
                    return "New";
                case AutoConsoleMode.Off:
                    return "Off";
                default:
                    return "Off";
            }
        }

        public static AutoConsoleMode AutoConsoleModeFromString(string str)
        {
            switch (str)
            {
                case "Old":
                    return AutoConsoleMode.Old;
                case "New":
                    return AutoConsoleMode.New;
                case "Off":
                    return AutoConsoleMode.Off;
                default:
                    return AutoConsoleMode.Off;
            }
        }

        public static void ToggleConsole(bool consoleOpen)
        {
            if (consoleOpen != consoleOn)
            {
                Process[] processes = Process.GetProcessesByName("Mordhau-Win64-Shipping");
                foreach (Process proc in processes)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)ConfigManager.GetKeybindProperty(PropertyItem.OpenConsole), 0);
                    if (!consoleOn)
                    {
                        PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)ConfigManager.GetKeybindProperty(PropertyItem.OpenConsole), 0);
                        consoleOn = true;
                    }
                    else
                    {
                        consoleOn = false;
                    }
                }
            }
        }

        #region imports
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
                                        // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
                                        //    0             The cursor is hidden.
                                        //    CURSOR_SHOWING    The cursor is showing.
            public IntPtr hCursor;          // Handle to the cursor. 
            public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        /// <summary>Must initialize cbSize</summary>
        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(ref CURSORINFO pci);

        private const Int32 CURSOR_SHOWING = 0x00000001;


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        #endregion




        private static void InputCommand(int noteId)
        {
           
            Process[] processes = Process.GetProcessesByName("Mordhau-Win64-Shipping"); // i refresh it on each call because it may get closed or rebooted etc
            if (processes.Length != 1) // if there is no game detected then fuck off
                return;
            IntPtr winhandle = processes[0].MainWindowHandle;
            if (winhandle != GetForegroundWindow()) // if the game has no focus, then fuck off
                return;

            CURSORINFO pci = new CURSORINFO();
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            GetCursorInfo(ref pci); // it stores the cursordata to the struct



            /*
             * If the cursor is showing, it means the user is typing or doesn't have the required state to play lute.
             */
            if (pci.flags == CURSOR_SHOWING)
                return;

            if (((Control.ModifierKeys & Keys.Control) == Keys.Control) ||
                ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) ||
                (Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                return;



            if (AutoConsoleModeFromString(ConfigManager.GetProperty(PropertyItem.ConsoleOpenMode)) == AutoConsoleMode.New)
            {
                foreach (Process proc in processes)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)ConfigManager.GetKeybindProperty(PropertyItem.OpenConsole), 0);
                }
            }
            foreach (char key in consoleCommand)
            {
                Enum.TryParse<Keys>("" + key, out Keys tempKey);
                foreach (Process proc in processes)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)tempKey, 0);
                }
            }
            foreach (Process proc in processes)
            {
                PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)Keys.Space, 0);
            }
            foreach (char key in noteId.ToString())
            {
                Enum.TryParse<Keys>("NumPad" + key, out Keys tempKey);
                foreach (Process proc in processes)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, (int)tempKey, 0);
                }
            }
            foreach (Process proc in processes)
            {
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
