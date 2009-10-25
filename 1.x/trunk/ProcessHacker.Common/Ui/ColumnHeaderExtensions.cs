using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessHacker.Common.Ui
{
    // From http://stackoverflow.com/questions/254129/how-to-i-display-a-sort-arrow-in-the-header-of-a-list-view-column-using-c
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ColumnHeaderExtensions
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LVCOLUMN
        {
            public Int32 mask;
            public Int32 cx;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 iSubItem;
            public Int32 iImage;
            public Int32 iOrder;
        }

        private const Int32 HDI_FORMAT = 0x4;
        private const Int32 HDF_SORTUP = 0x400;
        private const Int32 HDF_SORTDOWN = 0x200;
        private const Int32 LVM_GETHEADER = 0x101f;
        private const Int32 HDM_GETITEM = 0x120b;
        private const Int32 HDM_SETITEM = 0x120c;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, ref LVCOLUMN lPLVCOLUMN);

        public static void SetSortIcon(this ColumnHeader column, SortOrder order)
        {
            ListView listView = column.ListView;
            IntPtr columnHeader = SendMessage(listView.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            for (int i = 0; i <= listView.Columns.Count - 1; i++)
            {
                IntPtr ColumnPtr = new IntPtr(i);
                LVCOLUMN lvColumn = new LVCOLUMN();
                lvColumn.mask = HDI_FORMAT;
                SendMessage(columnHeader, HDM_GETITEM, ColumnPtr, ref lvColumn);

                if (!(order == SortOrder.None) && i == column.Index)
                {
                    switch (order)
                    {
                        case SortOrder.Ascending:
                            lvColumn.fmt &= ~HDF_SORTDOWN;
                            lvColumn.fmt |= HDF_SORTUP;
                            break;
                        case SortOrder.Descending:
                            lvColumn.fmt &= ~HDF_SORTUP;
                            lvColumn.fmt |= HDF_SORTDOWN;
                            break;
                    }
                }
                else
                {
                    lvColumn.fmt &= ~HDF_SORTDOWN & ~HDF_SORTUP;
                }

                SendMessage(columnHeader, HDM_SETITEM, ColumnPtr, ref lvColumn);
            }
        }
    }
}
