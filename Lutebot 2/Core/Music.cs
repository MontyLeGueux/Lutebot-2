using Lutebot_2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Core
{
    public abstract class Music
    {
        private string name;
        private int lengthInSeconds;
        private List<Track> tracks;

        public string Name { get => name; set => name = value; }
        public int LengthInSeconds { get => lengthInSeconds; set => lengthInSeconds = value; }
        internal List<Track> Tracks { get => tracks; set => tracks = value; }
    }
}
