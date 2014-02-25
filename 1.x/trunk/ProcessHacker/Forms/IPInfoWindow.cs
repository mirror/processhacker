/*
 * Process Hacker - 
 *   IP information window
 * 
 * Copyright (C) 2009 dmex
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public enum IpAction : int
    {
        Whois = 0,
        Ping = 1,
        Tracert = 2
    }

    public partial class IPInfoWindow : Form
    {
        private IPAddress _ipAddress;
        private IpAction _ipAction;

        public IPInfoWindow(IPAddress ipAddress, IpAction action)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _ipAddress = ipAddress;
            _ipAction = action;

            listInfo.AddShortcuts();
            listInfo.ContextMenu = listInfo.GetCopyMenu();
            listInfo.SetTheme("explorer");
            listInfo.SetDoubleBuffered(true);
        }

        private void IPInfoWindow_Load(object sender, EventArgs e)
        {
            Thread t = null;

            if (_ipAction == IpAction.Whois)
            {
                t = new Thread(new ParameterizedThreadStart(Whois), Utils.SixteenthStackSize);
                labelInfo.Text = "Whois host infomation for address: " + _ipAddress.ToString();
                labelStatus.Text = "Checking...";
                listInfo.Columns.Add("Results");
                ColumnSettings.LoadSettings(Settings.Instance.IPInfoWhoIsListViewColumns, listInfo);
            }
            else if (_ipAction == IpAction.Tracert)
            {
                t = new Thread(new ParameterizedThreadStart(Tracert), Utils.SixteenthStackSize);
                labelStatus.Text = "Tracing route...";
                listInfo.Columns.Add("Count");
                listInfo.Columns.Add("Reply Time");
                listInfo.Columns.Add("IP Address");
                listInfo.Columns.Add("Hostname");
                ColumnSettings.LoadSettings(Settings.Instance.IPInfoTracertListViewColumns, listInfo);
            }
            else if (_ipAction == IpAction.Ping)
            {
                t = new Thread(new ParameterizedThreadStart(Ping), Utils.SixteenthStackSize);
                labelStatus.Text = "Pinging...";
                listInfo.Columns.Add("Results");
                ColumnSettings.LoadSettings(Settings.Instance.IPInfoPingListViewColumns, listInfo);
            }

            t.IsBackground = true;
            t.Start(_ipAddress);
        }

        private void IPInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_ipAction == IpAction.Whois)
                Settings.Instance.IPInfoWhoIsListViewColumns = ColumnSettings.SaveSettings(listInfo);
            else if (_ipAction == IpAction.Tracert)
                Settings.Instance.IPInfoTracertListViewColumns = ColumnSettings.SaveSettings(listInfo);
            else if (_ipAction == IpAction.Ping)
                Settings.Instance.IPInfoPingListViewColumns = ColumnSettings.SaveSettings(listInfo);

            Settings.Instance.Save();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Ping(object ip)
        {
            using (Ping pingSender = new Ping()) 
            {
                PingOptions pingOptions = new PingOptions();
                PingReply pingReply = null;

                IPAddress ipAddress = (IPAddress)ip;
                int numberOfPings = 4;
                int pingTimeout = 1000;
                int byteSize = 32;
                byte[] buffer = new byte[byteSize];
                int sentPings = 0;
                int receivedPings = 0;
                int lostPings = 0;
                long minPingResponse = 0;
                long maxPingResponse = 0;

                //pingOptions.DontFragment = true;
                //pingOptions.Ttl = 128;

                WriteStatus(string.Format("Pinging {0} with {1} bytes of data:", ipAddress, byteSize) + Environment.NewLine, true);

                for (int i = 0; i < numberOfPings; i++)
                {
                    sentPings++;

                    try
                    {
                        pingReply = pingSender.Send(ipAddress, pingTimeout, buffer, pingOptions);
                    }
                    catch (Exception ex)
                    {
                        WriteStatus("Ping error: " + ex.Message, false);
                        break;
                    }

                    if (pingReply.Status == IPStatus.Success)
                    {
                        if (pingReply.Options != null) //IPv6 ping causes pingReply.Options to become null 
                        {
                            WriteResult(string.Format("Reply from {0}:  bytes={1}  time={2}ms  TTL={3}", ipAddress, byteSize, pingReply.RoundtripTime, pingReply.Options.Ttl), "", "");
                        }
                        else
                        {
                            WriteResult(string.Format("Reply from {0}:  bytes={1}  time={2}ms  TTL={3}", ipAddress, byteSize, pingReply.RoundtripTime, pingOptions.Ttl), "", "");
                        }

                        if (minPingResponse == 0)
                        {
                            minPingResponse = pingReply.RoundtripTime;
                            maxPingResponse = minPingResponse;
                        }
                        else if (pingReply.RoundtripTime < minPingResponse)
                        {
                            minPingResponse = pingReply.RoundtripTime;
                        }
                        else if (pingReply.RoundtripTime > maxPingResponse)
                        {
                            maxPingResponse = pingReply.RoundtripTime;
                        }

                        receivedPings++;
                    }
                    else
                    {
                        WriteResult(pingReply.Status.ToString(), "", "");
                        lostPings++;
                    }
                }
                WriteResult("", "", "");
                WriteResult(string.Format("Ping statistics for {0}:", ipAddress), "", "");
                WriteResult(string.Format("        Packets: Sent = {0}, Received = {1}, Lost = {2}", sentPings, receivedPings, lostPings), "", "");
                WriteResult("Approximate round trip times in milli-seconds:", "", "");
                WriteResult(string.Format("        Minimum = {0}ms, Maximum = {1}ms", minPingResponse, maxPingResponse), "", "");
            }
            WriteStatus("Ping complete.", false);
        }
         
        private void Tracert(object ip)
        {
            IPAddress ipAddress = (IPAddress)ip;

            using (Ping pingSender = new Ping())
            {
                PingOptions pingOptions = new PingOptions();
                Stopwatch stopWatch = new Stopwatch();
                byte[] bytes = new byte[32];

                pingOptions.DontFragment = true;
                pingOptions.Ttl = 1;
                int maxHops = 30;

                WriteStatus(string.Format("Tracing route to {0} over a maximum of {1} hops:", ipAddress, maxHops), true);

                for (int i = 1; i < maxHops + 1; i++)
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    PingReply pingReply;

                    try
                    {
                        pingReply = pingSender.Send(ipAddress, 5000, new byte[32], pingOptions);
                    }
                    catch (Exception ex)
                    {
                        WriteStatus("Trace error: " + ex.Message, false);
                        break;
                    }
                    finally
                    {
                        stopWatch.Stop();
                    }

                    WriteResult(string.Format("{0}" , i), string.Format("{0} ms",  stopWatch.ElapsedMilliseconds), string.Format("{0}", pingReply.Address));

                    WorkQueue.GlobalQueueWorkItemTag(new Action<IPAddress, int>((address, hopNumber) =>
                        {
                            string hostName;

                            try
                            {
                                hostName = Dns.GetHostEntry(address).HostName;
                            }
                            catch
                            {
                                hostName = "";
                            }

                            if (this.IsHandleCreated)
                            {
                                this.BeginInvoke(new MethodInvoker(() =>
                                    {
                                        foreach (ListViewItem item in listInfo.Items)
                                        {
                                            if (item.Text == hopNumber.ToString())
                                            {
                                                item.SubItems[3].Text = hostName;
                                                break;
                                            }
                                        }
                                    }));
                            }
                        }), "ipinfowindow-resolveaddress", pingReply.Address, i);

                    if (pingReply.Status == IPStatus.Success)
                    {
                        WriteStatus("Trace complete.", false);
                        break;
                    }

                    pingOptions.Ttl++;
                }
            }
            WriteStatus("Trace complete.", false);
        }

        private void Whois(object ip)
        {
            try
            {
                using (TcpClient tcpClinetWhois = new TcpClient("wq.apnic.net", 43))
                using (NetworkStream networkStreamWhois = tcpClinetWhois.GetStream())
                using (BufferedStream bufferedStreamWhois = new BufferedStream(networkStreamWhois))
                using (StreamWriter streamWriter = new StreamWriter(bufferedStreamWhois))
                {
                    streamWriter.WriteLine(((IPAddress)ip).ToString());
                    streamWriter.Flush();

                    StreamReader streamReaderReceive = new StreamReader(bufferedStreamWhois);

                    while (!streamReaderReceive.EndOfStream)
                    {
                        string data = streamReaderReceive.ReadLine();
                        if (!data.Contains("#") | !data.Contains("?"))
                        {
                            WriteResult(data, "", "");
                        }
                    }
                }
                WriteStatus("Whois complete.", false);
            }
            catch (Exception ex)
            {
                WriteStatus("Whois error: " + ex.Message, false);
            }
        }

        private void WriteStatus(string info, bool title)
        {
            if (!this.IsHandleCreated)
                return;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => WriteStatus(info, title)));
                return;
            }

            if (title)
            {
                labelInfo.Text = info;
            }
            else
            {
                labelStatus.Text = info;
            }
        }

        private void WriteResult(string hop, string time, string ip)
        {
            if (!this.IsHandleCreated)
                return;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => WriteResult(hop, time, ip)));
                return;
            }

            ListViewItem litem = new ListViewItem(hop);
            litem.SubItems.Add(time);
            litem.SubItems.Add(ip);
            litem.SubItems.Add("");

            listInfo.Items.Add(litem);
        }

    }
}
