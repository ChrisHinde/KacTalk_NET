using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktNode : ktIntObj
    {
        public ktNode(string N, object V, bool ABKT)
            : base("ktNode", 0, ABKT)
        {
            Name = N;
            Value = V;
        }
        public ktNode(string N, object V)
            : base("ktNode", 0)
        {
            Name = N;
            Value = V;
        }
        public ktNode(string N)
            : base("ktNode", 0)
        {
            Name = N;
            Value = null;
        }
        public ktNode(object V)
            : base("ktNode", 0)
        {
            Name = "";
            Value = V;
        }
        public ktNode(ktNode Node)
            : base("ktNode", 0)
        {
            Name = Node.Name;
            Value = Node.Value;
        }

        public bool Check(string Str)
        {//ktDebug.Log( "Check( " + Name + " == " + Str + " )" );
            return (Name == Str);
        }/*
		virtual public bool Check( int Val )
		{
			return false;
		}*/
        virtual public bool Check(object Val, bool CheckName)
        {
            return false;
        }
        public bool Check(object Val)
        {
            return Check(Val, true);
        }

        virtual public ktString Get_R(string Prefix)
        {
            string Ret = Prefix + "[";
            Ret += Name + "] => " + ExportValue() + "\n";

            return Ret;
        }
        virtual public ktString Get_R()
        {
            return Get_R("");
        }
        virtual public new string ToString()
        {
            if ((Name == null) || (Name == ""))
            {
                if (Value.GetType() == typeof(string))
                {
                    return (string)Value;
                }
                else if (Value.GetType() == typeof(ktString))
                {
                    return ((ktString)Value).GetValue();
                }
                else
                {
                    return Value.ToString();
                }
            }
            else
            {
                return Get_R();
            }
        }
        virtual public string ValueToString()
        {
            if (Value != null)
            {
                if (Value.GetType() == typeof(string))
                {
                    return (string)Value;
                }
                else if (Value.GetType() == typeof(ktString))
                {
                    return ((ktString)Value).GetValue();
                }
                else
                {
                    return Value.ToString();
                }
            }
            else
            {
                return ":null";
            }
        }
        public virtual ktString ExportValue()
        {
            if (Value != null)
            {
                if (Value.GetType() == typeof(string))
                {
                    ktString Str = new ktString((string)Value);
                    return Str.Export();
                }
                else
                {
                    try
                    {
                        ktString Str = new ktString();
                        Str = ((ktIntObj)Value).Export();
                        return Str;
                    }
                    catch (Exception Err)
                    {
                        Err.ToString();
                        return Value.ToString();
                    }
                }
            }
            else
            {
                return ":null";
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Name = "";
            Name = null;

            try
            {
                ((IDisposable)Value).Dispose();
            }
            catch (Exception) { }

            GC.SuppressFinalize(this);
        }

        #endregion

        public string Name;
        public object Value;
    }


    public class ktList : ktIntObj, IEnumerable, IEnumerator
    {
        protected class ktListEnumerator : IEnumerator
        {
            public ktListEnumerator(ktList List)
                : base()
            {
                m_List = List;
            }

            public void Reset()
            {
                m_List.Reset();
            }
            public bool MoveNext()
            {
                try
                {
                    m_List.ToNext();
                    return true;
                }
                catch (ktError)
                {
                    return false;
                }
            }
            public object Current
            {
                get { return m_List.CurrentNode; }
            }

            private ktList m_List;
        }
        protected class ktNodeEnumerable : IEnumerable, IEnumerator
        {
            public ktNodeEnumerable(ktList List)
                : base()
            {
                m_List = List;
            }

            public void Reset()
            {
                m_List.Reset();
            }
            public bool MoveNext()
            {
                try
                {
                    m_List.ToNext();
                    return true;
                }
                catch (ktError)
                {
                    return false;
                }
            }
            public object Current
            {
                get { return m_List.CurrentNode.Node; }
            }

            public IEnumerator GetEnumerator()
            {
                return (IEnumerator)this;
            }

            private ktList m_List;
        }

        #region Properties
        /// <summary>
        /// Give back how many nodes there are.
        /// </summary>
        public int Count
        {
            get
            {
                return m_iCount;
            }
        }
        /// <summary>
        /// Give back how many nodes there are.
        /// </summary>
        public bool IsCountModified
        {
            get
            {
                return m_IsCountModified;
            }
        }
        /// <summary>
        /// Gives back the Node
        /// </summary>
        public ktNode Node
        {
            get
            {
                return m_Node;
            }
            set
            {
                m_Node = value;
            }
        }
        /// <summary>
        /// Gives back the current Node
        /// </summary>
        public ktList CurrentNode
        {
            get
            {
                return m_lCurrent;
            }
        }
        /// <summary>
        /// Gives back the current Node
        /// </summary>
        public object Current
        {
            get
            {
                return m_lCurrent;
            }
        }
        /// <summary>
        /// Keeps track of the index where you are
        /// </summary>
        public int CurrentNodeIndex
        {
            get
            {
                return m_iCurrent;
            }
        }
        /// <summary>
        /// Gives back the first Node/child
        /// </summary>
        public ktList First
        {
            get
            {
                return m_Head;
            }
        }
        /// <summary>
        /// Gives back the last Node/child
        /// </summary>
        public ktList Last
        {
            get
            {
                return m_Tail;
            }
        }
        /// <summary>
        /// Gives back the first Node
        /// </summary>
        public ktNode FirstNode
        {
            get
            {
                if (m_Head != null)
                {
                    return m_Head.Node;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Gives back the last Node
        /// </summary>
        public ktNode LastNode
        {
            get
            {
                if (m_Tail != null)
                {
                    return m_Tail.Node;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Gives back the next Node/sibling
        /// </summary>
        public ktList Next
        {
            get
            {
                return m_Next;
            }
        }
        /// <summary>
        /// Gives back the previous Node/sibling
        /// </summary>
        public ktList Prev
        {
            get
            {
                return m_Prev;
            }
        }
        #endregion

        /// <summary>
        /// Default Constructor
        /// SetUp our ktList
        /// </summary>
        /// <param name="obj">Value for the first Node</param>
        public ktList(object obj)
            : base("ktList", 0)
        {
            /*m_lCurrent = new ktNode(null, null, obj);
            m_lCurrent.Next = null;
            m_lCurrent.Previous = null;*/
            m_Head = null;
            m_Tail = null;
        }
        public ktList()
            : base("ktList", 0)
        {
            m_Head = null;
            m_Tail = null;
        }
        public ktList(ktList List)
            : this()
        {
            if (List == null)
            {
                return;
            }

            if (List.Node != null)
            {
                m_Node = new ktNode(List.Node);
            }
            else
            {
                m_Node = null;
            }

            List.Reset();
            foreach (ktList L in List)
            {
                AddList(new ktList(L));
            }
        }
        public ktList(Array List)
            : this()
        {
            if (List == null)
            {
                return;
            }

            foreach (object O in List)
            {
                Add(O);
            }
        }

        /// <summary>
        /// This function will add an object to the list
        /// </summary>
        /// <param name="Obj">The object to add</param>
        public bool Add(object Obj)
        {
            return AddNode(new ktNode(Obj));
        }
        /// <summary>
        /// This function will add an object with a key
        /// </summary>
        /// <param name="Name">Value for the key</param>
        /// <param name="Obj">The object to add</param>
        public bool Add(string Name, object Obj)
        {
            return AddNode(new ktNode(Name, Obj));
        }
        /// <summary>
        /// This function will add an node to the list
        /// </summary>
        /// <param name="Node">The node to add</param>
        public bool AddNode(ktNode Node)
        {
            // Can't add nothing...
            if (Node == null)
                return false; // TODO: throw exception??

            // Create a new list
            ktList List = new ktList();
            // Set the node
            List.m_Node = Node;

            // Add the list/node
            return AddList(List);
        }
        /// <summary>
        /// This function will add an list/node to the list
        /// </summary>
        /// <param name="List">The list to add</param>
        public bool AddList(ktList List)
        {
            if (List == null)
            {
                return false;
            }
            // It will be at the end
            List.m_Next = null;

            // If there is no head (the list is empty)
            if (m_Head == null)
            {
                // Let the new list be the "head"
                m_Head = List;
                // If the head is the same as the tail
            }
            else if (m_Head == m_Tail)
            {
                // Set the next node after the head to list
                m_Head.m_Next = List;
                // Let the previous node before the list be the head
                List.m_Prev = m_Head;
                // If the tail exists (it should... but just checking to be sure)
            }
            else if (m_Tail != null)
            {
                // Let the next node after the "old tail" be the list
                m_Tail.m_Next = List;
                // Let the previous node before the list be the "old tail"
                List.m_Prev = m_Tail;
            }

            // Set the list as tail
            m_Tail = List;
            // Can't be anything after the tail
            m_Tail.m_Next = null;

            m_iCount++;

            // Modified
            m_IsModified = m_IsCountModified = true;

            // It all worked fine...
            return true;
        }

        /// <summary>
        /// This function will return the number of childs/nodes
        /// </summary>
        public int GetCount()
        {
            // Is it empty??
            if (m_Head == null)
                return m_iCount = 0;

            // If the count isn't modified, no need to recount..
            if (!IsCountModified)
                return m_iCount;

            // Init...
            int Count = 0;

            // Reset the current node
            Reset();
            // Go thru all childs and count
            foreach (ktList L in this)
            {
                L.HasError(); //L.IsEmpty(); // "hack" to avoid warnings on that L is never used
                Count++;
            }

            // Save the count
            m_iCount = Count;

            // It is correct...
            m_IsCountModified = false;

            // Return the count
            return m_iCount;
        }
        /// <summary>
        /// This function will tell you if the list is empty
        /// </summary>
        public bool IsEmpty()
        {
            return (GetCount() == 0);
        }

        /// <summary>
        /// This function will clear/remove all nodes in the list
        /// </summary>
        public int Clear()
        {
            // Init...
            int Count = 0;
            ktList TmpList = null;

            // Go thru (Pop) the list
            while ((TmpList = this.Pop(false)) != null)
            {
                // Clear the "current" child
                TmpList.Clear();

                // Dispose...
                TmpList.Dispose();
                TmpList = null;
                Count++;
            }
            // Reset count...
            m_iCount = 0;

            // Return how many we removed
            return Count;
        }

        /// <summary>
        /// This function will get the list/child corresponding the keys
        ///  (Keys can basically be a path [similar to a filesystem or XPath]
        ///    ex: "/root/child/subchild" or just an "element"/key)
        /// </summary>
        /// <param name="Keys">The path to the node/child</param>
        /// <param name="ReturnAll">Should we return all that matches?</param>
        public ktList Get(ktString Keys, bool ReturnAll)
        {
            // Init...
            ktString Key;
            ktString TmpKeys = Keys; // Temporary to save the given value

            //  Remove leading slashs (/)..
            Keys.Trim(ktStripType.leading, "/");

            if (Keys.Contains("/"))
            {
                // Get the first key
                Key = Keys.BeforeFirst('/');
                // Remove the first key
                Keys = Keys.AfterFirst('/');
            }
            else
            {
                Key = Keys;
            }

            // If the given key matches the key of "this node" (or we didn't get a key)
            if ((m_Node != null) && (m_Node.Check(Key)) || (Key == ""))
            {
                // Return "this"
                return this;
            }

            // No elements??
            if (m_Head == null)
            {
                // there's nothing to get
                throw new ktError("The list is empty (in ktList::Get( " + TmpKeys + ", bool ))", ktERR.EMPTY);
            }

            // Init...
            ktList TmpList = null;
            ktList AllMatches = null;
            bool GoOn = true;
            Reset();

            // Should we return all matches?
            if (ReturnAll)
            {
                AllMatches = new ktList();
            }

            // Go thru the list..
            while (MoveNext() && GoOn)
            {
                // Try it...
                try
                {
                    // Get the "sublist(s)"
                    TmpList = CurrentNode.Get(Keys, ReturnAll); // Key or Keys??

                    // If we should return all
                    if (ReturnAll)
                    {
                        // Add the found list... (and then go on)
                        AllMatches.Add(TmpList);
                    }
                    else
                    {
                        // Stop looking...
                        GoOn = false;
                    }
                    // Catch an error...
                }
                catch (ktError E)
                {
                    // If it was an error other than "Not found" or empty...
                    if ((E.ErrorNumber != ktERR._404) && (E.ErrorNumber != ktERR.EMPTY))
                    {
                        // Re-throw it
                        throw E;
                    } // If
                } // Catch
            } // While

            // If we should return all matches and
            //  we found something (The list isn't empty)
            if (ReturnAll && (!AllMatches.IsEmpty()))
            {
                // Set GoOn to false (as it marks a found node/list)
                GoOn = false;
            }

            // If we should get all matches but only found one
            if (ReturnAll && (AllMatches.GetCount() == 1))
            {
                // Get the first (and only) child
                TmpList = AllMatches.First;
                // Set "match/return all" to false
                ReturnAll = false;

                // Set tail and head to null...
                AllMatches.m_Head = AllMatches.m_Tail = null;

                // Dispose of the list...
                AllMatches.Dispose();
            }

            // If we didn't find a match
            if (GoOn)
            {
                // Throw an error
                throw new ktError("Couldn't find the list '" + TmpKeys + "' (in ktList::Get(  " + TmpKeys + " ))", ktERR._404);
                // If we should return all matches...
            }
            else if (ReturnAll)
            {
                // Return the list
                return AllMatches;
                // Found an match
            }
            else
            {
                // Return it..
                return TmpList;
            }
        }
        /// <summary>
        /// This function will get the list/child corresponding the keys
        ///  (Keys can basically be a path [similar to a filesystem or XPath]
        ///    ex: "/root/child/subchild" or just an "element"/key)
        ///  (Wrapper for ::Get( Keys, ReturnAll ), only Returns first match)
        /// </summary>
        /// <param name="Keys">The path to the node/child</param>
        public ktList Get(ktString Keys)
        {
            return Get(Keys, false);
        }

        /// <summary>
        /// This function will get the list/child at the position 'Pos'
        /// </summary>
        /// <param name="Pos">The position of the node/child</param>
        public ktList Get(uint Pos)
        {

            // No elements??
            if (m_Head == null)
            {
                // there's nothing to get
                throw new ktError("The list is empty (in ktList::Get( ktString ))", ktERR.EMPTY);
            }

            // Init..
            ktList List = First;
            bool Search = true;
            uint I = 0;
            uint Start = 0;

            if (!m_IsModified)
            {
                if (Pos == m_iCurrent)
                {
                    List = m_lCurrent;

                    Search = false;
                }
                else if ((Pos < m_iCurrent) && (Pos > m_iCurrent / 2 + 1))
                {
                    List = m_lCurrent;

                    // Go to Pos:th list
                    for (I = (uint)m_iCurrent; (List != null) && (I > Pos); I--)
                        List = List.Prev;

                    Search = false;
                }
                else if (Pos > m_iCurrent)
                {
                    if (m_iCurrent < 0)
                        m_iCurrent = 0;
                    List = m_lCurrent;
                    Start = (uint)m_iCurrent;

                    Search = true;
                }
                m_IsModified = false;
            }
            else
            {
                m_iCurrent = -1;
                m_lCurrent = null;
            }

            if (Search)
            {
                // Go to Pos:th list
                for (I = Start; (List != null) && (I < Pos); I++)
                    List = List.Next;
            }

            // If I is less or more than pos
            //  we didn't find anything on Pos
            if ((I < Pos) || (I > Pos))
                return null;
            else
            {
                // Save the last...
                m_lCurrent = List;
                m_iCurrent = (int)Pos;

                // Return the list
                return List;
            }
        }
        /// <summary>
        /// This function will get the node corresponding to the keys
        ///  (Keys can basically be a path [similar to a filesystem or XPath]
        ///    ex: "/root/child/subchild" or just an "element"/key)
        /// </summary>
        /// <param name="Keys">The path to the node/child</param>
        public ktNode GetNode(ktString Keys)
        {
            return Get(Keys).Node;
        }

        /// <summary>
        /// This function will get the node at position 'Pos'
        /// </summary>
        /// <param name="Pos">The position of the node to get</param>
        public ktNode GetNode(uint Pos)
        {
            // If there's no head
            if (m_Head == null)
            {
                // there's nothing to get
                throw new ktError("The list is empty (in ktList::GetNode( int ))", ktERR.EMPTY);
            }

            // Init...
            ktList List;
            ktNode Node = null;

            // Get the child/list(/node) at position Pos.
            List = Get(Pos);
            if (List != null)
            {
                // Get the node...
                Node = List.Node;
            }

            // Return the node
            return Node;
        }

        /// <summary>
        /// This function will get the value of the node at position 'Pos'
        /// </summary>
        /// <param name="Pos">The position of the value to get</param>
        public object GetValue(uint Pos)
        {
            object Value = null;
            ktNode Node = GetNode(Pos);

            if (Node != null)
            {
                Value = Node.Value;
            }

            return Value;
        }
        /// <summary>
        /// This function will get the name of the node at position 'Pos'
        /// </summary>
        /// <param name="Pos">The position of the name to get</param>
        public ktString GetName(uint Pos)
        {
            ktString Name = new ktString();
            ktNode Node = GetNode(Pos);

            if (Node != null)
            {
                Name = Node.Name;
            }

            return Name;
        }

        /// <summary>
        /// Get any node that mathes (is named) Key
        /// </summary>
        /// <param name="Key">The path to the node/child</param>
        public ktNode GetAnyNode(ktString Key)
        {
            // If there's no head
            if (m_Head == null)
            {
                // there's nothing to get
                throw new ktError("The list is empty (in ktList::GetAnyNode( ktString ))", ktERR.EMPTY);
            }

            // If the given key matches the key of "this node" (or we didn't get a key)
            if ((m_Node != null) && (m_Node.Check(Key)) || (Key == ""))
            {
                // Return "this"
                return m_Node;
            }

            // Init...
            ktNode TmpNode = null;
            bool GoOn = true;
            Reset();

            // Go thru the list
            while ((MoveNext()) && (GoOn))
            {
                try
                {
                    // Get "subnode"
                    TmpNode = m_lCurrent.GetAnyNode(Key);
                    // Stop searching
                    GoOn = false;
                }
                catch (ktError Er)
                {
                    if (Er.ErrorNumber != ktERR._404)
                    {
                        throw Er;
                    }
                }
            }

            // If we found nothing
            if (GoOn)
            {
                // Throw an error
                throw new ktError("Couldn't find a node matching '" + Key + "' (in ktList::GetAnyNode( ktString ))", ktERR._404);
            }
            else
            {
                // Return found Node;
                return TmpNode;
            }
        }
        /// <summary>
        /// Get any child that mathes (is named) Key
        /// </summary>
        /// <param name="Key">The path to the node/child</param>
        public ktList GetAny(ktString Key)
        {
            // If there's no head
            if (m_Head == null)
            {
                // there's nothing to get
                throw new ktError("The list is empty (in ktList::GetAny( ktString ))", ktERR.EMPTY);
            }

            // If the given key matches the key of "this node" (or we didn't get a key)
            if ((m_Node != null) && (m_Node.Check(Key)) || (Key == ""))
            {
                // Return "this"
                return this;
            }

            // Init...
            ktList TmpList = null;
            bool GoOn = true;
            Reset();

            // Go thru the list
            while ((MoveNext()) && (GoOn))
            {
                try
                {
                    // Get "sublist"
                    TmpList = m_lCurrent.GetAny(Key);
                    // Stop searching
                    GoOn = false;
                }
                catch (ktError Er)
                {
                    if (Er.ErrorNumber != ktERR._404)
                    {
                        throw Er;
                    }
                }
            }

            // If we found nothing
            if (GoOn)
            {
                // Throw an error
                throw new ktError("Couldn't find a child matching '" + Key + "' (in ktList::GetAny( ktString ))", ktERR._404);
            }
            else
            {
                // Return found Node;
                return TmpList;
            }
        }
        /// <summary>
        /// Get the node(s) who has
        ///  arguments that matches the parametres.
        /// This function can be compared with Get( Keys, WA )
        ///  cause they work in, basically, the same way.
        ///  This function will look for node(s) which has
        ///  a argument-name that matches 'Arguments' and
        ///  argument-value that equals 'Value'. It will return
        ///  that or those nodes that matches on the current "level".
        ///   Ex: List.GetElement("root").GetByArg( "name|id", "foo" )
        ///         will return all nodes under root
        ///         that has the name or id "foo".
        /// </summary>
        /// <param name="Arguments">The name of the argument to check</param>
        /// <param name="Value">The value to compare  with</param>
        /// <param name="ReturnAll">Should we return all or just the first match?</param>
        public ktList GetByArg(ktString Arguments, ktString Value, bool ReturnAll)
        {
            // If there's no head
            if (m_Head == null)
            {
                // there's nothing to get
                throw new ktError("The list is empty (in ktList::GetAny( ktString ))", ktERR.EMPTY);
            }
            // TODO: Write this code!!!
            return null;
        }
        /// <summary>
        ///  Wrapper for ::GetByArg( Arguments, Value, ReturnAll )
        /// </summary>
        /// <param name="Arguments">The name of the argument to check</param>
        /// <param name="Value">The value to compare  with</param>
        public ktList GetByArg(ktString Arguments, ktString Value)
        {
            return GetByArg(Arguments, Value, false);
        }

        /// <summary>
        ///  Remove a child from the list
        /// </summary>
        /// <param name="Pos">The position of the child to remove</param>
        public bool Remove(uint Pos)
        {
            if (Pos >= GetCount())
            {
                throw new ktError("The given position (" + Pos.ToString() +
                                  ") is out of range (in ktList::Remove( uint ))", ktERR.OUT_OF_RANGE);
            }
            ktList List = null;
            ktList Next = null, Prev = null;

            try
            {
                // Get the child at 'Pos'
                List = Get(Pos);
            }
            catch (ktError) { }

            // If we found a child...
            if (List != null)
            {
                try
                {
                    // If the found child is the only (first and last) child...
                    if ((List == m_Head) && (List == m_Tail))
                    {
                        // "Empty" the list...
                        m_Head = null;
                        m_Tail = null;
                        m_lCurrent = null;
                        m_iCount = 0;
                        // It's not the only child
                    }
                    else
                    {
                        // Get it's siblings
                        Next = List.Next;
                        Prev = List.Prev;

                        // If there's something after it
                        if (Next != null)
                            // Set the next sibling's previous to the previous sibling
                            //  of the one we're removing...
                            Next.m_Prev = Prev;
                        // If there's something before it
                        if (Prev == null)
                            // Set the previous sibling's next to the next sibling
                            //  of the one we're removing...
                            Prev.m_Next = Next;
                        // If the one we're removing is the first child
                        if (List == m_Head)
                            // Change the first element to the next child
                            m_Head = Next;
                        // If the one we're removing is the last child
                        if (List == m_Tail)
                            // Change the first element to the previous child
                            m_Tail = Prev;

                        // Reset the siblings...
                        List.m_Next = null;
                        List.m_Prev = null;

                        // Decrease the counting...
                        m_iCount--;
                        // Reset the "current" (as it currently is the child we're removing...)
                        Reset();
                    }
                    // Catch any problems..
                }
                catch (ktError)
                {
                    // TODO: Should we perhaps throw an nice exception... 
                    return false;
                }
            }
            else
                // TODO: Should we perhaps throw an nice exception... 
                return false;

            return true;
        }

        /// <summary>
        ///  Remove the "current child "from the list
        /// </summary>
        public bool RemoveCurrent()
        {
            if (m_lCurrent == null)
            {
                throw new ktError("No 'current' list to remove! (in ktList::RemoveCurrent( ))", ktERR.NOTSET);
            }

            //ktList List = null;
            ktList Next = null, Prev = null;

            // Get Previous
            Prev = m_lCurrent.Prev;
            // Get Next
            Next = m_lCurrent.Next;

            // If the current (the one we are removing) is the only one 
            if ((m_lCurrent == m_Head) && (m_lCurrent == m_Tail))
            {
                // Reset..
                m_Head = null;
                m_Tail = null;
                m_lCurrent = null;
                m_iCurrent = -1;
                m_iCount = 0;

                return true;
            }

            // "Reset"...
            m_lCurrent.m_Next = m_lCurrent.m_Prev = null;
            // Re-link
            if (Prev != null) { Prev.m_Next = Next; }
            if (Next != null) { Next.m_Prev = Prev; }

            // If current was our tail
            if (m_lCurrent == m_Tail)
            {
                // Make the previous one the tail
                m_Tail = Prev;
            }
            // If current was our head... Ô_ô
            if (m_lCurrent == m_Head)
            {
                // Make the next one the head
                m_Head = Next;
                // Set current pos to 1, so we don't end up with -1
                m_iCurrent = 1;
                // And set Prev to head
                Prev = m_Head;
            }

            // CLEAR...
            m_lCurrent.Clear();

            // Change current
            //  We need to go back one step, as we would mess up a "foreach-loop" otherwise
            m_lCurrent = Prev;
            m_iCurrent--;

            // Count down...
            m_iCount--;

            return true;
        }

        /// <summary>
        ///  Remove a child from the list, identifing it by name or value 
        /// </summary>
        /// <param name="Value">The name/value of the child to remove</param>
        /// <param name="ChName">Should we check name or value of the childs</param>
        public bool RemoveByValue(ktString Value, bool ChName)
        {
            int Pos = -1;

            if ((Pos = GetPos(Value, ChName)) >= 0)
                return Remove((uint)Pos);
            else
                return false;
        }
        /// <summary>
        ///  Remove a child from the list, identifing it by name 
        /// </summary>
        /// <param name="Value">The name of the child to remove</param>
        public bool RemoveByValue(ktString Value)
        {
            return RemoveByValue(Value, true);
        }
        /// <summary>
        ///  Remove a child from the list, identifing it by name or value 
        /// </summary>
        /// <param name="Value">The name/value of the child to remove</param>
        /// <param name="ChName">Should we check name or value of the childs</param>
        public bool RemoveByValue(object Value, bool ChName)
        {
            int Pos = -1;

            if ((Pos = GetPos(Value, ChName)) >= 0)
                return Remove((uint)Pos);
            else
                return false;
        }
        /// <summary>
        ///  Remove a child from the list, identifing it by name 
        /// </summary>
        /// <param name="Value">The name of the child to remove</param>
        public bool RemoveByValue(object Value)
        {
            return RemoveByValue(Value, true);
        }

        /// <summary>
        /// Find a position for a certain name/value
        /// </summary>
        /// <param name="Value">The value to look for (as object)</param>
        /// <param name="ChName">Should we check the name or value?? (Name only works w/ strings..)</param>
        public int GetPos(object Value, bool ChName)
        {
            int I = 0;
            foreach (ktNode Node in this.GetNodes())
            {
                if (Node.Check(Value, ChName))
                    return I;
                I++;
            }

            return -1;
        }
        /// <summary>
        /// Find a position for a certain name
        /// </summary>
        /// <param name="Value">The value to look for (as object)</param>
        public int GetPos(object Value)
        {
            return GetPos(Value, true);
        }
        /// <summary>
        /// Find a position for a certain name/value
        /// </summary>
        /// <param name="Value">The value to look for (as string)</param>
        /// <param name="ChName">Should we check the name or value?? (Name only works w/ strings..)</param>
        public int GetPos(ktString Value, bool ChName)
        {
            return GetPos((object)Value, ChName);
        }
        /// <summary>
        /// Find a position for a certain name
        /// </summary>
        /// <param name="Value">The value to look for (as string)</param>
        public int GetPos(ktString Value)
        {
            return GetPos(Value, true);
        }

        /// <summary>
        /// Pop (remove) and return the last post
        /// </summary>
        public ktList Pop(bool ThrowError)
        {
            // Init...
            ktList List = m_Tail;

            // Do we have something to pop?
            if (List != null)
            {
                // If it's the last child...
                if (List == m_Head)
                {
                    // "Reset"...
                    m_Head = null;
                    m_Tail = null;
                    Reset();
                }
                else
                {
                    // If the "current child" is the last (the one we're popping)
                    if (m_lCurrent == m_Tail)
                    {
                        // Reset...
                        Reset();
                    }
                    // Set tail/last child to the second last..
                    m_Tail = List.m_Prev;

                    // If we still have a tail
                    if (m_Tail != null)
                    {
                        // Tell it that it's the last one
                        m_Tail.m_Next = null;
                    }
                }

                // Tell the popped child that it's alone...
                List.m_Prev = null;
                List.m_Next = null;

                // hm?
                m_IsModified = false;
                m_IsCountModified = false;
                // One less...
                m_iCount--;
            }
            else
            {
                List = null;
                if (ThrowError)
                {
                    throw new ktError("Tried to pop on an empty list (in ktList::Pop())",
                                      ktERR.EMPTY);
                }
            }

            // Returned the poped list
            return List;
        }
        /// <summary>
        /// Pop (remove) and return the last post
        /// </summary>
        public ktList Pop()
        {
            return Pop(true);
        }
        /// <summary>
        /// Shift/Remove and return the first child
        /// </summary>
        public ktList Shift()
        {
            // Init
            ktList List = m_Head;

            // Do we have something to shift?
            if (List != null)
            {
                // If it's the last element..
                if (List == m_Tail)
                {
                    // "Reset"
                    m_Tail = null;
                }
                // Set the next child to be the first
                m_Head = List.m_Next;
                // If we still have a head
                if (m_Head != null)
                    // Tell it that it's the first...
                    m_Head.m_Prev = null;

                // Tell the "shifted one" that it's alone
                List.m_Next = null;

                // The list is now modified...
                m_IsModified = true;
                // Reset the "current child"
                Reset();
                // Nothing to shift...
            }
            else
            {
                throw new ktError("Tried to shift an empty list (in ktList::Shift())",
                                  ktERR.EMPTY);
            }

            // Return the "shifted one"
            return List;
        }

        /// <summary>
        /// Get the the list(s) recursive as a string.
        ///  I got the idea for this function from Perl and print_r
        ///  (var_dump in PHP does the same). For those who never
        ///  have heard about those functions (or even the languages)
        ///  I'll try to give a short explanation.
        /// The function goes thru the whole list (and sublists,
        ///  it's recursive...) and creates a string that shows
        ///  the structure of the list more readable.
        ///  An output can look like this:
        ///    { 
        ///        ["foo"] => "first",
        ///        ["bar"] => { 
        ///              ["sub"] => "lol" 
        ///          }
        ///    }
        /// </summary>
        /// <param name="List">The list to show the structure of</param>
        /// <param name="Prefix">The prefix to prepend to each line</param>
        /// <param name="ValueAsKey">Should we use the value as "key" instead of the name (which is default)</param>
        /// <param name="PrintOwn">Should we include information about the "current" node/child/list</param>
        /// <param name="PrefixComp">The "complete"/original prefix</param>
        public ktString Get_R(ktList List, ktString Prefix, bool ValueAsKey, bool PrintOwn, ktString PrefixComp)
        {
            // If we got null as list, use "this"
            if (List == null)
            {
                List = this;
            }

            // Declare and Init...
            ktString Str = new ktString();
            //			ktString Key = new ktString(), Value = new ktString();
            //			ktString RealPrefix = Prefix;

            //ktList SubList = List.First;
            ktNode Node = null;

            // Should we print info about "this" node
            if (PrintOwn)
            {
                // Put together the info... 
                Str = Prefix + "[" + ((List.Node != null) ? (ValueAsKey ? List.Node.Value.ToString() : List.Node.Name) : "??") + "] => {\n";
                // Indent...
                Prefix += PrefixComp;
            }

            Reset();

            // Go thru the list
            foreach (ktList SubList in this)
            {
                // If the (sub)list has node info, (it should)
                if (SubList.Node != null)
                {
                    Node = SubList.Node;

                    // Add "post" to the string...
                    //  If the list has its own sublist...
                    if (!SubList.IsEmpty())
                    {
                        //    (Name in square brackets, followed by arrow...)
                        //   Note the "prefix" in the beginning
                        Str += Prefix + "[" + (ValueAsKey ? Node.Value.ToString() : Node.Name) + "] => {\n";
                        // If the node/element has an value/content
                        if (Node.Value != null)
                        {
                            // Add prefixes and content
                            Str += Prefix + PrefixComp + Node.ExportValue() + "\n";
                        }
                        // Use Get_R on the sublist, extend the prefix
                        //  and then add to the result..
                        // (this is what makes it recursive)
                        Str += SubList.Get_R(null, Prefix + PrefixComp, ValueAsKey, false, PrefixComp);
                        //Str += Get_R( SubList, Prefix + "\t", ValueAsKey, false );
                        // Add Prefix W/ end bracket
                        Str += Prefix + "  }";
                    }
                    else
                    {
                        // Use the Get_R of the node
                        Str += Node.Get_R(Prefix).Trim(ktStripType.both, "\n");// + "\n";
                    }
                }
                else
                {
                    Str += Prefix + "[] => {\n";
                    // If the list has its own sublist...
                    if (!SubList.IsEmpty())
                    {
                        // Use Get_R on the sublist, extend the prefix
                        //  and then add to the result..
                        // (this is what makes it recursive
                        //Str += Get_R( SubList, Prefix + "\t", ValueAsKey, false );
                        Str += SubList.Get_R(null, Prefix + PrefixComp, ValueAsKey, false, PrefixComp);
                    }
                    // Add Prefix W/ end bracket
                    Str += Prefix + "  }";
                }
                // Add newline...
                Str += "\n";
            }

            // If we include "own info"
            if (PrintOwn)
            {
                Str += PrefixComp + "  }";
            }/* else if (this.Node != null)
				Str += PrefixComp + this.Node.Get_R() + "\n";*/

            // Return the result
            return Str;
        }
        /// <summary>
        /// Get the list as string, recursive
        /// </summary>
        public ktString Get_R()
        {
            return Get_R("\t");
        }
        /// <summary>
        /// Get the list as string, recursive
        /// </summary>
        /// <param name="Prefix">The prefix to prepend to each line</param>
        /// <param name="ValueAsKey">Should we use the value as "key" instead of the name (which is default)</param>
        /// <param name="PrintOwn">Should we include information about the "current" node/child/list</param>
        public ktString Get_R(ktString Prefix, bool ValueAsKey, bool PrintOwn)
        {
            return Get_R(this, Prefix, ValueAsKey, PrintOwn, "\t");
        }
        /// <summary>
        /// Get the list as string, recursive
        /// </summary>
        /// <param name="Prefix">The prefix to prepend to each line</param>
        /// <param name="PrintOwn">Should we include information about the "current" node/child/list</param>
        public ktString Get_R(ktString Prefix, bool PrintOwn)
        {
            return Get_R(Prefix, false, PrintOwn);
        }
        /// <summary>
        /// Get the list as string, recursive
        /// </summary>
        /// <param name="Prefix">The prefix to prepend to each line</param>
        public ktString Get_R(ktString Prefix)
        {
            return Get_R(Prefix, false);
        }
        /// <summary>
        /// Export the list...
        /// </summary>
        public override string Export()
        {
            ktString Str = new ktString("(");
            ktString Name = new ktString(), Value = new ktString();
            bool First = true;

            Reset();

            foreach (ktList List in this)
            {
                Name = "";
                Value = "";

                if (!First)
                {
                    Str += ", \n";
                }
                else
                {
                    First = false;
                }

                if (List.Node != null)
                {
                    Name = List.Node.Name;
                    Value = List.Node.ExportValue();
                }

                if (!Name.IsEmpty())
                {
                    if ((!Value.IsEmpty()) && (Value != ":null") && (!List.IsEmpty()))
                    {
                        //Name = "[\"" + Name + "\"," + Value + "]";
                    }
                    else
                    {
                        //Name = "\"" + Name + "\"";
                        //Name.Append( new ktString( Name.Len() ) );
                    }
                }
                else if (!Value.IsEmpty())
                {
                    Name = Value;
                    Value = "";
                }

                if (!List.IsEmpty())
                {
                    Value = List.Export();
                }
                else if ((Value.IsEmpty()) && (Name.IsEmpty()))
                {
                    Value = ":null";
                    //Value = "\"" + Value + "\"";
                }

                if (!Name.IsEmpty())
                {
                    Str += Name;
                    if (!Value.IsEmpty())
                    {
                        Str += " => " + Value;
                    }
                }
                else
                {
                    Str += Value;
                }
            }

            Str += ")";

            return Str;
            //return Get_R( Prefix, false );
        }

        /// <summary>
        /// Goes to the next Node
        /// </summary>
        public void ToNext()
        {
            if (m_lCurrent == null)
            {
                ResetH();
                if (m_lCurrent == null)
                {
                    throw new ktError("There is no current node!", ktERR.NOTSET);
                }
                return;
            }
            // Checks whether the Next Node is null
            // if so it throws an exception.
            // You can also do nothing but I chose this.
            if (m_lCurrent.m_Next == null)
            {
                throw new ktError("There is no next node!", ktERR.NOTSET);
                // If everything is OK
            }
            else
            {
                m_lCurrent = m_lCurrent.m_Next;
                m_iCurrent++;
            }
        }
        /// <summary>
        /// Goes to the previous Node
        /// </summary>
        public void ToPrevious()
        {
            // Look at ToNext();
            if (m_lCurrent.m_Prev == null)
            {
                throw new ktError("There is no previous node!", ktERR.NOTSET);
            }
            else
            {
                m_lCurrent = m_lCurrent.m_Prev;
                m_iCurrent--;
            }
        }
        /// <summary>
        /// Goes to the index you fill in
        /// </summary>
        /// <param name="index">Index Where to go?</param>
        public void GoTo(int index)
        {
            if (m_iCurrent < index)
            {
                ToNext();
            }
            else if (m_iCurrent > index)
            {
                ToPrevious();
            }
        }

        /// <summary>
        /// Return as a string
        /// </summary>
        public override string ToString()
        {
            if (m_Head == null)
            {
                if (m_Node == null)
                {
                    return ":null";
                }
                else
                {
                    return m_Node.ToString();
                }
            }
            else
            {
                return "#List";
            }
        }

        // Mainly for IEnumerator, but good to have anyhow..
        /// <summary>
        /// Reset/Go to the start
        /// </summary>
        public void Reset()
        {
            m_iCurrent = 0;
            m_lCurrent = null;
        }
        /// <summary>
        /// Reset/Go to the start
        /// </summary>
        public void ResetH()
        {
            m_iCurrent = 0;
            m_lCurrent = m_Head;
        }
        /// <summary>
        /// Are we at the end?
        /// </summary>
        public bool AtEnd()
        {
            if ((m_iCurrent >= (GetCount() - 1)) ||
                (m_lCurrent == m_Tail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Go to the next..
        /// </summary>
        public bool MoveNext()
        {
            try
            {
                ToNext();
                return true;
            }
            catch (ktError)
            {
                //ktDebug.Log(err);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerator GetEnumerator()
        {
            this.Reset();
            return (IEnumerator)this;
        }
        public IEnumerable GetNodeEnumerable()
        {
            return new ktNodeEnumerable(this);
        }
        public IEnumerable GetNodes()
        {
            return new ktNodeEnumerable(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Node != null)
            {
                Node.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        #endregion


        /** Properties **/
        protected ktNode m_Node;

        protected int m_iCount = 0;
        protected bool m_IsCountModified = false;
        protected bool m_IsModified = false;
        protected int m_iCurrent = 0;
        protected ktList m_lCurrent = null;

        protected ktList m_Head = null;
        protected ktList m_Tail = null;

        protected ktList m_Next = null;
        protected ktList m_Prev = null;
    }
}
