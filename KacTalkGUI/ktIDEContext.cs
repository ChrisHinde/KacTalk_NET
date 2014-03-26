using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KacTalk;
using ktMainLib;

namespace KacTalkGUI
{
    class ktIDEContext : ktContext
    {

        public ktIDEContext()
            : base("ktIDE")
        {
//            this.AddFunction( new ktDelegateFunction( "output", new ktFunction_Delegate(Output) ) );
        }

        public ktIDEContext(ktContext con)
            : base("ktIDE")
        {
            con.AddFunction(new ktDelegateFunction("output", new ktFunction_Delegate(Output)));
            con.AddFunction(new ktDelegateFunction("print", new ktFunction_Delegate(Output)));
        }

        public static ktValue Output( ktList Arguments )
        {
            string str, ret = "";
            foreach (ktList L in Arguments)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                str = L.Node.ValueToString();
                ret += str;
                Console.WriteLine("OUTPUT:" + str );

                try 
                {
                    Form1.LogText( str );
                } catch (Exception) {}
            }

            return new ktValue("return", "ktString", new ktStringClass(ret), true, true);
        }
    }
}
