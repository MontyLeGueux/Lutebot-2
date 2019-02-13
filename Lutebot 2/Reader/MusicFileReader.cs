using Lutebot.Core;
using Lutebot.Reader.Abc;
using LuteBot.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Reader
{
    class MusicFileReader
    {
        public static AbcMusic ReadAbcFile(string abcFileName)
        {
            AbcMusic result = null;
            string content = FileIO.LoadTextFile(abcFileName);
            if (content != null)
            {
                try
                {
                    result = AbcParser.ParseFile(content);
                }
                catch (AbcParsingException e)
                {
                    throw new LutebotException(e.Message);
                }
            }
            else
            {
                throw new LutebotException("Couldn't load the file :\n" + abcFileName);
            }
            return result;
        }
    }
}
