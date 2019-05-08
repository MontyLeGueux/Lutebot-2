namespace LuteBot.UI
{
    partial class LiveInputForm
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
            this.PianoControl = new Sanford.Multimedia.Midi.UI.PianoControl();
            this.OnOffButton = new System.Windows.Forms.Button();
            this.DeviceComboBox = new System.Windows.Forms.ComboBox();
            this.MidiDevicesLabel = new System.Windows.Forms.Label();
            this.MidiListeningLabel = new System.Windows.Forms.Label();
            this.MidiDeviceStatusLabel = new System.Windows.Forms.Label();
            this.ReceivedEventsLabel = new System.Windows.Forms.Label();
            this.DeviceGroupBox = new System.Windows.Forms.GroupBox();
            this.SettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.MordhauOctavesRangeLabel = new System.Windows.Forms.Label();
            this.PlusOctaveButton = new System.Windows.Forms.Button();
            this.MinusOctaveButton = new System.Windows.Forms.Button();
            this.DeviceGroupBox.SuspendLayout();
            this.SettingsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PianoControl
            // 
            this.PianoControl.HighNoteID = 24;
            this.PianoControl.Location = new System.Drawing.Point(12, 134);
            this.PianoControl.LowNoteID = 0;
            this.PianoControl.Name = "PianoControl";
            this.PianoControl.NoteOnColor = System.Drawing.Color.SkyBlue;
            this.PianoControl.Size = new System.Drawing.Size(776, 92);
            this.PianoControl.TabIndex = 0;
            this.PianoControl.Text = "pianoControl1";
            this.PianoControl.PianoKeyDown += new System.EventHandler<Sanford.Multimedia.Midi.UI.PianoKeyEventArgs>(this.PianoControl_PianoKeyDown);
            // 
            // OnOffButton
            // 
            this.OnOffButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.OnOffButton.Location = new System.Drawing.Point(86, 47);
            this.OnOffButton.Name = "OnOffButton";
            this.OnOffButton.Size = new System.Drawing.Size(130, 28);
            this.OnOffButton.TabIndex = 3;
            this.OnOffButton.Text = "Off";
            this.OnOffButton.UseVisualStyleBackColor = false;
            this.OnOffButton.Click += new System.EventHandler(this.OnOffButton_Click);
            // 
            // DeviceComboBox
            // 
            this.DeviceComboBox.FormattingEnabled = true;
            this.DeviceComboBox.Location = new System.Drawing.Point(86, 18);
            this.DeviceComboBox.Name = "DeviceComboBox";
            this.DeviceComboBox.Size = new System.Drawing.Size(130, 21);
            this.DeviceComboBox.TabIndex = 4;
            this.DeviceComboBox.SelectedIndexChanged += new System.EventHandler(this.DeviceComboBox_SelectedIndexChanged);
            // 
            // MidiDevicesLabel
            // 
            this.MidiDevicesLabel.AutoSize = true;
            this.MidiDevicesLabel.Location = new System.Drawing.Point(6, 21);
            this.MidiDevicesLabel.Name = "MidiDevicesLabel";
            this.MidiDevicesLabel.Size = new System.Drawing.Size(74, 13);
            this.MidiDevicesLabel.TabIndex = 5;
            this.MidiDevicesLabel.Text = "Midi Devices :";
            // 
            // MidiListeningLabel
            // 
            this.MidiListeningLabel.AutoSize = true;
            this.MidiListeningLabel.Location = new System.Drawing.Point(6, 55);
            this.MidiListeningLabel.Name = "MidiListeningLabel";
            this.MidiListeningLabel.Size = new System.Drawing.Size(49, 13);
            this.MidiListeningLabel.TabIndex = 6;
            this.MidiListeningLabel.Text = "Listening";
            // 
            // MidiDeviceStatusLabel
            // 
            this.MidiDeviceStatusLabel.AutoSize = true;
            this.MidiDeviceStatusLabel.Location = new System.Drawing.Point(222, 21);
            this.MidiDeviceStatusLabel.Name = "MidiDeviceStatusLabel";
            this.MidiDeviceStatusLabel.Size = new System.Drawing.Size(102, 26);
            this.MidiDeviceStatusLabel.TabIndex = 7;
            this.MidiDeviceStatusLabel.Text = "Midi Device Status :\r\nDisconnected";
            // 
            // ReceivedEventsLabel
            // 
            this.ReceivedEventsLabel.AutoSize = true;
            this.ReceivedEventsLabel.Location = new System.Drawing.Point(6, 88);
            this.ReceivedEventsLabel.Name = "ReceivedEventsLabel";
            this.ReceivedEventsLabel.Size = new System.Drawing.Size(137, 13);
            this.ReceivedEventsLabel.TabIndex = 8;
            this.ReceivedEventsLabel.Text = "No Midi message Received";
            // 
            // DeviceGroupBox
            // 
            this.DeviceGroupBox.Controls.Add(this.MidiDevicesLabel);
            this.DeviceGroupBox.Controls.Add(this.MidiListeningLabel);
            this.DeviceGroupBox.Controls.Add(this.ReceivedEventsLabel);
            this.DeviceGroupBox.Controls.Add(this.OnOffButton);
            this.DeviceGroupBox.Controls.Add(this.DeviceComboBox);
            this.DeviceGroupBox.Controls.Add(this.MidiDeviceStatusLabel);
            this.DeviceGroupBox.Location = new System.Drawing.Point(12, 9);
            this.DeviceGroupBox.Name = "DeviceGroupBox";
            this.DeviceGroupBox.Size = new System.Drawing.Size(559, 119);
            this.DeviceGroupBox.TabIndex = 9;
            this.DeviceGroupBox.TabStop = false;
            this.DeviceGroupBox.Text = "Device Connection";
            // 
            // SettingsGroupBox
            // 
            this.SettingsGroupBox.Controls.Add(this.MordhauOctavesRangeLabel);
            this.SettingsGroupBox.Controls.Add(this.PlusOctaveButton);
            this.SettingsGroupBox.Controls.Add(this.MinusOctaveButton);
            this.SettingsGroupBox.Location = new System.Drawing.Point(577, 9);
            this.SettingsGroupBox.Name = "SettingsGroupBox";
            this.SettingsGroupBox.Size = new System.Drawing.Size(211, 119);
            this.SettingsGroupBox.TabIndex = 10;
            this.SettingsGroupBox.TabStop = false;
            this.SettingsGroupBox.Text = "Settings";
            // 
            // MordhauOctavesRangeLabel
            // 
            this.MordhauOctavesRangeLabel.AutoSize = true;
            this.MordhauOctavesRangeLabel.Location = new System.Drawing.Point(6, 18);
            this.MordhauOctavesRangeLabel.Name = "MordhauOctavesRangeLabel";
            this.MordhauOctavesRangeLabel.Size = new System.Drawing.Size(197, 13);
            this.MordhauOctavesRangeLabel.TabIndex = 2;
            this.MordhauOctavesRangeLabel.Text = "Octaves covered by mordhau : C1 to B2";
            // 
            // PlusOctaveButton
            // 
            this.PlusOctaveButton.Location = new System.Drawing.Point(135, 34);
            this.PlusOctaveButton.Name = "PlusOctaveButton";
            this.PlusOctaveButton.Size = new System.Drawing.Size(68, 23);
            this.PlusOctaveButton.TabIndex = 1;
            this.PlusOctaveButton.Text = "+1 Octave";
            this.PlusOctaveButton.UseVisualStyleBackColor = true;
            this.PlusOctaveButton.Click += new System.EventHandler(this.PlusOctaveButton_Click);
            // 
            // MinusOctaveButton
            // 
            this.MinusOctaveButton.Location = new System.Drawing.Point(9, 34);
            this.MinusOctaveButton.Name = "MinusOctaveButton";
            this.MinusOctaveButton.Size = new System.Drawing.Size(68, 23);
            this.MinusOctaveButton.TabIndex = 0;
            this.MinusOctaveButton.Text = "-1 Octave";
            this.MinusOctaveButton.UseVisualStyleBackColor = true;
            this.MinusOctaveButton.Click += new System.EventHandler(this.MinusOctaveButton_Click);
            // 
            // LiveInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 238);
            this.Controls.Add(this.SettingsGroupBox);
            this.Controls.Add(this.DeviceGroupBox);
            this.Controls.Add(this.PianoControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LiveInputForm";
            this.Text = "LiveInputForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LiveInputForm_FormClosed);
            this.DeviceGroupBox.ResumeLayout(false);
            this.DeviceGroupBox.PerformLayout();
            this.SettingsGroupBox.ResumeLayout(false);
            this.SettingsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Sanford.Multimedia.Midi.UI.PianoControl PianoControl;
        private System.Windows.Forms.Button OnOffButton;
        private System.Windows.Forms.ComboBox DeviceComboBox;
        private System.Windows.Forms.Label MidiDevicesLabel;
        private System.Windows.Forms.Label MidiListeningLabel;
        private System.Windows.Forms.Label MidiDeviceStatusLabel;
        private System.Windows.Forms.Label ReceivedEventsLabel;
        private System.Windows.Forms.GroupBox DeviceGroupBox;
        private System.Windows.Forms.GroupBox SettingsGroupBox;
        private System.Windows.Forms.Button PlusOctaveButton;
        private System.Windows.Forms.Button MinusOctaveButton;
        private System.Windows.Forms.Label MordhauOctavesRangeLabel;
    }
}