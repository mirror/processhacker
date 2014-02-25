/*
 * Process Hacker - 
 *   network provider
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

// 'member' is obsolete: 'text'
#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ProcessHacker.Common;
using ProcessHacker.Common.Messaging;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native;

namespace ProcessHacker
{
    public class NetworkItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int Tag;
        public string Id;
        public NetworkConnection Connection;
        public string LocalString;
        public string RemoteString;
        public bool LocalTouched;
        public bool RemoteTouched;
        public bool JustProcessed;
    }

    public class NetworkProvider : Provider<string, NetworkItem>
    {
        private class AddressResolveMessage : Message
        {
            public string Id;
            public bool Remote;
            public string HostName;
        }

        private MessageQueue _messageQueue = new MessageQueue();
        private Dictionary<IPAddress, string> _resolveCache = new Dictionary<IPAddress, string>();
        private FastResourceLock _resolveCacheLock = new FastResourceLock();

        public NetworkProvider()
            : base()
        {
            this.Name = this.GetType().Name;

            _messageQueue.AddListener(
                new MessageQueueListener<AddressResolveMessage>((message) =>
                {
                    if (Dictionary.ContainsKey(message.Id))
                    {
                        var item = Dictionary[message.Id];

                        if (message.Remote)
                            item.RemoteString = message.HostName;
                        else
                            item.LocalString = message.HostName;

                        item.JustProcessed = true;
                    }
                }));
        }

        protected override void Update()
        {
            var networkDict = Windows.GetNetworkConnections();
            var preKeyDict = new Dictionary<string, KeyValuePair<int, NetworkConnection>>();
            var keyDict = new Dictionary<string, NetworkItem>();
            Dictionary<string, NetworkItem> newDict =
                new Dictionary<string, NetworkItem>(this.Dictionary);

            // Flattens list, assigns IDs and counts
            foreach (var list in networkDict.Values)
            {
                foreach (var connection in list)
                {
                    if (connection.Pid == Program.CurrentProcessId &&
                        Settings.Instance.HideProcessHackerNetworkConnections)
                        continue;

                    string id = connection.Pid.ToString() + "-" + connection.Local.ToString() + "-" +
                        (connection.Remote != null ? connection.Remote.ToString() : "") + "-" + connection.Protocol.ToString();

                    if (preKeyDict.ContainsKey(id))
                        preKeyDict[id] = new KeyValuePair<int, NetworkConnection>(
                            preKeyDict[id].Key + 1, preKeyDict[id].Value);
                    else
                        preKeyDict.Add(id, new KeyValuePair<int, NetworkConnection>(1, connection));
                }
            }

            // Merges counts into IDs
            foreach (string s in preKeyDict.Keys)
            {
                var connection = preKeyDict[s].Value;
                NetworkItem item = new NetworkItem();

                item.Id = s + "-" + preKeyDict[s].Key.ToString();
                item.Connection = connection;
                keyDict.Add(s + "-" + preKeyDict[s].Key.ToString(), item);
            }

            foreach (var connection in this.Dictionary.Values)
            {
                if (!keyDict.ContainsKey(connection.Id))
                {
                    OnDictionaryRemoved(connection);
                    newDict.Remove(connection.Id);
                }
            }

            // Get resolve results.
            _messageQueue.Listen();

            foreach (var connection in keyDict.Values)
            {
                if (!this.Dictionary.ContainsKey(connection.Id))
                {
                    connection.Tag = this.RunCount;

                    // Resolve the IP addresses.
                    if (connection.Connection.Local != null)
                    {
                        if (!connection.Connection.Local.Address.GetAddressBytes().IsEmpty())
                        {
                            bool queue = false;

                            // See if IP address is in the cache.

                            _resolveCacheLock.AcquireShared();

                            try
                            {
                                if (_resolveCache.ContainsKey(connection.Connection.Local.Address))
                                {
                                    // We have the resolved address.
                                    connection.LocalString = _resolveCache[connection.Connection.Local.Address];
                                }
                                else
                                {
                                    queue = true;
                                }
                            }
                            finally
                            {
                                _resolveCacheLock.ReleaseShared();
                            }

                            if (queue)
                            {
                                // Queue for resolve.
                                WorkQueue.GlobalQueueWorkItemTag(
                                    new Action<string, bool, IPAddress>(this.ResolveAddresses),
                                    "network-resolve-local",
                                    connection.Id,
                                    false,
                                    connection.Connection.Local.Address
                                    );
                            }
                        }
                    }

                    if (connection.Connection.Remote != null)
                    {
                        if (!connection.Connection.Remote.Address.GetAddressBytes().IsEmpty())
                        {
                            bool queue = false;

                            _resolveCacheLock.AcquireShared();

                            try
                            {
                                if (_resolveCache.ContainsKey(connection.Connection.Remote.Address))
                                {
                                    // We have the resolved address.
                                    connection.RemoteString = _resolveCache[connection.Connection.Remote.Address];
                                }
                                else
                                {
                                    queue = true;
                                }
                            }
                            finally
                            {
                                _resolveCacheLock.ReleaseShared();
                            }

                            if (queue)
                            {
                                WorkQueue.GlobalQueueWorkItemTag(
                                    new Action<string, bool, IPAddress>(this.ResolveAddresses),
                                    "network-resolve-remote",
                                    connection.Id,
                                    true,
                                    connection.Connection.Remote.Address
                                    );
                            }
                        }
                    }

                    // Update the dictionary.
                    newDict.Add(connection.Id, connection);
                    OnDictionaryAdded(connection);
                }
                else
                {
                    if (
                        connection.Connection.State != Dictionary[connection.Id].Connection.State ||
                        Dictionary[connection.Id].JustProcessed
                        )
                    {
                        NetworkItem oldConnection = Dictionary[connection.Id].Clone() as NetworkItem;

                        newDict[connection.Id].Connection.State = connection.Connection.State;
                        newDict[connection.Id].JustProcessed = false;

                        OnDictionaryModified(oldConnection, newDict[connection.Id]);
                    }
                }
            }

            this.Dictionary = newDict;
        }

        private void ResolveAddresses(string id, bool remote, IPAddress address)
        {
            string hostName = null;
            bool inCache = false;

            // Last minute check of the cache.

            _resolveCacheLock.AcquireShared();

            try
            {
                if (_resolveCache.ContainsKey(address))
                {
                    hostName = _resolveCache[address];
                    inCache = true;
                }
            }
            finally
            {
                _resolveCacheLock.ReleaseShared();
            }

            // If it wasn't in the cache, resolve the address.
            if (!inCache)
            {
                try
                {
                    hostName = Dns.GetHostEntry(address).HostName;
                }
                catch (SocketException)
                {
                    // Host was not found.
                    return;
                }

                // Update the cache.

                _resolveCacheLock.AcquireExclusive();

                try
                {
                    // Add the name if not present already.
                    if (!string.IsNullOrEmpty(hostName))
                    {
                        if (!_resolveCache.ContainsKey(address))
                            _resolveCache.Add(address, hostName);
                    }
                }
                finally
                {
                    _resolveCacheLock.ReleaseExclusive();
                }
            }

            _messageQueue.Enqueue(new AddressResolveMessage()
            {
                Id = id,
                Remote = remote,
                HostName = hostName
            });
        }
    }
}
