using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktClass : ktIntObj
    {
        public ktClass(ktString Name, ktClass Parent, ktList Properties, ktList Methods,
                       bool AddIfNotSet, bool IsClass_, bool HardType = false, bool IsConstant = false)
            : base("ktClass", 0)
        {
            m_Name = Name;
            m_Parent = Parent;
            m_Properties = Properties;
            m_Methods = Methods;
            m_AddIfNotSet = AddIfNotSet;
            m_IsClass = IsClass_;
            m_HardType = HardType;
            m_IsConstant = IsConstant;
        }
        public ktClass(ktString Name, ktList Properties, ktList Methods, bool AddIfNotSet,
                       bool IsClass_)
            : this(Name, null, Properties, Methods, AddIfNotSet, IsClass_)
        {
        }
        public ktClass(ktClass Parent)
            : this("", Parent, null, null, Parent.AddIfNotSet, false)
        {
        }
        public ktClass(ktString Name)
            : this(Name, null, null, null, true, false)
        {
        }

        public virtual ktClass CreateObject(ktString Value) { return null; }
        public virtual ktClass CreateObject(ktValue Value) { return CreateObject(Value.ToString()); }
        public virtual ktClass CreateObject(object Obj) { return CreateObject(Obj.ToString()); }
        public virtual ktClass CreateObject() { return CreateObject(""); }
        public virtual ktClass CreateClass()
        {
            ktDebug.Log("ktClass::CreateClass()");
            return new ktClass(this.Name, this.Parent, this.Properties, this.Methods,
                                    this.AddIfNotSet, true);
        }

        public override ktIntObj Copy()
        {
            if (m_IsClass)
            {
                return CreateClass();
            }
            else
            {
                return CreateObject(this);
            }
        }


        public ktValue GetProperty(ktString Name, bool Copy)
        {
            ktValue Value = ktValue.Null;

            if (Name.IsEmpty())
            {
                return Value;
            }
            //ktDebug.Log( "GetProperty( " + Name + " )" );
            switch (Name.AsLower())
            {
                case "as_class":
                case "class":
                    {
                        return new ktValue(Name, "ktClass", CreateClass(), true, true);
                    }
                default:
                    {
                        //ktDebug.Log( "Default" );
                        Value = _GetProperty(Name, Copy);
                        break;
                    }
            }

            //ktDebug.Log( "Val =  " + ((Value == null) ? "NULL" : Value.ToString() ) + " )" );

            return Value;
        }

        public virtual ktValue _GetProperty(ktString Name, bool Copy)
        {
            ktValue Value = ktValue.Null;

            if (Name.IsEmpty())
            {
                return Value;
            }

            if (m_Properties == null)
            {
                throw new ktError("Couldn't find the property '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            ktNode Node = m_Properties.GetNode(Name);
            if ((Node == null) || (Node.Value == null))
            {
                throw new ktError("Couldn't find the property '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
                //return Value;
            }

            if (Copy)
            {
                Value = new ktValue((ktValue)Node.Value);
            }
            else
            {
                Value = (ktValue)Node.Value;
            }

            return Value;
        }
        public ktValue GetProperty(ktString Name) { return GetProperty(Name, false); }

        public virtual ktList GetProperties()
        {
            ktList Props = new ktList();

            if (m_Properties == null)
            {
                return Props;
            }

            m_Properties.Reset();
            foreach (ktList PL in m_Properties)
            {
                if ((PL.Node == null) || (PL.Node.Value == null))
                {
                    continue;
                }

                Props.Add(PL.Node.Name, ((ktValue)PL.Node.Value).Type);
            }

            return Props;
        }

        public virtual ktValue AddProperty(ktString Name, ktValue Value)
        {
            if (Value == null)
            {
                throw new ktError("Need a value-object when adding property '" +
                                Name + "' to class '" + m_Name + "'.", ktERR.NOTSET);
            }

            Value.SetName(Name);

            return AddProperty(Value);
        }
        public virtual ktValue AddProperty(ktValue Value)
        {
            if (Value == null)
            {
                throw new ktError("Need a value-object when adding a property " +
                                      " to class '" + m_Name + "'.", ktERR.NOTSET);
            }

            if (m_Properties == null)
            {
                m_Properties = new ktList();
            }

            m_Properties.Add(Value.Name, Value);

            return Value;
        }
        public virtual ktValue SetProperty(ktString Name, ktValue Value)
        {
            if (Name.IsEmpty())
            {
                throw new ktError("Didn't get the name of the property to change in class '" +
                                m_Name + "'.", ktERR.NOTSET);
            }

            if (m_Properties == null)
            {
                m_Properties = new ktList();
            }

            ktNode Node = m_Properties.GetNode(Name);

            if ((Node == null) || (Node.Value == null))
            {
                if (m_AddIfNotSet)
                {
                    return AddProperty(Name, Value);
                }
                else
                {
                    throw new ktError("Couldn't find the property '" +
                                      Name + "' in class '" + m_Name + "'.", ktERR._404);
                }
            }
            else
            {
                ktValue Prop = (ktValue)Node.Value;

                if (Prop.Constant)
                {
                    throw new ktError("The property '" + Name + "' in class '" +
                            m_Name + "' is a constant and can't be changed.", ktERR.CONST);
                }
                else if (Value == null)
                {
                    // HUM???
                    Prop.Value = null;
                }
                else
                {
                    Prop.SetValue(Value);
                }

                return Prop;
            }
        }

        public virtual ktFunction AddMethod(ktFunction Method)
        {
            if (Method == null)
            {
                return null;
            }

            if (m_Methods == null)
            {
                m_Methods = new ktList();
            }

            m_Methods.Add(Method);

            return Method;
        }

        public virtual ktFunction GetMethod(ktString Name)
        {
            if (Name.IsEmpty())
            {
                throw new ktError("Didn't get the name of the method to find in class '" +
                                m_Name + "'.", ktERR.NOTSET);
            }

            if (m_Methods == null)
            {
                throw new ktError("Couldn't find the method '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            ktNode Node = m_Methods.GetNode(Name);
            if ((Node == null) || (Node.Value == null))
            {
                throw new ktError("Couldn't find the method '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            ktFunction Func = (ktFunction)Node.Value;

            return Func;
        }
        public ktValue RunMethod(ktString Name, ktList Arguments)
        {
            if (Name.IsEmpty())
            {
                throw new ktError("Didn't get the name of the method to run in class '" +
                                m_Name + "'.", ktERR.NOTSET);
            }
            ktDebug.Log("RunM Arg:" + Arguments.Get_R());

            ktValue Value = ktValue.Null;
            //ktDebug.Log( "ktClass::RM( " + Name + " );" );
            try
            {
                Value = _RunMethod(Name, Arguments);
            }
            catch (Exception Err)
            {
                Value = (ktValue)Arguments.First.Node.Value;
                if (Name == "Export")
                {
                    return new ktValue("", "ktString", kacTalk.Main.MakeObjectOf("ktString", Export()), true, true);
                }
                else if (Name.AsLower() == "tostring")
                {
                    return new ktValue("", "ktString", kacTalk.Main.MakeObjectOf("ktString", ToString()), true, true);
                }
                else if (Name.AsLower() == "operator~")
                {
                    return new ktValue("", "ktString", kacTalk.Main.MakeObjectOf("ktString", ToString() + Value.ToString()), true, true);
                }
                else if (Name.AsLower().StartsWith("to", out Name))
                {
                }

                throw Err;
            }

            return Value;
        }
        public virtual ktValue _RunMethod(ktString Name, ktList Arguments)
        {
            //ktDebug.Log( "ktClass::_RM( " + Name + " );" );
            if (Name.IsEmpty())
            {
                throw new ktError("Didn't get the name of the method to run in class '" +
                                m_Name + "'.", ktERR.NOTSET);
            }

            if (m_Methods == null)
            {
                throw new ktError("Couldn't find the method '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            ktNode Node = m_Methods.GetNode(Name);
            if ((Node == null) || (Node.Value == null))
            {
                throw new ktError("Couldn't find the method '" +
                                  Name + "' in class '" + m_Name + "'.", ktERR._404);
            }

            ktValue Value = ktValue.Null;
            ktFunction Func = (ktFunction)Node.Value;

            Value = Func.Run(Arguments);

            return Value;
        }
        public ktValue RunMethod(ktString Name) { return RunMethod(Name, null); }

        public virtual ktList GetMethods()
        {
            ktList Methods = new ktList();

            if (m_Methods == null)
            {
                return Methods;
            }

            m_Methods.Reset();
            foreach (ktList ML in m_Methods)
            {
                if ((ML.Node == null) || (ML.Node.Value == null))
                {
                    continue;
                }

                Methods.Add(ML.Node.Name, ((ktFunction)ML.Node.Value).Return);
            }

            Methods.Add("Export", CreateObject());

            return Methods;
        }

        public virtual ktValue GetMember(ktString Name)
        {
            ktValue Value = ktValue.Null;

            try
            {
                Value = GetProperty(Name);
                //ktDebug.Log( "GM::P V:" + Value.ToString() + ";" );
                if (!Value.IsNull())
                {
                    return Value;
                }
            }
            catch (Exception)
            {
                //ktDebug.Log( "Err:" + E.ToString() + ";" );
            }
            try
            {
                ktFunction Func = GetMethod(Name);

                Value = new ktValue(Name, "ktFunction", Func, true, true);

                if (!Value.IsNull())
                {
                    return Value;
                }
            }
            catch (Exception)
            {
                if (Name == "Export")
                {
                    ktDelegateFunction Func = new ktDelegateFunction("Export", ExportValue);

                    Value = new ktValue(Name, "ktFunction", Func, true, true);

                    if (!Value.IsNull())
                    {
                        return Value;
                    }
                }
                else if (Name.AsLower() == "tostring")
                {
                    ktDelegateFunction Func = new ktDelegateFunction("ToString", ToStringValue);

                    Value = new ktValue(Name, "ktFunction", Func, true, true);

                    if (!Value.IsNull())
                    {
                        return Value;
                    }
                }
            }

            throw new ktError("Can't find the member '" + Name + "' in class '" + m_Name + "'!", ktERR._404);
        }

        public virtual int Compare(ktString op, ktValue val)
        {
            throw new ktError("Compare not implemented in '" + this.m_Name + "'!");
        }
        public virtual int  Compare(ktString op, ktList arguments)
        {
            throw new ktError("Compare not implemented in '" + this.m_Name + "'!");
        }

        /// <summary>
        /// General method for stopping messing with constants!
        /// </summary>
        public void CheckIfConstant()
        {
            if (m_IsConstant)
            {
                throw new ktError("Can't change the value of a constant in '" + Name + "'!", ktERR.CONST);
            }
        }
        /// <summary>
        /// General method for stopping messing with constants!
        /// </summary>
        /// <param name="Method">The method that was called</param>
        public void CheckIfConstant( ktString Method )
        {
            if (m_IsConstant)
            {
                throw new ktError("Can't change the value of a constant with " + Method + " in '" + Name + "'!", ktERR.CONST);
            }
        }

        public virtual string Export(bool AsClass)
        {
            return "NEED CLASS EXPORT!!!!!!!";
        }
        public override string Export()
        {
            return Export(false);
        }

        public ktValue ExportValue()
        {
            return new ktValue("", "ktString", kacTalk.Main.MakeObjectOf("ktString", Export()), true, true);
        }

        public override string ToString()
        {
            return "NEED CLASS TO STRING";
        }

        public ktValue ToStringValue()
        {
            return new ktValue("", "ktString", kacTalk.Main.MakeObjectOf("ktString", ToString()), true, true);
        }

        #region properties
        public ktClass Parent
        {
            get { return m_Parent; }
        }
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public virtual ktList Properties
        {
            get { return m_Properties; }
        }
        public virtual ktList Methods
        {
            get { return m_Methods; }
        }
        public bool AddIfNotSet
        {
            get { return m_AddIfNotSet; }
            set { m_AddIfNotSet = value; }
        }
        public bool IsClass
        {
            get { return m_IsClass; }
            set { m_IsClass = value; }
        }
        public bool IsConstant
        {
            get { return m_IsConstant; }
            set { m_IsConstant = value; }
        }
        public bool HardType
        {
            get { return m_HardType; }
            set { m_HardType = value; }
        }
        #endregion

        protected ktList m_Properties;
        protected ktList m_Methods;
        protected ktString m_Name;
        protected ktClass m_Parent;
        protected bool m_AddIfNotSet;
        protected bool m_IsClass;
        protected bool m_HardType;
        protected bool m_IsConstant;
    }
}
