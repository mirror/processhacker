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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace ProcessHacker
{
    /// <summary>
    /// Provides automatic sorting support for the ListView control.
    /// </summary>
    /// <example>
    /// myListView.ListViewItemSorter = new SortedListComparer(myListView);
    /// </example>
    public class SortedListComparer : IComparer
    {
        private ListView _list;
        private bool _triState;
        private IComparer _triStateComparer;
        private int _sortColumn;
        private SortOrder _sortOrder;
        private Dictionary<int, Comparison<ListViewItem>> _customSorters = new Dictionary<int, Comparison<ListViewItem>>();

        /// <summary>
        /// Creates a new sorted list manager.
        /// </summary>
        /// <param name="list">The ListView to manage.</param>
        public SortedListComparer(ListView list)
        {
            _list = list;
            _list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);
            _sortColumn = 0;
            _sortOrder = SortOrder.Ascending;
        }

        /// <summary>
        /// Allows three states of sorting: Ascending, Descending and None. 
        /// You must specify the sorter used for the None state using 
        /// TriStateComparer.
        /// </summary>
        public bool TriState
        {
            get { return _triState; }
            set { _triState = value; }
        }

        /// <summary>
        /// Specifies the sorter used for the None sorting state.
        /// </summary>
        public IComparer TriStateComparer
        {
            get { return _triStateComparer; }
            set { _triStateComparer = value; }
        }

        /// <summary>
        /// Specifies the index of the column to sort.
        /// </summary>
        public int SortColumn
        {
            get { return _sortColumn; }
            set { _sortColumn = value; }
        }

        /// <summary>
        /// Specifies the sort order/state.
        /// </summary>
        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        /// <summary>
        /// Allows custom sorting for individual columns.
        /// </summary>
        public IDictionary<int, Comparison<ListViewItem>> CustomSorters
        {
            get { return _customSorters; }
        }

        private void list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumn)
            {
                if (_triState)
                {
                    if (_sortOrder == SortOrder.Ascending)
                        _sortOrder = SortOrder.Descending;
                    else if (_sortOrder == SortOrder.Descending)
                        _sortOrder = SortOrder.None;
                    else
                        _sortOrder = SortOrder.Ascending;
                }
                else
                {
                    _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
                }
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

        /// <summary>
        /// Compares two ListView objects.
        /// </summary>
        /// <param name="x">The first ListView.</param>
        /// <param name="y">The second ListView.</param>
        /// <returns>A comparison result.</returns>
        public int Compare(object x, object y)
        {
            if (_triState && _sortOrder == SortOrder.None)
                return _triStateComparer.Compare(x, y);

            ListViewItem lx = x as ListViewItem;
            ListViewItem ly = y as ListViewItem;

            if (_customSorters.ContainsKey(_sortColumn))
                return ModifySort(_customSorters[_sortColumn](lx, ly), _sortOrder);

            string sx, sy;
            int ix, iy;
            IComparable cx, cy;

            sx = lx.SubItems[_sortColumn].Text;
            sy = ly.SubItems[_sortColumn].Text;

            if (!int.TryParse(sx.StartsWith("0x") ? sx.Substring(2) : sx, 
                sx.StartsWith("0x") ? System.Globalization.NumberStyles.AllowHexSpecifier : 0, 
                null, out ix) ||
                !int.TryParse(sy.StartsWith("0x") ? sy.Substring(2) : sy, 
                sy.StartsWith("0x") ? System.Globalization.NumberStyles.AllowHexSpecifier : 0, 
                null, out iy))
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
