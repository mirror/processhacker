using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ProcessHacker.Common.Objects;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native.Objects;

namespace ProcessHacker
{
    public class SharedThreadProvider : BaseObject
    {
        private List<IProvider> _providers = new List<IProvider>();
        private Thread _thread;
        private bool _terminating = false;
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

        protected override void DisposeObject(bool disposing)
        {
            _terminating = true;
            _thread.Interrupt();
            _thread = null;

            IProvider[] providers = _providers.ToArray();

            foreach (IProvider provider in providers)
                this.Remove(provider);
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

            lock (_providers)
                _providers.Add(provider);
        }

        public void Remove(IProvider provider)
        {
            lock (_providers)
                _providers.Remove(provider);

            provider.CreateThread = true;
        }

        private void Update()
        {
            while (!_terminating)
            {
                IProvider[] providers;

                lock (_providers)
                    providers = _providers.ToArray();

                foreach (var provider in providers)
                {
                    if (_terminating)
                        return;

                    if (provider.Enabled)
                        provider.RunOnce();
                }

                try
                {
                    Thread.Sleep(_interval);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
            }
        }
    }
}
