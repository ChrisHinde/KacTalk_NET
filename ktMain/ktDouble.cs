using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KacTalk;
using System.Globalization;

namespace ktMainLib
{

    public class ktDouble : ktClass
    {
        public ktDouble(double value)
            : base("ktDouble")
        {
            m_value = value;
        }
        public ktDouble() : this(0.0) { }

        public ktDouble(ktDouble val)
            : base("ktDouble")
        {
            m_value = val.m_value;
            m_HardType = val.m_HardType;
            m_IsClass = val.m_IsClass;
            m_Parent = val.m_Parent;
        }

        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            switch (Name.AsLower())
            {
                case "_add":
                case "op+":
                case "operator+":
                case "add":
                    {
                        Value = Add(Arguments);
                        break;
                    }
            }

            return Value;
        }

        private ktValue Add(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            float res = 0.0f;

            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't add nothing (null) to a float!", ktERR.NOTDEF);
            }

            foreach (ktList L in Arguments)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                Arg = (ktValue)L.Node.Value;
                res += Arg.ToFloat();
            }
            Value = new ktValue("return", "ktDouble", new ktDouble(m_value + res), false, true);

            return Value;
        }

        private double GetAsDouble(ktValue Arg)
        {
            // Was we given a ktDouble??
            if (Arg.Type == "ktDouble")
            {
                // Get the value directly from the ktDouble object
                return ((ktDouble)Arg.Value).m_value;
            }
            // We got something else
            else
            {
                // Is this declared as a hard/strict type
                if (m_HardType)
                {
                    throw new ktError("ktDouble::Assign: Cant make '" + Arg.Type + "' into a (hard) ktDouble!", ktERR.WRONGTYPE);
                }
                // Not hard?
                else
                {
                    // Try to convert it to a float (Use the ToFloat of the argument value)
                    return Arg.ToFloat();
                }
            }
        }

        public override ktClass CreateObject(ktString Value)
        {
            return new ktDouble(Value.ToFloat());
        }
        public override ktClass CreateObject(ktValue Value)
        {
            return new ktDouble(GetAsDouble(Value));
        }
        public override ktClass CreateObject(object Value)
        {
            if (Value is float)
            {
                return new ktDouble((float)Value);
            }
            if ((Value is double) || (Value is Double))
            {
                return new ktDouble((double)Value);
            }
            else if ((Value is int) || (Value is Int32))
            {
                return new ktDouble((int)Value);
            }
            else
            {
                return new ktDouble((ktDouble)Value);
            }
        }

        public override ktClass CreateObject()
        {
            return new ktDouble();
        }
        public override string ToString()
        {
            return m_value.ToString(new CultureInfo("en-US"));
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }

        protected double m_value;
    }
    /*public class ktDouble : ktClass
    {
        public ktDouble(double value)
            : base("ktDouble")
        {
            m_value = value;
        }
        public ktDouble() : this(0.0f) { }

        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            switch (Name.AsLower())
            {
                case "_add":
                case "op+":
                case "operator+":
                case "add":
                    {
                        Value = Add(Arguments);
                        break;
                    }
            }

            return Value;
        }

        private ktValue Add(ktList Arguments)
        {
            ktValue Value = ktValue.Null;
            ktValue Arg = ktValue.Null;
            double res = 0.0f;

            if (Arguments.IsEmpty())
            {
                throw new ktError("Can't add nothing (null) to a double!", ktERR.NOTDEF);
            }

            foreach (ktList L in Arguments)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }
                Arg = (ktValue)L.Node.Value;
                res += (double)Arg.ToType("double");
            }
            Value = new ktValue("return", "ktDouble", new ktDouble(m_value + res), false, true);

            return Value;
        }

        public override ktClass CreateObject(ktString Value)
        {
            return new ktDouble(Value.ToDouble());
        }
        public override ktClass CreateObject(ktValue Value)
        {
            return new ktDouble((double)Value.ToType("double"));
        }
        public override ktClass CreateObject()
        {
            return new ktDouble();
        }
        public override string ToString()
        {
            return m_value.ToString();
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }

        protected double m_value;
    }*/
}
