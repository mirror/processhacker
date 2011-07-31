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
    /// Provides services for continuously updating data.
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
        private IDictionary<TKey, TValue> _dictionary;

        private bool _disposing = false;
        private bool _boosting = false;
        private bool _busy = false;
        private bool _enabled = false;
        private LinkedListEntry<IProvider> _listEntry;
        private ProviderThread _owner;
        private int _runCount = 0;
        private bool _unregistering = false;

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
            _listEntry = new LinkedListEntry<IProvider>();
            _listEntry.Value = this;
        }

        protected override void DisposeObject(bool disposing)
        {   
            Logging.Log(Logging.Importance.Information, "Provider (" + this.Name + "): disposing (" + disposing.ToString() + ")");

            _disposing = true;

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

        public bool Boosting
        {
            get { return _boosting; }
            set { _boosting = value; }
        }

        /// <summary>
        /// Determines whether the provider is currently updating.
        /// </summary>
        public bool Busy
        {
            get { return _busy; }
        }

        /// <summary>
        /// Gets whether the provider is shutting down.
        /// </summary>
        protected bool Disposing
        {
            get { return _disposing; }
        }

        /// <summary>
        /// Determines whether the provider should update.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public LinkedListEntry<IProvider> ListEntry
        {
            get { return _listEntry; }
        }

        public ProviderThread Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        /// <summary>
        /// Gets the number of times this provider has updated.
        /// </summary>
        public int RunCount
        {
            get { return _runCount; }
        }

        public bool Unregistering
        {
            get { return _unregistering; }
            set { _unregistering = value; }
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
        /// Causes the provider to run immediately.
        /// </summary>
        public void Boost()
        {
            _owner.Boost(this);
        }

        /// <summary>
        /// Updates the provider. Do not call this function.
        /// </summary>
        public void Run()
        {
            // Bail out if we are disposing
            if (_disposing)
            {
                Logging.Log(Logging.Importance.Warning, "Provider (" + _name + "): RunOnce: currently disposing");
                return;
            }

            _busy = true;

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
                    this.Update();
                    _runCount++;
                }
                catch (Exception ex)
                {
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
                        Logging.Log(Logging.Importance.Error, ex.ToString());
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

        protected virtual void Update()
        { }
    }
}
