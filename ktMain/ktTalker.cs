using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KacTalk;
using ktTalkers;
using System.Runtime.InteropServices;

namespace ktMainLib
{
    public enum ktTalkFormat
    {
        _NOT_SPECIFIED,
        KT_XML,
        JSON,
        JSONP,
        XML,
        XML_RPC,
        APACHE_CONF,
        INI,
        OBJEKT_KTS,
        OBJECT_PHP,
        OBJECT_JS = ktTalkFormat.JSON,

        _DEFUALT = ktTalkFormat.JSON
    };

    public class ktTalker : ktIntObj, IDisposable
    {
        public ktTalker( ktString InfoStr, ktTalkerChannel TalkerChannel = null )
            : base("ktTalker", 0)
        {
            m_InfoMap = ParseInfoString(InfoStr);

            m_APIKey = GetProperty("KEY");
            m_URI = GetProperty("URI");

            if (TalkerChannel != null)
            {
                m_Channel = TalkerChannel;
            }
            else
            {
                ktString Channel = "";
                if (m_InfoMap.ContainsKey("CHANNEL"))       Channel = m_InfoMap["CHANNEL"];
                else if (m_InfoMap.ContainsKey("PROTO"))    Channel = m_InfoMap["PROTO"];
                else if (m_InfoMap.ContainsKey("PROTOCOL")) Channel = m_InfoMap["PROTOCOL"];

                if (Channel.IsEmpty())
                {
                    throw new ktError("ktTalker wasn't given a channel!");
                }

                try
                {
                    Type type = Type.GetType("ktTalkers.ktTalker" + Channel.Capitalize() + "Channel");
                    object[] args = { this };

                    m_Channel = (ktTalkerChannel)Activator.CreateInstance(type,args);
                }
                catch (Exception Exc)
                {
                    if ((Exc is ArgumentNullException) || (Exc is ArgumentException) || (Exc is NotSupportedException)
                            || (Exc is InvalidComObjectException) || (Exc is TypeLoadException))
                    {
                        throw new ktError("ktTalker couldn't create a Channel object for '" + Channel + "'!");
                    }
                    else
                    {
                        throw Exc;
                    }
                }
            }
        }

        public void Initiate()
        {
            if (m_Channel == null)
            {
                throw new ktError("ktTalker::Initiate failed due to the lack of a Channel!");
            }
            m_AvailableObjects = new ktList();

            m_Channel.Initiate( m_URI, m_APIKey );

            Console.WriteLine(m_AvailableObjects.Get_R());
        }

        public static Dictionary<ktString,ktString> ParseInfoString( ktString InfoStr )
        {
            int p = 0, p2 = 0;
            ktString property;
            ktString prop_name, prop_value;

            Dictionary<ktString, ktString> InfoMap = new Dictionary<ktString, ktString>();

            while (!InfoStr.IsEmpty())
            {
                p = InfoStr.IndexOf(';');
                if (p < 0)
                {
                    property = InfoStr;
                    p = InfoStr.Length() - 1;
                }
                else
                {
                    property = InfoStr.SubString(0, p).Trim();
                }
                p2 = property.IndexOf('=');
                prop_name = property.SubString(0, p2).AsUpper();
                prop_value = property.SubString(p2 + 1);

                InfoMap.Add(prop_name, prop_value);

                InfoStr.Remove(0, p + 1);
                InfoStr = InfoStr.Trim();
            }

            return InfoMap;
        }

        internal void AddAvailableObject(ktString o)
        {
            if (m_AvailableObjects == null)
                m_AvailableObjects = new ktList();

            m_AvailableObjects.Add(o);
        }

        public ktString GetProperty( ktString Key )
        {
            try
            {
                return m_InfoMap[Key];
            } catch (Exception)
            {
                return "";
            }
        }

        public static ktString FormatToString(ktTalkFormat format)
        {
            switch (format)
            {
                case ktTalkFormat.JSON:
                    {
                        return "json";
                    }
                case ktTalkFormat.JSONP:
                    {
                        return "jsonp";
                    }
                case ktTalkFormat.KT_XML:
                    {
                        return "ktxml";
                    }
                case ktTalkFormat.XML:
                    {
                        return "xml";
                    }
                case ktTalkFormat.XML_RPC:
                    {
                        return "rpc";
                    }
                case ktTalkFormat.OBJECT_PHP:
                    {
                        return "php";
                    }
                case ktTalkFormat.OBJEKT_KTS:
                    {
                        return "kto";
                    }
                case ktTalkFormat.APACHE_CONF:
                    {
                        return "aconf";
                    }
                case ktTalkFormat.INI:
                    {
                        return "ini";
                    }
                default:
                    {
                        throw new ktError("Unrecognized talker format: '" + format.ToString() + "', in ktTalker::FormatToString!", ktERR.UNKNOWN);
                    }
            }
        }

        public void Dispose()
        {
            if (m_Channel == null) return;
            m_Channel.Dispose();
        }


        public Dictionary<ktString, ktString> Properties { get { return m_InfoMap; } }
        public ktList AvailableObjects { get { return m_AvailableObjects; } }
        public ktString APIKey { get { return m_APIKey; } }
        public ktString CurrentURI { get { return m_URI; } }


        protected ktTalkerChannel m_Channel;
        protected Dictionary<ktString, ktString> m_InfoMap;

        protected ktList m_AvailableObjects;

        protected ktString m_APIKey;
        protected ktString m_URI;
    }

    public struct ktRequestObject
    {
        public ktString Object;
        public ktString Member;
        public Dictionary<ktString, ktString> Arguments;
        public ktTalkFormat Format;

        public ktRequestObject(ktString obj, ktString memb, ktTalkFormat form = ktTalkFormat._DEFUALT)
        {
            Object = obj;
            Member = memb;
            Arguments = null;
            Format = form;
        }
        public ktRequestObject(ktString obj, ktString memb, Dictionary<ktString, ktString> args, ktTalkFormat form = ktTalkFormat._DEFUALT)
        {
            Object = obj;
            Member = memb;
            Arguments = args;
            Format = form;
        }
    }
}
