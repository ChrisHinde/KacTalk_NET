using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    /// <summary>
    /// The different types of errors that can be cast
    /// </summary>
    public enum ktERR
    {
        ERROR = 3000,
        NOTSET,
        NOTIMP,
        UNEXP,
        NOTDEF,
        NOTDEC,
        _404,
        NOTFOUND = _404,
        MISSING,
        NULL,
        OUT_OF_RANGE,
        WRONGTYPE,
        DIV_BY_ZERO,
        EMPTY,
        CONST,
        UNKNOWN,

        REGEX_COULDNT_SET_PATTERN,

        NOERROR = 0
    }

    /// <summary>
    /// Default class/exception for errors thrown within kactalk
    /// </summary>
    public class ktError : Exception
    {
        /// <summary>
        /// Constructor for the error class (mostely used when throwing errors)
        /// </summary>
        /// <param name="Error">The error message</param>
        /// <param name="ErrorNum">The error number</param>
        /// <param name="LineNo">The line on which the error was thrown</param>
        /// <param name="CharPos">The character by which the error was thrown</param>
        public ktError(ktString Error, ktERR ErrorNum, int LineNo, int CharPos)
            : base(Error)
        {
            m_Num = ErrorNum;
            m_LineNo = LineNo;
            m_CharPos = CharPos;
        }
        /// <summary>
        /// Constructor for the error class (mostely used when throwing errors)
        /// </summary>
        /// <param name="Error">The error message</param>
        /// <param name="ErrorNum">The error number</param>
        /// <param name="LineNo">The line on which the error was thrown</param>
        public ktError(ktString Error, ktERR ErrorNum, int LineNo)
            : this(Error, ErrorNum, LineNo, -1)
        {
        }
        /// <summary>
        /// Constructor for the error class (mostely used when throwing errors)
        /// </summary>
        /// <param name="Error">The error message</param>
        /// <param name="ErrorNum">The error number</param>
        public ktError(ktString Error, ktERR ErrorNum)
            : this(Error, ErrorNum, -1, -1)
        {
        }
        /// <summary>
        /// Constructor for the error class (mostely used when throwing errors)
        /// </summary>
        /// <param name="Error">The error message</param>
        public ktError(ktString Error)
            : this(Error, ktERR.ERROR, -1, -1)
        {
        }
        /// <summary>
        /// Constructor for the error class
        /// </summary>
        /// <param name="Error">A "normal" (C#/.NET) exception</param>
        public ktError(Exception Error)
            : base(Error.Message, Error)
        {
            ktDebug.Log(Error.GetType().ToString());
        }

        /// <summary>
        /// Construct a string that displays the error information
        /// </summary>
        public override string ToString()
        {
            return "Error #" + new ktString(((int)m_Num)) + " (ktERR_" + m_Num.ToString() + "): " +
                    Message;
        }

        /// <summary>
        /// Wrap the error in a "normal" object (ktIntObj)
        /// </summary>
        /// <param name="Error">The error object</param>
        public static implicit operator ktIntObj(ktError Error)
        {
            return new ktErrorWrap(Error);
        }
        /// <summary>
        /// Unwrap the error from a "normal" object (ktIntObj)
        /// </summary>
        /// <param name="ErrorW">The error object</param>
        public static implicit operator ktError(ktErrorWrap ErrorW)
        {
            return ErrorW.Error;
        }

        /// <summary>
        /// Set the line on which the error was caused
        /// </summary>
        /// <param name="LineNo">The line number</param>
        public void SetLine(int LineNo)
        {
            //Message.Replace( "#line", LineNo.ToString() );
            m_LineNo = LineNo;
        }
        /// <summary>
        /// Set the character by which the error was caused
        /// </summary>
        /// <param name="CharPos">The character position</param>
        public void SetChar(int CharPos)
        {
            //Message.Repla
            //m_CharPos = CharPos;ce( "#char", CharPos.ToString() );
            m_CharPos = CharPos;
        }

        #region Properties
        /// <summary>
        /// Gives back the error number
        /// </summary>
        public ktERR ErrorNumber
        {
            get
            {
                return m_Num;
            }
        }
        /// <summary>
        /// Gives back the error message
        /// </summary>
        public override string Message
        {
            get
            {
                string msg = base.Message;
                if (m_LineNo >= 0)
                {
                    msg = msg.Replace("#line", m_LineNo.ToString());
                }
                if (m_CharPos >= 0)
                {
                    msg = msg.Replace("#char", m_CharPos.ToString());
                }
                msg += "(" + m_LineNo.ToString() + "," + m_CharPos.ToString() + ")";
                return msg;
            }
        }
        #endregion

        /// <summary>
        /// The error type/number
        /// </summary>
        protected ktERR m_Num;
        /// <summary>
        /// The line the error was thrown at
        /// </summary>
        protected int m_LineNo;
        /// <summary>
        /// The character at which the error was thrown
        /// </summary>
        protected int m_CharPos;
    }

    /// <summary>
    /// Class that "wraps" the error in "normal" object (ktIntObj)
    /// </summary>
    public class ktErrorWrap : ktIntObj
    {
        /// <summary>
        /// Constructor for the error wrap class
        /// </summary>
        /// <param name="Error">The error message/object</param>
        public ktErrorWrap(ktError Error)
            : base("ktError", 0)
        {
            m_Error = Error;
        }

        #region Properties
        /// <summary>
        /// Returns the "real" exception/error
        /// </summary>
        public ktError Error
        {
            get
            {
                return m_Error;
            }
        }
        #endregion

        /// <summary>
        /// Wrap the error in a "normal" object (ktIntObj)
        /// </summary>
        /// <param name="Error">The error object</param>
        public static implicit operator ktErrorWrap(ktError Error)
        {
            return new ktErrorWrap(Error);
        }
        /// <summary>
        /// Unwrap the error from a "normal" object (ktIntObj)
        /// </summary>
        /// <param name="ErrorW">The error object</param>
        public static implicit operator ktError(ktErrorWrap ErrorW)
        {
            return ErrorW.Error;
        }

        /// <summary>
        /// The "real" exception/error
        /// </summary>
        protected ktError m_Error;
    }
}
