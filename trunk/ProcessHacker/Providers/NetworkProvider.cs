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

#pragma warning disable 0618

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ProcessHacker.Common;
using ProcessHacker.Native;
using System;
using ProcessHacker.Common.Messaging;

namespace ProcessHacker
{
    public class NetworkProvider : Provider<string, NetworkConnection>
    {
        private class AddressResolveMessage : Message
        {
            public string Id;
            public bool Remote;
            public string HostName;
        }

        private MessageQueue _messageQueue = new MessageQueue();
        private Dictionary<long, string> _resolveCache =
            new Dictionary<long, string>();

        public NetworkProvider()
            : base()
        {
            this.Name = this.GetType().Name;
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);

            _messageQueue.AddListener(
                new MessageQueueListener<AddressResolveMessage>((message) =>
                    {
                        if (Dictionary.ContainsKey(message.Id))
                        {
                            var modConnection = Dictionary[message.Id];

                            if (message.Remote)
                                modConnection.RemoteString = message.HostName;
                            else
                                modConnection.LocalString = message.HostName;

                            OnDictionaryModified(Dictionary[message.Id], modConnection);
                            Dictionary[message.Id] = modConnection;
                        }
                        else
                        {
                            Logging.Log(Logging.Importance.Warning, message.Id + " was not present!");
                        }
                    }));
        }

        private void UpdateOnce()
        {
            var networkDict = Windows.GetNetworkConnections();
            var preKeyDict = new Dictionary<string, KeyValuePair<int, NetworkConnection>>();
            var keyDict = new Dictionary<string, NetworkConnection>();
            Dictionary<string, NetworkConnection> newDict = 
                new Dictionary<string, NetworkConnection>(this.Dictionary);

            // flattens list, assigns IDs and counts
            foreach (var list in networkDict.Values)
            {
                foreach (var connection in list)
                {
                    if (connection.Pid == Program.CurrentProcessId &&
                        Properties.Settings.Default.HideProcessHackerNetworkConnections)
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

            // merges counts into IDs
            foreach (string s in preKeyDict.Keys)
            {
                var connection = preKeyDict[s].Value;

                connection.Id = s + "-" + preKeyDict[s].Key.ToString();
                keyDict.Add(s + "-" + preKeyDict[s].Key.ToString(), connection);
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
                    NetworkConnection newConnection = connection;

                    newConnection.Tag = this.RunCount;

                    // Resolve the IP addresses.
                    if (newConnection.Local != null)
                    {
                        if (newConnection.Local.Address.ToString() != "0.0.0.0")
                        {
                            // See if IP address is in the cache.
                            lock (_resolveCache)
                            {
                                if (_resolveCache.ContainsKey(newConnection.Local.Address.Address))
                                {
                                    // We have the resolved address.
                                    newConnection.LocalString = _resolveCache[newConnection.Local.Address.Address];
                                }
                                else
                                {
                                    // Queue for resolve.
                                    WorkQueue.GlobalQueueWorkItemTag(
                                        new Action<string, bool, IPAddress>(this.ResolveAddresses),
                                        "network-resolve-local",
                                        newConnection.Id,
                                        false,
                                        newConnection.Local.Address
                                        );
                                }
                            }
                        }
                    }

                    if (newConnection.Remote != null)
                    {
                        if (newConnection.Remote.Address.ToString() != "0.0.0.0")
                        {
                            lock (_resolveCache)
                            {
                                if (_resolveCache.ContainsKey(newConnection.Remote.Address.Address))
                                {
                                    // We have the resolved address.
                                    newConnection.RemoteString = _resolveCache[newConnection.Remote.Address.Address];
                                }
                                else
                                {
                                    WorkQueue.GlobalQueueWorkItemTag(
                                        new Action<string, bool, IPAddress>(this.ResolveAddresses),
                                        "network-resolve-remote",
                                        newConnection.Id,
                                        true,
                                        newConnection.Remote.Address
                                        );
                                }
                            }
                        }
                    }

                    // Update the dictionary.
                    newDict.Add(newConnection.Id, newConnection);
                    OnDictionaryAdded(newConnection);
                }
                else
                {
                    if (connection.State != Dictionary[connection.Id].State)
                    {
                        var newConnection = Dictionary[connection.Id];

                        newConnection.State = connection.State;

                        OnDictionaryModified(Dictionary[connection.Id], newConnection);
                        Dictionary[connection.Id] = connection;
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
            lock (_resolveCache)
            {
                if (_resolveCache.ContainsKey(address.Address))
                {
                    hostName = _resolveCache[address.Address];
                    inCache = true;
                }
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
                lock (_resolveCache)
                {
                    // Add the name if not present already.
                    if (!string.IsNullOrEmpty(hostName))
                        if (!_resolveCache.ContainsKey(address.Address))
                            _resolveCache.Add(address.Address, hostName);
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
