using LuteBot.Core;
using LuteBot.Core.Midi;
using LuteBot.Logger;
using LuteBot.OnlineSync;
using LuteBot.playlist;
using LuteBot.SoundBoard;
using LuteBot.TrackSelection;
using LuteBot.UI;
using Sanford.Multimedia.Midi;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LuteBot
{
    public partial class LuteBotForm : Form
    {
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

        static HotkeyManager hotkeyManager;
        static ConfigManager configManager;
        static ActionManager actionManager;
        LoggerManager loggerManager;

        TrackSelectionForm trackSelectionForm;
        OnlineSyncForm onlineSyncForm;
        SoundBoardForm soundBoardForm;
        PlayListForm playListForm;
        LoggerForm loggerForm;

        Player player;


        string playButtonStartString = "Play";
        string playButtonStopString = "Pause";
        string musicNameLabelHeader = "Playing : ";
        bool playButtonIsPlaying = false;
        string currentTrackName = "";
        bool autoplay = false;
        bool isDonePlaying = false;

        static PlayList playList;
        static SoundBoardManager soundBoardManager;
        static TrackSelectionManager trackSelectionManager;
        static OnlineSyncManager onlineManager;

        bool closing = false;

        public LuteBotForm()
        {
            InitializeComponent();
            loggerManager = new LoggerManager();
            configManager = new ConfigManager();
            loggerManager.IsOn = configManager.GetBooleanProperty("DebugMode");

            onlineManager = new OnlineSyncManager();
            playList = new PlayList();
            trackSelectionManager = new TrackSelectionManager();
            playList.PlayListUpdatedEvent += new EventHandler<PlayListEventArgs>(HandlePlayListChanged);
            soundBoardManager = new SoundBoardManager();
            soundBoardManager.SoundBoardTrackRequest += new EventHandler<SoundBoardEventArgs>(HandleSoundBoardTrackRequest);
            actionManager = new ActionManager(configManager);
            player = new MidiPlayer(actionManager, configManager, trackSelectionManager);
            player.SongLoaded += new EventHandler<AsyncCompletedEventArgs>(PlayerLoadCompleted);
            hotkeyManager = new HotkeyManager(configManager);
            hotkeyManager.NextKeyPressed += new EventHandler(NextButton_Click);
            hotkeyManager.PlayKeyPressed += new EventHandler(PlayButton_Click);
            hotkeyManager.PreviousKeyPressed += new EventHandler(PreviousButton_Click);
            trackSelectionManager.OutDeviceResetRequest += new EventHandler(ResetDevice);
            trackSelectionManager.ToggleTrackRequest += new EventHandler<TrackItem>(ToggleTrack);

            PlayButton.Enabled = false;
            StopButton.Enabled = false;
            PreviousButton.Enabled = false;
            NextButton.Enabled = false;
            MusicProgressBar.Enabled = false;

            _hookID = SetHook(_proc);
            OpenDialogs();
            this.StartPosition = FormStartPosition.Manual;
            Point coords = configManager.GetWindowCoordinates("MainWindowPos");
            Top = coords.X;
            Left = coords.Y;
            GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Essential, "LuteBotForm Initialised");
        }

        private void PlayerLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                StopButton_Click(null, null);
                PlayButton.Enabled = true;
                MusicProgressBar.Enabled = true;
                StopButton.Enabled = true;

                trackSelectionManager.UnloadTracks();
                if (player.GetType() == typeof(MidiPlayer))
                {
                    MidiPlayer midiPlayer = player as MidiPlayer;
                    trackSelectionManager.LoadTracks(midiPlayer.GetMidiChannels(), midiPlayer.GetMidiTracks());
                    trackSelectionManager.FileName = currentTrackName;
                }

                if (trackSelectionManager.autoLoadProfile)
                {
                    trackSelectionManager.LoadTrackManager();
                    GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Basic, "Loaded Channel profile");
                }

                MusicProgressBar.Value = 0;
                MusicProgressBar.Maximum = player.GetLenght();
                StartLabel.Text = TimeSpan.FromSeconds(0).ToString(@"mm\:ss");
                EndTimeLabel.Text = player.GetFormattedLength();
                CurrentMusicLabel.Text = musicNameLabelHeader + currentTrackName;
                if (autoplay)
                {
                    Play();
                    autoplay = false;
                }
                GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Essential, "Song : " + currentTrackName + "loaded successfully");
            }
            else
            {
                MessageBox.Show(e.Error.Message + " in " + e.Error.Source + e.Error.TargetSite + "\n" + e.Error.InnerException + "\n" + e.Error.StackTrace);
            }
        }

        private void ToggleTrack(object sender, TrackItem e)
        {
            timer1.Stop();
            (player as MidiPlayer).UpdateMutedTracks(e);
            timer1.Start();
        }

        private void ResetDevice(object sender, EventArgs e)
        {
            (player as MidiPlayer).ResetDevice();
        }

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
            if (trackSelectionForm != null && !trackSelectionForm.IsDisposed)
            {
                if (trackSelectionForm.WindowState == FormWindowState.Minimized)
                {
                    trackSelectionForm.WindowState = FormWindowState.Normal;
                }
                trackSelectionForm.Focus();
            }
            if (onlineSyncForm != null && !onlineSyncForm.IsDisposed)
            {
                if (onlineSyncForm.WindowState == FormWindowState.Minimized)
                {
                    onlineSyncForm.WindowState = FormWindowState.Normal;
                }
                onlineSyncForm.Focus();
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

        private void HandleSoundBoardTrackRequest(object sender, SoundBoardEventArgs e)
        {
            isDonePlaying = false;
            Pause();
            LoadHelper(e.SelectedTrack);
            autoplay = true;
        }

        private void HandlePlayListChanged(object sender, PlayListEventArgs e)
        {
            if (e.EventType == PlayListEventArgs.UpdatedComponent.UpdateNavButtons)
            {
                ToggleNavButtons(playList.HasNext());
            }
            if (e.EventType == PlayListEventArgs.UpdatedComponent.PlayRequest)
            {
                isDonePlaying = false;
                Pause();
                LoadHelper(playList.Get(e.Id));
                autoplay = true;
            }
        }

        private void ToggleNavButtons(bool enable)
        {
            PreviousButton.Enabled = enable;
            NextButton.Enabled = enable;
        }

        private void MusicProgressBar_Scroll(object sender, EventArgs e)
        {
            player.SetPosition(MusicProgressBar.Value);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (player.GetPosition() < MusicProgressBar.Maximum)
            {
                MusicProgressBar.Value = player.GetPosition();
                StartLabel.Text = player.GetFormattedPosition();
            }
            else
            {
                if (ActionManager.AutoConsoleModeFromString(configManager.GetProperty("AutoConsoleOpen").Code) == ActionManager.AutoConsoleMode.Old)
                {
                    actionManager.ToggleConsole(false);
                }
                StartLabel.Text = EndTimeLabel.Text;
                PlayButton.Text = playButtonStartString;
                isDonePlaying = true;
                timer1.Stop();
                if (NextButton.Enabled)
                {
                    NextButton.PerformClick();
                }
            }
        }

        private void OpenDialogs()
        {
            if (configManager.GetBooleanProperty("DebugMode"))
            {
                loggerForm = new LoggerForm(loggerManager, configManager);
                loggerForm.Show();
            }
            if (configManager.GetBooleanProperty("SoundBoard"))
            {
                soundBoardForm = new SoundBoardForm(soundBoardManager, configManager);
                Point coords = configManager.GetWindowCoordinates("SoundBoardPos");
                soundBoardForm.Show();
                soundBoardForm.Top = coords.X;
                soundBoardForm.Left = coords.Y;
            }
            if (configManager.GetBooleanProperty("PlayList"))
            {
                playListForm = new PlayListForm(playList, configManager);
                Point coords = configManager.GetWindowCoordinates("PlayListPos");
                playListForm.Show();
                playListForm.Top = coords.X;
                playListForm.Left = coords.Y;

            }
            if (configManager.GetBooleanProperty("TrackSelection"))
            {
                trackSelectionForm = new TrackSelectionForm(trackSelectionManager, configManager);
                Point coords = configManager.GetWindowCoordinates("TrackSelectionPos");
                trackSelectionForm.Show();
                trackSelectionForm.Top = coords.X;
                trackSelectionForm.Left = coords.Y;
            }
            if (configManager.GetBooleanProperty("OnlineSync"))
            {
                onlineSyncForm = new OnlineSyncForm(onlineManager);
                onlineSyncForm.Show();
            }
        }

        protected override void WndProc(ref Message m)
        {
            hotkeyManager.HotkeyPressed(m.Msg);
            base.WndProc(ref m);
        }

        private void KeyBindingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new KeyBindingForm()).ShowDialog();
            configManager.RefreshConfigAndSave();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new SettingsForm()).ShowDialog();
            loggerManager.IsOn = configManager.GetBooleanProperty("DebugMode");
            if (configManager.GetBooleanProperty("DebugMode") && (loggerForm == null || loggerForm.IsDisposed))
            {
                loggerForm = new LoggerForm(loggerManager, configManager);
                loggerForm.Show();
            }
            configManager.RefreshConfigAndSave();
            player.ResetSoundEffects();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            player.Dispose();
            configManager.RefreshConfigAndSave();
            configManager.SetWindowCoordinates("MainWindowPos", new Point() { X = this.Top, Y = this.Left });
            if (soundBoardForm != null)
            {
                soundBoardForm.Close();
            }
            if (playListForm != null)
            {
                playListForm.Close();
            }
            if (trackSelectionForm != null)
            {
                trackSelectionForm.Close();
            }
            configManager.Save();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
            base.OnClosed(e);
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openMidiFileDialog = new OpenFileDialog();
            openMidiFileDialog.DefaultExt = "mid";
            openMidiFileDialog.Filter = "MIDI files|*.mid|All files|*.*";
            openMidiFileDialog.Title = "Open MIDI file";
            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;
                player.LoadFile(fileName);
                if (fileName.Contains("\\"))
                {
                    string[] fileNameSplit = fileName.Split('\\');
                    string filteredFileName = fileNameSplit[fileNameSplit.Length - 1].Replace(".mid", "");
                    currentTrackName = filteredFileName;
                }
                else
                {
                    currentTrackName = fileName;
                }
            }
        }

        private void LoadHelper(PlayListItem item)
        {
            player.LoadFile(item.Path);
            currentTrackName = item.Name;
        }

        private void LoadHelper(SoundBoardItem item)
        {
            player.LoadFile(item.Path);
            currentTrackName = item.Name;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (isDonePlaying)
            {
                player.Stop();
                player.Play();
                playButtonIsPlaying = false;
                isDonePlaying = false;
            }
            if (!playButtonIsPlaying)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        private void Play()
        {
            if (ActionManager.AutoConsoleModeFromString(configManager.GetProperty("AutoConsoleOpen").Code) == ActionManager.AutoConsoleMode.Old)
            {
                actionManager.ToggleConsole(true);
            }
            PlayButton.Text = playButtonStopString;
            player.Play();
            timer1.Start();
            playButtonIsPlaying = true;
            GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Basic, "Playing now");
        }
        private void Pause()
        {
            if (ActionManager.AutoConsoleModeFromString(configManager.GetProperty("AutoConsoleOpen").Code) == ActionManager.AutoConsoleMode.Old)
            {
                actionManager.ToggleConsole(false);
            }
            PlayButton.Text = playButtonStartString;
            player.Pause();
            timer1.Stop();
            playButtonIsPlaying = false;
            GlobalLogger.Log("LuteBotForm", LoggerManager.LoggerLevel.Basic, "Pausing now");
        }

        private void PlayListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playListForm = new PlayListForm(playList, configManager);
            Point coords = configManager.GetWindowCoordinates("PlayListPos");
            playListForm.Show();
            playListForm.Top = coords.X;
            playListForm.Left = coords.Y;

        }

        private void SoundBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            soundBoardForm = new SoundBoardForm(soundBoardManager, configManager);
            Point coords = configManager.GetWindowCoordinates("SoundBoardPos");
            soundBoardForm.Show();
            soundBoardForm.Top = coords.X;
            soundBoardForm.Left = coords.Y;

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            player.Stop();
            timer1.Stop();
            MusicProgressBar.Value = 0;
            PlayButton.Enabled = false;
            MusicProgressBar.Enabled = false;
            StopButton.Enabled = false;
            StartLabel.Text = "00:00";
            EndTimeLabel.Text = "00:00";
            CurrentMusicLabel.Text = "";
            playButtonIsPlaying = false;
            PlayButton.Text = playButtonStartString;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            Pause();
            playList.Next();
            autoplay = true;
            LoadHelper(playList.Get(playList.CurrentTrackIndex));
            playButtonIsPlaying = true;
            isDonePlaying = false;
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            Pause();
            playList.Previous();
            autoplay = true;
            LoadHelper(playList.Get(playList.CurrentTrackIndex));
            playButtonIsPlaying = true;
            isDonePlaying = false;
        }

        private void OnlineSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onlineSyncForm = new OnlineSyncForm(onlineManager);
            onlineSyncForm.Show();
        }

        private void TrackFilteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackSelectionForm = new TrackSelectionForm(trackSelectionManager, configManager);
            Point coords = configManager.GetWindowCoordinates("TrackSelectionPos");
            trackSelectionForm.Show();
            trackSelectionForm.Top = coords.X;
            trackSelectionForm.Left = coords.Y;
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
