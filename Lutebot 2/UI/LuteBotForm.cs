using Lutebot.Config;
using LuteBot;
using LuteBot.IO;
using LuteBot.Logger;
using LuteBot.Playlist;
using LuteBot.SoundBoard;
using LuteBot.UI;
using Lutebot.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Lutebot.Core.Midi;

namespace Lutebot.UI
{
    /// <summary>
    /// The main LuteBot window 
    /// </summary>
    public partial class LuteBotForm : Form
    {
        #region Windows Hook related stuff (keyboard listening when window is out of focus
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        static KeyboardInput hotkeyManager;
        static ConfigManager configManager;
        static KeyboardOutput actionManager;
        LoggerManager loggerManager;
        Player player;

        SoundBoardForm soundBoardForm;
        PlayListForm playListForm;
        LoggerForm loggerForm;

        string playButtonStartString = "Play";
        string playButtonStopString = "Pause";
        string musicNameLabelHeader = "Playing : ";
        bool playButtonIsPlaying = false;
        bool autoplay = false;
        Timer timer;

        static PlayList playList;
        static SoundBoardManager soundBoardManager;

        bool closing = false;

        public LuteBotForm()
        {
            InitializeComponent();
            loggerManager = new LoggerManager();
            configManager = new ConfigManager();
            loggerManager.IsOn = configManager.GetBooleanProperty(PropertyItem.DebugMode);
            player = new MidiPlayer();
            player.SongLoadCompleted += new EventHandler<EventArgs>(HandleSongLoadCompleted);

            playList = new PlayList();
            playList.PlayListUpdatedEvent += new EventHandler<PlayListEventArgs>(HandlePlayListChanged);
            soundBoardManager = new SoundBoardManager();
            soundBoardManager.SoundBoardTrackRequest += new EventHandler<SoundBoardEventArgs>(HandleSoundBoardTrackRequest);
            actionManager = new KeyboardOutput(configManager);
            hotkeyManager = new KeyboardInput(configManager);
            hotkeyManager.NextKeyPressed += new EventHandler(NextButton_Click);
            hotkeyManager.PlayKeyPressed += new EventHandler(PlayButton_Click);
            hotkeyManager.PreviousKeyPressed += new EventHandler(PreviousButton_Click);
            InitTimer();

            PlayButton.Enabled = false;
            StopButton.Enabled = false;
            PreviousButton.Enabled = false;
            NextButton.Enabled = false;
            MusicProgressBar.Enabled = false;

            _hookID = SetHook(_proc);
            OpenDialogs();
            this.StartPosition = FormStartPosition.Manual;
            Point coords = configManager.GetWindowCoordinates(PropertyItem.MainWindowPos);
            Top = coords.X;
            Left = coords.Y;
            GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Essential, "LuteBotForm Initialised");
        }

        private void InitTimer()
        {
            timer = new Timer
            {
                Interval = 1000
            };
            timer.Tick += new EventHandler(HandleTimerTick);
        }

        private void HandleTimerTick(object sender, EventArgs e)
        {
            StartLabel.Text = FormatTimeFromSeconds(player.CurrentTimeInSeconds);
            MusicProgressBar.Value = player.CurrentTimeInSeconds;
        }

        private void HandleSoundBoardTrackRequest(object sender, SoundBoardEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// On focus of the main lutebot windows, all other opened windows get focus as well.
        /// (Need to add a list of forms instead)
        /// </summary>
        private void LuteBotForm_Focus(object sender, EventArgs e)
        {
            if (loggerForm != null && !loggerForm.IsDisposed)
            {
                if (loggerForm.WindowState == FormWindowState.Minimized)
                {
                    loggerForm.WindowState = FormWindowState.Normal;
                }
                loggerForm.Focus();
            }
            if (soundBoardForm != null && !soundBoardForm.IsDisposed)
            {
                if (soundBoardForm.WindowState == FormWindowState.Minimized)
                {
                    soundBoardForm.WindowState = FormWindowState.Normal;
                }
                soundBoardForm.Focus();
            }
            if (playListForm != null && !playListForm.IsDisposed)
            {
                if (playListForm.WindowState == FormWindowState.Minimized)
                {
                    playListForm.WindowState = FormWindowState.Normal;
                }
                playListForm.Focus();
            }
            this.Focus();
        }

        private void HandlePlayListChanged(object sender, PlayListEventArgs e)
        {
            if (e.EventType == PlayListEventArgs.UpdatedComponent.UpdateNavButtons)
            {
                ToggleNavButtons(playList.HasNext());
            }
            if (e.EventType == PlayListEventArgs.UpdatedComponent.PlayRequest)
            {
                player.LoadSong(playList.Get(e.Id).Path);
                autoplay = true;
            }
        }

        private void ToggleNavButtons(bool enable)
        {
            PreviousButton.Enabled = enable;
            NextButton.Enabled = enable;
        }

        private string FormatTimeFromSeconds(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"mm\:ss");
        }

