using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Reader.Abc.MusicElements
{
    /// <summary>
    /// Define one header line.
    /// </summary>
    public class AbcHeaderItem
    {
        public enum AbcHeaderType
        {
            Area = 'A',
            Book = 'B',
            Composer = 'C',
            Discography = 'D',
            Elemskip = 'E',
            FileName = 'F',
            Group = 'G',
            History = 'H',
            Information = 'I',
            Key = 'K',
            NoteLength = 'L',
            Meter = 'M',
            Notes = 'N',
            Origin = 'O',
            Parts = 'P',
            Tempo = 'Q',
            Rhythm = 'R',
            Voice = 'V',
            Source = 'S',
            Title = 'T',
            Words = 'W',
            WordsEvent = 'w',
            IndexNumber = 'X',
            Transcription = 'T'
        }
    }
}
