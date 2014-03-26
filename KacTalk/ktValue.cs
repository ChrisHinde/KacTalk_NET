using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktValue : ktIntObj
    {
        public ktValue(ktString Name, ktString Type, ktIntObj Object,
                       bool HardType, bool Constant, ktClass CallbackClass)
            : base("ktValue", 0)
        {
            m_Object = Object;

            SetName(Name);
            SetType(Type);
            SetHardType(HardType);
            SetConstant(Constant);
            SetCallback(CallbackClass);
        }
        public ktValue(ktString Name, ktString Type, ktIntObj Object,
                       bool HardType, bool Constant) :
            this(Name, Type, Object, HardType, Constant, null)
        {/*
			SetName( Name );
			SetType( Type );
			SetHardType( HardType );
			SetConstant( Constant );
			SetCallback( CallbackClass );*/

          //  m_Object = Object;
        }
        public ktValue(ktValue Value)
            : base("ktValue", 0)
        {//ktDebug.Log( "Value::CONSTR( " + Value.Export() + " )" );
            SetName(Value.Name);
            SetType(Value.Type);
            SetHardType(Value.HardType);
            SetConstant(Value.Constant);

            m_Object = Value.Value;
        }

        #region properties
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ktString Type
        {
            get { return m_VType; }
            set { m_VType = value; }
        }
        public bool HardType
        {
            get { return m_HardType; }
            set { m_HardType = value; }
        }
        public bool Constant
        {
            get { return m_Constant; }
            set { m_Constant = value; }
        }

        public ktIntObj Value
        {
            get { return m_Object; }
            set { m_Object = value; }
        }

        public static ktValue Null
        {
            get { return new ktValue(":null", "null", null, true, true); }
        }
        #endregion

        public void SetName(ktString Name) { m_Name = Name; }
        public void SetType(ktString Type) { m_VType = Type; }
        public void SetHardType(bool HardType) { m_HardType = HardType; }
        public void SetHardType() { m_HardType = true; }
        public void SetConstant(bool Constant) { m_Constant = Constant; if (m_Object is ktClass) { ((ktClass)m_Object).IsConstant = Constant; } }
        public void SetConstant() { m_Constant = true; if (m_Object is ktClass) { ((ktClass)m_Object).IsConstant = true; } }
        public void SetCallback(ktClass Callback) { m_Callback = Callback; }
        public void SetCallback() { m_Callback = null; }
        public void SetValue(ktValue Value)
        {//ktDebug.Log( "ktValue::SetValue( " + Value.Export() + " )" );
            m_Name = Value.m_Name;
            m_VType = Value.Type;
            m_HardType = Value.HardType;
            m_Constant = Value.Constant;
            m_Object = Value.m_Object;
        }
        public void SetTheValue(ktValue Value)
        {//ktDebug.Log( "ktValue::SetTheValue( " + Value.Export() + " )" );
            /*if (Hard && !CheckType( Value )) {
                throw new ktError( "The 
            }*/
            if (!HardType)
            {
                m_VType = Value.Type;
            }
            m_Object = Value.m_Object;

            CallCallback("PropertyChanged");
        }

        public ktIntObj GetValue()
        {
            return m_Object;
        }

        /// <summary>
        ///  Checks the type of this value against the type of the value V
        /// </summary>
        /// <param name="V">Value</param>
        /// <returns></returns>
        public bool CheckType(ktValue V)
        {
            return V.Type.Compare(Type);
        }

        public bool IsNull()
        {
            return ((m_Name == ":null") || (m_VType == "null") && (m_Object == null));
        }

        public override ktIntObj Copy()
        {
            ktIntObj Obj = null;
            if (m_Object != null)
            {
                Obj = m_Object.Copy();
            }

            return new ktValue(Name.ToString(), Type.ToString(), Obj, HardType, Constant);
        }
        public ktValue CopyValue()
        {
            return (ktValue)Copy();
        }

        protected void CallCallback(ktString Method)
        {
            if (Method.IsEmpty() || (m_Callback == null))
            {
                return;
            }

            ktList Args = new ktList();
            Args.Add("Name", Name);
            Args.Add("Value", this);

            try
            {
                m_Callback.RunMethod("_" + Method, Args);
            }
            catch (ktError Err)
            {
                if ((Err.ErrorNumber != ktERR._404) && (Err.ErrorNumber != ktERR.NOTDEC))
                {
                    throw Err;
                }
            }
        }

        public override string Export()
        {
            ktString Str = new ktString();

            if (!m_Name.IsEmpty())
            {
                if (m_Constant)
                {
                    Str = "const ";
                }
                else if (m_HardType)
                {
                    Str = "hard ";
                }

                if (!m_VType.IsEmpty())
                {
                    Str += m_VType + " ";
                }

                Str += m_Name + " ";
            }

            if (m_Object != null)
            {
                if (!m_Name.IsEmpty())
                {
                    Str += "= ";
                }

                Str += m_Object.Export();
            }
            else
            {
                if (!m_Name.IsEmpty())
                {
                    Str += "= ";
                }

                Str += ":null";
            }

            return Str + ";";
        }

        public override string ToString()
        {
            if (m_Object != null)
            {
                return m_Object.ToString();
            }
            else
            {
                return ":null";
            }
        }

        public object ToType(ktString Type)
        {
            if (m_Object == null)
            {
                return null;
            }
#if Debug
	ktDebug.Log( "ktValue::ToType( " + m_Object.GetType().ToString() + " )" );
#endif
            try
            {
                ktClass Class = (ktClass)m_Object;

                ktValue v = Class.RunMethod("To" + Type);
                object o = v.ToType(Type);
                return o;
            }
            catch (Exception E)
            {
#if Debug
	ktDebug.Log( E.ToString() + ";;;;;;;" + E.StackTrace.ToString() );
#endif
            }

            if ((m_Object.GetType() == typeof(ktObjectWrapper)) ||
                (m_Object.GetStringType() == "ktObjectWrapper"))
            {
                return ((ktObjectWrapper)m_Object).ToType(Type);
            }

            switch (Type.AsLower())
            {
                case "bool":
                    {
                        if (m_Object.GetType() == typeof(ktString))
                        {
                            return ((ktString)m_Object).AsLower() != "false";
                        }
                        else if (m_Object.GetType() == typeof(ktList))
                        {
                            return !((ktList)m_Object).IsEmpty();
                        }
                        else
                        {
                            return m_Object.ToString().ToLower() != "false";
                        }
                    }
                case "int":
                case "integer":
                    {
                        try
                        {
                            if (m_Object.GetType() == typeof(ktString))
                            {
                                return ((ktString)m_Object).ToInt();
                            }
                            else
                            {
                                return Convert.ToInt32(m_Object.ToString());
                            }
                        }
                        catch (Exception Err)
                        {
                            if (Err.GetType() == typeof(System.FormatException))
                            {
                                throw new ktError("ktValue::ToType: Cant make '" + Value + "' into an integer", ktERR.WRONGTYPE);
                            }
                            else
                            {
                                throw Err;
                            }
                        }
                    }
                case "float":
                    {
                        try
                        {
                            if (m_Object.GetType() == typeof(ktString))
                            {
                                return ((ktString)m_Object).ToFloat();
                            }
                            else
                            {
                                return Convert.ToSingle(m_Object.ToString());
                            }
                        }
                        catch (Exception Err)
                        {
                            if (Err.GetType() == typeof(System.FormatException))
                            {
                                throw new ktError("ktValue::ToType: Cant make '" + Name + "' into an " + Type, ktERR.WRONGTYPE);
                            }
                            else
                            {
                                throw Err;
                            }
                        }
                    }
                case "double":
                    {
                        try
                        {
                            if (m_Object.GetType() == typeof(ktString))
                            {
                                return ((ktString)m_Object).ToDouble();
                            }
                            else
                            {
                                return Convert.ToDouble(m_Object.ToString());
                            }
                        }
                        catch (Exception Err)
                        {
                            if (Err.GetType() == typeof(System.FormatException))
                            {
                                throw new ktError("ktValue::ToType: Cant make '" + Name + "' into an " + Type, ktERR.WRONGTYPE);
                            }
                            else
                            {
                                throw Err;
                            }
                        }
                    }
                default:
                    {
                        return m_Object;
                    }

            }

            //(((ktClass)Value.Value).ToString().ToLower() == "true")
            //			return true;
        }
        public bool ToBool()
        {
            return (bool)ToType("bool");
        }
        public int ToInt()
        {
            return (int)ToType("int");
        }
        public float ToFloat()
        {
            return (float)ToType("float");
        }


        protected ktString m_Name;
        protected ktString m_VType;
        protected bool m_HardType;
        protected bool m_Constant;

        protected ktIntObj m_Object;

        protected ktClass m_Callback;
    }

}
