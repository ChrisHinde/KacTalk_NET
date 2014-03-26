using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    /// <summary>
    /// The different formats that can be applied to the log messages
    /// </summary>
    public enum ktDebugFormat
    {
        Plain = 0,
        XML,
        HTML,
        Flat_XML,
        Flat_HTML
    }
    /// <summary>
    /// The different types of the log messages
    /// </summary>
    public enum ktDebugType
    {
        None = 0
    }

    public delegate void ktOnDebugEventHandler(string Info);

    public class ktDebug : ktIntObj
    {
        public ktDebug()
        {
            m_Debugger = this;
        }

        /// <summary>
        /// Enable the debugging with specified options
        /// </summary>
        /// <param name="Format">Sets the type of the format that debuginformation should be outputted in</param>
        /// <param name="Level">Sets the level of the debuginformation displayed</param>
        /// <param name="Types">Sets the type of debuginformation that is displayed</param>
        public void Enable(ktDebugFormat Format, int Level, int Types)
        {
            // Set properties
            m_DebugFormat = Format;
            m_DebugLevel = Level;
            m_AcceptedTypes = Types;

            // Enable...
            m_Enabled = true;
        }
        /// <summary>
        /// Enable the debugging with specified options (Wrapper for ::Enable( Format, Level, Types ) )
        /// </summary>
        /// <param name="Level">Sets the level of the debuginformation displayed</param>
        /// <param name="Types">Sets the type of debuginformation that is displayed</param>
        public void Enable(int Level, int Types)
        {
            Enable(ktDebugFormat.Plain, Level, Types);
        }
        /// <summary>
        /// Enable the debugging with specified options (Wrapper for ::Enable( Format, Level, Types ) )
        /// </summary>
        /// <param name="Level">Sets the level of the debuginformation displayed</param>
        public void Enable(int Level)
        {
            Enable(Level, 0);
        }
        /// <summary>
        /// Enable the debugging with specified options (Wrapper for ::Enable( Format, Level, Types ) )
        /// </summary>
        /// <param name="Format">Sets the type of the format that debuginformation should be outputted in</param>
        public void Enable(ktDebugFormat Format)
        {
            Enable(Format, 0, 0);
        }
        /// <summary>
        /// Enable the debugging with specified options (Wrapper for ::Enable( Format, Level, Types ) )
        /// </summary>
        public void Enable()
        {
            Enable(ktDebugFormat.Plain);
        }

        /// <summary>
        /// Disable the debugging...
        /// </summary>
        public void Disable()
        {
            // Disable...
            m_Enabled = false;
        }


        /// <summary>
        /// Log/store information
        /// </summary>
        /// <param name="Info">The text/information that should be logged</param>
        /// <param name="Level">The "level" of the information to be logged</param>
        /// <param name="Type">The type of the information that should be logged</param>
        public void LogInfo(ktString Info, int Level, ktDebugType Type)
        {
            if (!m_Enabled) { return; }

            switch (m_DebugFormat)
            {
                case ktDebugFormat.Plain:
                    {
                        LogPlain(Info);
                        break;
                    }
            }
        }
        /// <summary>
        /// Log information (Wrapper for ::LogInfo( Info, Level, Type ) )
        /// </summary>
        /// <param name="Info">The text/information that should be logged</param>
        public void LogInfo(ktString Info)
        {
            LogInfo(Info, 0, ktDebugType.None);
        }
        /// <summary>
        /// Log information (static Wrapper for ::LogInfo( Info, Level, Type ) )
        /// </summary>
        /// <param name="Info">The text/information that should be logged</param>
        /// <param name="Level">The "level" of the information to be logged</param>
        /// <param name="Type">The type of the information that should be logged</param>
        public static void Log(ktString Info, int Level, ktDebugType Type)
        {
            if (ktDebug.Debugger == null) { return; }

            ktDebug.Debugger.LogInfo(Info, Level, Type);
        }
        /// <summary>
        /// Log information (static Wrapper for ::LogInfo( Info ) )
        /// </summary>
        /// <param name="Info">The text/information that should be logged</param>
        public static void Log(ktString Info)
        {
            if (ktDebug.Debugger == null) { return; }

            ktDebug.Debugger.LogInfo(Info);
        }
        public static void Log(ktError Err)
        {
            if (ktDebug.Debugger == null) { return; }

            ktDebug.Debugger.LogInfo(Err.ToString());
        }


        /// <summary>
        /// Log the information as plain text
        /// </summary>
        /// <param name="Info">The text/information that should be logged</param>
        protected void LogPlain(ktString Info)
        {
            if (!m_Enabled) { return; }

            if (ktDebug.WrapLevel > 0)
            {
                if ( OnDebug != null )
                {
                    OnDebug(Info);
                }
                Info.Prepend(ktDebug.GetPrefix());
                /*				Info.Replace( "\n", "<ktDebug::Wrap::n>", true );
                                Info.Replace( "<ktDebug::Wrap::n>", "\n" + Prefix, true );*/
            }

            Console.WriteLine(Info);
        }

        /// <summary>
        /// Generate the prefix (indentation) that "should" be added to the beginning of the line
        /// </summary>
        public static ktString GetPrefix()
        {
            ktString Prefix = new ktString();

           /* for (int I = 0; I < ktDebug.WrapLevel; I++)
            {
                Prefix += ">  ";
            }*/

            return Prefix;
        }

        /// <summary>
        /// Reference to the main debugger (an object of this class)
        /// </summary>
        public static ktDebug Debugger
        {
            get { return m_Debugger; }
        }
        /// <summary>
        /// Reference to the main debugger (an object of this class)
        /// </summary>
        public static ktDebug D
        {
            get { return m_Debugger; }
        }
        /// <summary>
        /// Is it Enabled?
        /// </summary>
        public static bool Enabled
        {
            get { return m_Debugger.m_Enabled; }
        }
        /// <summary>
        /// Is it Enabled?
        /// </summary>
        public bool IsEnabled
        {
            get { return this.m_Enabled; }
        }

        public event ktOnDebugEventHandler OnDebug;

        /// <summary>
        /// The main debug-object...
        /// </summary>
        protected static ktDebug m_Debugger = null;
        /// <summary>
        /// Tells if the debugging is enable
        /// </summary>
        protected bool m_Enabled = false;
        /// <summary>
        /// The levele of indentation
        /// </summary>
        public static int WrapLevel = 0;

        /// <summary>
        /// The format of the outputted debugmessages
        /// </summary>
        protected ktDebugFormat m_DebugFormat = 0;

        /// <summary>
        /// The debuglevel
        /// </summary>
        protected int m_DebugLevel = 0;
        /// <summary>
        /// The debuglevel
        /// </summary>
        protected int m_AcceptedTypes = 0;

    }
}
