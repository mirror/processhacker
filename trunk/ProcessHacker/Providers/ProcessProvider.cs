using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ProcessHacker
{
    public struct ProcessItem
    {
        public int PID;
        public Process Process;

        public Icon Icon;
        public string Name;
        public string MemoryUsage;
        public string Username;
    }

    public class ProcessProvider : Provider<int, ProcessItem>
    {
        public ProcessProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        private void UpdateOnce()
        {
            Process[] processes = Process.GetProcesses();
            List<int> pids = new List<int>();
            Dictionary<int, ProcessItem> newdictionary = new Dictionary<int,ProcessItem>();

            foreach (int key in Dictionary.Keys)
                newdictionary.Add(key, Dictionary[key]);

            foreach (Process p in processes)
                pids.Add(p.Id);

            // look for dead processes
            foreach (int pid in Dictionary.Keys)
            {
                if (!pids.Contains(pid))
                {
                    ProcessItem item = new ProcessItem();

                    item.PID = pid;

                    newdictionary.Remove(pid);
                    this.CallDictionaryRemoved(item);
                }
            }

            // look for new processes
            foreach (Process p in processes)
            {
                if (!Dictionary.ContainsKey(p.Id))
                {
                    ProcessItem item = new ProcessItem();

                    item.PID = p.Id;
                    item.Process = p;

                    item.Icon = Win32.GetProcessIcon(p);

                    try
                    {
                        item.Name = p.MainModule.ModuleName;
                    }
                    catch
                    {
                        item.Name = Win32.GetNameFromPID(p.Id);

                        if (item.Name == "(error)" || item.Name == "(unknown)")
                        {
                            try
                            {
                                item.Name = "(" + p.ProcessName + ")";
                            }
                            catch
                            {
                                item.Name = "(unknown)";
                            }
                        }
                    }

                    try
                    {
                        item.MemoryUsage = Misc.GetNiceSizeName(p.PrivateMemorySize64);
                    }
                    catch
                    { }

                    // doesn't work on different thread for some reason.
                    Program.HackerWindow.Invoke(new MethodInvoker(delegate
                    {
                        try
                        {
                            item.Username = Win32.GetProcessUsername(p.Handle.ToInt32(),
                                Properties.Settings.Default.ShowProcessDomains);
                        }
                        catch
                        { }
                    }));

                    newdictionary.Add(p.Id, item);
                    this.CallDictionaryAdded(item);
                }
                // look for modified processes
                else
                {
                    ProcessItem item = Dictionary[p.Id];
                    ProcessItem newitem = new ProcessItem();

                    newitem.PID = item.PID;
                    newitem.Process = item.Process;

                    try
                    {
                        newitem.MemoryUsage = Misc.GetNiceSizeName(p.PrivateMemorySize64);
                    }
                    catch
                    { }

                    Program.HackerWindow.Invoke(new MethodInvoker(delegate
                    {
                        try
                        {
                            newitem.Username = Win32.GetProcessUsername(p.Handle.ToInt32(),
                                Properties.Settings.Default.ShowProcessDomains);
                        }
                        catch
                        { }
                    }));

                    if (newitem.MemoryUsage != item.MemoryUsage ||
                        newitem.Username != item.Username)
                    {
                        newdictionary[p.Id] = newitem;
                        this.CallDictionaryModified(newitem);
                    }
                }
            }

            Dictionary = newdictionary;
        }
    }
}
