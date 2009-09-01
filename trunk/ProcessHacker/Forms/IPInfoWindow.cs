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

            this.Text += " (" + IPAddress + ")";
        }

        private void IPInfoWindow_Load(object sender, EventArgs e)
        { 
            if (ipAction == IpAction.Whois)
            {  
                System.Text.StringBuilder stringBuilderResult = new System.Text.StringBuilder();

                using (System.Net.Sockets.TcpClient tcpClinetWhois = new System.Net.Sockets.TcpClient("wq.apnic.net", 43))
                using (System.Net.Sockets.NetworkStream networkStreamWhois = tcpClinetWhois.GetStream())
                using (System.IO.BufferedStream bufferedStreamWhois = new System.IO.BufferedStream(networkStreamWhois))
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(bufferedStreamWhois)) 
                {   
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
                        //stringBuilderResult.Remove(stringBuilderResult.Length - 69, 69); //remove last line
            
                        this.textInfo.AppendText(stringBuilderResult.ToString());
                    }  
                    else // value is not a number
                    {  
                        PhUtils.ShowError("Unable to get Whois information for the IP address " + ipAddress);
                        this.Close();
                    }       
                }  
            }  
            else 
            {  
                throw new NotSupportedException();
            }
        }

        private bool IsNumber(string text)
        {
            System.Text.RegularExpressions.Regex objNotWholePattern = new System.Text.RegularExpressions.Regex("[^0-9]");
            return objNotWholePattern.IsMatch(text) && (text != "");  
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
