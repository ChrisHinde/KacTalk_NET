using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public delegate ktValue ktFunction_Delegate(ktList Arguments);
    public delegate ktValue ktFunction_NoArg_Delegate();
    public delegate double ktFunction_Double_Delegate(double DoubleArgument);

    public class ktFunction : ktIntObj
    {
        public ktFunction(ktString Name, ktList Arguments, ktBlock Block, ktValue Ret) :
            base("ktFunction", 0)
        {
            SetProperties(Name, Arguments, Block, Ret);
        }
        public ktFunction(ktString Name) :
            base("ktFunction", 0)
        {
            SetName(Name);
        }

        public virtual bool SetProperties(ktString Name, ktList Arguments, ktBlock Block, ktValue Ret)
        {
            if (!SetName(Name))
                return false;
            if (!SetArguments(Arguments))
                return false;
            if (!SetBlock(Block))
                return false;
            if (!SetReturn(Ret))
                return false;

            return true;
        }

        public bool SetName(ktString Name)
        {
            if (Name.IsEmpty())
            {
                return false;
            }

            m_Name = Name;

            return true;
        }

        public bool SetArguments(ktList Arguments)
        {
            if (m_Arguments != null)
            {
                m_Arguments.Clear();
                m_Arguments = null;
            }

            if (Arguments == null)
            {
                return true;
            }

            /*if (Arguments.CopyingAllowed())
                m_Arguments = new ktList( Arguments );
            else*/
            m_Arguments = Arguments;

            if (m_Arguments == null)
                return false;
            else
                return true;
        }

        public bool SetBlock(ktBlock Block)
        {
            if (m_Block != null)
            {
                m_Block = null;
            }

            if (Block == null)
            {
                return true;
            }

            /*if (Block->CopyingAllowed())
                m_Block = new ktBlock( Block );
            else*/
            m_Block = Block;

            if (m_Block == null)
                return false;
            else
                return true;
        }

        public bool SetReturn(ktValue Ret)
        {
            if (m_Return != null)
            {
                m_Return = null;
            }

            if (Ret == null)
            {
                return true;
            }

            /*if (Ret->CopyingAllowed())
                m_Return = new ktValue( Ret );
            else*/
            m_Return = Ret;

            if (m_Return == null)
                return false;
            else
                return true;
        }


        public virtual ktValue Run(ktList Arguments)
        {
            ktValue Ret = ktValue.Null;

            MapArguments(Arguments);

            ktDebug.Log(m_Arguments.Get_R());
            ktDebug.Log(m_Block.Variables.Get_R());

            Ret = m_Block.Run();

            return Ret;
        }

        public virtual bool AddVariable(ktValue Variable)
        {
            if (m_Block == null)
                return false;

            return m_Block.AddVariable(Variable);
        }
        public virtual ktValue GetVariable(ktString Name)
        {
            if (m_Block == null)
            {
                return ktValue.Null;
            }

            return m_Block.GetVariable(Name);
        }

        protected bool MapArguments(ktList Arguments)
        {
            if (((Arguments == null) || Arguments.IsEmpty()) &&
                 ((m_Arguments == null) || m_Arguments.IsEmpty()))
            {
                return true;
            }
            else if ((m_Arguments == null) || m_Arguments.IsEmpty())
            {
                return true;
            }

            ktString Name = new ktString();
            ktString RestName = null;
            ktList Rest = null, CurrArgL = null;
            ktValue Arg = null, CurrArg = null, Arg2Add = null;
            //bool GoOn = true;

            CurrArgL = Arguments.First;

            m_Arguments.Reset();
            foreach (ktList AL in m_Arguments)
            {

                Arg2Add = null;

                /*if ((CurrArgL == null) || (CurrArgL.Node == null) || (CurrArgL.Node.Value == null)) {
                    Arg2Add = ktValue.Null;
                }*/
                if ((AL == null) || (AL.Node == null) || (AL.Node.Value == null))
                {
                    ktDebug.Log("NA");
                    // ????????????????????????????????????????????????????????????
                    Arg2Add = ktValue.Null;
                }
                else
                {
                    Arg = (ktValue)AL.Node.Value;
                    Name = Arg.Name;

                    if ((CurrArgL == null) || (CurrArgL.Node == null) || (CurrArgL.Node.Value == null))
                    {
                        ktDebug.Log("NULL");
                        if (Arg.Value != null)
                        {
                            Arg2Add = new ktValue(Name, Arg.Type, Arg.Value, Arg.HardType, Arg.Constant);
                        }
                        else
                        {
                            Arg2Add = new ktValue(Name, "null", null, true, true);
                        }
                        AddVariable(Arg2Add);
                        continue;
                    }
                    CurrArg = (ktValue)CurrArgL.Node.Value;

                    if (Name[0] == '#')
                    {
                        Name.RemoveFirst();
                        Arg2Add = new ktValue(Name, "ktList", Arguments, true, false);
                    }
                    else if (Name[0] == '$')
                    {
                        Name.RemoveFirst();
                        RestName = Name;
                        Rest = new ktList();

                        while (CurrArgL != null)
                        {
                            Arg2Add = new ktValue(CurrArg);

                            if (CurrArgL != null)
                            {
                                CurrArgL = CurrArgL.Next;

                                if ((CurrArgL == null) || (CurrArgL.Node == null) ||
                                    (CurrArgL.Node.Value == null))
                                {
                                    continue;
                                }
                                else
                                {
                                    CurrArg = (ktValue)CurrArgL.Node.Value;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((Arg.HardType) && (!Arg.CheckType(CurrArg)))
                        {
                            throw new ktError("ktFunction::MapArguments() : Didn't get a value with the right type for the argument '" +
                                    Name + "' in the function " +
                                    m_Name + "!", ktERR.WRONGTYPE);
                        }

                        Arg2Add = new ktValue(CurrArg);
                    }
                }

                if (Rest == null)
                {
                    Arg2Add.Name = Name;
                    AddVariable(Arg2Add);
                    //	AddVariable( new ktValue( Name, Arg2Add.Type, Arg2Add, Arg.HardType, Arg.Constant ) );
                }/* else {
					Rest.Add( Arg2Add );
				}*/

                if (CurrArgL != null)
                {
                    CurrArgL = CurrArgL.Next;
                }
            }

            if (Rest != null)
            {
                AddVariable(new ktValue(RestName, "ktList", Rest, true, false));
            }

            return true;
        }

        public override string Export()
        {
            ktString Exp = "function " + Name;

            if (Arguments != null)
            {
                Exp += Arguments.Export();
            }

            return Exp + ";";
        }
        public override string ToString()
        {
            ktString Exp = "function " + Name;

            if (Arguments != null)
            {
                Exp += Arguments.Export();
            }

            return Exp + ";";
        }

        #region properties
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ktList Arguments
        {
            get { return m_Arguments; }
            set { m_Arguments = value; }
        }
        public ktBlock Block
        {
            get { return m_Block; }
            set { m_Block = value; }
        }
        public ktValue Return
        {
            get { return m_Return; }
            set { m_Return = value; }
        }
        #endregion

        public ktString m_Name;

        public ktValue m_Return;
        public ktList m_Arguments;
        public ktBlock m_Block;

        public ktIntObj m_Parent;
    }

    public class ktDelegateFunction : ktFunction
    {
        public ktDelegateFunction(ktString Name, ktFunction_Delegate Delegate) :
            base(Name, null, null, ktValue.Null)
        {
            m_Delegate = Delegate;
        }
        public ktDelegateFunction(ktString Name, ktFunction_NoArg_Delegate Delegate) :
            base(Name, null, null, ktValue.Null)
        {
            m_Delegate = delegate(ktList Args) { return Delegate(); };
        }
        public ktDelegateFunction(ktString Name, ktFunction_Double_Delegate Delegate) :
            base(Name, null, null, ktValue.Null)
        {
            m_Arguments = new ktList();
            m_Arguments.Add(new ktValue("D", "double",
                                              kacTalk.Main.GetClass("double"),
                                              true, true));

            m_Delegate = delegate(ktList Args)
            {
                if ((Args == null) || (Args.FirstNode == null) ||
                    (Args.FirstNode.Value == null))
                {
                    throw new ktError("Didn't get an argument for '" +
                                      Name + "'!", ktERR.MISSING);
                }
                //ktDebug.Log( "ktDF::DOUBLE(" +Args.FirstNode.Value.ToString() + ")" );
                ktString S_In = new ktString(Args.FirstNode.Value.ToString());
                double D_In = 0.0;
                double D_Out = 0.0;

                try
                {
                    if (System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == ",")
                    {
                        S_In.Replace(".", ",", true);
                    }

                    D_In = S_In.ToDouble();
                }
                catch (Exception E)
                {
                    if (E.GetType() == typeof(System.FormatException) && !S_In.IsEmpty())
                    {
                        throw new ktError("ktDouble::CreateObject: Cant make '" + S_In + "' into an double", ktERR.WRONGTYPE);
                    }
                }
                ktDebug.Log("D_In: " + D_In.ToString());
                D_Out = Delegate(D_In);
                ktDebug.Log("D_Out: " + D_Out.ToString());

                return new ktValue("return", "double",
                                   kacTalk.Main.MakeObjectOf("double", D_Out),
                                   true, true);
            };
        }

        public override ktValue Run(ktList Arguments)
        {
            if (m_Delegate == null)
            {
                throw new ktError("ktDelegateFunction: No delegate to run!", ktERR.NOTSET);
            }

            return m_Delegate(Arguments);
        }
        public override string Export()
        {
            ktString Exp = "function " + Name;

            if (Arguments != null)
            {
                Exp += Arguments.Export();
            }

            return Exp + ";";
        }
        public override string ToString()
        {
            ktString Exp = "function " + Name;

            if (Arguments != null)
            {
                Exp += Arguments.Export();
            }

            return Exp + ";";
        }

        protected ktFunction_Delegate m_Delegate;
    }
}
