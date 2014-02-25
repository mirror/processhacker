using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native.Api;
using System.Diagnostics;
using System.Timers;
using System.Collections;
using System.Net.NetworkInformation;
using ProcessHacker.Common;

namespace ProcessHacker
{
    public partial class NetInfoWindow : Form
    {
        public NetInfoWindow()
        {
            InitializeComponent();
        }

        private void NetInfoWindow_Load(object sender, EventArgs e)
        {
            this._monitor = new NetworkMonitor();
            this._monitor.StopMonitoring();
            this._monitor.StartMonitoring();
        }

        private ProcessHacker.NetworkMonitor _monitor;

        private void getStats()
        {
            MibTcpStats mtcp = Win32.GetTcpStats();
            label1.Text = String.Format("ActiveOpens: {0}", mtcp.ActiveOpens);
            label2.Text = String.Format("AttemptFails: {0}", mtcp.AttemptFails);
            label3.Text = String.Format("CurrEstab: {0}", mtcp.CurrEstab);
            label4.Text = String.Format("EstabResets: {0}", mtcp.EstabResets);
            label5.Text = String.Format("InErrs: {0}", mtcp.InErrs);
            label6.Text = String.Format("InSegs: {0}", mtcp.InSegs);
            label7.Text = String.Format("MaxConn: {0}", mtcp.MaxConn);
            label8.Text = String.Format("NumConns: {0}", mtcp.NumConns);
            label9.Text = String.Format("OutRsts: {0}", mtcp.OutRsts);
            label10.Text = String.Format("OutSegs: {0}", mtcp.OutSegs);
            label11.Text = String.Format("PassiveOpens: {0}", mtcp.PassiveOpens);
            label12.Text = String.Format("RetransSegs: {0}", mtcp.RetransSegs);
            label13.Text = String.Format("RtoAlgorithm: {0}", mtcp.RtoAlgorithm);
            label14.Text = String.Format("RtoMax: {0}", mtcp.RtoMax);
            label15.Text = String.Format("RtoMin: {0}", mtcp.RtoMin);

            MibUdpStats mudp = Win32.GetUdpStats();
            label16.Text = String.Format("InDatagrams: {0}", mudp.InDatagrams);
            label17.Text = String.Format("InErrors: {0}", mudp.InErrors);
            label18.Text = String.Format("NoPorts: {0}", mudp.NoPorts);
            label19.Text = String.Format("NumAddrs: {0}", mudp.NumAddrs);
            label20.Text = String.Format("OutDatagrams: {0}", mudp.OutDatagrams);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getStats();

            foreach (NetworkAdapter i in _monitor.Adapters)
            {
                try
                {
                    int down = unchecked((int)Convert.ToInt32(Math.Round(i.DownloadSpeedKbps, 0)));
                    int up = unchecked((int)Convert.ToInt32(Math.Round(i.DownloadSpeedKbps, 0)));

                    this.label25.Text = up.ToString();
                    this.label26.Text = down.ToString();

                    this.label21.Text = String.Format("U: {0:n}kbps", i.UploadSpeedKbps);
                    this.label22.Text = String.Format("D: {0:n}kbps", i.DownloadSpeedKbps);

                    NetworkInformation nic = new NetworkInformation();

                    this.label23.Text = "TSen: " + nic.BytesSent(0).ToString();
                    this.label24.Text = "TRec: " + nic.BytesReceived(0).ToString();

                }
                catch (Exception)
                {
                }
            }
        }
    }

/// <summary>
/// Represents a network adapter installed on the machine.
/// Properties of this class can be used to obtain current network speed.
/// </summary>
public class NetworkAdapter
{

    //http://www.dotnet247.com/247reference/System/Net/NetworkInformation/System.Net.NetworkInformation.aspx
    //MibTcpStats plus others are locatated in NetworkInfomation class

	/// <summary>
	/// Instances of this class are supposed to be created only in an NetworkMonitor.
	/// </summary>
	internal NetworkAdapter(string name)
	{
		this.name	=	name;
	}

	private long dlSpeed, ulSpeed;				// Download\Upload speed in bytes per second.
	private long dlValue, ulValue;				// Download\Upload counter value in bytes.
	private long dlValueOld, ulValueOld;		// Download\Upload counter value one second earlier, in bytes.

	internal string name;								// The name of the adapter.
	internal PerformanceCounter dlCounter, ulCounter;	// Performance counters to monitor download and upload speed.

	/// <summary>
	/// Preparations for monitoring.
	/// </summary>
	internal void init()
	{
		// Since dlValueOld and ulValueOld are used in method refresh() to calculate network speed, they must have be initialized.
		this.dlValueOld	=	this.dlCounter.NextSample().RawValue;
		this.ulValueOld	=	this.ulCounter.NextSample().RawValue;
	}

	/// <summary>
	/// Obtain new sample from performance counters, and refresh the values saved in dlSpeed, ulSpeed, etc.
	/// This method is supposed to be called only in NetworkMonitor, one time every second.
	/// </summary>
	internal void refresh()
	{
		this.dlValue	=	this.dlCounter.NextSample().RawValue;
		this.ulValue	=	this.ulCounter.NextSample().RawValue;
			
		// Calculates download and upload speed.
		this.dlSpeed	=	this.dlValue - this.dlValueOld;
		this.ulSpeed	=	this.ulValue - this.ulValueOld;

		this.dlValueOld	=	this.dlValue;
		this.ulValueOld	=	this.ulValue;
	}

