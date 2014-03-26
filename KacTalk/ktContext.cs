using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktContext : ktIntObj
    {
        public ktContext(ktString Name, ktString Aliases, ktList Classes,
            ktList Variables, ktList Functions, ktContext Parent,
            bool AddIfNotSet)
            : base("ktContext", 0)
        {
            m_Name = Name;
            m_Parent = Parent;
            m_Aliases = Aliases;
            m_Variables = Variables;
            m_Functions = Functions;
            m_AddIfNotSet = AddIfNotSet;
        }
        public ktContext(ktString Name)
            : base("ktContext", 0)
        {
            m_Name = Name;
            m_Parent = null;
            m_Aliases = null;
            m_Variables = null;
            m_Functions = null;
            m_AddIfNotSet = true;
        }

        public virtual ktList GetFunctions()
        {
            ktList Funcs = new ktList();
            Funcs.Node = new ktNode("Functions");

            //Funcs.Add( "echo" );

            return Funcs;
        }
        public virtual ktList GetClasses(bool JustNames)
        {
            ktList Classes = new ktList();
            Classes.Node = new ktNode("Classes");

            if (m_Classes == null)
            {
                return Classes;
            }

            m_Classes.Reset();
            foreach (ktList CL in m_Classes)
            {
                if (CL.Node == null)
                {
                    continue;
                }

                if (JustNames)
                {
                    Classes.Add(CL.Node.Name);
                }
                else if (CL.Node.Value != null)
                {
                    Classes.Add((ktClass)CL.Node.Value);
                }
            }

            return Classes;
        }
        public ktList GetClasses()
        {
            return GetClasses(true);
        }


        public virtual ktValue GetVariable(ktString Name, bool Copy)
        {
            ktValue Value = ktValue.Null;

            if (Name.IsEmpty())
            {
                return Value;
            }

            if (m_Variables == null)
            {
                throw new ktError("Couldn't find the variable  '" +
                                  Name + "' in context '" + m_Name + "'.", ktERR._404);
            }

            ktNode Node = m_Variables.GetNode(Name);
            if ((Node == null) || (Node.Value == null))
            {
                throw new ktError("Couldn't find the variable '" +
                                  Name + "' in context '" + m_Name + "'.", ktERR._404);
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
        public ktValue GetVariable(ktString Name)
        {
            return GetVariable(Name, false);
        }
        public virtual ktClass GetClass(ktString Name)
        {
            if (Name.IsEmpty() || (m_Classes == null))
                return null;

            ktNode Node = null;

            try
            {
                m_Classes.Reset();
                foreach (ktList l in m_Classes)
                {
                    //ktDebug.Log("Class[" + this.m_Name + "]: " + ((ktClass)(((ktNode)l.Node.Value).Value)).Name);
                    if (((ktClass)(((ktNode)l.Node.Value).Value)).Name == Name)
                    {
                        Node = (ktNode)l.Node.Value;
                        break;
                    }
                }
                //Node = m_Classes.GetNode(Name);
            }
            catch (ktError) { }

            if (Node == null)
            {
                if (m_Parent != null)
                {
                    return m_Parent.GetClass(Name);
                }/* else if (m_Main != null) {
					return m_Main.GetClass( Name );
				} */else
                {
                    throw new ktError("ktContext::GetClass() : There's no class with the name '" +
                                      Name + "'!", ktERR.NOTDEF);
                }
            }

            return (ktClass)Node.Value;
        }
        public virtual ktFunction GetFunction(ktString Name)
        {
            ktFunction Meth = null;
            ktNode Node = null;
            bool NotFound = false;

            if (Name.IsEmpty())
            {
                throw new ktError("ktContext::GetFunction() : Didn't get the name of the method to run.", ktERR.NOTSET);
            }

            if ((m_Functions == null) || (m_Functions.IsEmpty()))
            {
                NotFound = true;
            }
            else
            {
                Node = m_Functions.GetNode(Name);

                if (Node != null)
                {
                    Meth = (ktFunction)Node.Value;
                }
                else
                {
                    NotFound = true;
                }
            }

            if (NotFound)
            {
                if (m_Parent != null)
                {
                    Meth = m_Parent.GetFunction(Name);
                }
                else
                {
                    throw new ktError("ktContext::GetFunction() : Couldn't find the function '" +
                                      Name + "'.", ktERR.NOTFOUND);
                }
            }

            return Meth;
        }
        public virtual ktValue RunFunction(ktString Name, ktList Arguments)
        {
            ktValue Ret = ktValue.Null;
            ktNode Node = null;
            bool NotFound = false;

            if (Name.IsEmpty())
            {
                throw new ktError("ktBlock::RunFunction() : Didn't get the name of the method to run.", ktERR.NOTSET);
            }

            if (m_Functions == null)
            {
                NotFound = true;
            }
            else
            {
                Node = m_Functions.GetNode(Name);

                if (Node != null)
                {
                    ktFunction Meth = (ktFunction)Node.Value;

                    return Meth.Run(Arguments);
                }
                else
                {
                    NotFound = true;
                }
            }

            if (NotFound)
            {
                if (m_Parent != null)
                {
                    return m_Parent.RunFunction(Name, Arguments);
                }/* else if (m_Main != null) {
					return m_Main.RunFunction( Name, Arguments );
				}*/ else
                {
                    throw new ktError("ktContext::RunFunction() : Couldn't find the function '" +
                                      Name + ".", ktERR.NOTFOUND);
                }
            }

            return Ret;
        }
        public ktValue RunFunction(ktString Name)
        {
            return RunFunction(Name, null);
        }

        public virtual ktValue GetMember(ktString Name)
        {
            ktValue Value = ktValue.Null;

            try
            {
                ktFunction Meth = GetFunction(Name);

                Value = kacTalk.Main.MakeValueOf("ktFunction", Meth);
            }
            catch (ktError Err)
            {
                if (Err.ErrorNumber != ktERR._404)
                {
                    throw Err;
                }

                try
                {
                    Value = GetVariable(Name);
                }
                catch (ktError E)
                {
                    if (E.ErrorNumber == ktERR._404)
                    {
                        throw new ktError("ktContext::GetMember(): Couldn't find member '" +
                                            Name + "' in context " + m_Name + "!", ktERR._404);
                    }
                    else
                    {
                        throw E;
                    }
                }
            }

            return Value;
        }

        public virtual ktValue SetVariable(ktString Name, ktValue Value, bool Add)
        {
            if (Name.IsEmpty() || (Value == null))
            {
                return ktValue.Null; ;
            }

            ktValue Var = null;
            if ((Var = GetVariable(Name)) != null)
            {
                Var.SetValue(Value);
                return Var;
            }
            else if (Add)
            {
                Value.Name = Name;
                return AddVariable(Value);
            }
            else
            {
                return ktValue.Null; ;
            }
        }
        public ktValue SetVariable(ktString Name, ktValue Value)
        {
            return SetVariable(Name, Value, false);
        }
        public virtual ktValue AddVariable(ktString Name, ktValue Value)
        {
            if (Value == null)
            {
                throw new ktError("Need a value-object when adding variable '" +
                                Name + "' to context '" + m_Name + "'.", ktERR.NOTSET);
            }

            Value.SetName(Name);

            return AddVariable(Value);
        }
        public virtual ktValue AddVariable(ktValue Value)
        {
            if (Value == null)
            {
                throw new ktError("Need a value-object when adding a variable " +
                                      " to context '" + m_Name + "'.", ktERR.NOTSET);
            }

            if (m_Variables == null)
            {
                m_Variables = new ktList();
            }

            m_Variables.Add(Value.Name, Value);

            return Value;
        }

        public virtual bool AddClass(ktClass Class)
        {
            if (Class == null)
                return false;

            if (m_Classes == null)
                m_Classes = new ktList();

            ktNode Node = new ktNode(Class);
            Node.Name = Class.Name;

            return m_Classes.Add(Node);
        }

        public virtual bool AddFunction(ktFunction Function)
        {
            // No Function??
            if (Function == null)
            {
                return false;
            }

            // Create Function list if not set 
            if (m_Functions == null)
            {
                m_Functions = new ktList();
            }

            // Create a new node (with name and func), and add it
            return m_Functions.Add(Function.Name, Function);
        }

        public virtual bool CheckName(ktString N)
        {
            if (N.IsEmpty())
                return false;

            return ((m_Name == N) || (m_Aliases.Contains(":" + N + ":")));
        }

        public override string Export()
        {
            ktString Str = new ktString();

            Str = "context " + Name;
            if (m_Parent != null)
            {
                Str += " : " + m_Parent.Name;
            }
            Str += " {\n";

            if ((m_Variables != null) && (m_Variables.Count != 0))
            {
                ktValue Val = null;
#if Debug
				ktDebug.Log( "VARIABLES" );
				Str += "Variables:\n";
#endif
                m_Variables.Reset();
                foreach (ktList L in m_Variables)
                {
                    if ((L.Node == null) || (L.Node.Value == null))
                    {
                        continue;
                    }
                    Val = (ktValue)L.Node.Value;
                    Str += "\t" + Val.Export() + "\n";
                }
            }
            if ((m_Classes != null) && (m_Classes.Count != 0))
            {
                ktClass Class = null;
#if Debug
				Str += "Classes:\n";
#endif
                m_Classes.Reset();
                foreach (ktList L in m_Classes)
                {
                    if ((L.Node == null) || (L.Node.Value == null))
                    {
                        Str += "NOCLASS:" + L.Node.Name + "\n";
                        continue;
                    }
                    try
                    {
                        try
                        {
                            Class = (ktClass)(((ktNode)L.Node.Value).Value);//(ktClass)L.Node.Value;
                            Str += "\t" + Class.Export() + "\n";
                        }
                        catch (ktError Err)
                        {
                            ktDebug.Log("ERR:" + Err.ToString());
                        }
                    }
                    catch (Exception Err)
                    {
                        ktDebug.Log("ERR:" + Err.ToString());
                    }
                }
            }
            if ((m_Functions != null) && (m_Functions.Count != 0))
            {
                ktFunction Func = null;
#if Debug
				ktDebug.Log( "FUNCCTION" );
				Str += "Functions:\n";
#endif
                m_Functions.Reset();
                foreach (ktList L in m_Functions)
                {
                    if ((L.Node == null) || (L.Node.Value == null))
                    {
                        continue;
                    }
                    Func = (ktFunction)L.Node.Value;
                    Str += "\t" + Func.Export() + "\n";
                }
            }

            Str += "}\n";

            return Str;
        }

        public override string ToString()
        {
            return Export();
        }


        #region properties
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ktString Aliases
        {
            get { return m_Aliases; }
            set { m_Aliases = value; }
        }
        public ktContext Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }
        public ktList Variables
        {
            get { return m_Variables; }
            set { m_Variables = value; }
        }
        public ktList Classes
        {
            get { return m_Classes; }
            set { m_Classes = value; }
        }
        public ktList Functions
        {
            get { return m_Functions; }
            set { m_Functions = value; }
        }
        public bool AddIfNotSet
        {
            get { return m_AddIfNotSet; }
            set { m_AddIfNotSet = value; }
        }
        #endregion

        protected ktString m_Name;
        protected ktString m_Aliases;
        protected ktContext m_Parent;
        protected ktList m_Variables;
        protected ktList m_Classes;
        protected ktList m_Functions;

        protected bool m_AddIfNotSet;
    }
}
