using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Core
{
    class Track
    {
        private List<Note> notes;
        private int id;

        public List<Note> Notes { get => notes; set => notes = value; }
        public int Id { get => id; set => id = value; }
    }
}
