//#define Debug
//#define ParseDebug
//#define ParseDebug2
//#define RunDebug
//#define Debug2
//#define DebugXML
//#define DeepDebug
//#define ControlDebug

/*Jag vill lära dig och undervisa dig om den väg du skall vandra,
jag vill ge dig råd och låta mitt öga vaka över dig.
		Ps 32:8
Vad ni än gör, gör det av hjärtat, så som ni tjänar Herren och inte människor.
Ni vet att det är av Herren som ni skall få arvet som lön. Det är Herren Kristus ni tjänar.
		Kor 3:23-24
*/

// TODO: Fix so that a line like puts Math::sin Math::degToRad 42 ; works correctely

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace KacTalk
{
    /// <summary>
    /// kacTalk is the main/mother(ship) class, that controlls the intepretation etc. 
    /// </summary>
    public class kacTalk : ktIntObj
    {
        /// <summary>
        /// Main constructor for kacTalk,
        /// takes a parameter that decides if the default libraries/modules etc should be loaded
        /// </summary>
        public kacTalk(bool LoadDefs)
            : base("kactalk", 0)
        {
            m_Tokens = null;
            m_Lines = null;
            m_TokenStack = null;
            m_LineStack = null;
            m_CurLine = 0;

            m_MainBlock = null;

            if (LoadDefs)
            {
                LoadDefaults();
                AddDefaultValues();
                //ktDebug.Log( "===============\n"+ m_Modules.Get_R() );
            }
            m_MainKT = this;
        }
        /// <summary>
        /// Default constructor for kacTalk,
        /// loads the default libraries/modules etc
        /// </summary>
        public kacTalk() : this(true) { }

        /// <summary>
        /// Tries to load in and parse the script-/textfile defined by 'Filename'
        /// </summary>
        public bool LoadScript(ktString Filename)
        {
            try
            {
                FileInfo FileInfo = new FileInfo(Filename);
                FileStream FileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                StreamReader StreamReader = new StreamReader(FileStream);

                // Reduce the buffer to fit the filesize, if the buffer is less than the file  
                if ((int)FileInfo.Length < m_BufferSize)
                    m_BufferSize = (int)FileInfo.Length;

                char[] FileContents = new char[m_BufferSize];
                int BytesRead = StreamReader.Read(FileContents, 0, m_BufferSize);

                // Can't do much with 0 bytes  
                if (BytesRead == 0)
                {
                    throw new Exception("File is 0 bytes");
                }

                StringBuilder SB = new StringBuilder();
                while (BytesRead > 0)
                {
                    SB.Append(FileContents);
                    BytesRead = StreamReader.Read(FileContents, 0, m_BufferSize);
                }

                StreamReader.Close();
                FileStream.Close();

                return Scan(SB.ToString());
            }
            catch (DirectoryNotFoundException)
            {
                throw new ktError("Can't load the script-file '" + Filename +
                                  "'!", ktERR._404);
            }
        }

        /// <summary>
        /// Scan/Parse the script into tokens
        /// </summary>
        public bool Scan(ktString Script, ktString Name)
        {
            bool Ret = true;

            Script.Replace("\r\n", "\n",true);

            // Initiate the lineno.
            m_CurLine = 1;
            m_CharPos = 1;
              // .. and name
            m_Name = Name;

            if (m_Tokens != null)
            {
                m_Tokens.Clear();
                m_Tokens = null;
            }

            // Init...
            ktString Line = new ktString();
            int Pos = 0;
            Script.Trim();
            ktRegEx RE = new ktRegEx(ktToken.Separators);

            // ... the block-stack
            if (m_BlockStack != null)
            {
                m_BlockStack.Clear();
                m_BlockStack = null;
            }
            m_BlockStack = new ktList();
            // ... the token-list
            if (m_Tokens != null)
            {
                m_Tokens.Clear();
                m_Tokens = null;
            }
            m_Tokens = new ktList();
            m_Tokens.Node = new ktNode("ktTStatement", new ktToken(ktTokenType.Statement, "", 0, 0));
            // ... the token-stack
            if (m_TokenStack != null)
            {
                m_TokenStack.Clear();
                m_TokenStack = null;
            }
            m_TokenStack = new ktList();
            // ... the lines (??)
            if (m_Lines != null)
            {
                m_Lines.Clear();
                m_Lines = null;
            }
            m_Lines = new ktList();
            m_Lines.Node = new ktNode(Name, m_CurToken = new ktToken(ktTokenType.Program, Name, 0, 0));
            // ... the line-stack
            if (m_LineStack != null)
            {
                m_LineStack.Clear();
                m_LineStack = null;
            }
            m_LineStack = new ktList();

            if (m_MainBlock == null)
            {
                m_MainBlock = new ktBlock(new ktList());
            }
            else
            {
                if (m_MainBlock.Lines != null)
                {
                    m_MainBlock.Lines.Clear();
                }
                m_MainBlock.ClearKTSymbols();
            }
            m_MainBlock.SetMain(this);

            m_BlockStack.Add("ktTBlock", m_MainBlock);

            /* In the "original scanner" (C++), we took one line (terminated by \n)
             * 	at a time and worked on, now we just go through the line.
             * And We hope that it will be much "leaner"!
             */
            // Go on until the there's nothing left...
            while (!Script.IsEmpty())
            {
                // Get the position for the next separator
                Pos = RE.Find(Script);

                // If there was none...
                if (Pos < 0)
                {
                    // Take it to the end... 
                    Pos = Script.Len();
                }
                else if (Pos == 0)
                {
                    Pos++;
                }
                // Get the next "token"
                Line = Script.SubStr(0, Pos);

                // If it's the start of a comment 
                if ((Line == "/") && (Script.StartsWith("//") || Script.StartsWith("/*")))
                {
                    Line = Script.SubStr(0, 2);
                    Pos++;
                }
                else if ((Line == "*") && (Script.StartsWith("*/")))
                {
                    Line = "*/";
                    Pos++;
                }

                ReactOnToken(Line, m_CurLine, ref m_CharPos);

                if (Line == "\n")
                {
                    m_CurLine++;
                    m_CharPos = 1;
                }
                else
                {
                    m_CharPos += Line.Len();
                }

                // Remove the "token", we just worked on...
                Script.Remove(0, Pos);
            }

#if ParseDebug || DebugXML
            ktDebug.Log("XML111111:");
            ktDebug.Log(ktXML.FromList(m_TokenStack).AsXML());
            ktDebug.Log("==================");
            ktDebug.Log(ktXML.FromList(m_Tokens).AsXML());
#endif
            if (!m_Tokens.IsEmpty())
            {
                if (m_AllowMissingEOL)
                {
                    ((ktToken)m_Tokens.Node.Value).Type = ktTokenType.Line;
                    ((ktToken)m_Tokens.Node.Value).Name = m_Tokens.Node.Name = "ktTLine";
                    m_LineStack.AddList(m_Tokens);

                    m_Tokens = null;
                }
                else
                {
                    throw new ktError("Expected a ktTEOL at line " + m_CurLine.ToString() + " but didn't find one!",
                                      ktERR.MISSING);
                }
            }
            if (m_BlockStack.Count > 1)
            {
                throw new ktError("Expecting ktTEOB (}) at " + m_CharPos.ToString() + ", line " +
                                    m_CurLine.ToString() + ".", ktERR.MISSING);
            }

            //ktToken.OnlyExportValue = false;
            //ktDebug.Log( m_LineStack.Get_R(  ) );
            //ktToken.OnlyExportValue = false;
#if ParseDebug || DebugXML
			ktDebug.Log( "XML:" );
			ktDebug.Log( ktXML.FromList(m_LineStack).AsXML() );
			ktDebug.Log( "==================" );
			ktDebug.Log( ktXML.FromList(m_Lines).AsXML() );
#endif

            /*			ktDebug.Log( "?+++++\n" + m_Tokens.Get_R( "\t", true ) );
			ktDebug.Log( "?+++++\n" + m_CurToken.Export 
			if (m_CurToken != null) {
				if (m_CurToken.Type == ktTokenType.List) {
					throw new ktError( "Expected a ktTEndPar at line " + m_CurLine.ToString() + " but didn't find one!",
					                  ktERR.MISSING );
				} else if (m_CurToken.Type == ktTokenType.String) {
					throw new ktError( "Expected a ktTStringQuot at line " + m_CurLine.ToString() + " but didn't find one!",
					                  ktERR.MISSING );
				} else if (m_CurToken.Type == ktTokenType.Block) {
					throw new ktError( "Expected a ktTEndOfBlock at line " + m_CurLine.ToString() + " but didn't find one!",
					                  ktERR.MISSING );
				}
			} else if ((m_Tokens != null) && (!m_Tokens.IsEmpty())) {
				throw new ktError( "Expected a ktTEOL at line " + m_CurLine.ToString() + " but didn't find one!",
				                  ktERR.MISSING );
			}
*/
            //			MakeATree( );
            //			MakeAOpTree( );

            if ((m_BlockStack == null) || (m_BlockStack.IsEmpty()))
            {
                return Ret;
            }

            ktBlock Block = (ktBlock)(m_BlockStack.Pop().Node.Value);
#if ParseDebug
	ktDebug.Log( "BLOCK1:" + ktXML.FromList(Block.Lines).AsXML() );r
#endif

            MakeATree();
#if ParseDebug
            ktDebug.Log("BLOCK2:" + ktXML.FromList(m_Lines).AsXML());
#endif
            Block.Lines = MakeAOpTree();

            m_LineStack.Clear();
            m_LineStack = null;

            // Add Current "statement"/"post" to the block and theń switch them
            //Temp.AddList( m_Tokens );
            //m_Tokens = Temp;
#if ParseDebug
	ktDebug.Log( "BLOCK:" + ((Block == m_MainBlock) ? "MAIN":"NOT_MAIN") );
#endif
            /*ktList Temp = null;

			if (LastBlockLines == null) {
				throw new ktError( "No Last Block Lines!", ktERR.MISSING );
			}
			LastBlockLines.Add( "ktTBlock", new ktToken( Block, m_CurToken.LineNo, m_CurToken.CharPos ) );*/
#if ParseDebug || DebugXML
			ktDebug.Log( "XML_After Tree:" + ktXML.FromList(Block.Lines).AsXML() );
			ktDebug.Log( "XML_After Tree:" + Block.Lines.Get_R( "\t", true ) );
#endif
            //ktDebug.Log( m_Lines.Export() );


            return Ret;
        }
        /// <summary>
        /// Wrapper for ::Scan()
        /// </summary>
        public bool Scan(ktString Script) { return Scan(Script, ""); }
        /// <summary>
        /// Wrapper for ::Scan()
        /// </summary>
        public bool Parse(ktString Script, ktString Name) { return Scan(Script, Name); }
        /// <summary>
        /// Wrapper for ::Scan()
        /// </summary>
        public bool Parse(ktString Script) { return Parse(Script, ""); }

        /// <summary>
        /// Scans the text/code in 'Code' and returns it as a block (will not change the internaly stored block)
        /// </summary>
        public ktBlock ScanToBlock(ktString Code)
        {
            ktBlock Block = null;
            ktList Lines = m_Lines;
            m_Lines = null;

            try
            {
                Scan(Code);

                Block = new ktBlock(m_Lines.First);
                Block.SetMain(this);
            }
            catch (Exception Err)
            {
                m_Lines = Lines;
                throw Err;
            }

            m_Lines = Lines;

            return Block;
        }

        /// <summary>
        /// React on the "token" in Word
        /// </summary>
        protected bool ReactOnToken(ktString Word, int Line, ref int CharPos)
        {
            /* In the C++/"original" version of this method, it didn't do what
             *  it's called (_React_ On Token), instead it just added the token to the list.
             *  This method, hovever, does react properly to the given token
             *	This also reduces the number of steps for scanning/parsering (creating the op. tree)
             *	from three to two (we could probably just do it all in one step, but it would be messy
             * 		and a bit complex..)
             */

#if ParseDebug
	ktDebug.Log( "ReactOnToken(" + Word + ")" );
	ktDebug.Log( "BS:" + ktXML.FromList(m_BlockStack).AsXML() );
#endif

            ktToken Token = new ktToken(Word, Line, CharPos);
            ktToken LastToken = null;
            ktToken LastAddedToken = null;
            ktToken.OnlyExportValue = true;

            if ((m_TokenStack != null) && (m_TokenStack.Last != null) &&
                (m_TokenStack.Last.Node != null) && (m_TokenStack.Last.Node.Value != null))
            {
                LastToken = (ktToken)m_TokenStack.Last.Node.Value;
            }
            if ((m_Tokens != null) && (m_Tokens.Last != null) &&
                (m_Tokens.Last.Node != null) && (m_Tokens.Last.Node.Value != null))
            {
                LastAddedToken = (ktToken)m_Tokens.Last.Node.Value;
            }

            if (m_CurToken != null)
            {
                // React differentely deppendig on "current token/'state'"
                switch (m_CurToken.Type)
                {
                    // If it's an comment...
                    case ktTokenType.MLComment:
                        {
                            // ... Check if the token is EOC? ...
                            if (Token.Type == ktTokenType.EOC)
                            {
                                m_CurToken = (ktToken)(m_TokenStack.Pop().Node.Value);
                            }
                            // ... ignore...
                            return true;
                        }
                    // If it's an comment...
                    case ktTokenType.OLComment:
                        {
                            // ... Check if the token is EOC? ...
                            if (Word == "\n")
                            {
                                m_CurToken = (ktToken)(m_TokenStack.Pop().Node.Value);
                            }
                            // ... ignore...
                            return true;
                        }
                    // If it's a string...
                    case ktTokenType.String:
                    case ktTokenType.StringQuot:
                    case ktTokenType.StartString:
                        {
                            // If we are at end of the string?
                            if ((Token.Type == ktTokenType.StringQuot) &&
                                (m_CurToken.Value.Last() != '\\'))
                            {
                                ktList Temp = null;

                                // Add string...
                                m_Tokens.Add("ktTString", m_CurToken);

                                // If we can, put back the previous token...
                                if ((m_TokenStack != null) && ((Temp = m_TokenStack.Pop()) != null) &&
                                    (Temp.Node != null))
                                {
                                    m_CurToken = (ktToken)Temp.Node.Value;
                                }
                                else
                                {
                                    m_CurToken = null;
                                }

                                return true;
                            }

                            m_CurToken.Value += Word;

                            return true;
                        }
                }
            }
#if ParseDebug2
            /*Debug || */
           // if (Line == 4) { 
	ktDebug.Log( "Token:" + Token.Type.ToString() + " (" + Token.Value + ")" );
	ktDebug.Log( "CurToken:" + m_CurToken.Type.ToString() );
	ktDebug.Log( "BS:" + ktXML.FromList(m_BlockStack).AsXML() );
	ktDebug.Log( "Ts:" + ((m_TokenStack == null) ? "null" : m_TokenStack.Get_R().ToString() ));
	ktDebug.Log( "LS:" + ((m_LineStack == null) ? "null" : m_LineStack.Get_R( "\t", true ).ToString() ));
	ktDebug.Log( "Ls:" + ((m_Lines == null) ? "null" : m_Lines.Get_R( "\t", true ).ToString() ));
	ktDebug.Log( "ts:" + ((m_Tokens == null) ? "null" : m_Tokens.Get_R().ToString() ));
    //            }
#endif
            // Check the token..
            switch (Token.Type)
            {
                // Is it a whitespage??
                case ktTokenType.WhiteSpace:
                    {
                        // ... ignore...
                        return true;
                    }
                // Is it an id (var/function)?
                case ktTokenType.Id:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Is it a number?
                case ktTokenType.Number:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Is it a boolean (true/false)?
                case ktTokenType.Boolean:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Is it an separator (. or ::)?
                case ktTokenType.Separator:
                    {
                        if ((LastAddedToken != null) && (LastAddedToken.Value == ":") &&
                                (Token.Value == ":"))
                        {
                            LastAddedToken.Value = "::";
                        }
                        else
                        {
                            AddToken(Token);
                        }
                        return true;
                    }
                // Is it an operator?????
                case ktTokenType.Operator:
                case ktTokenType.AssignmentOperator:
                    {
                        // We should perhaps do some more here??
                        AddToken(Token);
                        return true;
                    }
                // Is it an const/hard?????
                case ktTokenType.Const:
                case ktTokenType.Hard:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Create a new object?
                case ktTokenType.New:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Null?
                case ktTokenType.Null:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Is it a If-statement?
                case ktTokenType.If:
                    {
                        AddToken(Token);
                        return true;
                    }
                // Is it a quot (")?
                case ktTokenType.StringQuot:
                    {
                        // No Token stack??
                        if (m_TokenStack == null)
                        {
                            // Create it...
                            m_TokenStack = new ktList();
                        }
                        // Add the current token to the stack
                        m_TokenStack.Add(m_CurToken);
                        // And Replace it with this "string token"
                        m_CurToken = new ktToken(ktTokenType.String, "", m_CurLine, m_CharPos);

                        return true;
                    }
                // Is it an start par. [(]?
                case ktTokenType.StartPar:
                    {
                        // No Token stack??
                        if (m_TokenStack == null)
                        {
                            // Create it...
                            m_TokenStack = new ktList();
                        }
                        // No Line Stack?
                        if (m_LineStack == null)
                        {
                            // Create it...
                            m_LineStack = new ktList();
                        }
                        // Add the current token to the stack
                        m_TokenStack.Add(m_CurToken);
                        // And Replace it with this "list token"
                        m_CurToken = new ktToken(ktTokenType.List, "ktList", m_CurLine, m_CharPos);

                        // Put the current ""line"" onto the stack
                        m_LineStack.AddList(m_Tokens);
                        // Create a new list...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode("ktList", m_CurToken);

                        // Put the current ""list"" onto the stack
                        m_LineStack.AddList(m_Tokens);
                        // Create a new list...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode("ktTStatement",
                                                    new ktToken(ktTokenType.Statement, "", m_CurLine, m_CharPos));

                        return true;
                    }
                // Is it an comma (,)?
                case ktTokenType.Comma:
                    {
                        if (m_CurToken.Type != ktTokenType.List)
                        {
                            throw new ktError("Unexpected " + ktToken.TokenToString(Token.Type) + " on line " +
                                                Line.ToString() + " by character " + CharPos.ToString() + ".", ktERR.UNEXP);
                        }

                        // get the last list
                        ktList List = m_LineStack.Last;

                        // Put the current ""statement"" onto the stack
                        List.AddList(m_Tokens);

                        // Create a new list...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode("ktStatement",
                                                new ktToken(ktTokenType.Statement, "", m_CurLine, m_CharPos));
                        return true;
                    }
                // Is it an end par. [)]?
                case ktTokenType.EndPar:
                    {
                        if (m_CurToken.Type != ktTokenType.List)
                        {
                            throw new ktError("Unexpected " + ktToken.TokenToString(Token.Type) + " on line " +
                                                Line.ToString() + " by character " + CharPos.ToString() + ".", ktERR.UNEXP);
                        }

                        // Get the last "statement"
                        ktList Temp = m_LineStack.Pop();

                        // Add Current "statement"/"post" to the list and then switch them
                        Temp.AddList(m_Tokens);
                        m_Tokens = Temp;

                        // Get the "last statement" (where the list was "defined") 
                        Temp = m_LineStack.Pop();
                        Temp.AddList(m_Tokens);
                        // "Switch"
                        m_Tokens = Temp;

                        // If we can, put back the previous token...
                        if ((m_TokenStack != null) && ((Temp = m_TokenStack.Pop()) != null) &&
                            (Temp.Node != null))
                        {
                            m_CurToken = (ktToken)Temp.Node.Value;
                        }
                        else
                        {
                            m_CurToken = null;
                        }
                        return true;
                    }

                // Is it an End-of-line (semicolon). (;)?
                case ktTokenType.EOL:
                    {
                        // Should we use m_Lines instead???
                        if (m_LineStack == null)
                        {
                            m_LineStack = new ktList();
                        }

                        // KacTalk special, runnable arguments!
                        // If we are in a list and finds a semicolon, it means
                        //  that that "post" should be treated as "runnable"
                        if (m_CurToken.Type == ktTokenType.List)
                        {
                            ktToken T = (ktToken)m_Tokens.Node.Value;

                            // Mark as "runnable"
                            T.Name = m_Tokens.Node.Name = "ktTRunStatement";
                            T.Type = ktTokenType.RunStatement;

                            // NOTE: basically the same as for comma (,) (see above)

                            // get the last list
                            ktList List = m_LineStack.Last;

                            // Put the current ""statement"" onto the stack
                            List.AddList(m_Tokens);

                            // Create a new list...
                            m_Tokens = new ktList();
                            m_Tokens.Node = new ktNode("ktTStatement",
                                                    new ktToken(ktTokenType.Statement, "", m_CurLine, m_CharPos));
                            return true;
                        }

                        ((ktToken)m_Tokens.Node.Value).Type = ktTokenType.Line;
                        ((ktToken)m_Tokens.Node.Value).Name = m_Tokens.Node.Name = "ktTLine";
                        m_LineStack.AddList(m_Tokens);

                        // Create a new list...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode("ktTStatement_",
                                                    new ktToken(ktTokenType.Statement, "", m_CurLine, m_CharPos));

                        return true;
                    }

                // Is it a block ({)!?
                case ktTokenType.Block:
                    {
                        // No Token stack??
                        if (m_TokenStack == null)
                        {
                            // Create it...
                            m_TokenStack = new ktList();
                        }
                        /*/ No Line Stack?
                        if (m_LineStack == null) {
                            // Create it...
                            m_LineStack = new ktList();
                        } else {
                            if (m_BlockStack == null) {
                                m_BlockStack = new ktList();
                            }
                            m_BlockStack.AddList( m_LineStack );
                        }*/
#if ParseDebug
   ktDebug.Log("BLOCK!!!");
#endif
                        if (m_BlockStack == null)
                        {
#if ParseDebug
	ktDebug.Log( "NEW_BLOCK_STACK" );
#endif
                            m_BlockStack = new ktList();
                            if (m_MainBlock == null)
                            {
                                m_MainBlock = new ktBlock();
                                m_MainBlock.SetMain(this);
                            }
                            m_BlockStack.Add(m_MainBlock);
                        }

                        // Put the current ""line"" onto the stack
                        m_LineStack.AddList(m_Tokens);
                        LastBlock.SetLines(m_LineStack);

                        ktBlock Block = new ktBlock(new ktList());
                        Block.SetMain(this);

                        m_BlockStack.Add("ktTBlock", Block);

                        m_LineStack = new ktList();

                        // Add the current token to the stack
                        m_TokenStack.Add(m_CurToken);
                        // And Replace it with this "block token"
                        m_CurToken = new ktToken(ktTokenType.Block, "ktTBlock", m_CurLine, m_CharPos);

                        // Create a new list...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode("ktTStatement.",
                                                    new ktToken(ktTokenType.Statement, "", m_CurLine, m_CharPos));

                        // Start a "new block"
                        /*m_LineStack = new ktList();

                        // Put the current ""line"" onto the stack
                        m_LineStack.AddList( m_Tokens );
                        // Create a new block...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode( "ktTBlock", m_CurToken );

                        // Put the current ""list"" onto the stack
                        m_LineStack.AddList( m_Tokens );
                        // Create a new list...
                        m_Tokens = new ktList();
                        m_Tokens.Node = new ktNode( "ktTLine",
                                                    new ktToken( ktTokenType.Line, "", m_CurLine, m_CharPos ) );*/

                        return true;
                    }
                // Is it an end of a block (})?
                case ktTokenType.EOB:
                case ktTokenType.End:
                    {
#if ParseDebug
	ktDebug.Log( "EOB;EOB;EOB;EOB;EOB;EOB;EOB;EOB;EOB;EOB;EOB;EOB;EOB;" );
    ktDebug.Log("CurrToken:" + m_CurToken.ToString());
#endif
                        if ((m_CurToken.Type != ktTokenType.Block) ||
                                (m_BlockStack == null) || (m_BlockStack.Last == null) ||
                                (m_BlockStack.Last.Node == null) || (m_BlockStack.Last.Node.Value == null))
                        {
                            throw new ktError("Unexpected " + ktToken.TokenToString(Token.Type) + " on line " +
                                                Line.ToString() + " by character " + CharPos.ToString() + ".", ktERR.UNEXP);
                        }

#if ParseDebug2
	ktDebug.Log( "ROT::B::BLS:" + ktXML.FromList(m_BlockStack).AsXML()  );
#endif
                        ktBlock Block = (ktBlock)(m_BlockStack.Pop().Node.Value);
#if ParseDebug2
	ktDebug.Log( "ROT::B::BLS2:" + ktXML.FromList(m_BlockStack).AsXML() );

	ktDebug.Log( "ROT::B::Tokens:" + m_Tokens.Get_R() );
	ktDebug.Log( "ROT::B::LineStack:" + m_LineStack.Get_R( "\t", true ) );
#endif

                        MakeATree();
#if ParseDebug2
	ktDebug.Log( "ROT::B::Lines:" + m_Lines.Get_R() );
#endif
                        Block.Lines = MakeAOpTree();
#if ParseDebug2
	ktDebug.Log( "ROT::Block:" + Block.ToString() );
	ktDebug.Log( "ROT::Block:Lines:" + Block.Lines.Get_R() );
#endif

                        m_LineStack.Clear();
                        m_LineStack = LastBlockLines;
                        //					m_Lines.Clear();
                        m_Lines = null;
#if ParseDebug2
	ktDebug.Log( "ROT::B::LineStack22:" + m_LineStack.Get_R( "\t", true ) );
#endif
                        /*Block.Lines = MakeATree( MakeATree( m_LineStack, true, false ), false, true );
					Block.Lines = MakeAOpTree( MakeAOpTree( MakeAOpTree( Block.Lines, 1 ), 2 ), 3 )*/
                        m_Tokens.Node.Name = "ktTBlock";
                        ((ktToken)m_Tokens.Node.Value).Name = "ktTBlock";
                        ((ktToken)m_Tokens.Node.Value).Type = ktTokenType.Block;

                        // Add Current "statement"/"post" to the block and then switch them
                        //Temp.AddList( m_Tokens );
                        //m_Tokens = Temp;

                        ktList Temp = null;

                        if (LastBlockLines == null)
                        {
                            throw new ktError("No Last Block Lines!", ktERR.MISSING);
                        }
                        LastBlockLines.Add("ktTBlock", new ktToken(Block, m_CurToken.LineNo, m_CurToken.CharPos));
                        // "Switch"
                        //m_Tokens = Temp;
#if ParseDebug2
	ktDebug.Log( "ROT::B::LastBlockLines:" + LastBlockLines.Get_R( "\t", true ) );
#endif

                        // If we can, put back the previous token...
                        if ((m_TokenStack != null) && ((Temp = m_TokenStack.Pop()) != null) &&
                            (Temp.Node != null))
                        {
                            m_CurToken = (ktToken)Temp.Node.Value;
                        }
                        else
                        {
                            m_CurToken = null;
                        }
                        return true;
                    }   

                // Comments...
                // One line comment??
                //	(bash only indicates comments if it's the first thing on the line...)
                case ktTokenType.Bash:
                    {
                        // If it's the first thing on the line!?
                        if (m_CharPos <= 1)
                        {
                            // Indicate one line comment...
                            // No Token stack??
                            if (m_TokenStack == null)
                            {
                                // Create it...
                                m_TokenStack = new ktList();
                            }
                            // Add the current token to the stack
                            m_TokenStack.Add(m_CurToken);
                            // And Replace it with this "commeent token"
                            m_CurToken = new ktToken(ktTokenType.OLComment, "ktTOLComment", m_CurLine, m_CharPos);
                            return true;
                            // Not First at the line...
                        }
                        else
                        {
                            // What should we do??
                            // Exception, AddToken or somee magic for ids!???

                            // Unexpected...
                            throw new ktError("Unexpeected " + ktToken.TokenToString(Token.Type) + " on line " +
                                                m_CurLine.ToString() + " by character " + m_CharPos.ToString(), ktERR.UNEXP);
                        }
                    }
                // One line comment
                case ktTokenType.OLComment:
                    {
                        // No Token stack??
                        if (m_TokenStack == null)
                        {
                            // Create it...
                            m_TokenStack = new ktList();
                        }
                        // Add the current token to the stack
                        m_TokenStack.Add(m_CurToken);
                        // And Replace it with this "commeent token"
                        m_CurToken = new ktToken(ktTokenType.OLComment, "ktTOLComment", m_CurLine, m_CharPos);
                        return true;
                    }
                // Multi line comment
                case ktTokenType.MLComment:
                    {
                        // No Token stack??
                        if (m_TokenStack == null)
                        {
                            // Create it...
                            m_TokenStack = new ktList();
                        }
                        // Add the current token to the stack
                        m_TokenStack.Add(m_CurToken);
                        // And Replace it with this "Comment token"
                        m_CurToken = new ktToken(ktTokenType.MLComment, "ktTMLComment", m_CurLine, m_CharPos);
                        return true;
                    }
                // If it's an End of comment
                case ktTokenType.EOC:
                    {
                        // Unexpected...
                        throw new ktError("Unexpeected " + ktToken.TokenToString(Token.Type) + " on line " +
                                            m_CurLine.ToString() + " by character " + m_CharPos.ToString(), ktERR.UNEXP);
                    }
            }

            // To see if we missed any tokens...
            ktToken.OnlyExportValue = false;
            ktDebug.Log("IGNmORED:" + Token.Export() + "\n");

            return true;
        }
        /// <summary>
        /// Wrapper for ::ReactOnToken
        /// </summary>
        protected bool ReactOnToken(ktString Word)
        {
            int CharPos = 0;
            return ReactOnToken(Word, 0, ref CharPos);
        }
        /// <summary>
        /// Add a token to the tokenlist
        /// </summary>
        protected bool AddToken(ktToken Token)
        {
            /**/
            if (m_Tokens == null)
            {
                m_Tokens = new ktList();
            }

            return m_Tokens.Add(Token.Name, Token);
            /*/
            if (LastBlockLines == null) {
                if (LastBlock == null) {
                    throw new ktError( "No Last Block!", ktERR.MISSING );
                }
                LastBlock.SetLines( new ktList() );
            }

            return LastBlockLines.Add( Token.Name, Token );/**/
        }

        /// <summary>
        /// Parse the tokens and create a tree /*(mainly for operators)**/
        /// </summary>
        protected ktList MakeATree(ktList List, bool FirstRun, bool ThirdRun)
        {
            //ktDebug.Log( "MAT!" + List.Get_R( "\t", true ));
            ktList Tree = new ktList(), Prev = null, Next = null, Temp = null, Temp2 = null;
            ktToken Token = null, PrevToken = null;

            /*/ 	If the list are the "root"
            if (List == m_LineStack) {
                Tree.Node = new ktNode( "ktTBlock",  new ktToken( ktTokenType.Block, "", 0, 0 )  );
            } else/**/
            {
                Tree.Node = List.Node;
            }

            List.Reset();
            foreach (ktList L in List)
            {
                //ktDebug.Log( "\nLLL: "+ List.Get_R() /*+ " :" +L.Export() */);
                if ((L.Node != null) && (L.Node.Value != null))
                {
                    Token = (ktToken)L.Node.Value;
                    Prev = Tree.Last;
                    if ((Prev != null) && (Prev.Node != null) && (Prev.Node.Value != null))
                    {
                        PrevToken = (ktToken)Prev.Node.Value;
                    }
                    else
                    {
                        PrevToken = new ktToken(ktTokenType.NULLTOKEN, ":null", 0, 0);
                    }
                }
                else
                {
                    continue;
                }
//#if ParseDebug
    ktDebug.Log( "TT:" + Token.Type.ToString() + "(" + Token.Value + ")" );
    ktDebug.Log( "PTT:" + PrevToken.Type.ToString() + "(" + PrevToken.Value + ")" );
//#endif
                if ((FirstRun) && (
                        ((PrevToken.Type == ktTokenType.Const) || (PrevToken.Type == ktTokenType.Hard)) ||
                        ((Token.Type == ktTokenType.Const) ||  (Token.Type == ktTokenType.Hard)) ||
                        ((PrevToken.Type == ktTokenType.VarStatement) && (
                            (Token.Type != ktTokenType.AssignmentOperator)/* ||
				        	(Token.Type != ktTokenType.)*/
                         )
                        )))
                {
                    //ktDebug.Log( "CONSTCONSTCONSTCONSTCONSTCONSTCONSTCONSTCONST" );
                    if (PrevToken.Type == ktTokenType.VarStatement)
                    {
                        if ((Token.Type != ktTokenType.Id) && (Token.Type != ktTokenType.Hard) &&
                            (Token.Type != ktTokenType.Const))
                        {
                            throw new ktError("Unexpected " + Token.Name + " on line " + Token.LineNo.ToString() +
                                              " by character " + Token.CharPos.ToString() + "!", ktERR.UNEXP);
                        }
                        Prev.Add(Token);
                    }
                    else
                    {
                        Temp = new ktList();
                        Temp.Node = new ktNode("ktTVarStatement", new ktToken(ktTokenType.VarStatement, "", Token.LineNo, Token.CharPos));

                        Temp.Add(Token);
                        //						Temp.AddList( MakeATree( L, FirstRun ) );

                        Tree.AddList(Temp);
                    }
                }
                else if (
                   (
                      ((PrevToken.Type == ktTokenType.Id) && (ThirdRun)) ||
                      ((/*!*/FirstRun) && (
                          (PrevToken.Type == ktTokenType.CompStatement) &&
                              (
                                  (PrevToken.Value == ".") ||
                                  (PrevToken.Value == "::")
                              )
                      )
                   )) && (
                      (Token.Type == ktTokenType.List) ||
                      (Token.Type == ktTokenType.Statement) ||
                    //				    	(Token.Type == ktTokenType.CompStatement) ||
                      (Token.Type == ktTokenType.Const) ||
                      (Token.Type == ktTokenType.Hard) ||
                      (Token.Type == ktTokenType.String) ||
                      (Token.Type == ktTokenType.Boolean) ||
                      (Token.Type == ktTokenType.Number) ||
                      (Token.Type == ktTokenType.Float) ||
                      (Token.Type == ktTokenType.Null) ||
                      (Token.Type == ktTokenType.Id) // ?? Not shure on how this actually will work!?
                   ))
                {
                    ktDebug.Log("Tree:" + Tree.Get_R());
                    Tree.Pop(false);
 /*   ktDebug.Log("TToxen:" + Token.ToString());
    ktDebug.Log("PToxen:" + PrevToken.ToString());*/
                    if ( (PrevToken.Value == ".") ) {
                        if (Token.Type == ktTokenType.List)
                        {
                            Temp = MakeATree(L, FirstRun, ThirdRun);

                            Prev.Last.AddList(Temp);
                        }
                        else
                        {
                            Prev.Last.AddList(new ktList(L));
                        }
                        ((ktToken)Prev.Node.Value).Value = "§";
                        Tree.AddList(Prev);

                        //((ktToken)Tree.Node.Value).Value = "§";

                        ktDebug.Log("Tree:" + Tree.Get_R());
                    }
                    else if ((Prev.Prev == null) || ((Prev.Prev.Node != null) && (Prev.Prev.Node.Value != null) && (((ktToken)Prev.Prev.Node.Value).Type != ktTokenType.Id)))
                    {
                        Temp = new ktList();
                        Temp.Node = new ktNode("ktCompStatement", new ktToken(ktTokenType.CompStatement, "F", PrevToken.LineNo, PrevToken.CharPos));

                        if (Token.Type == ktTokenType.List)
                        {
                            Temp2 = MakeATree(L, FirstRun, ThirdRun);
                            //Temp2.Node.Value = Token;

                            Temp.AddList(Prev);  // Add function name
                            Temp.AddList(Temp2); // Add arguments
#if ParseDebug
                            ktDebug.Log("FComp_List:\n" + Temp.Get_R());
                            ktDebug.Log("FComp_List T2:\n" + Temp2.Get_R());
#endif
                        }
                        else
                        {
                            Temp.AddList(Prev); // Add function name
                            Temp.AddList(new ktList(L));    // Add argument
#if ParseDebug
                            ktDebug.Log("FComp_NoList:\n" + Temp.Get_R());
#endif
                        }
                        Tree.AddList(Temp);
#if ParseDebug
                        ktDebug.Log("FComp Tree:\n" + Tree.Get_R());
#endif
                    }
                    else
                    {
                        Temp = new ktList();
                        Temp.Node = new ktNode("ktTCompStatement", new ktToken(ktTokenType.CompStatement, "§", PrevToken.LineNo, PrevToken.CharPos));
//ktDebug.Log("§§§§§§§§§§§§§§§§§§§§§§§§§§§");
                        if (Token.Type == ktTokenType.List)
                        {
                            Temp2 = new ktList();
                            Temp2.Node = new ktNode("ktTCompStatement", new ktToken(ktTokenType.CompStatement, ".", PrevToken.LineNo, PrevToken.CharPos));

                            Temp2.AddList(Prev.Last); // Add Method
                            Temp2.AddList(MakeATree(L, FirstRun, ThirdRun)); // Add Arguments

                            Temp.AddList(Prev.First); // Add object
                            Temp.AddList(Temp2); // Add Method call


                            Tree.AddList(Temp);
#if Debug
                            ktDebug.Log("L_Temp2:\n" + Temp2.Get_R());
                            ktDebug.Log("L_Temp:\n" + Temp.Get_R());
                            ktDebug.Log("L_Tree:\n" + Tree.Get_R());
#endif
                        }
                        else
                        {
                            //Temp.AddList(Prev);
                            //Temp.AddList(MakeATree(L, FirstRun, ThirdRun));
                            Prev.AddList(MakeATree(L, FirstRun, ThirdRun));
                           /* ktDebug.Log("Temp:" + Temp.Get_R());
                            ktDebug.Log("Prev:" + Prev.Get_R());*/
                            //  Tree.AddList(Temp);
                            Tree.AddList(Prev);
                            //ktDebug.Log("Tree:" + Tree.Get_R());
                        }
                    }
                }
                else if (
                          (ThirdRun) &&
                          (
                              (PrevToken.Type == ktTokenType.Id) ||
                              (PrevToken.Type == ktTokenType.CompStatement)
                          ) &&
                          (Token.Type == ktTokenType.CompStatement)
                   )
                {
#if ParseDebug2
	ktDebug.Log( "PREV:"+ PrevToken.Value );
	ktDebug.Log( "LLLL:"+ L.Get_R() );
	ktDebug.Log( "NNN:"+ ((Next == null) ? "nulL" : Next.Get_R().ToString()) );
#endif
                    Tree.Remove((uint)(Tree.Count - 1));

                    Temp = new ktList();
                    Temp.Node = new ktNode("ktTCompStatement", new ktToken(ktTokenType.CompStatement, "$", PrevToken.LineNo, PrevToken.CharPos));

                    Temp.AddList(Prev);
                    Tree.AddList(new ktList(L));

                    Tree.AddList(Temp);
                }
                else if ((Token.Type == ktTokenType.Line) ||
                  (Token.Type == ktTokenType.List) ||
                  (Token.Type == ktTokenType.Statement) ||
                  (Token.Type == ktTokenType.RunStatement) /*||
				    (Token.Type == ktTokenType.New)*/)
                {
#if ParseDebug2
	ktDebug.Log( "\nMAT::MAT::MAT::MAT::MAT::MAT::MAT::MAT::\n" +L.Get_R() );
#endif
                    /*if ((!FirstRun) && (PrevToken != null) && (
					    (PrevToken.Type == ktTokenType.Id) ||
						(PrevToken.Type == ktTokenType.CompStatement) ||
						(PrevToken.Type == ktTokenType.Statement))) {
							Tree.Remove( (uint)(Tree.Count - 1) );

							Temp = new ktList( );
							Temp.Node = new ktNode( "ktTCompStatement", new ktToken( ktTokenType.CompStatement, Token.Value, Token.LineNo, Token.CharPos ) );

							Temp.AddList( Prev );
							Temp.AddList( MakeATree( L, FirstRun ) );

							Tree.AddList( Temp );
					} else {*/
                    Tree.AddList(MakeATree(L, FirstRun, ThirdRun));
                    /*}*/
                }
                else if ((Token.Type == ktTokenType.Separator) && FirstRun)
                {
                    Prev = L.Prev;
                    Next = L.Next;

                    //					Tree.Remove( (uint)(Tree.Count - 1) );

                    if ((Next != null) && (Next.Node != null) && (Next.Node.Value != null) &&
                            (Token.Value == ":") &&
                             (((ktToken)Next.Node.Value).Type == ktTokenType.Null))
                    {
                        Token = (ktToken)Next.Node.Value;
                        Token.Value = ":null";

                        Tree.Add("ktTNull", Token);

                        List.MoveNext();
                        List.MoveNext();
                        continue;
                    }
#if ParseDebug2
	ktDebug.Log( "MakeATree calling HandleSep!" );
#endif
                    HandleSep(Prev, Next, ref Tree, Token);

                    List.MoveNext();
                }
                else if ((FirstRun) && (
                          (Token.Type == ktTokenType.Operator) ||
                          (Token.Type == ktTokenType.AssignmentOperator)
                      ) && (
                          (PrevToken.Type == ktTokenType.Operator) ||
                          (PrevToken.Type == ktTokenType.AssignmentOperator)
                      ))
                {
                    if ( (Token.Value == "=") || (PrevToken.Value == "=") )
                    {
                        switch (PrevToken.Value)
                        {
                            case "=": case "!": case ">": case "<":
                                {
                                    //   PrevToken.Value = "==";
                                    if (PrevToken.Value == "=")
                                        PrevToken.Value.Append(Token.Value);
                                    else
                                        PrevToken.Value.Append("=");
                                    PrevToken.Type = ktTokenType.ComparisonOperator;
                                    Prev.Node.Name = PrevToken.Name = ktToken.TokenToString(ktTokenType.ComparisonOperator);
                                    break;
                                }
                            case "+": case "-": case "*": case "/": case "%":
                            case "&": case "|": case "~": case "^":
                                {
                                    if (PrevToken.Value == "=")
                                        PrevToken.Value.Append(Token.Value);
                                    else
                                        PrevToken.Value.Append("=");
                                    PrevToken.Type = ktTokenType.AssignmentOperator;
                                    Prev.Node.Name = PrevToken.Name = ktToken.TokenToString(ktTokenType.AssignmentOperator);
                                    break;
                                }
                            default:
                                {
                                    throw new ktError("Unknown operator '" + PrevToken.Value + Token.Value +
                                                        "' on line " + PrevToken.LineNo.ToString() +
                                                        " by character " + PrevToken.CharPos.ToString() + "!",
                                                      ktERR.UNKNOWN);
                                }
                        }
                    } else if ( PrevToken.Value == Token.Value )
                    {
                        PrevToken.Value.Append(Token.Value);
                        PrevToken.Type = ktTokenType.Operator;
                        Prev.Node.Name = PrevToken.Name = ktToken.TokenToString(ktTokenType.Operator);
                    }
                    else
                    {
                        throw new ktError("Unknown operator '" + PrevToken.Value + Token.Value +
                                            "' on line " + PrevToken.LineNo.ToString() +
                                            " by character " + PrevToken.CharPos.ToString() + "!",
                                          ktERR.UNKNOWN);
                    }
                    /*Prev = L.Prev;
                    Next = L.Next;

                    HandleSep( Prev, Next, ref Tree, Token );

                    List.MoveNext();*/
                }
                else if ((ThirdRun) && (PrevToken.Type == ktTokenType.New))
                {
#if ParseDebug2
	ktDebug.Log( "NEW_PREV:"+ PrevToken.Value );
	ktDebug.Log( "NEW_LLLL:"+ L.Get_R() );
	ktDebug.Log( "NEW_NNN:" + ((Next == null) ? "null" : Next.Get_R().ToString()) );
#endif
                    Tree.Remove((uint)(Tree.Count - 1));

                    Temp = new ktList();
                    Temp.Node = new ktNode("ktTNewStatment", PrevToken);

                    //					Temp.AddList( Prev );
                    Temp.AddList(GetTheRest(List));

                    Tree.AddList(Temp);
                    break;
                }
                else
                {
                    Tree.AddList(new ktList(L));
                }
            }
//#if ParseDebug2
	ktDebug.Log( "+++++++++++++++++++++" + Tree.Get_R( "\t", true ) + "-----------__");
//#endif

            if (List == m_LineStack)
            {
                Tree = MakeATree(Tree, false, false);
            }

            return Tree;
        }
        /// <summary>
        /// Parse the tokens and create a tree/* (mainly for operators)*/
        /// </summary>
        protected ktList MakeATree()
        {
            if (m_Lines == null)
            {
                m_Lines = new ktList();
                //m_Lines.Node = new ktNode( m_Name, new ktToken( ktTokenType.Program, m_Name, 0, 0 ) );
            }

            ktList Tree = MakeATree(m_LineStack, true, false);
#if ParseDebug
ktDebug.Log( "MAT_TREE1:" + Tree.Get_R( " ", true ) );
#endif
            /*
			Tree = MakeATree( Tree, false, false );
ktDebug.Log( "MAT_TREE2:" + Tree.Get_R( "\t", true ) );*/
            Tree = MakeATree(Tree, false, true);
#if ParseDebug
	ktDebug.Log( "MAT_TREEEEEEEE:\n" + Tree.Get_R( " ", true ) );
#endif

            /**/
            m_Lines = Tree;/*/
			m_Lines.AddList( Tree );/**/
#if ParseDebug
	ktDebug.Log( "MAT_LINES:" + m_Lines.Get_R( " ", true ) );
#endif

            return m_Lines;
        }

        /// <summary>
        /// Handle "separators"
        /// </summary>
        protected void HandleSep(ktList First, ktList Second, ref ktList Tree, ktToken Token)
        {
            ktDebug.Log("HASEP: T:" + Token.ToString() + "; F:" + First.ToString());
            if ((First == null) || (First.Node == null) || (First.Node.Value == null) ||
                (Second == null) || (Second.Node == null) || (Second.Node.Value == null))
            {
                throw new ktError("Unexpected ktTSeparator (" + Token.Value +
                                    ") on line " + Token.LineNo.ToString() +
                                    " by character " + Token.CharPos.ToString() + ".",
                                    ktERR.UNEXP);
            }
            ktList Temp = null;
            ktList Last = Tree.Last;
            ktToken FirstToken = (ktToken)First.Node.Value;
            ktToken SecondToken = (ktToken)Second.Node.Value;
            ktToken LastToken = null;
            ktDebug.Log( "HASEP:" + SecondToken.ToString() );/*
                        if ((Token.Value == ":") && (SecondToken.Value == ":") ) {
                            //T
                        } else */
            //ktDebug.Log( "HASEP:: FT:" + FirstToken.ToString() + "; ST:" + SecondToken.ToString() );
            if ( (FirstToken.Type == ktTokenType.Id) &&
                    ( SecondToken.Type == ktTokenType.AssignmentOperator ) &&
                    ( Token.Value == ":" ) ) {
                LastToken = new ktToken(ktTokenType.AssignmentOperator, ":=", FirstToken.LineNo, FirstToken.CharPos, ktToken.TokenToString(ktTokenType.AssignmentOperator));

                Tree.Remove((uint)(Tree.Count - 1));
                Tree.AddList(Temp);
ktDebug.Log("TREE:" + Tree.Get_R("\t", true));
                return;
            } else if ((FirstToken.Type != ktTokenType.Id) && (FirstToken.Type != ktTokenType.List) &&
                (FirstToken.Type != ktTokenType.CompStatement) && (FirstToken.Type != ktTokenType.Boolean) &&
                (FirstToken.Type != ktTokenType.Float) && (FirstToken.Type != ktTokenType.Number) &&
                (FirstToken.Type != ktTokenType.String) ||
                (
                 (SecondToken.Type != ktTokenType.Id) &&
                 (SecondToken.Type != ktTokenType.CompStatement) &&
                 (SecondToken.Type != ktTokenType.Number)
                ))
            {
                throw new ktError("Unexpected ktTSeparator (" + Token.Value +
                                    ") on line " + Token.LineNo.ToString() +
                                    " by character " + Token.CharPos.ToString() + ".",
                                    ktERR.UNEXP);
            }
#if DeepDebug
#if ControlDebug
Console.ReadLine();
#endif
#endif

            if ((Last != null) && (Last.Node != null) && (Last.Node.Value != null) &&
                    (Last.Node.Value.GetType() == typeof(ktToken)))
            {
                LastToken = (ktToken)Last.Node.Value;
            }

//#if ParseDebug2
	ktDebug.Log( "FIRST(" + FirstToken.ToString() + "):\n" + First.Get_R( "\t", true ) + "\nSECOND(" + SecondToken.ToString() + "):\n" + Second.Get_R( "\t", true ) );
	ktDebug.Log( "TREE:\n" + Tree.Get_R( "\t", true ) );
	if (LastToken != null) {
		ktDebug.Log( "LAST(" + LastToken.ToString() + "):\n" + Last.Get_R( "\t", true ) );
	  }
//#endif
            if ((FirstToken.Type == ktTokenType.Number) && (SecondToken.Type == ktTokenType.Number))
            {
                //((ktToken)Tree.Last.Node.Value).Value += "." + SecondToken.Value;
                Tree.Remove((uint)(Tree.Count - 1));

                FirstToken.Value += "." + SecondToken.Value;
                FirstToken.Type = ktTokenType.Float;
                FirstToken.Name = "ktTFloat";
                Tree.Add(FirstToken);
            }
            else if ((Last != null) && (LastToken.Type == ktTokenType.CompStatement))
            {
                Tree.Pop();

                Temp = new ktList();
                Temp.Node = new ktNode("ktTCompStatement", new ktToken(ktTokenType.CompStatement, Token.Value, Token.LineNo, Token.CharPos));

                Temp.AddList(new ktList(Last));
                Temp.AddList(new ktList(Second));

#if DeepDebug
	ktDebug.Log( "LCT:" + Temp.Get_R( "\t", true ) );
#endif

                Tree.AddList(Temp);
            }
            else 
            {
                Tree.Pop();

                Temp = new ktList();
                Temp.Node = new ktNode("ktTCompStatement", new ktToken(ktTokenType.CompStatement, Token.Value, Token.LineNo, Token.CharPos));

                Temp.AddList(new ktList(First));
                Temp.AddList(new ktList(Second));

//#if DeepDebug
	ktDebug.Log( "T:" + Temp.Get_R( "\t", true ) );
//#endif

                Tree.AddList(Temp);
            }
#if DeepDebug
	ktDebug.Log( "=========================================" );
#if ControllDebug
	Console.ReadLine();
#endif
#endif
        }


        /// <summary>
        /// Parse the tokens and create a optree
        /// </summary>
        protected ktList MakeAOpTree(ktList List, int RunNumb)
        {
            ktList Tree = new ktList(), Prev = null, Temp = null;
            ktToken Token = null, PrevToken = null;
            bool result = false, SkipNext = false, ReturnTree = false;

//            ktDebug.Log("MAOT!! [" + RunNumb + "]\n" + List.Get_R("\t") );
            // null is null..
            if (List == null)
            {
                return null;
            }

            ktDebug.WrapLevel++;
            // Same, same, same...
            Tree.Node = List.Node;
            if ((Tree.Node == null) || (Tree.Node.Value == null) ||
                    (Tree.Node.Value.GetType() != typeof(ktToken)))
            {
                if (Tree.Node == null)
                {
                    Tree.Node = new ktNode("ktBlock");
                }
                Tree.Node.Value = new ktToken(ktTokenType.Block, "ktBlock", 0, 0);
            }
            ((ktToken)Tree.Node.Value).RunnedStep = RunNumb;

//#if ParseDebug
           ktDebug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\nMAOT(" + RunNumb + "):" + List.Get_R());
//#endif

            List.Reset();
            foreach (ktList L in List)
            {
                SkipNext = false;

                // Check so we don't try to work on nothing!
                if ((L.Node != null) && (L.Node.Value != null))
                {
                    // Get Token and previous...
                    Token = (ktToken)L.Node.Value;
                    Prev = Tree.Last;
                    // Get previous token if possible
                    if ((Prev != null) && (Prev.Node != null) && (Prev.Node.Value != null))
                    {
                        PrevToken = (ktToken)Prev.Node.Value;
                    }
                    else
                    {
                        PrevToken = new ktToken(ktTokenType.NULLTOKEN, ":null", 0, 0);
                    }
                    // Nothing here??
                }
                // Nothing there? ...
                else
                {
                    // ... Lets continue!
                    continue;
                }

                //ktDebug.Log("LLLLLLLLLL:::\n" + L.Get_R());
                //ktDebug.Log("LiLiLiLiLiLiLiLiLiLi:::\n" + List.Get_R());
                
//#if ParseDebug3
                ktDebug.Log("opTT:" + Token.Type.ToString() + "(" + Token.Value + ")");
                ktDebug.Log("opPTT:" + PrevToken.Type.ToString() + "(" + PrevToken.Value + ")");
//#endif
                // On run/pass 1
                if (RunNumb == 1)
                {
                    // Handle run 1
                    result = MakeAOpTree_Run1(PrevToken, Prev, Token, L, ref Tree, ref List, out SkipNext, out ReturnTree);
                }
                // On run/pass 2
                else if (RunNumb == 2)
                {
                    result = MakeAOpTree_Run2(PrevToken, Prev, Token, L, ref Tree, ref List, out SkipNext, out ReturnTree);
                }
                // On run/pass 3
                else if (RunNumb == 3)
                {
                    result = MakeAOpTree_Run3(PrevToken, Prev, Token, L, ref Tree, ref List, out SkipNext, out ReturnTree);
                }

                // If nothing was done ...
                if (!result)
                {
                    if ((Token.Type == ktTokenType.Id) || (Token.Type == ktTokenType.Null) ||
                         (Token.Type == ktTokenType.Number) || (Token.Type == ktTokenType.Float) ||
                         (Token.Type == ktTokenType.Boolean) ||
                         (Token.Type == ktTokenType.String)/* ||
                          (Token.Type == ktTokenType.If)*/ )
                    {
                        //ktDebug.Log("Constant etc!!(" + Token.Value + ")");
                        Temp = new ktList();
                        Temp.Node = L.Node;
                        ((ktToken)Temp.Node.Value).RunnedStep = RunNumb;
                        Tree.AddList(Temp);
                    }
                    else if (Token.Type == ktTokenType.If)
                    {
                        //ktDebug.Log("IF-TOKEN!");
                        Tree.AddList(new ktList(L));
                    }
                    else
                    {
                        if ((Token.Type == ktTokenType.Statement) && (L.Count == 0))
                        {
                            //ktDebug.Log("MAOT(" + RunNumb + "): SKIP(Statement)");
                            continue;
                        }

                        Temp = MakeAOpTree(L, RunNumb);
                        ktDebug.Log("MAOT(" + RunNumb + "): TEMP:\n" + Temp.Get_R());

                        if ((Token.Type == ktTokenType.Statement) && (((ktToken)Temp.FirstNode.Value).Type == ktTokenType.If))
                        {
                            Temp = Temp.First;
                        }

                        if ((Token.Type == ktTokenType.Line) &&
                            (!PrevToken.HasBlock) &&
                            ((PrevToken.Type == ktTokenType.If) ||
                              (PrevToken.Type == ktTokenType.Else) ||
                              (PrevToken.Type == ktTokenType.ElseIf)
                            ))
                        {
                            /*ktDebug.Log("ISLINE:PREVIF!");
                            ktDebug.Log("TEMP: " + Temp.Get_R());*/
                            Prev.AddList(Temp);
                        }
                        else
                        {
                            Tree.AddList(Temp);
                        }
                    }
                }
                //ktDebug.Log("MAOT(" + RunNumb + "): TREE:\n" + Tree.Get_R());



                // Should we return the tree we have created??
                if (ReturnTree)
                {
                    // Return the tree and be done with this part!
                    return Tree;
                }

                // Should we skip this one?
                if (SkipNext)
                {
                    SkipNext = false;
                    List.MoveNext();
                }
                continue;

// OLD CODE FOR REFERENCE:
                if ((RunNumb == 1) && (Token.Type == ktTokenType.Block))
                {
                    if (((PrevToken.Type == ktTokenType.If) ||
                            (PrevToken.Type == ktTokenType.Else) ||
                            (PrevToken.Type == ktTokenType.ElseIf)
                        ) &&
                        (!PrevToken.HasBlock))
                    {
                        /*Tree.Last.AddList(new ktList(L));
                        ((ktToken)Tree.Last.Node.Value).HasBlock = true;*/
                        Prev.AddList(new ktList(L));
                        PrevToken.HasBlock = true;
                    }
                    else
                    {
                        Tree.AddList(new ktList(L));
                    }
                }
                else if ((Token.Type == ktTokenType.Id) || (Token.Type == ktTokenType.Null) ||
                          (Token.Type == ktTokenType.Number) || (Token.Type == ktTokenType.Float) ||
                          (Token.Type == ktTokenType.Boolean) ||
                          (Token.Type == ktTokenType.String)/* ||
                          (Token.Type == ktTokenType.If)*/ )
                {
                    //ktDebug.Log("Constant etc!!(" + Token.Value + ")");
                    Temp = new ktList();
                    Temp.Node = L.Node;
                    ((ktToken)Temp.Node.Value).RunnedStep = RunNumb;

                    if (L.GetCount() != 0)
                    {
                        Temp.AddList(MakeAOpTree(L, RunNumb));
                    }

                    Tree.AddList(Temp);
                }
                else if (Token.Type == ktTokenType.If)
                {
                    //ktDebug.Log("IF-TOKEN!");
                    Tree.AddList(new ktList(L));
                }
                else if ((RunNumb == 1) && (PrevToken.Type == ktTokenType.If) && (!PrevToken.HasBlock) && (RunNumb == 1) &&
                            ((Token.Type == ktTokenType.Statement) || (Token.Type == ktTokenType.List)) )
                {
                        ktList ifL = Tree.Pop();
                        Temp = new ktList();
                        Temp.Node = ifL.Node;
                        ((ktToken)Temp.Node.Value).RunnedStep = RunNumb;
                        Temp.AddList(MakeAOpTree(L, RunNumb));

#if ParseDebug
                        ktDebug.Log("MAOT(" + RunNumb + "): IF-statement: " + Temp.Get_R());
#endif

                        Tree.AddList(Temp);
#if ParseDebug
                    ktDebug.Log("MAOT(" + RunNumb + "): IF TREE: " + Tree.Get_R());
#endif
                }
            }
#if ParseDebug
	ktDebug.Log( "MAOTREE(" + RunNumb + "):\n" + Tree.Get_R( "\t", true ) );
	ktDebug.WrapLevel--;
	if (this.m_enabledDebug && (this.m_enabledAt == ktDebug.WrapLevel)) { ktDebug.D.Disable(); }
#endif

            return Tree;
        }

        /// <summary>
        /// What we do in the first run/level of the tree making!
        /// </summary>
        private bool MakeAOpTree_Run1(ktToken PrevToken, ktList Prev, ktToken Token, ktList L,
                                        ref ktList Tree, ref ktList List, out bool SkipNext, out bool ReturnTree)
        {
            //ktDebug.Log("MAO_1");
            SkipNext = false;
            ReturnTree = false;

            // Is it an operator (doesn't include "assignment operators")?
            if (Token.Type == ktTokenType.Operator)
            {
                // Is it a ^ ?
                if (Token.Value == "^")
                {
                    // Handle the operator
                    HandleOperator(PrevToken, Prev, Token, L, ref Tree, 1, out SkipNext);

                // Is it NOT one of the ordinary (twosided?) operators?
                } else if ((Token.Value != "/") &&
                            (Token.Value != "*") && (Token.Value != "-") &&
                            (Token.Value != "+") && (Token.Value != "~"))
                {
                    // Handle the operator
                    HandleOperator(PrevToken, Prev, Token, L, ref Tree, 1, out SkipNext,true);

                    // Hey! That went well!
                    return true;
                }
            }
            // Is it an assignment operator?
            else if (Token.Type == ktTokenType.AssignmentOperator)
            {
                return HandleAssignmentOperator(PrevToken, Prev, Token, L, ref Tree, ref List, out SkipNext, out ReturnTree);
            }
            // Is it a "compound statement"?
            else if (Token.Type == ktTokenType.CompStatement)
            {
                return HandleCompStatement(Token, L, ref Tree, ref List, out SkipNext, out ReturnTree, 1);
            }

            // We did nothing!
            return false;
        }

        /// <summary>
        ///  What we do in the second run/level of the tree making!
        /// </summary>
        /// <param name="PrevToken"></param>
        /// <param name="Prev"></param>
        /// <param name="Token"></param>
        /// <param name="L"></param>
        /// <param name="Tree"></param>
        /// <param name="SkipNext"></param>
        /// <param name="ReturnTree"></param>
        /// <returns></returns>
        private bool MakeAOpTree_Run2(ktToken PrevToken, ktList Prev, ktToken Token, ktList L,
                                        ref ktList Tree, ref ktList List, out bool SkipNext, out bool ReturnTree)
        {
            //ktDebug.Log("MAO_2");
            SkipNext = false;
            ReturnTree = false;

            // Is it an operator (doesn't include "assignment operators")?
            if (Token.Type == ktTokenType.Operator)
            {
                // Is it a / or * ("Higher priority operators")
                if ((Token.Value == "/") ||
                    (Token.Value == "*"))
                {
                    // Handle the operator
                    HandleOperator(PrevToken, Prev, Token, L, ref Tree, 2, out SkipNext);

                    //ktDebug.Log("MAO_2: TREE:\n" + Tree.Get_R());

                    // Hey! That went well!
                    return true;
                }
            }
            // Is it a "compound statement"?
            else if (Token.Type == ktTokenType.CompStatement)
            {
                return HandleCompStatement(Token, L, ref Tree, ref List, out SkipNext, out ReturnTree, 2);
            }

            // We did nothing!
            return false;
        }
        /// <summary>
        /// What we do in the third   run/level of the tree making!
        /// </summary>
        private bool MakeAOpTree_Run3(ktToken PrevToken, ktList Prev, ktToken Token, ktList L,
                                        ref ktList Tree, ref ktList List, out bool SkipNext, out bool ReturnTree)
        {
            //ktDebug.Log("MAO_3");
            SkipNext = false;
            ReturnTree = false;

            // Is it an operator (doesn't include "assignment operators")?
            if (Token.Type == ktTokenType.Operator)
            {
                // Is it a + or - ("Lower priority operators")
                if ((Token.Value == "+") ||
                    (Token.Value == "-") ||
                    (Token.Value == "~"))
                {
                    // Handle the operator
                    HandleOperator(PrevToken, Prev, Token, L, ref Tree, 3, out SkipNext);

                    //ktDebug.Log("MAO_3: TREE:\n" + Tree.Get_R());

                    // Hey! That went well!
                    return true;
                }
            }
            // Is it a "compound statement"?
            else if (Token.Type == ktTokenType.CompStatement)
            {
                return HandleCompStatement(Token, L, ref Tree, ref List, out SkipNext, out ReturnTree, 3);
            }

            // We did nothing!
            return false;
        }

        /// <summary>
        /// Handle a "compound satement"
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="L"></param>
        /// <param name="Tree"></param>
        /// <param name="List"></param>
        /// <param name="SkipNext"></param>
        /// <param name="ReturnTree"></param>
        /// <returns></returns>
        private bool HandleCompStatement(ktToken Token, ktList L, ref ktList Tree, ref ktList List, out bool SkipNext, out bool ReturnTree, int RunNumb = 1)
        {
            ktList First = null, Second = null, SecondChild = null, Temp = null;
            ktToken FirstToken = null, SecondToken = null, SecondChildToken = null;

            SkipNext = false;
            ReturnTree = false;

            ktDebug.Log("HandleComp:\n" + L.Get_R());

            First = L.First;
            Second = L.First.Next;
            SecondChild = Second.First;
            FirstToken = (ktToken)First.Node.Value;
            SecondToken = (ktToken)Second.Node.Value;

            if (FirstToken.Type != ktTokenType.Id)
            {
                First = MakeAOpTree(First, RunNumb);
            }

            if (SecondChild != null)
            {
                SecondChildToken = (ktToken)SecondChild.Node.Value;
                if (SecondChildToken.Type == ktTokenType.List)
                {
                    SecondChild = MakeAOpTree(SecondChild, RunNumb);

                    Second.Clear();
                    Second.AddList(SecondChild);
                }
            }
            else if (SecondToken.Type != ktTokenType.Id)
            {
                Second = MakeAOpTree(Second, RunNumb);
            }

            Temp = new ktList();
            Temp.Node = L.Node;
            ((ktToken)Temp.Node.Value).RunnedStep = RunNumb;

            Temp.AddList(First);
            Temp.AddList(Second);

            Tree.AddList(Temp);

            /*ktDebug.Log("HandleComp__:\n" + Temp.Get_R());
            ktDebug.Log("HandleComp Tree:\n" + Tree.Get_R());*/

            return true;
        }

        /// <summary>
        /// Handle a operator in the tree
        /// </summary>
        private bool HandleOperator(ktToken PrevToken, ktList Prev, ktToken Token, ktList L, ref ktList Tree,
                                        int RunNumb, out bool SkipNext, bool OneSided = false )
        {
            ktList Next = null;
            ktList Temp = null;

            SkipNext = false;

            // Create a new "subtree"(/operator) and assign appropriate data
            Temp = new ktList();
            Temp.Node = L.Node;
            ((ktToken)Temp.Node.Value).RunnedStep = RunNumb;

            // If the left part of the "operator tree" hasn't been parsed on this level ...
            if (PrevToken.RunnedStep < RunNumb)
            {
                // ... parse it!
                Prev = MakeAOpTree(Prev, RunNumb);
            }
            else
            {
                Prev = new ktList(Prev);
            }

            // Add the first part to the op tree 
            Temp.AddList(Prev);

            // Is it a operator with two "sides"
            if (!OneSided)
            {
                // Parse the right part of the "operator tree"
                Next = MakeAOpTree(L.Next, RunNumb);
                // And add it to the op tree
                Temp.AddList(Next);

                SkipNext = true;
            }

            /*ktDebug.Log("HO: Temp:\n" + Temp.Get_R());
            ktDebug.Log("HO: Tree:\n" + Tree.Get_R());*/

            // Remove the last leaf(/item) in the tree (it will now become a leaf under this operator)
            Tree.Pop(false); // .Pop() is more efficient than .Remove() and 'false' as an argument means that it will "fail" gracefully in the event of an empty list!
            
            //ktDebug.Log("HO: TreeP:\n" + Tree.Get_R());

            // Add the new subtree/operator to the main tree
            Tree.AddList(Temp);

            //ktDebug.Log("HO: TreeA:\n" + Tree.Get_R());

            // Hey! That went well!
            return true;
        }

        private bool HandleAssignmentOperator(ktToken PrevToken, ktList Prev, ktToken Token, ktList L, ref ktList Tree, ref ktList List, out bool SkipNext, out bool ReturnTree)
        {
            ktList Next = null;
            ktList Temp = null;

            SkipNext = false;
            ReturnTree = false;

            /*ktDebug.Log("Tree::\n" + Tree.Get_R());
            ktDebug.Log("Prev: T:\n" + Prev.Node.Value.ToString());
            ktDebug.Log("Prev::\n" + Prev.Get_R());*/

            // Remove the last leaf(/item) in the tree (it will now become a leaf under this operator)
            Tree.Pop(false); // .Pop() is more efficient than .Remove() and 'false' as an argument means that it will "fail" gracefully in the event of an empty list!

            // Take a step forward
            List.MoveNext();

            // If the previous token has childs
            if (Prev.Count > 0) {
                if ( (((ktToken)Prev.First.Node.Value).Type == ktTokenType.Id) &&
                     (Prev.First.Next != null) &&
                     (((ktToken)Prev.First.Next.Node.Value).Type == ktTokenType.Id)
                    )
                {
                    Prev.Node = new ktNode("ktTVarStatement", new ktToken(ktTokenType.VarStatement, "", ((ktToken)Prev.Node.Value).LineNo, ((ktToken)Prev.Node.Value).CharPos)); ;
                }
                else
                {
                    // Parse the left part of the op tree
                    Prev = MakeAOpTree(Prev, 1);
                }
            }

            // Get the right part of the op tree
            Next = GetTheRest(List);

            // Parse the right part of the tree
            Next = MakeAOpTree(Next, 1);

            // Create a new "subtree"(/operator) and assign appropriate data
            Temp = new ktList();
            Temp.Node = L.Node;
            ((ktToken)Temp.Node.Value).RunnedStep = 1;

            // Add the parts of op tree
            Temp.AddList(Prev);
            Temp.AddList(Next);

            // Add the operator to the tree
            Tree.AddList(Temp);

           // ktDebug.Log("Tree::\n" + Tree.Get_R());

            ktDebug.WrapLevel--;

            ReturnTree = true;

            // Hey! That went well!
            return true;
        }

        /// <summary>
        /// Parse the tokens and create a optree
        /// </summary>
        protected ktList MakeAOpTree()
        {
            m_Lines = MakeAOpTree(MakeAOpTree(MakeAOpTree(m_Lines, 1), 2), 3);
            return m_Lines;
        }

        /// <summary>
        /// Get the rest of the line/statement...
        /// </summary>
        public ktList GetTheRest(ktList List)
        {
            ktList Rest = new ktList(), Curr = null, Next = null;
            ktToken Token = null;

            if (List == null)
            {
                return null;
            }
            if ((List.Node != null) && (List.Node.Value != null))
            {
                Token = (ktToken)List.Node.Value;
                Token = new ktToken(ktTokenType.Statement, "", Token.LineNo, Token.CharPos);
            }
            else
            {
                Token = new ktToken(ktTokenType.Statement, "", m_CurLine, m_CharPos);
            }
            Rest.Node = new ktNode("ktTStatement", Token);

            Curr = List.CurrentNode;

            while (Curr != null)
            {
                Next = Curr.Next;

                Rest.AddList(Curr);

                Curr = Next;
            }

            return Rest;
        }


        /// <summary>
        /// Run the script...
        /// </summary>
        public ktValue Run(ktList Arguments)
        {
            ktValue Ret = ktValue.Null;

            if (m_MainBlock == null)
            {
                m_MainBlock = new ktBlock(m_Lines.First);
            }
            m_MainBlock.SetMain(this);
//#if Debug
            ktDebug.Log("RUN:LINES:\n" + ktXML.FromList(m_MainBlock.Lines).AsXML());
//#endif

            Ret = m_MainBlock.Run(Arguments);

            return Ret;
        }
        /// <summary>
        /// Run the Script (Wrapper for ::Run( Arguments )
        /// </summary>
        public ktValue Run() { return Run(null); }

        /// <summary>
        /// Add a function to the main block...
        /// </summary>
        public void AddFunction(ktFunction Func)
        {
            if (m_MainBlock == null)
            {
                m_MainBlock = new ktBlock(m_Lines.First);
            }
            m_MainBlock.AddFunction(Func);
        }

        /// <summary>
        /// Run a function
        /// </summary>
        public ktValue RunFunction(ktString Name, ktList Arguments, ktString ConName)
        {
            ktValue Ret = ktValue.Null;
            ktContext Context = GetContext(ConName);

            if (Context == null)
            {
                throw new ktError("Couldn't find the context '" +
                                    ConName + "' and so, we can't run the function '" +
                                    Name + "'.", ktERR._404);
            }

            Ret = Context.RunFunction(Name, Arguments);

            return Ret;
        }
        /// <summary>
        /// Run a function
        /// </summary>
        public ktValue RunFunction(ktString Name, ktList Arguments)
        {
            return RunFunction(Name, Arguments, "Main");
        }

        /// <summary>
        /// Get a function
        /// </summary>
        public ktFunction GetFunction(ktString Name, ktString ConName)
        {
            ktFunction Ret = null;
            ktContext Context = GetContext(ConName);

            if (Context == null)
            {
                throw new ktError("Couldn't find the context '" +
                                  ConName + "' and so, we can't find the function '" +
                                    Name + "'.", ktERR._404);
            }

            Ret = Context.GetFunction(Name);

            return Ret;
        }
        /// <summary>
        /// Get a function
        /// </summary>
        public ktFunction GetFunction(ktString Name)
        {
            return GetFunction(Name, "Main");
        }

        /// <summary>
        /// Get a module...
        /// </summary>
        public ktModule GetModule(ktString Name)
        {
            if (Name.IsEmpty())
            {
                return null;
            }

            ktModule Module = null;

            if (m_Modules != null)
            {
                ktNode Node = m_Modules.GetNode(Name);

                if ((Node != null) && (Node.Value != null))
                {
                    Module = (ktModule)Node.Value;
                }
            }

            if (Module == null)
            {
                throw new ktError("kactalk::GetModule() : There's no module with the name '" +
                                  Name + "'!", ktERR.NOTDEF);
            }

            return Module;
        }

        /// <summary>
        /// Get a context
        /// </summary>
        public ktContext GetContext(ktString Name)
        {
            if (Name.IsEmpty() || (m_Modules == null))
            {
                return null;
            }

            ktContext Context = null;
            ktModule Module = null;

            m_Modules.Reset();
            //while ((Curr) && (!Context)) {
            foreach (ktList ML in m_Modules)
            {
                if ((ML.Node == null) || (ML.Node.Value == null))
                {
                    continue;
                }
                Module = (ktModule)ML.Node.Value;

                try
                {
                    Context = Module.GetContext(Name);

                    if (Context != null)
                    {
                        return Context;
                    }
                }
                catch (ktError Err)
                {
                    if (Err.ErrorNumber != ktERR.NOTFOUND)
                    {
                        throw Err;
                    }
                }
            }

            if (Context == null)
            {
                throw new ktError("Couldn't find the context '" + Name + "'.", ktERR.NOTFOUND);
            }

            return Context;
        }

        /// <summary>
        /// Get a variable
        /// </summary>
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
            catch (ktError E)
            {
                if (!((E.ErrorNumber == ktERR.NOTFOUND) || (E.ErrorNumber == ktERR.NOTDEF)))
                {
                    throw E;
                }
            }

            if (Var == null)
            {
                throw new ktError("kactalk::GetVariable() : There's no variable with the name '" +
                                  Name + "'!", ktERR.NOTDEF);
            }

            return Var;
        }

        /// <summary>
        /// Get a class
        /// </summary>
        public ktClass GetClass(ktString Name, ktString ContName)
        {
            if (Name.IsEmpty())
                return null;

            //ktNode Node = null;//m_Classes.GetNode( Name );

            ktContext Context = null;
            ktClass Class = null;

            //try {
            if ((Context = GetContext(ContName)) != null)
            {
                Class = Context.GetClass(Name);
            }
            /*} catch (ktError Err) {
                throw Err;
            }*/

            if (Class == null)
            {
                throw new ktError("kactalk::GetClass() : There's no class with the name '" +
                                  Name + "'!", ktERR.NOTDEF);
            }

            return Class;
        }
        /// <summary>
        /// Get a class
        /// </summary>
        public ktClass GetClass(ktString Name)
        {
            return GetClass(Name, "Main");
        }

        /// <summary>
        /// Get the class 'Name' and create an object of it
        /// </summary>
        public ktClass MakeObjectOf(ktString Name, ktString Value)
        {
            ktClass Class = GetClass(Name);

            if (Class == null)
            {
                return null;
            }
            ktClass Class2 = Class.CreateObject(Value);

            return Class2;
        }
        /// <summary>
        /// Get the class 'Name' and create an object of it
        /// </summary>
        public ktClass MakeObjectOf(ktString Name, ktValue Value)
        {
            ktClass Class = GetClass(Name);

            if (Class == null)
            {
                return null;
            }

            ktClass Class2 = Class.CreateObject(Value);

            return Class2;
        }
        /// <summary>
        /// Get the class 'Name' and create an object of it
        /// </summary>
        public ktClass MakeObjectOf(ktString Name, object Value)
        {
            ktClass Class = GetClass(Name);

            if (Class == null)
            {
                return null;
            }

            ktClass Class2 = Class.CreateObject(Value);

            return Class2;
        }
        /// <summary>
        /// Get the class 'Name' and create an object of it
        /// </summary>
        public ktValue MakeValueOf(ktString Name, ktString Value)
        {
            ktValue Val = null;

            ktClass Object = MakeObjectOf(Name, Value);

            Val = new ktValue("", Name, Object, false, false);

            return Val;
        }
        /// <summary>
        /// Get the class 'Name' and create an object of it
        /// </summary>
        public ktValue MakeValueOf(ktString Name, object Value)
        {
            ktValue Val = null;

            ktClass Object = MakeObjectOf(Name, Value);

            Val = new ktValue("", Name, Object, false, false);

            return Val;
        }

        /// <summary>
        /// Load a module
        /// </summary>
        public bool LoadModule(ktString Name)
        {
            ktString Filename = Name + "Lib.dll";
            ktString Path = m_ModulePath;
            ktString ModuleName = Name + "Module";

            if (Path.Last() != '/')
            {
                Path += "/";
            }

            ktModule Module = null;
            Assembly ModuleLib = null;

            try
            {
                using (FileStream FS = File.Open(Path + Filename, FileMode.Open))
                {
                    using (MemoryStream MS = new MemoryStream())
                    {
                        byte[] Buffer = new byte[1024];
                        int Read = 0;

                        while ((Read = FS.Read(Buffer, 0, 1024)) > 0)
                        {
                            MS.Write(Buffer, 0, Read);
                        }

                        ModuleLib = Assembly.Load(MS.ToArray());
                    }
                }

                foreach (Type type in ModuleLib.GetExportedTypes())
                {
                    if (ModuleName.Compare(type.Name, true))
                    {
                        Module = Activator.CreateInstance(type) as ktModule;
                        break;
                    }
                }
            }
            catch (Exception Err)
            {
                if ((Err.GetType() == typeof(System.IO.FileNotFoundException)) ||
                        (Err.GetType() == typeof(System.IO.DirectoryNotFoundException)))
                {
                    throw new ktError("Couldn't load the module/lib '" + Name +
                                      "' (looking for '" + Path + Filename + "')!", ktERR._404);
                }
                else
                {
                    throw Err;
                }
            }

            // If it's not ok....
            if (Module == null)
            {
                throw new ktError("Couldn't get a module in the library " + Name + " (" +
                                  Filename + ") ", ktERR._404);
            }


            //		      throw new Exception("no class found that implements interface IClass1");

            // Open the lib-file...
            /*void *Hndl = dlopen( Dir + Filename, RTLD_NOW|RTLD_GLOBAL);
            // If it's not ok....
            if (Hndl == NULL) {
                SetError( ktS("Couldn't load module ") + Name + " (" +
                        Dir + Filename + "): " + dlerror(), ktERR_404 );
                return false;
            }

            // Get the pointer to the GetModule-function
            ktModule *(*GetM)() = (ktModule*(*)())dlsym(Hndl, "GetModule");
            // If it's not ok....
            if (GetM == NULL){
                SetError( ktS("Couldn't find the function GetModule in library ") + Name + " (" +
                        Filename + "): " + dlerror(), ktERR_404 );
                return false;
            }

            // Get the module...
            ktModule *Module = (*GetM)();

            // If it's not ok....
            if (Module == NULL){
                SetError( ktS("Couldn't get a module in the library ") + Name + "(" +
                        Filename + "): " + dlerror(), ktERR_404 );
                return false;
            }*/

            return AddModule(Module);
        }
        /// <summary>
        /// Add a module to the interpreter
        /// </summary>
        public bool AddModule(ktModule Module)
        {
            if (Module == null)
            {
                return false;
            }

            if (m_Modules == null)
            {
                m_Modules = new ktList();
            }

            return m_Modules.Add(Module.Name, Module);
        }

        /// <summary>
        /// Load default modules etc
        /// </summary>
        public bool LoadDefaults()
        {
            LoadModule("ktMain");
            return LoadModule("ktMath");
            //			return AddModule( (ktModule)((object)new ktMainModule()) );
        }

        /// <summary>
        /// Add a global variable
        /// </summary>
        public bool AddVariable(ktValue Value)
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

            // Create a new node (with name and var), and add it
            ktNode Node = new ktNode(Value.Name, Value);
            return m_Variables.AddNode(Node);
        }

        /// <summary>
        /// Add the default constants and variables
        /// </summary>
        public void AddDefaultValues()
        {
            AddVariable(new ktValue("pi", "ktFloat", MakeObjectOf("ktFloat", Math.PI), true, true));
            AddVariable(new ktValue("pid", "ktDouble", MakeObjectOf("ktDouble", Math.PI), true, true));
            AddVariable(new ktValue("_kactalk", "kactalk", new kactalkClass(this), true, true));
        }

        #region properties
        /// <summary>
        /// Set/Get The name of the intepreter
        /// </summary>
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        /// <summary>
        /// Set/Get The path in which we should look for the modules
        /// </summary>
        public ktString ModulePath
        {
            get { return m_ModulePath; }
            set { m_ModulePath = value; }
        }
        /// <summary>
        /// Should we allow classes to be created?
        /// </summary>
        public bool AllowClasses
        {
            get { return m_AllowClasses; }
            set { m_AllowClasses = value; }
        }
        /// <summary>
        /// Should we allow functions to be created?
        /// </summary>
        public bool AllowFunctions
        {
            get { return m_AllowFunctions; }
            set { m_AllowFunctions = value; }
        }
        /// <summary>
        /// Should we execute the script in "math mode" or nat? (Changes the meaning of some operators)
        /// </summary>
        public bool MathMode
        {
            get { return m_MathMode; }
            set { m_MathMode = value; }
        }
        /// <summary>
        /// Should we allow a missing ktEOL (;) on the last line?
        /// </summary>
        public bool AllowMissingEOL
        {
            get { return m_AllowMissingEOL; }
            set { m_AllowMissingEOL = value; }
        }
        /// <summary>
        /// Get the defined variables
        /// </summary>
        public ktList Variables
        {
            get { return m_Variables; }
        }
        /// <summary>
        /// Reference to the main intepreter (an object of this class)
        /// </summary>
        public static kacTalk Main
        {
            get { return m_MainKT; }
        }
        /// <summary>
        /// Get the "main block"
        /// </summary>
        public ktBlock MainBlock
        {
            get { return m_MainBlock; }
        }
        /// <summary>
        /// Get the "last block"
        /// </summary>
        protected ktBlock LastBlock
        {
            get
            {
                if ((m_BlockStack == null) || (m_BlockStack.Last == null) ||
                    (m_BlockStack.Last.Node == null) || (m_BlockStack.Last.Node.Value == null))
                {
                    return null;
                }
                return (ktBlock)m_BlockStack.Last.Node.Value;
            }
        }
        /// <summary>
        /// Get the lines of the "last block"
        /// </summary>
        protected ktList LastBlockLines
        {
            get
            {
                ktBlock B = LastBlock;
                if (B != null)
                {
                    if (B.Lines == null)
                    {
                        return null;
                    }
                    return B.Lines;
                }
                return null;
            }
        }
        #endregion


        /// <summary>
        /// The name of the intepreter...
        /// </summary>
        protected ktString m_Name;
        /// <summary>
        /// The path where we should look for modules
        /// </summary>
        protected ktString m_ModulePath = "../lib/";

        /// <summary>
        /// Should we allow classes to be created?
        /// </summary>
        protected bool m_AllowClasses;
        /// <summary>
        /// Should we allow functions to be created?
        /// </summary>
        protected bool m_AllowFunctions;
        /// <summary>
        /// Should we execute the script in "math mode" or not? (Changes the meaning of some operators)
        /// </summary>
        protected bool m_MathMode;
        /// <summary>
        /// Should we allow a missing ktEOL (;) on the last line?
        /// </summary>
        protected bool m_AllowMissingEOL;

        /// <summary>
        /// The tokens...
        /// </summary>
        protected ktList m_Tokens;
        /// <summary>
        /// The lines...
        /// </summary>
        protected ktList m_Lines;
        /// <summary>
        /// The stack of blocks...
        /// </summary>
        protected ktList m_BlockStack;
        /// <summary>
        /// The stack of tokens...
        /// </summary> 
        protected ktList m_TokenStack;
        /// <summary>
        /// The stack of lines...
        /// </summary>
        protected ktList m_LineStack;

        /// <summary>
        /// The global variables...
        /// </summary>
        protected ktList m_Variables;
        /// <summary>
        /// The modules...
        /// </summary>
        protected ktList m_Modules;

        /// <summary>
        /// The main block...
        /// </summary>
        protected ktBlock m_MainBlock;

        /// <summary>
        /// The current token...
        /// </summary>
        protected ktToken m_CurToken;
        /// <summary>
        /// The "last" token...
        /// </summary>
        protected ktToken m_LastToken;

        /// <summary>
        /// The current line#...
        /// </summary>
        protected int m_CurLine;
        /// <summary>
        /// The current character...
        /// </summary>
        protected int m_CharPos;

        /// <summary>
        /// The main intepreter/kacTalk-object...
        /// </summary>
        protected static kacTalk m_MainKT = null;

        private int m_BufferSize = 16384;

        private bool m_enabledDebug = false;
        private int m_enabledAt = 0;
    }
}
