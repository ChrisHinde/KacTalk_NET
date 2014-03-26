using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KacTalk
{
    public class ktModule : ktIntObj
    {
        public ktModule(ktString Name, ktString Filename, bool Load)
            : base("ktIntObj", 0)
        {
            m_Name = Name;
            m_Filename = Filename;

            if (Load)
            {
            }
        }
        public ktModule(ktString Name, ktString Filename)
            : this(Name, Filename, true)
        {
        }
        public ktModule(ktString Filename)
            : this("", Filename, true)
        {
        }
        public ktModule(ktModule Module)
            : this("", "", false)
        {
        }

        public bool AddContext(ktContext Context)
        {
            if (Context == null)
            {
                return false;
            }

            if (m_Contexts == null)
            {
                m_Contexts = new ktList();
            }

            return m_Contexts.Add(Context.Name, Context);
        }
        public virtual ktContext GetContext(ktString Name)
        {
            if (Name.IsEmpty())
            {
                throw new ktError("Didn't get the name of the context to get in the module '" +
                                  m_Name + "'.", ktERR.NOTSET);
            }

            if (m_Contexts == null)
            {
                throw new ktError("Couldn't find the context '" + Name + "', in module '" +
                                  m_Name + "'.", ktERR.NOTFOUND);
            }

            ktNode Node = null;

            try
            {/*
				Node = m_Contexts.GetNode( Name );
				Node.HasError(); // Error Trigger??*/

                ktContext Con = null;

                m_Contexts.Reset();
                foreach (ktList CL in m_Contexts)
                {
                    if ((CL == null) || (CL.Node == null) || (CL.Node.Value == null) ||
                            (CL.Node.Value.GetType() == typeof(ktContext)))
                    {
                        continue;
                    }
                    Con = (ktContext)CL.Node.Value;

                    if ((Con.Name == Name) || (Con.Aliases.Contains(":" + Name + ":")))
                    {
                        return Con;
                    }
                }
                Node.HasError(); // Error Trigger??
            }
            catch (Exception)
            {
                throw new ktError("Couldn't find the context '" + Name + "', in module '" +
                                  m_Name + "'.", ktERR.NOTFOUND);
            }

            return (ktContext)Node.Value;
        }
        public virtual ktList GetContexts(bool JustNames)
        {
            ktList Contexts = new ktList();
            Contexts.Node = new ktNode("Contexts of module  '" + m_Name + "'");

            if (m_Contexts == null)
            {
                return Contexts;
            }

            m_Contexts.Reset();
            foreach (ktList CL in m_Contexts)
            {
                if (CL.Node == null)
                {
                    continue;
                }

                if (JustNames)
                {
                    Contexts.Add(CL.Node.Name);
                }
                else if (CL.Node.Value != null)
                {
                    Contexts.Add((ktContext)CL.Node.Value);
                }
            }

            return Contexts;
        }
        public ktList GetContexts() { return GetContexts(true); }

        public override string ToString()
        {
            return "#ktModule( " + Name + " )";
        }
        public override string Export()
        {
            ktString Str = GetContexts(false).Get_R();
            /*Str.Replace( "\n", "#ktCONTEXTEXPORTBROKENREPLACENEWLINE#\t\t", true );
            Str.Replace( "#ktCONTEXTEXPORTBROKENREPLACENEWLINE#", "\n", true );*/

            return "module " + Name + " {\n\t\t" + Str + " }\n";
        }
        public virtual void DoSomething()
        {
            ktDebug.Log("Not Doing something!!");
        }
        #region properties
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ktString Filename
        {
            get { return m_Filename; }
            set { m_Filename = value; }
        }
        #endregion

        protected ktString m_Name;
        protected ktString m_Filename;

        protected ktList m_Contexts;
    }
}
