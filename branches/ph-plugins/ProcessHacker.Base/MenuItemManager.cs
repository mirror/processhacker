using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Base
{
    public class MenuItemManager
    {
        private MenuItem.MenuItemCollection _menuItems;

        public MenuItemManager(MenuItem.MenuItemCollection menuItems)
        {
            _menuItems = menuItems;
        }

        private void Insert(string text, MenuItem item, int offset)
        {
            for (int i = 0; i < _menuItems.Count; i++)
            {
                if (_menuItems[i].Text.Replace("&", "").Equals(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    _menuItems.Add(i + offset, item);
                    return;
                }
            }

            throw new InvalidOperationException("No menu item exists with the specified text.");
        }

        public void InsertAfter(string text, MenuItem item)
        {
            this.Insert(text, item, 1);
        }

        public void InsertBefore(string text, MenuItem item)
        {
            this.Insert(text, item, 0);
        }

        public void Remove(string text)
        {
            for (int i = 0; i < _menuItems.Count; i++)
            {
                if (_menuItems[i].Text.Replace("&", "").Equals(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    _menuItems.RemoveAt(i);
                    return;
                }
            }

            throw new InvalidOperationException("No menu item exists with the specified text.");
        }
    }

    public sealed class MainMenuManager : MenuItemManager
    {
        private MenuItemManager _hacker;
        private MenuItemManager _view;
        private MenuItemManager _tools;
        private MenuItemManager _help;

        public MainMenuManager(MenuItem.MenuItemCollection menuItems)
            : base(menuItems)
        { }

        public MenuItemManager Hacker
        {
            get { return _hacker; }
            set { _hacker = value; }
        }

        public MenuItemManager View
        {
            get { return _view; }
            set { _view = value; }
        }

        public MenuItemManager Tools
        {
            get { return _tools; }
            set { _tools = value; }
        }

        public MenuItemManager Help
        {
            get { return _help; }
            set { _help = value; }
        }
    }
}
