namespace LuteBot.UI
{
    partial class LoggerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoggerForm));
            this.LoggerTextBox = new System.Windows.Forms.RichTextBox();
            this.SaveLogsButton = new System.Windows.Forms.Button();
            this.LoggerLevelBox = new System.Windows.Forms.ComboBox();
            this.LoggerLevelLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LoggerTextBox
            // 
            this.LoggerTextBox.Location = new System.Drawing.Point(13, 13);
            this.LoggerTextBox.Name = "LoggerTextBox";
            this.LoggerTextBox.ReadOnly = true;
            this.LoggerTextBox.Size = new System.Drawing.Size(1291, 589);
            this.LoggerTextBox.TabIndex = 0;
            this.LoggerTextBox.Text = "";
            // 
            // SaveLogsButton
            // 
            this.SaveLogsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveLogsButton.Location = new System.Drawing.Point(12, 608);
            this.SaveLogsButton.Name = "SaveLogsButton";
            this.SaveLogsButton.Size = new System.Drawing.Size(214, 30);
            this.SaveLogsButton.TabIndex = 1;
            this.SaveLogsButton.Text = "Save Logs";
            this.SaveLogsButton.UseVisualStyleBackColor = true;
            this.SaveLogsButton.Click += new System.EventHandler(this.SaveLoggerButton_Click);
            // 
            // LoggerLevelBox
            // 
            this.LoggerLevelBox.FormattingEnabled = true;
            this.LoggerLevelBox.Items.AddRange(new object[] {
            "Essential",
            "Basic",
            "Medium",
            "Complete"});
            this.LoggerLevelBox.Location = new System.Drawing.Point(1116, 614);
            this.LoggerLevelBox.Name = "LoggerLevelBox";
            this.LoggerLevelBox.Size = new System.Drawing.Size(188, 21);
            this.LoggerLevelBox.TabIndex = 2;
            this.LoggerLevelBox.SelectedIndexChanged += new System.EventHandler(this.LoggerLevelBox_SelectedIndexChanged);
            // 
            // LoggerLevelLabel
            // 
            this.LoggerLevelLabel.AutoSize = true;
            this.LoggerLevelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoggerLevelLabel.Location = new System.Drawing.Point(1011, 615);
            this.LoggerLevelLabel.Name = "LoggerLevelLabel";
            this.LoggerLevelLabel.Size = new System.Drawing.Size(99, 17);
            this.LoggerLevelLabel.TabIndex = 3;
            this.LoggerLevelLabel.Text = "Logger Level :";
            // 
            // LoggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1316, 650);
            this.Controls.Add(this.LoggerLevelLabel);
            this.Controls.Add(this.LoggerLevelBox);
            this.Controls.Add(this.SaveLogsButton);
            this.Controls.Add(this.LoggerTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoggerForm";
            this.Text = "Lute Bot Logs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LoggerTextBox;
        private System.Windows.Forms.Button SaveLogsButton;
        private System.Windows.Forms.ComboBox LoggerLevelBox;
        private System.Windows.Forms.Label LoggerLevelLabel;
    }
}