using Lutebot.Config;
using LuteBot.Logger;
using LuteBot.Playlist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot
{
    public partial class PlayListForm : Form
    {
        private Point mDownPos;
        private PlayList playList;
        private ConfigManager configManager;

        public PlayListForm(PlayList playList, ConfigManager configManager)
        {
            InitializeComponent();
            this.playList = playList;
            this.playList.PlayListUpdatedEvent += new EventHandler<PlayListEventArgs>(PlayList_Updated);
            this.configManager = configManager;
            RefreshPlayListBox();
            GlobalLogger.Log("PlayListForm", LoggerManager.LoggerLevel.Essential, "PlayListForm Initialised");
        }

        private void ContextMenuHelper()
        {
            if (PlayListBox.SelectedIndex >= 0)
            {
                ContextMenu playListContextMenu = new ContextMenu();
                
                MenuItem playItem = playListContextMenu.MenuItems.Add("Play");
                playItem.Click += new EventHandler(PlayMenuItem_Click);
                MenuItem deleteItem = playListContextMenu.MenuItems.Add("Remove");
                deleteItem.Click += new EventHandler(DeleteMenuItem_Click);
                PlayListBox.ContextMenu = playListContextMenu;
            }
        }

        private void PlayMenuItem_Click(object sender, EventArgs e)
        {
            playList.Play(PlayListBox.SelectedIndex);
            RefreshPlayListBox();
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            playList.Remove(PlayListBox.SelectedIndex);
            RefreshPlayListBox();
        }

        private void RefreshPlayListBox()

        {
            PlayListBox.Items.Clear();
            for (int i = 0; i < playList.Count(); i++)
            {
                PlayListBox.Items.Add(playList.Get(i));
            }
            Refresh();
        }

        private void PlayList_Updated(object sender, PlayListEventArgs e)
        {
            if (e.EventType.Equals(PlayListEventArgs.UpdatedComponent.TrackChanged))
            {
                playList.CurrentTrackIndex = e.Id;
            }
            Refresh();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openMidiFileDialog = new OpenFileDialog()
            {
                DefaultExt = "mid",
                Filter = "MIDI files|*.mid|All files|*.*",
                Title = "Open MIDI file"
            };
            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;
                string filteredFileName = fileName;
                if (fileName.Contains("\\"))
                {
                    string[] fileNameSplit = fileName.Split('\\');
                    filteredFileName = fileNameSplit[fileNameSplit.Length - 1].Replace(".mid", "");
                }
                PlayListItem music = new PlayListItem();
                music.Name = filteredFileName;
                music.Path = fileName;
                PlayListBox.Items.Add(music);
                playList.AddTrack(music);
            }
        }

        private class DragObject
        {
            public ListBox source;
            public object item;
            public DragObject(ListBox box, object data) { source = box; item = data; }
        }

        void List_DragEnter(object sender, DragEventArgs e)
        {
            DragObject obj = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (obj != null) e.Effect = e.AllowedEffect;
        }

        void List_DragDrop(object sender, DragEventArgs e)
        {
            DragObject obj = e.Data.GetData(typeof(DragObject)) as DragObject;
            int index = PlayListBox.IndexFromPoint(new Point(PlayListBox.PointToClient(Cursor.Position).X, PlayListBox.PointToClient(Cursor.Position).Y));
            int oldIndex = obj.source.Items.IndexOf(obj.item);
            PlayListItem tempItem = (PlayListItem)obj.item;
            obj.source.Items.Remove(obj.item);
            playList.Remove(tempItem);
            if (index < 0 || index >= PlayListBox.Items.Count)
            {
                PlayListBox.Items.Add(obj.item);
                playList.AddTrack(tempItem);
            }
            else
            {
                PlayListBox.Items.Insert(index, obj.item);
                playList.InsertTrack(index, tempItem);
            }
        }

        void List_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int index = PlayListBox.IndexFromPoint(e.Location);
            if (index < 0) return;
            if (Math.Abs(e.X - mDownPos.X) >= SystemInformation.DragSize.Width ||
                Math.Abs(e.Y - mDownPos.Y) >= SystemInformation.DragSize.Height)
                DoDragDrop(new DragObject(PlayListBox, PlayListBox.Items[index]), DragDropEffects.Move);
        }

        void List_MouseDown(object sender, MouseEventArgs e)
        {
            mDownPos = e.Location;
            int index = PlayListBox.IndexFromPoint(new Point(PlayListBox.PointToClient(Cursor.Position).X, PlayListBox.PointToClient(Cursor.Position).Y));
            if (index >= 0 && index < PlayListBox.Items.Count)
            {
                PlayListBox.SelectedIndex = index;
            }
            ContextMenuHelper();
        }

        void List_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                Brush myBrush = Brushes.Black;
                var box = (ListBox)sender;
                var fore = box.ForeColor;

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) fore = SystemColors.HighlightText;
                string text = "";
                if (box.Items[e.Index].GetType() == typeof(PlayListItem))
                {
                    if (((PlayListItem)box.Items[e.Index]).IsActive)
                    {
                        fore = Color.DarkOrange;
                    }
                    text = ((PlayListItem)box.Items[e.Index]).Name;
                }
                TextRenderer.DrawText(e.Graphics, text,
                    box.Font, e.Bounds, fore);
                e.DrawFocusRectangle();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            playList.SavePlayList();
            RefreshPlayListBox();
        }

        private void PlayListForm_Closing(object sender, FormClosingEventArgs e)
        {
            configManager.SetWindowCoordinates(PropertyItem.PlayListPos, new Point() { X = this.Top, Y = this.Left });
            configManager.SaveConfig();
        }

        private void LoadPlayListButton_Click(object sender, EventArgs e)
        {
            playList.LoadPlayList();
            RefreshPlayListBox();
        }
    }
}
 