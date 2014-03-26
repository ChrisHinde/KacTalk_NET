using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    /// <summary>
    /// kactalkClass is a wrapper for the main/mother(ship) class: kacTalk. 
    /// </summary>
    public class kactalkClass : ktClass
    {
        /// <summary>
        /// Default constructor for kactalkClass
        /// </summary>
        public kactalkClass()
            : base("kactalk", null, null, false, true)
        {
            m_Value = null;
        }
        /// <summary>
        /// Main constructor for kactalkClass,
        /// it takes the kacTalk object/interpreter it should wrap/represent
        /// </summary>
        public kactalkClass(kacTalk Value)
            : this()
        {
            m_Value = Value;
            m_IsClass = false;
        }
        /// <summary>
        /// Main constructor for kactalkClass,
        /// it takes the kacTalk object/interpreter it should wrap/represent (as a ktValue)
        /// </summary>
        public kactalkClass(ktValue Value)
            : this()
        {
            m_IsClass = false;
            SetProperty("_", Value);
        }

        /// <summary>
        /// Creates a new object of this class (using a new intepreter)
        /// </summary>
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

            return new kactalkClass(new kacTalk());
        }
        /// <summary>
        /// Creates a new object of this class (using a new intepreter)
        /// </summary>
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

            return new kactalkClass(new kacTalk());
        }
        /// <summary>
        /// Creates a new object of this class (using a new intepreter)
        /// </summary>
        public override ktClass CreateObject(object Object)
        {
            return new kactalkClass(new kacTalk());
        }
        /// <summary>
        /// Creates a new object of this class (using a new intepreter)
        /// </summary>
        public override ktClass CreateClass()
        {
            return new kactalkClass();
        }


        /// <summary>
        /// Creates a string that should be able to be used as o "representation" of this class/object
        /// </summary>
        public override string ToString()
        {
            if (m_IsClass)
            {
                return "class kactalkClass;";
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
        /// <summary>
        /// Export this object/class as a string
        /// </summary>
        public override string Export(bool AsClass)
        {
            if (m_IsClass)
            {
                if (!AsClass)
                {
                    return "new kactalk";
                }

                return "	class kactalkClass : ktClass\n" +
                    "	{\n" +
                    "		public kactalkClass() : base( \"kactalkClass\" )\n" +
                    "		{\n" +
                    "			m_Value		= " + ((m_Value == null) ? ":null" : m_Value.ToString()) + ";\n" +
                    "			m_Name		= \"" + Name + "\";\n" +
                    "			m_Parent	= " + ((Parent == null) ? "null" : Parent.Export()) + ";\n" +
                    "			m_Properties	= " + ((Properties == null) ? "null" : Properties.Export()) + ";\n" +
                    "			m_Methods	= " + ((Methods == null) ? "null" : Methods.Export()) + ";\n" +
                    "			m_AddIfNotSet	= " + AddIfNotSet.ToString() + ";\n" +
                    "			m_IsClass	= " + IsClass.ToString() + ";\n" +
                    "		}\n" +
                    "		public kactalkClass( kactalk Value ) : this()\n" +
                    "		{\n" +
                    "			m_Value = Value;\n" +
                    "		}\n" +
                    "		public kactalkClass( ktValue Value ) : this()\n" +
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


        /// <summary>
        /// "Find" and run the method 'Name', with arguments supplied
        /// </summary>
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
                    throw new ktError("kactalk::_PropertyChanged() : Didn't get the two needed arguments!",
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

        /// <summary>
        /// Add a property/variable to the class, does nothing as this class is "complete"
        /// </summary>
        public override ktValue AddProperty(ktValue Value)
        {
            return Value;
        }
        /// <summary>
        /// Returns a property of this class, matching 'Name'.
        /// </summary>
        public override ktValue _GetProperty(ktString Name, bool Copy)
        {//ktDebug.Log( "kactalkclass::_GP( " + Name+ " )" );
            if ((Name == "this") || (Name == "_this") ||
                (Name == "object") || (Name == "_object") ||
                (Name == "_") || (Name.IsEmpty()))
            {
                return new ktValue(m_Name, "kactalk", this, true, false);
            }
            switch (Name)
            {
                case "MathMode":
                    {
                        return new ktValue(Name, "bool", m_Value.MakeObjectOf("ktBool", m_Value.MathMode), true, false, this);
                    }
            }

            throw new ktError("Couldn't find the property '" +
                              Name + "' in class '" + m_Name + "'.", ktERR._404);
        }
        /// <summary>
        /// Sets a property of this class, matching 'Name', with the value provided (by 'Value')
        /// </summary>
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
            }
            else if (Name == "MathMode")
            {
                //				m_Value.MathMode = (((ktClass)Value.Value).ToString().ToLower() == "true");
                m_Value.MathMode = Value.ToBool();
            }
            else
            {
                throw new ktError("Couldn't find the property '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            return GetProperty("_");
        }

        /// <summary>
        /// Returns a list of available methods...
        /// </summary>
        public override ktList GetMethods()
        {
            ktList List = new ktList();

            List.Add("Export", CreateObject("0"));

            return List;
        }
        /// <summary>
        /// Returns a list of available properties...
        /// </summary>
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
        /// <summary>
        /// Returns the "internal" "value" (i.e. the value of the object: a kacTalk object)
        /// </summary>
        public kacTalk Value
        {
            get { return m_Value; }
        }
        #endregion

        /// <summary>
        /// Holds the kacTalk object/interpreter that this class/object represents
        /// </summary>
        protected kacTalk m_Value;
    }
}
