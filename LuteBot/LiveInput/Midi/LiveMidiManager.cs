using LuteBot.Core;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuteBot.LiveInput.Midi
{
    public class LiveMidiManager
    {
        private InputDevice inputDevice;
        private MordhauOutDevice outDevice;

        public EventHandler<ChannelMessageEventArgs> ChannelEventReceived;
        public int DeviceCount = InputDevice.DeviceCount;
        public int CurrentDevice { get => (inputDevice != null) ? inputDevice.DeviceID : -1; }

        public MordhauOutDevice OutDevice { get => outDevice; }

        public LiveMidiManager()
        {
            outDevice = new MordhauOutDevice();
            outDevice.HighMidiNoteId = 127;
            outDevice.LowMidiNoteId = 0;
            outDevice.CooldownNeeded = false;
        }

        public void Dispose()
        {
            if (inputDevice != null)
            {
                inputDevice.Dispose();
            }
        }

        public void SetMidiDevice(int id)
        {
            if (inputDevice != null)
            {
                inputDevice.Dispose();
            }
            inputDevice = new InputDevice(id);
            inputDevice.ChannelMessageReceived += ChannelMessageReceived;
        }

        public void ForceLowBound(int value)
        {
            outDevice.LowNoteId = value;
        }

        private void ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            ChannelEventReceived.Invoke(sender, e);
            outDevice.SendNote(e.Message);
        }

        public void On()
        {
            if (inputDevice != null)
            {
                inputDevice.StartRecording();
            }
        }

        public void Off()
        {
            if (inputDevice != null)
            {
                inputDevice.StopRecording();
            }
        }
    }
}
