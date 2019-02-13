using LuteBot.Logger;
using LuteBot.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuteBot.playlist
{
    public class PlayList
    {
        private List<PlayListItem> musicList;
        private int currentTrackIndex;

        public int CurrentTrackIndex { get => currentTrackIndex; set => currentTrackIndex = value; }

        public PlayList()
        {
            musicList = new List<PlayListItem>();
            currentTrackIndex = 0;
            GlobalLogger.Log("PlayList", LoggerManager.LoggerLevel.Essential, "PlayList Initialised");
        }

        public event EventHandler<PlayListEventArgs> PlayListUpdatedEvent;

        public void SavePlayList()
        {
            SaveManager.SavePlayList(musicList);
        }

        public void LoadPlayList()
        {
            List<PlayListItem> tempMusicList = SaveManager.LoadPlayList();
            if (tempMusicList != null)
            {
                musicList = tempMusicList;
                EventHelper(PlayListEventArgs.UpdatedComponent.TrackChanged, 0);
                EventHelper(PlayListEventArgs.UpdatedComponent.UpdateNavButtons, -1);
            } 
        }

        public int Count()
        {
            return musicList.Count;
        }

        public void Next()
        {
            musicList[currentTrackIndex].IsActive = false;
            if (CurrentTrackIndex + 1 < musicList.Count)
            {
                currentTrackIndex++;
            }
            else
            {
                currentTrackIndex = 0;
            }
            musicList[currentTrackIndex].IsActive = true;
            EventHelper(PlayListEventArgs.UpdatedComponent.TrackChanged, CurrentTrackIndex);
        }

        public void Play(int trackId)
        {
            musicList[currentTrackIndex].IsActive = false;
            currentTrackIndex = trackId;
            musicList[currentTrackIndex].IsActive = true;
            EventHelper(PlayListEventArgs.UpdatedComponent.PlayRequest, CurrentTrackIndex);
        }

        public void Previous()
        {
            musicList[currentTrackIndex].IsActive = false;
            if (CurrentTrackIndex - 1 < 0)
            {
                currentTrackIndex = musicList.Count - 1;
            }
            else
            {
                currentTrackIndex--;
            }
            musicList[currentTrackIndex].IsActive = true;
            EventHelper(PlayListEventArgs.UpdatedComponent.TrackChanged, CurrentTrackIndex);
        }

        private void EventHelper(PlayListEventArgs.UpdatedComponent component, int trackId)
        {
            EventHandler<PlayListEventArgs> handler = PlayListUpdatedEvent;
            PlayListEventArgs eventArgs = new PlayListEventArgs();
            eventArgs.EventType = component;
            eventArgs.Id = trackId;
            handler?.Invoke(this, eventArgs);
        }

        public PlayListItem Get(int index)
        {
            return musicList[index];
        }

        public void AddTrack(PlayListItem item)
        {
            musicList.Add(item);
            if (musicList.Count == 2)
            {
                EventHelper(PlayListEventArgs.UpdatedComponent.UpdateNavButtons, -1);
            }
        }

        public void InsertTrack(int index, PlayListItem item)
        {
            musicList.Insert(index, item);
            if (musicList.Count == 2)
            {
                EventHelper(PlayListEventArgs.UpdatedComponent.UpdateNavButtons, -1);
            }   
        }

        public void Remove(int index)
        {
            musicList.RemoveAt(index);
            if (musicList.Count == 1)
            {
                EventHelper(PlayListEventArgs.UpdatedComponent.UpdateNavButtons, -1);
            } 
        }

        public void Remove(PlayListItem item)
        {
            musicList.Remove(item);
            if (musicList.Count == 1)
            {
                EventHelper(PlayListEventArgs.UpdatedComponent.UpdateNavButtons, -1);
            }
        }

        public bool HasNext()
        {
            return musicList.Count > 1;
        }
    }
}
