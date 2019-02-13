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

        public string Name { get => Name; set => Name = value; }
        public int LengthInSeconds { get => lengthInSeconds; set => lengthInSeconds = value; }
    }
}
