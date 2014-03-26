using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KacTalk;

namespace ktMainLib
{
    public class ktMain : ktModule
    {
        public ktMain()
            : base( "ktMain", "" )
        {
            this.AddContext(new ktMainContext());
        }
    }
}
