using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Core.Midi
{
    class MidiPlayer : Player
    {
        private OutputDevice outDevice;
        private Sequence sequence;
        private Sequencer sequencer;

        private int outDeviceID = 0;

        public MidiPlayer()
        {
            if (OutputDevice.DeviceCount == 0)
            {
                //todo Error
            }
            else
            {
                try
                {
                    sequence = new Sequence();
                    outDevice = new OutputDevice(outDeviceID);

                    sequencer = new Sequencer();
                    sequencer.Position = 0;
                    sequencer.Sequence = sequence;
                    sequencer.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                    sequencer.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                    sequencer.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                    sequencer.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                    sequencer.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                    sequence.LoadCompleted += HandleLoadCompleted;
                    sequence.Format = 1;
                }
                catch (Exception ex)
                {
                    //todo error
                }
            }
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void ChangeCurrentTime(int newTimeInSecond)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            sequence.Dispose();

            if (outDevice != null)
            {
                outDevice.Dispose();
            }
        }

        public override void LoadSong(string Path)
        {
            try
            {
                sequencer.Stop();
                base.isPlaying = false;
                sequence.LoadAsync(Path);
            }
            catch (Exception ex)
            {
                //todo error
            }
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Play()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            isPlaying = false;
            sequencer.Stop();
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            base.OnLoadCompleted(this);
        }
    }
}
