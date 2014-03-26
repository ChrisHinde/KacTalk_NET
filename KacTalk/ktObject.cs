using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktIntObj
    {
        internal ktIntObj(string Type, int iType, bool ABKT)
        {
            m_Type = Type;
            m_iType = iType;

            m_ErrNum = ktERR.NOERROR;
            m_Copy = false;
            m_AddedByKT = ABKT;
        }
        public ktIntObj(string Type, int iType)
            : this(Type, iType, false)
        {
        }
        public ktIntObj()
            : this("", 0)
        {
        }

        public string GetError()
        {
            return m_Err;
        }
        public ktERR GetErrorNum()
        {
            return m_ErrNum;
        }

        public bool HasError()
        {
            return m_ErrNum != ktERR.NOERROR;
        }

        public bool AllowCopy(bool Copy)
        {
            return m_Copy = Copy;
        }
        public bool CopyingAllowed()
        {
            return m_Copy;
        }

        public virtual ktIntObj Copy()
        {
            return new ktIntObj(m_Type, m_iType);
        }

        public int GetObjType()
        {
            return m_iType;
        }
        public int GetIntType()
        {
            return m_iType;
        }
        public string GetStringType()
        {
            return m_Type;
        }
        public virtual string Export()
        {
            return "#" + m_Type;
        }


        protected void SetError(string Err, ktERR ErrNum)
        {
            m_Err = Err;
            m_ErrNum = ErrNum;
        }
        protected void SetError(string Err)
        {
            m_Err = Err;
            m_ErrNum = ktERR.ERROR;
        }

        #region properties
        public bool AddedByKT
        {
            get { return m_AddedByKT; }
            set { m_AddedByKT = value; }
        }
        #endregion

        protected string m_Type;
        protected int m_iType;

        protected string m_Err;
        protected ktERR m_ErrNum;
        protected bool m_Copy;

        protected bool m_AddedByKT;
    }


    public class ktObjectWrapper : ktIntObj
    {
        public ktObjectWrapper(object Object)
            : base("ktObjectWrapper", 0)
        {
            m_Object = Object;
        }
        public ktObjectWrapper()
            : this(null)
        {
        }


        public object ToType(ktString Type)
        {
            if (m_Object == null)
            {
                return null;
            }

            try
            {
                ktClass Class = (ktClass)m_Object;

                return Class.RunMethod("To" + Type).ToType(Type);
            }
            catch (Exception)
            {
            }

            switch (Type.AsLower())
            {
                case "bool":
                    {
                        if (m_Object.GetType() == typeof(bool))
                        {
                            return m_Object;
                        }
                        else if (m_Object.GetType() == typeof(int))
                        {
                            return ((int)m_Object) != 0;
                        }
                        else if (m_Object.GetType() == typeof(float))
                        {
                            return ((float)m_Object) != 0.0f;
                        }
                        else if (m_Object.GetType() == typeof(double))
                        {
                            return ((double)m_Object) != 0.0;
                        }
                        else if (m_Object.GetType() == typeof(char))
                        {
                            return ((char)m_Object) != 0;
                        }
                        else if (m_Object.GetType() == typeof(string))
                        {
                            return ((string)m_Object) != "false";
                        }
                        else if (m_Object.GetType() == typeof(ktString))
                        {
                            return ((ktString)m_Object) != "false";
                        }
                        else if (m_Object.GetType() == typeof(ktList))
                        {
                            return !((ktList)m_Object).IsEmpty();
                        }
                        else
                        {
                            return m_Object.ToString() != "false";
                        }
                    }
                case "int":
                case "integer":
                    {
                        try
                        {
                            if (m_Object.GetType() == typeof(bool))
                            {
                                return (((bool)m_Object) ? 1 : 0);
                            }
                            else if (m_Object.GetType() == typeof(int))
                            {
                                return m_Object;
                            }
                            else if (m_Object.GetType() == typeof(float))
                            {
                                return (int)((float)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(double))
                            {
                                return (int)((double)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(char))
                            {
                                return (int)((char)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(string))
                            {
                                return Convert.ToInt32(((string)m_Object));
                            }
                            else if (m_Object.GetType() == typeof(ktString))
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
                                throw new ktError("ktObjectWrapper::ToType: Cant make '" + Value.ToString() + "' into an integer", ktERR.WRONGTYPE);
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
                            if (m_Object.GetType() == typeof(bool))
                            {
                                return (((bool)m_Object) ? 1.0f : 0.0f);
                            }
                            else if (m_Object.GetType() == typeof(int))
                            {
                                return (float)((int)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(float))
                            {
                                return m_Object;
                            }
                            else if (m_Object.GetType() == typeof(double))
                            {
                                return (float)((double)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(char))
                            {
                                return (float)((char)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(string))
                            {
                                return Convert.ToSingle(((string)m_Object));
                            }
                            else if (m_Object.GetType() == typeof(ktString))
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
                                throw new ktError("ktObjectWrapper::ToType: Cant make '" + m_Object.ToString() + "' into an " + Type, ktERR.WRONGTYPE);
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
                            if (m_Object.GetType() == typeof(bool))
                            {
                                return (((bool)m_Object) ? 1.0 : 0.0);
                            }
                            else if (m_Object.GetType() == typeof(int))
                            {
                                return (double)((int)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(float))
                            {
                                return (double)((float)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(double))
                            {
                                return m_Object;
                            }
                            else if (m_Object.GetType() == typeof(char))
                            {
                                return (double)((char)m_Object);
                            }
                            else if (m_Object.GetType() == typeof(string))
                            {
                                return Convert.ToDouble(((string)m_Object));
                            }
                            else if (m_Object.GetType() == typeof(ktString))
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
                                throw new ktError("ktValue::ToType: Cant make '" + m_Object.ToString() + "' into an " + Type, ktERR.WRONGTYPE);
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
            //			return null;
        }

        #region properties
        public object Object
        {
            get { return m_Object; }
            set { m_Object = value; }
        }
        public object Value
        {
            get { return m_Object; }
            set { m_Object = value; }
        }
        #endregion

        protected object m_Object;
    }
}
