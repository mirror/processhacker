/*
 * Process Hacker - 
 *   sorted list comparer
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

namespace ProcessHacker
{
    public class SortedListComparer : System.Collections.IComparer
    {
        private ListView _list;
        private int _sortColumn;
        private SortOrder _sortOrder;

        public SortedListComparer(ListView list)
        {
            _list = list;
            _list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);
            _sortColumn = 0;
            _sortOrder = SortOrder.Ascending;
        }

        private void list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumn)
            {
                _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _sortColumn = e.Column;
                _sortOrder = SortOrder.Ascending;
            }

            _list.Sort();
        }

        private int ModifySort(int result, SortOrder order)
        {
            if (order == SortOrder.Ascending)
                return result;
            else if (order == SortOrder.Descending)
                return -result;
            else
                return 0;
        }

        public int Compare(object x, object y)
        {
            ListViewItem lx = x as ListViewItem;
            ListViewItem ly = y as ListViewItem;
            int ix, iy;
            IComparable cx, cy;

            if (!int.TryParse(lx.SubItems[_sortColumn].Text, out ix) ||
                !int.TryParse(ly.SubItems[_sortColumn].Text, out iy))
            {
                cx = lx.SubItems[_sortColumn].Text;
                cy = ly.SubItems[_sortColumn].Text;
            }
            else
            {
                cx = ix;
                cy = iy;
            }

            return ModifySort(cx.CompareTo(cy), _sortOrder);
        }
    }
}
