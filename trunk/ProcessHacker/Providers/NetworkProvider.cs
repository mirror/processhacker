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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

namespace ProcessHacker
{
    public class NetworkProvider : Provider<string, Win32.NetworkConnection>
    {
        public NetworkProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        private void UpdateOnce()
        {
            var networkDict = Win32.GetNetworkConnections();
            var preKeyDict = new Dictionary<string, KeyValuePair<int, Win32.NetworkConnection>>();
            var keyDict = new Dictionary<string, Win32.NetworkConnection>();
            var newDict = new Dictionary<string, Win32.NetworkConnection>(this.Dictionary);

            // flattens list, assigns IDs and counts
            foreach (var list in networkDict.Values)
            {
                foreach (var connection in list)
                {
                    string id = connection.PID.ToString() + "-" + connection.Local.ToString() + "-" +
                        (connection.Remote != null ? connection.Remote.ToString() : "") + "-" + connection.State.ToString() + "-" +
                        connection.Protocol.ToString();

                    if (preKeyDict.ContainsKey(id))
                        preKeyDict[id] = new KeyValuePair<int, Win32.NetworkConnection>(
                            preKeyDict[id].Key + 1, preKeyDict[id].Value);
                    else
                        preKeyDict.Add(id, new KeyValuePair<int, Win32.NetworkConnection>(1, connection));
                }
            }

            // merges counts into IDs
            foreach (string s in preKeyDict.Keys)
            {
                var connection = preKeyDict[s].Value;

                connection.ID = s + "-" + preKeyDict[s].Key.ToString();
                keyDict.Add(s + "-" + preKeyDict[s].Key.ToString(), connection);
            }

            foreach (var connection in Dictionary.Values)
            {
                if (!keyDict.ContainsKey(connection.ID))
                {
                    CallDictionaryRemoved(connection);   
                    newDict.Remove(connection.ID);
                }
            }

            foreach (var connection in keyDict.Values)
            {
                if (!Dictionary.ContainsKey(connection.ID))
                {
                    newDict.Add(connection.ID, connection);
                    CallDictionaryAdded(connection);

                    // resolve the IP addresses

                    if (connection.Local.Address.ToString() != "0.0.0.0")
                    {
                        Dns.BeginGetHostEntry(connection.Local.Address, result =>
                            {
                                string id = (string)result.AsyncState;

                                if (Dictionary.ContainsKey(id))
                                {
                                    lock (Dictionary)
                                    {
                                        try
                                        {
                                            var dnsResult = Dns.EndGetHostEntry(result);
                                            var modConnection = Dictionary[id];

                                            modConnection.LocalString = dnsResult.HostName;
                                            CallDictionaryModified(Dictionary[id], modConnection);
                                            Dictionary[id] = modConnection;
                                        }
                                        catch { }
                                    }
                                }
                            }, connection.ID);
                    }

                    if (connection.Remote != null && connection.Remote.Address.ToString() != "0.0.0.0")
                    {
                        Dns.BeginGetHostEntry(connection.Remote.Address, result =>
                        {
                            string id = (string)result.AsyncState;

                            if (Dictionary.ContainsKey(id))
                            {
                                lock (Dictionary)
                                {
                                    try
                                    {
                                        var dnsResult = Dns.EndGetHostEntry(result);
                                        var modConnection = Dictionary[id];

                                        modConnection.RemoteString = dnsResult.HostName;
                                        CallDictionaryModified(Dictionary[id], modConnection);
                                        Dictionary[id] = modConnection;
                                    }
                                    catch
                                    { }
                                }
                            }
                        }, connection.ID);
                    }
                }
                else
                {
                    if (connection.State != Dictionary[connection.ID].State)
                    {
                        lock (Dictionary)
                        {
                            var newConnection = Dictionary[connection.ID];

                            newConnection.State = connection.State;

                            CallDictionaryModified(Dictionary[connection.ID], newConnection);
                            Dictionary[connection.ID] = connection;
                        }
                    }
                }
            }

            Dictionary = newDict;
        }
    }
}