        private void HandleSongLoadCompleted(object sender, EventArgs e)
        {
            StopButton_Click(null, null);
            PlayButton.Enabled = true;
            MusicProgressBar.Enabled = true;
            StopButton.Enabled = true;

            MusicProgressBar.Value = 0;
            MusicProgressBar.Maximum = player.CurrentMusic.LengthInSeconds;
            StartLabel.Text = FormatTimeFromSeconds(0);
            EndTimeLabel.Text = FormatTimeFromSeconds(player.CurrentMusic.LengthInSeconds);
            CurrentMusicLabel.Text = musicNameLabelHeader + player.CurrentMusic.Name;
        }

        private void MusicProgressBar_Scroll(object sender, EventArgs e)
        {
            player.ChangeCurrentTime(MusicProgressBar.Value);
        }

        private void OpenDialogs()
        {
            if (configManager.GetBooleanProperty(PropertyItem.DebugMode))
            {
                loggerForm = new LoggerForm(loggerManager);
                loggerForm.Show();
            }
            if (configManager.GetBooleanProperty(PropertyItem.SoundBoard))
            {
                soundBoardForm = new SoundBoardForm(soundBoardManager, configManager);
                Point coords = configManager.GetWindowCoordinates(PropertyItem.SoundBoardPos);
                soundBoardForm.Show();
                soundBoardForm.Top = coords.X;
                soundBoardForm.Left = coords.Y;
            }
            if (configManager.GetBooleanProperty(PropertyItem.PlayList))
            {
                playListForm = new PlayListForm(playList, configManager);
                Point coords = configManager.GetWindowCoordinates(PropertyItem.PlayListPos);
                playListForm.Show();
                playListForm.Top = coords.X;
                playListForm.Left = coords.Y;

            }
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openMusicFileDialog = new OpenFileDialog();
            openMusicFileDialog.DefaultExt = "abc";
            openMusicFileDialog.Filter = "Music files|*.abc|All files|*.*";
            openMusicFileDialog.Title = "Open Music file";
            if (openMusicFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMusicFileDialog.FileName;
                player.LoadSong(fileName);
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (player.IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        private void Play()
        {
            if (KeyboardOutput.AutoConsoleModeFromString(configManager.GetProperty(PropertyItem.AutoConsoleMode)) == KeyboardOutput.AutoConsoleMode.Old)
            {
                actionManager.ToggleConsole(true);
            }
            PlayButton.Text = playButtonStopString;
            player.Play();
            timer.Enabled = true;
            GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Basic, "Playing now");
        }

        private void Pause()
        {
            if (KeyboardOutput.AutoConsoleModeFromString(configManager.GetProperty(PropertyItem.AutoConsoleMode)) == KeyboardOutput.AutoConsoleMode.Old)
            {
                actionManager.ToggleConsole(false);
            }
            PlayButton.Text = playButtonStartString;
            player.Pause();
            timer.Enabled = false;
            GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Basic, "Pausing now");
        }

        private void Stop()
        {
            timer.Enabled = false;
            MusicProgressBar.Value = 0;
            PlayButton.Enabled = false;
            MusicProgressBar.Enabled = false;
            StopButton.Enabled = false;
            StartLabel.Text = "00:00";
            EndTimeLabel.Text = "00:00";
            CurrentMusicLabel.Text = "";
            PlayButton.Text = playButtonStartString;
            player.Stop();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            Pause();
            playList.Next();
            autoplay = true;
            player.LoadSong(playList.Get(playList.CurrentTrackIndex).Path);
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            Pause();
            playList.Previous();
            autoplay = true;
            player.LoadSong(playList.Get(playList.CurrentTrackIndex).Path);
        }

        private void SoundBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            soundBoardForm = new SoundBoardForm(soundBoardManager, configManager);
            Point coords = configManager.GetWindowCoordinates(PropertyItem.SoundBoardPos);
            soundBoardForm.Show();
            soundBoardForm.Top = coords.X;
            soundBoardForm.Left = coords.Y;

        }

        private void PlayListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playListForm = new PlayListForm(playList, configManager);
            Point coords = configManager.GetWindowCoordinates(PropertyItem.PlayListPos);
            playListForm.Show();
            playListForm.Top = coords.X;
            playListForm.Left = coords.Y;

        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new SettingsForm()).ShowDialog();
            loggerManager.IsOn = configManager.GetBooleanProperty(PropertyItem.DebugMode);
            if (configManager.GetBooleanProperty(PropertyItem.DebugMode) && (loggerForm == null || loggerForm.IsDisposed))
            {
                loggerForm = new LoggerForm(loggerManager);
                loggerForm.Show();
            }
            configManager.Refresh();
        }

        private void KeyBindingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new KeyBindingForm()).ShowDialog();
            configManager.Refresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            configManager.SetWindowCoordinates(PropertyItem.MainWindowPos, new Point() { X = this.Top, Y = this.Left });
            if (soundBoardForm != null)
            {
                soundBoardForm.Close();
            }
            if (playListForm != null)
            {
                playListForm.Close();
            }
            configManager.SaveConfig();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            timer.Dispose();
            base.OnClosed(e);
            UnhookWindowsHookEx(_hookID);
        }

        protected override void WndProc(ref Message m)
        {
            hotkeyManager.HotkeyPressed(m.Msg);
            base.WndProc(ref m);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                hotkeyManager.HotkeyPressed(vkCode);
                if (Enum.TryParse(vkCode.ToString(), out Keys tempkey))
                {
                    soundBoardManager.KeyPressed(tempkey);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
