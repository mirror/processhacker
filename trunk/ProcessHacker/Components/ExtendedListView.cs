using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using ProcessHacker.Native.Api;
using ProcessHacker.Native;

namespace ProcessHacker
{
    class ExtendedListView : ListView
    {
        private const int LVM_FIRST = 0x1000;                    // ListView messages
        private const int LVM_SETGROUPINFO = (LVM_FIRST + 147);  // ListView messages Setinfo on Group
        
        private delegate void CallBackSetGroupState(ListViewGroup lstvwgrp, ListViewGroupState state);
        private delegate void CallbackSetGroupString(ListViewGroup lstvwgrp, string value);

        public ExtendedListView()
        {
            //Activate double buffering and
            //Enable the OnNotifyMessage event so we get a chance to filter out 
            //Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.EnableNotifyMessage, true);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, LVGROUP lParam);

        //Listview messages
        public const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        public const int LVS_EX_FULLROWSELECT = 0x00000020;
        //Listview extended styles
        public const int LVS_EX_DOUBLEBUFFER = 0x00010000;

        private Boolean isThemeSet = false;

        private static int? GetGroupID(ListViewGroup lstvwgrp)
        {
            int? rtnval = null;
            Type GrpTp = lstvwgrp.GetType();
            if (GrpTp != null)
            {
                PropertyInfo pi = GrpTp.GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pi != null)
                {
                    object tmprtnval = pi.GetValue(lstvwgrp, null);
                    if (tmprtnval != null)
                    {
                        rtnval = tmprtnval as int?;
                    }
                }
            }
            return rtnval;
        }

