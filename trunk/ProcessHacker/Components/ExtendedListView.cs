/*
 * Process Hacker - 
 *   ProcessHacker Extended ListView 
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
 * 
 */


using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using ProcessHacker.Native.Api;
using ProcessHacker.Native;

namespace ProcessHacker
{
    public class ExtendedListView : ListView
    {
        private const int LVM_First = 0x1000;                              // ListView messages
        private const int LVM_SetGroupInfo = (LVM_First + 147);            // ListView messages Setinfo on Group
        private const int LVM_SetExtendedListViewStyle = (LVM_First + 54); // Sets extended styles in list-view controls. 
        private const int LVS_Ex_DoubleBuffer = 0x00010000;                // Paints via double-buffering, which reduces flicker. also enables alpha-blended marquee selection.

        private delegate void CallBackSetGroupState(ListViewGroup lvGroup, ListViewGroupState lvState);
        private delegate void CallbackSetGroupString(ListViewGroup lvGroup, string value);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVGroup lParam);

        public ExtendedListView()
        {
            //Activate double buffering and
            //Enable the OnNotifyMessage event so we get a chance to filter out 
            //Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);
        }

        public void SetGroupState(ListViewGroupState state)
        {
            foreach (ListViewGroup lvg in this.Groups)
            {
                SetGrpState(lvg, state);
            }
        }

        public void SetGroupFooter(ListViewGroup lvg, string footerText)
        {
            SetGrpFooter(lvg, footerText);
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

        private static void SetGrpState(ListViewGroup lvGroup, ListViewGroupState grpState)
        {
            if (OSVersion.IsBelow(WindowsVersion.Vista))
                return;
            if (lvGroup == null || lvGroup.ListView == null)
                return;
            if (lvGroup.ListView.InvokeRequired)
                lvGroup.ListView.Invoke(new CallBackSetGroupState(SetGrpState), lvGroup, grpState);
            else
            {
                int? GrpId = GetGroupID(lvGroup);
                int gIndex = lvGroup.ListView.Groups.IndexOf(lvGroup);
                LVGroup group = new LVGroup();
                group.CbSize = Marshal.SizeOf(group);
                group.GroupState = grpState;
                group.Mask = ListViewGroupMask.State;
                if (GrpId != null)
                {
                    group.GroupId = GrpId.Value;
                    SendMessage(lvGroup.ListView.Handle, LVM_SetGroupInfo, GrpId.Value, ref group);
                    SendMessage(lvGroup.ListView.Handle, LVM_SetGroupInfo, GrpId.Value, ref group);
                }
                else
                {
                    group.GroupId = gIndex;
                    SendMessage(lvGroup.ListView.Handle, LVM_SetGroupInfo, gIndex, ref group);
                    SendMessage(lvGroup.ListView.Handle, LVM_SetGroupInfo, gIndex, ref group);
                }
                lvGroup.ListView.Refresh();
            }
        }

        private static void SetGrpFooter(ListViewGroup lvGroup, string footer)
        {
            if (OSVersion.IsBelow(WindowsVersion.Vista))
                return;
            if (lvGroup == null || lvGroup.ListView == null)
                return;
            if (lvGroup.ListView.InvokeRequired)
                lvGroup.ListView.Invoke(new CallbackSetGroupString(SetGrpFooter), lvGroup, footer);
            else
            {
                int? grpId = GetGroupID(lvGroup);
                int gIndex = lvGroup.ListView.Groups.IndexOf(lvGroup);
              
                LVGroup group = new LVGroup();
                group.CbSize = Marshal.SizeOf(group);
                group.Footer = footer;
                group.Mask = ListViewGroupMask.Footer;
               
                if (grpId != null)
                {
                    group.GroupId = grpId.Value;
                    SendMessage(lvGroup.ListView.Handle, LVM_SetGroupInfo, grpId.Value, ref group);
                }
                else
                {
                    group.GroupId = gIndex;
                    SendMessage(lvGroup.ListView.Handle, LVM_SetGroupInfo, gIndex, ref group);
                }
            }
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message and prevent any type of flickering
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x1: /*WM_CREATE*/
                    {
                        Win32.SetWindowTheme(this.Handle, "explorer", null);
                        Win32.SendMessage(this.Handle, (WindowMessage)LVM_SetExtendedListViewStyle, LVS_Ex_DoubleBuffer, LVS_Ex_DoubleBuffer);
                        break;
                    }                  
                case 0x0202: /*WM_LBUTTONUP*/
                    {
                        base.DefWndProc(ref m);
                        break;
                    }  
                default:
                    { 
                        break;
                    }  
            }
            
            base.WndProc(ref m);
        }

        //http://msdn.microsoft.com/en-us/library/bb774769(VS.85).aspx
        [StructLayout(LayoutKind.Sequential)]
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
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Header;

            /// <summary>
            /// Size in TCHARs of the buffer pointed to by the pszHeader member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchHeader;

            /// <summary>
            /// Pointer to a null-terminated string that contains the footer text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the footer text.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Footer;

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
            public int StateMask;

            /// <summary>
            /// Flag that can have one of the following values:LVGS_NORMALGroups are expanded, the group name is displayed, and all items in the group are displayed.
            /// </summary>
            public ListViewGroupState GroupState;

            /// <summary>
            /// Indicates the alignment of the header or footer text for the group. It can have one or more of the following values. Use one of the header flags. Footer flags are optional. Windows XP: Footer flags are reserved.LVGA_FOOTER_CENTERReserved.
            /// </summary>
            public int Alignment;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the subtitle text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subtitle text. This element is drawn under the header text.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string PszSubtitle;

            /// <summary>
            /// Windows Vista. Size, in TCHARs, of the buffer pointed to by the pszSubtitle member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchSubtitle;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the text for a task link when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the task text. This item is drawn right-aligned opposite the header text. When clicked by the user, the task link generates an LVN_LINKCLICK notification.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Task;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszTask member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchTask;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the top description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the top description text. This item is drawn opposite the title image when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string DescriptionTop;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionTop member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchDescriptionTop;

            /// <summary>
            /// Windows Vista. Pointer to a null-terminated string that contains the bottom description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the bottom description text. This item is drawn under the top description text when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string DescriptionBottom;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionBottom member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchDescriptionBottom;

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
            public int CItems;

            /// <summary>
            /// Windows Vista. NULL if group is not a subset. Pointer to a null-terminated string that contains the subset title text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subset title text.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string PszSubsetTitle;

            /// <summary>
            /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszSubsetTitle member. If the structure is not receiving information about a group, this member is ignored.
            /// </summary>
            public int CchSubsetTitle;
        }
    }

    [Flags]
    public enum ListViewGroupMask : int
    {
        None = 0x00000,
        Header = 0x00001,
        Footer = 0x00002,
        State = 0x00004,
        Align = 0x00008,
        GroupId = 0x00010,
        SubTitle = 0x00100,
        Task = 0x00200,
        DescriptionTop = 0x00400,
        DescriptionBottom = 0x00800,
        TitleImage = 0x01000,
        ExtendedImage = 0x02000,
        Items = 0x04000,
        Subset = 0x08000,
        SubsetItems = 0x10000
    }

    [Flags]
    public enum ListViewGroupState : int
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