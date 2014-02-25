using System;

namespace ProcessHacker
{
    partial class MemoryEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            _data = null;
            hexBoxMemory.ByteProvider = null;

            Program.MemoryEditors.Remove(this.Id);

            Program.CollectGarbage();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoryEditor));
            this.labelHexSelection = new System.Windows.Forms.TextBox();
            this.buttonValues = new System.Windows.Forms.Button();
            this.buttonGoToMemory = new System.Windows.Forms.Button();
            this.textGoTo = new System.Windows.Forms.TextBox();
            this.buttonTopFind = new System.Windows.Forms.Button();
            this.buttonNextFind = new System.Windows.Forms.Button();
            this.textSearchMemory = new System.Windows.Forms.TextBox();
            this.labelFind = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.writeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.windowMenuItem = new System.Windows.Forms.MenuItem();
            this.buttonStruct = new System.Windows.Forms.Button();
            this.hexBoxMemory = new Be.Windows.Forms.HexBox();
            this.utilitiesButtonMemory = new ProcessHacker.Components.UtilitiesButton();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // labelHexSelection
            // 
            this.labelHexSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHexSelection.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelHexSelection.Location = new System.Drawing.Point(12, 2);
            this.labelHexSelection.Name = "labelHexSelection";
            this.labelHexSelection.ReadOnly = true;
            this.labelHexSelection.Size = new System.Drawing.Size(751, 13);
            this.labelHexSelection.TabIndex = 0;
            this.labelHexSelection.Text = "Selection:";
            // 
            // buttonValues
            // 
            this.buttonValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonValues.Image = global::ProcessHacker.Properties.Resources.information;
            this.buttonValues.Location = new System.Drawing.Point(709, 328);
            this.buttonValues.Name = "buttonValues";
            this.buttonValues.Size = new System.Drawing.Size(24, 24);
            this.buttonValues.TabIndex = 9;
            this.toolTip.SetToolTip(this.buttonValues, "Show Data Representations");
            this.buttonValues.UseVisualStyleBackColor = true;
            this.buttonValues.Click += new System.EventHandler(this.buttonValues_Click);
            // 
            // buttonGoToMemory
            // 
            this.buttonGoToMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGoToMemory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonGoToMemory.Location = new System.Drawing.Point(275, 330);
            this.buttonGoToMemory.Name = "buttonGoToMemory";
            this.buttonGoToMemory.Size = new System.Drawing.Size(47, 23);
            this.buttonGoToMemory.TabIndex = 7;
            this.buttonGoToMemory.Text = "&Go";
            this.toolTip.SetToolTip(this.buttonGoToMemory, "Go to the specified address");
            this.buttonGoToMemory.UseVisualStyleBackColor = true;
            this.buttonGoToMemory.Click += new System.EventHandler(this.buttonGoToMemory_Click);
            // 
            // textGoTo
            // 
            this.textGoTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textGoTo.Location = new System.Drawing.Point(188, 332);
            this.textGoTo.Name = "textGoTo";
            this.textGoTo.Size = new System.Drawing.Size(81, 20);
            this.textGoTo.TabIndex = 6;
            this.textGoTo.Leave += new System.EventHandler(this.textGoTo_Leave);
            this.textGoTo.Enter += new System.EventHandler(this.textGoTo_Enter);
            // 
            // buttonTopFind
            // 
            this.buttonTopFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTopFind.Image = global::ProcessHacker.Properties.Resources.arrow_up;
            this.buttonTopFind.Location = new System.Drawing.Point(159, 330);
            this.buttonTopFind.Name = "buttonTopFind";
            this.buttonTopFind.Size = new System.Drawing.Size(23, 23);
            this.buttonTopFind.TabIndex = 5;
            this.toolTip.SetToolTip(this.buttonTopFind, "Top");
            this.buttonTopFind.UseVisualStyleBackColor = true;
            this.buttonTopFind.Click += new System.EventHandler(this.buttonTopFind_Click);
            // 
            // buttonNextFind
            // 
            this.buttonNextFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNextFind.Image = global::ProcessHacker.Properties.Resources.arrow_right;
            this.buttonNextFind.Location = new System.Drawing.Point(130, 330);
            this.buttonNextFind.Name = "buttonNextFind";
            this.buttonNextFind.Size = new System.Drawing.Size(23, 23);
            this.buttonNextFind.TabIndex = 4;
            this.toolTip.SetToolTip(this.buttonNextFind, "Next Result");
            this.buttonNextFind.UseVisualStyleBackColor = true;
            this.buttonNextFind.Click += new System.EventHandler(this.buttonNextFind_Click);
            // 
            // textSearchMemory
            // 
            this.textSearchMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textSearchMemory.Location = new System.Drawing.Point(48, 332);
            this.textSearchMemory.Name = "textSearchMemory";
            this.textSearchMemory.Size = new System.Drawing.Size(76, 20);
            this.textSearchMemory.TabIndex = 3;
            this.textSearchMemory.TextChanged += new System.EventHandler(this.textSearchMemory_TextChanged);
            this.textSearchMemory.Leave += new System.EventHandler(this.textSearchMemory_Leave);
            this.textSearchMemory.Enter += new System.EventHandler(this.textSearchMemory_Enter);
            // 
            // labelFind
            // 
            this.labelFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFind.AutoSize = true;
            this.labelFind.Location = new System.Drawing.Point(12, 335);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(30, 13);
            this.labelFind.TabIndex = 2;
            this.labelFind.Text = "Find:";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.windowMenuItem});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem6,
            this.writeMenuItem,
            this.menuItem2,
            this.menuItem5,
            this.menuItem4});
            this.menuItem1.Text = "&Data";
            // 
            // menuItem6
            // 
            this.vistaMenu.SetImage(this.menuItem6, global::ProcessHacker.Properties.Resources.page);
            this.menuItem6.Index = 0;
            this.menuItem6.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuItem6.Text = "&Read";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // writeMenuItem
            // 
            this.vistaMenu.SetImage(this.writeMenuItem, global::ProcessHacker.Properties.Resources.page_edit);
            this.writeMenuItem.Index = 1;
            this.writeMenuItem.Text = "&Write";
            this.writeMenuItem.Click += new System.EventHandler(this.writeMenuItem_Click);
            // 
            // menuItem2
            // 
            this.vistaMenu.SetImage(this.menuItem2, global::ProcessHacker.Properties.Resources.disk);
            this.menuItem2.Index = 2;
            this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.menuItem2.Text = "&Save...";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "-";
            // 
            // menuItem4
            // 
            this.vistaMenu.SetImage(this.menuItem4, global::ProcessHacker.Properties.Resources.door_out);
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "&Close";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 1;
            this.windowMenuItem.Text = "&Window";
            // 
            // buttonStruct
            // 
            this.buttonStruct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStruct.Image = global::ProcessHacker.Properties.Resources.bricks;
            this.buttonStruct.Location = new System.Drawing.Point(679, 328);
            this.buttonStruct.Name = "buttonStruct";
            this.buttonStruct.Size = new System.Drawing.Size(24, 24);
            this.buttonStruct.TabIndex = 8;
            this.toolTip.SetToolTip(this.buttonStruct, "View Struct...");
            this.buttonStruct.UseVisualStyleBackColor = true;
            this.buttonStruct.Click += new System.EventHandler(this.buttonStruct_Click);
            // 
            // hexBoxMemory
            // 
            this.hexBoxMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBoxMemory.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBoxMemory.HexCasing = Be.Windows.Forms.HexCasing.Lower;
            this.hexBoxMemory.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBoxMemory.LineInfoVisible = true;
            this.hexBoxMemory.Location = new System.Drawing.Point(12, 21);
            this.hexBoxMemory.Name = "hexBoxMemory";
            this.hexBoxMemory.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBoxMemory.Size = new System.Drawing.Size(751, 301);
            this.hexBoxMemory.StringViewVisible = true;
            this.hexBoxMemory.TabIndex = 1;
            this.hexBoxMemory.UseFixedBytesPerLine = true;
            this.hexBoxMemory.VScrollBarVisible = true;
            this.hexBoxMemory.SelectionStartChanged += new System.EventHandler(this.hexBoxMemory_SelectionStartChanged);
            this.hexBoxMemory.SelectionLengthChanged += new System.EventHandler(this.hexBoxMemory_SelectionLengthChanged);
            // 
            // utilitiesButtonMemory
            // 
            this.utilitiesButtonMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.utilitiesButtonMemory.HexBox = this.hexBoxMemory;
            this.utilitiesButtonMemory.Location = new System.Drawing.Point(739, 328);
            this.utilitiesButtonMemory.Name = "utilitiesButtonMemory";
            this.utilitiesButtonMemory.Size = new System.Drawing.Size(24, 24);
            this.utilitiesButtonMemory.TabIndex = 10;
            this.toolTip.SetToolTip(this.utilitiesButtonMemory, "Insert Data");
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            this.vistaMenu.DelaySetImageCalls = false;
            // 
            // MemoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 364);
            this.Controls.Add(this.hexBoxMemory);
            this.Controls.Add(this.labelFind);
            this.Controls.Add(this.utilitiesButtonMemory);
            this.Controls.Add(this.textGoTo);
            this.Controls.Add(this.textSearchMemory);
            this.Controls.Add(this.buttonGoToMemory);
            this.Controls.Add(this.labelHexSelection);
            this.Controls.Add(this.buttonTopFind);
            this.Controls.Add(this.buttonNextFind);
            this.Controls.Add(this.buttonStruct);
            this.Controls.Add(this.buttonValues);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.Name = "MemoryEditor";
            this.Text = "Memory Editor";
            this.Load += new System.EventHandler(this.MemoryEditor_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MemoryEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProcessHacker.Components.UtilitiesButton utilitiesButtonMemory;
        private System.Windows.Forms.TextBox labelHexSelection;
        private System.Windows.Forms.Button buttonValues;
        private System.Windows.Forms.Button buttonGoToMemory;
        private System.Windows.Forms.TextBox textGoTo;
        private System.Windows.Forms.Button buttonTopFind;
        private System.Windows.Forms.Button buttonNextFind;
        private System.Windows.Forms.TextBox textSearchMemory;
        private System.Windows.Forms.Label labelFind;
        private Be.Windows.Forms.HexBox hexBoxMemory;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.MenuItem writeMenuItem;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem windowMenuItem;
        private System.Windows.Forms.Button buttonStruct;
        private System.Windows.Forms.ToolTip toolTip;
    }
}