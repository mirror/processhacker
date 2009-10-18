using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace ProcessHacker
{
    public class SharedThreadProvider : IDisposable
    {
        private object _disposeLock = new object();
        private bool _disposed;
        private List<IProvider> _providers = new List<IProvider>();
        private Thread _thread;
        private int _interval;

        public SharedThreadProvider(int interval)
        {
            _interval = interval;
            _thread = new Thread(new ThreadStart(this.Update), ProcessHacker.Common.Utils.QuarterStackSize);
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            _thread.Priority = ThreadPriority.Lowest;
        }

        ~SharedThreadProvider()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Monitor.Enter(_disposeLock);
                    Monitor.Enter(_providers);
                }

                if (!_disposed)
                {
                    _thread.Abort();
                    _thread = null;

                    IProvider[] providers = _providers.ToArray();

                    foreach (IProvider provider in providers)
                        this.Remove(provider);

                    _disposed = true;
                }
            }
            finally
            {
                if (disposing)
                {
                    Monitor.Exit(_disposeLock);
                    Monitor.Exit(_providers);
                }
            }
        }

        public int Count
        {
            get { return _providers.Count; }
        }

        public ReadOnlyCollection<IProvider> Providers
        {
            get { return new ReadOnlyCollection<IProvider>(_providers); }
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public void Add(IProvider provider)
        {
            provider.CreateThread = false;
            provider.Disposed += provider_Disposed;

            lock (_providers)
                _providers.Add(provider);
        }

        public void Remove(IProvider provider)
        {
            lock (_providers)
                _providers.Remove(provider);

            provider.CreateThread = true;
            provider.Disposed -= provider_Disposed;
        }

        private void provider_Disposed(IProvider provider)
        {
            if (_providers.Contains(provider))
                this.Remove(provider);
        }

        private void Update()
        {
            while (true)
            {
                IProvider[] providers;

                lock (_providers)
                    providers = _providers.ToArray();

                foreach (var provider in providers)
                    if (provider.Enabled)
                        provider.RunOnce();

                Thread.Sleep(_interval);
            }
        }
    }
}
