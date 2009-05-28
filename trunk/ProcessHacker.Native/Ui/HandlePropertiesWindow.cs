using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Native.Ui
{
    public partial class HandlePropertiesWindow : Form
    {
        public delegate void HandlePropertiesDelegate(Control objectGroup, string name, string typeName);

        public event HandlePropertiesDelegate HandlePropertiesCallback;

        private string _name, _typeName;

        public HandlePropertiesWindow(SystemHandleInformation handle)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    e.Handled = true;
                }
            };

            var handleInfo = handle.GetHandleInfo();

            textName.Text = _name = handleInfo.BestName;
            if (textName.Text == "")
                textName.Text = "(unnamed object)";
            textType.Text = _typeName = handleInfo.TypeName;
            textAddress.Text = "0x" + handle.Object.ToString("x");
            textGrantedAccess.Text = "0x" + handle.GrantedAccess.ToString("x");

            try
            {
                Type accessEnumType = NativeTypeFactory.GetAccessType(handleInfo.TypeName);

                textGrantedAccess.Text += " (" + Enum.Parse(accessEnumType, handle.GrantedAccess.ToString()).ToString() + ")";
            }
            catch
            { }

            var basicInfo = handle.GetBasicInfo();

            labelReferences.Text = "References: " + (basicInfo.PointerCount - 1).ToString();
            labelHandles.Text = "Handles: " + basicInfo.HandleCount.ToString();
            labelPaged.Text = "Paged: " + basicInfo.PagedPoolUsage.ToString();
            labelNonPaged.Text = "Non-Paged: " + basicInfo.NonPagedPoolUsage.ToString();
        }

        private void HandlePropertiesWindow_Load(object sender, EventArgs e)
        {
            if (HandlePropertiesCallback != null)
            {
                try
                {
                    HandlePropertiesCallback(groupObjectInfo, _name, _typeName);
                }
                catch
                { }

                if (groupObjectInfo.Controls.Count == 0)
                    groupObjectInfo.Visible = false;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
