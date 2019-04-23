using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lutebot.Reader.Abc.MusicElements;
using static Lutebot.Reader.Abc.MusicElements.AbcHeaderItem;

namespace Lutebot.Reader.Abc
{
    /// <summary>
    /// Headers used outside of the tune's header block.
    /// </summary>
    class AbcEvent : AbcItem
    {
        private AbcHeaderType type;
        private string content;

        public AbcHeaderType Type { get => type; set => type = value; }
        public string Content { get => content; set => content = value; }
    }
}
