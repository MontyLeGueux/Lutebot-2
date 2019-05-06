using LuteBot.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot
{
    public partial class SettingsForm : Form
    {
        private readonly string versionAvaliable = "A new version is avaliable to download";
        private ConfigManager configManager;
        private static string VERSION;
        private static string THREAD_URL = "https://mordhau.com/forum/topic/13519/mordhau-lute-bot/";
        private string latestVersion;
        private int Timeout = 200;

        public SettingsForm()
        {
            InitializeComponent();
            configManager = new ConfigManager();
            UpdateLinkLabel.LinkArea = new LinkArea() { Length = 0, Start = 0 };
            SetVersion();
            InitSettings();
            CheckLatestVersion(Timeout);
            GlobalLogger.Log("SettingsForm", LoggerManager.LoggerLevel.Essential, "SettingsForm Initialised");
        }

        private void CheckLatestVersion(int timeout)
        {
            try
            {
                Thread latestVersionFetchThread;
                latestVersionFetchThread = new Thread(() => DownloadUrlSynchronously(THREAD_URL));
                latestVersionFetchThread.Start();
                latestVersionFetchThread.Join(timeout);

                if (!latestVersionFetchThread.IsAlive)
                {
                    if (latestVersion.CompareTo(VERSION) <= 0)
                    {
                        UpdateLinkLabel.Text = "You have the latest version avaliable";
                        UpdateLinkLabel.Links.Clear();
                    }
                    else
                    {
                        UpdateLinkLabel.Text = "New version avaliable : Click here";
                        UpdateLinkLabel.Links.Clear();
                        UpdateLinkLabel.Links.Add(24, 33, THREAD_URL);
                    }
                }
                else
                {
                    UpdateLinkLabel.Text = "Couldn't retrieve version. Retry";
                    UpdateLinkLabel.Links.Clear();
                    UpdateLinkLabel.Links.Add(27, 31, THREAD_URL);
                    latestVersionFetchThread.Abort();
                }
            }
            catch (WebException ex)
            {
                UpdateLinkLabel.Text = "Couldn't retrieve version. Retry";
            }
            catch (ThreadInterruptedException ex)
            {
                UpdateLinkLabel.Text = "Couldn't retrieve version. Retry";
            }
        }

        public void DownloadUrlSynchronously(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string downloadString = client.DownloadString(THREAD_URL);
                    string pattern = @"Mordhau Lute Bot V\d\.(\d\d|\d)";
                    Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                    Match m = r.Match(downloadString);
                    if (m.Success)
                    {
                        latestVersion = m.ToString().Split('V')[1];
                    }
                }
            }
            catch (WebException ex)
            {
            }
        }

        private void UpdateLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel.Link Link = UpdateLinkLabel.Links[UpdateLinkLabel.Links.IndexOf(e.Link)];
            if (Link.Start == 27)
            {
                if(Timeout < 3000){

                }
                CheckLatestVersion(Timeout + 1000);
            }
            else
            {
                Link.Visited = true;
                System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
            }
        }

        private void InitSettings()
        {
            SoundBoardCheckBox.Checked = configManager.GetBooleanProperty("SoundBoard");
            PlaylistCheckBox.Checked = configManager.GetBooleanProperty("PlayList");
            TrackSelectionCheckBox.Checked = configManager.GetBooleanProperty("TrackSelection");
            OnlineSyncCheckBox.Checked = configManager.GetBooleanProperty("OnlineSync");
            SoundEffectsCheckBox.Checked = configManager.GetBooleanProperty("SoundEffects");
            InitRadioButtons();

            NoteConversionMode.SelectedIndex = int.Parse(configManager.GetProperty("NoteConversionMode").Code);
            LowestNoteNumeric.Value = int.Parse(configManager.GetProperty("LowestNoteId").Code);
            NoteCountNumeric.Value = int.Parse(configManager.GetProperty("AvaliableNoteCount").Code);
            NoteCooldownNumeric.Value = int.Parse(configManager.GetProperty("NoteCooldown").Code);
            DebugModeCheckBox.Checked = configManager.GetBooleanProperty("DebugMode");
        }

        private void SetVersion()
        {
            VersionLabel.Text = VersionLabel.Text.Replace("[VERSION]", configManager.GetVersion());
            VERSION = configManager.GetVersion();
        }

        private void PlaylistCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("PlayList", PlaylistCheckBox.Checked.ToString());
        }

        private void SoundBoardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("SoundBoard", SoundBoardCheckBox.Checked.ToString());
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            configManager.RefreshConfigAndSave();
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            configManager.Save();
            configManager.RefreshConfigAndSave();
            this.Close();
        }

        private void SoundEffectsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("SoundEffects", SoundEffectsCheckBox.Checked.ToString());
        }

        private void TrackSelectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("TrackSelection", TrackSelectionCheckBox.Checked.ToString());
        }

        private void OnlineSyncCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("OnlineSync", OnlineSyncCheckBox.Checked.ToString());
        }

        private void NoteConversionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("NoteConversionMode", NoteConversionMode.SelectedIndex.ToString());
        }

        private void LowestNoteNumeric_ValueChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("LowestNoteId", LowestNoteNumeric.Value.ToString());
        }

        private void NoteCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("AvaliableNoteCount", (NoteCountNumeric.Value).ToString());
        }

        private void NoteCooldownNumeric_ValueChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("NoteCooldown", NoteCooldownNumeric.Value.ToString());
        }

        private void DebugModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.ChangeProperty("DebugMode", DebugModeCheckBox.Checked.ToString());
        }

        private void InitRadioButtons()
        {
            ActionManager.AutoConsoleMode consoleMode = ActionManager.AutoConsoleModeFromString(configManager.GetProperty("AutoConsoleOpen").Code);
            switch (consoleMode)
            {
                case ActionManager.AutoConsoleMode.New:
                    NewAutoConsoleRadio.Checked = true;
                    OldAutoConsoleRadio.Checked = false;
                    OffAutoConsoleRadio.Checked = false;
                    return;
                case ActionManager.AutoConsoleMode.Old:
                    NewAutoConsoleRadio.Checked = false;
                    OldAutoConsoleRadio.Checked = true;
                    OffAutoConsoleRadio.Checked = false;
                    return;
                case ActionManager.AutoConsoleMode.Off:
                    NewAutoConsoleRadio.Checked = false;
                    OldAutoConsoleRadio.Checked = false;
                    OffAutoConsoleRadio.Checked = true;
                    return;
            }
        }

        private void OldAutoConsoleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (OldAutoConsoleRadio.Checked)
            {
                configManager.ChangeProperty("AutoConsoleOpen", ActionManager.AutoConsoleModeToString(ActionManager.AutoConsoleMode.Old));
                NewAutoConsoleRadio.Checked = false;
                OffAutoConsoleRadio.Checked = false;
            }
        }

        private void NewAutoConsoleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (NewAutoConsoleRadio.Checked)
            {
                configManager.ChangeProperty("AutoConsoleOpen", ActionManager.AutoConsoleModeToString(ActionManager.AutoConsoleMode.New));
                OldAutoConsoleRadio.Checked = false;
                OffAutoConsoleRadio.Checked = false;
            }
        }

        private void OffAutoConsoleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (OffAutoConsoleRadio.Checked)
            {
                configManager.ChangeProperty("AutoConsoleOpen", ActionManager.AutoConsoleModeToString(ActionManager.AutoConsoleMode.Off));
                NewAutoConsoleRadio.Checked = false;
                OldAutoConsoleRadio.Checked = false;
            }
        }
    }
}
