using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KacTalk;

namespace ktMainLib
{
    public class ktMainContext : ktContext
    {
        public ktMainContext() : base("Main")
        {
            this.AddClass(new ktInt());
            this.AddClass(new ktFloat());
            this.AddClass(new ktDouble());

            this.AddClass(new ktBool());

            this.AddClass(new ktStringClass());

            this.AddClass(new ktFunctionClass());
        }
    }
}
