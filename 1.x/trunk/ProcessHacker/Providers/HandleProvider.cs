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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker
{
    public class HandleItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int RunId;
        public SystemHandleEntry Handle;
        public ObjectInformation ObjectInfo;
    }

    public class HandleProvider : Provider<short, HandleItem>
    {
        private ProcessHandle _processHandle;
        private int _pid;

        public HandleProvider(int pid)
            : base()
        {
            this.Name = this.GetType().Name;
            _pid = pid;

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

            this.Disposed += (provider) => { if (_processHandle != null) _processHandle.Dispose(); };
        }

        protected override void Update()
        {
            if (_processHandle == null)
                return;

            var handles = Windows.GetHandles();
            var processHandles = new Dictionary<short, SystemHandleEntry>();
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
                // If a handle now points to a different object, force a re-add.
                if (!processHandles.ContainsKey(h) ||
                    processHandles[h].Object != this.Dictionary[h].Handle.Object)
                {
                    this.OnDictionaryRemoved(this.Dictionary[h]);
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

                    item.RunId = this.RunCount;
                    item.Handle = processHandles[h];
                    item.ObjectInfo = info;

                    newdictionary.Add(h, item);
                    this.OnDictionaryAdded(item);
                }
                else
                {
                    // check if the handle has been modified
                    if (this.Dictionary[h].Handle.Flags != processHandles[h].Flags)
                    {
                        this.Dictionary[h].Handle.Flags = processHandles[h].Flags;
                        this.OnDictionaryModified(null, this.Dictionary[h]);
                    }
                }
            }

            this.Dictionary = newdictionary;
        }

        public bool HideHandlesWithNoName { get; set; }

        public int Pid
        {
            get { return _pid; }
        }
    }
}