	/// <summary>
	/// Overrides method to return the name of the adapter.
	/// </summary>
	/// <returns>The name of the adapter.</returns>
	public override string ToString()
	{
		return this.name;
	}

	/// <summary>
	/// The name of the network adapter.
	/// </summary>
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	/// <summary>
	/// Current download speed in bytes per second.
	/// </summary>
	public long DownloadSpeed
	{
		get
		{
			return this.dlSpeed;
		}
	}
	
    /// <summary>
	/// Current upload speed in bytes per second.
	/// </summary>
	public long UploadSpeed
	{
		get
		{
			return this.ulSpeed;
		}
	}
	
    /// <summary>
	/// Current download speed in kbytes per second.
	/// </summary>
	public double DownloadSpeedKbps
	{
		get
		{
			return this.dlSpeed/1024.0;
		}
	}

	/// <summary>
	/// Current upload speed in kbytes per second.
	/// </summary>
	public double UploadSpeedKbps
	{
		get
		{
			return this.ulSpeed/1024.0;
		}
	}
}

public class NetworkInformation
{
        private static NetworkInterface[] NIC;

        public NetworkInformation()
        {
            NIC = NetworkInterface.GetAllNetworkInterfaces();
        }

        public string BytesReceived(int index)
        {
            return NIC[index].GetIPv4Statistics().BytesReceived.ToString();
        }

        public string BytesSent(int index)
        {
            return NIC[index].GetIPv4Statistics().BytesSent.ToString();
        }

        public string IncomingPacketsDiscarded(int index)
        {
            return NIC[index].GetIPv4Statistics().IncomingPacketsDiscarded.ToString();
        }

        public string IncomingPacketsWithErrors(int index)
        {
            return NIC[index].GetIPv4Statistics().IncomingPacketsWithErrors.ToString();
        }

        public string Description(int index)
        {
            return NIC[index].Description;
        }

        public string Speed(int index)
        {
            return NIC[index].Speed.ToString();
        }

       
}

/// <summary>
/// The NetworkMonitor class monitors network speed for each network adapter on the computer, using classes for Performance counter in .NET library.
/// </summary>
public class NetworkMonitor
{
    private System.Timers.Timer timer;						// The timer event executes every second to refresh the values in adapters.
    private ArrayList adapters;					// The list of adapters on the computer.
    private ArrayList monitoredAdapters;		// The list of currently monitored adapters.

    /// <summary>
    /// NetworkMonitor
    /// </summary>
    public NetworkMonitor()
    {
        this.adapters = new ArrayList();
        this.monitoredAdapters = new ArrayList();
        this.EnumerateNetworkAdapters();

        this.timer = new System.Timers.Timer(1000);
        this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
    }

    /// <summary>
    /// Enumerates network adapters installed on the computer.
    /// </summary>
    private void EnumerateNetworkAdapters()
    {
        PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");

        foreach (string name in category.GetInstanceNames())
        {
            // This one exists on every computer.
            if (name == "MS TCP Loopback interface")
            { continue; }
            // Create an instance of NetworkAdapter class, and create performance counters for it.
            NetworkAdapter adapter = new NetworkAdapter(name);
            adapter.dlCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
            adapter.ulCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
            this.adapters.Add(adapter);			// Add it to ArrayList adapter
        }
    }

    private void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        foreach (NetworkAdapter adapter in this.monitoredAdapters)
        { adapter.refresh(); }
    }

    /// <summary>
    /// Get instances of NetworkAdapter for installed adapters on this computer.
    /// </summary>
    public NetworkAdapter[] Adapters
    {
        get
        {
            return (NetworkAdapter[])this.adapters.ToArray(typeof(NetworkAdapter));
        }
    }

    /// <summary>
    /// Enable the timer and add all adapters to the monitoredAdapters list, unless the adapters list is empty.
    /// </summary>
    public void StartMonitoring()
    {
        if (this.adapters.Count > 0)
        {
            foreach (NetworkAdapter adapter in this.adapters)
                if (!this.monitoredAdapters.Contains(adapter))
                {
                    this.monitoredAdapters.Add(adapter);
                    adapter.init();
                }

            this.timer.Enabled = true;
        }
    }

    /// <summary>
    /// Enable the timer, and add the specified adapter to the monitoredAdapters list
    /// </summary>
    /// <param name="adapter"></param>
    public void StartMonitoring(NetworkAdapter adapter)
    {
        if (!this.monitoredAdapters.Contains(adapter))
        {
            this.monitoredAdapters.Add(adapter);
            adapter.init();
        }
        this.timer.Enabled = true;
    }

    /// <summary>
    /// Disable the timer, and clear the monitoredAdapters list.
    /// </summary>
    public void StopMonitoring()
    {
        this.monitoredAdapters.Clear();
        this.timer.Enabled = false;
    }

    /// <summary>
    /// Remove the specified adapter from the monitoredAdapters list, and disable the timer if the monitoredAdapters list is empty.
    /// </summary>
    /// <param name="adapter"></param>
    public void StopMonitoring(NetworkAdapter adapter)
    {
        if (this.monitoredAdapters.Contains(adapter))
        { this.monitoredAdapters.Remove(adapter); }
        if (this.monitoredAdapters.Count == 0)
        { this.timer.Enabled = false; }
    }
}
}
