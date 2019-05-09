using LuteBot.Config;
using LuteBot.IO.KB;
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
        private bool cooldownNeeded = true;

        public int LowMidiNoteId { get => lowMidiNoteId; set { lowMidiNoteId = value; UpdateNoteIdBounds(); } }
        public int HighMidiNoteId { get => highMidiNoteId; set { highMidiNoteId = value; UpdateNoteIdBounds(); } }

        public int LowNoteId { get => lowNoteId; set { ForceNoteBounds(value, true); } }
        public int HighNoteId { get => highNoteId; set { ForceNoteBounds(value, false); } }

        public bool ConversionNeeded { get => conversionNeeded; }
        public bool CooldownNeeded { get => cooldownNeeded; set => cooldownNeeded = value; }

        private Stopwatch stopWatch;

        public MordhauOutDevice()
        {
            stopWatch = new Stopwatch();
        }

        private void UpdateNoteIdBounds()
        {
            int noteRange = highMidiNoteId - lowMidiNoteId;
            int luteRange = ConfigManager.GetIntegerProperty(PropertyItem.AvaliableNoteCount);
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

        private void ForceNoteBounds(int value, bool isLower)
        {
            if (isLower)
            {
                lowNoteId = value;
                highNoteId = value + ConfigManager.GetIntegerProperty(PropertyItem.AvaliableNoteCount) - 1;
            }
            else
            {
                highNoteId = value;
                lowNoteId = value - ConfigManager.GetIntegerProperty(PropertyItem.AvaliableNoteCount) - 1;
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
            if (message != null && message.Command == ChannelCommand.NoteOn && message.Data2 > 0)
            {
                int noteCooldown = int.Parse(ConfigManager.GetProperty(PropertyItem.NoteCooldown));
                if (cooldownNeeded)
                {
                    if (!stopWatch.IsRunning)
                    {
                        ActionManager.PlayNote(FilterNote(message).Data1 - lowNoteId);
                        stopWatch.Start();
                    }
                    else
                    {
                        if (stopWatch.ElapsedMilliseconds >= noteCooldown)
                        {
                            ActionManager.PlayNote(FilterNote(message).Data1 - lowNoteId);
                            stopWatch.Reset();
                        }
                    }
                }
                else
                {
                    ActionManager.PlayNote(FilterNote(message).Data1 - lowNoteId);
                }
            }
        }
    }
}
