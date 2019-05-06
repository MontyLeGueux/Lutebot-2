using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuteBot
{
    /**
     * Represent a hotkey, with a property name and a key code.
     */
    public class ActionKey
    {
        private string name;
        private string code;

        public string Name { get => name; set => name = value; }
        public string Code { get => code; set => code = value; }

        public ActionKey (string name, string code){
            this.name = name;
            this.code = code;
        }
    }
}
