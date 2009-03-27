using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker
{
    internal class SharedThreadProvider
    {
        private List<IProvider> _providers = new List<IProvider>();
        private Thread _thread;
        private int _interval;

        public SharedThreadProvider(int interval)
        {
            _interval = interval;
            _thread = new Thread(new ThreadStart(this.Update));
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            _thread.Priority = ThreadPriority.Lowest;
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public void Add(IProvider provider)
        {
            provider.CreateThread = false;
            _providers.Add(provider);
        }

        public void Remove(IProvider provider)
        {
            _providers.Remove(provider);
            provider.CreateThread = true;
        }

        private void Update()
        {
            while (true)
            {
                foreach (var provider in _providers)
                    if (provider.Enabled)
                        provider.RunOnce();

                Thread.Sleep(_interval);
            }
        }
    }
}
