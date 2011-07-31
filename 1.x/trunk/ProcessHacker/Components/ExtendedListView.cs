/*
 * Process Hacker - 
 *   extended ListView component
 * 
 * Copyright (C) 2009 wj32
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
 * 
 * References: 
 *  http://blogs.msdn.com/hippietim/archive/2006/03/27/562256.aspx
 *  http://msdn.microsoft.com/en-us/library/bb774769(VS.85).aspx
 *  http://msdn.microsoft.com/en-us/library/ms229669.aspx
 *  http://msdn.microsoft.com/en-us/magazine/dvdarchive/cc163384.aspx      
 */

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Components
{
    public delegate void LinkClickedEventHandler(object sender, LinkClickedEventArgs e);

    public sealed class LinkClickedEventArgs : EventArgs
    {
        private ListViewGroup _group;

        public LinkClickedEventArgs(ListViewGroup group)
        {
            _group = group;
        }

        public ListViewGroup Group
        {
            get { return _group; }
        }
    }

    public class ExtendedListView : ListView
    {
        #region Control Variables

        private const int LVM_First = 0x1000;                                                     // ListView messages
        private const int LVM_HitTest = LVM_First + 18;                                    // Determines which list-view item, if any, is at a specified position.
        private const int LVM_SetGroupInfo = LVM_First + 147;                      // ListView messages Setinfo on Group
        private const int LVM_SetExtendedListViewStyle = LVM_First + 54;  // Sets extended styles in list-view controls.

        private const int LVN_First = -100;
        private const int LVN_LinkClick = (LVN_First - 84);                              // Notifies a list-view control's parent window that a link has been clicked on.
        private const int WM_LButtonUp = 0x202;                                             // Sent when the user releases the left mouse button while the cursor is in the client area of a window.
        private const int NM_DBLClk = -3;                                                          // Sent when the user double-clicks an item with the left mouse button.
                
        private bool _doubleClickChecks = true;
        private bool _doubleClickCheckHackActive = false;

        public event LinkClickedEventHandler GroupLinkClicked;

        private delegate void CallBackSetGroupState(ListViewGroup lvGroup, ListViewGroupState lvState, string task);
        private delegate void CallbackSetGroupString(ListViewGroup lvGroup, string value);

        #endregion

        public ExtendedListView()
        {
            this.DoubleBuffered = true;
            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc.
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        public bool DoubleClickChecks
        {
            get { return _doubleClickChecks; }
            set { _doubleClickChecks = value; }
        }

        private void OnGroupLinkClicked(ListViewGroup group)
        {
            if (this.GroupLinkClicked != null)
                this.GroupLinkClicked(this, new LinkClickedEventArgs(group));
        }

        public void SetGroupState(ListViewGroupState state)
        {
            this.SetGroupState(state, null);
        }

        public void SetGroupState(ListViewGroupState state, string taskLabel)
        {
            foreach (ListViewGroup lvg in this.Groups)
            {
                SetGrpState(lvg, state, taskLabel);
            }
        }

        private ListViewGroup FindGroup(int id)
        {
            foreach (ListViewGroup group in this.Groups)
            {
                if (GetGroupID(group).Value == id)
                    return group;
            }

            return null;
        }

        private static int? GetGroupID(ListViewGroup lvGroup)
        {
            int? grpId = null;
            Type grpType = lvGroup.GetType();
            if (grpType != null)
            {
                PropertyInfo pInfo = grpType.GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pInfo != null)
                {
                    object tmprtnval = pInfo.GetValue(lvGroup, null);
                    if (tmprtnval != null)
                    {
                        grpId = tmprtnval as int?;
                    }
                }
            }
            return grpId;
        }

        private void SetGrpState(ListViewGroup lvGroup, ListViewGroupState grpState, string task)
        {
            if (OSVersion.IsBelow(WindowsVersion.Vista))
                return;
            if (lvGroup == null || lvGroup.ListView == null)
                return;
            if (lvGroup.ListView.InvokeRequired)
                lvGroup.ListView.Invoke(new CallBackSetGroupState(SetGrpState), lvGroup, grpState, task);
            else
            {
                int? GrpId = GetGroupID(lvGroup);
                int gIndex = lvGroup.ListView.Groups.IndexOf(lvGroup);
                LVGroup group = new LVGroup();
                group.CbSize = Marshal.SizeOf(group);

                if (!string.IsNullOrEmpty(task))
                {
                    group.Mask =
                        ListViewGroupMask.Task |
                        ListViewGroupMask.State |
                        ListViewGroupMask.Align;

                    group.Task = task;
                    group.CchTask = task.Length;
                }
                else
                {
                    group.Mask = ListViewGroupMask.State;
                }

                group.GroupState = grpState;

                if (GrpId != null)
                {
                    group.GroupId = GrpId.Value;
                    SendMessage(base.Handle, LVM_SetGroupInfo, GrpId.Value, ref group);
                }
                else
                {
                    group.GroupId = gIndex;
                    SendMessage(base.Handle, LVM_SetGroupInfo, gIndex, ref group);
                }

                lvGroup.ListView.Refresh();
            }
        }

        #region Message Handlers

        private unsafe void OnWmReflectNotify(ref Message m)
        {
            NMHDR* hdr = (NMHDR*)m.LParam;

            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista) && hdr->code == LVN_LinkClick)
            {
                NMLVLINK link = (NMLVLINK)Marshal.PtrToStructure(m.LParam, typeof(NMLVLINK));

                this.OnGroupLinkClicked(this.FindGroup(link.SubItemIndex));
            }
            else if (hdr->code == NM_DBLClk)
            {
                if (!_doubleClickChecks && this.CheckBoxes)
                {
                    _doubleClickCheckHackActive = true;
                }
            }
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void WndProc(ref Message m)
        {
            // OptionsWindow has a problem on XP where the BackColor of all ListViewItems 
            // appears White. Here's what I've found so far:
            // 
            // * Commenting out the unsafe block fixes it.
            // * Surrounding the switch block with a try-catch block where the catch 
            //   block includes MessageBox.Show(ex.ToString()) fixes it.
            // * Replacing the unsafe block with a function like OnWmReflectNotify 
            //   fixes it.
            // 
            // Conclusion: What The Fuck?

            switch (m.Msg)
            {
                case LVM_HitTest:
                    {
                        if (_doubleClickCheckHackActive)
                        {
                            m.Result = (-1).ToIntPtr();
                            _doubleClickCheckHackActive = false;

                            return;
                        }
                    }
                    break;
                case (int)WindowMessage.Reflect + (int)WindowMessage.Notify:
                    this.OnWmReflectNotify(ref m);
                    break;
                case WM_LButtonUp:  //handle LButtonUp event and allow groups to be collapsed
                    {
                        base.DefWndProc(ref m);
                        break;
                    }

                //todo: mouse flicker bug:  http://blogs.msdn.com/oldnewthing/archive/2006/11/21/1115695.aspx
            }

            base.WndProc(ref m);
        }

        #endregion

        #region Native Signatures

        #region Structures

        /// <summary>
        /// Used to set and retrieve groups.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct LVGroup
        {
            /// <summary>
            /// Size of this structure, in bytes.
            /// </summary>
            public int CbSize;

            /// <summary>
            /// Mask that specifies which members of the structure are valid input. One or more of the following values:LVGF_NONENo other items are valid.
            /// </summary>
            public ListViewGroupMask Mask;

            /// <summary>
            /// Pointer to a null-terminated string that contains the header text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the header text.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr pszHeader;

            /// <summary>
            /// Size in TCHARs of the buffer pointed to by the pszHeader member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchHeader;

            /// <summary>
            /// Pointer to a null-terminated string that contains the footer text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the footer text.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public string pszFooter;

            /// <summary>
            /// Size in TCHARs of the buffer pointed to by the pszFooter member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchFooter;

            /// <summary>
            /// ID of the group.
            /// </summary>
            public int GroupId;

            /// <summary>
            /// Mask used with LVM_GETGROUPINFO (Microsoft Windows XP and Windows Vista) and LVM_SETGROUPINFO (Windows Vista only) to specify which flags in the state value are being retrieved or set.
            /// </summary>
            public uint stateMask;

            /// <summary>
            /// Flag that can have one of the following values:LVGS_NORMALGroups are expanded, the group name is displayed, and all items in the group are displayed.
            /// </summary>
            public ListViewGroupState GroupState;

            /// <summary>
            /// Indicates the alignment of the header or footer text for the group. It can have one or more of the following values. Use one of the header flags. Footer flags are optional. Windows XP: Footer flags are reserved.LVGA_FOOTER_CENTERReserved.
            /// </summary>
            public uint uAlign;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the subtitle text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subtitle text. This element is drawn under the header text.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public string PszSubtitle;

            /// <summary>
            /// Windows Vista. Size, in TCHARs, of the buffer pointed to by the pszSubtitle member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public uint CchSubtitle;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the text for a task link when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the task text. This item is drawn right-aligned opposite the header text. When clicked by the user, the task link generates an LVN_LINKCLICK notification.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public string Task;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszTask member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchTask;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the top description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the top description text. This item is drawn opposite the title image when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public string DescriptionTop;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionTop member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public uint CchDescriptionTop;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the bottom description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the bottom description text. This item is drawn under the top description text when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public string DescriptionBottom;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionBottom member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public uint CchDescriptionBottom;

            /// <summary>
            /// Windows Vista. Index of the title image in the control imagelist.
            /// </summary>
            public int ITitleImage;

            /// <summary>
            /// Windows Vista. Index of the extended image in the control imagelist.
            /// </summary>
            public int IExtendedImage;

            /// <summary>
            /// Windows Vista. Read-only.
            /// </summary>
            public int IFirstItem;

            /// <summary>
            /// Windows Vista. Read-only in non-owner data mode.
            /// </summary>
            public uint CItems;

            /// <summary>
            /// Windows Vista. NULL if group is not a subset. Pointer to a null-terminated string that contains the subset title text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subset title text.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPWStr)]
            public string PszSubsetTitle;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszSubsetTitle member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public uint CchSubsetTitle;
        }
 
        /// <summary>
        /// WM_NOTIFY notificaiton message header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            /// <summary>
            /// Window handle to the control sending a message.
            /// </summary>
            public IntPtr hwndFrom;
            /// <summary>
            /// Identifier of the control sending a message.
            /// </summary>
            public IntPtr idFrom;
            /// <summary>
            /// Notification code. This member can be a control-specific notification code or it can be one of the common notification codes.
            /// </summary>
            public int code;
        }

        /// <summary>
        ///  Used to set and retrieve information about a link item.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LITEM
        {
            /// <summary>
            /// Combination of one or more of the LIF flags
            /// </summary>
            public int Mask;
            /// <summary>
            /// Value of type int that contains the item index.
            /// This numeric index is used to access a SysLink control link.
            /// </summary>
            public int LinkIndex;
            /// <summary>
            /// Combination of one or more of the LIS flags.
            /// </summary>
            public int State;
            /// <summary>
            /// Use stateMask to get or set the state of the link.
            /// </summary>
            public int StateMask;
            /// <summary>
            /// Specify the item by the ID value.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string Id;
            /// <summary>
            /// Set or get the URL for this item.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048 + 32 + 4)]
            public string Url;
        }

        /// <summary>
        /// Contains information about an LVN_LINKCLICK  notification. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct NMLVLINK
        {
            /// <summary>
            /// NMHDR structure that contains basic
            /// information about the notification message.
            /// </summary>
            public NMHDR Header;
            /// <summary>
            /// LITEM structure that contains 
            /// information about the link that was clicked.
            /// </summary>
            public LITEM Link;
            /// <summary>
            /// Index of the item that contains the link.
            /// </summary>
            public int ItemIndex;
            /// <summary>
            /// Subitem if any. This member may be NULL.
            /// For a link in a group header, this is the
            /// group identifier, as set in LVGROUP.
            /// </summary>
            public int SubItemIndex;
        }

        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVGroup lParam);
        
        #endregion
    }

    [Flags]
    public enum ListViewGroupMask : uint
    {
        None = 0x0,
        Header = 0x1,
        Footer = 0x2,
        State = 0x4,
        Align = 0x8,
        GroupId = 0x10,
        SubTitle = 0x100,
        Task = 0x200,
        DescriptionTop = 0x400,
        DescriptionBottom = 0x800,
        TitleImage = 0x1000,
        ExtendedImage = 0x2000,
        Items = 0x4000,
        Subset = 0x8000,
        SubsetItems = 0x10000
    }

    [Flags]
    public enum ListViewGroupState : uint
    {
        /// <summary>
        /// Groups are expanded, the group name is displayed, and all items in the group are displayed.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The group is collapsed.
        /// </summary>
        Collapsed = 1,
        /// <summary>
        /// The group is hidden.
        /// </summary>
        Hidden = 2,
        /// <summary>
        /// Version 6.00 and Windows Vista. The group does not display a header.
        /// </summary>
        NoHeader = 4,
        /// <summary>
        /// Version 6.00 and Windows Vista. The group can be collapsed.
        /// </summary>
        Collapsible = 8,
        /// <summary>
        /// Version 6.00 and Windows Vista. The group has keyboard focus.
        /// </summary>
        Focused = 16,
        /// <summary>
        /// Version 6.00 and Windows Vista. The group is selected.
        /// </summary>
        Selected = 32,
        /// <summary>
        /// Version 6.00 and Windows Vista. The group displays only a portion of its items.
        /// </summary>
        SubSeted = 64,
        /// <summary>
        /// Version 6.00 and Windows Vista. The subset link of the group has keyboard focus.
        /// </summary>
        SubSetLinkFocused = 128,
    }
}
