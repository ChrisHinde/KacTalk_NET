using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KacTalk;

namespace ktMainLib
{
    public class ktBool : ktClass
    {
        public ktBool(bool value)
            : base("ktBool")
        {
            m_value = value;
        }
        public ktBool() : this(false) { }


        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            ktValue Value = ktValue.Null;

            switch (Name.AsLower())
            {
                case "!":
                case "not":
                case "_not":
                case "op!":
                case "operator!":
                    {
                        Value = _Not();
                        break;
                    }
                case "&&":
                case "and":
                case "_and":
                case "op&&":
                case "operator&&":
                    {
                    //    Value = _And(Arguments);
                        break;
                    }
                case ">":
                case "op>":
                case "operator>":
                case "mt":
                case "gt":
                case "greater":
                case "greaterthan":
                case "more":
                case "morethan":
                case "isgreater":
                case "isgreaterthan":
                case "ismore":
                case "ismorethan":
                case ">=":
                case "op>=":
                case "operator>=":
                case "mte":
                case "gte":
                case "greaterorequal":
                case "greaterthanorequal":
                case "moreorequal":
                case "morethanorequal":
                case "isgreaterorequal":
                case "isgreaterthanorequal":
                case "ismoreorequal":
                case "ismorethanorequal":
                case "<":
                case "op<":
                case "operator<":
                case "lt":
                case "less":
                case "lessthan":
                case "isless":
                case "islessthan":
                case "<=":
                case "op<=":
                case "operator<=":
                case "lte":
                case "lessorequal":
                case "lessthanorequal":
                case "islessorequal":
                case "islessthanorequal":
                case "<>":
                case "!=":
                case "op<>":
                case "op!=":
                case "operator<>":
                case "operator!=":
                case "ne":
                case "isnotequal":
                case "notequal":
                case "==":
                case "op==":
                case "operator==":
                case "isequal":
                case "equal":
                case "eq":
                    {
                        Value = _Compare(Name, Arguments);
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

        public override int Compare(ktString op, ktValue val)
        {
            int ret = 0;
            bool bVal = val.ToBool();

            switch (op)
            {
                case "<>":
                case "!=":
                case "op<>":
                case "op!=":
                case "operator<>":
                case "operator!=":
                case "ne":
                case "isnotequal":
                case "notequal":
                    {
                        ret = (m_value != bVal) ? 1 : 0;
                        break;
                    }
                case "==":
                case "op==":
                case "operator==":
                case "isequal":
                case "equal":
                case "eq":
                    {
                        ret = (m_value == bVal) ? 1 : 0;
                        break;
                    }
                default:
                    {
                        throw new ktError("Couldn't find the method '" +
                                          op + "' in class '" + m_Name + "'.", ktERR._404);
                    }
            }

            return ret;
        }
        public override int Compare(ktString op, ktList arguments)
        {
            if (arguments.Count == 1)
            {
                return Compare(op, (ktValue)arguments.FirstNode.Value);
            }
            else
            {
                throw new ktError("Compare for more than 1 value is not implemented in '" + this.m_Name + "'!");
            }
        }
        public ktValue _Compare(ktString Name, ktList Arguments)
        {
            int ret = Compare(Name, Arguments);

            return new ktValue("return", "ktInt", new ktInt(ret), false, true);
        }

        public ktValue _Not()
        {
            return new ktValue( "return", "ktBool", new ktBool(!m_value), true, true );
        }

        public override ktClass CreateObject(ktString Value)
        {
            return new ktBool(Value.AsLower() != "false");
        }
        public override ktClass CreateObject(ktValue Value)
        {
            return new ktBool(Value.ToBool());
        }
        public override ktClass CreateObject()
        {
            return new ktBool();
        }
        public override string ToString()
        {
            return m_value.ToString();
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }
        

        protected bool m_value;
    }
}