        private static void setGrpState(ListViewGroup lstvwgrp, ListViewGroupState state)
        {
            if (OSVersion.IsBelow(WindowsVersion.Vista))
                return;
            if (lstvwgrp == null || lstvwgrp.ListView == null)
                return;
            if (lstvwgrp.ListView.InvokeRequired)
                lstvwgrp.ListView.Invoke(new CallBackSetGroupState(setGrpState), lstvwgrp, state);
            else
            {
                int? GrpId = GetGroupID(lstvwgrp);
                int gIndex = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
                LVGROUP group = new LVGROUP();
                group.CbSize = Marshal.SizeOf(group);
                group.State = state;
                group.Mask = ListViewGroupMask.State;
                if (GrpId != null)
                {
                    group.IGroupId = GrpId.Value;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, GrpId.Value, group);
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, GrpId.Value, group);
                }
                else
                {
                    group.IGroupId = gIndex;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                }
                lstvwgrp.ListView.Refresh();
            }
        }

        private static void setGrpFooter(ListViewGroup lstvwgrp, string footer)
        {
            if (OSVersion.IsBelow(WindowsVersion.Vista))
                return;
            if (lstvwgrp == null || lstvwgrp.ListView == null)
                return;
            if (lstvwgrp.ListView.InvokeRequired)
                lstvwgrp.ListView.Invoke(new CallbackSetGroupString(setGrpFooter), lstvwgrp, footer);
            else
            {
                int? GrpId = GetGroupID(lstvwgrp);
                int gIndex = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
                LVGROUP group = new LVGROUP();
                group.CbSize = Marshal.SizeOf(group);
                group.PszFooter = footer;
                group.Mask = ListViewGroupMask.Footer;
                if (GrpId != null)
                {
                    group.IGroupId = GrpId.Value;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, GrpId.Value, group);
                }
                else
                {
                    group.IGroupId = gIndex;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                }
            }
        }

        public void SetGroupState(ListViewGroupState state)
        {
            foreach (ListViewGroup lvg in this.Groups)
            {
                setGrpState(lvg, state);
            }
        }

        public void SetGroupFooter(ListViewGroup lvg, string footerText)
        {
            setGrpFooter(lvg, footerText);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                    case 15: /*Paint Event*/
                        if (!isThemeSet) //run once
                        {
                            Win32.SetWindowTheme(this.Handle, "explorer", null); //Vista Explorer style
                            Win32.SendMessage(this.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, LVS_EX_DOUBLEBUFFER, LVS_EX_DOUBLEBUFFER);
                            isThemeSet = true;
                        }
                        break;
                    case 0x0202: /*WM_LBUTTONUP - left button event*/
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

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message and prevent any type of flickering
            if (m.Msg != 0x14)
            {   
                base.OnNotifyMessage(m);
            }
        }
    }

    /// <summary>
    /// LVGROUP Structure used to set and retrieve groups.
    /// </summary>
    /// <example>
    /// LVGROUP myLVGROUP = new LVGROUP();
    /// myLVGROUP.CbSize	// is of managed type uint
    /// myLVGROUP.Mask	// is of managed type uint
    /// myLVGROUP.PszHeader	// is of managed type string
    /// myLVGROUP.CchHeader	// is of managed type int
    /// myLVGROUP.PszFooter	// is of managed type string
    /// myLVGROUP.CchFooter	// is of managed type int
    /// myLVGROUP.IGroupId	// is of managed type int
    /// myLVGROUP.StateMask	// is of managed type uint
    /// myLVGROUP.State	// is of managed type uint
    /// myLVGROUP.UAlign	// is of managed type uint
    /// myLVGROUP.PszSubtitle	// is of managed type IntPtr
    /// myLVGROUP.CchSubtitle	// is of managed type uint
    /// myLVGROUP.PszTask	// is of managed type string
    /// myLVGROUP.CchTask	// is of managed type uint
    /// myLVGROUP.PszDescriptionTop	// is of managed type string
    /// myLVGROUP.CchDescriptionTop	// is of managed type uint
    /// myLVGROUP.PszDescriptionBottom	// is of managed type string
    /// myLVGROUP.CchDescriptionBottom	// is of managed type uint
    /// myLVGROUP.ITitleImage	// is of managed type int
    /// myLVGROUP.IExtendedImage	// is of managed type int
    /// myLVGROUP.IFirstItem	// is of managed type int
    /// myLVGROUP.CItems	// is of managed type IntPtr
    /// myLVGROUP.PszSubsetTitle	// is of managed type IntPtr
    /// myLVGROUP.CchSubsetTitle	// is of managed type IntPtr
    /// </example>
    /// <remarks>
    /// Reference: http://msdn.microsoft.com/en-us/library/bb774769(VS.85).aspx
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LVGROUP
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
        public string PszHeader;

        /// <summary>
        /// Size in TCHARs of the buffer pointed to by the pszHeader member. If the structure is not receiving information about a group, this member is ignored.
        /// </summary>
        public int CchHeader;

        /// <summary>
        /// Pointer to a null-terminated string that contains the footer text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the footer text.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string PszFooter;

        /// <summary>
        /// Size in TCHARs of the buffer pointed to by the pszFooter member. If the structure is not receiving information about a group, this member is ignored.
        /// </summary>
        public int CchFooter;

        /// <summary>
        /// ID of the group.
        /// </summary>
        public int IGroupId;

        /// <summary>
        /// Mask used with LVM_GETGROUPINFO (Microsoft Windows XP and Windows Vista) and LVM_SETGROUPINFO (Windows Vista only) to specify which flags in the state value are being retrieved or set.
        /// </summary>
        public int StateMask;

        /// <summary>
        /// Flag that can have one of the following values:LVGS_NORMALGroups are expanded, the group name is displayed, and all items in the group are displayed.
        /// </summary>
        public ListViewGroupState State;

        /// <summary>
        /// Indicates the alignment of the header or footer text for the group. It can have one or more of the following values. Use one of the header flags. Footer flags are optional. Windows XP: Footer flags are reserved.LVGA_FOOTER_CENTERReserved.
        /// </summary>
        public uint UAlign;

        /// <summary>
        /// Windows Vista. Pointer to a null-terminated string that contains the subtitle text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subtitle text. This element is drawn under the header text.
        /// </summary>
        public IntPtr PszSubtitle;

        /// <summary>
        /// Windows Vista. Size, in TCHARs, of the buffer pointed to by the pszSubtitle member. If the structure is not receiving information about a group, this member is ignored.
        /// </summary>
        public uint CchSubtitle;

        /// <summary>
        /// Windows Vista. Pointer to a null-terminated string that contains the text for a task link when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the task text. This item is drawn right-aligned opposite the header text. When clicked by the user, the task link generates an LVN_LINKCLICK notification.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string PszTask;

        /// <summary>
        /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszTask member. If the structure is not receiving information about a group, this member is ignored.
        /// </summary>
        public uint CchTask;

        /// <summary>
        /// Windows Vista. Pointer to a null-terminated string that contains the top description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the top description text. This item is drawn opposite the title image when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string PszDescriptionTop;

        /// <summary>
        /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszDescriptionTop member. If the structure is not receiving information about a group, this member is ignored.
        /// </summary>
        public uint CchDescriptionTop;

        /// <summary>
        /// Windows Vista. Pointer to a null-terminated string that contains the bottom description text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the bottom description text. This item is drawn under the top description text when there is a title image, no extended image, and uAlign==LVGA_HEADER_CENTER.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string PszDescriptionBottom;

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
        public IntPtr CItems;

        /// <summary>
        /// Windows Vista. NULL if group is not a subset. Pointer to a null-terminated string that contains the subset title text when item information is being set. If group information is being retrieved, this member specifies the address of the buffer that receives the subset title text.
        /// </summary>
        public IntPtr PszSubsetTitle;

        /// <summary>
        /// Windows Vista. Size in TCHARs of the buffer pointed to by the pszSubsetTitle member. If the structure is not receiving information about a group, this member is ignored.
        /// </summary>
        public IntPtr CchSubsetTitle;
    }

    public enum ListViewGroupMask
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

    public enum ListViewGroupState
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