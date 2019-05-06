using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuteBot.Core
{
    public class MordhauControl
    {
        private int lowNoteId = 0;
        private int highNoteId = 24;

        private int lowMidiNoteId = 0;
        private int highMidiNoteId = 127;

        public int LowMidiNoteId { get => lowMidiNoteId; set { lowMidiNoteId = value; UpdateNoteIdBounds(); } }
        public int HighMidiNoteId { get => highMidiNoteId; set { highMidiNoteId = value; UpdateNoteIdBounds(); } }

        public int LowNoteId { get => lowNoteId; }
        public int HighNoteId { get => highNoteId; }

        public MordhauControl(ActionManager actionManager, ConfigManager configManager)
        {

        }

        private void UpdateNoteIdBounds()
        {
            throw new NotImplementedException();
        }

        public ChannelMessage FilterNote(ChannelMessage message)
        {
            //count the amount of octaves covered by the loaded midi file.
            int octaveCount = (highMidiNoteId - lowMidiNoteId) / 12;


        }
    }
}
