using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace KacTalk
{
    public enum ktRE_Mode
    {
        PERL = 9001,
        Signs,
        Plain,

        Default = Plain
    };

    public delegate ktString ktRE_MatchEvaluator(ktRE_Match Match);

    public class ktRE_Capture : ktIntObj
    {
        // Constructors...
        public ktRE_Capture()
            : base("ktRE_Capture", 0)
        {
        }
        public ktRE_Capture(Capture Capt)
            : this()
        {
            m_Index = Capt.Index;
            m_Length = Capt.Length;
            m_Value = new ktString(Capt.Value);
        }
        public ktRE_Capture(ktRE_Capture Capt)
            : this()
        {
            m_Index = Capt.Index;
            m_Length = Capt.Length;
            m_Value = new ktString(Capt.Value);
        }

        public override string ToString()
        {
            return Value;
        }

        #region properties
        public int Index
        {
            get { return m_Index; }
        }
        public int Length
        {
            get { return m_Length; }
        }
        public ktString Value
        {
            get
            {
                try
                {
                    m_Value.IsEmpty();
                    return m_Value;
                }
                catch (Exception)
                {
                    return new ktString();
                }
            }
        }
        #endregion


        protected int m_Index;
        protected int m_Length;
        protected ktString m_Value;
    }

    public class ktRE_Group : ktRE_Capture
    {
        public ktRE_Group()
            : base()
        {
            m_Type = "ktRE_Group";
            m_Success = false;
        }
        public ktRE_Group(Group Gr)
            : this()
        {
            m_Index = Gr.Index;
            m_Length = Gr.Length;
            m_Value = new ktString(Gr.Value);
            m_Captures = new ktRE_CaptureCollection(Gr.Captures);
            m_Success = Gr.Success;
        }
        public ktRE_Group(ktRE_Group Gr)
            : this()
        {
            m_Index = Gr.Index;
            m_Length = Gr.Length;
            m_Value = Gr.Value;
            m_Captures = new ktRE_CaptureCollection(Gr.Captures);
            m_Success = Gr.Success;
        }

        public static ktRE_Group Synchronized(ktRE_Group Gr)
        {
            return new ktRE_Group(Gr);
        }

        public override string Export()
        {
            ktString Str = new ktString();

            Str = "ktRE_Group {\n";
            Str += "\tIndex\t\t= " + m_Index.ToString() + ",\n";
            Str += "\tLength\t\t= " + m_Length.ToString() + ",\n";
            Str += "\tValue\t\t= " + m_Value.Export() + ",\n";
            Str += "\tSuccess\t\t= " + m_Success.ToString() + ",\n";
            Str += "\tCaptures\t\t= " + m_Captures.Export() + ",\n";
            Str += "}\n";

            return Str;
        }

        #region properties
        public ktRE_CaptureCollection Captures
        {
            get { return m_Captures; }
        }
        /*	public ktRE_GroupCollection Groups
            {
                get { return m_Groups; }
            }*/
        public bool Success
        {
            get { return m_Success; }
        }
        #endregion

        protected ktRE_CaptureCollection m_Captures;
        //	protected ktRE_GroupCollection m_Groups;
        protected bool m_Success;
    }

    public class ktRE_CaptureCollection : ktIntObj//, ICollection, IEnumerable
    {
        public ktRE_CaptureCollection()
            : base("ktRE_CaptureCollection", 0)
        {
        }
        public ktRE_CaptureCollection(CaptureCollection GC)
            : this()
        {
            m_List = new ktList();

            foreach (Capture G in GC)
            {
                m_List.Add(G);
            }
        }
        public ktRE_CaptureCollection(ktRE_CaptureCollection GC)
            : this()
        {
            m_List = new ktList();

            foreach (ktRE_Capture G in GC)
            {
                m_List.Add(G);
            }
        }

        public void CopyTo(ktList List)
        {
            foreach (ktRE_Capture G in this)
            {
                List.Add(G);
            }
        }
        public void CopyTo(Array List, int Start)
        {
            foreach (ktRE_Capture G in this)
            {
                List.SetValue(G, Start++);
            }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        public override string Export()
        {
            if (m_List == null)
            {
                return ":null";
            }
            else
            {
                return m_List.Export();
            }
        }


        #region properties
        public virtual int Count
        {
            get
            {
                if (m_List == null)
                {
                    return 0;
                }
                else
                {
                    return m_List.Count;
                }
            }
        }
        public bool IsReadOnly
        {
            get { return true; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public ktRE_Capture this[int Num]
        {
            get
            {
                return null;
            }
        }
        public object SyncRoot
        {
            get { return null; }
        }
        #endregion

        protected ktList m_List;
    }

    public class ktRE_GroupCollection : ktIntObj, ICollection, IEnumerable/*, IEnumerator*/
    {
        public ktRE_GroupCollection()
            : base("ktRE_GroupCollection", 0)
        {
        }
        public ktRE_GroupCollection(GroupCollection GC)
            : this()
        {
            m_List = new ktList();

            foreach (Group G in GC)
            {
                m_List.Add(new ktRE_Group(G));
            }
        }
        public ktRE_GroupCollection(ktRE_GroupCollection GC)
            : this()
        {
            m_List = new ktList();

            foreach (ktRE_Group G in GC)
            {
                m_List.Add(G);
            }
        }

        public void CopyTo(ktList List)
        {
            foreach (ktRE_Group G in this)
            {
                List.Add(G);
            }
        }
        public void CopyTo(Array List, int Start)
        {
            foreach (ktRE_Group G in this)
            {
                List.SetValue(G, Start++);
            }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        public override string Export()
        {
            if (m_List == null)
            {
                return ":null";
            }
            else
            {
                return ktRegEx.Replace("\n", m_List.Get_R(), "\n\t");
            }
        }

        #region properties
        public virtual int Count
        {
            get
            {
                if (m_List == null)
                {
                    return 0;
                }
                else
                {
                    return m_List.Count;
                }
            }
        }
        public bool IsReadOnly
        {
            get { return true; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public ktRE_Group this[int Num]
        {
            get
            {
                if (m_List != null)
                {
                    ktList L = m_List.Get((uint)Num);
                    if ((L != null) && (L.Node != null))
                    {
                        return (ktRE_Group)(L.Node.Value);
                    }
                }
                return new ktRE_Group();
            }
        }
        public object SyncRoot
        {
            get { return null; }
        }
        #endregion

        protected ktList m_List;
    }

    public class ktRE_Match : ktRE_Group
    {
        public ktRE_Match()
            : base()
        {
            m_Type = "ktRE_Match";
        }
        public ktRE_Match(MatchCollection Match)
            : this()
        {
        }
        public ktRE_Match(System.Text.RegularExpressions.Match Match)
            : this()
        {
            m_Index = Match.Index;
            m_Length = Match.Length;
            m_Value = new ktString(Match.Value);
            m_Captures = new ktRE_CaptureCollection(Match.Captures);
            m_Success = Match.Success;

            m_Groups = new ktRE_GroupCollection(Match.Groups);
            if ((Match.NextMatch() != null) && (Match.NextMatch().Success))
            {
                m_NextMatch = new ktRE_Match(Match.NextMatch());
            }
            else
            {
                m_NextMatch = Empty;
                m_Matches = null;
            }
        }

        public ktRE_Match NextMatch()
        {
            if (m_NextMatch == null)
            {
                return Empty;
            }

            m_Matches.ToNext();
            return (ktRE_Match)(m_Matches.CurrentNode.Node.Value);
        }
        public ktRE_Match NextChildMatch()
        {
            if (m_Matches == null)
            {
                //throw new ktError( "ktRE_Match::NextChildMatch(): No matches!", ktERR.EMPTY );
                return Empty;
            }

            m_Matches.ToNext();
            return (ktRE_Match)(m_Matches.CurrentNode.Node.Value);
        }

        public ktString Result(ktString Replacement)
        {
            ktRE_MatchEvaluator Ev = new ktRE_MatchEvaluator(delegate(ktRE_Match Match)
            {
                ktString Val = new ktString();

                if (Match.Value.StartsWith("$", out Val))
                {
                    int iVal = Val.ToInt();

                    if (iVal < Count)
                    {
                        Val = m_Groups[iVal].Value;
                    }
                    else
                    {
                        Val = "";
                    }
                }

                return Val;
            });
            return ktRegEx.Replace(@"\$\d", Replacement, Ev);
        }

        public override string ToString()
        {
            ktString Str = new ktString();

            Str = "#ktRE_Match {\n";
            Str += "\tIndex\t\t= " + m_Index.ToString() + ",\n";
            Str += "\tLength\t\t= " + m_Length.ToString() + ",\n";
            Str += "\tValue\t\t= " + m_Value.Export() + ",\n";
            Str += "\tSuccess\t\t= " + m_Success.ToString() + ",\n";
            Str += "\tCaptures\t= " + m_Captures.Export() + ",\n";
            Str += "\tGroups\t\t= " + m_Groups.Export() + "\n";
            Str += "}\n";

            return Str;
        }

        public override string Export()
        {
            ktString Str = new ktString();

            Str = "ktRE_Match {\n";
            Str += "\tIndex\t= " + m_Index.ToString() + ",\n";
            Str += "\tLength\t= " + m_Length.ToString() + ",\n";
            Str += "\tValue\t= " + m_Value.Export() + ",\n";
            Str += "\tSuccess\t= " + m_Success.ToString() + ",\n";
            Str += "\tCaptures\t= " + m_Captures.Export() + ",\n";
            Str += "\tGroups\t\t= " + m_Groups.Export() + "\n";
            Str += "}\n";

            return Str;
        }


        #region Properties
        public int Count
        {
            get
            {
                if (m_Groups == null)
                {
                    return 0;
                }
                else
                {
                    return m_Groups.Count;
                }
            }
        }
        public static ktRE_Match Empty
        {
            get
            {
                return new ktRE_Match();
            }
        }
        public virtual ktRE_GroupCollection Groups
        {
            get
            {
                return m_Groups;
            }
        }
        #endregion

        protected ktRE_GroupCollection m_Groups;
        protected ktList m_Matches;
        protected ktRE_Match m_NextMatch;
    }


    public class ktRegEx : ktIntObj
    {
        // Constructors...
        // Main Constructor (takes both pattern and mode)
        public ktRegEx(ktString Pattern, ktRE_Mode Mode)
            : base("ktRegEx", 0)
        {
            SetMode(Mode);
            SetPattern(Pattern);
        }
        // Constructor that only takes pattern (wrapper for main constructor)
        public ktRegEx(ktString Pattern)
            : this(Pattern, ktRE_Mode.Default)
        {
        }
        // Constructor that only takes mode (wrapper for main constructor)
        public ktRegEx(ktRE_Mode Mode)
            : this("", Mode)
        {
        }
        // ("Default") Constructor that takes nothing (wrapper for main constructor)
        public ktRegEx()
            : this("", ktRE_Mode.Default)
        {
        }

        // Properties
        #region properties
        // Mode...
        public ktRE_Mode Mode
        {
            get { return GetMode(); }
            set { SetMode(value); }
        }
        // Pattern...
        public ktString Pattern
        {
            get { return m_Pattern; }
            set { SetPattern(value); }
        }
        // Which way?
        public bool RightToLeft
        {
            get { return m_P.RightToLeft; }
        }
        #endregion

        // "State changing"
        // Set the mode to use
        public bool SetMode(ktRE_Mode Mode)
        {
            m_Mode = Mode;
            return true;
        }
        // Set the pattern to use
        public bool SetPattern(ktString Pattern)
        {
            RegexOptions Opt = new RegexOptions();

            switch (m_Mode)
            {
                case ktRE_Mode.PERL:
                    {
                        ktString OPattern = new ktString(Pattern);
                        char Delim = Pattern.First();
                        Pattern.RemoveFirst();

                        int Pos = Find(Pattern, @"[^\\]" + Delim.ToString());
                        if (Pos < 0)
                        {
                            throw new ktError("ktRegEx::SetPattern() : The pattern isn't Perl compatible (" + OPattern + ")", ktERR.NOTFOUND);
                        }
                        else
                        {
                            Pos++;
                        }

                        ktString Modifiers = Pattern.SubStr(Pos + 1);
                        Pattern.Remove((uint)Pos);

                        for (uint I = 0; I < Modifiers.Len(); I++)
                        {
                            //						Console.Write( Modifiers[I].ToString() + ";" );
                            switch (Modifiers[I])
                            {
                                case 'i':
                                    {
                                        Opt = Opt | RegexOptions.IgnoreCase;
                                        break;
                                    }
                                case 'm':
                                    {
                                        Opt = Opt | RegexOptions.Multiline;
                                        break;
                                    }
                                case 'x':
                                    {
                                        Opt = Opt | RegexOptions.IgnorePatternWhitespace;
                                        break;
                                    }
                                case 'A':
                                    {
                                        if (Pattern.First() != '^')
                                        {
                                            Pattern.Prepend("^");
                                        }
                                        break;
                                    }/*
							case 'i': {
								Opt = Opt | RegexOptions.IgnoreCase;
								break;
							 }*/
                            }
                        }

                        break;
                    }
                case ktRE_Mode.Plain:
                    {
                        break;
                    }
                default:
                    {
                        throw new ktError("ktRegExp::SetPattern(): The mode '" + GetModeAsString() +
                                "' is not implementet yet!",
                            ktERR.NOTIMP);
                    }
            }

            // Set the pattern
            m_Pattern = Pattern;

            // Take care of the (if any) exception
            try
            {
                // Create the (internal) regex object
                m_P = new Regex(m_Pattern.GetValue(), Opt);
            }
            catch (ktError Ex)
            {
                SetError("Couldn't set pattern and create a new (internal) Regex object" +
                         Ex.Message, ktERR.REGEX_COULDNT_SET_PATTERN);
                return false;
            }
            catch (Exception Ex)
            {
                SetError("Couldn't set pattern and create a new (internal) Regex object" +
                         Ex.Message, ktERR.REGEX_COULDNT_SET_PATTERN);
                return false;
            }

            return true;
        }
        // Clear the state/settings (mode, pattern etc.)
        public void Clear()
        {
            // Clear the patterns etc..
            m_P = null;
            m_Pattern.Clear();
            m_Mode = 0;
        }

        // "State retrieving"
        // Get the mode
        public ktRE_Mode GetMode()
        {
            return m_Mode;
        }
        // Get the mode as a string
        public ktString GetModeAsString()
        {
            /*ktString Mode = new ktString();

            if (m_Mode == ktRE_Mode.Signs)
                Mode = "ktRE_Mode.Signs";
            else if (m_Mode == ktRE_Mode.Plain)
                Mode = "ktRE_Mode.Plain";
            else if (m_Mode == ktRE_Mode.PERL)
                Mode = "ktRE_Mode.PERL";

            return Mode;*/
            return GetModeAsString(m_Mode);
        }
        public static ktString GetModeAsString(ktRE_Mode Mode)
        {
            ktString ModeStr = new ktString();

            if (Mode == ktRE_Mode.Signs)
                ModeStr = "ktRE_Mode.Signs";
            else if (Mode == ktRE_Mode.Plain)
                ModeStr = "ktRE_Mode.Plain";
            else if (Mode == ktRE_Mode.PERL)
                ModeStr = "ktRE_Mode.PERL";

            return ModeStr;
        }
        // Get the pattern that's in use
        public ktString GetPattern()
        {
            return m_Pattern;
        }

        // "Operations"
        // Escape a string /*(so it can be use as a ""plain" pattern")*/
        public static ktString Escape(ktString Str)
        {
            Str.Replace("\\", "\\\\", true);
            Str.Replace("*", "\\*", true);
            Str.Replace("+", "\\+", true);
            Str.Replace("?", "\\?", true);
            Str.Replace("|", "\\|", true);
            Str.Replace("{", "\\{", true);
            Str.Replace("[", "\\[", true);
            Str.Replace("(", "\\)", true);
            Str.Replace(")", "\\)", true);
            Str.Replace("]", "\\]", true);
            Str.Replace("}", "\\}", true);
            Str.Replace("^", "\\^", true);
            Str.Replace("$", "\\$", true);
            Str.Replace(".", "\\.", true);
            Str.Replace("#", "\\#", true);
            Str.Replace("/", "\\/", true);
            return Str;
        }


        // Match a pattern against a value (static!)
        public static bool Matches(ktString Pattern, ktString Value, ktRE_Mode Mode)
        {
            // "Init" the object
            Regex RegObj;

            // Check which mode to use
            // If we should use the mode "plain"
            if (Mode == ktRE_Mode.Plain)
            {
                // Create the (internal) regex object using the givven pattern
                RegObj = new Regex(Pattern.GetValue());
                // Doesn't support the mode for now
            }
            else
            {
                // We set an error that we doesn't support the choosed mode
                //  (should we throw and exception???)
                /*SetError( "ktRegExp::Match() : The mode '" + GetModeAsString().GetValue() +
                            "' is not implementet yet!",
                        ktERR.NOTIMP );*/
                return false;
            }

            // Do the match...
            return RegObj.IsMatch(Value.GetValue());
        }
        // Match a pattern against a value (static!) (Wrapper for ::Matches(), without Mode)
        public static bool Matches(ktString Pattern, ktString Value)
        {
            // Use ::Matches above..
            return Matches(Pattern, Value, ktRE_Mode.Default);
        }
        // Match a value against the storred pattern
        public bool IsMatch(ktString Value)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                SetError("ktRegExp::IsMatch(): The pattern is not set!", ktERR.NOTSET);
                return false;
            }

            // Match...
            return m_P.IsMatch(Value.GetValue());
        }
        // Match a value against the storred pattern
        public bool IsMatch(ktString Value, int Start)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                SetError("ktRegExp::IsMatch(): The pattern is not set!", ktERR.NOTSET);
                return false;
            }

            return m_P.IsMatch(Value.GetValue(), Start);
        }
        // Match a value against the storred pattern
        public static bool IsMatch(ktString Pattern, ktString Value)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.IsMatch(Value);
        }

        /// Search...
        // Find all occurence of a pattern in the given value/string
        public ktList FindAllIn(ktString Value, bool AsObject)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::FindAllIn(): The pattern is not set!", ktERR.NOTSET);
            }

            ktList List = new ktList();

            MatchCollection Found = m_P.Matches(Value);
            foreach (Match M in Found)
            {
                if (AsObject)
                {
                    List.Add(new ktRE_Match(M));
                }
                else
                {
                    List.Add(M.ToString());
                }
            }

            return List;
        }
        // Get the matches as string (wrapper for ::FindAllIn())
        public ktList FindAllIn(ktString Value)
        {
            return FindAllIn(Value, false);
        }
        // Get the matches (wrapper for ::FindAllIn())
        public ktList GetMatches(ktString Value, bool AsObject)
        {
            return FindAllIn(Value, AsObject);
        }
        // Get the matches (wrapper for ::FindAllIn())
        public ktList GetMatches(ktString Value)
        {
            return FindAllIn(Value, true);
        }
        // Find/return the "Nth" occurence of a pattern
        public ktString FindNth(ktString Value, uint Num)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::FindNth(): The pattern is not set!", ktERR.NOTSET);
                //return null;
            }

            // Use ::FindAllIn() to find all the values 
            ktList Matches = FindAllIn(Value);

            // Get the Nth match
            ktString Match = (ktString)Matches.GetValue(Num - 1);

            // Return the match..
            return Match;
        }
        public int Find(ktString Value)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Find(): The pattern is not set!", ktERR.NOTSET);
            }

            int Pos = -1;

            Match Found = m_P.Match(Value);

            if (Found.Success)
            {
                Pos = Found.Index;
            }

            return Pos;
        }
        public static int Find(ktString Haystack, ktString Value, ktRE_Mode Mode)
        {
            ktRegEx RE = new ktRegEx(Value, Mode);

            return RE.Find(Haystack);
        }
        public static int Find(ktString Haystack, ktString Value)
        {
            return Find(Haystack, Value, ktRE_Mode.Default);
        }
        public ktRE_Match Match(ktString Value)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Match(): The pattern is not set!", ktERR.NOTSET);
            }

            return new ktRE_Match(m_P.Match(Value));
        }
        public static ktRE_Match Match(ktString Haystack, ktString Value, ktRE_Mode Mode)
        {
            ktRegEx RE = new ktRegEx(Value, Mode);

            return RE.Match(Haystack);
        }
        public static ktRE_Match Match(ktString Haystack, ktString Value)
        {
            return Match(Haystack, Value);
        }

        public ktString Replace(ktString Haystack, ktString Replacement)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Replace(): The pattern is not set!", ktERR.NOTSET);
            }

            return new ktString(m_P.Replace(Haystack, Replacement));
        }
        public ktString Replace(ktString Haystack, ktString Replacement, int Count)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Replace(): The pattern is not set!", ktERR.NOTSET);
            }

            return new ktString(m_P.Replace(Haystack, Replacement, Count));
        }
        public ktString Replace(ktString Haystack, ktString Replacement, int Count, int StartAt)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Replace(): The pattern is not set!", ktERR.NOTSET);
            }

            return new ktString(m_P.Replace(Haystack, Replacement, Count, StartAt));
        }
        public ktString Replace(ktString Haystack, ktRE_MatchEvaluator Evaluator)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Replace(): The pattern is not set!", ktERR.NOTSET);
            }

            MatchEvaluator Ev = new MatchEvaluator(delegate(Match M)
            {
                return Evaluator(new ktRE_Match(M));
            });
            return new ktString(m_P.Replace(Haystack, Ev));
        }
        public ktString Replace(ktString Haystack, ktRE_MatchEvaluator Evaluator, int Count)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Replace(): The pattern is not set!", ktERR.NOTSET);
            }

            MatchEvaluator Ev = new MatchEvaluator(delegate(Match M)
            {
                return Evaluator(new ktRE_Match(M));
            });
            return new ktString(m_P.Replace(Haystack, Ev, Count));
        }
        public ktString Replace(ktString Haystack, ktRE_MatchEvaluator Evaluator, int Count, int StartAt)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Replace(): The pattern is not set!", ktERR.NOTSET);
            }

            MatchEvaluator Ev = new MatchEvaluator(delegate(Match M)
            {
                return Evaluator(new ktRE_Match(M));
            });
            return new ktString(m_P.Replace(Haystack, Ev, Count, StartAt));
        }

        public static ktString Replace(ktString Pattern, ktString Haystack, ktString Replacement)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.Replace(Haystack, Replacement);
        }
        public static ktString Replace(ktString Pattern, ktString Haystack, ktString Replacement, int Count)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.Replace(Haystack, Replacement, Count);
        }
        public static ktString Replace(ktString Pattern, ktString Haystack, ktRE_MatchEvaluator Evaluator)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.Replace(Haystack, Evaluator);
        }
        public static ktString Replace(ktString Pattern, ktString Haystack, ktRE_MatchEvaluator Evaluator, int Count)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.Replace(Haystack, Evaluator, Count);
        }

        public static ktList Replace(ktString[] Patterns, ktString Haystack, ktRE_MatchEvaluator Evaluator)
        {
            ktList Results = new ktList();

            foreach (ktString Pattern in Patterns)
            {
                Results.Add(Replace(Pattern, Haystack, Evaluator));
            }

            return Results;
        }
        public static ktList Replace(ktString[] Patterns, ktString Haystack, ktString Replacement)
        {
            ktList Results = new ktList();

            foreach (ktString Pattern in Patterns)
            {
                Results.Add(Replace(Pattern, Haystack, Replacement));
            }

            return Results;
        }
        public static ktList Replace(ktString Pattern, ktString[] Haystacks, ktRE_MatchEvaluator Evaluator)
        {
            ktList Results = new ktList();

            foreach (ktString Haystack in Haystacks)
            {
                Results.Add(Replace(Pattern, Haystack, Evaluator));
            }

            return Results;
        }
        public static ktList Replace(ktString Pattern, ktString[] Haystacks, ktString Replacement)
        {
            ktList Results = new ktList();

            foreach (ktString Haystack in Haystacks)
            {
                Results.Add(Replace(Pattern, Haystack, Replacement));
            }

            return Results;
        }
        public static ktList Replace(ktString Pattern, ktString Haystack, ktRE_MatchEvaluator[] Evaluators)
        {
            ktList Results = new ktList();

            foreach (ktRE_MatchEvaluator Evaluator in Evaluators)
            {
                Results.Add(Replace(Pattern, Haystack, Evaluator));
            }

            return Results;
        }
        public static ktList Replace(ktString Pattern, ktString Haystack, ktString[] Replacements)
        {
            ktList Results = new ktList();

            foreach (ktString Replacement in Replacements)
            {
                Results.Add(Replace(Pattern, Haystack, Replacement));
            }

            return Results;
        }
        public static ktList Replace(ktList Replacements, ktString Haystack)
        {
            ktList Results = new ktList();
            ktString Pattern = new ktString();
            ktString Replacement = new ktString();
            ktRE_MatchEvaluator Ev = null;
            ktNode Node;
            ktString Result = "";
            object First = null, Second = null;

            Replacements.Reset();
            foreach (ktList Post in Replacements)
            {
                if (Post.IsEmpty())
                {
                    if ((Post.Node != null) && (Post.Node.Value != null))
                    {
                        Pattern = Post.Node.Name;
                        if ((Post.Node.Value.GetType() == typeof(ktString)) ||
                            (Post.Node.Value.GetType() == typeof(string)))
                        {
                            Replacement = Post.Node.Value.ToString();
                            Results.Add(Replace(Pattern, Haystack, Replacement));
                        }
                        else if (Post.Node.Value.GetType() == typeof(ktRE_MatchEvaluator))
                        {
                            Ev = (ktRE_MatchEvaluator)Post.Node.Value;
                            Results.Add(Replace(Pattern, Haystack, Ev));
                        }
                        else if (Post.Node.Value.GetType() == typeof(ktRegEx))
                        {
                            //ktRegEx RE = (ktRegEx)Post.Node.Value;
                            if (Pattern.IsEmpty())
                            {
                                //Results.Add( RE.Replace( Haystack, Ev ) );
                            }
                            else
                            {
                                //Results.Ad‭d( Replace( Pattern, Haystack, Ev ) );
                            }
                        }
                    }
                }
                else if (Post.Count == 2)
                {
                    if ((Node = Post.GetNode(0)) != null)
                    {
                        First = Node.Value;
                        if ((Node = Post.GetNode(1)) != null)
                        {
                            Second = Node.Value;
                        }
                    }
                    if (Second != null)
                    {
                        if (First.GetType() == typeof(ktRegEx))
                        {
                            Result = HandleRegExFirst((ktRegEx)First, Second, Haystack);
                            Results.Add(Result);
                        }
                        else if (First.GetType() == typeof(ktString[]))
                        {
                            ktString[] Patterns = (ktString[])First;

                            foreach (ktString P in Patterns)
                            {
                                Result = HandleStringFirst(P, Second, Haystack);
                                Results.Add(Result);
                            }
                        }
                        else if (First.GetType() == typeof(ktList))
                        {
                            ktList Patterns = (ktList)First;

                            foreach (ktList P in Patterns)
                            {
                                if ((P.Node != null) && (P.Node.Value != null))
                                {
                                    Pattern = P.Node.Value.ToString();
                                    Result = HandleStringFirst(Pattern, Second, Haystack);
                                    Results.Add(Result);
                                }
                            }
                        }
                        else /*if ((First.GetType() == typeof( ktString )) ||
							(First.GetType() == typeof( string )))*/
                        {
                            Result = HandleStringFirst(First.ToString(), Second, Haystack);
                            Results.Add(Result);
                        }
                    }
                }
            }

            return Results;
        }

        protected static ktString HandleStringFirst(ktString Pattern, object Obj, ktString Haystack)
        {
            if ((Obj.GetType() == typeof(ktString)) ||
                (Obj.GetType() == typeof(string)))
            {
                return Replace(Pattern, Haystack, Obj.ToString());
            }
            else if (Obj.GetType() == typeof(ktRE_MatchEvaluator))
            {
                ktRE_MatchEvaluator Ev = (ktRE_MatchEvaluator)Obj;
                return Replace(Pattern, Haystack, Ev);
            }
            else if (Obj.GetType() == typeof(ktRegEx))
            {
                ktRegEx RE = (ktRegEx)Obj;
                ktRE_MatchEvaluator Ev = new ktRE_MatchEvaluator(
                    delegate(ktRE_Match Match)
                    {
                        return RE.Replace(Pattern, Match.Value);
                        //return RE.Replace( Match.Value, Pattern ); 
                    });

                return RE.Replace(Haystack, Ev);
            }

            return Haystack;
        }
        protected static ktString HandleRegExFirst(ktRegEx RE, object Obj, ktString Haystack)
        {/*
			if ((Obj.GetType() == typeof( ktString )) ||
				(Obj.GetType() == typeof( string ))) {
					return Replace( Pattern, Haystack, Obj.ToString() );
			} else if (Obj.GetType() == typeof( ktRE_MatchEvaluator )) {
				ktRE_MatchEvaluator Ev = (ktRE_MatchEvaluator)Obj;
				return Replace( Pattern, Haystack, Ev );
			} else if (Obj.GetType() == typeof( ktRegEx )) {
				ktRegEx RE = (ktRegEx)Obj;
				ktRE_MatchEvaluator Ev = new ktRE_MatchEvaluator(
					delegate (ktRE_Match Match) {
						return RE.Replace( Pattern, Match.Value );
						//return RE.Replace( Match.Value, Pattern ); 
					} );

				return RE.Replace( Haystack, Ev );
			}*/

            return Haystack;
        }

        public ktList Split(ktString Input)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Split(): The pattern is not set!", ktERR.NOTSET);
            }

            ktList Parts = new ktList();
            string[] OrgParts = m_P.Split(Input);

            foreach (string Part in OrgParts)
            {
                Parts.Add(new ktString(Part));
            }

            return Parts;
        }
        public ktString[] SplitAsArr(ktString Input)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Split(): The pattern is not set!", ktERR.NOTSET);
            }

            string[] OrgParts = m_P.Split(Input);
            ktString[] Parts = new ktString[OrgParts.Length];
            int Pos = 0;

            foreach (string Part in OrgParts)
            {
                Parts.SetValue(new ktString(Part), Pos);
                Pos++;
            }

            return Parts;
        }
        public ktList Split(ktString Input, int Count)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Split(): The pattern is not set!", ktERR.NOTSET);
            }

            ktList Parts = new ktList();
            string[] OrgParts = m_P.Split(Input, Count);

            foreach (string Part in OrgParts)
            {
                Parts.Add(new ktString(Part));
            }

            return Parts;
        }
        public ktString[] SplitAsArr(ktString Input, int Count)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption??) and return false
                throw new ktError("ktRegExp::Split(): The pattern is not set!", ktERR.NOTSET);
            }

            string[] OrgParts = m_P.Split(Input, Count);
            ktString[] Parts = new ktString[OrgParts.Length];
            int Pos = 0;

            foreach (string Part in OrgParts)
            {
                Parts.SetValue(new ktString(Part), Pos);
                Pos++;
            }

            return Parts;
        }
        public static ktList Split(ktString Pattern, ktString Input)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.Split(Input);
        }
        public static ktString[] SplitAsArr(ktString Pattern, ktString Input)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.SplitAsArr(Input);
        }
        public static ktList Split(ktString Pattern, ktString Input, int Count)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.Split(Input, Count);
        }
        public static ktString[] SplitAsArr(ktString Pattern, ktString Input, int Count)
        {
            ktRegEx RE = new ktRegEx(Pattern);

            return RE.SplitAsArr(Input, Count);
        }

        public ktList GetGroupNames()
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption...) and return false
                throw new ktError("ktRegExp::GetGroupNames(): The pattern is not set!", ktERR.NOTSET);
            }

            ktList Names = new ktList();

            foreach (string Group in m_P.GetGroupNames())
            {
                Names.Add(new ktString(Group));
            }

            return Names;
        }

        public ktList GetGroupNumbers()
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption...) and return false
                throw new ktError("ktRegExp::GetGroupNumbers(): The pattern is not set!", ktERR.NOTSET);
            }

            ktList Names = new ktList();

            foreach (int Group in m_P.GetGroupNumbers())
            {
                Names.Add(Group);
            }

            return Names;
        }
        public ktString GroupNameFromNumber(int I)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption...) and return false
                throw new ktError("ktRegExp::GroupNameFromNumber(): The pattern is not set!", ktERR.NOTSET);
            }
            return new ktString(m_P.GroupNameFromNumber(I));
        }
        public int GroupNumberFromName(ktString Name)
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                // Set error (throw exeption...) and return false
                throw new ktError("ktRegExp::GroupNumberFromName(): The pattern is not set!", ktERR.NOTSET);
            }
            return m_P.GroupNumberFromName(Name);
        }


        public override int GetHashCode()
        {
            // If we have a pattern?!
            if ((m_Pattern.IsEmpty()) || (m_P == null))
            {
                return 0;
            }
            return m_P.GetHashCode() + m_Pattern.GetHashCode();
        }

        // Properties...
        protected ktRE_Mode m_Mode;
        protected ktString m_Pattern;
        protected Regex m_P;
    }
}
