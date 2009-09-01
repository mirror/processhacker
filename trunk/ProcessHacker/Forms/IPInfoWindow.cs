using System;
using System.Windows.Forms;
using ProcessHacker.Common;

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
        }

        private void IPInfoWindow_Load(object sender, EventArgs e)
        {
            try
            {
                if (ipAction == IpAction.Whois)
                {
                    System.Text.StringBuilder stringBuilderResult = new System.Text.StringBuilder();
                    System.Net.Sockets.TcpClient tcpClinetWhois = new System.Net.Sockets.TcpClient("wq.apnic.net", 43);
                    System.Net.Sockets.NetworkStream networkStreamWhois = tcpClinetWhois.GetStream();
                    System.IO.BufferedStream bufferedStreamWhois = new System.IO.BufferedStream(networkStreamWhois);
                    System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(bufferedStreamWhois);

                    if (IsNumber(ipAddress))
                    {
                        streamWriter.WriteLine(ipAddress);
                        streamWriter.Flush();

                        System.IO.StreamReader streamReaderReceive = new System.IO.StreamReader(bufferedStreamWhois);

                        while (!streamReaderReceive.EndOfStream)
                        {
                            stringBuilderResult.AppendLine(streamReaderReceive.ReadLine());
                        }

                        //stringBuilderResult.Remove(0, 77); //cleanup return string, remove first line
                        //stringBuilderResult.Remove(stringBuilderResult.Length - 68, 68); //remove last line

                        this.textInfo.AppendText(stringBuilderResult.ToString());
                    }
                    else // value is not a number
                    {
                        PhUtils.ShowError("Unable to get Whois information for the IP address " + ipAddress);
                        this.Close();
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to query IP address information", ex);
                this.Close();
            }
        }

        private bool IsNumber(string text)
        {
            foreach (char c in text)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
