using Lutebot.Config;
using LuteBot.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot
{
    /// <summary>
    /// The form used to let the user change his keybinds
    /// </summary>
    public partial class KeyBindingForm : Form
    {
        private ConfigManager configManager;
        private string currentKey;

        private readonly string keyChangeString = "[input a key or ESC to cancel]";
        public KeyBindingForm()
        {
            InitializeComponent();
            configManager = new ConfigManager();
            InitPropertiesList();
        }

        private void SetConfig_Click(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.MordhauInputIniLocation, FileIO.SetMordhauConfigLocation());
            configManager.SaveConfig();
            RefreshConfigFoundLabel();
        }

        private void RefreshConfigFoundLabel()
        {
            if (string.IsNullOrWhiteSpace(FileIO.LoadMordhauConfig(configManager.GetProperty(PropertyItem.MordhauInputIniLocation))))
            {
                MordhauConfigLabel.Text = "Mordhau configuration file not found. Please set the location of DefaultInput.ini in the menu above.";
            }
            else
            {
                MordhauConfigLabel.Text = "Mordhau configuration file found";
            }
        }

        private void InitPropertiesList()
        {
            HotkeysList.Items.Clear();
            List<Hotkey> hotkeys = configManager.GetAll();

            String[] row =  new String[2];
            foreach (Hotkey hotkey in hotkeys)
            {
                row[0] = hotkey.Item.ToString();
                row[1] = hotkey.Key.ToString();
                HotkeysList.Items.Add(new ListViewItem(row));
            }
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

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            Keys tempKey;
            if (e.KeyCode == Keys.Escape)
            {
                if (HotkeysList.SelectedItems.Count > 0)
                {
                    ListViewItem selectedItem = HotkeysList.SelectedItems[0];
                    selectedItem.SubItems[1].Text = currentKey;
                }
            }
            else {
                if (HotkeysList.SelectedItems.Count > 0)
                {
                    ListViewItem selectedItem = HotkeysList.SelectedItems[0];
                    Enum.TryParse<Keys>(e.KeyCode.ToString(), out tempKey);
                    selectedItem.SubItems[1].Text = tempKey.ToString();
                    configManager.SetProperty(Property.FromString(selectedItem.SubItems[0].Text), selectedItem.SubItems[1].Text);
                }
            }
            ToggleEnableLists(true);
        }

        private void HotkeysList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HotkeysList.SelectedItems.Count > 0)
            {
                ToggleEnableLists(false);
                ListViewItem selectedItem = HotkeysList.SelectedItems[0];
                currentKey = selectedItem.SubItems[1].Text;
                selectedItem.SubItems[1].Text = keyChangeString;
            }
        }

        private void ToggleEnableLists(bool enabled)
        {
            HotkeysList.Enabled = enabled;
        }

        private void AutoConFigButton_Click(object sender, EventArgs e)
        {
            configManager.SetProperty(PropertyItem.UserSavedConsoleKey, configManager.GetHotkey(HotkeyItem.OpenConsole).ToString());
            configManager.SetHotkey(HotkeyItem.OpenConsole, Keys.Next);
            InitPropertiesList();
            string configLocation = configManager.GetProperty(PropertyItem.MordhauInputIniLocation);
            string configContent = FileIO.LoadMordhauConfig(configLocation);
            if (!configContent.Contains("+ConsoleKeys=PageDown"))
            {
                int index = -1;
                int length = -1;
                foreach (Match match in Regex.Matches(configContent, @"(-|\+|)ConsoleKey(s|)=.*(\s|$)"))
                {
                    index = match.Index;
                    length = match.Length;
                }
                if (index >= 0 && length > 0)
                {
                    configContent = configContent.Insert((index + length), "+ConsoleKeys=PageDown\n");
                    FileIO.SaveMordhauConfig(configLocation, configContent);
                }
            }
            configManager.SaveConfig();
        }

        private void RevertAutoConfig_Click(object sender, EventArgs e)
        {
            string configLocation = configManager.GetProperty(PropertyItem.MordhauInputIniLocation);
            string configContent = FileIO.LoadMordhauConfig(configLocation);
            if (configContent.Contains("+ConsoleKeys=PageDown"))
            {
                configContent = configContent.Replace("+ConsoleKeys=PageDown","");
                FileIO.SaveMordhauConfig(configLocation, configContent);
            }
            configManager.SetHotkey(HotkeyItem.OpenConsole, Hotkey.KeyFromString(configManager.GetProperty(PropertyItem.UserSavedConsoleKey)));
            InitPropertiesList();
        }
        private void OpenConfig_Click(object sender, EventArgs e)
        {
            string path = configManager.GetProperty(PropertyItem.MordhauInputIniLocation);
            string cmd = "explorer.exe";
            string arg = "/select, " + path;
            Process.Start(cmd, arg);
        }
    }
}
