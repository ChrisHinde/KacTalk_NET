using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktRunStatement : ktIntObj
    {
        public ktRunStatement(ktList Statement, ktBlock Block)
            : base("ktRunStatement", 0)
        {
            m_Statement = Statement;
            m_Block = Block;
        }
        public ktRunStatement(ktList Statement)
            : this(Statement, new ktBlock())
        {
        }
        public ktRunStatement()
            : this(null, null)
        {
        }

        public bool SetVariable(ktString Name, ktValue Value, bool Add, bool Copy, bool IgnoreConstant)
        {
            // Nothing to use??
            if (Name.IsEmpty() || (Value == null) || (m_Block == null))
            {
                return false;
            }

            return m_Block.SetVariable(Name, Value, Add, Copy, IgnoreConstant);
        }
        public bool SetVariable(ktString Name, ktValue Value, bool Add, bool Copy)
        {
            return SetVariable(Name, Value, Add, Copy, false);
        }
        public bool SetVariable(ktString Name, ktValue Value, bool Copy)
        {
            return SetVariable(Name, Value, true, Copy);
        }
        public bool SetVariable(ktString Name, ktValue Value)
        {
            return SetVariable(Name, Value, true);
        }

        public ktValue Run()
        {
            ktValue Value = ktValue.Null;

            if (m_Block != null)
            {
                Value = m_Block.RunLine(m_Statement);
            }

            return Value;
        }

        public ktValue AsValue()
        {
            return new ktValue("RunStatement", "ktRunStatement", this, true, false);
        }

        protected ktList m_Statement;
        protected ktBlock m_Block;
    }


    public class ktRunStatementClass : ktClass
    {
        public ktRunStatementClass()
            : base("ktRunStatement", null, null, false, true)
        {
            m_Value = null;
        }
        public ktRunStatementClass(ktRunStatement Value)
            : this()
        {
            m_Value = Value;
            m_IsClass = false;
        }
        public ktRunStatementClass(ktValue Value)
            : this()
        {
            m_IsClass = false;
            SetProperty("_", Value);
        }

        public override ktClass CreateObject(ktString Value)
        {
            /*int I = 0;

            try {
                I = Value.ToInt();
            } catch (Exception E) {
                if (E.GetType() == typeof( System.FormatException )) {
                    throw new ktError( "kactalkClass::CreateObject: Cant make '" + Value + "' into an integer", ktERR.WRONGTYPE );
                }
            }*/

            return new ktRunStatementClass(new ktRunStatement());
        }
        public override ktClass CreateObject(ktValue Value)
        {/*
			int I = 0;

			try {
				I = Convert.ToInt32( Value.ToString() );
			} catch (Exception E) {
				if (E.GetType() == typeof( System.FormatException )) {
					throw new ktError( "kactalkClass::CreateObject: Cant make '" + Value + "' into an integer", ktERR.WRONGTYPE );
				}
			}*/

            return new ktRunStatementClass(new ktRunStatement());
        }
        public override ktClass CreateObject(object Object)
        {
            return new ktRunStatementClass(new ktRunStatement());
        }
        public override ktClass CreateClass()
        {
            return new ktRunStatementClass();
        }


        public override string ToString()
        {
            if (m_IsClass)
            {
                return "class ktRunStatement;";
            }
            else if (m_Value == null)
            {
                return ":null";
            }
            else
            {
                return m_Value.ToString();
            }
        }
        public override string Export(bool AsClass)
        {
            if (m_IsClass)
            {
                if (!AsClass)
                {
                    return "new ktFunction";
                }

                return "	class ktRunStatement : ktClass\n" +
                    "	{\n" +
                    "		public ktRunStatement() : base( \"ktRunStatement\" )\n" +
                    "		{\n" +
                    "			m_Value		= " + ((m_Value == null) ? ":null" : m_Value.ToString()) + ";\n" +
                    "			m_Name		= \"" + Name + "\";\n" +
                    "			m_Parent	= " + ((Parent == null) ? "null" : Parent.Export()) + ";\n" +
                    "			m_Properties	= " + ((Properties == null) ? "null" : Properties.Export()) + ";\n" +
                    "			m_Methods	= " + ((Methods == null) ? "null" : Methods.Export()) + ";\n" +
                    "			m_AddIfNotSet	= " + AddIfNotSet.ToString() + ";\n" +
                    "			m_IsClass	= " + IsClass.ToString() + ";\n" +
                    "		}\n" +
                    "		public ktRunStatement( ktValue Value ) : this()\n" +
                    "		{\n" +
                    "			m_Value = 0;\n" +
                    "		}\n\n" +
                    "		protected int m_Value;\n	}\n";
            }
            else
            {
                return m_Value.ToString();
            }
        }


        public override ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            if (Name.IsEmpty())
            {
                throw new ktError("Didn't get the name of the method to run in class '" +
                                m_Name + "'.", ktERR.NOTSET);
            }
            //ktDebug.Log( ";Name::"+ Name + ";;;;_\n" );
            if (Name == "_PropertyChanged")
            {
                if ((Arguments == null) || (Arguments.GetCount() != 2))
                {
                    throw new ktError("kactalk::_PropertyChanged() : Didn't get the two nnede arguments!",
                                      ktERR.MISSING);
                }
#if Debug
	ktDebug.Log( "Args::" + Arguments.Get_R( "\t", true ) );
#endif

                Name = Arguments.Get("Name").Node.Value.ToString();
                ktValue Value = (ktValue)Arguments.Get("Value").Node.Value;

                SetProperty(Name, Value);

                return ktValue.Null;
            }
            else /*if (Name.StartsWith( "operator", out Name )) {
				return HandleOperator( Name, Arguments );
			} else */
            {
                throw new ktError("Couldn't find the method '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }
        }

        public override ktValue AddProperty(ktValue Value)
        {
            return Value;
        }
        public override ktValue _GetProperty(ktString Name, bool Copy)
        {//ktDebug.Log( "kactalkclass::_GP( " + Name+ " )" );
            if ((Name == "this") || (Name == "_this") ||
                (Name == "object") || (Name == "_object") ||
                (Name == "_") || (Name.IsEmpty()))
            {
                return new ktValue(m_Name, "kactalk", this, true, false);
            }/*
			switch (Name) {
			}*/

            throw new ktError("Couldn't find the property '" +
                              Name + "' in class '" + m_Name + "'.", ktERR._404);
        }
        public override ktValue SetProperty(ktString Name, ktValue Value)
        {
            if ((Name == "this") || (Name == "_this") ||
                (Name == "object") || (Name == "_object") ||
                (Name == "_") || (Name.IsEmpty()))
            {
                /*try {
                    m_Value = Convert.ToInt32( Value.ToString() );
                } catch (Exception E) {
                    if (E.GetType() == typeof( System.FormatException )) {
                        throw new ktError( "kactalkClass::CreateObject: Cant make '" + Value + "' into an integer", ktERR.WRONGTYPE );
                    }
                }*/
            } /*else if (Name == "MathMode") { 
//				m_Value.MathMode = (((ktClass)Value.Value).ToString().ToLower() == "true");
				m_Value.MathMode = Value.ToBool();
			} */else
            {
                throw new ktError("Couldn't find the property '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            return GetProperty("_");
        }

        public override ktList GetMethods()
        {
            ktList List = new ktList();

            List.Add("Export", CreateObject("0"));

            return List;
        }
        public override ktList GetProperties()
        {
            ktValue This = new ktValue(m_Name, "int", this, true, false);
            ktList List = new ktList();

            List.Add("_", This);
            List.Add("this", This);
            List.Add("_this", This);
            List.Add("object", This);
            List.Add("_object", This);

            List.Add("MathMode", This);

            return List;
        }

        #region properties
        public ktRunStatement Value
        {
            get { return m_Value; }
        }
        #endregion

        protected ktRunStatement m_Value;
    }
}
