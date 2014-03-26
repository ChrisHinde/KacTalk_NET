//#define Debug

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktBlock : ktIntObj
    {
        public ktBlock()
            : base("ktBlock", 0)
        {
            m_Lines = null;
            m_Variables = null;
            m_Functions = null;
            m_Classes = null;
            m_Parent = null;
            m_Main = null;
        }
        public ktBlock(ktList Lines)
            : this()
        {
            SetLines(Lines);
        }
        public ktBlock(ktBlock Block)
            : this()
        {
            //?? SetBlock( Block );
        }
        public ktBlock(kacTalk Main)
            : this()
        {
            SetMain(Main);
        }/*
		public ktBlock( ktBlock Block ) : this()
		{
			SetBlock( Block );
		}*/


        public bool SetLines(ktList Lines)
        {
            if (Lines == null)
            {
                return false;
            }

            /* * /
            m_Lines = new ktList( Lines );/*/
            m_Lines = Lines;/* */

            return true;
        }

        public ktValue Run(ktList Arguments)
        {
            AddVariables(Arguments);
            return Run();
        }
        public ktValue Run()
        {
            if ((m_Lines == null) || (m_Lines.GetCount() == 0))
            {
                throw new ktError("ktBlock::Run(): Has no lines to run!", ktERR.NOTSET);
            }
            ktValue Ret = null;
         //   SetVariable("_Block", kacTalk.Main.MakeValueOf("ktBlock", this), true, true, true, true);
            //Console.WriteLine("ktBlock::Run() : " + m_Lines.Get_R());
            //			ktToken Token = null;
            //			ktValue Value = null;

            m_Lines.Reset();
            foreach (ktList Line in m_Lines)
            {
                if (m_skipNextLine)
                {
                    m_skipNextLine = false;
                    continue;
                }
                ktToken Token = (ktToken)Line.Node.Value;
                switch (Token.Type)
                {
                    case ktTokenType.Block:
                        {
                            Token.Block.SetParent(this);
                            Ret = Token.Block.Run();
                            break;
                        }
                    default:
                        {
                            Ret = RunLine(Line);
                            break;
                        }
                };
                //Ret.Name = "_";
                SetVariable("_", Ret, true, true, true, true);
            }

            return Ret;
        }

        public ktValue RunLine(ktList Line)
        {
            ktValue Ret = ktValue.Null;
            ktToken Token = null;
            ktToken LToken = null;

            if (Line == null)
            {
                throw new ktError("ktBlock::RunLine() : Didn't get a line to run!", ktERR.NOTSET);
            }
//#if Debug
	ktDebug.Log( "RL:" +  Line.Get_R( " ", true ) );
//#endif

            /*if (Token.LineNo == 3)
            {
                //ktDebug.Log("L3T: ");
                ktDebug.Log("L3: " + Line.Get_R(" ", true));
            }*/

            try
            {
                Token = (ktToken)Line.Node.Value;
                if (Line.GetCount() == 0)
                {
                    if (Line.Node != null)
                    {
                        if (Token.Type == ktTokenType.String)
                        {
                            Ret = new ktValue(":ktString", "ktString", MakeObjectOf("ktString", Token.Value),
                                              true, true);
                        }
                        else if (Token.Type == ktTokenType.Number)
                        {
                            Ret = new ktValue(":ktInt", "ktInt", MakeObjectOf("ktInt", Token.Value),
                                        true, true);
                        }
                        else if (Token.Type == ktTokenType.Float)
                        {
                            Ret = new ktValue(":ktFloat", "ktFloat", MakeObjectOf("ktFloat", Token.Value),
                                        true, true);
                        }
                        else if (Token.Type == ktTokenType.Id)
                        {
                            Ret = GetVariable(Token.Value);
                        }
                    }
                    else
                    {
                        //throw new ktError( "ktBlock::RunLine() : Didn't get a line to run!", ktERR.NOTSET );
                        return Ret;
                    }
                }

                switch (Token.Type)
                {
                    case ktTokenType.If:
                        {
                            bool r = CheckLogicStatement(Line.First,Token, ref Ret);
                            if (r)
                            {
                               // m_skipNextLine = true;
                                ktList Next = Line.First.Next;
                                ktToken IfToken = (ktToken)Next.Node.Value;
                                switch (IfToken.Type)
                                {
                                    case ktTokenType.Block:
                                        {
                                            IfToken.Block.SetParent(this);
                                            Ret = IfToken.Block.Run();
                                            break;
                                        }
                                    default:
                                        {
                                            Ret = RunLine(Next);
                                            break;
                                        }
                                };
                            }
                            return Ret;
                        }
                    case ktTokenType.For:
                        {
                            break;
                        }
                    case ktTokenType.Foreach:
                        {
                            break;
                        }
                    case ktTokenType.While:
                        {
                            break;
                        }
                }

                Line.Reset();
                foreach (ktList TL in Line)
                {
                    if ((TL.Node == null) || (TL.Node.Value == null))
                    {
                        continue;
                    }
                    LToken = Token;
                    Token = (ktToken)TL.Node.Value;

                    switch (Token.Type)
                    {
                        case ktTokenType.CompStatement:
                            {
                                Ret = HandleCompound(TL);
                                break;
                            }
                        case ktTokenType.AssignmentOperator:
                            {
                                Ret = HandleOperator(TL);
                                break;
                            }
                        case ktTokenType.Operator: {
                                Ret = HandleOperator(TL);
                                break;
                            }
                        /*case ktTokenType.If: {
                            break;
                        }*/
                        case ktTokenType.Number:
                        case ktTokenType.String:
                        case ktTokenType.Id:
                            {
                                ktList L = new ktList();
                                L.AddList(TL);
                                Ret = HandleStatement(L);
                                break;
                            }
                        default:
                            {
                                throw new ktError("ktBlock::RunLine(): Unexpected " + Token.Name +
                                                " on line " + Token.LineNo.ToString() +
                                                " (by character " + Token.CharPos.ToString() + ")",
                                                ktERR.UNEXP);
                            }
                    }
#if Debug
	ktDebug.Log( "New Line!!!!!!!!!!!!" );
#endif
                }
            }
            catch (ktError Err)
            {
                //ktDebug.Log("SL:SC:" + Err.ToString() + ";" + Line.Get_R());
                if (Token == null)
                {
                    if (LToken != null)
                    {
                        Token = LToken;
                    }
                    else if (Line.Node != null)
                    {
                        Token = (ktToken)Line.Node.Value;
                    }
                    else if (Line.FirstNode != null)
                    {
                        Token = (ktToken)Line.FirstNode.Value;
                    }
                    else if (Line.LastNode != null)
                    {
                        Token = (ktToken)Line.LastNode.Value;
                    }
                }
                if (Token != null)
                {
                    Err.SetLine(Token.LineNo);
                    Err.SetChar(Token.CharPos);
                    ktDebug.Log(Err.ToString());
                }
                else ktDebug.Log("NOOOOOOOO!!");
                throw Err;
            }
#if Debug
	ktDebug.Log( "EORL" );
#endif

            /*			if (Ret == null) {
				Ret = Value;
			}*/
            return Ret;
        }

        private bool CheckLogicStatement(ktList List, ktToken Token, ref ktValue r)
        {
            bool ret = false;
            //ktToken token = new ktToken(ktTokenType.Statement, "logic", 0, 0);
            Token.Type = ktTokenType.Statement;

          //  ktDebug.Log("CheckLogicST: " + List.Get_R(" "));

            r = TokenToValue( Token, List );

            ret = r.ToBool();
          //  ktDebug.Log("CheckLogicST: R:" + ret.ToString());

            return ret;
        }
        public static ktValue RunALine(ktList Line)
        {
            ktValue Ret = null;
            ktBlock Block = new ktBlock();

            Ret = Block.RunLine(Line);

            return Ret;
        }

        public ktList GetArguments(ktList Arguments)
        {
            ktList Args = null;
            ktToken Token = null;

#if Debug
	ktDebug.Log( "GAGAGAGAGAGA:" + Arguments.Get_R( "\t", true ) );
#endif
            if (Arguments == null)
            {
                return null;
            }
            else if ((Arguments.Node != null) && (Arguments.Node.Value != null) &&
                      ((Token = (ktToken)(Arguments.Node.Value)).Type == ktTokenType.CompStatement))
            {
                Args = new ktList();

                Args.Add(HandleCompound(Arguments).CopyValue());

                return Args;
            }
            else if (((Arguments.First != null) && (Arguments.First.Node != null) &&
                       (Arguments.First.Node.Value != null) &&
                       ((Token = (ktToken)(Arguments.First.Node.Value)).Type == ktTokenType.CompStatement)
                      ) || ((Arguments.Node != null) && (Arguments.Node.Value != null) &&
                       ((Token = (ktToken)(Arguments.Node.Value)).Type == ktTokenType.CompStatement)
                      )
                    )
            {
#if Debug
	ktDebug.Log( "GACSGACSGACSGACSGACSGACS:" + Arguments.Get_R() );
#endif
                ktList Stat = new ktList();
                Stat.Node = new ktNode("ktStatement", new ktToken(ktTokenType.Statement, "ktStatement",
                                            Token.LineNo, Token.CharPos));

                Args = new ktList();
                Args.Node = new ktNode("ktList", new ktToken(ktTokenType.List, "ktList", Token.LineNo,
                                            Token.CharPos));
                Args.AddList(Stat);
                Stat.AddList(Arguments);

                return GetArguments(Args);
            }
#if Debug
	ktDebug.Log( "gQAGAGAGAGAGAGAGGAgA::::" + Arguments.Get_R( "\t", true )  );
#endif

            if (Arguments.GetCount() == 0)
            {
                if (Arguments.Node == null)
                {
                    return null;
                }

                Token = (ktToken)Arguments.Node.Value;
                //ktDebug.Log( "GAGA111:" + Token.Name );
                Args = new ktList();

                if (Token.Type == ktTokenType.RunStatement)
                {
                    Args.Add(TokenToValue(Token, null));
                }
                else
                {
                    Args.Add(TokenToValue(Token, null).CopyValue());
                }

            }
            else
            {
                ktValue Value = null;

                Arguments.Reset();
                foreach (ktList L in Arguments)
                {
                    if ((L.Node == null) || (L.Node.Value == null))
                    {
                        continue;
                    }

                    if (Args == null)
                    {
                        Args = new ktList();
                    }

                    Token = (ktToken)L.Node.Value;

                    if (Token.Type == ktTokenType.RunStatement)
                    {
                        Value = TokenToValue(Token, L);
                    }
                    else
                    {
                        Value = TokenToValue(Token, L).CopyValue();
                    }

                    if (Value != null)
                    {
                        Args.Add(Value);
                    }
                }
            }
#if Debug
	ktDebug.Log( "EOFGA" );
	ktDebug.Log( Args.Get_R( "\t", true ) );
#endif
            return Args;
        }

        public bool AddVariable(ktValue Value, bool ABKT)
        {
            // No Var??
            if (Value == null)
            {
                return false;
            }

            // Create var. list if not set 
            if (m_Variables == null)
            {
                m_Variables = new ktList();
            }

            Value.AddedByKT = ABKT;

            // Create a new node (with name and var), and add it
            ktNode Node = new ktNode(Value.Name, Value, ABKT);
            return m_Variables.AddNode(Node);
        }
        public bool AddVariable(ktValue Value)
        {
            return AddVariable(Value, false);
        }
        public bool AddVariables(ktList Values)
        {
            // No??
            if (Values == null)
            {
                return false;
            }

            //ktValue Value;

            // Go through the list...
            foreach (ktList L in Values)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                //Value = new ktValue( (ktValue)L.Node.Value );
                AddVariable((ktValue)L.Node.Value);
            }

            return true;
        }
        public ktValue GetVariable(ktString Name)
        {
            if (Name.IsEmpty())
            {
                return null;
            }

            ktValue Var = null;

            try
            {
                if (m_Variables != null)
                {
                    ktNode Node = m_Variables.GetNode(Name);

                    if ((Node != null) && (Node.Value != null))
                    {
                        Var = (ktValue)Node.Value;
                    }
                }
            }
            catch (Exception) { }

            if (Var == null)
            {
                if (m_Parent != null)
                {
                    Var = m_Parent.GetVariable(Name);
                }
                else if (m_Main != null)
                {
                    Var = m_Main.GetVariable(Name);
                }
            }

            return Var;
        }

        /// <summary>
        /// Set the value of a variable
        /// </summary>
        /// <param name="Name">The name of the variable to set</param>
        /// <param name="Value">The value to give the variable</param>
        /// <param name="Add">Add the variable if it doesn't exists</param>
        /// <param name="Copy">Copy the value</param>
        /// <param name="IgnoreConstant">Ignore whether the var is constant (only used externally!)</param>
        /// <param name="ABKT">Is this variable added by KT/a script</param>
        /// <returns></returns>
        public bool SetVariable(ktString Name, ktValue Value, bool Add, bool Copy,
                                    bool IgnoreConstant, bool ABKT)
        {
            // Nothing to use??
            if (Name.IsEmpty() || (Value == null))
            {
                return false;
            }
#if Debug
	ktDebug.Log( "SetVariable( " + Name + ", " + Value.ToString() + ", " + Add.ToString() + ", " + Copy.ToString() + " )" );
#endif

            if (Copy)
            {
                Value = Value.CopyValue();
                Value.Name = Name;
            }

            ktValue Var = null;
            // Catch errors...
            try
            {
                // try to get Variable
                Var = GetVariable(Name);

                if ((!IgnoreConstant) && (Var.Constant))
                {
                    throw new ktError("You are trying to assign a value to " + Name +
                                        ", which is a constant!", ktERR.CONST);
                }

                // Set value...
                Var.SetValue(Value);
            }
            catch (ktError Err)
            {
                // If the error is due to nonexisting var??
                if ((Err.ErrorNumber == ktERR.NOTDEF) || (Err.ErrorNumber == ktERR.NOTFOUND))
                {
                    // Should we add?
                    if (Add)
                    {
                        // Set name
                        Value.Name = Name;
                        // Add...
                        return AddVariable(Value, ABKT);
                    }
                }

                // If we reach this, something went wrong or we just wasn't alloweed to add
                //  the variable...
                throw Err;
            }

            // Ok!?!?!
            return true;
        }
        public bool SetVariable(ktString Name, ktValue Value, bool Add, bool Copy, bool IgnoreConstant)
        {
            return SetVariable(Name, Value, Add, Copy, IgnoreConstant, false);
        }
        public bool SetVariable(ktString Name, bool ABKT, ktValue Value, bool Add, bool Copy)
        {
            return SetVariable(Name, Value, Add, Copy, false, ABKT);
        }
        public bool SetVariable(ktString Name, ktValue Value, bool Add, bool Copy)
        {
            return SetVariable(Name, Value, Add, Copy, false);
        }
        public bool SetVariable(ktString Name, bool ABKT, ktValue Value, bool Copy)
        {
            return SetVariable(Name, Value, false, Copy, ABKT);
        }
        public bool SetVariable(ktString Name, ktValue Value, bool Copy)
        {
            return SetVariable(Name, Value, false, Copy);
        }
        public bool SetVariable(ktString Name, bool ABKT, ktValue Value)
        {
            return SetVariable(Name, Value, true, ABKT);
        }
        public bool SetVariable(ktString Name, ktValue Value)
        {
            return SetVariable(Name, Value, true);
        }
        public void ClearVariables()
        {
            if ( m_Variables != null )
                m_Variables.Clear();
        }

        public bool AddFunction(ktString Name, ktBlock Block, ktList Arguments, ktValue Ret)
        {
            ktFunction Func = new ktFunction(Name, Arguments, Block, Ret);

            return AddFunction(Func);
        }
        public bool AddFunction(ktString Name, ktBlock Block, ktList Arguments)
        {
            return AddFunction(Name, Block, Arguments, ktValue.Null);
        }
        public bool AddFunction(ktString Name, ktBlock Block)
        {
            return AddFunction(Name, Block, null);
        }
        public bool AddFunction(ktFunction Function)
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

        public ktValue RunFunction(ktString Name, ktList Arguments)
        {
            ktValue Ret = ktValue.Null;
            ktNode Node = null;
            bool NotFound = false;
#if Debug
	ktDebug.Log( "RFRFRFRFRFRFRFRFRFRFRFRRFRF" );
#endif
            if (Name.IsEmpty())
            {
                throw new ktError("ktBlock::RunFunction() : Didn't get the name of the method to run.", ktERR.NOTSET);
            }

            if ((m_Functions == null) || (m_Functions.Count == 0))
            {//ktDebug.Log( "HC::NFunc!!\n");
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
                }
                else if (m_Main != null)
                {
                    return m_Main.RunFunction(Name, Arguments);
                }
                else
                {
                    throw new ktError("ktBlock::RunFunction() : Couldn't find the function '" +
                                      Name + ".", ktERR.NOTFOUND);
                }
            }

            return Ret;
        }

        public ktFunction GetFunction(ktString Name)
        {
            ktFunction Func = null;
            ktNode Node = null;
            bool NotFound = false;
#if Debug
	ktDebug.Log( "GFGFGFGFGFGFGFGFGFGFGFGGFGF(" + Name + ")" );
#endif
            if (Name.IsEmpty())
            {
                throw new ktError("ktBlock::GetFunction() : Didn't get the name of the method to find.", ktERR.NOTSET);
            }

            if ((m_Functions == null) || (m_Functions.Count == 0))
            {//ktDebug.Log( "HC::NFunc!!\n");
                NotFound = true;
            }
            else
            {
                Node = m_Functions.GetNode(Name);

                if (Node != null)
                {
                    Func = (ktFunction)Node.Value;
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
                    Func = m_Parent.GetFunction(Name);
                }
                else if (m_Main != null)
                {
                    Func = m_Main.GetFunction(Name);
                }
                else
                {
                    throw new ktError("ktBlock::GetFunction() : Couldn't find the function '" +
                                      Name + ".", ktERR.NOTFOUND);
                }
            }

            return Func;
        }

        public bool AddClass(ktClass Class)
        {
            if (Class == null)
                return false;

            if (m_Classes == null)
                m_Classes = new ktList();

            ktNode Node = new ktNode(Class);
            Node.Name = Class.Name;

            return m_Classes.Add(Node);
        }
        public ktClass GetClass(ktString Name)
        {
            if (Name.IsEmpty())
                return null;

            ktNode Node = null;
            
            if (m_Classes != null)
                Node = m_Classes.GetNode(Name);

            if (Node == null)
            {
                if (m_Parent != null)
                {
                    return m_Parent.GetClass(Name);
                }
                else if (m_Main != null)
                {
                    return m_Main.GetClass(Name);
                }
            }

            return (ktClass)Node.Value;
        }

        public ktIntObj MakeObjectOf(ktString Name, ktString Value)
        {
            if (Name.IsEmpty())
            {
                return null;
            }
            if (m_Main != null)
            {
                try
                {
                    ktClass Class = m_Main.GetClass(Name);

                    if (Class == null)
                    {
                        return null;
                    }
                    /*#if Debug
                        ktDebug.Log( "=0==0====00===0000==0000==0000===0000==0" );
                    #endif/**/
                    ktClass Class2 = Class.CreateObject(Value);
                    return Class2;
                }
                catch (ktError Err)
                {
#if Debug
Console.WriteLine( "MOOs:ERR" );
	ktDebug.Log( Err.ToString( ) );
	ktDebug.Log( Err.StackTrace );
#endif
                    return null;
                }
            }
            return null;
        }

        public ktIntObj MakeObjectOf(ktString Name, ktValue Value)
        {
            if (Name.IsEmpty())
            {
                return null;
            }
            if (m_Main != null)
            {
                try
                {
                    ktClass Class = m_Main.GetClass(Name);

                    if (Class == null)
                    {
                        return null;
                    }
                    /**#if Debug
                        ktDebug.Log( "=0==0====00===0000==0000==0000===0000==0" );
                    #endif/**/
                    ktClass Class2 = Class.CreateObject(Value);
                    return Class2;
                }
                catch (ktError Err)
                {
#if Debug
Console.WriteLine( "MOOv:ERR" );
	ktDebug.Log( Err.ToString( ) );
	ktDebug.Log( Err.StackTrace );
#endif
                    return null;
                }
            }
            return null;
        }

        public ktIntObj MakeObjectOf(ktString Name, object Value)
        {
            if (Name.IsEmpty())
            {
                return null;
            }
            if (m_Main != null)
            {
                try
                {
                    ktClass Class = m_Main.GetClass(Name);

                    if (Class == null)
                    {
                        return null;
                    }
                    /**#if Debug
                        ktDebug.Log( "=0==0====00===0000==0000==0000===0000==0" );
                    #endif/**/
                    ktClass Class2 = Class.CreateObject(Value);
                    return Class2;
                }
                catch (ktError Err)
                {
#if Debug
Console.WriteLine( "MOOo:ERR" );
	ktDebug.Log( Err.ToString( ) );
	ktDebug.Log( Err.StackTrace );
#endif
                    return null;
                }
            }
            return null;
        }

        protected ktValue HandleCompound(ktList Comp)
        {
            ktValue Value = ktValue.Null;
            //ktDebug.WrapLevel++;

            if (Comp == null)
            {
                return Value;
            }
#if Debug
	if (!ktDebug.Enabled) { ktDebug.D.Enable(); this.m_enabledDebug = true;  this.m_enabledAt = ktDebug.WrapLevel; }
	ktDebug.Log( "HCHCHCHCHCHCHCHCHHCHCHC:\n" + Comp.Get_R( ktDebug.GetPrefix(), true ) );
#endif

            ktToken SToken= null;
            ktToken First = null;
            ktToken Last = null;

            if ((Comp.Node != null) && (Comp.Node.Value != null))
            {
                SToken = (ktToken)(Comp.Node.Value);
            }
            if ((Comp.First != null) && (Comp.First.Node != null))
            {
                First = (ktToken)(Comp.First.Node.Value);
            }
            if ((Comp.Last != null) && (Comp.Last.Node != null))
            {
                Last = (ktToken)(Comp.Last.Node.Value);
            }

            if ((First == null) && (Last == null))
            {
                return Value;
            }

            ktDebug.Log("SEPV::" + First.Value + ";;" + Last.Value );
            if ((SToken != null) && (SToken.Value == "§"))
            {
#if Debug
                    ktDebug.Log("METHODCALL");
#endif
                

                ktClass Class = null;

                try
                {
                    Value = GetObjectFor(First);
                    if (Value.IsNull())
                    {
                        throw new Exception();
                    }
                    Class = (ktClass)Value.Value;
                }
                catch (Exception)
                {
#if Debug
                    ktDebug.Log("ID:" + Err.Message);
#endif
                    throw new ktError("Can't find the symbol '" + First.Value + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR._404);
                }

                First = (ktToken)Comp.First.Next.Node.Value;
                Last = (ktToken)Comp.Last.First.Node.Value;
                /*First = (ktToken)Comp.Last.First.Node.Value;
                Last = (ktToken)Comp.Last.Last.Node.Value;*/
                //ktDebug.Log(".." + First.Value + ";" + SToken.Value + "::\n" + Last.Value + "===----------====-===");

                Value = Class.RunMethod(First.Value,GetArguments(Comp.Last.Last));
            }
            else if ((First.Type == ktTokenType.CompStatement) && (First.Value == "."))
            {
#if Debug
	ktDebug.Log( "SEP::" + Comp.First.Get_R( ktDebug.GetPrefix() ) );
#endif
                Value = HandleCompound(Comp.First);
//                ktDebug.Log("SLU:" + Value.ToString() + ";");
                if (Value.Type == "ktFunction")
                {
                    ktDebug.Log("WTF???");
                    if ((Value.Value.GetType() == typeof(ktFunction)) ||
                        (Value.Value.GetType() == typeof(ktDelegateFunction)))
                    {
                        ktDebug.Log("WTF!!!_" + Value.ToString() + "!|!" + Value.Value.GetType().ToString());
                        ktFunction Func = (ktFunction)Value.Value;
                        Value = Func.Run(GetArguments(Comp.Last));
                    }
                    else
                    {
                        ktDebug.Log("WTF???_" + Value.ToString() + "!|!" + Value.Value.GetType().ToString());
                        ktClass Class = (ktClass)Value.Value;
                        Value = Class.RunMethod("run", GetArguments(Comp.Last));
                    }
                }
                else
                {
                    ktDebug.Log("HUM:" + Value.Type);
                    ktDebug.Log("HUM_T:" + Value.GetType() + ";" + Value.Value.GetType());
                    ktDebug.Log("HUM_T:" + Value.Value.GetStringType());
                    ktDebug.Log("Last:" + Comp.Last.Get_R(ktDebug.GetPrefix()));
                    ktDebug.Log("LastT:" + Last.Type);
                    if (Last.Type == ktTokenType.Id)
                    {
                        if (Value.Value.GetStringType() == "ktClass")
                        {
                            Value = ((ktClass)Value.Value).GetMember(Last.Value);
                        }
                        else
                        {
                            ktDebug.Log("__HUMMM??????????????");
                        }
                    }
                }
                /*if (Value.Type == "ktClass") {
                    ktClass Class = null;

                    try {;
                        if (Value.IsNull()) {
                            throw new Exception();
                        }
                        Class = (ktClass)Value.Value;

                        if (Last.Type == ktTokenType.Id) {
                            Value = Class.GetMember( Last.Value );
                        } else {
                            throw new ktError( "Unexpected '" + Last.ToString() + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR.UNEXP );
                        }
                    } catch (Exception Err) {
#if Debug
    ktDebug.Log( Err.Message );
#endif
                        if (Err.GetType() == typeof( ktError )) {
                            throw Err;
                        }

                        throw new ktError( "Can't find the symmmmmmmmmmbol '" + First.Value + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR._404 );
                    }
                }*/
            }
            else if ((First.Type == ktTokenType.CompStatement) && (First.Value == "::"))
            {
                ktClass Class = null;
                ktDebug.Log("::::::::::::::::::::::::");

                Value = HandleCompound(Comp.First);
                if (Value.Type == "ktFunction")
                {
                    if (Value.Value.GetType() == typeof(ktFunction))
                    {
                        ktDebug.Log("::WTF???_" + Value.ToString() + "!|!" + Value.Value.GetType().ToString());
                        ktFunction Func = (ktFunction)Value.Value;
                        Value = Func.Run(GetArguments(Comp.Last));
                    }
                    else
                    {
                        Class = (ktClass)Value.Value;
                        Value = Class.RunMethod("run", GetArguments(Comp.Last));
                    }
                }
                else ktDebug.Log("CHUM:" + Value.Type);
#if Debug
	ktDebug.Log( "SEP.::::::::::." + Value.ToString() + "E;" + First.LineNo );
#endif
            }
            else if ((SToken != null) && (SToken.Value == "§"))
            {
#if Debug
                    ktDebug.Log("METHODCALL");
#endif
                

                ktClass Class = null;

                try
                {
                    Value = GetObjectFor(First);
                    if (Value.IsNull())
                    {
                        throw new Exception();
                    }
                    Class = (ktClass)Value.Value;
                }
                catch (Exception Err)
                {
#if Debug
                    ktDebug.Log("ID:" + Err.Message);
#endif
                    throw new ktError("Can't find the symbol '" + First.Value + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR._404);
                }

                First = (ktToken)Comp.Last.First.Node.Value;
                Last = (ktToken)Comp.Last.Last.Node.Value;
                //ktDebug.Log(".." + First.Value + ";" + SToken.Value + "::\n" + Last.Value + "===----------====-===");
                

                Value = Class.RunMethod(First.Value,GetArguments(Comp.Last.Last));
            }
            else if ((SToken != null) && (SToken.Value == "."))
            {
                ktClass Class = null;

                try
                {
                    Value = GetObjectFor(First);
                    ktDebug.Log("SLU!!!!!!!!!!");
                    if (Value.IsNull())
                    {
                        throw new Exception();
                    }
                    Class = (ktClass)Value.Value;
                    ktDebug.Log("SLU!!++++++!!");
                    //					Class.GetM
                }
                catch (Exception Err)
                {
#if Debug
	ktDebug.Log( "ST.DOT.:" + Err.Message );
#endif
                    throw new ktError("Can't find the symbol '" + First.Value + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR._404);
                } /*catch (NullReferenceException Err) {
#if Debug
	ktDebug.Log( "ST.DOT.NULL:" + Err.Message );
#endif
					throw new ktError( "Can't find the symbol '" + First.Value + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR._404 );
				}*/
                if (Class == null)
                {
                    throw new ktError("A null value was found where an object instance was required, on line " + Last.LineNo.ToString() + " by character " + Last.CharPos.ToString() + "!", ktERR.NULL);
                }

                Value = Class.GetMember(Last.Value);
#if Debug
	ktDebug.Log( "SEP......." + Value.ToString() + ";E:" + First.LineNo );
#endif
            }
            else if ((SToken != null) && (SToken.Value == "::"))
            {
                ktDebug.Log("CONFU::" + Last.Value + ";");
                ktDebug.Log("SEPV::" + First.Value);
                ktContext Con = kacTalk.Main.GetContext(First.Value);
                ktDebug.Log("CON::" + ((Con == null) ? "NULL" : Con.Export()) + ";");
                Value = Con.GetMember(Last.Value);
                ktDebug.Log("CON_MEM::" + ((Value == null) ? "NULL" : Value.Export()) + ";");
            }
            else 
            {
//                ktDebug.Log(".." + First.Value + ";" + SToken.Value + "::\n" + Comp.Node.Value + "===----------====-===");
                if ((First.Type == ktTokenType.Id) && (Comp.GetCount() == 2))
                {
#if Debug
                    ktDebug.Log("IDIDIDIDIDIDIDIDIDIDID");
#endif
                    ktClass Class = null;

                    try
                    {
                        Value = GetObjectFor(First);
                        if (Value.IsNull())
                        {
                            throw new Exception();
                        }
                        Class = (ktClass)Value.Value;

                        //						Class.GetM
                    }
                    catch (Exception Err)
                    {
#if Debug
                        ktDebug.Log("ID:" + Err.Message);
#endif
                        throw new ktError("Can't find the symbol '" + First.Value + "' on line " + First.LineNo.ToString() + " by character " + First.CharPos.ToString() + "!", ktERR._404);
                    }

                    Value = Class.RunMethod("_func_call", GetArguments(Comp.Last));
                    /*
                    if (Comp.GetCount() == 2) {
                        Value = RunFunction( First.Value, GetArguments( Comp.Last ) );
                    }*/
                }
            }
#if Debug
	ktDebug.Log( "- - - EO_HC - - -" );
	if (this.m_enabledDebug && (this.m_enabledAt == ktDebug.WrapLevel)) { ktDebug.D.Disable(); }
#endif

           // ktDebug.WrapLevel--;
            return Value;
        }
        protected ktValue HandleStatement(ktList Statement)
        {
#if Debug
	ktDebug.Log( "HSHSHSHSHSHSHSHSHSHSHSHSHSHSHS" );
#endif
            ktValue Value = ktValue.Null;
ktDebug.Log(Statement.Get_R());

            if ((Statement == null) || (Statement.IsEmpty()) ||
                (Statement.First.Node == null) || (Statement.First.Node.Value == null))
            {
                ktDebug.Log(Statement.Get_R());
                return Value;
            }
#if Debug
	ktDebug.Log( Statement.Get_R() );
#endif
            ktToken Token = (ktToken)Statement.First.Node.Value;

            Value = TokenToValue(Token, Statement.First);
#if Debug
	ktDebug.Log( "EOHS" );
#endif
            return Value;
        }
        protected ktValue HandleRunStatement(ktList Statement)
        {
#if Debug
	ktDebug.Log( "HrSHRSHRSHRSHRSHRSHRSHRSHRSHRSHRSHRSHRSHRSHRS" );
#endif
            ktValue Value = ktValue.Null;

            if ((Statement == null) || (Statement.IsEmpty()))
            {
                return Value;
            }
#if Debug
	ktDebug.Log( Statement.Get_R() );
#endif
            ktRunStatement RunStatement = new ktRunStatement(Statement, this);

            Value = RunStatement.AsValue();
#if Debug
	ktDebug.Log( "EOHRS" );
#endif
            return Value;
        }

        protected ktValue HandleList(ktList List)
        {
            ktValue Value = ktValue.Null;

            if (List == null)
            {
                return Value;
            }
#if Debug
	ktDebug.Log( "HandleList:::" + List.Get_R() );
#endif
            ktClass ListClass = kacTalk.Main.MakeObjectOf("ktList", GetArguments(List));

            Value = new ktValue("ktList", "ktList", ListClass, true, true);
#if Debug
	ktDebug.Log( "EOHL" );
#endif
            return Value;
        }


        protected ktValue TokenToValue(ktToken Token, ktList List)
        {
            ktValue Value = ktValue.Null;
#if Debug
	//ktDebug.WrapLevel++;
	ktDebug.Log( "TokenToValue( " + Token + ", List )" );
#endif
            switch (Token.Type)
            {
                case ktTokenType.String:
                    {
                        Value = new ktValue("", "ktString", MakeObjectOf("ktString", Token.Value), true, true);
                        break;
                    }
                case ktTokenType.EndString:
                    {
                        Value = new ktValue("", Token.Name, new ktString(Token.Value), true, true);
                        break;
                    }
                case ktTokenType.Number:
                    {
                        Value = new ktValue("", "ktInt", MakeObjectOf("ktInt", Token.Value), true, true);
                        break;
                    }
                case ktTokenType.Float:
                    {
                        Value = new ktValue("", "ktFloat", MakeObjectOf("ktFloat", Token.Value), true, true);
                        break;
                    }
                case ktTokenType.Boolean:
                    {
                        Value = new ktValue("", "ktBool", MakeObjectOf("ktBool", Token.Value), true, true);
                        break;
                    }
                case ktTokenType.Statement:
                    {
                        Value = new ktValue(HandleStatement(List));
                        break;
                    }
                case ktTokenType.CompStatement:
                    {
                        Value = new ktValue(HandleCompound(List));
                        break;
                    }
                case ktTokenType.RunStatement:
                    {
                        Value = new ktValue(HandleRunStatement(List));
                        break;
                    }
                case ktTokenType.List:
                    {
                        Value = new ktValue(HandleList(List));
                        break;
                    }
                case ktTokenType.AssignmentOperator:
                case ktTokenType.Operator:
                    {
                        Value = HandleOperator(List);
                        break;
                    }
                case ktTokenType.Id:
                    {
                        try
                        {
                            Value = GetVariable(Token.Value);
                            break;
                        }
                        catch (ktError Err)
                        {
                            if ((Err.ErrorNumber != ktERR.NOTDEF) &&
                                (Err.ErrorNumber != ktERR.NOTFOUND))
                            {
                                throw Err;
                            }
                        }
                        try
                        {
                            ktClass Class = GetClass(Token.Value);
                            Value = Class.GetProperty("_");//m_Main.MakeValueOf( "ktClass", Class );
                            break;
                        }
                        catch (ktError Err)
                        {
                            if ((Err.ErrorNumber != ktERR.NOTDEF) &&
                                (Err.ErrorNumber != ktERR.NOTFOUND))
                            {
                                throw Err;
                            }
                        }
                        catch (NullReferenceException) { }
                        try
                        {
                            ktFunction Function = GetFunction(Token.Value);
                            Value = m_Main.MakeValueOf("ktFunction", Function);
                            break;
                        }
                        catch (ktError Err)
                        {
                            ktDebug.Log(Err.ToString());
                            if ((Err.ErrorNumber != ktERR.NOTDEF) &&
                                (Err.ErrorNumber != ktERR.NOTFOUND))
                            {
                                throw Err;
                            }
                        }
                        throw new ktError("Can't find the symbol '" + Token.Value + "'!",
                                            ktERR.NOTFOUND);
                    }
                case ktTokenType.New:
                    {
                        Value = HandleNew(List);
                        break;
                    }
                case ktTokenType.Null:
                    {
                        Value = ktValue.Null;
                        break;
                    }
                default:
                    {
                        ktDebug.Log("UNKNOWN TOKEN:" + Token.Name + ":" + ktToken.TokenToString(Token.Type) + "?");
                        break;
                    }
            }

         //   ktDebug.WrapLevel--;
    //        Console.WriteLine("TTV:" + Value.Export());
            return Value;
        }

        protected ktValue HandleNew(ktList Statement)
        {
            ktValue NewValue = ktValue.Null;
            ktClass NewObj = null;

#if Debug
	ktDebug.Log( "HandleNew:" + Statement.Get_R() );
#endif

            if ((Statement == null) || (Statement.GetCount() == 0) ||
                (Statement.First == null) || (Statement.First.Node == null) ||
                (Statement.First.Node.Value == null) ||
                ((Statement.First.First != null) && (
                        (Statement.First.First.Node == null) || (Statement.First.First.Node.Value == null)))
               )
            {
                throw new ktError("ktBlock::HandleNew: Didn't get enough information to create a new object!", ktERR.MISSING);
            }

            ktToken Token = (ktToken)Statement.First.Node.Value;
            ktString Name = new ktString();
            ktList Arguments = null;

            if (Token.Type == ktTokenType.Id)
            {
                Name = Token.Value;
            }
            else if ((Token.Type == ktTokenType.CompStatement) ||
                      (Token.Type == ktTokenType.Statement))
            {
                Token = (ktToken)Statement.First.First.Node.Value;
                Name = Token.Value;

                Arguments = GetArguments(Statement.First.Last);
            }

            NewObj = (ktClass)MakeObjectOf(Name, (object)null);

            if (Arguments != null)
            {
                ktDebug.Log("NO: " + NewObj.Export() + ";;Args:" + Arguments.Get_R());
                try
                {
                    NewObj.RunMethod("constructor", Arguments);
                }
                catch (ktError Err)
                {
                    ktDebug.Log("NEW_CONS:ERR:" + Err.ToString());
                    if (Err.ErrorNumber == ktERR.NOTFOUND)
                    {
                        throw new ktError("The class " + Name + " is missing an constructor!", ktERR.NOTFOUND);
                    }
                    else
                    {
                        throw Err;
                    }
                }
            }

            NewValue = new ktValue("", Name, NewObj, false, false);
            ktDebug.Log("NV: " + NewValue.ToString());
            return NewValue;
        }

        protected ktValue HandleOperator(ktList OpTree)
        {
            ktValue Value = ktValue.Null;
            ktValue SecondValue = ktValue.Null;

            if (OpTree == null)
            {
                ktDebug.Log("NOOOOOOOHO!!O!!!!!!!");
                return Value;
            }
#if Debug
	ktDebug.Log( "HOHOHOHOHOHOHOHOHOHOHOHOHOHO:" + OpTree.Get_R( " ", true ) );
#endif

            ktToken Op = null;
            ktToken First = null;
            ktToken Last = null;

            if ((OpTree.Node != null) && (OpTree.Node.Value != null))
            {
                Op = (ktToken)(OpTree.Node.Value);
            }
            if ((OpTree.First != null) && (OpTree.First.Node != null))
            {
                if (OpTree.First.Node.Value is ktToken)
                {
                    First = (ktToken)(OpTree.First.Node.Value);
                }
                else if (OpTree.First.Node.Value is ktValue)
                {
                    Value = (ktValue)(OpTree.First.Node.Value);
                    First = new ktToken(ktTokenType.NULLTOKEN, "Value", 0, 0);
                }
            }
            if ((OpTree.Last != null) && (OpTree.Last.Node != null))
            {
                Last = (ktToken)(OpTree.Last.Node.Value);
            }

            if ((First == null) && (Last == null) || (Op == null))
            {
                return Value;
            }

            if (Op.Type == ktTokenType.Operator)
            {
                ktClass Class = null;

                if (First.Type == ktTokenType.NULLTOKEN)
                {
                    //ktDebug.Log( "000000000000000000000000000000" );
                }
                else
                {
                    Value = TokenToValue(First, OpTree.First);
                }

                SecondValue = TokenToValue(Last, OpTree.Last);

                if (Value.IsNull() || SecondValue.IsNull())
                {
                    throw new ktError("The " + (Value.IsNull() ? "left-hand" : "right-hand") +
                                  " operand can not be a null value!", ktERR.NULL);
                }

                try
                {
                    //ktDebug.Log("111:" + ((ktClass)Value.Value).Name + "!!!!222:" + ((ktClass)SecondValue.Value).Name + "#####");
                    Class = (ktClass)(Value.Value);
                    //ktDebug.Log( "?????????????????????????????????????" );
                    ktList Args = null;
                    if (((ktClass)SecondValue.Value).Name == "ktList")
                    {
                        ktDebug.Log("!!!!!!!!!");
                        Args = (ktList)(((ktClass)SecondValue.Value).GetProperty("___").Value);
                    }
                    else
                    {
                        Args = new ktList();
                        Args.Add(SecondValue);
                    }

                    ArrayList MethNames = new ArrayList(4);
                    MethNames.Add("operator" + Op.Value.ToString());
                    MethNames.Add("op" + Op.Value.ToString());

                    switch (Op.Value)
                    {
                        case "+":
                            {
                                MethNames.Add("_add");
                                break;
                            }
                        case "-":
                            {
                                MethNames.Add("_subtract");
                                break;
                            }
                        case "*":
                            {
                                MethNames.Add("_multiply");
                                MethNames.Add("_times");
                                break;
                            }
                        case "/":
                            {
                                MethNames.Add("_divide");
                                break;
                            }
                        case "^":
                            {
                                if (kacTalk.Main.MathMode)
                                {
                                    MethNames.Add("_pow");
                                }
                                break;
                            }
                    }

                    foreach (string Meth in MethNames)
                    {
                        try
                        {
                            Value = Class.RunMethod(Meth, Args);
#if Debug
ktDebug.Log( "AFTREOP:" + Value.Export() + ";" );
#endif
                            return Value;
                        }
                        catch (ktError Err)
                        {
//                            ktDebug.Log("ASSMETHERR:" + Err.ToString());
                            if ((Err.ErrorNumber != ktERR._404) &&
                                (!Err.Message.StartsWith("Couldn't find the method ")))
                            {
                                throw Err;
                            }
                        }
                    }
                    throw new ktError("-", ktERR._404);
                }
                catch (ktError Err)
                {
#if Debug
ktDebug.Log( "##############" + Err.ToString() + "\n" + Err.StackTrace ); //ktDebug.Log
#endif
                    if (Err.ErrorNumber != ktERR._404)
                    {
                        throw Err;
                    }
                    else if (Class == null)
                    {
                        throw new ktError("The variable '" + Value.Value + "' is not set", ktERR.NOTSET);
                    }
                    else if (Class.Name == "ktList")
                    {
                        ktList L = new ktList();
                        L.Node = OpTree.Node;
                        ktDebug.Log("HANDLELIST:::" + ((ktList)(Class.GetProperty("_FirstObject").Value)).Get_R() + " ==");
                        L.AddList((ktList)(Class.GetProperty("_FirstObject").Value));
                        L.AddList(OpTree.Last);

                        return HandleOperator(L);
                    }
                    else
                    {
                        throw new ktError("The operator '" + Op.Value + "' isn't handled by the class " +
                                            Class.Name, ktERR.NOTIMP);
                    }
                }

                //	ktDebug.Log( "111:" + Value.Value.Export( ) + "!!!!222:" + SecondValue.Value.Export() + "#####" );
                //break;
            }
            else if (Op.Type == ktTokenType.AssignmentOperator)
            {
                Value = TokenToValue(Last, OpTree.Last);
                if ((First.Type == ktTokenType.Id) || (First.Type == ktTokenType.CompStatement))
                {
                    Value.Constant = false;
#if Debug
ktDebug.Log( "HO:AO: " + First.Value + ";" );
#endif
                    Value = HandleAssignment(Op, OpTree.First, First, Value);
                    //						SetVariable( First.Value, Value, true, true );
                }
                else if (First.Type == ktTokenType.VarStatement)
                {
                    if (Op.Value != "=")
                    {
                        throw new ktError("Expected a simple assignment operator (=) on line " + Op.LineNo.ToString() +
                                          " by character " + Op.CharPos.ToString() + ", but found " +
                                          Op.Value + "!", ktERR.UNEXP);
                    }
                    Value = HandleCompAssignment(OpTree.First, Value);
                }
                else
                {
                    throw new ktError("Can't assign a value to an " + First.Name + " (" + First.Value + ") on line " + Op.LineNo.ToString() +
                                        ", character " + Op.CharPos.ToString() + "!", ktERR.UNKNOWN);
                }

                //break;
            }
            else
            {
                throw new ktError("Unrecognized operator (" + Op.Value + ") on line " + Op.LineNo.ToString() +
                                    ", character " + Op.CharPos.ToString() + "!", ktERR.UNKNOWN);
            }
#if Debug
	ktDebug.Log( "END OF HO!!!" );
#endif
            return Value;
        }
        protected ktValue HandleAssignment(ktToken Operator, ktList ListTarget, ktToken Target, ktValue AssValue)
        {
            ktValue Value = ktValue.Null;
            ktClass Class = null;
            ktList Args = null;

            AssValue.HardType = false;
#if Debug
	ktDebug.Log(  "ASSSVALUE:" + Value.Export() + ";" );
#endif

            try
            {
                if (Target.Type == ktTokenType.Id)
                {
                    Value = GetVariable(Target.Value);
                }
                else if (Target.Type == ktTokenType.CompStatement)
                {
                    Value = HandleCompound(ListTarget);
#if Debug
	ktDebug.Log(  "ASCP:" + Value.Export() + ";" );
#endif
                }
            }
            catch (ktError Err)
            {
                if ((Err.ErrorNumber == ktERR.NOTFOUND) || (Err.ErrorNumber == ktERR.NOTDEF))
                {
                    if (Operator.Value == "=")
                    {
                        SetVariable(Target.Value, true, AssValue, true, true);
                        return AssValue;
                    }
                    else
                    {
                        throw new ktError("You can't use a complex assignment operator (" +
                                            Operator.Value + ") when creating a variable!",
                                          ktERR.UNEXP);
                    }
                }
                else
                {
                    throw Err;
                }
            }
            //ktDebug.Log("HARD:" + Value.HardType);
            if (Value.Constant)
            {
                throw new ktError("You are trying to assign a value to " + Target.Value +
                                    ", which is a constant!", ktERR.CONST);
            }
            else if (Value.IsNull())
            {
                SetVariable(Target.Value, true, AssValue, true, true);
                return AssValue;
            }/*
            else if (!Value.HardType)
            {
                SetVariable(Target.Value, Value = AssValue, true, true);
            }*/

            try
            {
                Class = (ktClass)Value.Value;

                Args = new ktList();
                Args.Add(AssValue);

                ArrayList MethNames = new ArrayList(4);
                MethNames.Add("operator" + Operator.Value);
                MethNames.Add("op" + Operator.Value);

                switch (Operator.Value)
                {
                    case "=":
                        {
                            MethNames.Add("_assign");
                            break;
                        }
                    case "+=":
                        {
                            MethNames.Add("_append");
                            break;
                        }
                }

                foreach (ktString Meth in MethNames)
                {
                    try
                    {
                        Value.SetTheValue(Class.RunMethod(Meth, Args));
#if Debug
	ktDebug.Log( "AFTREEEEOP:" + Value.Export() + ";" );
#endif
                        return Value;
                    }
                    catch (ktError Err)
                    {
//                        ktDebug.Log("ASSMETHERR:" + Err.ToString());
                        if ((Err.ErrorNumber != ktERR._404) &&
                            (!Err.Message.StartsWith("Couldn't find the method ")))
                        {
                            throw Err;
                        }
                    }
                }
                throw new ktError("-", ktERR._404);
            }
            catch (ktError Err)
            {
#if Debug
	ktDebug.Log( "##############" + Err.ToString() + "\n" + Err.StackTrace );
#endif
                if (Err.ErrorNumber != ktERR._404)
                {
                    throw Err;
                }
                else if (Operator.Value == "=")
                {
                    SetVariable(Target.Value, Value = AssValue, true, true);
                }
                else if (Class == null)
                {
                    throw new ktError("The variable '" + Target.Value + "' is not set", ktERR.NOTSET);
                }
                else
                {
                    throw new ktError("The operator '" + Operator.Value + "' isn't handled by the class " +
                                        Class.Name, ktERR.NOTIMP);
                }
            }

            return Value;
        }
        protected ktValue HandleCompAssignment(ktList Target, ktValue Value)
        {
            bool Constant = false, Hard = false;
            ktString Name = new ktString(), ClassName = null;
            ktToken Token = null;
            ktClass Class = null;
#if Debug
	ktDebug.Log( "HCAHCAHCAHCAHCAHCAHCAHCAHCAHCAHCAHCAHCAHCA" );
#endif
            ktDebug.Log("HCA: V:" + Value.Export() + "\n" + Target.Get_R());
            Target.Reset();
            foreach (ktList L in Target)
            {
                if ((L.Node == null) || (L.Node.Value == null))
                {
                    continue;
                }

                Token = (ktToken)L.Node.Value;
#if Debug
	ktDebug.Log( "HCA:" + Token.ToString() );
#endif

                switch (Token.Type)
                {
                    case ktTokenType.Const:
                        {
                            if (Constant)
                            {
                                throw new ktError("Unexpected " + Token.ToString() + " on line " +
                                                      Token.LineNo.ToString() + " by character " +
                                                      Token.CharPos.ToString() + "!",
                                                  ktERR.UNEXP);
                            }

                            Constant = true;
                            break;
                        }
                    case ktTokenType.Hard:
                        {
                            if (Hard)
                            {
                                throw new ktError("Unexpected " + Token.ToString() + " on line " +
                                                      Token.LineNo.ToString() + " by character " +
                                                      Token.CharPos.ToString() + "!",
                                                  ktERR.UNEXP);
                            }

                            Hard = true;
                            break;
                        }
                    case ktTokenType.Id:
                        {
                            if (!Name.IsEmpty())
                            {
                                try
                                {
                                    Class = GetClass(Name);
                                    ClassName = Name;
                                    Hard = true;
                                } catch (Exception) {
                                    throw new ktError("Unexpected " + Token.ToString() + " (" +
                                                          Token.Value + ") on line " +
                                                          Token.LineNo.ToString() + " by character " +
                                                          Token.CharPos.ToString() + "!",
                                                      ktERR.UNEXP);
                                }
                            }

                            Name = Token.Value;
                            break;
                        }
                }
            }

            Value.Constant = Constant;
            Value.HardType = Hard;
            if (!Value.IsNull())
            {
                ((ktClass)Value.Value).HardType = Hard;
                ((ktClass)Value.Value).IsConstant = Constant;
            }

            if (Class != null)
            {
                ktList Arg = new ktList();
                ktClass Obj = Class.CreateObject();
                Obj.HardType = Hard;
                Obj.IsConstant = Constant;

                if (!Value.IsNull())
                {
                    Arg.Add(Value);
                    Obj.RunMethod("Assign", Arg);
                }

                Value = new ktValue(Name, ClassName, Obj, Hard, Constant); 
            }

            SetVariable(Name, true, Value, true, true);

            return Value;
        }


        public ktValue GetObjectFor(ktToken Token)
        {
            ktValue Value = ktValue.Null;

            if (Token.Type != ktTokenType.Id)
            {
                throw new ktError("ktBlock::GetObjectFor: Excpected an id-token but got (" + Token.ToString() + ")", ktERR.UNEXP);
            }
#if Debug
	ktDebug.Log( "GOF:" + Token.ToString() + "!" );
#endif
            /*try {
				Value = GetVariable( Token.Value );
			} catch (Exception) {
				ktClass Class = GetClass( Token.Value );

				Value = new ktValue( Token.Value, Class.Name, Class, true, true );
			}*/
            try
            {
             //   ktDebug.Log("GOF - Var");
                Value = GetVariable(Token.Value);
                return Value;
            }
            catch (ktError Err)
            {
                if ((Err.ErrorNumber != ktERR.NOTDEF) &&
                    (Err.ErrorNumber != ktERR.NOTFOUND))
                {
                    throw Err;
                }
            }
            catch (NullReferenceException) { }
            try
            {
//                ktDebug.Log("GOF - Class");
                ktClass Class = GetClass(Token.Value);
                Value = Class.GetProperty("_"); //m_Main.MakeValueOf( "ktClass", Class );
                return Value;
            }
            catch (ktError Err)
            {
                if ((Err.ErrorNumber != ktERR.NOTDEF) &&
                    (Err.ErrorNumber != ktERR.NOTFOUND))
                {
                    throw Err;
                }
            }
            catch (NullReferenceException) { }
            try
            {
//                ktDebug.Log("GOF - Func");
                ktFunction Function = GetFunction(Token.Value);
                Value = m_Main.MakeValueOf("ktFunction", Function);
                return Value;
            }
            catch (ktError Err)
            {
                ktDebug.Log("_" + Err.ToString());
                if ((Err.ErrorNumber != ktERR.NOTDEF) &&
                    (Err.ErrorNumber != ktERR.NOTFOUND))
                {
                    throw Err;
                }
            }

            return Value;
        }


        public bool ClearKTSymbols()
        {
            if (m_Classes != null)
            {
                foreach (ktList CL in m_Classes)
                {
                    if ((CL == null) || (CL.Node == null))
                    {
                        continue;
                    }
                    if (CL.Node.AddedByKT)
                    {
                        m_Classes.RemoveCurrent();
                    }
                }
            }
            return true;
        }


        public override string ToString()
        {
            return "{" + "}";
        }

        public override string Export()
        {
            if (m_Lines != null)
            {
                return m_Lines.Get_R();
            }
            else
            {
                return ":null";
            }
        }

        public bool SetParent(ktBlock Block)
        {
            m_Parent = Block;
            return true;
        }
        public ktBlock GetParent() { return m_Parent; }

        public bool SetMain(kacTalk Main)
        {
            m_Main = Main;
            return true;
        }
        public kacTalk GetMain() { return m_Main; }


        #region properties
        public ktBlock Parent
        {
            get { return m_Parent; }
        }
        public kacTalk Main
        {
            get { return m_Main; }
        }
        public ktList Variables
        {
            get { return m_Variables; }
        }
        public ktList Lines
        {
            get { return m_Lines; }
            set { m_Lines = value; }
        }
        #endregion

        protected ktBlock m_Parent;
        protected kacTalk m_Main;
        protected ktList m_Lines;
        protected ktList m_Classes;
        protected ktList m_Functions;

        protected ktList m_Variables;

        protected bool m_skipNextLine = false;

        private bool m_enabledDebug = false;
        private int m_enabledAt = 0;
    }
}
