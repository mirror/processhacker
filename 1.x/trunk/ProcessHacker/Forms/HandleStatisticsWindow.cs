using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class HandleStatisticsWindow : Form
    {
        private int _pid;

        public HandleStatisticsWindow(int pid)
        {
            InitializeComponent();

            this.AddEscapeToClose();
            this.SetTopMost();

            _pid = pid;

            listTypes.AddShortcuts();
            listTypes.ContextMenu = listTypes.GetCopyMenu();
            listTypes.ListViewItemSorter = new SortedListViewComparer(listTypes);

            var typeStats = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            using (ProcessHandle phandle = new ProcessHandle(pid, ProcessAccess.DupHandle))
            {
                if (phandle.LastError == null)
                {
                    SystemHandleEntry[] handles = Windows.GetHandles();

                    foreach (var handle in handles)
                    {
                        if (pid != -1 && handle.ProcessId != pid)
                            continue;

                        ObjectInformation info;

                        try
                        {
                            if (pid != -1)
                            {
                                info = handle.GetHandleInfo(phandle, false);
                            }
                            else
                            {
                                info = handle.GetHandleInfo(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(ex);

                            info = new ObjectInformation
                            {
                                TypeName = "(unknown)"
                            };
                        }

                        if (typeStats.ContainsKey(info.TypeName))
                            typeStats[info.TypeName]++;
                        else
                            typeStats.Add(info.TypeName, 1);
                    }
                }
            }

            foreach (var pair in typeStats)
            {
                listTypes.Items.Add(new ListViewItem(new string[]
                {
                    pair.Key,
                    pair.Value.ToString("N0")
                }));
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
