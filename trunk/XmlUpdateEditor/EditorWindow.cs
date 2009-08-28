/*
 * Process Hacker - 
 *   XML update editor window
 * 
 * Copyright (C) 2009 dmex
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace XPath
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
        #region Windows Form Designer generated code

        private System.Windows.Forms.Button refreshItemsBtn;
        private System.Windows.Forms.Label label1;
        private Label label4;
        private TextBox hashtxtBox;
        private ComboBox titlecomboBox;
        private Button removeItemBtn;
        private Button insertItemBtn;
        private Button updateItemBtn;
        private Label timelbl;
        private TextBox versiontxtbox;
        private TextBox msgtxtbox;
        private Label label3;
        private Label label5;
        private Button openXML_FileBtn;
        private DateTimePicker dateTimePicker1;
        private Label label2;
        private TextBox updateURLtxtBox;
        private TextBox textBox1;
        private RadioButton radioIsbeta;
        private RadioButton radioIsAlpha;
        private RadioButton radioIsRelease;
        private Label label6;

        /// <summary>
        /// 
        /// </summary>
		public Form1()
		{
			InitializeComponent();
		}

		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.refreshItemsBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.hashtxtBox = new System.Windows.Forms.TextBox();
            this.titlecomboBox = new System.Windows.Forms.ComboBox();
            this.removeItemBtn = new System.Windows.Forms.Button();
            this.insertItemBtn = new System.Windows.Forms.Button();
            this.updateItemBtn = new System.Windows.Forms.Button();
            this.timelbl = new System.Windows.Forms.Label();
            this.versiontxtbox = new System.Windows.Forms.TextBox();
            this.msgtxtbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.openXML_FileBtn = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.updateURLtxtBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radioIsbeta = new System.Windows.Forms.RadioButton();
            this.radioIsAlpha = new System.Windows.Forms.RadioButton();
            this.radioIsRelease = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // refreshItemsBtn
            // 
            this.refreshItemsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.refreshItemsBtn.Location = new System.Drawing.Point(244, 430);
            this.refreshItemsBtn.Name = "refreshItemsBtn";
            this.refreshItemsBtn.Size = new System.Drawing.Size(72, 24);
            this.refreshItemsBtn.TabIndex = 12;
            this.refreshItemsBtn.Text = "Refresh";
            this.refreshItemsBtn.Click += new System.EventHandler(this.refreshItemsBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Source:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Hash:";
            // 
            // hashtxtBox
            // 
            this.hashtxtBox.Location = new System.Drawing.Point(67, 120);
            this.hashtxtBox.Name = "hashtxtBox";
            this.hashtxtBox.Size = new System.Drawing.Size(249, 22);
            this.hashtxtBox.TabIndex = 4;
            // 
            // titlecomboBox
            // 
            this.titlecomboBox.Location = new System.Drawing.Point(67, 42);
            this.titlecomboBox.Name = "titlecomboBox";
            this.titlecomboBox.Size = new System.Drawing.Size(249, 21);
            this.titlecomboBox.TabIndex = 1;
            this.titlecomboBox.SelectedIndexChanged += new System.EventHandler(this.titlecomboBox_SelectedIndexChanged);
            // 
            // removeItemBtn
            // 
            this.removeItemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeItemBtn.Location = new System.Drawing.Point(168, 430);
            this.removeItemBtn.Name = "removeItemBtn";
            this.removeItemBtn.Size = new System.Drawing.Size(72, 24);
            this.removeItemBtn.TabIndex = 11;
            this.removeItemBtn.Text = "Remove";
            this.removeItemBtn.Click += new System.EventHandler(this.removeItemBtn_Click);
            // 
            // insertItemBtn
            // 
            this.insertItemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.insertItemBtn.Location = new System.Drawing.Point(90, 430);
            this.insertItemBtn.Name = "insertItemBtn";
            this.insertItemBtn.Size = new System.Drawing.Size(72, 24);
            this.insertItemBtn.TabIndex = 10;
            this.insertItemBtn.Text = "Insert";
            this.insertItemBtn.Click += new System.EventHandler(this.insertItemBtn_Click);
            // 
            // updateItemBtn
            // 
            this.updateItemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateItemBtn.Location = new System.Drawing.Point(12, 430);
            this.updateItemBtn.Name = "updateItemBtn";
            this.updateItemBtn.Size = new System.Drawing.Size(72, 24);
            this.updateItemBtn.TabIndex = 9;
            this.updateItemBtn.Text = "Update";
            this.updateItemBtn.Click += new System.EventHandler(this.updateItemBtn_Click);
            // 
            // timelbl
            // 
            this.timelbl.AutoSize = true;
            this.timelbl.Location = new System.Drawing.Point(12, 72);
            this.timelbl.Name = "timelbl";
            this.timelbl.Size = new System.Drawing.Size(49, 13);
            this.timelbl.TabIndex = 40;
            this.timelbl.Text = "Version:";
            // 
            // versiontxtbox
            // 
            this.versiontxtbox.Location = new System.Drawing.Point(67, 69);
            this.versiontxtbox.Name = "versiontxtbox";
            this.versiontxtbox.Size = new System.Drawing.Size(249, 22);
            this.versiontxtbox.TabIndex = 2;
            // 
            // msgtxtbox
            // 
            this.msgtxtbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.msgtxtbox.Location = new System.Drawing.Point(12, 190);
            this.msgtxtbox.Multiline = true;
            this.msgtxtbox.Name = "msgtxtbox";
            this.msgtxtbox.Size = new System.Drawing.Size(301, 211);
            this.msgtxtbox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Date:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 174);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Update Message:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Title:";
            // 
            // openXML_FileBtn
            // 
            this.openXML_FileBtn.Location = new System.Drawing.Point(244, 12);
            this.openXML_FileBtn.Name = "openXML_FileBtn";
            this.openXML_FileBtn.Size = new System.Drawing.Size(72, 24);
            this.openXML_FileBtn.TabIndex = 0;
            this.openXML_FileBtn.Text = "Open XML";
            this.openXML_FileBtn.Click += new System.EventHandler(this.openXML_FileBtn_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(67, 95);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(249, 22);
            this.dateTimePicker1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "URL:";
            // 
            // updateURLtxtBox
            // 
            this.updateURLtxtBox.Location = new System.Drawing.Point(67, 148);
            this.updateURLtxtBox.Name = "updateURLtxtBox";
            this.updateURLtxtBox.Size = new System.Drawing.Size(249, 22);
            this.updateURLtxtBox.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(322, 11);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(450, 439);
            this.textBox1.TabIndex = 13;
            // 
            // radioIsbeta
            // 
            this.radioIsbeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioIsbeta.AutoSize = true;
            this.radioIsbeta.Location = new System.Drawing.Point(90, 407);
            this.radioIsbeta.Name = "radioIsbeta";
            this.radioIsbeta.Size = new System.Drawing.Size(56, 17);
            this.radioIsbeta.TabIndex = 7;
            this.radioIsbeta.Text = "IsBeta";
            this.radioIsbeta.UseVisualStyleBackColor = true;
            // 
            // radioIsAlpha
            // 
            this.radioIsAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioIsAlpha.AutoSize = true;
            this.radioIsAlpha.Location = new System.Drawing.Point(152, 407);
            this.radioIsAlpha.Name = "radioIsAlpha";
            this.radioIsAlpha.Size = new System.Drawing.Size(63, 17);
            this.radioIsAlpha.TabIndex = 8;
            this.radioIsAlpha.Text = "IsAlpha";
            this.radioIsAlpha.UseVisualStyleBackColor = true;
            // 
            // radioIsRelease
            // 
            this.radioIsRelease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioIsRelease.AutoSize = true;
            this.radioIsRelease.Checked = true;
            this.radioIsRelease.Location = new System.Drawing.Point(12, 407);
            this.radioIsRelease.Name = "radioIsRelease";
            this.radioIsRelease.Size = new System.Drawing.Size(72, 17);
            this.radioIsRelease.TabIndex = 41;
            this.radioIsRelease.TabStop = true;
            this.radioIsRelease.Text = "IsRelease";
            this.radioIsRelease.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(784, 462);
            this.Controls.Add(this.radioIsRelease);
            this.Controls.Add(this.radioIsAlpha);
            this.Controls.Add(this.radioIsbeta);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.updateURLtxtBox);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.openXML_FileBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.hashtxtBox);
            this.Controls.Add(this.titlecomboBox);
            this.Controls.Add(this.removeItemBtn);
            this.Controls.Add(this.insertItemBtn);
            this.Controls.Add(this.updateItemBtn);
            this.Controls.Add(this.timelbl);
            this.Controls.Add(this.versiontxtbox);
            this.Controls.Add(this.msgtxtbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.refreshItemsBtn);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XML Update Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        String _xmlfile;
        XPathDocument _doc;
        XPathNavigator _nav;
        XPathExpression _expr;
        XPathNodeIterator _iterator;

		[STAThread]
		static void Main() 
		{
            Application.EnableVisualStyles();
			Application.Run(new Form1());
		}

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dateTimePicker1.CustomFormat = "dd/MM/yyyy hh:mm tt";

            //// Add the keywords to the list
            //SyntaxRichTextBox1.Settings.Keywords.Add("xml");
            //SyntaxRichTextBox1.Settings.Keywords.Add("schema");
            //SyntaxRichTextBox1.Settings.Keywords.Add("simpleType");
            //SyntaxRichTextBox1.Settings.Keywords.Add("restriction");
            //SyntaxRichTextBox1.Settings.Keywords.Add("complexType");
            //SyntaxRichTextBox1.Settings.Keywords.Add("element");
            //SyntaxRichTextBox1.Settings.Keywords.Add("encoding");
            //SyntaxRichTextBox1.Settings.Keywords.Add("version");
            //SyntaxRichTextBox1.Settings.Keywords.Add("xmlns");
            //SyntaxRichTextBox1.Settings.Keywords.Add("xs");
            //SyntaxRichTextBox1.Settings.Keywords.Add("xmlns");
            //SyntaxRichTextBox1.Settings.Keywords.Add("xsd");
            //SyntaxRichTextBox1.Settings.Keywords.Add("name");
            //SyntaxRichTextBox1.Settings.Keywords.Add("base");
            //SyntaxRichTextBox1.Settings.Keywords.Add("type");
            //SyntaxRichTextBox1.Settings.Keywords.Add("value");
            //// Set the comment identifier
            //SyntaxRichTextBox1.Settings.Comment = "<!--";
            //// Enable strings and integers highlighting
            //SyntaxRichTextBox1.Settings.EnableStrings = true;
            //SyntaxRichTextBox1.Settings.EnableIntegers = true;
            //// Set the colors that will be used
            //SyntaxRichTextBox1.Settings.KeywordColor = Color.Green;
            //SyntaxRichTextBox1.Settings.CommentColor = Color.Gray;
            //SyntaxRichTextBox1.Settings.StringColor = Color.Blue;
            //SyntaxRichTextBox1.Settings.IntegerColor = Color.Red;
            //// Let's make the settings we just set valid by compiling
            //// the keywords to a regular expression
            //SyntaxRichTextBox1.CompileKeywords();

        }

        private void openXML_FileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML File (.xml)|*.xml";
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                this._xmlfile = ofd.FileName;
                this.label1.Text = "Source: " + ofd.FileName;

                this.LoadXml();
            }
            ofd.Dispose();

        }

        private void LoadXml()
        {
            if (!File.Exists(this._xmlfile))
            {
                Debug.Fail("{0} does not exist.", this._xmlfile);
                return;
            }
            StreamReader sr = File.OpenText(this._xmlfile);
            String input;

            input = sr.ReadToEnd();
            sr.Close();
            this.textBox1.Text = input;

            this._doc = new XPathDocument(this._xmlfile);
            this._nav = this._doc.CreateNavigator();

            // Compile a standard XPath expression
            this._expr = this._nav.Compile("//update//title");
            this._iterator = this._nav.Select(this._expr);

            // Iterate on the node set
            this.titlecomboBox.Items.Clear();
            try
            {
                while (this._iterator.MoveNext())
                {
                    XPathNavigator nav2 = this._iterator.Current.Clone();
                    this.titlecomboBox.Items.Add(nav2.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }

        private void titlecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = titlecomboBox.Text;
            if (str == "") 
                return;

            XmlDocument xmldoc = new XmlDocument();

            xmldoc.Load(this._xmlfile);

            XmlNodeList items = xmldoc.SelectNodes("//update");
            foreach (XmlNode xn in items)
            {
                this.msgtxtbox.Text = items.Item(this.titlecomboBox.SelectedIndex).SelectSingleNode("description").InnerText;
                this.versiontxtbox.Text = items.Item(this.titlecomboBox.SelectedIndex).SelectSingleNode("version").InnerText;
                this.dateTimePicker1.Value = DateTime.Parse(items.Item(this.titlecomboBox.SelectedIndex).SelectSingleNode("released").InnerText);
                this.updateURLtxtBox.Text = items.Item(this.titlecomboBox.SelectedIndex).SelectSingleNode("updateurl").InnerText;
                this.hashtxtBox.Text = items.Item(this.titlecomboBox.SelectedIndex).SelectSingleNode("hash").InnerText;

                switch (items.Item(this.titlecomboBox.SelectedIndex).SelectSingleNode("type").InnerText)
                {
                    case "release":
                        this.radioIsRelease.Checked = true;
                        break;
                    case "beta":
                        this.radioIsbeta.Checked = true;
                        break;
                    case "alpha":
                        this.radioIsAlpha.Checked = true;
                        break;
                    default:
                        this.radioIsRelease.Checked = true;
                        break;
                } 
            }
        }

        private void updateItemBtn_Click(object sender, EventArgs e)
        {   
            // basically the same as remove node
            XmlTextReader reader = new XmlTextReader(this._xmlfile);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            reader.Close();

            string type = null;

            if (this.radioIsRelease.Checked)
            { type = "release"; }
            else if (this.radioIsbeta.Checked)
            { type = "beta"; }
            else if (this.radioIsAlpha.Checked)
            { type = "alpha"; }

            //Select the cd node with the matching title
            XmlNode oldCd;
            XmlElement root = doc.DocumentElement;
            oldCd = root.SelectSingleNode("//update[hash='" + this.hashtxtBox.Text + "']");

            XmlElement newCd = doc.CreateElement("update");

            newCd.InnerXml =
                "<title>" + this.titlecomboBox.Text + "</title>" +
                "<description>" + this.msgtxtbox.Text + "</description>" +
                "<version>" + this.versiontxtbox.Text + "</version>" +
                "<released>" + this.dateTimePicker1.Value.ToString() + "</released>" +
                "<updateurl>" + this.updateURLtxtBox.Text + "</updateurl>" +
                "<hash>" + this.hashtxtBox.Text + "</hash>" +
                "<type>" + type + "</type>";

            root.ReplaceChild(newCd, oldCd);
            //save the output to a file 
            doc.Save(this._xmlfile);

            this.LoadXml();
        }

        private void insertItemBtn_Click(object sender, EventArgs e)
        {
            XmlTextReader reader = new XmlTextReader(this._xmlfile);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            reader.Close();
            XmlNode currNode;

            string type = null;

            if (this.radioIsRelease.Checked)
            { type = "release"; }
            else if (this.radioIsbeta.Checked)
            { type = "beta"; }
            else if (this.radioIsAlpha.Checked)
            { type = "alpha"; }

            XmlDocumentFragment docFrag = doc.CreateDocumentFragment();
            docFrag.InnerXml =
                "<update>" +
                "<title>" + this.titlecomboBox.Text + "</title>" +
                "<description>" + this.msgtxtbox.Text + "</description>" +
                "<version>" + this.versiontxtbox.Text + "</version>" +
                "<released>" + this.dateTimePicker1.Value.ToString() + "</released>" +
                "<updateurl>" + this.updateURLtxtBox.Text + "</updateurl>" +
                "<hash>" + this.hashtxtBox.Text + "</hash>" +
                "<type>" + type + "</type>" +
                "</update>";

            // insert the availability node into the document 
            currNode = doc.DocumentElement;
            currNode.InsertAfter(docFrag, currNode.LastChild);
            //save the output to a file 

            doc.Save(this._xmlfile);

            this.LoadXml();
        }

        private void removeItemBtn_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "Are you sure you want to remove this entry?", this.Text,  MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dr == DialogResult.Yes)
            {
                XmlTextReader reader = new XmlTextReader(this._xmlfile);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();

                //Select the cd node with the matching title
                XmlNode cd;
                XmlElement root = doc.DocumentElement;
                cd = root.SelectSingleNode("//update[hash='" + this.hashtxtBox.Text + "']");

                root.RemoveChild(cd);
                //save the output to a file 
                doc.Save(this._xmlfile);

                this.LoadXml();
            }
        }

        private void refreshItemsBtn_Click(object sender, System.EventArgs e)
        {
            this.LoadXml();
        }
	}
}
