using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker
{
    public delegate void ProviderUpdateOnce();
    public delegate object ProviderInvokeMethod(Delegate method, params object[] args);
    public delegate void ProviderDictionaryAdded(object item);
    public delegate void ProviderDictionaryModified(object item);
    public delegate void ProviderDictionaryRemoved(object item);
                                                           
    /// <summary>
    /// Provides services for continuously updating a dictionary.
    /// </summary>
    public class Provider<TKey, TValue>
    {
        private delegate void InvokeDelegate(object item);

        protected event ProviderUpdateOnce ProviderUpdate;
        public ProviderInvokeMethod Invoke;
        public event ProviderDictionaryAdded DictionaryAdded;
        public event ProviderDictionaryModified DictionaryModified;
        public event ProviderDictionaryRemoved DictionaryRemoved;
                                                           
        Thread _thread;
        Dictionary<TKey, TValue> _dictionary;

        bool _busy = false;
        bool _enabled = false;
        bool _useInvoke = false;
        int _interval;

        public Provider()
        {       
            _dictionary = new Dictionary<TKey, TValue>();

            _thread = new Thread(new ThreadStart(Update));
            _thread.Priority = ThreadPriority.Lowest;
            _thread.Start();
        }

        public bool Busy
        {
            get { return _busy; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool UseInvoke
        {
            get { return _useInvoke; }
            set { _useInvoke = value; }
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

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

        public void UpdateNow()
        {
            while (_busy) Thread.Sleep(10);

            if (ProviderUpdate != null)
                ProviderUpdate();
        }

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
