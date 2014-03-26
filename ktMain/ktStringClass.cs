using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KacTalk;

namespace ktMainLib
{
    public class ktStringClass : ktClass
    {
        public ktStringClass(ktString Value)
            : base("ktString")
        {
            m_value = Value;
        }
        public ktStringClass(string Value)
            : base("ktString")
        {
            m_value = new ktString( Value );
        }
        public ktStringClass() : this(ktString.EmptyString) { }


        public override ktClass CreateObject(ktString Value)
        {
            return new ktStringClass(Value);
        }
        public override ktClass CreateObject(ktValue Value)
        {
            return new ktStringClass(Value.ToString());
        }
        public override ktClass CreateObject()
        {
            return new ktStringClass();
        }
        public override string ToString()
        {
            return m_value.ToString();
        }
        public override string Export(bool AsClass)
        {
            return ToString();
        }

        private  ktString m_value;
        private string ret;
    }
}
