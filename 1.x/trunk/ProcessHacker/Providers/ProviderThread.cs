/*
 * Process Hacker - 
 *   provider host thread
 * 
 * Copyright (C) 2009 wj32
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
using System.Collections.ObjectModel;
using System.Threading;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

// This code is partially back-ported from PH 2.x.

namespace ProcessHacker
{
    public class ProviderThread : BaseObject, IEnumerable<IProvider>
    {
        private LinkedListEntry<IProvider> _listHead = new LinkedListEntry<IProvider>();
        private int _boostCount = 0;

        private int _count;
        private Thread _thread;
        private ThreadHandle _threadHandle;
        private FastEvent _initializedEvent = new FastEvent(false);
        private bool _terminating = false;
        private int _interval;
        private TimerHandle _timerHandle;

        public ProviderThread(int interval)
        {
            LinkedList.InitializeListHead(_listHead);

            _timerHandle = TimerHandle.Create(TimerAccess.All, TimerType.SynchronizationTimer); 
            this.Interval = interval;

            _thread = new Thread(new ThreadStart(this.Update), ProcessHacker.Common.Utils.QuarterStackSize);
            _thread.IsBackground = true;
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
            _thread.Priority = ThreadPriority.Lowest;
            _initializedEvent.Wait();
        }

        protected override void DisposeObject(bool disposing)
        {
            _terminating = true;
            _threadHandle.Alert();
            _threadHandle.Wait();
            _threadHandle.Dispose();
            _threadHandle = null;
            _thread = null;

            _timerHandle.Dispose();
        }

        public int Count
        {
            get { return _count; }
        }

        public int Interval
        {
            get { return _interval; }
            set
            {
                if (_interval != value)
                {
                    _interval = value;
                    _timerHandle.Set(_interval * Win32.TimeMsTo100Ns, _interval);
                }
            }
        }

        public void Add(IProvider provider)
        {
            provider.Owner = this;
            provider.Unregistering = false;
            provider.Boosting = false;

            lock (_listHead)
            {
                LinkedList.InsertTailList(_listHead, provider.ListEntry);
                _count++;
            }
        }

        public void Boost(IProvider provider)
        {
            if (provider.Unregistering)
                return;

            lock (_listHead)
            {
                LinkedList.RemoveEntryList(provider.ListEntry);
                LinkedList.InsertHeadList(_listHead, provider.ListEntry);

                provider.Boosting = true;
                _boostCount++;
            }

            // Wake up the thread.
            _threadHandle.Alert();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<IProvider> GetEnumerator()
        {
            for (
                LinkedListEntry<IProvider> entry = _listHead.Flink;
                entry != _listHead;
                entry = entry.Flink
                )
            {
                yield return entry.Value;
            }
        }

        public void Remove(IProvider provider)
        {
            provider.Unregistering = false;

            lock (_listHead)
            {
                LinkedList.RemoveEntryList(provider.ListEntry);

                // Fix the boost count.
                if (provider.Boosting)
                    _boostCount--;

                _count--;
            }
        }

        private void Update()
        {
            LinkedListEntry<IProvider> tempListHead = new LinkedListEntry<IProvider>();
            LinkedListEntry<IProvider> listEntry;
            NtStatus status = NtStatus.Success;

            _threadHandle = ThreadHandle.OpenCurrent(
                ThreadAccess.Alert | (ThreadAccess)StandardRights.Synchronize
                );
            _initializedEvent.Set();

            while (!_terminating)
            {
                LinkedList.InitializeListHead(tempListHead);

                Monitor.Enter(_listHead);

                // Main loop.

                while (true)
                {
                    if (status == NtStatus.Alerted)
                    {
                        // Check if we have any more providers to boost.
                        if (_boostCount == 0)
                            break;
                    }

                    listEntry = LinkedList.RemoveHeadList(_listHead);

                    if (listEntry == _listHead)
                        break;

                    // Add the provider to the temp list.
                    LinkedList.InsertTailList(tempListHead, listEntry);

                    if (status != NtStatus.Alerted)
                    {
                        if (!listEntry.Value.Enabled || listEntry.Value.Unregistering)
                            continue;
                    }
                    else
                    {
                        if (listEntry.Value.Unregistering)
                        {
                            // Give the unregistering thread a chance to fix 
                            // the boost count.
                            Monitor.Exit(_listHead);
                            Monitor.Enter(_listHead);

                            continue;
                        }
                    }

                    if (status == NtStatus.Alerted)
                    {
                        listEntry.Value.Boosting = false;
                        _boostCount--;
                    }

                    Monitor.Exit(_listHead);

                    try
                    {
                        listEntry.Value.Run();
                    }
                    finally
                    {
                        Monitor.Enter(_listHead);
                    }
                }

                // Re-add the items in the temp list to the main list.

                while ((listEntry = LinkedList.RemoveHeadList(tempListHead)) != tempListHead)
                    LinkedList.InsertTailList(_listHead, listEntry);

                Monitor.Exit(_listHead);

                // Wait for the interval. We may get alerted.
                status = _timerHandle.Wait(true);
            }
        }
    }
}
