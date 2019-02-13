using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lutebot.Core
{
    class LutebotException : Exception
    {
        public LutebotException(string message)
        {
            MessageBox.Show("An error has occured", "Something went wrong :\n"+message);
        }
    }
}
