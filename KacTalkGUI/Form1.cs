using KacTalk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace KacTalkGUI
{
    public partial class Form1 : Form
    {


        private kacTalk m_KT;
        private static ktDebug m_debugger;

        private static Form1 _me = null;    
        private static string m_LogText = "";
        private delegate void DebuggerLog();
        private DebuggerLog _debuggerLogDelegate;

        private ktMainLib.ktTalker m_Talker;

        public Form1()
        {
            Form1._me = this;
            _debuggerLogDelegate = new DebuggerLog(DebuggerLogMethod);

            InitializeComponent();
            m_debugger = new ktDebug();
            m_debugger.Enable();
            m_debugger.OnDebug += m_debugger_OnDebug;

            ktModule mainLib = new ktMainLib.ktMain();
            new ktIDEContext(mainLib.GetContext("Main"));

            m_Talker = new ktMainLib.ktTalker("URI=localhost/kacTalk_PHP;PROTO=HTTP;KEY=658B8C89-BA37-42D6-8D02-7119A5FA613A");
            m_Talker.Initiate();

            m_Talker.AddExtraParameter("ID", "u1234455");

            m_Talker.GetObject("kto");
            m_Talker.RunMethod("kto");

            m_KT = new kacTalk(false);
            m_KT.AllowMissingEOL = true;
            m_KT.MathMode = true;
            m_KT.AddModule( mainLib );
            //m_KT.AddModule( new ktIDEModule() );
            m_KT.AddDefaultValues();

            LoadExample(2);
        }

        static void m_debugger_OnDebug(string Info)
        {
            Form1.m_LogText = Info;
            Form1._me.Invoke(Form1._me._debuggerLogDelegate);
        }
        public void DebuggerLogMethod()
        {
            LogText(m_LogText);
        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (clearVariablesBeforeRunToolStripMenuItem.Checked)
            {
                if ( m_KT.MainBlock != null )
                    m_KT.MainBlock.ClearVariables();
            }
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        public static void LogText(string p)
        {
            String str = "[" + DateTime.Now.ToString() + "]: " + p;

            Form1._me.logTB.Text += str + "\n";
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {

                worker.ReportProgress(0,"Parsing code");
                if (m_KT.Parse(codeTB.Text))
                {
                    worker.ReportProgress(50, "Parsing: Done");
                }
                else
                {
                    worker.ReportProgress(0,"Parsing: Failed (" + m_KT.GetError() + ")");
                }

                worker.ReportProgress(55, "Running code");
                m_KT.Run();
                //ktDebug.Log("Variables:\n" + ktXML.FromList(m_KT.MainBlock.Variables).AsXML());
            }
                catch (ktError Error)
            {
                worker.ReportProgress(0, "ERROR:" + Error.ToString());
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_KT.MainBlock.Variables != null)
            {
                ktDebug.Log("Variables:\n" + m_KT.MainBlock.Variables.Get_R());
                variableLB.Items.Clear();

                foreach (ktList L in m_KT.MainBlock.Variables)
                {
                    ktValue Var = (ktValue)L.Node.Value;
                    variableLB.Items.Add(Var.Name + ": " + Var.Export());
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            String Str = e.UserState.ToString();
            if (Str.StartsWith("ERROR:"))
            {
                MessageBox.Show(Str, "An error occured with the script!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LogText(Str);
        }

        private void codeTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadExample(int e)
        {
            switch (e)
            {
                case 1:
                    {
                        codeTB.Text = "a=42;\r\na=null;\r\nif ( a < b ) {\r\n    a++;\r\n}\r\noutput(\"a:\",a,\"; b:\",b);";
                        break;
                    }
                case 2:
                    {
                        codeTB.Text = "a=pi._Times(2);\r\nb=pi._Times(2).Round(1);";//\r\noutput a;\r\na.Times(7);\r\noutput(a);";
                        break;
                    }
            }
        }

        private void example1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadExample(1);
        }

        private void example2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadExample(2);
        }

        private void ktTalkToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Talker.Dispose();
        }
    }
}
