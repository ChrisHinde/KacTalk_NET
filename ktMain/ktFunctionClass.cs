using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KacTalk;

namespace ktMainLib
{
    public class ktFunctionClass : ktClass
    {
        public ktFunctionClass( ktFunction Func ) : base("ktFunction")
        {
            m_function = Func;
        }
        public ktFunctionClass() : this(null) { }

        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            switch (Name.AsLower())
            {
                case "run":
                case "_run":
                case "execute":
                case "_execute":
                case "_func_call":
                    {
                        Value = Run( Arguments );
                        break;
                    }
                default:
                    {
                        throw new ktError("Couldn't find the method '" +
                                          Name + "' in class '" + m_Name + "'.", ktERR._404);
                    }
            }

            return Value;
        }

        public ktValue Run(ktList Arguments)
        {
            if (m_function == null)
            {
                throw new ktError("No function defined in ktFunction wrapper class!", ktERR.NOTDEF);
            }

            return m_function.Run(Arguments);
        }

        public override ktClass CreateObject(object Obj)
        {
            //ktDebug.Log("ktF::CO :" +  Obj.GetType().ToString() );
            if (Obj is ktFunctionClass)
            {
                return new ktFunctionClass(((ktFunctionClass)Obj).m_function);
            }
            else if (Obj is ktFunction)
            {
                return new ktFunctionClass((ktFunction)Obj);
            } 
            else
            {
                return null;
            }
            //return base.CreateObject(Obj);
        }
        public override ktClass CreateObject()
        {
            return new ktFunctionClass();
        }
        public override string ToString()
        {
            return m_function.ToString();
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }

        private ktFunction m_function;
    }
}
