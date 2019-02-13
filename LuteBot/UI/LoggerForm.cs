using LuteBot.Logger;
using LuteBot.Saving;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot.UI
{
    public partial class LoggerForm : Form
    {
        private LoggerManager logger;
        private ConfigManager configManager;
        private Stopwatch stopWatch;
        private int updateTiming = 2000;
        private string buffer;

        public LoggerForm(LoggerManager logger, ConfigManager configManager)
        {
            this.logger = logger;
            this.configManager = configManager;
            logger.InboundMessage += new EventHandler<LoggerEvent>(Logger_Log);
            InitializeComponent();
            RefreshLogView();
            LoggerLevelBox.SelectedIndex = (int)logger.CurrentLoggerLevel;
            stopWatch = new Stopwatch();
            stopWatch.Start();
            buffer = "";
            LoggerLevelBox.SelectedIndex = 3;
            GlobalLogger.Log("LoggerForm", LoggerManager.LoggerLevel.Essential, "LoggerForm Initialised");
        }

        private void RefreshLogView()
        {
            LoggerTextBox.Text = logger.GetLoggedEvents();
        }

        private void Logger_Log(object sender, LoggerEvent e)
        {
            if (stopWatch.ElapsedMilliseconds >= updateTiming || e.LogLevel == LoggerManager.LoggerLevel.Essential || e.LogLevel == LoggerManager.LoggerLevel.Basic) {
                stopWatch.Restart();
                updateTiming += 1000;
                try
                {
                    LoggerTextBox.Text += buffer + e.ToString();
                }
                catch (InvalidOperationException)
                {
                    string newText = buffer + e.ToString();
                    this.LoggerTextBox.Invoke((MethodInvoker)delegate {
                        this.LoggerTextBox.Text += newText;
                    });
                }
                buffer = "";
            }
            else
            {
                buffer += e.ToString();
            }
        }

        private void SaveLoggerButton_Click(object sender, EventArgs e)
        {
            SaveManager.SaveLog(logger);
        }

        private void LoggerLevelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(LoggerLevelBox.Items[LoggerLevelBox.SelectedIndex].ToString()))
            {
                Enum.TryParse(LoggerLevelBox.Items[LoggerLevelBox.SelectedIndex].ToString(), out LoggerManager.LoggerLevel loggerLevel);
                logger.CurrentLoggerLevel = loggerLevel;
            }
        }
    }
}
