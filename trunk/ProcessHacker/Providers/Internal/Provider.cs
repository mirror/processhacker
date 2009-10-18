/*
 * Process Hacker - 
 *   provider base class
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.Collections.Generic;
using System.Threading;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;

namespace ProcessHacker
{                                                 
    /// <summary>
    /// Provides services for continuously updating a dictionary.
    /// </summary>
    public abstract class Provider<TKey, TValue> : BaseObject, IProvider
    {
        /// <summary>
        /// A generic delegate which is used when updating the dictionary.
        /// </summary>
        public delegate void ProviderUpdateOnce();

        /// <summary>
        /// Represents a handler called when a dictionary item is added.
        /// </summary>
        /// <param name="item">The added item.</param>
        public delegate void ProviderDictionaryAdded(TValue item);

        /// <summary>
        /// Represents a handler called when a dictionary item is modified.
        /// </summary>
        /// <param name="item">The modified item.</param>
        public delegate void ProviderDictionaryModified(TValue oldItem, TValue newItem);

        /// <summary>
        /// Represents a handler called when a dictionary item is removed.
        /// </summary>
        /// <param name="item">The removed item.</param>
        public delegate void ProviderDictionaryRemoved(TValue item);

        /// <summary>
        /// Represents a handler called when an error occurs while updating.
        /// </summary>
        /// <param name="ex">The raised exception.</param>
        public delegate void ProviderError(Exception ex);

        /// <summary>
        /// Occurs when the provider needs to update the dictionary (after waiting the duration of the interval).
        /// </summary>
        protected event ProviderUpdateOnce ProviderUpdate;

        public new event Action<IProvider> Disposed;

        public event ProviderUpdateOnce BeforeUpdate;

        /// <summary>
        /// Occurs when the provider has been updated.
        /// </summary>
        public event ProviderUpdateOnce Updated;

        /// <summary>
        /// Occurs when the provider adds an item to the dictionary.
        /// </summary>
        public event ProviderDictionaryAdded DictionaryAdded;

        /// <summary>
        /// Occurs when the provider modifies an item in the dictionary.
        /// </summary>
        public event ProviderDictionaryModified DictionaryModified;

        /// <summary>
        /// Occurs when the provider removes an item from the dictionary.
        /// </summary>
        public event ProviderDictionaryRemoved DictionaryRemoved;

        /// <summary>
        /// Occurs when an exception is raised while updating.
        /// </summary>
        public event ProviderError Error;

        private string _name = string.Empty;
        private Thread _thread;
        private IDictionary<TKey, TValue> _dictionary;

        private object _busyLock = new object();
        private bool _disposing = false;
        private bool _busy = false;
        private bool _createThread = true;
        private bool _enabled = false;
        private int _runCount = 0;
        private int _interval;

        /// <summary>
        /// Creates a new instance of the Provider class.
        /// </summary>
        public Provider()
            : this(new Dictionary<TKey, TValue>())
        { }

        /// <summary>
        /// Creates a new instance of the Provider class, specifying a 
        /// custom equality comparer.
        /// </summary>
        public Provider(IEqualityComparer<TKey> comparer)
            : this(new Dictionary<TKey, TValue>(comparer))
        { }

        /// <summary>
        /// Creates a new instance of the Provider class, specifying a
        /// custom <seealso cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> instance.
        /// </summary>
        public Provider(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            _dictionary = dictionary;
        }

        protected override void DisposeObject(bool disposing)
        {   
            Logging.Log(Logging.Importance.Information, "Provider (" + this.Name + "): disposing (" + disposing.ToString() + ")");

            _disposing = true;

            if (disposing)
                Monitor.Enter(_busyLock);

            if (this.Disposed != null)
            {
                try
                {
                    this.Disposed(this);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }

            if (disposing)
                Monitor.Exit(_busyLock);

            Logging.Log(Logging.Importance.Information, "Provider (" + this.Name + "): finished disposing (" + disposing.ToString() + ")");
        }

        public string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
                if (_name == null)
                    _name = string.Empty;
            }
        }

        /// <summary>
        /// Determines whether the provider is currently updating.
        /// </summary>
        public bool Busy
        {
            get { return _busy; }
        }

        /// <summary>
        /// If enabled, the provider manages a background thread for the updater.
        /// </summary>
        public bool CreateThread
        {
            get { return _createThread; }
            set { _createThread = value; }
        }

        /// <summary>
        /// Determines whether the provider should update.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                if (_enabled && _createThread && _thread == null)
                {
                    _thread = new Thread(new ThreadStart(Update));
                    _thread.IsBackground = true;
                    _thread.SetApartmentState(ApartmentState.STA);
                    _thread.Start();
                    _thread.Priority = ThreadPriority.Lowest;
                }
            }
        }

        /// <summary>
        /// Gets the number of times this provider has updated.
        /// </summary>
        public int RunCount
        {
            get { return _runCount; }
        }

        /// <summary>
        /// Gets or sets the interval to wait between each update.
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public IDictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
            protected set { _dictionary = value; }
        }

        /// <summary>
        /// Updates the provider if it is enabled.
        /// </summary>
        private void Update()
        {
            while (true)
            {
                if (_enabled && !_disposing)
                {
                    this.RunOnce();
                }

                Thread.Sleep(_interval);
            }
        }

        /// <summary>
        /// Updates the provider. If it is already updating, this function waits until it finishes.
        /// </summary>
        public void RunOnce()
        {
            lock (_busyLock)
            {
                // Bail out if we are disposing
                if (_disposing)
                {
                    Logging.Log(Logging.Importance.Warning, "Provider (" + _name + "): RunOnce: currently disposing");
                    return;
                }

                _busy = true;

                if (ProviderUpdate != null)
                {
                    try
                    {
                        if (BeforeUpdate != null)
                            BeforeUpdate();
                    }
                    catch
                    { }

                    try
                    {
                        ProviderUpdate();
                        _runCount++;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);

                        if (Error != null)
                        {
                            try
                            {
                                Error(ex);
                            }
                            catch
                            { }
                        }
                        else
                        {
                            throw ex;
                        }
                    }

                    try
                    {
                        if (Updated != null)
                            Updated();
                    }
                    catch
                    { }
                }

                _busy = false;
            }
        }

        /// <summary>
        /// Updates the provider in an internal worker thread.
        /// </summary>
        public void RunOnceAsync()
        {
            WorkQueue.GlobalQueueWorkItemTag(new Action(this.RunOnce), "provider-runonceasync");
        }

        /// <summary>
        /// Executes code as soon as no updater is running.
        /// </summary>
        public void InterlockedExecute(Delegate action, params object[] args)
        {
            this.InterlockedExecute(action, -1, args);
        }

        /// <summary>
        /// Executes code as soon as no updater is running.
        /// </summary>
        public void InterlockedExecute(Delegate action, int timeout, params object[] args)
        {
            lock (_busyLock)
                action.DynamicInvoke(args);
        }

        /// <summary>
        /// Waits for the current update process to finish. If an update process is not currently 
        /// running, this function returns immediately.
        /// </summary>
        public void Wait()
        {
            this.Wait(-1);
        }

        /// <summary>
        /// Waits for the current update process to finish. If an update process is not currently 
        /// running, this function returns immediately. You may specify a timeout for the wait.
        /// </summary>
        /// <param name="timeout">The time in milliseconds to wait for the update process to finish.</param>
        /// <returns>Whether the update process was finished before the timeout.</returns>
        public bool Wait(int timeout)
        {
            if (Monitor.TryEnter(_busyLock, timeout))
            {
                Monitor.Exit(_busyLock);
                return true;
            }

            return false;
        }

        private void CallEvent(Delegate e, params object[] args)
        {
            if (e != null)
            {
                try
                {
                    e.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }
        }

        protected void OnDictionaryAdded(TValue item)
        {
            this.CallEvent(this.DictionaryAdded, item);
        }

        protected void OnDictionaryModified(TValue oldItem, TValue newItem)
        {
            this.CallEvent(this.DictionaryModified, oldItem, newItem);
        }

        protected void OnDictionaryRemoved(TValue item)
        {
            this.CallEvent(this.DictionaryRemoved, item);
        }
    }
}
