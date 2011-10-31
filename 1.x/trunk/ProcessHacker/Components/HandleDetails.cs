/*
 * Process Hacker - 
 *   .NET counters control
 * 
 * Copyright (C) 2009-2010 wj32
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
using System.Windows.Forms;
using ProcessHacker.Api;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Components
{
    public partial class HandleDetails : ProcessPropertySheetPage
    {
        public delegate void HandlePropertiesDelegate(Control objectGroup, string name, string typeName);

        public event HandlePropertiesDelegate HandlePropertiesCallback;
        public string _name;
        public string _typeName;

        public SystemHandleEntry ObjectHandle { get; set; }

        public void Init()
        {
            try
            {
                ObjectInformation handleInfo = ObjectHandle.GetHandleInfo();

                _name = textName.Text = handleInfo.BestName;

                if (string.IsNullOrEmpty(textName.Text))
                    textName.Text = "(unnamed object)";

                _typeName = textType.Text = handleInfo.TypeName;
                textAddress.Text = "0x" + ObjectHandle.Object.ToString("x");
                textGrantedAccess.Text = "0x" + ObjectHandle.GrantedAccess.ToString("x");

                if (ObjectHandle.GrantedAccess != 0)
                {
                    try
                    {
                        Type accessEnumType = NativeTypeFactory.GetAccessType(handleInfo.TypeName);
                        textGrantedAccess.Text += " (" + NativeTypeFactory.GetAccessString(accessEnumType, ObjectHandle.GrantedAccess) + ")";
                    }
                    catch (NotSupportedException)
                    {
                    }
                }

                ObjectBasicInformation basicInfo = ObjectHandle.GetBasicInfo();

                labelReferences.Text = "References: " + (basicInfo.PointerCount - 1);
                labelHandles.Text = "Handles: " + basicInfo.HandleCount.ToString();
                labelPaged.Text = "Paged: " + basicInfo.PagedPoolUsage.ToString();
                labelNonPaged.Text = "Non-Paged: " + basicInfo.NonPagedPoolUsage.ToString();

                if (HandlePropertiesCallback != null)
                {
                    try
                    {
                        HandlePropertiesCallback(groupObjectInfo, _name, _typeName);
                    }
                    catch
                    {
                    }

                    if (groupObjectInfo.Controls.Count == 0)
                    {
                        groupObjectInfo.Visible = false;
                    }
                    else if (groupObjectInfo.Controls.Count == 1)
                    {
                        Control control = groupObjectInfo.Controls[0];

                        // If it's a user control, dock it.
                        if (control is UserControl)
                        {
                            control.Dock = DockStyle.Fill;
                            control.Margin = new Padding(3);
                        }
                        else
                        {
                            control.Location = new System.Drawing.Point(10, 20);
                        }
                    }
                }
            }
            catch (Exception)
            { }

            this.ActiveControl = this.label1;
        }

        public HandleDetails()
        {
            InitializeComponent();

            this.ActiveControl = this.label1;

            this.label1.Refresh();
        }
    }
}
