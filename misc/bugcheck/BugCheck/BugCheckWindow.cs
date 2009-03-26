using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BugCheck
{
    public partial class BugCheckWindow : Form
    {
        private KBugCheck _KBugCheck;
        private Dictionary<string, BugCheckCode> _bugCheckCodes = new Dictionary<string, BugCheckCode>();

        public BugCheckWindow()
        {
            InitializeComponent();

            try
            {
                _KBugCheck = new KBugCheck("KBugCheck");
            }
            catch
            {
                MessageBox.Show("Could not create or open the kernel driver. Make sure you are running as an administrator.", "Bug Check",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }

            comboBugCheckCode.Items.Add("Custom...");

            try
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(Application.StartupPath + "\\db.xml");

                foreach (XmlNode node in doc.SelectSingleNode("db").SelectNodes("code"))
                {
                    BugCheckCode code = new BugCheckCode();
                    code.Code = (uint)BaseConverter.ToNumberParse(node.Attributes["id"].Value);
                    code.Name = node.Attributes["name"].Value;

                    foreach (XmlNode n in node.SelectNodes("description"))
                        code.Description = n.ChildNodes[0].Value.Trim();
                    foreach (XmlNode n in node.SelectNodes("p1"))
                        code.Param1 = n.ChildNodes[0].Value.Trim();
                    foreach (XmlNode n in node.SelectNodes("p2"))
                        code.Param2 = n.ChildNodes[0].Value.Trim();
                    foreach (XmlNode n in node.SelectNodes("p3"))
                        code.Param3 = n.ChildNodes[0].Value.Trim();
                    foreach (XmlNode n in node.SelectNodes("p4"))
                        code.Param4 = n.ChildNodes[0].Value.Trim();

                    comboBugCheckCode.Items.Add(code.Name);
                    _bugCheckCodes.Add(code.Name, code);
                }
            }
            catch
            {
                MessageBox.Show("Could not load the bug check database.", "Bug Check",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            comboBugCheckCode_SelectedIndexChanged(null, null);
        }

        private void buttonBugCheck_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to crash your computer?", "Bug Check",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                uint code = 0;

                if (comboBugCheckCode.SelectedItem.ToString() == "Custom...")
                {
                    try
                    {
                        code = (uint)BaseConverter.ToNumberParse(textCustomCode.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Could not parse the custom bug check code.", "Bug Check",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    code = _bugCheckCodes[comboBugCheckCode.SelectedItem.ToString()].Code;
                }

                try
                {
                    _KBugCheck.BugCheckEx(code,
                        (uint)BaseConverter.ToNumberParse(textParam1.Text),
                        (uint)BaseConverter.ToNumberParse(textParam2.Text),
                        (uint)BaseConverter.ToNumberParse(textParam3.Text),
                        (uint)BaseConverter.ToNumberParse(textParam4.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not invoke a bug check: " + ex.Message, "Bug Check",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBugCheckCode_SelectedIndexChanged(object sender, EventArgs e)
        {                  
            if (comboBugCheckCode.SelectedItem == null)
            {
                textCustomCode.Enabled = false;
                return;
            }

            if (comboBugCheckCode.SelectedItem.ToString() == "Custom...")
            {
                textDescription.Text = "";
                textCustomCode.Enabled = true;
            }
            else
            {
                textCustomCode.Enabled = false;

                BugCheckCode code = _bugCheckCodes[comboBugCheckCode.SelectedItem.ToString()];

                textDescription.Text = code.Name + " (0x" + code.Code.ToString("x") + ")";

                if (code.Description != null)
                    textDescription.Text += "\r\n\r\n" + code.Description + "\r\n";

                if (code.Param1 != null)
                    textDescription.Text += "\r\nParameter 1: " + code.Param1;
                if (code.Param2 != null)
                    textDescription.Text += "\r\nParameter 2: " + code.Param2;
                if (code.Param3 != null)
                    textDescription.Text += "\r\nParameter 3: " + code.Param3;
                if (code.Param4 != null)
                    textDescription.Text += "\r\nParameter 4: " + code.Param4;
            }
        }
    }
}
