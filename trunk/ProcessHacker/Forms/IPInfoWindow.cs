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
        private string ipAddress;
        private IpAction ipAction;

        public IPInfoWindow(string IPAddress, IpAction action)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            ipAddress = IPAddress;
            ipAction = action;

            this.Text += " (" + IPAddress + ")";
        }

        private void IPInfoWindow_Load(object sender, EventArgs e)
        {
            Thread t = null;

            if (ipAction == IpAction.Whois)
            {
                t = new Thread(new ParameterizedThreadStart(Whois)); //Whois(ipAddress);    
            }
            else if (ipAction == IpAction.Tracert)
            {
                t = new Thread(new ParameterizedThreadStart(Tracert)); // Tracert(ipAddress);
            }
            else if (ipAction == IpAction.Ping)
            {
                t = new Thread(new ParameterizedThreadStart(Ping)); // Ping(ipAddress);
            }

            t.IsBackground = true;
            t.Start(ipAddress);
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

                IPAddress ipAddress = IPAddress.Parse(ip.ToString());
                int numberOfPings = 4;
                int pingTimeout = 1000;
                int byteSize = 32;
                byte[] buffer = new byte[byteSize];
                int sentPings = 0;
                int receivedPings = 0;
                int lostPings = 0;
                long minPingResponse = 0;
                long maxPingResponse = 0;

                pingOptions.DontFragment = true;
                //pingOptions.Ttl = 128;

                WriteResult(string.Format("Pinging {0} with {1} bytes of data:", ipAddress, byteSize) + Environment.NewLine);

                for (int i = 0; i < numberOfPings; i++)
                {
                    sentPings++;

                    pingReply = pingSender.Send(ipAddress, pingTimeout, buffer, pingOptions);

                    if (pingReply.Status == IPStatus.Success)
                    {
                        WriteResult(Environment.NewLine + string.Format("Reply from {0}: bytes={1} time={2}ms TTL={3}", ipAddress, byteSize, pingReply.RoundtripTime, pingReply.Options.Ttl));

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
                        WriteResult(Environment.NewLine + pingReply.Status.ToString());
                        lostPings++;
                    }
                }

                WriteResult(Environment.NewLine + Environment.NewLine + string.Format("Ping statistics for {0}:", ipAddress));
                WriteResult(Environment.NewLine + string.Format("\tPackets: Sent = {0}, Received = {1}, Lost = {2}", sentPings, receivedPings, lostPings));
                WriteResult(Environment.NewLine + "Approximate round trip times in milli-seconds:");
                WriteResult(Environment.NewLine + string.Format("\tMinimum = {0}ms, Maximum = {1}ms", minPingResponse, maxPingResponse));
            }
        }

        private void Tracert(object ip)
        {
            IPAddress ipAddress = IPAddress.Parse(ip.ToString());

            using (Ping pingSender = new Ping())
            {
                PingOptions pingOptions = new PingOptions();
                Stopwatch stopWatch = new Stopwatch();
                byte[] bytes = new byte[32];

                pingOptions.DontFragment = true;
                pingOptions.Ttl = 1;
                int maxHops = 30;

                WriteResult(string.Format(
                    "Tracing route to {0} over a maximum of {1} hops:",
                    ipAddress,
                    maxHops) + Environment.NewLine + Environment.NewLine);

                for (int i = 1; i < maxHops + 1; i++)
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    PingReply pingReply = pingSender.Send(ipAddress, 5000, new byte[32], pingOptions);

                    stopWatch.Stop();

                    WriteResult(string.Format(
                        "{0}\t{1} ms\t{2}",
                        i,
                        stopWatch.ElapsedMilliseconds,
                        pingReply.Address) + Environment.NewLine); //Dns.GetHostEntry(pingReply.Address).HostName)

                    if (pingReply.Status == IPStatus.Success)
                    {
                        WriteResult(Environment.NewLine + "Trace complete.");
                        break;
                    }
                    pingOptions.Ttl++;
                }
            }
        }

        private void Whois(object ip)
        {
            using (TcpClient tcpClinetWhois = new TcpClient("wq.apnic.net", 43))
            using (NetworkStream networkStreamWhois = tcpClinetWhois.GetStream())
            using (BufferedStream bufferedStreamWhois = new BufferedStream(networkStreamWhois))
            using (StreamWriter streamWriter = new StreamWriter(bufferedStreamWhois))
            {
                streamWriter.WriteLine(ip);
                streamWriter.Flush();

                StreamReader streamReaderReceive = new StreamReader(bufferedStreamWhois);

                while (!streamReaderReceive.EndOfStream)
                {
                    WriteResult(Environment.NewLine + streamReaderReceive.ReadLine());
                }
            }
        }

        private void WriteResult(string info)
        {
            if (!this.IsHandleCreated)
                return;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => WriteResult(info)));
                return;
            }

            textInfo.AppendText(info);
        }
    }
}
