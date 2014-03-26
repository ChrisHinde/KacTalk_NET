using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KacTalk;

namespace KacTalkGUI
{
    class ktIDEModule : ktModule
    {
        public ktIDEModule()
            : base("ktIDEModule", "")
        {
            this.AddContext(new ktIDEContext());
        }
    }
}
