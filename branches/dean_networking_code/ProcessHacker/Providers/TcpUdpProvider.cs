/*
 * Process Hacker - 
 *   TcpUdp provider
 * 
 * Copyright (C) 2009 Dean
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
using System.Threading;
using System.Windows.Forms;

namespace ProcessHacker
{    
    public struct TcpUdpItem
    {
        public Protocol Protocol;        
        public string LocalAddress;
        public string RemoteAddress;
        public string State;
        public override int GetHashCode()
        {
            string str=((byte)Protocol).ToString()+LocalAddress+RemoteAddress+State;
            return str.GetHashCode();
        }
    }
    public enum State : int
    {
        CLOSED = 1,
        LISTEN,
        SYN_SENT,
        SYN_RCVD,
        ESTAB,
        FIN_WAIT1,
        FIN_WAIT2,
        CLOSE_WAIT,
        CLOSING,
        LAST_ACK,
        TIME_WAIT,
        DELETE_TCB
    }
    public enum Protocol : byte
    {            
        TCP=0,
        UDP
    }
    // should handle it like ThreadProvider,need change ProcessSystemProvider
    // later to do
    public class TcpUdpProvider : Provider<int, TcpUdpItem>
    {
        private int _pid;
        public int PID
        {
            get { return _pid; }
        }

        public TcpUdpProvider(int PID) : base()
        {
            _pid = PID;
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);
        } 
        private void UpdateOnce()
        {
            Win32.MIB_TCPTABLE_OWNER_PID tcpTable= Win32.GetExTcpConnexions();
            Win32.MIB_UDPTABLE_OWNER_PID udpTable = Win32.GetExUdpConnexions();

            Dictionary<int, TcpUdpItem> processTcpUdpDic = new Dictionary<int, TcpUdpItem>();
            Dictionary<int, TcpUdpItem> newdictionary = new Dictionary<int, TcpUdpItem>(this.Dictionary);

            foreach (Win32.MIB_TCPROW_OWNER_PID tcpRow in tcpTable.Table)
            {
                if (tcpRow.OwningProcessId == _pid)
                {
                    TcpUdpItem item=GenerateTcpUdpItem(tcpRow);
                    processTcpUdpDic.Add(item.GetHashCode(), item);
                }
            }
            foreach (Win32.MIB_UDPROW_OWNER_PID udpRow in udpTable.Table)
            {
                if (udpRow.OwningProcessId == _pid)
                {
                    TcpUdpItem item = GenerateTcpUdpItem(udpRow);
                    processTcpUdpDic.Add(item.GetHashCode(), item);
                }
            }

            foreach (int b in Dictionary.Keys)
            {
                if (!processTcpUdpDic.ContainsKey(b))
                {
                    this.CallDictionaryRemoved(this.Dictionary[b]);
                    newdictionary.Remove(b);
                }
            }
            foreach (int b in processTcpUdpDic.Keys)
            {
                if (!Dictionary.ContainsKey(b))
                {  
                    newdictionary.Add(b, processTcpUdpDic[b]);
                    this.CallDictionaryAdded(processTcpUdpDic[b]);
                }
            }
            this.Dictionary = newdictionary;
       
        }

        private TcpUdpItem GenerateTcpUdpItem(Win32.MIB_TCPROW_OWNER_PID tcpRow)
        {
            TcpUdpItem tcpUdpItem = new TcpUdpItem();
            tcpUdpItem.Protocol=Protocol.TCP;
            tcpUdpItem.LocalAddress=tcpRow.Local.Address.ToString()+":"+tcpRow.Local.Port.ToString();
            tcpUdpItem.RemoteAddress=tcpRow.Remote.Address.ToString()+":"+tcpRow.Remote.Port.ToString();
            tcpUdpItem.State=((State)((int)tcpRow.State)).ToString();
            return tcpUdpItem;
        }
        private TcpUdpItem GenerateTcpUdpItem(Win32.MIB_UDPROW_OWNER_PID udpRow)
        {
            TcpUdpItem tcpUdpItem = new TcpUdpItem();
            tcpUdpItem.Protocol = Protocol.UDP;
            tcpUdpItem.LocalAddress = udpRow.Local.Address.ToString() + ":" + udpRow.Local.Port.ToString();
            tcpUdpItem.RemoteAddress = "*:*";
            tcpUdpItem.State = string.Empty;
            return tcpUdpItem;
        }
        
    }
}
