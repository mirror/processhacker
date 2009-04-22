/*
 * Process Hacker - 
 *   handle provider
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
using ProcessHacker.Native;
using ProcessHacker.Native.Objects;

namespace ProcessHacker
{
    public class HandleItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public SystemHandleInformation Handle;
        public ObjectInformation ObjectInfo;
    }

    public class HandleProvider : Provider<short, HandleItem>
    {
        private ProcessHandle _processHandle;
        private int _pid;

        public HandleProvider(int PID)
            : base()
        {
            _pid = PID;

            try
            {
                _processHandle = new ProcessHandle(_pid, ProcessHacker.Native.Security.ProcessAccess.DupHandle);
            }
            catch
            {
                try
                {
                    _processHandle = new ProcessHandle(_pid, Program.MinProcessGetHandleInformationRights);
                }
                catch
                { }
            }

            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);  
            this.Disposed += (provider) => { if (_processHandle != null) _processHandle.Dispose(); };
        }

        private void UpdateOnce()
        {
            var handles = Windows.GetHandles();
            var processHandles = new Dictionary<short, SystemHandleInformation>();
            var newdictionary = new Dictionary<short, HandleItem>(this.Dictionary);

            foreach (var handle in handles)
            {
                if (handle.ProcessId == _pid)
                {
                    processHandles.Add(handle.Handle, handle);
                }
            }

            // look for closed handles
            foreach (short h in this.Dictionary.Keys)
            {
                if (!processHandles.ContainsKey(h) ||
                    processHandles[h].Object != this.Dictionary[h].Handle.Object)
                {
                    this.CallDictionaryRemoved(this.Dictionary[h]);
                    newdictionary.Remove(h);
                }
            }

            // look for new handles
            foreach (short h in processHandles.Keys)
            {
                if (!this.Dictionary.ContainsKey(h))
                {
                    ObjectInformation info;
                    HandleItem item = new HandleItem();

                    try
                    {
                        info = processHandles[h].GetHandleInfo(_processHandle);

                        if ((info.BestName == null || info.BestName == "") &&
                            HideHandlesWithNoName)
                            continue;
                    }
                    catch
                    {
                        continue;
                    }

                    item.Handle = processHandles[h];
                    item.ObjectInfo = info;

                    newdictionary.Add(h, item);
                    this.CallDictionaryAdded(item);
                }
            }

            this.Dictionary = newdictionary;
        }

        public bool HideHandlesWithNoName { get; set; }

        public int PID
        {
            get { return _pid; }
        }
    }
}
