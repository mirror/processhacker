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

        /// <summary>
        /// Creates a new instance of the Provider class.
        /// </summary>
        protected Provider()
            : this(new Dictionary<TKey, TValue>())
        { }

        /// <summary>
        /// Creates a new instance of the Provider class, specifying a 
        /// custom equality comparer.
        /// </summary>
        protected Provider(IEqualityComparer<TKey> comparer)
            : this(new Dictionary<TKey, TValue>(comparer))
        { }

        /// <summary>
        /// Creates a new instance of the Provider class, specifying a
        /// custom <seealso cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> instance.
        /// </summary>
        protected Provider(IDictionary<TKey, TValue> dictionary)
        {
            Name = string.Empty;
            Boosting = false;
            Busy = false;
            Disposing = false;
            Enabled = false;
            RunCount = 0;
            Unregistering = false;
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            this.Dictionary = dictionary;
            this.ListEntry = new LinkedListEntry<IProvider>();
            this.ListEntry.Value = this;
        }

        protected override void DisposeObject(bool disposing)
        {   
            this.Disposing = true;

            if (this.Disposed != null)
            {
                this.Disposed(this);
            }
        }

        public string Name { get; protected set; }

        public bool Boosting { get; set; }

        /// <summary>
        /// Determines whether the provider is currently updating.
        /// </summary>
        public bool Busy { get; private set; }

        /// <summary>
        /// Gets whether the provider is shutting down.
        /// </summary>
        protected bool Disposing { get; private set; }

        /// <summary>
        /// Determines whether the provider should update.
        /// </summary>
        public bool Enabled { get; set; }

        public LinkedListEntry<IProvider> ListEntry { get; private set; }

        public ProviderThread Owner { get; set; }

        /// <summary>
        /// Gets the number of times this provider has updated.
        /// </summary>
        public int RunCount { get; private set; }

        public bool Unregistering { get; set; }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public IDictionary<TKey, TValue> Dictionary { get; protected set; }

        /// <summary>
        /// Causes the provider to run immediately.
        /// </summary>
        public void Boost()
        {
            this.Owner.Boost(this);
        }

        /// <summary>
        /// Updates the provider. Do not call this function.
        /// </summary>
        public void Run()
        {
            // Bail out if we are disposing
            if (this.Disposing)
            {
                Logging.Log(Logging.Importance.Warning, "Provider (" + this.Name + "): RunOnce: currently disposing");
                return;
            }

            this.Busy = true;

            try
            {
                if (this.BeforeUpdate != null)
                    this.BeforeUpdate();
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    Error(ex);
                }
                else
                {
                    Logging.Log(Logging.Importance.Error, ex.ToString());
                }
            }

            try
            {
                this.Update();
                this.RunCount++;
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    Error(ex);
                }
                else
                {
                    Logging.Log(Logging.Importance.Error, ex.ToString());
                }
            }

            try
            {
                if (this.Updated != null)
                    this.Updated();
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    Error(ex);
                }
                else
                {
                    Logging.Log(Logging.Importance.Error, ex.ToString());
                }
            }
            this.Busy = false;
        }

        protected void OnDictionaryAdded(TValue item)
        {
            if (this.DictionaryAdded != null)
                this.DictionaryAdded(item);
        }

        protected void OnDictionaryModified(TValue oldItem, TValue newItem)
        {
            if (this.DictionaryModified != null)
                this.DictionaryModified(oldItem, newItem);
        }

        protected void OnDictionaryRemoved(TValue item)
        {
            if (this.DictionaryRemoved != null)
                this.DictionaryRemoved(item);
        }

        protected virtual void Update()
        { }
    }
}
