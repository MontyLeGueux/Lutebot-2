using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutebot.Reader.Abc
{
    /// <summary>
    /// Header block at the beginning of a tune
    /// </summary>
    public class AbcHeader
    {
        private List<AbcHeader> headers;

        public List<AbcHeader> Headers { get => headers; set => headers = value; }
    }
}
