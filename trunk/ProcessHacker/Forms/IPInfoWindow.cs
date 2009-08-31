using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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

            ipAddress = IPAddress;
            ipAction = action;
        }

        private void IPInfoWindow_Load(object sender, EventArgs e)
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

                    this.textBox1.AppendText(stringBuilderResult.ToString());
                }
                else // value is not a number
                {   
                    MessageBox.Show("There was an error obtaining Whois infomation for IP: " + ipAddress);
                    this.Close();
                }
            }
            else
            {


            }
        }

        private bool IsNumber(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[0-9]"); 
            return regex.IsMatch(text);
        }

    }
}
