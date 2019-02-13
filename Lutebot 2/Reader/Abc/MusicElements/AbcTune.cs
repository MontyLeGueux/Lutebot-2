using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Reader.Abc
{
    /// <summary>
    /// A tune in an abc file. Composed of a header block and a list of notes or events.
    /// </summary>
    class AbcTune
    {
        private AbcHeader header;
        private List<AbcEvent> track;

        public List<AbcEvent> Track { get => track; set => track = value; }
        public AbcHeader Header { get => header; set => header = value; }
    }
}
