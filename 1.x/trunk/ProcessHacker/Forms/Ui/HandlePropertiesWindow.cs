/*
 * Process Hacker - 
 *   handle properties window
 *
 * Copyright (C) 2009 wj32
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Ui
{
    public partial class HandlePropertiesWindow : Form
    {
        public HandlePropertiesWindow(SystemHandleEntry handle)
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

            this.handleDetails1.ObjectHandle = handle;
        }

        public void Init()
        {
            this.handleDetails1.Init();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPermissions_Click(object sender, EventArgs e)
        {
            //if (_objectHandle != null)
            //{
            //    try
            //    {
            //        SecurityEditor.EditSecurity(
            //            this,
            //            SecurityEditor.GetSecurableWrapper(_objectHandle),
            //            _name,
            //            NativeTypeFactory.GetAccessEntries(NativeTypeFactory.GetObjectType(_typeName))
            //            );
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Unable to edit security: " + ex.Message, "Security Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }
    }
}
