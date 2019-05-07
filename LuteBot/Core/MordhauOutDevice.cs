using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuteBot.Core
{
    public class MordhauOutDevice
    {
        private int lowNoteId = 0;
        private int highNoteId = 24;

        private int lowMidiNoteId = 0;
        private int highMidiNoteId = 127;

        private bool conversionNeeded;

        private int chordPosition = 0;

        public int LowMidiNoteId { get => lowMidiNoteId; set { lowMidiNoteId = value; UpdateNoteIdBounds(); } }
        public int HighMidiNoteId { get => highMidiNoteId; set { highMidiNoteId = value; UpdateNoteIdBounds(); } }

        public int LowNoteId { get => lowNoteId; }
        public int HighNoteId { get => highNoteId; }

        public bool ConversionNeeded { get => conversionNeeded; }

        private ConfigManager configManager;
        private ActionManager actionManager;
        private Stopwatch stopWatch;

        public MordhauOutDevice(ConfigManager configManager, ActionManager actionManager)
        {
            this.configManager = configManager;
            this.actionManager = actionManager;
            stopWatch = new Stopwatch();
        }

        private void UpdateNoteIdBounds()
        {
            int noteRange = highMidiNoteId - lowMidiNoteId;
            int luteRange = configManager.GetIntegerProperty("AvaliableNoteCount");
            if (noteRange > luteRange)
            {
                lowNoteId = ((noteRange / 2) + lowMidiNoteId) - (luteRange / 2);
                highNoteId = ((noteRange / 2) + lowMidiNoteId) + (luteRange / 2);
                lowNoteId = lowNoteId - (lowNoteId % 12);
                highNoteId = highNoteId - (highNoteId % 12) - 1;
                conversionNeeded = true;
            }
            else
            {
                conversionNeeded = false;
                //if the note range of the midi is lower than the lute range
                lowNoteId = lowMidiNoteId;
                highNoteId = highMidiNoteId;
            }
        }

        public ChannelMessage FilterNote(ChannelMessage message)
        {
            if (conversionNeeded && (message.Command == ChannelCommand.NoteOn || message.Command == ChannelCommand.NoteOff))
            {
                int newData1 = 0;
                int oldData1 = message.Data1;
                if (oldData1 < lowNoteId)
                {
                    newData1 = lowNoteId + (oldData1 % 12);
                }
                else
                {
                    if (oldData1 > highNoteId)
                    {
                        newData1 = (highNoteId - 11) + (oldData1 % 12);
                    }
                    else
                    {
                        newData1 = oldData1;
                    }
                }
                return new ChannelMessage(message.Command, message.MidiChannel, newData1, message.Data2);
            }
            else
            {
                return message;
            }
        }

        public void SendNote(ChannelMessage message)
        {
            if (message.Command == ChannelCommand.NoteOn)
            {
                int noteCooldown = int.Parse(configManager.GetProperty("NoteCooldown").Code);
                if (!stopWatch.IsRunning)
                {
                    actionManager.PlayNote(FilterNote(message).Data1 - lowNoteId);
                    stopWatch.Start();
                }
                else
                {
                    if (stopWatch.ElapsedMilliseconds >= noteCooldown)
                    {
                        actionManager.PlayNote(FilterNote(message).Data1 - lowNoteId);
                        stopWatch.Reset();
                    }
                }
            }
        }
    }
}
