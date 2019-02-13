using LuteBot.Saving;
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
            RefreshConfigFoundLabel();
        }

        private void SetConfig_Click(object sender, EventArgs e)
        {
            configManager.RefreshConfigAndSave();
            configManager.ChangeProperty("MordhauInputIniLocation" , SaveManager.SetMordhauConfigLocation());
            configManager.Save();
            RefreshConfigFoundLabel();
        }

        private void RefreshConfigFoundLabel()
        {
            if (string.IsNullOrWhiteSpace(SaveManager.LoadMordhauConfig(configManager.GetProperty("MordhauInputIniLocation").Code)))
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
            List<ActionKey> hotkeys = configManager.GetHotkeys();

            String[] row =  new String[2];
            foreach (ActionKey hotkey in hotkeys)
            {
                row[0] = hotkey.Name;
                row[1] = hotkey.Code;
                HotkeysList.Items.Add(new ListViewItem(row));
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            configManager.Save();
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
                    configManager.ChangeProperty(selectedItem.SubItems[0].Text, selectedItem.SubItems[1].Text);
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
            configManager.Save();
            configManager.ChangePropertyAndSave("UserSavedConsoleKey", configManager.GetAction("OpenConsole").Code);
            configManager.ChangePropertyAndSave("OpenConsole", "Next");
            InitPropertiesList();
            string configLocation = configManager.GetProperty("MordhauInputIniLocation").Code;
            string configContent = SaveManager.LoadMordhauConfig(configLocation);
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
                    SaveManager.SaveMordhauConfig(configLocation, configContent);
                }
            }
        }

        private void RevertAutoConfig_Click(object sender, EventArgs e)
        {
            configManager.RefreshConfig();
            string configLocation = configManager.GetProperty("MordhauInputIniLocation").Code;
            string configContent = SaveManager.LoadMordhauConfig(configLocation);
            if (configContent.Contains("+ConsoleKeys=PageDown"))
            {
                configContent = configContent.Replace("+ConsoleKeys=PageDown","");
                SaveManager.SaveMordhauConfig(configLocation, configContent);
            }
            configManager.ChangePropertyAndSave("OpenConsole", configManager.GetProperty("UserSavedConsoleKey").Code);
            InitPropertiesList();
        }
        private void OpenConfig_Click(object sender, EventArgs e)
        {
            string path = configManager.GetProperty("MordhauInputIniLocation").Code;
            string cmd = "explorer.exe";
            string arg = "/select, " + path;
            Process.Start(cmd, arg);
        }
    }
}
