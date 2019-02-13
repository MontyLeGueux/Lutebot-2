using Lutebot.Config;
using LuteBot.IO;
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
    /// <summary>
    /// The form used to change the settings stored in the configuration manager.
    /// </summary>
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

        /// <summary>
        /// Check the forum post for a newer version
        /// </summary>
        /// Need a better way to do it.
        /// <param name="timeout">The amount of time the window will be frozen to wait for the page to download</param>
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
            catch (WebException)
            {
                UpdateLinkLabel.Text = "Couldn't retrieve version. Retry";
            }
            catch (ThreadInterruptedException)
            {
                UpdateLinkLabel.Text = "Couldn't retrieve version. Retry";
            }
        }

        private void DownloadUrlSynchronously(string url)
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
            catch (WebException)
            {
            }
        }

        /// <summary>
        /// Event handler for the retry button when looking for the latest version doesn't work.
        /// </summary>
        /// Every time this button is clicked, the program will wait longer, up to 3 seconds (in case of bad internet).
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

        /// <summary>
        /// Initialize the ui with values in the configuration
        /// </summary>
        private void InitSettings()
        {
            SoundBoardCheckBox.Checked = bool.Parse(configManager.GetProperty(PropertyItem.SoundBoard));
            PlaylistCheckBox.Checked = bool.Parse(configManager.GetProperty(PropertyItem.PlayList));
            OnlineSyncCheckBox.Checked = bool.Parse(configManager.GetProperty(PropertyItem.OnlineSync));
            SoundEffectsCheckBox.Checked = bool.Parse(configManager.GetProperty(PropertyItem.SoundEffects));
            InitRadioButtons();

            NoteConversionMode.SelectedIndex = int.Parse(configManager.GetProperty(PropertyItem.NoteConversionMode));
            LowestNoteNumeric.Value = int.Parse(configManager.GetProperty(PropertyItem.LowestNoteId));
            NoteCountNumeric.Value = int.Parse(configManager.GetProperty(PropertyItem.AvaliableNoteCount));
            NoteCooldownNumeric.Value = int.Parse(configManager.GetProperty(PropertyItem.NoteCooldown));
            DebugModeCheckBox.Checked = bool.Parse(configManager.GetProperty(PropertyItem.DebugMode));
        }

        private void SetVersion()
        {
            VERSION = configManager.GetProperty(PropertyItem.Version);
            VersionLabel.Text = VersionLabel.Text.Replace("[VERSION]", VERSION);
        }

        /// <summary>
        /// Initialize the radio buttons with the values in the configuration.
        /// </summary>
        private void InitRadioButtons()
        {
            KeyboardOutput.AutoConsoleMode consoleMode = KeyboardOutput.AutoConsoleModeFromString(configManager.GetProperty(PropertyItem.AutoConsoleMode));
            switch (consoleMode)
            {
                case KeyboardOutput.AutoConsoleMode.New:
                    NewAutoConsoleRadio.Checked = true;
                    OldAutoConsoleRadio.Checked = false;
                    OffAutoConsoleRadio.Checked = false;
                    return;
                case KeyboardOutput.AutoConsoleMode.Old:
                    NewAutoConsoleRadio.Checked = false;
                    OldAutoConsoleRadio.Checked = true;
                    OffAutoConsoleRadio.Checked = false;
                    return;
                case KeyboardOutput.AutoConsoleMode.Off:
                    NewAutoConsoleRadio.Checked = false;
                    OldAutoConsoleRadio.Checked = false;
                    OffAutoConsoleRadio.Checked = true;
                    return;
            }
        }

        //------------- Handlers -------------

        private void PlaylistCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.PlayList, PlaylistCheckBox.Checked.ToString());
        }

        private void SoundBoardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.SoundBoard, SoundBoardCheckBox.Checked.ToString());
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            configManager.SaveConfig();
            this.Close();
        }

        private void SoundEffectsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.SoundEffects, SoundEffectsCheckBox.Checked.ToString());
        }

        private void TrackSelectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void OnlineSyncCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.OnlineSync, OnlineSyncCheckBox.Checked.ToString());
        }

        private void NoteConversionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.NoteConversionMode, NoteConversionMode.SelectedIndex.ToString());
        }

        private void LowestNoteNumeric_ValueChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.LowestNoteId, LowestNoteNumeric.Value.ToString());
        }

        private void NoteCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.AvaliableNoteCount, (NoteCountNumeric.Value).ToString());
        }

        private void NoteCooldownNumeric_ValueChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.NoteCooldown, NoteCooldownNumeric.Value.ToString());
        }

        private void DebugModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.DebugMode, DebugModeCheckBox.Checked.ToString());
        }

        private void OldAutoConsoleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (OldAutoConsoleRadio.Checked)
            {
                configManager.SetProperty(PropertyItem.AutoConsoleMode, KeyboardOutput.AutoConsoleModeToString(KeyboardOutput.AutoConsoleMode.Old));
                NewAutoConsoleRadio.Checked = false;
                OffAutoConsoleRadio.Checked = false;
            }
        }

        private void NewAutoConsoleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (NewAutoConsoleRadio.Checked)
            {
                configManager.SetProperty(PropertyItem.AutoConsoleMode, KeyboardOutput.AutoConsoleModeToString(KeyboardOutput.AutoConsoleMode.New));
                OldAutoConsoleRadio.Checked = false;
                OffAutoConsoleRadio.Checked = false;
            }
        }

        private void OffAutoConsoleRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (OffAutoConsoleRadio.Checked)
            {
                configManager.SetProperty(PropertyItem.AutoConsoleMode, KeyboardOutput.AutoConsoleModeToString(KeyboardOutput.AutoConsoleMode.Off));
                NewAutoConsoleRadio.Checked = false;
                OldAutoConsoleRadio.Checked = false;
            }
        }
    }
}
