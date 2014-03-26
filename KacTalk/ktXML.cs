using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KacTalk
{
    public class ktXMLNode : ktNode
    {
        public ktXMLNode(ktXMLNode Node)
            : base(Node)
        {
            //ktDebug.Log( "ktXMLNode( ktXMLNode )" );
        }
        public ktXMLNode(ktNode Node)
            : base(Node)
        {
            //ktDebug.Log( "ktXMLNode( ktNode )" );
        }
        public ktXMLNode(ktString N)
            : base(N)
        {
            //ktDebug.Log( "ktXMLNode( " + N + " )" );
        }
    }

    public class ktXML : ktList
    {
        public ktXML()
            : base()
        {
            m_Type = "ktXML";
            m_iType = 0;
        }
        public ktXML(ktList List)
            : this()
        {
            if (List == null)
            {
                return;
            }

            if (List.Node != null)
            {
                m_Node = new ktXMLNode(List.Node);
            }
            else
            {
                m_Node = new ktXMLNode("post");
            }
            //ktDebug.Log( "POST:" + List.Get_R( "\t", true ) + "=###=#==#=#==##==#=#=#=");

            List.Reset();
            foreach (ktList L in List)
            {
                if (L != null)
                {
                    AddList(new ktXML(L));
                }
            }
        }

        public ktString AsXML(bool IncludeDeclaration, ktString Prefix)
        {
            // Define the variable to store the xml-structure
            ktString XML = new ktString();

            // Should we include the XML-declaration
            if (IncludeDeclaration)
            {
                // Do so...
                XML = Prefix + "<?xml version=\"" + m_Version + "\" encoding=\"" + m_Encoding + "\"?>\n";
            }

            // Define...
            ktString StartElm = "", EndElm = "", Content = "", Name = "";
            bool First = true;

            // If the list has info...
            if (m_Node != null)
            {
                // Add beginning
                StartElm = Prefix + "<";
                EndElm = "</";

                Name.SetValue(m_Node.Name);
                Name.Trim();

                // No name??
                if (Name.IsEmpty())
                {
                    StartElm += "NN";
                    EndElm += "NN";
                    // Yippie, name...
                }
                else
                {
                    // Add name
                    StartElm += Name;
                    EndElm += Name;
                }

                // Add end...
                StartElm += ">";
                EndElm += ">";

                // If the list has a value
                if (m_Node.Value != null)
                {
                    // Special, just for blocks...
                    if (m_Node.Value.GetType() == typeof(ktBlock))
                    {
                        ktList L = ((ktBlock)m_Node.Value).Lines;
                        if (L != null)
                        {
                            // Get it...
                            Content = ktXML.FromList(L).AsXML(IncludeDeclaration, m_Prefix + Prefix);
                        }
                    }
                    else if ((m_Node.Value.GetType() == typeof(ktToken)) &&
                              (((ktToken)m_Node.Value).Type == ktTokenType.Block))
                    {
                        if (((ktToken)m_Node.Value).Block == null)
                        {
                            goto EndOfNode;
                        }
                        ktList L = ((ktToken)m_Node.Value).Block.Lines;
                        if (L != null)
                        {
                            // Get it...
                            Content = ktXML.FromList(L).AsXML(IncludeDeclaration, m_Prefix + Prefix);
                        }
                    }
                    else
                    {
                        // Get it...
                        Content = m_Node.Value.ToString();
                    }
                }
                // No node info...
            }
            else
            {
                // Use default...
                StartElm = Prefix + "<POST>";
                EndElm = "</POST>";
            }

        EndOfNode:

            // Little "hack" if it,s the "first level" 
            if (Prefix.IsEmpty())
            {
                First = false;
            }

            // Go thrugh the child nodes
            foreach (ktXML N in this)
            {
                // If it's the first
                if (First)
                {
                    // If content isn't empty
                    if (!Content.IsEmpty())
                    {
                        // Add approiate prefixes...
                        Content = Prefix + m_Prefix + Content + "\n"/* + Prefix*/;
                    }
                    // No more first...
                    First = false;
                }

                // Get the childs xml-structure (extended prefix and no XML-decl.)
                Content += N.AsXML(false, Prefix + m_Prefix) + "\n";
            }

            // Remove trialing newlines
            Content.Trim(ktStripType.trailing, "\n");

            // If the "content"/structure contains newlines or elements
            if ((!Content.IsEmpty()) && (Content.Contains("\n") || Content.Contains("<")))
            {
                // Add newline after start-element 
                StartElm = StartElm + "\n";

                // hum... Add newline if the content doesn't end with one!?
                if ((!Content.StartsWith("\n")) && (Content.Last() != '\n'))
                {
                    Prefix.Prepend("\n");
                }

                // Add Prefix before end-element and a newline after..
                EndElm = Prefix + EndElm + "\n";
            }

            // Put together start content/childs and end element...
            XML += StartElm + Content + EndElm;

            // Done.. Return the structure
            return XML;// + Get_R();
        }
        public ktString AsXML(bool ID)
        {
            return AsXML(ID, "");
        }
        public ktString AsXML()
        {
            return AsXML(true);
        }

        public static ktXML FromList(ktList List)
        {
            ktXML XML = new ktXML(List);

            return XML;
        }

        protected ktString m_Version = "1.0";
        protected ktString m_Encoding = "UTF-8";
        protected ktString m_Prefix = "\t";
    }
}
