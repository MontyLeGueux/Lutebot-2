using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lutebot.Reader.Abc.MusicElements.AbcHeaderItem;

namespace Lutebot.Reader.Abc
{
    class AbcParser
    {
        public static AbcMusic ParseFile(string abcFile)
        {
            AbcMusic music = new AbcMusic();
            string[] elements = abcFile.Split(null);
            List<int> tuneIds = new List<int>();
            AbcTune currentTune = new AbcTune();

            bool inATune = false;
            bool inAHeader = false;

            foreach (string element in elements)
            {

            }
            return music;
        }
    }
}
