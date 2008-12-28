/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker
{                                                 
    /// <summary>
    /// Provides services for continuously updating a dictionary.
    /// </summary>
    public class Provider<TKey, TValue>
    {
        /// <summary>
        /// A generic delegate which is used when updating the dictionary.
        /// </summary>
        public delegate void ProviderUpdateOnce();

        /// <summary>
        /// Represents an invoke method (either Invoke() or BeginInvoke() on a form).
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="args">The arguments to pass to the method.</param>
        /// <returns></returns>
        public delegate object ProviderInvokeMethod(Delegate method, params object[] args);

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

        private delegate void InvokeDelegate(TValue item);
        private delegate void InvokeDelegate2(TValue oldItem, TValue newItem);

        /// <summary>
        /// Occurs when the provider needs to update the dictionary (after waiting the duration of the interval).
        /// </summary>
        protected event ProviderUpdateOnce ProviderUpdate;

        protected event System.Windows.Forms.MethodInvoker Killed;

        public event ProviderUpdateOnce BeforeUpdate;

        /// <summary>
        /// Occurs when the provider has been updated.
        /// </summary>
        public event ProviderUpdateOnce Updated;

        /// <summary>
        /// Occurs when the provider needs to invoke a method on another thread.
        /// </summary>
        public ProviderInvokeMethod Invoke;

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

        Thread _thread;
        List<Thread> _asyncThreads = new List<Thread>();
        Dictionary<TKey, TValue> _dictionary;

        object _busyLock = new object();
        bool _busy = false;
        bool _enabled = false;
        bool _useInvoke = false;
        int _runCount = 0;
        int _interval;

        /// <summary>
        /// Creates a new instance of the Provider class.
        /// </summary>
        public Provider()
        {
            _dictionary = new Dictionary<TKey, TValue>();

            _thread = new Thread(new ThreadStart(Update));
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            _thread.Priority = ThreadPriority.Lowest;
        }

        /// <summary>
        /// Determines whether the provider is currently updating.
        /// </summary>
        public bool Busy
        {
            get { return _busy; }
        }

        /// <summary>
        /// Determines whether the provider should update.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// Determines whether the provider should invoke the DictionaryAdded, Modified and Removed events.
        /// </summary>
        public bool UseInvoke
        {
            get { return _useInvoke; }
            set { _useInvoke = value; }
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
        public Dictionary<TKey, TValue> Dictionary
        {
            get { return _dictionary; }
            protected set { _dictionary = value; }
        }

        private void Update()
        {
            while (true)
            {
                if (_enabled)
                {
                    this.RunOnce();
                }

                Thread.Sleep(_interval);
            }
        }

        public void RunOnce()
        {
            lock (_busyLock)
            {
                _busy = true;

                if (ProviderUpdate != null)
                {
                    if (BeforeUpdate != null)
                        BeforeUpdate();

                    try
                    {
                        ProviderUpdate();
                        _runCount++;
                    }
                    catch (Exception ex)
                    {
                        if (Error != null)
                            Error(ex);
                    }

                    if (Updated != null)
                        Updated();
                }

                _busy = false;
            }
        }

        public void RunOnceAsync()
        {
            Thread t = new Thread(new ThreadStart(this.RunOnce));

            t.SetApartmentState(ApartmentState.STA);
            _asyncThreads.Add(t);
            t.Start();
        }

        /// <summary>
        /// Removes all handlers from the DictionaryAdded, Modified and Removed events.
        /// </summary>
        public void ClearEvents()
        {
            DictionaryAdded = null;
            DictionaryModified = null;
            DictionaryRemoved = null;
        }

        public void Kill()
        {
            _thread.Abort();

            foreach (Thread t in _asyncThreads)
                t.Abort();

            if (this.Killed != null)
                this.Killed();
        }

        private void CallEvent(Delegate e, TValue item, bool useInvoke)
        {
            if (useInvoke)
            {
                this.Invoke(new InvokeDelegate(delegate(TValue item_)
                {
                    CallEvent(e, item_, false);
                }), item);
            }
            else
            {
                if (e != null)
                {
                    try
                    {
                        e.DynamicInvoke(item);
                    }
                    catch
                    { }
                }
            }
        }

        private void CallEvent(Delegate e, TValue oldItem, TValue newItem, bool useInvoke)
        {
            if (useInvoke)
            {
                this.Invoke(new InvokeDelegate2(delegate(TValue oldItem_, TValue newItem_)
                {
                    CallEvent(e, oldItem_, newItem_, false);
                }), oldItem, newItem);
            }
            else
            {
                if (e != null)
                {
                    try
                    {
                        e.DynamicInvoke(oldItem, newItem);
                    }
                    catch
                    { }
                }
            }
        }

        protected void CallDictionaryAdded(TValue item)
        {
            CallDictionaryAdded(item, _useInvoke);
        }

        protected void CallDictionaryAdded(TValue item, bool useInvoke)
        {
            CallEvent(this.DictionaryAdded, item, useInvoke);
        }

        protected void CallDictionaryModified(TValue oldItem, TValue newItem)
        {
            CallDictionaryModified(oldItem, newItem, _useInvoke);
        }

        protected void CallDictionaryModified(TValue oldItem, TValue newItem, bool useInvoke)
        {
            CallEvent(this.DictionaryModified, oldItem, newItem, useInvoke);
        }

        protected void CallDictionaryRemoved(TValue item)
        {
            CallDictionaryRemoved(item, _useInvoke);
        }

        protected void CallDictionaryRemoved(TValue item, bool useInvoke)
        {
            CallEvent(this.DictionaryRemoved, item, useInvoke);
        }
    }
}
