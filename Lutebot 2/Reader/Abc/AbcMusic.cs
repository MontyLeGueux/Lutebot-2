using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Reader.Abc
{
    class AbcMusic
    {
        private List<AbcTune> tunes;

        public List<AbcTune> Tunes { get => tunes; set => tunes = value; }
    }
}
