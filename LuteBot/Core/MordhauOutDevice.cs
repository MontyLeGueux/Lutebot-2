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
        private int lowNoteId = 1;
        private int highNoteId = 60;

        private int lowMidiNoteId = 0;
        private int highMidiNoteId = 127;

        private bool conversionNeeded;
        private bool cooldownNeeded = true;
        private bool muteOutOfRange = false;

        public int LowMidiNoteId { get => lowMidiNoteId; set { lowMidiNoteId = value; UpdateNoteIdBounds(); } }
        public int HighMidiNoteId { get => highMidiNoteId; set { highMidiNoteId = value; UpdateNoteIdBounds(); } }

        public int LowNoteId { get => lowNoteId; set { ForceNoteBounds(value, true); } }
        public int HighNoteId { get => highNoteId; set { ForceNoteBounds(value, false); } }

        public bool ConversionNeeded { get => conversionNeeded; }
        public bool CooldownNeeded { get => cooldownNeeded; set => cooldownNeeded = value; }
        public bool MuteOutOfRange { get => muteOutOfRange; set => muteOutOfRange = value; }

        private Stopwatch stopWatch;

        public MordhauOutDevice()
        {
            stopWatch = new Stopwatch();
        }

        private void UpdateNoteIdBounds()
        {
            
        }

        private void ForceNoteBounds(int value, bool isLower)
        {
           
        }



        public ChannelMessage FilterNote(ChannelMessage message)
        {
     
        
            int oldData1 = message.Data1;
            int velocity = message.Data2;
            Console.WriteLine(oldData1.ToString());

            float x = oldData1;
            float x1 = 36;
            float x2 = 96;
            float y1 = 1;
            float y2 = 60;

            var m = (y2 - y1) / (x2 - x1);
            float c = y1 - m * x1;

            int _out =(int)( m * (float)(x) + c);


            return new ChannelMessage(message.Command, message.MidiChannel, _out, velocity);

        }

        public void SendNote(ChannelMessage message)
        {
            ChannelMessage filterResult;
            if (message.Command == ChannelCommand.NoteOn && message.Data2 > 0)
            {
                int noteCooldown = int.Parse(ConfigManager.GetProperty(PropertyItem.NoteCooldown));
                if (cooldownNeeded)
                {
                    if (!stopWatch.IsRunning)
                    {
                        filterResult = FilterNote(message);
                        if (message.Data2 > 0)
                        {
                            ActionManager.PlayNote(filterResult.Data1 - lowNoteId);
                        }

                        stopWatch.Start();
                    }
                    else
                    {
                        if (stopWatch.ElapsedMilliseconds >= noteCooldown)
                        {
                            filterResult = FilterNote(message);
                            if (message.Data2 > 0)
                            {
                                ActionManager.PlayNote(filterResult.Data1 - lowNoteId);
                            }
                            stopWatch.Reset();
                        }
                    }
                }
                else
                {
                    filterResult = FilterNote(message);
                    if (message.Data2 > 0)
                    {
                        ActionManager.PlayNote(filterResult.Data1 - lowNoteId);
                    }
                }
            }
        }
    }
}
