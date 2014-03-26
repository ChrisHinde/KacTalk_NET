using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using KacTalk;
using ktMainLib;
using Newtonsoft.Json;

namespace ktTalkers
{
    public class ktTalkerHttpChannel : ktTalkerChannel
    {
        public ktTalkerHttpChannel(ktTalker Parent)
            : base(Parent)
        {
            m_UseQuery = false;
        }

        public override bool Initiate(ktString URI, ktString APIKey)
        {
            m_Initiated = true;
            m_URI = URI;
            m_Key = APIKey;

            OpenConnection();

            CheckVersion();
            RequestAvailableObjects();

            CloseConnection();

            return true;
        }

        public override void RequestAvailableObjects()
        {
            ktString URL = PrepBaseURL(m_URI);

            URL += CreateQuery(new ktRequestObject("_kactalk", "objects", ktTalkFormat.JSON));

            ktString response = SendHTTPRequest(URL);

            ktString[] obj = JsonConvert.DeserializeObject<ktString[]>(response);

            foreach (ktString o in obj)
            {
                AddAvailableObject(o);
            }
        }
        public override ktString CheckVersion( bool full = false )
        {
            ktString URL = PrepBaseURL(m_URI);
            ktString f = "";
            if (full)
                f = "full_";

            URL += CreateQuery(new ktRequestObject("_kactalk", f + "version", ktTalkFormat.JSON));

            return SendHTTPRequest(URL);
        }

        private ktString SendHTTPRequest(ktString URL)
        {
            try
            {
                ktString responseBody = Task.Run(() => m_HttpClient.GetStringAsync(URL)).Result;
                return responseBody;
            }
            catch (HttpRequestException e)
            {

            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("WTF??????????");
                Console.WriteLine(e.StackTrace);
            }
            return "";
        }

        private ktString CreateQuery(ktRequestObject req)
        {
            ktString query = "/";
            bool addedQMark = m_UseQuery;

            if (req.Object.IsEmpty())
            {
                throw new ktError("ktTalker needs an object to request!", ktERR.MISSING);
            }

            switch ( req.Object )
            {
                case "kt":
                case "_":
                case "kactalk":
                    {
                        query += "_kactalk";
                        break;
                    }
                default:
                    {
                        query += req.Object;
                        break;
                    }
            }
            query += "/";

            query += req.Member;

            if (req.Arguments != null)
            {
                throw new NotImplementedException();
            }

            if (req.Format != ktTalkFormat._NOT_SPECIFIED)
            {
                query += "." + ktTalker.FormatToString(req.Format);
            }

            if (!m_Key.IsEmpty())
            {
                if (!addedQMark)
                    query += "?";
                else
                    query += "&";

                query += "api_key=" + m_Key;
            }

            return query;
        }

        private ktString PrepBaseURL(ktString URL)
        {
            if (!URL.AsLower().StartsWith("http://"))
            {
                URL = "http://" + URL;
            }
            if ( m_UseQuery )
            {
                URL += "?kuery=";
            }

            return URL;
        }

        public override void OpenConnection()
        {
            if (m_ConnectionOpen) return;

            m_HttpClient = new HttpClient();

            m_ConnectionOpen = true;
        }
        public override void CloseConnection()
        {
            if (!m_ConnectionOpen) return;

            m_HttpClient.Dispose();

            m_ConnectionOpen = false;
        }

        protected HttpClient m_HttpClient;


        protected bool m_UseQuery;
        protected bool m_ConnectionOpen;
    }
}
