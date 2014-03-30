using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public enum ktTokenType
    {
        NULLTOKEN = 1000,
        Program,
        Context,
        Use,
        Block,
        Line,
        OLComment,
        MLComment,
        EOL,
        EOB,
        EOC,
        End,
        StartPar,
        EndPar,
        If,
        Unless,
        Else,
        ElseIf,
        For,
        Foreach,
        While,
        Do,
        Return,
        Id,
        Number,
        Float,
        Boolean,
        Repeat,
        Function,
        Class,
        Const,
        Hard,
        New,
        List,
        Operator,
        AssignmentOperator,
        ComparisonOperator,
        Separator,
        Comma,
        WhiteSpace,
        StringQuot,
        StartString,
        EndString,
        String,
        Bash,
        Statement,
        CompStatement,
        RunStatement,
        VarStatement,
        Null
    };

    public class ktToken : ktIntObj
    {
        public ktToken(ktTokenType Type, ktString Value,
                       int Line, int CharPos, ktString Name)
            : base("ktToken", 0)
        {
            m_TType = Type;
            m_Value = Value;
            m_Line = Line;
            m_CharPos = CharPos;
            m_Name = Name;
        }
        public ktToken(ktTokenType Type, ktString Value,
                       int Line, int CharPos) :
            this(Type, Value, Line, CharPos, TokenToString(Type))
        {
        }
        public ktToken(ktBlock _Block,
                       int Line, int CharPos) :
            this(ktTokenType.Block, "", Line, CharPos, "ktTBlock")
        {
            m_Block = _Block;
        }
        public ktToken(ktString Word, int Line, int CharPos)
            : this(StringToToken(Word), Word, Line, CharPos, TokenToString(StringToToken(Word)))
        {
        }

        public override string Export()
        {
            ktString Ret = new ktString();

            if (m_Block != null)
            {
                return m_Block.Export();
            }
            else if (ktToken.OnlyExportValue)
            {
                Ret = m_Name + "(" + m_Value + ")";
            }
            else
            {
                Ret = "\n\t\t['";
                Ret += m_Name + "'] => {\n\t\t\tValue: " + m_Value + ",\n";
                Ret += "\t\t\tLine: " + m_Line.ToString() + ",\n";
                Ret += "\t\t\tCharPos: " + m_CharPos.ToString() + ",\n";
                Ret += "\t\t\tToken: " + m_TType.ToString();

                Ret += "\n\t\t}";
            }

            return Ret;
        }
        public override string ToString()
        {
            if (m_Block != null)
            {
                return m_Block.ToString();
            }
            else
            {
                return m_Name + "( " + m_Value + " )";
            }
        }

        public static ktString TokenToString(ktTokenType T)
        {
            return "ktT" + T.ToString();
        }
        public static ktTokenType StringToToken(ktString Str)
        {
            ktTokenType Token;
            //ktDebug.Log( "S2TT( " + Str + " )" );
            if (Str == "program")
                Token = ktTokenType.Program;
            else if (Str == "context")
                Token = ktTokenType.Context;
            else if (Str == "use")
                Token = ktTokenType.Use;
            else if (Str == "{")
                Token = ktTokenType.Block;
            else if (Str == "begin")
                Token = ktTokenType.Block;/*
			else if (Str == "")
				Token = ktTokenType.Line;*/
            else if (Str == "//")
                Token = ktTokenType.OLComment;
            else if (Str == "/*")
                Token = ktTokenType.MLComment;
            else if (Str == ";")
                Token = ktTokenType.EOL;
            else if (Str == "}")
                Token = ktTokenType.EOB;
            else if (Str == "*/")
                Token = ktTokenType.EOC;
            else if (Str == "end")
                Token = ktTokenType.End;
            else if (Str == "(")
                Token = ktTokenType.StartPar;
            else if (Str == ")")
                Token = ktTokenType.EndPar;
            else if (Str == "if")
                Token = ktTokenType.If;
            else if (Str == "unless")
                Token = ktTokenType.Unless;
            else if (Str == "else")
                Token = ktTokenType.Else;
            else if (Str == "elseif")
                Token = ktTokenType.ElseIf;
            else if (Str == "for")
                Token = ktTokenType.For;
            else if (Str == "foreach")
                Token = ktTokenType.Foreach;
            else if (Str == "while")
                Token = ktTokenType.While;
            else if (Str == "do")
                Token = ktTokenType.Do;
            else if (Str == "return")
                Token = ktTokenType.Return;
            else if (Str == "repeat")
                Token = ktTokenType.Repeat;
            else if (Str == "function")
                Token = ktTokenType.Function;
            else if (Str == "class")
                Token = ktTokenType.Class;
            else if (Str == "const")
                Token = ktTokenType.Const;
            else if (Str == "hard")
                Token = ktTokenType.Hard;
            else if (Str == "new")
                Token = ktTokenType.New;
            //			else if (ktRegEx.Find( " \n\t", /*ktRegEx.Escape(*/ Str /*)*/ ) >= 0)
            else if (ktRegEx.Find(Str, "[ \n\t]") >= 0)
                Token = ktTokenType.WhiteSpace;
            else if ((Str == ":=") || (Str == "=:") || (Str == "="))
                Token = ktTokenType.AssignmentOperator;
            //			else if (ktRegEx.Find( "'\"", /*ktRegEx.Escape(*/ Str /*)*/ ) >= 0)
            else if (ktRegEx.Find(Str, "['\"]") >= 0)
                Token = ktTokenType.StringQuot;
            //			else if (ktRegEx.Find(@"\.:", /*ktRegEx.Escape(*/ Str /*)*/ ) >= 0)
            else if (ktRegEx.Find(Str, @"[\.:]") >= 0)
                Token = ktTokenType.Separator;
            else if (Str == ",")
                Token = ktTokenType.Comma;
            //			else if (ktRegEx.Find("+-*/=><!%&|[]?^", /*ktRegEx.Escape(*/ Str /*)*/ ) >= 0)
            else if (ktRegEx.Find(Str, "[-" + ktRegEx.Escape("+*/=><!%&|[]?^~") + "]") >= 0)
                Token = ktTokenType.Operator;
            else if (Str == "#")
                Token = ktTokenType.Bash;/*
			else if (Str == "use")
				Token = ktTokenType.List;/*
			else if (Str == "")
				Token = ktTokenType.Statement*/
            else if (ktRegEx.IsMatch("\\d+", Str))
                Token = ktTokenType.Number;
            else if ((Str == "true") || (Str == "false"))
                Token = ktTokenType.Boolean;
            else if ((Str == "null") || (Str == ":null"))
                Token = ktTokenType.Null;
            else
                Token = ktTokenType.Id;

            return Token;
        }

        public const string Separators = "\n|\t| |\\.|:|\\-|\\+|\\*|/|\\(|\\)|\\[|\\]|\\{|\\}|!|=|;|<|>|\\?|&|\\||%|'|\"|,|#|\\^|¬|\xE2|√";
        public static bool OnlyExportValue = false;

        #region properties
        public ktString Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        public ktString Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ktTokenType Type
        {
            get { return m_TType; }
            set { m_TType = value; }
        }
        public ktBlock Block
        {
            get { return m_Block; }
            set { m_Block = value; }
        }
        public int LineNo
        {
            get { return m_Line; }
            set { m_Line = value; }
        }
        public int CharPos
        {
            get { return m_CharPos; }
            set { m_CharPos = value; }
        }
        public int RunnedStep
        {
            get { return m_RunnedStep; }
            set { m_RunnedStep = value; }
        }
        public bool HasBlock
        {
            get { return m_HasBlock; }
            set { m_HasBlock = value; }
        }
        #endregion

        protected ktString m_Value;
        protected ktString m_Name;
        protected ktTokenType m_TType;
        protected int m_Line;
        protected int m_CharPos;
        protected ktBlock m_Block;
        protected bool m_HasBlock;
        protected int m_RunnedStep = -1;
    }
}
