using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KacTalk;
using ktMainLib;

namespace ktTalkers
{
    public abstract class ktTalkerChannel : ktIntObj, IDisposable
    {
        public ktTalkerChannel(ktTalker Parent) :
            base("ktTalkerChannel",0)
        {
            m_URI = "";
            m_Key = "";
            m_Initiated = false;

            m_TalkerParent = Parent;
        }

        public abstract bool Initiate(ktString URI, ktString APIKey);

        public abstract void RequestAvailableObjects();
        public abstract ktString CheckVersion(bool full = false);

        public virtual void OpenConnection() { }
        public virtual void CloseConnection() { }

        protected void AddAvailableObject(ktString o)
        {
            m_TalkerParent.AddAvailableObject(o);
        }


        protected ktString m_URI;
        protected ktString m_Key;
        protected bool m_Initiated;
        protected ktTalker m_TalkerParent;

        public virtual void Dispose()
        {
            CloseConnection();
        }
    }
}
