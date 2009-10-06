/*
 * Process Hacker - 
 *   ProcessHacker Taskbar Extensions
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using TaskbarLib.Interop;
using System.Runtime.CompilerServices;
using ProcessHacker.Native.Api;

namespace TaskbarLib
{
    /// <summary>
    /// Provides services to manage taskbar jump lists, including
    /// custom destinations and custom tasks.
    /// </summary>
    /// <remarks>
    /// This class mostly borrows the Windows Shell's concepts where
    /// jump lists are concerned including:
    /// Application destinations - Destinations added to the application's
    /// recent and frequent categories by the shell or by the application.
    /// Custom destinations - Destinations added to the application's
    /// jump list in other categories by the application.
    /// Tasks - Tasks added to the application's jump list.
    /// <b>The methods of this class are not thread-safe.</b>
    /// </remarks>
    public sealed class JumpListManager : IDisposable
    {
        #region Members

        string _appId;
        uint _maxSlotsInList;

        JumpListTasks _tasks;
        JumpListDestinations _destinations;
        EventHandler _displaySettingsChangeHandler;
        ICustomDestinationList _customDestinationList;
        ApplicationDestinationType _enabledAutoDestinationType; // = ApplicationDestinationType.Recent;

        #endregion

        /// <summary>
        /// Initializes a new instance of the jump list manager
        /// with the specified application id.
        /// </summary>
        /// <param name="appId">The application id.</param>
        public JumpListManager(string appId)
        {
            _appId = appId;
            _destinations = new JumpListDestinations();
            _tasks = new JumpListTasks();

            _customDestinationList = (ICustomDestinationList)new CDestinationList();
           
            if (String.IsNullOrEmpty(_appId))
            {
                _appId = Windows7Taskbar.ProcessAppId;
            }
            if (!String.IsNullOrEmpty(_appId))
            {
                _customDestinationList.SetAppID(_appId);
            }

            _displaySettingsChangeHandler = delegate
            {
                RefreshMaxSlots();
            };

            SystemEvents.DisplaySettingsChanged += _displaySettingsChangeHandler;
        }

        /// <summary>
        /// Initializes a new instance of the jump list manager
        /// with the specified window handle.
        /// </summary>
        public JumpListManager()
            : this(Windows7Taskbar.AppId)
        {
        }

        /// <summary>
        /// Adds a task to the application's jump list.
        /// </summary>
        /// <param name="task">An object implementing <see cref="IJumpListTask"/>,
        /// such as <see cref="ShellLink"/>.</param>
        public void AddUserTask(IJumpListTask task)
        {
            _tasks.AddTask(task);
        }

        /// <summary>
        /// Retrieves the tasks currently present in the application's
        /// jump list.  If the tasks are modified through the use of this
        /// property, the <see cref="Refresh"/> method must be called to
        /// repopulate the application's jump list.
        /// </summary>
        public IEnumerable<IJumpListTask> Tasks
        {
            get { return _tasks.Tasks; }
        }

        /// <summary>
        /// Deletes the specified task from the application's jump list.
        /// </summary>
        /// <param name="task">The task to delete.</param>
        public void DeleteTask(IJumpListTask task)
        {
            _tasks.DeleteTask(task);
        }

        /// <summary>
        /// Deletes all the tasks from the application's jump list.
        /// </summary>
        public void ClearTasks()
        {
            _tasks.Clear();
        }

        /// <summary>
        /// Adds a custom destination to the application's jump list.
        /// </summary>
        /// <param name="destination">An object implementing
        /// <see cref="IJumpListDestination"/> such as <see cref="ShellLink"/>
        /// or <see cref="ShellItem"/>.</param>
        public void AddCustomDestination(IJumpListDestination destination)
        {
            // Do not use CustomDestinations as they will cause an
            // System.UnauthorizedAccessException: Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))
            // error when the user has recent document tracking disabled Via the Group Policy setting:
            //“Do not keep history of recently opened documents”. or via the Users setting: 
            //“Store and display recently opened items in the Start menu and the taskbar” in the Start menu property dialog.
           
            //_destinations.AddDestination(destination);
        }

        /// <summary>
        /// Deletes the specified custom destination from the application's
        /// jump list.
        /// </summary>
        /// <param name="destination">The destination to delete.</param>
        public void DeleteCustomDestination(IJumpListDestination destination)
        {
            _destinations.DeleteDestination(destination);
        }

        /// <summary>
        /// The currently enabled automatic application destination type.
        /// The supported values are the values of the
        /// <see cref="ApplicationDestinationType"/> enumeration.
        /// Only of the values can be set at any given time.
        /// </summary>
        public ApplicationDestinationType EnabledAutoDestinationType
        {
            get
            {
                return _enabledAutoDestinationType;
            }
            set
            {
                if (_enabledAutoDestinationType == value)
                    return;

                _enabledAutoDestinationType = value;
            }
        }

        /// <summary>
        /// Removes all destinations from the application's jump list.
        /// </summary>
        public void ClearAllDestinations()
        {
            ClearApplicationDestinations();
            ClearCustomDestinations();
        }

        /// <summary>
        /// Removes all application destinations (such as frequent and recent)
        /// from the application's jump list.
        /// </summary>
        public void ClearApplicationDestinations()
        {
            IApplicationDestinations destinations = (IApplicationDestinations)new CApplicationDestinations();
            if (!String.IsNullOrEmpty(_appId))
            {
                HResult setAppIDResult = destinations.SetAppID(_appId);
                setAppIDResult.ThrowIf();
            }
            try
            {
                //This does not remove pinned items.
                HResult removeAllDestinationsResult = destinations.RemoveAllDestinations();
                removeAllDestinationsResult.ThrowIf();
            }
            catch (FileNotFoundException)
            { /* There are no destinations. That's cool. */ }
        }

        /// <summary>
        /// Retrieves all application destinations belonging to the specified
        /// application destination type.
        /// </summary>
        /// <param name="type">The application destination type.</param>
        /// <returns>A copy of the application destinations belonging to
        /// the specified type; modifying the returned objects has no effect
        /// on the application's destination list.</returns>
        public IEnumerable<IJumpListDestination> GetApplicationDestinations(ApplicationDestinationType type)
        {
            if (type == ApplicationDestinationType.None)
                throw new ArgumentException("ApplicationDestinationType can't be NONE");

            IApplicationDocumentLists destinations = (IApplicationDocumentLists)new CApplicationDocumentLists();
            Guid iidObjectArray = typeof(IObjectArray).GUID;
           
            object obj;
            HResult getListResult = destinations.GetList((AppDocListType)type, 100, ref iidObjectArray, out obj);
            getListResult.ThrowIf();

            List<IJumpListDestination> returnValue = new List<IJumpListDestination>();

            Guid iidShellItem = typeof(IShellItem).GUID;
            Guid iidShellLink = typeof(IShellLinkW).GUID;
            IObjectArray array = (IObjectArray)obj;
           
            uint count;
            HResult getCountResult = array.GetCount(out count);
            getCountResult.ThrowIf();

            for (uint i = 0; i < count; ++i)
            {
                try
                {
                    array.GetAt(i, ref iidShellItem, out obj);
                }
                catch (Exception) //Wrong type
                { }

                if (obj == null)
                {
                    HResult getAtResult = array.GetAt(i, ref iidShellLink, out obj);
                    getAtResult.ThrowIf();
                    //This shouldn't fail since if it's not IShellItem
                    //then it must be IShellLink.

                    IShellLinkW link = (IShellLinkW)obj;
                    ShellLink wrapper = new ShellLink();

                    StringBuilder sb = new StringBuilder(256);  
                    HResult getPathResult = link.GetPath(sb, sb.Capacity, IntPtr.Zero, 2);
                    getPathResult.ThrowIf();
                    wrapper.Path = sb.ToString();

                    HResult getArgumentsResult = link.GetArguments(sb, sb.Capacity);
                    getArgumentsResult.ThrowIf();
                    wrapper.Arguments = sb.ToString();

                    int iconId;
                    HResult getIconLocationResult = link.GetIconLocation(sb, sb.Capacity, out iconId);
                    getIconLocationResult.ThrowIf();
                    wrapper.IconIndex = iconId;
                    wrapper.IconLocation = sb.ToString();

                    uint showCmd;
                    HResult getShowCmdResult = link.GetShowCmd(out showCmd);
                    getShowCmdResult.ThrowIf();
                    wrapper.ShowCommand = (WindowShowCommand)showCmd;

                    HResult getWorkingDirectoryResult = link.GetWorkingDirectory(sb, sb.Capacity);
                    getWorkingDirectoryResult.ThrowIf();
                    wrapper.WorkingDirectory = sb.ToString();

                    returnValue.Add(wrapper);
                }
                else //It's an IShellItem.
                {
                    IShellItem item = (IShellItem)obj;
                    ShellItem wrapper = new ShellItem();

                    string path;
                    HResult getDisplayNameResult = item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out path);
                    getDisplayNameResult.ThrowIf();
                    wrapper.Path = path;

                    //Title and Category are irrelevant here, because it's
                    //an IShellItem.  The user might want to see them, but he's
                    //free to go to the IShellItem and look at its property store.

                    returnValue.Add(wrapper);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Deletes the specified application destination from the application's
        /// jump list.
        /// </summary>
        /// <param name="destination">The application destination.</param>
        public void DeleteApplicationDestination(IJumpListDestination destination)
        {
            IApplicationDestinations destinations = (IApplicationDestinations)new CApplicationDestinations();
            if (!String.IsNullOrEmpty(_appId))
            {
               HResult setAppIDResult = destinations.SetAppID(_appId);
               setAppIDResult.ThrowIf();
            }

            HResult removeDestinationResult = destinations.RemoveDestination(destination.GetShellRepresentation());
            removeDestinationResult.ThrowIf();
        }

        /// <summary>
        /// Deletes all custom destinations from the application's jump list.
        /// </summary>
        public void ClearCustomDestinations()
        {
            try
            {
                HResult deleteListResult = _customDestinationList.DeleteList(_appId);
                deleteListResult.ThrowIf();
            }
            catch (FileNotFoundException)
            { /*Means the list is empty, that's cool. */ }
            
            _destinations.Clear();
        }

        /// <summary>
        /// Repopulates the application's jump list.
        /// Use this method after all current changes to 
        /// the application's jump list have been introduced,
        /// and you want the list to be refreshed.
        /// </summary>
        /// <returns><b>true</b> if the list was refreshed; <b>false</b>
        /// if the operation was cancelled.  The operation might have
        /// been cancelled if the <see cref="UserRemovedItems"/> event
        /// handler instructed us to cancel the operation.</returns>
        /// <remarks>
        /// If the user removed items from the jump list between the
        /// last refresh operation and this one, then the
        /// <see cref="UserRemovedItems"/> event will be invoked.
        /// If the event handler for this event instructed us to cancel
        /// the operation, then the current transaction is aborted,
        /// no items are added, and this method returns <b>false</b>.
        /// Check the return value to determine whether the jump list
        /// needs to be changed and the operation attempted again.
        /// </remarks>
        public bool Refresh()
        {
            if (!BeginList())
                return false;   //Operation was cancelled

            _tasks.RefreshTasks(_customDestinationList);
            _destinations.RefreshDestinations(_customDestinationList);

            switch (EnabledAutoDestinationType)
            {
                case ApplicationDestinationType.Frequent:
                    HResult appendKnownCategoryFrequentResult = _customDestinationList.AppendKnownCategory(KnownDestCategory.FREQUENT);
                    appendKnownCategoryFrequentResult.ThrowIf();
                    break;
                case ApplicationDestinationType.Recent:
                    HResult appendKnownCategoryRecentResult = _customDestinationList.AppendKnownCategory(KnownDestCategory.RECENT);
                    appendKnownCategoryRecentResult.ThrowIf();
                    break;
            }

            CommitList();
            return true;
        }

        /// <summary>
        /// Returns the maximum number of items to be placed
        /// in the application's jump list.  This number depends
        /// on factors such as the display resolution or monitor
        /// change - do not assume it is always constant.
        /// </summary>
        public uint MaximumSlotsInList
        {
            get
            {
                if (_maxSlotsInList == 0)
                {
                    RefreshMaxSlots();
                }
                return _maxSlotsInList;
            }
        }

        /// <summary>
        /// Cleans the resources associated with this jump list.
        /// </summary>
        public void Dispose()
        {
            SystemEvents.DisplaySettingsChanged -= _displaySettingsChangeHandler;
            if (_customDestinationList != null)
                Marshal.ReleaseComObject(_customDestinationList);
        }

        /// <summary>
        /// Register to this event to receive notifications when custom
        /// destinations are being removed from your jump list by the user.
        /// If you do not register to this event, you will not be able
        /// to refresh the list.  Additionally, if you attempt to add
        /// items to the list which have been previously removed by the user,
        /// the next refresh will fail to add your category.
        /// </summary>
        public event EventHandler<UserRemovedItemsEventArgs> UserRemovedItems;

        #region Implementation

        private void RefreshMaxSlots()
        {
            object obj;
            _customDestinationList.BeginList(out _maxSlotsInList, ref SafeNativeMethods.IID_IObjectArray, out obj);
            _customDestinationList.AbortList();
        }

        private bool BeginList()
        {
            if (UserRemovedItems == null)
            {
                throw new InvalidOperationException("You must register for the JumpListManager.UserRemovedItems event before adding any items");
            }

            object obj;
            _customDestinationList.BeginList(out _maxSlotsInList, ref SafeNativeMethods.IID_IObjectArray, out obj);

            IObjectArray removedItems = (IObjectArray)obj;
            uint count;
            removedItems.GetCount(out count);
            if (count == 0)
                return true;

            string[] removedItemsArr = new string[count];
            for (uint i = 0; i < count; ++i)
            {
                object item;
                removedItems.GetAt(i, ref SafeNativeMethods.IID_IUnknown, out item);

                try
                {
                    IShellLinkW shellLink = (IShellLinkW)item;
                    if (shellLink != null)
                    {
                        StringBuilder sb = new StringBuilder(256);
                        shellLink.GetPath(sb, sb.Capacity, IntPtr.Zero, 2);
                        removedItemsArr[i] = sb.ToString();
                    }
                    continue;
                }
                catch (InvalidCastException) //It's not a ShellLink
                {  }

                try
                {
                    IShellItem shellItem = (IShellItem)item;
                    if (shellItem != null)
                    {
                        string path;
                        shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out path);
                        removedItemsArr[i] = path;
                    }
                }
                catch (InvalidCastException)
                {
                    //It's neither a shell link nor a shell item.
                    //This is impossible.
                    Debug.Assert(false,
                        "List of removed items contains something that is neither a shell item nor a shell link");
                }
            }

            UserRemovedItemsEventArgs args = new UserRemovedItemsEventArgs(removedItemsArr);
            UserRemovedItems(this, args);
            if (args.Cancel)
            {
                _customDestinationList.AbortList();
            }
            return !args.Cancel;
        }

        private void CommitList()
        {
            _customDestinationList.CommitList();
        }

        #endregion
    }
    
    /// <summary>
    /// The application destination type.
    /// </summary>
    public enum ApplicationDestinationType
    {
        /// <summary>
        /// No application destination type is selected.
        /// </summary>
        None = -1,
        /// <summary>
        /// Destinations used recently.
        /// </summary>
        Recent = 0,
        /// <summary>
        /// Destinations used frequently.
        /// </summary>
        Frequent
    }

    /// <summary>
    /// The event arguments for the event that occurs
    /// when the user removes items from the application's
    /// jump list.
    /// </summary>
    public class UserRemovedItemsEventArgs : EventArgs
    {
        readonly string[] _removedItems;

        internal UserRemovedItemsEventArgs(string[] removedItems)
        {
            _removedItems = removedItems;
        }

        /// <summary>
        /// The collection of removed items. Each item is the path.
        /// </summary>
        public string[] RemovedItems
        {
            get
            {
                return _removedItems;
            }
        }

        /// <summary>
        /// Set to <b>true</b> if the current operation
        /// should be cancelled.  Should be used by the application
        /// if because of the items the user has removed
        /// there is no real work to do with the jump list.
        /// </summary>
        public bool Cancel { get; set; }
    }
}