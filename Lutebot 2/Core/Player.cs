using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Core
{
    /// <summary>
    /// Class used to play a Music object.
    /// </summary>
    public abstract class Player
    {
        protected bool isPlaying;
        protected int currentTimeInSeconds;

        public bool IsPlaying { get => isPlaying; }
        public int CurrentTimeInSeconds { get => currentTimeInSeconds; }

        public event EventHandler<EventArgs> SongLoadCompleted;

        public event EventHandler<EventArgs> DonePlaying;

        public event EventHandler<EventArgs> PlayingProgress;

        public abstract void Stop();

        public abstract void LoadSong(string Path);

        public abstract void Play();

        public abstract void Pause();

        public abstract void ChangeCurrentTime(int newTimeInSecond);

        public abstract void Dispose();

        protected void OnLoadCompleted(object sender)
        {
            SongLoadCompleted.Invoke(sender, null);
        }

        protected void IsDonePlaying(object sender)
        {
            DonePlaying.Invoke(sender, null);
        }
    }
}
