using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot.UI
{
    public partial class OverlayForm : Form
    {
        /*
         * Consulted Hex Murder's Tutorial at https://www.youtube.com/watch?v=t1ErGj0YnaM
         */

        // variables needed for transparent overlay click-thru 
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        static System.Windows.Forms.Timer refreshTimer = new System.Windows.Forms.Timer();

        RECT gameWindowRect;
        public struct RECT
        {
            public int left, top, right, bottom;
        }

        private bool On;
        private bool wasOn;
        public string State;
        public string Song;
        Graphics g;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public OverlayForm()
        {
            InitializeComponent();
            InitializeTimer();
            On = false;
            wasOn = false;
            State = "pending";
            Song = "";
            Paint += UpdateText;
        }

        public void Toggle()
        {
            IntPtr gameWindow = getGameWindow();
            if (gameWindow == IntPtr.Zero || gameWindow != GetForegroundWindow())
            {
                if (On)
                {
                    wasOn = true;
                }
                On = false;
            }
            else
            {
                if (!On)
                {
                    wasOn = false;
                }
                On = !On;
            }
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            // allow click through
            SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        private IntPtr getGameWindow()
        {
            Process[] processes = Process.GetProcessesByName("Mordhau-Win64-Shipping");
            if (processes.Length == 0)
                return IntPtr.Zero;
            return processes[0].MainWindowHandle;
        }

        private void InitializeTimer()
        {
            refreshTimer.Interval = 17; // this locks overlay to game window
            refreshTimer.Enabled = true;
            refreshTimer.Tick += new System.EventHandler(LocationUpdate);
        }

        private void LocationUpdate(object sender, EventArgs e)
        {
            IntPtr gameWindow = getGameWindow();
            if (gameWindow == IntPtr.Zero || gameWindow != GetForegroundWindow() || !On && !wasOn)
            {
                Hide();
                return;
            }
            if (!On && wasOn)
            {
                On = true;
                wasOn = false;
            }
            // if game is in focus and was set to On
            Height = 20;
            Width = 250;
            GetWindowRect(gameWindow, out gameWindowRect);
            // if full screen
            if (gameWindowRect.top == 0 && gameWindowRect.left == 0)
            {
                Top = gameWindowRect.bottom - Height;
                Left = gameWindowRect.right - Width;
            }
            // if windowed mode (has border)
            else
            {
                /*
                * the values returned by getWindowRect() are off by a little bit compared to the actual value in window mode,
                * So I'm correcting it (it's roughly 7px on Windows 10, untested on Windows 7)
                */
                int offset = 7;
                Top = gameWindowRect.bottom - Height - offset;
                Left = gameWindowRect.right - Width - offset;
            }
            Show();
        }

        private void UpdateText(object sender, PaintEventArgs e)
        {
            // draw Text
            g = e.Graphics;
            Font font = new Font("Lucida Console", 12);
            SolidBrush white = new SolidBrush(Color.White);
            SolidBrush green = new SolidBrush(Color.Green);
            SolidBrush red = new SolidBrush(Color.Red);
            // correct Song string
            if (Song.Length >= 23) {
                Song = Song.Substring(0, 20) + "...";
            }
            int paddingTop = 4;
            switch (State)
            {
                case "pending":
                    g.DrawString("Waiting for action...", font, white, 0, paddingTop);
                    break;
                case "play":
                    g.DrawString("\u25B6", new Font("Lucida Console", 15), white, 5, 2);
                    g.DrawString(Song, font, white, 20, paddingTop);
                    break;
                case "pause":
                    g.DrawString("\u23F8", font, white, 0, 2);
                    g.DrawString(Song, font, white, 20, paddingTop);
                    break;
                case "stop":
                    g.DrawString("\u25A0", font, white, 5, 2);
                    g.DrawString(Song, font, white, 20, paddingTop);
                    break;
                case "live-input-mode":
                    g.DrawString("Live Input Mode", font, white, 0, paddingTop);
                    break;
                case "live-input-on":
                    g.DrawString("Live Input On", font, green, 0, paddingTop);
                    break;
                case "live-input-off":
                    g.DrawString("Live Input Off", font, red, 0, paddingTop);
                    break;
            }
        }
    }
}
