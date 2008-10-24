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
    public delegate void ProviderDictionaryAdded(object item);

    /// <summary>
    /// Represents a handler called when a dictionary item is modified.
    /// </summary>
    /// <param name="item">The modified item.</param>
    public delegate void ProviderDictionaryModified(object item);

    /// <summary>
    /// Represents a handler called when a dictionary item is removed.
    /// </summary>
    /// <param name="item">The removed item.</param>
    public delegate void ProviderDictionaryRemoved(object item);
                                                          
    /// <summary>
    /// Provides services for continuously updating a dictionary.
    /// </summary>
    public class Provider<TKey, TValue>
    { 
        private delegate void InvokeDelegate(object item);
                             
        /// <summary>
        /// Occurs when the provider needs to update the dictionary (after waiting the duration of the interval).
        /// </summary>
        protected event ProviderUpdateOnce ProviderUpdate;

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
                                                           
        Thread _thread;
        Dictionary<TKey, TValue> _dictionary;

        bool _busy = false;
        bool _enabled = false;
        bool _useInvoke = false;
        int _interval;

        /// <summary>
        /// Creates a new instance of the Provider class.
        /// </summary>
        public Provider()
        {          
            _dictionary = new Dictionary<TKey, TValue>();

            _thread = new Thread(new ThreadStart(Update));
            _thread.Priority = ThreadPriority.Lowest;
            _thread.Start();
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
                    _busy = true;

                    if (ProviderUpdate != null)
                        ProviderUpdate();
                    
                    _busy = false;
                }

                Thread.Sleep(_interval);
            }
        }

        /// <summary>
        /// Updates the dictionary after waiting for the current update operation to finish.
        /// </summary>
        public void UpdateNow()
        {
            while (_busy) Thread.Sleep(10);

            if (ProviderUpdate != null)
                ProviderUpdate();
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

        protected void CallDictionaryAdded(object item)
        {
            CallDictionaryAdded(item, _useInvoke);
        }

        protected void CallDictionaryAdded(object item, bool useInvoke)
        {
            if (useInvoke)
            {
                this.Invoke(new InvokeDelegate(delegate(object item_)
                {
                    CallDictionaryAdded(item, false);
                }), item);
            }
            else
            {
                if (this.DictionaryAdded != null)
                    this.DictionaryAdded(item);
            }
        }

        protected void CallDictionaryModified(object item)
        {
            CallDictionaryModified(item, _useInvoke);
        }

        protected void CallDictionaryModified(object item, bool useInvoke)
        {
            if (useInvoke)
            {
                this.Invoke(new InvokeDelegate(delegate(object item_)
                {
                    CallDictionaryModified(item, false);
                }), item);
            }
            else
            {
                if (this.DictionaryModified != null)
                    this.DictionaryModified(item);
            }
        }

        protected void CallDictionaryRemoved(object item)
        {
            CallDictionaryRemoved(item, _useInvoke);
        }

        protected void CallDictionaryRemoved(object item, bool useInvoke)
        {
            if (useInvoke)
            {
                this.Invoke(new InvokeDelegate(delegate(object item_)
                {
                    CallDictionaryRemoved(item, false);
                }), item);
            }
            else
            {
                if (this.DictionaryRemoved != null)
                    this.DictionaryRemoved(item);
            }
        }
    }
}
