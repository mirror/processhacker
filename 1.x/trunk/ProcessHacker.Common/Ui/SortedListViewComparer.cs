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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;

namespace ProcessHacker.Common.Ui
{
    public interface ISortedListViewComparer
    {
        int Compare(ListViewItem x, ListViewItem y, int column);
    }

    /// <summary>
    /// Provides automatic sorting support for the ListView control.
    /// </summary>
    /// <example>
    /// myListView.ListViewItemSorter = new SortedListComparer(myListView);
    /// </example>
    public class SortedListViewComparer : IComparer
    {
        private class DefaultComparer : ISortedListViewComparer
        {
            private SortedListViewComparer _sortedListComparer;

            public DefaultComparer(SortedListViewComparer sortedListComparer)
            {
                _sortedListComparer = sortedListComparer;
            }

            public int Compare(ListViewItem x, ListViewItem y, int column)
            {
                string sx, sy;
                long ix, iy;
                IComparable cx, cy;

                sx = x.SubItems[column].Text.Replace(",", "");
                sy = y.SubItems[column].Text.Replace(",", "");

                if (!long.TryParse(sx.StartsWith("0x") ? sx.Substring(2) : sx,
                    sx.StartsWith("0x") ? NumberStyles.AllowHexSpecifier : 0,
                    null, out ix) ||
                    !long.TryParse(sy.StartsWith("0x") ? sy.Substring(2) : sy,
                    sy.StartsWith("0x") ? NumberStyles.AllowHexSpecifier : 0,
                    null, out iy))
                {
                    cx = x.SubItems[column].Text;
                    cy = y.SubItems[column].Text;
                }
                else
                {
                    cx = ix;
                    cy = iy;
                }

                return cx.CompareTo(cy);
            }
        }

        private ListView _list;
        private bool _virtualMode = false;
        private RetrieveVirtualItemEventHandler _retrieveVirtualItem;
        private bool _triState = false;
        private ISortedListViewComparer _comparer;
        private ISortedListViewComparer _triStateComparer;
        private int _sortColumn;
        private SortOrder _sortOrder;
        private Dictionary<int, Comparison<ListViewItem>> _customSorters =
            new Dictionary<int, Comparison<ListViewItem>>();
        private List<int> _columnSortOrder = new List<int>();

        /// <summary>
        /// Creates a new sorted list manager.
        /// </summary>
        /// <param name="list">The ListView to manage.</param>
        public SortedListViewComparer(ListView list)
        {
            _list = list;
            _list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);
            _sortColumn = 0;
            _sortOrder = SortOrder.Ascending;
            _comparer = new DefaultComparer(this);
            this.SetSortIcon();
        }

        /// <summary>
        /// Specifies whether the ListView is using VirtualMode. If true,
        /// the SortedListComparer will not automatically sort the ListView.
        /// </summary>
        public bool VirtualMode
        {
            get { return _virtualMode; }
            set { _virtualMode = value; }
        }

        public RetrieveVirtualItemEventHandler RetrieveVirtualItem
        {
            get { return _retrieveVirtualItem; }
            set { _retrieveVirtualItem = value; }
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
        /// The comparer to use when sorting. This is optional because a
        /// default comparer will be provided.
        /// </summary>
        public ISortedListViewComparer Comparer
        {
            get { return _comparer; }
            set
            {
                if (value == null)
                    _comparer = new DefaultComparer(this);
                else
                    _comparer = value;
            }
        }

        /// <summary>
        /// Specifies the sorter used for the None sorting state.
        /// </summary>
        public ISortedListViewComparer TriStateComparer
        {
            get { return _triStateComparer; }
            set { _triStateComparer = value; }
        }

        public ListView ListView
        {
            get { return _list; }
        }

        /// <summary>
        /// Specifies the index of the column to sort.
        /// </summary>
        public int SortColumn
        {
            get { return _sortColumn; }
            set
            {
                _sortColumn = value;
                this.SetSortIcon();
            }
        }

        /// <summary>
        /// Specifies the sort order/state.
        /// </summary>
        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set
            {
                _sortOrder = value;
                this.SetSortIcon();
            }
        }

        /// <summary>
        /// Allows custom sorting for individual columns.
        /// </summary>
        public IDictionary<int, Comparison<ListViewItem>> CustomSorters
        {
            get { return _customSorters; }
        }

        public IList<int> ColumnSortOrder
        {
            get { return _columnSortOrder; }
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

            this.SetSortIcon();

            if (!_virtualMode)
                _list.Sort();
        }

        private void SetSortIcon()
        {
            // Avoid forcing handle creation before all other initialization 
            // has finished. This is done by handling the Layout event and 
            // performing the icon setting there.
            _list.DoDelayed((control) => _list.Columns[_sortColumn].SetSortIcon(_sortOrder));
        }

        private ListViewItem GetItem(int index)
        {
            if (_virtualMode)
            {
                var args = new RetrieveVirtualItemEventArgs(index);
                _retrieveVirtualItem(this, args);
                return args.Item;
            }
            else
            {
                return _list.Items[index];
            }
        }

        private int ModifySort(int result, SortOrder order)
        {
            if (order == SortOrder.Ascending)
                return result;
            else if (order == SortOrder.Descending)
                return -result;
            else
                return result;
        }

        private int Compare(ListViewItem x, ListViewItem y, int column)
        {
            int result = 0;

            if (_triState && _sortOrder == SortOrder.None)
                result = _triStateComparer.Compare(x, y, column);

            if (result != 0)
                return result;

            if (_customSorters.ContainsKey(column))
                result = ModifySort(_customSorters[column](x, y), _sortOrder);

            if (result != 0)
                return result;

            return ModifySort(_comparer.Compare(x, y, column), _sortOrder);  
        }

        public int Compare(ListViewItem x, ListViewItem y)
        {
            int result = this.Compare(x, y, _sortColumn);

            if (result != 0)
                return result;

            foreach (int column in _columnSortOrder)
            {
                if (column == _sortColumn)
                    continue;

                result = this.Compare(x, y, column);

                if (result != 0)
                    return result;
            }

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
            return this.Compare(x as ListViewItem, y as ListViewItem);
        }
    }
}
