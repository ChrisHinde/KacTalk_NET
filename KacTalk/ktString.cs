using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public enum ktStripType
    {
        trailing,
        right = trailing,
        leading,
        left = leading,
        both
    }

    public class ktString : ktIntObj
    {
        public ktString()
            : base("ktString", 0)
        {
            SetValue("");
        }
        public ktString(string Str)
            : base("ktString", 0)
        {
            SetValue(Str);
        }
        public ktString(char Ch)
            : base("ktString", 0)
        {
            SetValue(Ch);
        }
        public ktString(bool Value)
            : base("ktString", 0)
        {
            SetValue(Value);
        }
        public ktString(int Value)
            : base("ktString", 0)
        {
            SetFromNumber(Value);
        }

        public string GetValue()
        {
            return m_Content;
        }
        public override string ToString()
        {
            return m_Content;
        }
        public override string Export()
        {
            return "\"" + m_Content + "\"";
        }

        public int ToInt()
        {
            return Convert.ToInt32(m_Content);
        }
        public double ToDouble()
        {
            return Convert.ToDouble(m_Content);
        }
        public float ToFloat()
        {
            return Convert.ToSingle(m_Content,new CultureInfo("en-US"));
        }
        public long ToLong()
        {
            return Convert.ToInt64(m_Content);
        }
        public System.Int64 ToInt64()
        {
            return Convert.ToInt64(m_Content);
        }
        public uint ToUInt()
        {
            return Convert.ToUInt32(m_Content);
        }
        public System.UInt64 ToULong()
        {
            return Convert.ToUInt64(m_Content);
        }
        public System.UInt64 ToUInt64()
        {
            return Convert.ToUInt64(m_Content);
        }

        public ktString AsUpper()
        {
            return new ktString(m_Content.ToUpper());
        }
        public ktString AsLower()
        {
            return new ktString(m_Content.ToLower());
        }
        public ktString Capitalize()
        {
            return new ktString(First().ToString().ToUpper() +
                                    SubStr(1).AsLower());
        }

        public ktString Strip(ktStripType St, ktString Delim)
        {
            string Str = m_Content;

            if ((St == ktStripType.both) || (St == ktStripType.leading))
            {
                Str = Str.TrimStart(Delim.GetValue().ToCharArray());
            }
            if ((St == ktStripType.both) || (St == ktStripType.trailing))
            {
                Str = Str.TrimEnd(Delim.GetValue().ToCharArray());
            }

            return new ktString(Str);
        }
        public ktString Strip()
        {
            return Strip(ktStripType.both, new ktString("\n\t "));
        }
        public ktString Strip(ktStripType St)
        {
            return Strip(St, new ktString("\n\t "));
        }

        // Substring extraction
        // Get string from the beginning
        public ktString Left(uint Count)
        {
            return SubStr(0, (int)Count);
        }
        // Get strings from the end
        public ktString Right(uint Count)
        {
            return SubStr(Length() - (int)Count);
        }

        // Get string before the First occurence of the
        //  substring that Is supplied
        public ktString BeforeFirst(ktString Str)
        {
            // Get the position for the first occurence of 'Str'
            int Pos = m_Content.IndexOf(Str.GetValue());

            if (Pos < 0)
                Pos = 0;

            // Get everything before that position
            return Left((uint)Pos);
        }
        // Get string before the First occurence of the
        //  substring that Is supplied
        public ktString BeforeFirst(char Ch)
        {
            // Get the position for the first occurence of 'Ch'
            int Pos = m_Content.IndexOf(Ch);

            if (Pos < 0)
                Pos = 0;

            // Get everything before that position
            return Left((uint)Pos);
        }
        // Get after before the First occurence of the
        //  substring that Is supplied
        public ktString AfterFirst(ktString Str)
        {
            // Get the position for the first occurence of 'Str'
            int Pos = IndexOf(Str) + Length();

            if (Pos < 0)
                Pos = 0;

            // Get everything after that occurence
            return SubStr(Pos);
        }
        // Get after before the First occurence of the
        //  substring that Is supplied
        public ktString AfterFirst(char Ch)
        {
            // Get the position for the first occurence of 'Ch'
            int Pos = IndexOf(Ch) + Length();

            if (Pos < 0)
                Pos = 0;

            // Get everything after that occurence
            return SubStr(Pos);
        }
        // Get string before the last occurence of the
        //  substring that Is supplied
        public ktString BeforeLast(ktString Str)
        {
            // Get the position for the last occurence of 'Str'
            int Pos = m_Content.LastIndexOf(Str.GetValue());

            if (Pos < 0)
                Pos = Length() - 1;

            // Get everything before that occurence
            return Left((uint)Pos);
        }
        // Get string before the last occurence of the
        //  substring that Is supplied
        public ktString BeforeLast(char Ch)
        {
            // Get the position for the last occurence of 'Ch'
            int Pos = m_Content.LastIndexOf(Ch);

            if (Pos < 0)
                Pos = Length() - 1;

            // Get everything before that occurence
            return Left((uint)Pos);
        }
        // Get string after the last occurence of the
        //  substring that Is supplied
        public ktString AfterLast(ktString Str)
        {
            // Get the position for the last occurence of 'Str'
            //int Pos = m_Content.LastIndexOf( Str.GetValue() ) + Length();
            int Pos = Find(Str, true) + Length();

            if (Pos < 0)
                Pos = Length() - 1;

            // Get everything after that occurence
            return SubStr(Pos);
        }
        // Get string after the last occurence of the
        //  substring that Is supplied
        public ktString AfterLast(char Ch)
        {
            // Get the position for the last occurence of 'Ch'
            int Pos = IndexOf(Ch) + Length();

            if (Pos < 0)
                Pos = Length() - 1;

            // Get everything after that occurence
            return SubStr(Pos);
        }

        // Extract a substring from the string (to the end of the string)
        public ktString SubStr(int First)
        {
            return SubStr(First, -1);
        }
        // Extract a substring from the string
        public ktString SubStr(int First, int Count)
        {
            if (Count >= 0)
                return new ktString(m_Content.Substring(First, Count));
            else
                return new ktString(m_Content.Substring(First));
        }
        // Extract a substring from the string (to the end of the string)
        public ktString SubString(int First)
        {
            return SubStr(First, -1);
        }
        // Extract a substring from the string
        public ktString SubString(int First, int Count)
        {
            return SubStr(First, Count);
        }
        // Extract a substring from the string (to the end of the string)
        public ktString Mid(int First)
        {
            return SubStr(First, -1);
        }
        // Extract a substring from the string
        public ktString Mid(int First, int Count)
        {
            return SubStr(First, Count);
        }

        // Get a character from a certaion Pos. in the string
        public char GetChar(uint Pos)
        {
            return m_Content[(int)Pos];
        }

        // Get the First character of the string
        public char First()
        {
            return GetChar(0);
        }
        // Get the Last character of the string
        public char Last()
        {
            return GetChar((uint)(Length() - 1));
        }

        // String (value) setting/writing
        // Empty the string
        public void Clear()
        {
            m_Content = "";
        }
        // Empty the string
        public void Empty()
        {
            m_Content = "";
        }

        // Set the value of the string, from ktString format
        public void SetValue(ktString Str)
        {
            if (((object)Str) == null)
            {
                m_Content = "";
            }
            else
            {
                m_Content = Str.GetValue();
            }
        }
        // Set the value of the string, from string format
        public void SetValue(string Str)
        {
            if (Str == null)
            {
                m_Content = "";
            }
            else
            {
                m_Content = Str;
            }
        }
        // Set the value of the string, from char Array format
        public void SetValue(char[] Arr)
        {
            string Str = "";

            if (Arr != null)
            {
                foreach (char Ch in Arr)
                {
                    Str.PadRight(1, Ch);
                }
            }

            SetValue(Str);
        }
        // Set the value of the string, from char format
        public void SetValue(char Ch)
        {
            m_Content = "" + Ch;/*
			m_Content.PadLeft( 1, Ch );*/
        }
        // Set the value of the string, from a bool
        public void SetValue(bool Boolesk)
        {
            if (Boolesk)
                m_Content = "true";
            else
                m_Content = "false";
        }
        // Set the value of the string, from a bool
        //   It sets the string value to 'true' or 'false' (depending on the argument)
        public void SetBool(bool Boolesk)
        {
            if (Boolesk)
                m_Content = "true";
            else
                m_Content = "false";
        }
        // Set the value of the string, from a bool
        //   It sets the string value to 'true' or 'false' (depending on the argument)
        public void SetBoolesk(bool Boolesk)
        {
            SetBool(Boolesk);
        }
        // Set the value of the string, from a bool
        //   It sets the string value to 'nil' or empty (depending on the argument)
        public void SetNil(bool Nil)
        {
            if (Nil)
                m_Content = "nil";
            else
                m_Content = "";
        }

        // Set the value of the string from number/int
        public void SetFromNumber(int Value)
        {
            SetValue(Convert.ToString(Value));
        }
        // Set the value of the string from number/float/double
        public void SetFromNumber(double Value, int DecCnt)
        {
            string Decimals = ".";

            if (DecCnt < 0)
                DecCnt = 9;
            for (int I = 0; I < DecCnt; I++)
                Decimals += "#";

            if (Decimals == ".")
                Decimals = "";

            SetValue(Value.ToString("0" + Decimals));
        }
        // Set the value of the string from number/float/double
        public void SetFromNumber(double Value)
        {
            SetFromNumber(Value, -1);
        }

        // Set the value of the string using "format" (as printf or string::format)
        public ktString Printf(ktString Format, params object[] ParamList)
        {
            m_Content = string.Format(Format.GetValue(), ParamList);

            return this;
        }
        // Same as above but this function(s) returns the string
        //  instead of change the string in the object...
        public static ktString Format(ktString Format, params object[] ParamList)
        {
            return new ktString(string.Format(Format.GetValue(), ParamList));
        }

        // String value tampering
        // Appends (concate, in the end) a string to the string
        //  returns a reference to the string
        public ktString Append(ktString Str)
        {
            return Append(Str.GetValue());
        }
        // Appends (concate, in the end) a string to the string
        //  returns a reference to the string
        public ktString Append(string Str)
        {
            m_Content += Str;

            return this;
        }
        // Appends (concate, in the end) a char to the string
        //  returns a reference to the string
        public ktString Append(char Ch)
        {
            m_Content.PadRight(1, Ch);

            return this;
        }
        // Appends (concate, in the end) a string to the string
        //  returns a reference to the string
        public ktString Prepend(ktString Str)
        {
            return Prepend(Str.GetValue());
        }
        // Appends (concate, in the end) a string to the string
        //  returns a reference to the string
        public ktString Prepend(string Str)
        {
            m_Content = Str + m_Content;

            return this;
        }
        // Appends (concate, in the end) a char to the string
        //  returns a reference to the string
        public ktString Prepend(char Ch)
        {
            m_Content.PadLeft(1, Ch);

            return this;
        }

        // Change the value of the character at Pos. n
        public void SetChar(uint Pos, char Ch)
        {
            string Str = m_Content.Substring(0, (int)Pos);

            Str.PadLeft(1, Ch);

            m_Content = Str + m_Content.Substring((int)Pos + 1);
        }

        // Add 'Count' coppies of 'PadCh' to the string to the end (default)
        //  or to the beginning. Removes spaces from the right (default) or left
        public ktString Pad(uint Count, char PadCh, bool FromRight)
        {
            if (FromRight)
            {
                if (PadCh == 0)
                    m_Content.PadRight((int)Count);
                else
                    m_Content.PadRight((int)Count, PadCh);
            }
            else
            {
                if (PadCh == 0)
                    m_Content.PadLeft((int)Count);
                else
                    m_Content.PadLeft((int)Count, PadCh);
            }

            return this;
        }
        // Wrapper for ::Pad() above
        public ktString Pad(uint Count, char PadCh)
        {
            return Pad(Count, PadCh, true);
        }
        // Wrapper for ::Pad() above
        public ktString Pad(uint Count)
        {
            return Pad(Count, (char)0, true);
        }

        // Trim the string (remove whitespaces etc, from the beginning and/or end)
        public ktString Trim(ktStripType St, ktString WS)
        {
            if ((St == ktStripType.both) || (St == ktStripType.leading))
            {
                m_Content = m_Content.TrimStart(WS.GetValue().ToCharArray());
            }
            if ((St == ktStripType.both) || (St == ktStripType.trailing))
            {
                m_Content = m_Content.TrimEnd(WS.GetValue().ToCharArray());
            }

            return this;
        }
        // Trim the string (remove whitespaces etc, from the beginning and/or end)
        public ktString Trim(ktStripType St)
        {
            return Trim(St, "\n\t ");
        }
        // Trim the string (remove whitespaces etc, from the beginning and end)
        public ktString Trim()
        {
            return Trim(ktStripType.both);
        }

        // Remove a part of the string (simillary to "Str = Str.SubStr(0,Pos) + Str.SubStr(Pos+Len)")
        //  Starting at Pos and goes 'Length' characters forward. Default or -1 means all the way to the end.
        public ktString Remove(uint Pos, int Len)
        {
            if (Pos < 0)
                return this;

            if ((Pos >= Length()) || (Pos + Len >= Length()))
            {
                Clear();
                return this;
            }

            // Get the first part of the string (that we should save)
            string Tmp = SubStr(0, (int)Pos);

            // If we shouldn't go on to the end...
            if (Len != -1)
            {
                // Add the rest of the string (after the that we should remove)
                Tmp = Tmp + SubStr((int)(Pos + Len));
            }

            // Save the new string...
            m_Content = Tmp;

            // Return a reference to this string/object
            return this;
        }
        // Remove a part of the string (simillary to "Str = Str.SubStr(0,Pos) + Str.SubStr(Pos+Len)")
        //  Starting at Pos and goes 'Length' characters forward. Default or -1 means all the way to the end.
        public ktString Remove(uint Pos)
        {
            return Remove(Pos, -1);
        }
        // Only Remove the first character
        public ktString RemoveFirst()
        {
            return Remove(0, 1);
        }
        // Only Remove the last character
        public ktString RemoveLast()
        {
            return Remove((uint)(Length() - 1));
        }

        // Replace First (or all) occurrences of substring with another one.
        //   (returns number of replacements)
        public int Replace(ktString OldString, ktString NewString, bool ReplaceAll)
        {
            // Init Variables
            int Count = 0;
            string Tmp;
            int Pos, tPos = 0;

            // Get the first position
            Pos = tPos = Find(OldString);

            // Go on as long as we can find 'OldString'
            while (tPos >= 0)
            {
                // Cut out everything before the 'OldString'
                Tmp = m_Content.Substring(0, Pos);
                // Insert 'NewString' (I.e. replace)
                Tmp = Tmp + NewString.GetValue();
                // Append everything after 'OldString'
                Tmp = Tmp + m_Content.Substring(Pos + OldString.Length());

                // Save
                m_Content = Tmp;

                // Find the next position
                tPos = SubStr(Pos + NewString.Length()).Find(OldString);
                Pos = Pos + NewString.Length() + tPos;

                // Count how many replaces we made
                Count++;

                // If we only should replace the first occurence of 'OldString'
                if (!ReplaceAll)
                    // Stop (-1 indicates that we didn't find 'OldString' again)
                    Pos = -1;
            }

            // Return the Count
            return Count;
        }

        // Make the whole string in lower case (OBS! Modifies the string)
        //    Also returns a reference to the string
        public ktString MakeLower()
        {
            m_Content = m_Content.ToLower();

            return this;
        }
        // Make the whole string in upper case (OBS! Modifies the string)
        //    Also returns a reference to the string
        public ktString MakeUpper()
        {
            m_Content = m_Content.ToUpper();

            return this;
        }
        // Make the whole string in lower case (OBS! Modifies the string)
        public void LowerCase()
        {
            m_Content = m_Content.ToLower();
        }
        // Make the whole string in upper case (OBS! Modifies the string)
        public void UpperCase()
        {
            m_Content = m_Content.ToUpper();
        }

        // "Information functions"
        // State Functions
        // Check if the string Is empty...
        public bool IsEmpty()
        {
            return (m_Content == "");
        }
        // Check if the string Is empty...
        public bool IsNull()
        {
            return (m_Content == "");
        }

        // Check if the string Is alpha (A-Z a-z {space} and added characters)
        public bool IsAlpha(ktString AddedChars)
        {
            bool Res = true;

            // If the string is empty...
            if (IsNull())
            {
                Res = false;
            }

            // Go thru all characters and check if they is "alpha"
            for (uint I = 0; (I < Length()) && (Res); I++)
            {
                // Use a internal function to check if the character match...
                Res = IsAlpha(GetChar(I), AddedChars);
            }

            return Res;
        }
        // Check if the string Is alpha (A-Z a-z {space} and default Extra characters)
        public bool IsAlpha()
        {
            return IsAlpha(".,_-+");
        }
        // Check if the string Is a number (integer or float) (opposite to IsAlpha)
        //   you can change the "rules", if the string can contain signs (+ or -, also includes
        //	^ and e or E) and if the string can contain dec. dots [.] (be a float)
        public bool IsNumber(bool AllowSigns, bool AllowDots)
        {
            bool Res = true;
            bool HasE = false;

            // An empty string makes no number...
            if (IsEmpty() || IsSameAs(".") || IsSameAs("+") || IsSameAs("-"))
            {
                Res = false;
            }


            // Go thru all characters
            //  or stop when we found something that we didn't like
            for (uint I = 0; (I < Length()) && (Res); I++)
            {
                switch (GetChar(I))
                {
                    // If we found a sign
                    case '-':
                    case '+':
                        {
                            // If shouldn't allow signs
                            //   or we've passed the first char
                            //    (as we only can allow signs in the beginning)
                            if ((!AllowSigns) || (I > 0))
                            {
                                // It's no a valid number
                                Res = false;
                            }
                            else
                            {
                                Res = true;
                            }
                            break;
                        }
                    case 'e':
                    case 'E':
                        {
                            // If we should allow signs (and there wasn't any e/E earlier
                            if ((AllowSigns) && (!HasE))
                            {
                                Res = true;
                                // Found an e/E
                                HasE = true;
                            }
                            else
                            {
                                // Not valid...
                                Res = false;
                            }
                            break;
                        }
                    // If we found a decimal dot
                    case '.':
                        {
                            // If we should allow dots...
                            if (AllowDots)
                            {
                                // We can only allow one dot per string...
                                AllowDots = false;

                                // It's OK
                                Res = true;
                                // We don't allow dots (anymore?)
                            }
                            else
                            {
                                Res = false;
                            }
                            break;
                        }
                    // If it's a number...
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            Res = true;
                            break;
                        }
                    // None of the above..
                    default:
                        {
                            // We've found an "illegal" character
                            Res = false;
                            break;
                        }
                } // end switch
            } // end for()

            // Return
            return Res;
        }
        // Wrapper for IsNumber( AllowSigns, AllowDot [= true] )
        public bool IsNumber(bool AllowSigns)
        {
            return IsNumber(AllowSigns, true);
        }
        // Wrapper for IsNumber( AllowSigns [= true], AllowDot [= true] )
        public bool IsNumber()
        {
            return IsNumber(true, true);
        }
        // Check if the string Is one word (checks if it only include A-Z,a-z and any
        //   characters that may be added, default: '_' [underscore])
        // OBS! This function doesn'T alowe spaces (even if it's added using 'AddedChars')
        public bool IsWord(ktString AddedChars)
        {
            bool Res = true;

            // We simply use IsAlpha() to check if it's valid
            Res = IsAlpha(AddedChars.GetValue());

            // If IsAlpha() OK'd the string
            //  Check for spaces...
            if ((Res) && (Contains(" ")))
            {
                // Find space and that's not allowed...
                Res = false;
            }

            return Res;
        }
        // Wrapper for IsNumber( AllowSigns [= true], AllowDot [= true] )
        public bool IsWord()
        {
            return IsWord("_");
        }
        // Check if the string only contains white spaces
        public bool IsWhiteSpace()
        {
            // Default.. (!?)
            bool Res = true;

            // If the string is empty
            if (Length() == 0)
            {
                // A empty string can't contain anything...
                Res = false;
            }

            // go thru the char in the string
            for (uint I = 0; (I < Length()) && (Res); I++)
            {
                // Check the char. at Pos. I
                switch (GetChar(I))
                {
                    // if it's an whitespace (ws) (space - ' ', tab - '\t' or newline - '\n')
                    case ' ':
                    case '\t':
                    case '\n':
                        // Only ws so far
                        Res = true;
                        break;
                    // Not an ws
                    default:
                        // stop the loop and return false
                        Res = false;
                        break;
                }
            }

            // return
            return Res;
        }

        // Get the length of the string
        public int Len()
        {
            return m_Content.Length;
        }
        // Get the length of the string
        public int Length()
        {
            return Len();
        }
        // Get the length of the string
        public int Size()
        {
            return Len();
        }

        // Get the number of occurrenses of a character in the string
        public int Freq(char Ch)
        {
            int Cnt = 0;

            // Go thru all characters
            for (uint I = 0; I < Length(); I++)
            {
                // Found a occurence
                if (GetChar(I) == Ch)
                    // Count it...
                    Cnt++;
            }

            return Cnt;
        }
        // Get the number of occurrenses of a substring in the string
        public int Freq(ktString Str)
        {
            int Cnt = 0;

            // Go thru the string
            for (int I = 0; I < Length(); I++)
            {
                // Found a occurence
                if (SubStr(I, Str.Length()) == Str)
                {
                    // Skip the occurence..
                    //   We don't want to Count all occurences of "ab" (for instance)
                    //    when we're looking for "abab"
                    I += Str.Length() - 1;
                    // Count it...
                    Cnt++;
                }
            }

            return Cnt;
        }
        // Get the number of occurrenses of a substring in the string
        //   Wrapper for ::Freq( ktString )
        public int Freq(string Str)
        {
            return Freq(new ktString(Str));
        }

        // Search Functions
        // Find a substring in the string
        //   either search front-to-end (default) or end-to-front
        public int Find(ktString Needle, bool FromEnd)
        {
            int Pos = -1;
            int AddI = 1, StartI = 0, StopI = Length() -1;

            // If we should go backwards...
            if (FromEnd)
            {
                // Go backwards (Count down)
                AddI = -1;
                // start at the end
                StartI = Length() - Needle.Length();
                // Stop in the beginning
                StopI = 1;
            }

            // Go thru all characters (either forward or reverse...)
            for (int I = StartI; ((AddI * I) <= StopI) && (Pos < 0); I += AddI)
            {
                // If we found an occurence
                if (SubStr(I, Needle.Length()) == Needle)
                {
                    // Save it's position
                    Pos = I;
                }
            }

            // Return the position
            return Pos;
        }
        // Find a substring in the string (front-to-end)
        //  Wrapper for ::Find( Needle, FromEnd )
        public int Find(ktString Needle)
        {
            return Find(Needle, false);
        }
        // Find a substring in the string (front-to-end)
        //  Wrapper for ::Find( Needle, FromEnd )
        public int Find(String Needle)
        {
            return Find(Needle, false);
        }
        // Find the first occurence of a substring
        //  Wrapper for ::Find( Needle, FromEnd )
        public int First(ktString Needle)
        {
            return Find(Needle, false);
        }
        // Find the first occurence of a substring
        //  Wrapper for ::Find( Needle, FromEnd )
        public int Index(ktString Needle)
        {
            return Find(Needle, false);
        }
        // Find the first occurence of a character
        //  Wrapper for ::Find( Needle, FromEnd )
        public int Index(char Ch)
        {
            return Find(Ch, false);
        }
        // Find the first occurence of a substring
        //  Wrapper for ::Find( Needle, FromEnd )
        public int IndexOf(ktString Needle)
        {
            return Find(Needle, false);
        }
        // Find the first occurence of a character
        //  Wrapper for ::Find( Needle, FromEnd )
        public int IndexOf(char Ch)
        {
            return Find(Ch, false);
        }

        public int FindK(ktString Haystack)
        {
            int Pos = -1;

            /*			while (!true) {
                        }*/

            return Pos;
        }

        // Match and compare...
        // Find, but with a mask (using ? and *)
        public bool Matches(ktString Mask)
        {
            /* ** <note>
               For now we just cross our fingers and hopes that this function (/method?)
               actually works. We haven't runned this code yet.
                (BTW this goes with most of the whole class...)</note>
            ***/
            bool Match = false;
            bool GoOn = true;

            int Pos;
            ktTripple Joker = 0; // 1 == * and 2 == ?

            ktString MatchTmp;
            ktString Str = new ktString(GetValue());

            // Don't stop until we say so
            while (GoOn)
            {
                Joker = 0;

                // If there's no more "Joker"
                if ((Pos = ktRegEx.Find(Mask.GetValue(), "\\*|\\?")) < 0)
                {
                    // If (the rest of) the mask matches the string
                    if (Mask == Str)
                    {
                        // Stop looping
                        GoOn = false;
                        // And "OK it"
                        Match = true;
                    }
                    else
                    {
                        // Stop looping
                        GoOn = false;
                        // Didn' match...
                        Match = false;
                    }
                }
                else
                {
                    // Find a "Joker" and get everything infront of it
                    MatchTmp = Mask.SubStr(0, Pos);
                    // Remove what we got from the mask
                    Mask.Remove(0, MatchTmp.Length());

                    // Chech the "Joker"
                    //  If the first char is * in the "new mask"
                    //   then indicate that
                    if (Mask.First() == '*')
                        Joker = 1;
                    //  Or if it's ?...
                    else
                        Joker = 2;

                    // Remove the "Joker"
                    Mask.RemoveFirst();

                    // If this part of the mask doesn't match... (simply not a match)
                    if ((!MatchTmp.IsEmpty()) && (MatchTmp != Str.SubStr(0, MatchTmp.Length())))
                    {
                        // Stop looping
                        GoOn = false;
                        Match = false;
                    }
                    else
                    {
                        // As we now that this part of the mask matches the string
                        //  Remove that part from the string
                        Str.Remove(0, MatchTmp.Length());

                        // If the "Joker" is *
                        if (Joker == 1)
                        {
                            // Get the position of the next "Joker"
                            Pos = ktRegEx.Find(Mask.GetValue(), "\\*|\\?");

                            // If we didn't find a new "Joker"
                            if (Pos < 0)
                            {
                                // get the length of the rest of the mask ("find the end")
                                Pos = Mask.Length();
                                // If Pos is 0
                                if (Pos == 0)
                                    // No more mask...
                                    Pos = -1;
                            }

                            // If Pos is less than 0
                            //  This (should) means that the * was the last thing in
                            //   the mask and should therefor match the rest of the string
                            if (Pos < 0)
                            {
                                // Stop looping
                                GoOn = false;
                                // It's a match...
                                Match = true;
                            }
                            else
                            {
                                // Get the next part of the mask
                                MatchTmp = Mask.SubStr(0, Pos);
                                // Remove that part of the mask...
                                Mask.Remove(0, Pos);

                                // If the "submask" matches the corresponding "substring"
                                if ((Pos = Str.Find(MatchTmp)) >= 0)
                                {
                                    // The substring matched...
                                    Match = true;
                                    // Remove the matched "substring"
                                    Str.Remove(0, Pos + MatchTmp.Length());

                                    // If the mask now is empty
                                    if (Mask.IsEmpty())
                                    {
                                        // Stop looping
                                        GoOn = false;
                                        // If the string isn't empty
                                        if (!Str.IsEmpty())
                                            // As the mask is empty but not the
                                            //  the string, it means that it's
                                            //  an missmatch...
                                            Match = false;
                                    }
                                    // It wasn't a match (the match failed...
                                }
                                else
                                {
                                    // Stop looping
                                    GoOn = false;
                                    Match = false;
                                }
                            }
                            // If the "Joker" is ?
                        }
                        else
                        {
                            // Just remove the first char
                            Str.RemoveFirst();

                            // As the mask has matched so far...
                            Match = true;
                        }
                    }

                }

                // If the mask is empty (and we can go on)
                //   Means that the mask matched the string
                if (((Str == "") || (Mask == "")) && (GoOn))
                {
                    // Stop looping
                    GoOn = false;
                    // And "OK it"
                    Match = true;
                }
            }

            return Match;
        }

        // Check if the string contains the substring Str
        public bool Contains(ktString Str)
        {
            return (Find(Str) >= 0);
        }

        // Check if the string contains the substring Str
        public bool StartsWith(ktString Str, out ktString Rest)
        {
            bool Ret = false;

            if ((Length() >= Str.Length()) &&
                 (Str == SubStr(0, Str.Length())))
            {
                Ret = true;
                Rest = SubStr(Str.Length());
            }
            else
                Rest = "";

            return Ret;
        }
        // Check if the string contains the substring Str
        //  Wrapper for ::StartsWith( Str, Rest )
        public bool StartsWith(ktString Str)
        {
            ktString Rest;
            return StartsWith(Str, out Rest);
        }

        // Compare the string with 'Str' (ref. strcmp) (case-sensitive)
        public int Cmp(ktString Str)
        {
            return String.Compare(Str.GetValue(), m_Content);
        }
        // Compare the string with 'Str' (ref. strcmp) (case-sensitive)
        public int CmpNoCase(ktString Str)
        {
            return String.Compare(Str.AsLower().GetValue(), AsLower().GetValue());
        }

        // Compare the string with 'Str' (case-sensitive or -insensitive)
        //   Actually a wrapper for both function types above (Cmp() and CmpNoCase())
        //    controll case-sensitvity with the second argument
        public bool Compare(ktString Str, bool IgnoreCase)
        {
            /*
                Here we simply use Cmp() or CmpNoCase()
                depending on if we should ignore the case or not.
                Then we compare the result from that with 0
                  (as 0 means that the string did match)
            */
            if (IgnoreCase)
                return CmpNoCase(Str) == 0;
            else
                return Cmp(Str) == 0;
        }
        // Compare the string with 'Str' (case-sensitive)
        //   Wrapper for ::Compare( Str, IgnoreCase )
        public bool Compare(ktString Str)
        {
            return Compare(Str, false);
        }
        // Compare the string with 'Str' (case-sensitive)
        //   Wrapper for ::Compare( Str, IgnoreCase )
        public bool IsSameAs(ktString Str, bool IgnoreCase)
        {
            return Compare(Str, IgnoreCase);
        }
        // Compare the string with 'Str' (case-sensitive)
        //   Wrapper for ::Compare( Str, IgnoreCase )
        public bool IsSameAs(ktString Str)
        {
            return Compare(Str, false);
        }
        // Compare the string with 'Str' (case-sensitive)
        //   Wrapper for ::Compare( Str, IgnoreCase )
        public override bool Equals(object Obj)
        {
            if (Obj == null) return false;

            if (this.GetType() != Obj.GetType()) return false;
            // TODO: Other types???

            // safe because of the GetType check
            ktString Str = (ktString)Obj;

            return Compare(Str, false);
        }

        public override int GetHashCode()
        {
            return m_Content.GetHashCode();
        }

        // Operators...
        // Implentation of the negation operator
        public static bool operator !(ktString Str)
        {
            return !Str.IsEmpty();
        }

        // Assignment of a string
        public static implicit operator ktString(string Str)
        {
            return new ktString(Str);
        }
        // Assignment of a Character
        public static implicit operator ktString(char Ch)
        {
            return new ktString(Ch);
        }
        // Assignment of a string
        public static implicit operator string(ktString Str)
        {
            return Str.GetValue();
        }
        /*/ Assignment to a Character
        public static implicit operator char( ktString Str )
        {
            return Str.First();
        }*/

        // Get a character at a specific position
        public char this[uint Index]
        {
            get
            {
                return GetChar(Index);
            }
            set
            {
                SetChar(Index, value);
            }
        }

        public static ktString operator +(ktString Str, string Value)
        {
            return new ktString(Str.GetValue() + Value);
        }
        public static ktString operator +(ktString Str, ktString Value)
        {
            return new ktString(Str.GetValue() + Value.GetValue());
        }
        public static ktString operator +(ktString Str, char Value)
        {
            return Str + new ktString(Value);
        }
        public static ktString operator +(string Value, ktString Str)
        {
            return new ktString(Value + Str.GetValue());
        }
        public static ktString operator +(char Value, ktString Str)
        {
            ktString StrN = new ktString(Value);
            return StrN + Str;
        }

        //		public static ktString operator+= ( ktString Str, string Value )
        //		{
        //			return Str.Append( Value );
        //		}
        //		public static ktString operator+= ( ktString Str, ktString Value )
        //		{
        //			return Str.Append( Value );
        //		}
        //		public static ktString operator+= ( ktString Str, char Value )
        //		{
        //			return Str.Append( Value );
        //		}

        /*public static ktString operator<< ( ktString Str, char Value )
        {
            return Str.Append( Value );
        }*/

        public static bool operator ==(ktString Str, string Value)
        {
            return Str.Compare(Value);
        }
        public static bool operator ==(ktString Str, ktString Value)
        {
            return Str.Compare(Value);
        }
        public static bool operator ==(ktString Str, char Value)
        {
            return Str.Compare(Value);
        }

        public static bool operator !=(ktString Str, string Value)
        {
            return !Str.Compare(Value);
        }
        public static bool operator !=(ktString Str, ktString Value)
        {
            return !Str.Compare(Value);
        }
        public static bool operator !=(ktString Str, char Value)
        {
            return !Str.Compare(Value);
        }

        public static bool operator >(ktString Str, string Value)
        {
            return Str.Cmp(Value) > 0;
        }
        public static bool operator >(ktString Str, ktString Value)
        {
            return Str.Cmp(Value) > 0;
        }
        public static bool operator >(ktString Str, char Value)
        {
            return Str.Cmp(Value) > 0;
        }

        public static bool operator >=(ktString Str, string Value)
        {
            return Str.Cmp(Value) >= 0;
        }
        public static bool operator >=(ktString Str, ktString Value)
        {
            return Str.Cmp(Value) >= 0;
        }
        public static bool operator >=(ktString Str, char Value)
        {
            return Str.Cmp(Value) >= 0;
        }

        public static bool operator <(ktString Str, string Value)
        {
            return Str.Cmp(Value) < 0;
        }
        public static bool operator <(ktString Str, ktString Value)
        {
            return Str.Cmp(Value) < 0;
        }
        public static bool operator <(ktString Str, char Value)
        {
            return Str.Cmp(Value) < 0;
        }

        public static bool operator <=(ktString Str, string Value)
        {
            return Str.Cmp(Value) <= 0;
        }
        public static bool operator <=(ktString Str, ktString Value)
        {
            return Str.Cmp(Value) <= 0;
        }
        public static bool operator <=(ktString Str, char Value)
        {
            return Str.Cmp(Value) <= 0;
        }

        // Properties....
        // Length of the string
        /*public int Length
        {
            get
            {
                return Length();
            }
        }*/

        //// Private...
        // Check if Ch is an alpha character (or one of the "added ones")
        private bool IsAlpha(char Ch, ktString AddedChars)
        {
            // Check if the char is valid
            switch (Ch)
            {
                case 'a':
                case 'A':
                case 'b':
                case 'B':
                case 'c':
                case 'C':
                case 'd':
                case 'D':
                case 'e':
                case 'E':
                case 'f':
                case 'F':
                case 'g':
                case 'G':
                case 'h':
                case 'H':
                case 'i':
                case 'I':
                case 'j':
                case 'J':
                case 'k':
                case 'K':
                case 'l':
                case 'L':
                case 'm':
                case 'M':
                case 'n':
                case 'N':
                case 'o':
                case 'O':
                case 'p':
                case 'P':
                case 'q':
                case 'Q':
                case 'r':
                case 'R':
                case 's':
                case 'S':
                case 't':
                case 'T':
                case 'u':
                case 'U':
                case 'v':
                case 'V':
                case 'w':
                case 'W':
                case 'x':
                case 'X':
                case 'y':
                case 'Y':
                case 'z':
                case 'Z':
                    {
                        return true;
                    }
                // Not a-z or A-Z
                default:
                    {
                        if (AddedChars != "")
                        {
                            ktString s_chr = " ";
                            s_chr[0] = Ch;
                            if (AddedChars.Find(s_chr) >= 0)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
            }
        }

        #region properties
        public static ktString EmptyString
        {
            get { return new ktString("");  }
        }
        #endregion

        private string m_Content;
    }
}
