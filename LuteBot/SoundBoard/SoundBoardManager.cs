using LuteBot.Logger;
using LuteBot.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot.SoundBoard
{
    public class SoundBoardManager
    {
        private SoundBoardItem[] soundBoard;

        public event EventHandler<SoundBoardEventArgs> SoundBoardTrackRequest;

        public SoundBoardManager()
        {
            soundBoard = new SoundBoardItem[9];
            GlobalLogger.Log("SoundBoard", LoggerManager.LoggerLevel.Essential, "SoundBoard Initialised");
        }

        public void Save()
        {
            SaveManager.SaveSoundBoard(soundBoard);
        }

        public void Load()
        {
            SoundBoardItem[] tempSoundBoard = SaveManager.LoadSoundBoard();
            if (soundBoard != null)
            {
                soundBoard = tempSoundBoard;
            }
        }

        public SoundBoardManager(int size)
        {
            soundBoard = new SoundBoardItem[size];
        }

        public bool IsTrackAssigned(int index)
        {
            bool result = false;
            if (index >= 0 && index < soundBoard.Length)
            {
                result = soundBoard[index] != null;
            }
            return result;
        }

        public void KeyPressed(Keys key)
        {
            for (int i = 0; i < soundBoard.Length; i++)
            {
                if (soundBoard[i] != null && soundBoard[i].Hotkey == key)
                {
                    PlayTrack(i);
                    break;
                }
            }
        }

        public void PlayTrack(int index)
        {
            if (index >= 0 && index < soundBoard.Length && soundBoard[index] != null && soundBoard[index].Path != null  && soundBoard[index].Path != "")
            {
                EventHelper(index);
            }
        }

        public void SetTrack(SoundBoardItem item, int index)
        {
            if (index >= 0 && index < soundBoard.Length)
            {
                soundBoard[index] = item;
            }
        }

        public SoundBoardItem GetTrack(int index)
        {
            
            SoundBoardItem newItem = new SoundBoardItem()
            {
                Hotkey = soundBoard[index].Hotkey,
                Path = soundBoard[index].Path,
                Name = soundBoard[index].Name
            };
            return newItem;
        }

        public List<SoundBoardItem> GetTrackList()
        {
            List<SoundBoardItem> trackList = new List<SoundBoardItem>();

            foreach (SoundBoardItem item in soundBoard)
            {
                SoundBoardItem newItem = new SoundBoardItem()
                {
                    Hotkey = item.Hotkey,
                    Path = item.Path,
                    Name = item.Name
                };
                trackList.Add(newItem);
            }

            return trackList;
        }

        public void DeleteTrack(int index)
        {
            soundBoard[index] = null;
        }

        private void EventHelper(int index)
        {
            EventHandler<SoundBoardEventArgs> handler = SoundBoardTrackRequest;
            SoundBoardEventArgs eventArgs = new SoundBoardEventArgs();
            eventArgs.SelectedTrack = soundBoard[index];
            handler?.Invoke(this, eventArgs);
        }
    }
}
