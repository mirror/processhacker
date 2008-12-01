/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        delegate void QueueUpdatedCallback();
        delegate void AddIconCallback(Icon icon);
        delegate void AddListViewItemCallback(ListView lv, string[] text);

        #region Variables

        public int RefreshInterval = 1000;

        public HelpWindow HelpForm = new HelpWindow();

        ProcessProvider processP = new ProcessProvider();
        ServiceProvider serviceP = new ServiceProvider();
        ThreadProvider threadP;

        Dictionary<int, List<string>> processServices = new Dictionary<int, List<string>>();

        int processSelectedItems;
        int processSelectedPID;
        Process processSelected;

        Process virtualProtectProcess;
        int virtualProtectAddress;
        int virtualProtectSize;

        Process memoryProcess;
        int memoryAddress;
        int memorySize;

        List<Control> listViews = new List<Control>();

        Queue<KeyValuePair<string, Icon>> statusMessages = new Queue<KeyValuePair<string, Icon>>();
        List<string> log = new List<string>();

        #endregion

        #region Properties

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        public ProcessProvider ProcessProvider
        {
            get { return processP; }
        }

        public ServiceProvider ServiceProvider
        {
            get { return serviceP; }
        }

        public ThreadProvider ThreadProvider
        {
            get { return threadP; }
        }

        public ProcessList ProcessList
        {
            get { return listProcesses; }
        }

        #endregion

        #region Events

        #region Buttons

        private void buttonCloseProc_Click(object sender, EventArgs e)
        {
            panelProc.Visible = false;
            this.AcceptButton = null;

            listProcesses.Enabled = true;
            listThreads.Enabled = true;
            tabControl.Enabled = true;
        }

        private void buttonCloseVirtualProtect_Click(object sender, EventArgs e)
        {
            CloseVirtualProtect();
        }

        private void buttonGetProcAddress_Click(object sender, EventArgs e)
        {
            if (listModules.SelectedItems.Count != 1)
                return;

            bool loaded = Win32.GetModuleHandle(listModules.SelectedItems[0].ToolTipText) != 0;
            int module;
            int address = 0;
            int ordinal = 0;

            if (loaded)
                module = Win32.GetModuleHandle(listModules.SelectedItems[0].ToolTipText);
            else
                module = Win32.LoadLibraryEx(listModules.SelectedItems[0].ToolTipText, 0, Win32.DONT_RESOLVE_DLL_REFERENCES);

            if (module == 0)
            {
                textProcAddress.Text = "Could not load library!";
            }

            if ((textProcName.Text.Length > 0) &&
                (textProcName.Text[0] >= '0' && textProcName.Text[0] <= '9'))
                ordinal = (int)BaseConverter.ToNumberParse(textProcName.Text, false);

            if (ordinal != 0)
            {
                address = Win32.GetProcAddress(module, ordinal);
            }
            else
            {
                address = Win32.GetProcAddress(module, textProcName.Text);
            }

            if (address != 0)
            {
                textProcAddress.Text = String.Format("0x{0:x8}", address);
                textProcAddress.SelectAll();
                textProcAddress.Focus();
            }
            else
            {
                textProcAddress.Text = Win32.GetLastErrorMessage();
            }

            // don't unload libraries we had before
            if (module != 0 && !loaded)
                Win32.FreeLibrary(module);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            PerformSearch(buttonSearch.Text);
        }

        private void buttonVirtualProtect_Click(object sender, EventArgs e)
        {
            try
            {
                int old = 0;
                int newprotect;

                try
                {
                    newprotect = (int)BaseConverter.ToNumberParse(textNewProtection.Text);
                }
                catch
                {
                    return;
                }

                if (Win32.VirtualProtectEx(virtualProtectProcess.Handle.ToInt32(), virtualProtectAddress,
                    virtualProtectSize, newprotect, ref old) == 0)
                {
                    MessageBox.Show("There was an error setting memory protection:\n\n" + 
                        Win32.GetLastErrorMessage(), "Process Hacker",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CloseVirtualProtect();

                try
                {
                    listMemory.SelectedItems[0].SubItems[4].Text =
                        ((Win32.MEMORY_PROTECTION)newprotect).ToString().Replace("PAGE_", "");
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error setting memory protection:\n\n" + ex.Message, "Process Hacker",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Lists

        private void listMemory_DoubleClick(object sender, EventArgs e)
        {
            readWriteMemoryMemoryMenuItem_Click(null, null);
        }

        private void listModules_DoubleClick(object sender, EventArgs e)
        {
            goToInMemoryViewModuleMenuItem_Click(null, null);
        }

        private void listProcesses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                terminateMenuItem_Click(null, null);
            }
        }

        private void listProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            processSelectedItems = listProcesses.SelectedItems.Count;

            if (processSelectedItems == 1)
            {
                processSelectedPID = Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text);

                treeMisc.Enabled = true;
                buttonSearch.Enabled = true;

                try
                {
                    try
                    {
                        if (processSelected != null)
                            processSelected.Close();
                    }
                    catch
                    { }

                    processSelected = Process.GetProcessById(processSelectedPID);

                    UpdateProcessExtra();
                }
                catch
                {
                    processSelected = null;

                    listMemory.Enabled = false;
                    listModules.Enabled = false;
                    listThreads.Enabled = false;
                }
            }
            else
            {
                processSelectedPID = -1;

                try
                {
                    if (processSelected != null)
                        processSelected.Close();
                }
                catch
                { }

                processSelected = null;

                listThreads.Items.Clear();
                treeMisc.Enabled = false;
                buttonSearch.Enabled = false;

                UpdateProcessExtra();
            }
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            propertiesServiceMenuItem_Click(null, null);
        }

        private void listThreads_DoubleClick(object sender, EventArgs e)
        {
            inspectThreadMenuItem_Click(null, null);
        }

        #endregion

        #region Main Menu

        private void selectAllHackerMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control c in listViews)
            {
                if (c.Focused)
                {
                    try
                    {
                        Misc.SelectAll((ListView.ListViewItemCollection)c.GetType().GetProperty("Items").GetValue(c, null));
                    }
                    catch
                    { }
                }
            }
        }

        private void getSNFAMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox box = new PromptBox();

            if (box.ShowDialog() == DialogResult.OK)
            {
                int address = (int)BaseConverter.ToNumberParse(box.Value);

                InformationBox infoBox = new InformationBox(Symbols.GetNameFromAddress(address));

                infoBox.ShowDialog();
            }
        }

        private void FSPWSSIDMenuItem_Click(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            int myId = Win32.GetProcessSessionId(Process.GetCurrentProcess().Id);

            DeselectAll(listProcesses.List);

            foreach (Process p in processes)
            {
                try
                {
                    if (Win32.TSGetProcessUsername(p.Id, true) == "NT AUTHORITY\\SYSTEM" &&
                        Win32.GetProcessSessionId(p.Id) == myId)
                    {
                        listProcesses.List.Items[p.Id.ToString()].Selected = true;
                        listProcesses.List.Items[p.Id.ToString()].EnsureVisible();
                    }
                }
                catch
                { }
            }

            tabControlBig.SelectedTab = tabProcesses;
            listProcesses.List.Select();
        }

        private void inspectPEFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PEWindow pw = Program.GetPEWindow(ofd.FileName, new Program.PEWindowInvokeAction(delegate(PEWindow f)
                {
                    try
                    {
                        f.Show();
                    }
                    catch
                    { }
                }));
            }
        }

        private void logMenuItem_Click(object sender, EventArgs e)
        {
            string str = "";

            foreach (string item in log)
                str += item + "\r\n";

            InformationBox box = new InformationBox(str);

            box.TopMost = this.TopMost;
            box.ShowDialog();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();

            about.TopMost = this.TopMost;
            about.ShowDialog();
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            OptionsWindow options = new OptionsWindow();

            options.ShowDialog();

            RefreshInterval = Properties.Settings.Default.RefreshInterval;
            timerFire.Interval = RefreshInterval;
        }

        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm.Show();
            HelpForm.Activate();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Memory Context Menu

        private void menuMemory_Popup(object sender, EventArgs e)
        {
            if (listMemory.SelectedItems.Count == 1 && listProcesses.SelectedItems.Count == 1)
            {
                Misc.EnableAllMenuItems(menuMemory);
            }
            else
            {
                Misc.DisableAllMenuItems(menuMemory);

                if (listProcesses.SelectedItems.Count == 1)
                    readWriteAddressMemoryMenuItem.Enabled = true;

                if (listMemory.SelectedItems.Count > 1)
                {
                    copyMemoryMenuItem.Enabled = true;
                }

                if (listMemory.Items.Count > 0)
                {
                    selectAllMemoryMenuItem.Enabled = true;
                }
                else
                {
                    selectAllMemoryMenuItem.Enabled = false;
                }
            }
        }

        private void changeMemoryProtectionMemoryMenuItem_Click(object sender, EventArgs e)
        {
            virtualProtectProcess = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));

            virtualProtectAddress = Int32.Parse(listMemory.SelectedItems[0].SubItems[0].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);
            virtualProtectSize = Int32.Parse(listMemory.SelectedItems[0].SubItems[1].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);

            ShowVirtualProtect();
        }

        private void readWriteMemoryMemoryMenuItem_Click(object sender, EventArgs e)
        {
            memoryProcess = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
            memoryAddress = Int32.Parse(listMemory.SelectedItems[0].SubItems[0].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);
            memorySize = Int32.Parse(listMemory.SelectedItems[0].SubItems[1].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);

            ReadWriteMemory();
        }

        private void readWriteAddressMemoryMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox prompt = new PromptBox();

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                int address = -1;
                bool found = false;

                try
                {
                    address = (int)BaseConverter.ToNumberParse(prompt.Value);
                }
                catch
                {
                    MessageBox.Show("Invalid address!", "Process Hacker", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                if (address < 0)
                    return;

                memoryProcess = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));

                foreach (ListViewItem item in listMemory.Items)
                {
                    int itemaddress = Int32.Parse(item.SubItems[0].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);

                    if (itemaddress > address)
                    {
                        listMemory.Items[item.Index - 1].Selected = true;
                        listMemory.Items[item.Index - 1].EnsureVisible();
                        memoryAddress = Int32.Parse(listMemory.Items[item.Index - 1].SubItems[0].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);
                        memorySize = Int32.Parse(listMemory.Items[item.Index - 1].SubItems[1].Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber);
                        found = true;

                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Memory address not found!", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MemoryEditor m_e = ReadWriteMemory(true);

                try
                {
                    m_e.BeginInvoke(new MethodInvoker(delegate { m_e.Select(address - memoryAddress, 1); }));
                }
                catch
                { }
            }
        }

        private void selectAllMemoryMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listMemory.Items);
        }

        #endregion

        #region Module Context Menu

        private void menuModule_Popup(object sender, EventArgs e)
        {
            if (listModules.SelectedItems.Count == 1 && listProcesses.SelectedItems.Count == 1)
            {
                if (Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text) == 4)
                {
                    Misc.DisableAllMenuItems(menuModule);

                    inspectModuleMenuItem.Enabled = true;
                    searchModuleMenuItem.Enabled = true;
                    copyFileNameMenuItem.Enabled = true;
                    copyModuleMenuItem.Enabled = true;
                    openContainingFolderMenuItem.Enabled = true;
                    propertiesMenuItem.Enabled = true;
                }
                else
                {
                    Misc.EnableAllMenuItems(menuModule);
                }
            }
            else
            {
                Misc.DisableAllMenuItems(menuModule);

                if (listModules.SelectedItems.Count > 1)
                {
                    copyFileNameMenuItem.Enabled = true;
                    copyModuleMenuItem.Enabled = true;
                }
            }

            if (listModules.Items.Count > 0)
            {
                selectAllModuleMenuItem.Enabled = true;
            }
            else
            {
                selectAllModuleMenuItem.Enabled = false;
            }
        }

        private void searchModuleMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.SearchEngine.Replace("%s",
                    listModules.SelectedItems[0].Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void copyFileNameMenuItem_Click(object sender, EventArgs e)
        {
            string text = "";

            for (int i = 0; i < listModules.SelectedItems.Count; i++)
            {
                text += listModules.SelectedItems[i].ToolTipText;

                if (i != listModules.SelectedItems.Count - 1)
                    text += "\r\n";
            }

            Clipboard.SetText(text);
        }

        private void openContainingFolderMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "/select," + listModules.SelectedItems[0].ToolTipText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void propertiesMenuItem_Click(object sender, EventArgs e)
        {
            Win32.SHELLEXECUTEINFO info = new Win32.SHELLEXECUTEINFO();

            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.SHELLEXECUTEINFO));
            info.lpFile = listModules.SelectedItems[0].ToolTipText;
            info.nShow = Win32.SW_SHOW;
            info.fMask = Win32.SEE_MASK_INVOKEIDLIST;
            info.lpVerb = "properties";

            Win32.ShellExecuteEx(ref info);
        }

        private void inspectModuleMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                PEWindow pw = Program.GetPEWindow(listModules.SelectedItems[0].ToolTipText,
                    new Program.PEWindowInvokeAction(delegate(PEWindow f)
                    {
                        try
                        {
                            f.Show();
                            f.Activate();
                        }
                        catch
                        { }
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inspecting:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void goToInMemoryViewModuleMenuItem_Click(object sender, EventArgs e)
        {
            string address = listModules.SelectedItems[0].SubItems[1].Text;

            foreach (ListViewItem item in listMemory.Items)
            {
                if (item.SubItems[0].Text == address)
                {
                    DeselectAll(listMemory);
                    item.Selected = true;
                    tabControl.SelectedTab = tabMemory;
                    listMemory.EnsureVisible(item.Index);
                    listMemory.Select();
                    listMemory.Focus();

                    break;
                }
            }
        }

        private void getFuncAddressMenuItem_Click(object sender, EventArgs e)
        {
            listProcesses.Enabled = false;
            listThreads.Enabled = false;
            tabControl.Enabled = false;

            panelProc.Visible = true;
            panelProc.BringToFront();
            this.AcceptButton = buttonGetProcAddress;
            textProcName.SelectAll();
            textProcName.Focus();
            textProcAddress.Text = "";
        }

        private void changeMemoryProtectionModuleMenuItem_Click(object sender, EventArgs e)
        {
            virtualProtectProcess = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
            ProcessModule module = null;

            foreach (ProcessModule m in virtualProtectProcess.Modules)
            {
                if (m.FileName == listModules.SelectedItems[0].ToolTipText)
                {
                    module = m;
                    break;
                }
            }

            if (module == null)
                return;

            virtualProtectAddress = module.BaseAddress.ToInt32();
            virtualProtectSize = module.ModuleMemorySize;

            ShowVirtualProtect();
        }

        private void readMemoryModuleMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
            ProcessModule module = null;

            foreach (ProcessModule m in p.Modules)
            {
                if (m.FileName == listModules.SelectedItems[0].ToolTipText)
                {
                    module = m;
                    break;
                }
            }

            if (module == null)
                return;

            memoryProcess = p;
            memoryAddress = module.BaseAddress.ToInt32();
            memorySize = module.ModuleMemorySize;

            ReadWriteMemory(true);
        }

        private void selectAllModuleMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listModules.Items);
        }

        #endregion

        #region Process Context Menu

        private void menuProcess_Popup(object sender, EventArgs e)
        {
            if (listProcesses.SelectedItems.Count == 0)
            {
                Misc.DisableAllMenuItems(menuProcess);
            }
            else
            {
                priorityMenuItem.Text = "&Priority";

                if (listProcesses.SelectedItems.Count == 1)
                {
                    try
                    {
                        int parent = Win32.GetProcessParent(processSelectedPID);
                        ListViewItem item = listProcesses.List.Items[parent.ToString()];

                        goToParentProcessMenuItem.Text = "Go to Parent (" + item.Text + ")";
                        goToParentProcessMenuItem.Enabled = true;
                    }
                    catch
                    {
                        goToParentProcessMenuItem.Text = "Go to Parent";
                        goToParentProcessMenuItem.Enabled = false;
                    }

                    try
                    {
                        List<string> services = processServices[processSelectedPID];

                        if (services == null)
                            throw new Exception();
                        if (services.Count == 0)
                            throw new Exception();

                        servicesProcessMenuItem.Enabled = true;
                    }
                    catch
                    {
                        servicesProcessMenuItem.Enabled = false;
                    }

                    try
                    {
                        if (Win32.GetProcessSessionId(processSelectedPID) ==
                            Win32.GetProcessSessionId(Process.GetCurrentProcess().Id))
                            injectorMenuItem.Enabled = true;
                        else
                            injectorMenuItem.Enabled = false;
                    }
                    catch
                    {
                        injectorMenuItem.Enabled = false;
                    }

                    if (Program.WindowsVersion == "XP")
                        startProcessProcessMenuItem.Visible = false;
                    else
                        startProcessProcessMenuItem.Visible = true;

                    priorityMenuItem.Enabled = true;
                    inspectProcessMenuItem.Enabled = true;
                    searchProcessMenuItem.Enabled = true;
                    privilegesMenuItem.Enabled = true;
                    groupsMenuItem.Enabled = true;
                    terminateMenuItem.Text = "&Terminate Process";
                    closeActiveWindowMenuItem.Text = "&Close Active Window";
                    suspendMenuItem.Text = "&Suspend Process";
                    resumeMenuItem.Text = "&Resume Process";

                    realTimeMenuItem.Checked = false;
                    highMenuItem.Checked = false;
                    aboveNormalMenuItem.Checked = false;
                    normalMenuItem.Checked = false;
                    belowNormalMenuItem.Checked = false;
                    idleMenuItem.Checked = false;

                    try
                    {
                        switch (Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text)).PriorityClass)
                        {
                            case ProcessPriorityClass.RealTime:
                                realTimeMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.High:
                                highMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.AboveNormal:
                                aboveNormalMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.Normal:
                                normalMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.BelowNormal:
                                belowNormalMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.Idle:
                                idleMenuItem.Checked = true;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        priorityMenuItem.Text = "(" + ex.Message + ")";
                        priorityMenuItem.Enabled = false;
                    }
                }
                else
                {
                    goToParentProcessMenuItem.Enabled = false;
                    priorityMenuItem.Enabled = false;
                    inspectProcessMenuItem.Enabled = false;
                    searchProcessMenuItem.Enabled = false;
                    privilegesMenuItem.Enabled = false;
                    groupsMenuItem.Enabled = false;
                    servicesProcessMenuItem.Enabled = false;
                    injectorMenuItem.Enabled = false;
                    terminateMenuItem.Text = "&Terminate Processes";
                    closeActiveWindowMenuItem.Text = "&Close Active Windows";
                    suspendMenuItem.Text = "&Suspend Processes";
                    resumeMenuItem.Text = "&Resume Processes";
                }

                terminateMenuItem.Enabled = true;
                closeActiveWindowMenuItem.Enabled = true;
                suspendMenuItem.Enabled = true;
                resumeMenuItem.Enabled = true;
                copyProcessMenuItem.Enabled = true;
            }

            if (listProcesses.Items.Count == 0)
            {
                selectAllMenuItem.Enabled = false;
            }
            else
            {
                selectAllMenuItem.Enabled = true;
            }
        }

        private void terminateMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to terminate the selected process(es)?", 
                "Process Hacker", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                foreach (ListViewItem item in listProcesses.SelectedItems)
                {
                    try
                    {
                        Process.GetProcessById(Int32.Parse(item.SubItems[1].Text)).Kill();
                    }
                    catch (Exception ex)
                    {
                        DialogResult result = MessageBox.Show("Could not terminate process \"" + item.SubItems[0].Text +
                            "\" with PID " + item.SubItems[1].Text + ":\n\n" +
                                ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                        if (result == DialogResult.Cancel)
                            return;
                    }
                }
            }
        }

        private void suspendMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listProcesses.SelectedItems)
            {
                Process process;

                try
                {
                    process = Process.GetProcessById(Int32.Parse(item.SubItems[1].Text));
                }
                catch { return; }

                if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(process.Id))
                {
                    DialogResult result = MessageBox.Show("The process with PID " + process.Id + " is a system process. Are you" +
                        " sure you want to suspend it?", "Process Hacker", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.No)
                        continue;
                    else if (result == DialogResult.Cancel)
                        return;
                }

                try
                {
                    foreach (ProcessThread thread in process.Threads)
                    {
                        int handle = Win32.OpenThread(Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME, 0, thread.Id);

                        if (handle == 0)
                        {
                            throw new Exception("Could not open thread handle:\n\n" + Win32.GetLastErrorMessage());
                        }

                        if (Win32.SuspendThread(handle) == -1)
                        {
                            Win32.CloseHandle(handle);
                            throw new Exception("Could not suspend thread:\n\n" + Win32.GetLastErrorMessage());
                        }

                        Win32.CloseHandle(handle);
                    }
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not suspend process with PID " + item.SubItems[1].Text +
                        ".\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void resumeMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listProcesses.SelectedItems)
            {
                Process process;

                try
                {
                    process = Process.GetProcessById(Int32.Parse(item.SubItems[1].Text));
                }
                catch { return; }

                try
                {
                    foreach (ProcessThread thread in process.Threads)
                    {
                        int handle = Win32.OpenThread(Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME, 0, thread.Id);

                        if (handle == 0)
                        {
                            throw new Exception("Could not open thread handle:\n\n" + Win32.GetLastErrorMessage());
                        }

                        if (Win32.ResumeThread(handle) == -1)
                        {
                            Win32.CloseHandle(handle);
                            throw new Exception("Could not resume thread:\n\n" + Win32.GetLastErrorMessage());
                        }

                        Win32.CloseHandle(handle);
                    }
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not resume process with PID " + item.SubItems[1].Text +
                        ".\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void closeActiveWindowMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listProcesses.SelectedItems)
            {
                try
                {
                    Process.GetProcessById(Int32.Parse(item.SubItems[1].Text)).Kill();
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not close active window of process \"" + item.SubItems[0].Text +
                        "\" with PID " + item.SubItems[1].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void goToParentProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int parent = Win32.GetProcessParent(processSelectedPID);
                ListViewItem item = listProcesses.List.Items[parent.ToString()];

                DeselectAll(listProcesses.List);
                item.Selected = true;
                item.EnsureVisible();
            }
            catch
            { }
        }

        private void inspectProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path;

                if (processSelectedPID == 4)
                {
                    path = Misc.GetKernelFileName();
                }
                else
                {
                    path = Misc.GetRealPath(processSelected.MainModule.FileName);
                }

                PEWindow pw = Program.GetPEWindow(path,
                    new Program.PEWindowInvokeAction(delegate(PEWindow f)
                    {
                        try
                        {
                            f.Show();
                            f.Activate();
                        }
                        catch
                        { }
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inspecting:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void privilegesMenuItem_Click(object sender, EventArgs e)
        {
            ProcessPrivileges privForm = new ProcessPrivileges(processSelectedPID);

            try
            {
                privForm.TopMost = this.TopMost;
                privForm.ShowDialog();
            }
            catch
            { }
        }

        private void groupsMenuItem_Click(object sender, EventArgs e)
        {
            ProcessGroups grpForm = new ProcessGroups(processSelectedPID);

            try
            {
                grpForm.TopMost = this.TopMost;
                grpForm.ShowDialog();
            }
            catch
            { }
        }

        private void servicesProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceWindow sw = new ServiceWindow(processServices[processSelectedPID].ToArray());

                sw.TopMost = this.TopMost;
                sw.ShowDialog();
            }
            catch
            { }
        }

        #region Injector

        private void startProcessProcessMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox box = new PromptBox();

            box.TextBox.AutoCompleteSource = AutoCompleteSource.FileSystem;
            box.TextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            if (box.ShowDialog() == DialogResult.OK)
            {
                ProcessStartInfo info = new ProcessStartInfo();

                info.FileName = Application.StartupPath + "\\Injector.exe";
                info.Arguments = "createprocessc " + processSelectedPID.ToString() + " \"" + 
                    box.Value.Replace("\"", "\\\"") + "\"";
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                Process p = Process.Start(info);

                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    InformationBox infoBox = new InformationBox(p.StandardOutput.ReadToEnd() + "\r\nReturn code: " + p.ExitCode +
                        " (" + Win32.GetErrorMessage(p.ExitCode) + ")");

                    infoBox.ShowDialog();
                }
            }
        }

        private void getCommandLineProcessMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = Application.StartupPath + "\\Injector.exe";
            info.Arguments = "cmdline " + processSelectedPID.ToString();
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process p = Process.Start(info);

            p.WaitForExit();

            InformationBox infoBox = new InformationBox(p.StandardOutput.ReadToEnd() + (p.ExitCode != 0 ? "\r\nReturn code: " + p.ExitCode + 
                " (" + Win32.GetErrorMessage(p.ExitCode) + ")" : ""));

            infoBox.ShowDialog();
        }

        private void exitProcessProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to terminate the selected process?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;

            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = Application.StartupPath + "\\Injector.exe";
            info.Arguments = "exitprocess " + processSelectedPID.ToString();
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process p = Process.Start(info);

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                InformationBox infoBox = new InformationBox(p.StandardOutput.ReadToEnd() + "\r\nReturn code: " + p.ExitCode +
                    " (" + Win32.GetErrorMessage(p.ExitCode) + ")");

                infoBox.ShowDialog();
            }
        }

        #endregion

        #region Priority

        private void realTimeMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.RealTime);
        }

        private void highMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.High);
        }

        private void aboveNormalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.AboveNormal);
        }

        private void normalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.Normal);
        }

        private void belowNormalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.BelowNormal);
        }

        private void idleMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.Idle);
        }

        #endregion

        private void searchProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.SearchEngine.Replace("%s",
                    listProcesses.SelectedItems[0].Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listProcesses.Items);
        }

        #endregion

        #region Providers

        public void processP_DictionaryAdded(object item)
        {
            ProcessItem pitem = (ProcessItem)item;
            string parentText = "";

            try
            {
                ProcessItem parent = processP.Dictionary[Win32.GetProcessParent(pitem.PID)];

                parentText += " started by " + parent.Name + " (PID " + parent.PID.ToString() + ")";
            }
            catch
            { }

            this.QueueMessage("New Process: " + pitem.Name + " (PID " + pitem.PID.ToString() + ")" + parentText, pitem.Icon);
        }

        public void processP_DictionaryRemoved(object item)
        {
            ProcessItem pitem = (ProcessItem)item;

            this.QueueMessage("Terminated Process: " + pitem.Name + " (PID " + pitem.PID.ToString() + ")", pitem.Icon);

            if (processServices.ContainsKey(pitem.PID))
                processServices.Remove(pitem.PID);
        }

        public void UpdateListViewItemToolTipText(int pid)
        {
            if (pid == 0)
                return;

            ListViewItem litem = listProcesses.Items[pid.ToString()];

            if (litem == null)
                return;

            if (litem.Tag == null)
                litem.Tag = litem.ToolTipText;

            litem.ToolTipText = litem.Tag.ToString();

            if (!processServices.ContainsKey(pid))
            {
                return;
            }
            else
            {
                string servicesText = "";

                foreach (string service in processServices[pid])
                {
                    if (serviceP.Dictionary[service].Status.DisplayName != "")
                        servicesText += service + " (" + serviceP.Dictionary[service].Status.DisplayName + ")\n";
                    else
                        servicesText += service + "\n";
                }

                litem.ToolTipText += "\n\nServices:\n" + servicesText.TrimEnd('\n');
            }
        }

        public void serviceP_DictionaryAdded(object item)
        {
            ServiceItem sitem = (ServiceItem)item;

            this.QueueMessage("New Service: " + sitem.Status.ServiceName +
                " (" + sitem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                ((sitem.Status.DisplayName != "") ?
                " (" + sitem.Status.DisplayName + ")" :
                ""), null);

            if (sitem.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(sitem.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(sitem.Status.ServiceStatusProcess.ProcessID, new List<string>());

                processServices[sitem.Status.ServiceStatusProcess.ProcessID].Add(sitem.Status.ServiceName);
            }

            this.UpdateListViewItemToolTipText(sitem.Status.ServiceStatusProcess.ProcessID);
        }

        public void serviceP_DictionaryModified(object oldItem, object newItem)
        {
            ServiceItem sitem = (ServiceItem)newItem;
            Win32.SERVICE_STATE oldState = ((ServiceItem)oldItem).Status.ServiceStatusProcess.CurrentState;
            Win32.SERVICE_STATE newState = sitem.Status.ServiceStatusProcess.CurrentState;

            if ((oldState == Win32.SERVICE_STATE.Paused || oldState == Win32.SERVICE_STATE.Stopped ||
                oldState == Win32.SERVICE_STATE.StartPending) &&
                newState == Win32.SERVICE_STATE.Running)
                this.QueueMessage("Service Started: " + sitem.Status.ServiceName +
                    " (" + sitem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((sitem.Status.DisplayName != "") ?
                    " (" + sitem.Status.DisplayName + ")" :
                    ""), null);

            if (oldState == Win32.SERVICE_STATE.Running &&
                newState == Win32.SERVICE_STATE.Paused)
                this.QueueMessage("Service Paused: " + sitem.Status.ServiceName +
                    " (" + sitem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((sitem.Status.DisplayName != "") ?
                    " (" + sitem.Status.DisplayName + ")" :
                    ""), null);

            if (oldState == Win32.SERVICE_STATE.Running &&
                newState == Win32.SERVICE_STATE.Stopped)
                this.QueueMessage("Service Stopped: " + sitem.Status.ServiceName +
                    " (" + sitem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((sitem.Status.DisplayName != "") ?
                    " (" + sitem.Status.DisplayName + ")" :
                    ""), null);

            if (sitem.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(sitem.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(sitem.Status.ServiceStatusProcess.ProcessID, new List<string>());

                if (!processServices[sitem.Status.ServiceStatusProcess.ProcessID].Contains(
                    sitem.Status.ServiceName))
                    processServices[sitem.Status.ServiceStatusProcess.ProcessID].Add(sitem.Status.ServiceName);

                processServices[sitem.Status.ServiceStatusProcess.ProcessID].Sort();
                this.UpdateListViewItemToolTipText(sitem.Status.ServiceStatusProcess.ProcessID);
            }
            else
            {
                if (processServices.ContainsKey(sitem.Status.ServiceStatusProcess.ProcessID))
                {
                    if (processServices[sitem.Status.ServiceStatusProcess.ProcessID].Contains(
                        sitem.Status.ServiceName))
                        processServices[sitem.Status.ServiceStatusProcess.ProcessID].Remove(sitem.Status.ServiceName);
                }

                this.UpdateListViewItemToolTipText(((ServiceItem)oldItem).Status.ServiceStatusProcess.ProcessID);
            }
        }

        public void serviceP_DictionaryRemoved(object item)
        {
            ServiceItem sitem = (ServiceItem)item;

            this.QueueMessage("Deleted Service: " + sitem.Status.ServiceName +
                " (" + sitem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                ((sitem.Status.DisplayName != "") ?
                " (" + sitem.Status.DisplayName + ")" :
                ""), null);
            
            if (sitem.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(sitem.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(sitem.Status.ServiceStatusProcess.ProcessID, new List<string>());

                if (!processServices[sitem.Status.ServiceStatusProcess.ProcessID].Contains(
                    sitem.Status.ServiceName))
                    processServices[sitem.Status.ServiceStatusProcess.ProcessID].Add(sitem.Status.ServiceName);

                processServices[sitem.Status.ServiceStatusProcess.ProcessID].Sort();
                this.UpdateListViewItemToolTipText(sitem.Status.ServiceStatusProcess.ProcessID);
            }
        }

        #endregion

        #region Service Context Menu

        private void menuService_Popup(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count == 0)
            {
                Misc.DisableAllMenuItems(menuService);
                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                selectAllServiceMenuItem.Enabled = true;
            }
            else if (listServices.SelectedItems.Count == 1)
            {
                Misc.EnableAllMenuItems(menuService);

                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                try
                {
                    ServiceItem item = serviceP.Dictionary[listServices.SelectedItems[0].Name];

                    if (item.Status.ServiceStatusProcess.ProcessID != 0)
                    {
                        goToProcessServiceMenuItem.Enabled = true;
                    }
                    else
                    {
                        goToProcessServiceMenuItem.Enabled = false;
                    }
                          
                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & Win32.SERVICE_ACCEPT.PauseContinue)
                        == 0)
                    {
                        continueServiceMenuItem.Visible = false;
                        pauseServiceMenuItem.Visible = false;
                    }
                    else
                    {
                        continueServiceMenuItem.Visible = true;
                        pauseServiceMenuItem.Visible = true;
                    }

                    if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Paused)
                    {                                        
                        startServiceMenuItem.Enabled = false;
                        pauseServiceMenuItem.Enabled = false;
                    }
                    else if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Running)
                    {
                        startServiceMenuItem.Enabled = false;
                        continueServiceMenuItem.Enabled = false;
                    }
                    else if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Stopped)
                    {
                        pauseServiceMenuItem.Enabled = false;
                        stopServiceMenuItem.Enabled = false;
                    }

                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & Win32.SERVICE_ACCEPT.Stop) == 0 &&
                        item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Running)
                    {
                        stopServiceMenuItem.Enabled = false;
                    }
                }
                catch
                {
                    Misc.DisableAllMenuItems(menuService);
                    copyServiceMenuItem.Enabled = true;
                    propertiesServiceMenuItem.Enabled = true;
                }
            }
            else
            {
                Misc.DisableAllMenuItems(menuService);

                goToProcessServiceMenuItem.Visible = false;
                startServiceMenuItem.Visible = false;
                continueServiceMenuItem.Visible = false;
                pauseServiceMenuItem.Visible = false;
                stopServiceMenuItem.Visible = false;

                copyServiceMenuItem.Enabled = true;
                propertiesServiceMenuItem.Enabled = true;
                selectAllServiceMenuItem.Enabled = true;
            }

            if (listServices.List.Items.Count == 0)
                selectAllServiceMenuItem.Enabled = false;
        }

        private void goToProcessServiceMenuItem_Click(object sender, EventArgs e)
        {
            DeselectAll(listProcesses.List);

            try
            {
                listProcesses.List.Items[serviceP.Dictionary[
                    listServices.SelectedItems[0].Name].Status.ServiceStatusProcess.ProcessID.ToString()].Selected = true;
                listProcesses.List.Items[serviceP.Dictionary[
                    listServices.SelectedItems[0].Name].Status.ServiceStatusProcess.ProcessID.ToString()].EnsureVisible();

                tabControlBig.SelectedTab = tabProcesses;
            }
            catch
            { }
        }

        private void startServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void continueServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Continue);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error continuing service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pauseServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Pause);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pausing service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the service '" + 
                listServices.SelectedItems[0].Name + "'?", 
                "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                        listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                        service.Delete();

                    listServices.SelectedItems[0].Remove();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting service:\n\n" + ex.Message, "Process Hacker",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void propertiesServiceMenuItem_Click(object sender, EventArgs e)
        {
            List<string> selected = new List<string>();
            ServiceWindow sw;

            foreach (ListViewItem item in listServices.SelectedItems)
                selected.Add(item.Name);

            if (selected.Count == 1)
            {
                sw = new ServiceWindow(selected[0]);
            }
            else
            {
                sw = new ServiceWindow(selected.ToArray());
            }

            sw.TopMost = this.TopMost;
            sw.ShowDialog();
        }

        private void selectAllServiceMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listServices.Items);
        }

        #endregion

        #region Thread Context Menu

        private void menuThread_Popup(object sender, EventArgs e)
        {
            inspectThreadMenuItem.Enabled = false;

            if (listThreads.Items.Count == 0)
            {
                selectAllThreadMenuItem.Enabled = false;
            }
            else
            {
                selectAllThreadMenuItem.Enabled = true;
            }

            if (listProcesses.SelectedItems.Count == 0 || listThreads.SelectedItems.Count == 0)
            {
                terminateThreadMenuItem.Enabled = false;
                suspendThreadMenuItem.Enabled = false;
                resumeThreadMenuItem.Enabled = false;
                priorityThreadMenuItem.Enabled = false;
                copyThreadMenuItem.Enabled = false;

                return;
            }
            else if (listThreads.SelectedItems.Count > 0)
            {
                if (listThreads.SelectedItems.Count == 1)
                {
                    inspectThreadMenuItem.Enabled = true;
                }

                terminateThreadMenuItem.Enabled = true;
                suspendThreadMenuItem.Enabled = true;
                resumeThreadMenuItem.Enabled = true;
                priorityThreadMenuItem.Enabled = true;
                copyThreadMenuItem.Enabled = true;
            }

            priorityThreadMenuItem.Text = "&Priority";

            if (listThreads.SelectedItems.Count == 1)
            {
                timeCriticalThreadMenuItem.Checked = false;
                highestThreadMenuItem.Checked = false;
                aboveNormalThreadMenuItem.Checked = false;
                normalThreadMenuItem.Checked = false;
                belowNormalThreadMenuItem.Checked = false;
                lowestThreadMenuItem.Checked = false;
                idleThreadMenuItem.Checked = false;
                terminateThreadMenuItem.Text = "&Terminate Thread";
                suspendThreadMenuItem.Text = "&Suspend Thread";
                resumeThreadMenuItem.Text = "&Resume Thread";

                try
                {
                    Process p = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
                    ProcessThread thread = null;

                    foreach (ProcessThread t in p.Threads)
                    {
                        if (t.Id.ToString() == listThreads.SelectedItems[0].SubItems[0].Text)
                        {
                            thread = t;
                            break;
                        }
                    }

                    if (thread == null)
                        return;

                    switch (thread.PriorityLevel)
                    {
                        case ThreadPriorityLevel.TimeCritical:
                            timeCriticalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Highest:
                            highestThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.AboveNormal:
                            aboveNormalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Normal:
                            normalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.BelowNormal:
                            belowNormalThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Lowest:
                            lowestThreadMenuItem.Checked = true;
                            break;

                        case ThreadPriorityLevel.Idle:
                            idleThreadMenuItem.Checked = true;
                            break;
                    }

                    priorityThreadMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    priorityThreadMenuItem.Text = "(" + ex.Message + ")";
                    priorityThreadMenuItem.Enabled = false;
                }
            }
            else
            {
                terminateThreadMenuItem.Text = "&Terminate Threads";
                suspendThreadMenuItem.Text = "&Suspend Threads";
                resumeThreadMenuItem.Text = "&Resume Threads";
                priorityThreadMenuItem.Enabled = false;
            }
        }

        private void inspectThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (processSelectedPID == Process.GetCurrentProcess().Id)
            {
                if (MessageBox.Show(
                    "Inspecting Process Hacker's threads will lead to instability. Are you sure you want to continue?",
                    "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                    == DialogResult.No)
                    return;
            }

            if (IsDangerousPID(processSelectedPID))
            {
                if (MessageBox.Show(
                  "Inspecting a system process' threads will lead to instability. Are you sure you want to continue?",
                  "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                  == DialogResult.No)
                    return;
            }

            ThreadWindow window;

            this.UseWaitCursor = true;

            foreach (string s in Symbols.Keys)
            {
                // unload EXE symbols - they usually conflict with the current process
                if (s.ToLower().EndsWith(".exe"))
                    Symbols.UnloadSymbols(s);
            }

            try
            {
                foreach (ProcessModule module in processSelected.Modules)
                {
                    try
                    {
                        statusIcon.Icon = null;
                        statusText.Text = "Loading symbols for " + module.ModuleName + "...";
                        Symbols.LoadSymbolsFromLibrary(module.FileName, module.BaseAddress.ToInt32());
                    }
                    catch (Exception ex)
                    {
                        QueueMessage("Could not load symbols for " + module.ModuleName + ": " + ex.Message, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load symbols for selected process:\n\n" + ex.Message,
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            statusIcon.Icon = null;
            statusText.Text = "";
            this.UseWaitCursor = false;

            try
            {
                window = Program.GetThreadWindow(processSelectedPID,
                    Int32.Parse(listThreads.SelectedItems[0].SubItems[0].Text),
                    new Program.ThreadWindowInvokeAction(delegate(ThreadWindow f)
                {
                    try
                    {
                        f.Show();
                        f.Activate();
                    }
                    catch
                    { }
                }));
            }
            catch
            { }
        }

        private void terminateThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(processSelectedPID))
            {
                DialogResult result = MessageBox.Show("The process with PID " + processSelectedPID + " is a system process. Are you" +
                    " sure you want to terminate the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    int handle = Win32.OpenThread(Win32.THREAD_RIGHTS.THREAD_TERMINATE, 0, Int32.Parse(item.SubItems[0].Text));

                    if (handle == 0)
                    {
                        throw new Exception("Could not open thread:\n\n" + Win32.GetLastErrorMessage());
                    }

                    if (Win32.TerminateThread(handle, 0) == 0)
                    {
                        Win32.CloseHandle(handle);
                        throw new Exception("Could not terminate thread:\n\n" + Win32.GetLastErrorMessage());
                    }

                    Win32.CloseHandle(handle);

                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not terminate thread with ID " + item.SubItems[0].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void suspendThreadMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(processSelectedPID))
            {
                DialogResult result = MessageBox.Show("The process with PID " + processSelectedPID + " is a system process. Are you" +
                    " sure you want to suspend the selected thread(s)?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;
            }

            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    int handle = Win32.OpenThread(Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME, 0, Int32.Parse(item.SubItems[0].Text));

                    if (handle == 0)
                    {
                        throw new Exception("Could not open thread:\n\n" + Win32.GetLastErrorMessage());
                    }

                    if (Win32.SuspendThread(handle) == -1)
                    {
                        Win32.CloseHandle(handle);
                        throw new Exception("Could not suspend thread:\n\n" + Win32.GetLastErrorMessage());
                    }

                    Win32.CloseHandle(handle);
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not suspend thread with ID " + item.SubItems[0].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void resumeThreadMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listThreads.SelectedItems)
            {
                try
                {
                    int handle = Win32.OpenThread(Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME, 0, Int32.Parse(item.SubItems[0].Text));

                    if (handle == 0)
                    {
                        throw new Exception("Could not open thread:\n\n" + Win32.GetLastErrorMessage());
                    }

                    if (Win32.ResumeThread(handle) == -1)
                    {
                        Win32.CloseHandle(handle);
                        throw new Exception("Could not resume thread:\n\n" + Win32.GetLastErrorMessage());
                    }

                    Win32.CloseHandle(handle);
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not resume thread with ID " + item.SubItems[0].Text + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        #region Priority

        private void timeCriticalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.TimeCritical);
        }

        private void highestThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Highest);
        }

        private void aboveNormalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.AboveNormal);
        }

        private void normalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Normal);
        }

        private void belowNormalThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.BelowNormal);
        }

        private void lowestThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Lowest);
        }

        private void idleThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetThreadPriority(ThreadPriorityLevel.Idle);
        }

        #endregion

        private void selectAllThreadMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listThreads.Items);
        }

        #endregion

        #region Timers

        private void timerFire_Tick(object sender, EventArgs e)
        {
            UpdateMiscInfo();
            UpdateStatusInfo();
        }

        private void timerMessages_Tick(object sender, EventArgs e)
        {
            if (statusMessages.Count != 0)
            {
                KeyValuePair<string, Icon> v = statusMessages.Dequeue();
                statusText.Text = v.Key;

                if (v.Value != null)
                    statusIcon.Icon = v.Value;
                else
                    statusIcon.Icon = null;
            }
            else
            {
                statusText.Text = "";
                statusIcon.Icon = null;
            }
        }

        #endregion

        #endregion

        #region Form-related Helper functions

        private void AddIcon(Icon icon)
        {
            imageList.Images.Add(icon);
        }

        private void AddListViewItem(ListView lv, string[] text)
        {
            ListViewItem item = new ListViewItem();

            item.Text = text[0];

            for (int i = 1; i < text.Length; i++)
            {
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, text[i]));
            }
        }

        private void CloseVirtualProtect()
        {
            this.AcceptButton = null;
            virtualProtectProcess = null;
            panelVirtualProtect.Visible = false;
            listProcesses.Enabled = true;
            tabControl.Enabled = true;
            listThreads.Enabled = true;
        }

        private void DeselectAll(ListView list)
        {
            foreach (ListViewItem item in list.SelectedItems)
                item.Selected = false;
        }

        private void LoadSettings()
        {
            RefreshInterval = Properties.Settings.Default.RefreshInterval;
            this.Location = Properties.Settings.Default.WindowLocation;
            this.Size = Properties.Settings.Default.WindowSize;
            this.WindowState = Properties.Settings.Default.WindowState;
            splitMain.SplitterDistance = Properties.Settings.Default.SplitterDistance;
            buttonSearch.Text = Properties.Settings.Default.SearchType;

            if (tabControl.TabPages[Properties.Settings.Default.SelectedTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.SelectedTab];

            ColumnSettings.LoadSettings(Properties.Settings.Default.ProcessListViewColumns, listProcesses.List);
            ColumnSettings.LoadSettings(Properties.Settings.Default.ThreadListViewColumns, listThreads.List);
            ColumnSettings.LoadSettings(Properties.Settings.Default.ModuleListViewColumns, listModules);
            ColumnSettings.LoadSettings(Properties.Settings.Default.MemoryListViewColumns, listMemory);
        }

        private void PerformSearch(string text)
        {
            Point location = this.Location;
            System.Drawing.Size size = this.Size;

            ResultsWindow rw = Program.GetResultsWindow(processSelectedPID, new Program.ResultsWindowInvokeAction(delegate(ResultsWindow f)
            {
                if (text == "&New Results Window...")
                {
                    f.Show();
                }
                else if (text == "&Literal Search...")
                {
                    if (f.EditSearch(SearchType.Literal, location, size) == DialogResult.OK)
                    {
                        f.Show();
                        f.StartSearch();
                    }
                    else
                    {
                        f.Close();
                    }
                }
                else if (text == "&Regex Search...")
                {
                    if (f.EditSearch(SearchType.Regex, location, size) == DialogResult.OK)
                    {
                        f.Show();
                        f.StartSearch();
                    }
                    else
                    {
                        f.Close();
                    }
                }
                else if (text == "&String Scan...")
                {
                    f.SearchOptions.Type = SearchType.String;
                    f.Show();
                    f.StartSearch();
                }
                else if (text == "&Heap Scan...")
                {
                    f.SearchOptions.Type = SearchType.Heap;
                    f.Show();
                    f.StartSearch();
                }
            }));

            buttonSearch.Text = text;
        }

        private void PerformSearch(object sender, EventArgs e)
        {
            PerformSearch(((MenuItem)sender).Text);
        }

        public void QueueMessage(string message)
        {
            this.QueueMessage(message, null);
        }

        public void QueueMessage(string message, Icon icon)
        {
            log.Add(DateTime.Now.ToString() + ": " + message);
            statusMessages.Enqueue(new KeyValuePair<string,Icon>(message, icon));
        }

        private MemoryEditor ReadWriteMemory()
        {
            return ReadWriteMemory(false);
        }

        private MemoryEditor ReadWriteMemory(bool RO)
        {
            try
            {
                MemoryEditor ed = null;

                this.Cursor = Cursors.WaitCursor;

                ed = Program.GetMemoryEditor(processSelectedPID, memoryAddress, memorySize,
                    new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f)
                {
                    try
                    {
                        f.ReadOnly = RO;
                        f.Show();
                        f.Activate();
                    }
                    catch
                    { }
                }));

                this.Cursor = Cursors.Default;

                return ed;
            }
            catch
            {
                this.Cursor = Cursors.Default;

                return null;
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.RefreshInterval = RefreshInterval;

            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
            }

            Properties.Settings.Default.WindowState = this.WindowState == FormWindowState.Minimized ?
                FormWindowState.Normal : this.WindowState;
            Properties.Settings.Default.SplitterDistance = splitMain.SplitterDistance;

            Properties.Settings.Default.SearchType = buttonSearch.Text;

            Properties.Settings.Default.SelectedTab = tabControl.SelectedTab.Name;

            Properties.Settings.Default.ProcessListViewColumns = ColumnSettings.SaveSettings(listProcesses.List);
            Properties.Settings.Default.ThreadListViewColumns = ColumnSettings.SaveSettings(listThreads.List);
            Properties.Settings.Default.ModuleListViewColumns = ColumnSettings.SaveSettings(listModules);
            Properties.Settings.Default.MemoryListViewColumns = ColumnSettings.SaveSettings(listMemory);
            
            try
            {
                Properties.Settings.Default.Save();
            }
            catch
            { }
        }

        private void ShowVirtualProtect()
        {
            panelVirtualProtect.Visible = true;
            panelVirtualProtect.BringToFront();
            textNewProtection.SelectAll();
            textNewProtection.Focus();
            this.AcceptButton = buttonVirtualProtect;
            listProcesses.Enabled = false;
            tabControl.Enabled = false;
            listThreads.Enabled = false;
        }

        private void UpdateStatusInfo()
        {
            statusGeneral.Text = string.Format("{0} processes", processP.Dictionary.Count);
        }

        #endregion

        #region Helper functions

        private bool IsDangerousPID(int pid)
        {
            if (pid == 4)
                return true;

            try
            {
                Process p = Process.GetProcessById(pid);

                foreach (string s in Misc.DangerousNames)
                {
                    if ((Environment.SystemDirectory + "\\" + s).ToLower() == Misc.GetRealPath(p.MainModule.FileName).ToLower())
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        private void SetProcessPriority(ProcessPriorityClass priority)
        {
            try
            {
                Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text)).PriorityClass = priority;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The priority could not be set:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetThreadPriority(ThreadPriorityLevel priority)
        {
            try
            {
                Process p = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
                ProcessThread thread = null;

                foreach (ProcessThread t in p.Threads)
                {
                    if (t.Id.ToString() == listThreads.SelectedItems[0].SubItems[0].Text)
                    {
                        thread = t;
                        break;
                    }
                }

                if (thread == null)
                {
                    MessageBox.Show("Thread not found.", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                thread.PriorityLevel = priority;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The priority could not be set:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Updaters

        #region Misc Decl.

        delegate string MiscInfoDelegate(Process p);

        string[] misctoplevel = { "Process", "DEP", "Handles", "Memory" };

        string[][] miscinfo = {
                                  new string[] { "Session ID", "Priority Boost Enabled", "Total CPU Time",
                                  "Privileged CPU Time", "User CPU Time", "Start Time"},
                                  new string[] { "Status", "Permanent" },
                                  new string[] { "Handle Count" },
                                  new string[] { "Non-paged System Memory Size", "Paged Memory Size", 
                                      "Paged System Memory Size", "Peak Paged Memory Size", "Peak Virtual Memory Size", 
                                      "Peak Working Set", "Private Memory Size", "Virtual Memory Size", "Working Set"}
                              };

        MiscInfoDelegate[][] miscinfofuncs = {
                                  // Process
                                  new MiscInfoDelegate[]
                                  {
                                      delegate (Process p)
                                      {
                                          int id = Win32.GetProcessSessionId(p.Id);

                                          return id == -1 ? "Unknown" : id.ToString();
                                      },

                                      delegate (Process p)
                                      {                    
                                          return p.PriorityBoostEnabled ? "Yes" : "No";
                                      },  

                                      delegate (Process p)
                                      {
                                          return Misc.GetNiceTimeSpan(p.TotalProcessorTime);
                                      },            

                                      delegate (Process p)
                                      {
                                          return Misc.GetNiceTimeSpan(p.PrivilegedProcessorTime);
                                      },    

                                      delegate (Process p)
                                      {
                                          return Misc.GetNiceTimeSpan(p.UserProcessorTime);
                                      },

                                      delegate (Process p)
                                      {
                                          return Misc.GetNiceDateTime(p.StartTime);
                                      }
                                  },

                                  // DEP
                                  new MiscInfoDelegate[]
                                  {
                                      delegate (Process p)
                                      {
                                          Win32.DEPFLAGS flags = 0;
                                          int perm = 0;

                                          Win32.GetProcessDEPPolicy(p.Handle.ToInt32(), ref flags, ref perm);

                                          return flags == Win32.DEPFLAGS.PROCESS_DEP_DISABLE ? "Disabled" :
                                              (flags == Win32.DEPFLAGS.PROCESS_DEP_ENABLE ? "Enabled" :
                                              (flags == (Win32.DEPFLAGS.PROCESS_DEP_ENABLE |
                                              Win32.DEPFLAGS.PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION)) ? 
                                              "Enabled, DEP-ATL thunk emulation disabled" : "Unknown"
                                              );
                                      },

                                      delegate (Process p) 
                                      {     
                                          Win32.DEPFLAGS flags = 0;
                                          int perm = 0;

                                          Win32.GetProcessDEPPolicy(p.Handle.ToInt32(), ref flags, ref perm);

                                          return perm == 0 ? "No" : "Yes";
                                      }
                                  },

                                  // Handles
                                  new MiscInfoDelegate[]
                                  {
                                      delegate (Process p) { return p.HandleCount.ToString(); }   
                                  },

                                  // Memory
                                  new MiscInfoDelegate[]
                                  {
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.NonpagedSystemMemorySize64); },    
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.PagedMemorySize64); },            
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.PagedSystemMemorySize64); },  
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.PeakPagedMemorySize64); },  
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.PeakVirtualMemorySize64); },   
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.PeakWorkingSet64); },  
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.PrivateMemorySize64); },
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.VirtualMemorySize64); },
                                      delegate (Process p) { return Misc.GetNiceSizeName(p.WorkingSet64); }  
                                  }
                              };

        #endregion

        private void InitMiscInfo()
        {
            treeMisc.BeginUpdate();

            TreeNode n;

            for (int i = 0; i < misctoplevel.Length; i++)
            {
                n = treeMisc.Nodes.Add(misctoplevel[i]);

                for (int j = 0; j < miscinfo[i].Length; j++)
                {
                    n.Nodes.Add(miscinfo[i][j] + ": Unknown");
                }
            }

            treeMisc.ExpandAll();

            treeMisc.EndUpdate();
        }

        private void UpdateMiscInfo()
        {
            Process p;

            try
            {
                p = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
            }
            catch
            {
                return;
            }

            treeMisc.BeginUpdate();

            for (int i = 0; i < misctoplevel.Length; i++)
            {
                for (int j = 0; j < miscinfo[i].Length; j++)
                {
                    try
                    {
                        string newtext = miscinfo[i][j] + ": " +
                            miscinfofuncs[i][j].Invoke(p);

                        if (treeMisc.Nodes[i].Nodes[j].Text != newtext)
                            treeMisc.Nodes[i].Nodes[j].Text = newtext;
                    }
                    catch
                    {
                        treeMisc.Nodes[i].Nodes[j].Text = miscinfo[i][j] +
                            ": Unknown";
                    }
                }
            }

            p.Close();

            treeMisc.EndUpdate();
        }

        private void UpdateDriversInfo()
        {
            int RequiredSize = 0;
            int[] ImageBases;
            List<int> done = new List<int>();
            ListViewItem primary = null;

            Win32.EnumDeviceDrivers(null, 0, ref RequiredSize);

            ImageBases = new int[RequiredSize];

            Win32.EnumDeviceDrivers(ImageBases, RequiredSize * sizeof(int), ref RequiredSize);

            listModules.BeginUpdate();

            for (int i = 0; i < RequiredSize; i++)
            {
                if (done.Contains(ImageBases[i]))
                    break;

                if (ImageBases[i] == 0)
                    continue;

                StringBuilder name = new StringBuilder(256);
                StringBuilder filename = new StringBuilder(256);
                string realname = "";
                string desc = "";
                ListViewItem item = new ListViewItem();

                Win32.GetDeviceDriverBaseName(ImageBases[i], name, 255);
                Win32.GetDeviceDriverFileName(ImageBases[i], filename, 255);

                try
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(Misc.GetRealPath(filename.ToString()));

                    realname = fi.FullName;

                    desc = FileVersionInfo.GetVersionInfo(realname).FileDescription;
                }
                catch
                { }

                item = new ListViewItem();

                item.SubItems.Add(new ListViewItem.ListViewSubItem());
                item.SubItems.Add(new ListViewItem.ListViewSubItem());
                item.SubItems.Add(new ListViewItem.ListViewSubItem());

                item.ToolTipText = realname;
                item.SubItems[0].Text = name.ToString();
                item.SubItems[1].Text = String.Format("0x{0:x8}", ImageBases[i]);
                item.SubItems[2].Text = "";
                item.SubItems[3].Text = desc;

                try
                {
                    bool kernel = false;

                    foreach (string k in Misc.KernelNames)
                    {
                        if (realname.ToLower() == Environment.SystemDirectory.ToLower() + "\\" + k.ToLower())
                        {
                            kernel = true;

                            break;
                        }
                    }

                    if (kernel)
                    {
                        primary = item;
                    }
                    else
                    {
                        listModules.Items.Add(item);
                    }
                }
                catch
                { }

                done.Add(ImageBases[i]);
            }

            // sorts the list
            listModules.Sorting = SortOrder.Ascending;
            listModules.Sorting = SortOrder.None;

            if (primary != null)
            {
                primary.Font = new Font(primary.Font, FontStyle.Bold);
                listModules.Items.Insert(0, primary);
            }

            listModules.EndUpdate();
        }

        private void UpdateMemoryInfo()
        {
            Process p;

            try
            {
                p = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
            }
            catch
            {
                return;
            }

            Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
            int address = 0;

            listMemory.BeginUpdate();
            try
            {
                while (true)
                {
                    if (Win32.VirtualQueryEx(p.Handle.ToInt32(), address, ref info,
                        Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))) == 0)
                    {
                        break;
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem();

                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());

                        item.SubItems[0].Text = String.Format("0x{0:x8}", info.BaseAddress);
                        item.SubItems[1].Text = String.Format("0x{0:x8}", info.RegionSize);
                        item.SubItems[2].Text = info.State.ToString().Replace("MEM_", "").Replace("0", "");
                        item.SubItems[3].Text = info.Type.ToString().Replace("MEM_", "").Replace("0", "");
                        item.SubItems[4].Text = info.Protect.ToString().Replace("PAGE_", "");

                        listMemory.Items.Add(item);

                        address += info.RegionSize;
                    }
                }

                tabMemory.Enabled = true;
            }
            catch
            {
                tabMemory.Enabled = false;
            }
            listMemory.EndUpdate();
        }

        // .NET based
        private void UpdateModuleInfo()
        {
            Process p = null;
            ListViewItem primary = null;

            try
            {
                p = Process.GetProcessById(Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text));
            }
            catch
            {
                listModules.Items.Clear();
                listModules.Enabled = false;

                return;
            }

            // Get drivers instead
            if (p.Id == 4)
            {
                UpdateDriversInfo();

                return;
            }

            listModules.BeginUpdate();

            try
            {
                ListViewItem item;

                item = new ListViewItem();

                foreach (ProcessModule m in p.Modules)
                {
                    item = new ListViewItem();

                    item.SubItems.Add(new ListViewItem.ListViewSubItem());
                    item.SubItems.Add(new ListViewItem.ListViewSubItem());
                    item.SubItems.Add(new ListViewItem.ListViewSubItem());

                    try { item.ToolTipText = Misc.GetRealPath(m.FileName); }
                    catch { item.ToolTipText = ""; }

                    try { item.SubItems[0].Text = m.ModuleName; }
                    catch { item.SubItems[0].Text = "(error)"; }

                    try { item.SubItems[1].Text = String.Format("0x{0:x8}", m.BaseAddress.ToInt32()); }
                    catch { item.SubItems[1].Text = ""; }

                    try { item.SubItems[2].Text = Misc.GetNiceSizeName(m.ModuleMemorySize); }
                    catch { item.SubItems[2].Text = ""; }

                    try { item.SubItems[3].Text = FileVersionInfo.GetVersionInfo(Misc.GetRealPath(m.FileName)).FileDescription; }
                    catch { item.SubItems[3].Text = ""; }

                    try
                    {
                        if (m.ModuleName.ToLower() == listProcesses.SelectedItems[0].SubItems[0].Text.ToLower())
                        {
                            primary = item;
                        }
                        else
                        {
                            listModules.Items.Add(item);
                        }
                    }
                    catch
                    { }
                }

                // sorts the list
                listModules.Sorting = SortOrder.Ascending;
                listModules.Sorting = SortOrder.None;

                if (primary != null)
                {
                    primary.Font = new Font(primary.Font, FontStyle.Bold);
                    listModules.Items.Insert(0, primary);
                }

                listModules.Enabled = true;
            }
            catch (Exception ex)
            {
                listModules.Items.Clear();
                listModules.Items.Add(ex.Message);
                listModules.Enabled = false;
            }

            listModules.EndUpdate();
        }

        // toolhelp based
        private void UpdateModuleInfoToolhelp()
        {
            int pid;
            int snapshot;
            Win32.MODULEENTRY32 module = new Win32.MODULEENTRY32();
            ListViewItem primary = null;

            try
            {
                pid = Int32.Parse(listProcesses.SelectedItems[0].SubItems[1].Text);
            }
            catch
            {
                return;
            }

            // Get drivers instead
            if (pid == 4)
            {
                UpdateDriversInfo();

                return;
            }

            snapshot = Win32.CreateToolhelp32Snapshot(Win32.SnapshotFlags.Module, pid);

            module.dwSize = Marshal.SizeOf(typeof(Win32.MODULEENTRY32));

            if (snapshot != 0 && Marshal.GetLastWin32Error() == 0 && pid != 0)
            {
                listModules.BeginUpdate();

                try
                {
                    ListViewItem item;

                    item = new ListViewItem();

                    Win32.Module32First(snapshot, ref module);

                    do
                    {
                        item = new ListViewItem();

                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());
                        item.SubItems.Add(new ListViewItem.ListViewSubItem());

                        item.ToolTipText = module.szExePath;
                        item.SubItems[0].Text = module.szModule;
                        item.SubItems[1].Text = String.Format("0x{0:x8}", module.modBaseAddr);
                        item.SubItems[2].Text = Misc.GetNiceSizeName(module.modBaseSize);

                        try { item.SubItems[3].Text = 
                            FileVersionInfo.GetVersionInfo(Misc.GetRealPath(module.szExePath)).FileDescription; }
                        catch { item.SubItems[3].Text = ""; }

                        if (module.szModule.ToLower() == listProcesses.SelectedItems[0].SubItems[0].Text.ToLower())
                        {
                            primary = item;
                        }
                        else
                        {
                            listModules.Items.Add(item);
                        }
                    } while (Win32.Module32Next(snapshot, ref module) != 0);

                    // sorts the list
                    listModules.Sorting = SortOrder.Ascending;
                    listModules.Sorting = SortOrder.None;

                    if (primary != null)
                    {
                        primary.Font = new Font(primary.Font, FontStyle.Bold);
                        listModules.Items.Insert(0, primary);
                    }

                    listModules.Enabled = true;
                }
                catch (Exception ex)
                {
                    listModules.Items.Clear();
                    listModules.Items.Add(ex.Message);
                    listModules.Enabled = false;
                }
            }
            else if (pid == 0)
            {
                listModules.Items.Clear();
                listModules.Enabled = false;
            }
            else
            {
                listModules.Items.Clear();
                listModules.Items.Add(Win32.GetLastErrorMessage());
                listModules.Enabled = false;
            }

            listModules.EndUpdate();
        }

        private void UpdateProcessExtra()
        {
            listModules.Items.Clear();
            listMemory.Items.Clear();
                                  
            listThreads.Provider = null;

            if (threadP != null)
                threadP.Kill();

            threadP = null;

            GC.Collect();

            if (listProcesses.SelectedItems.Count != 1)
                return;
            if (processSelectedPID == 0)
                return;

            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            threadP = new ThreadProvider(processSelectedPID);
            listThreads.Provider = threadP;
            threadP.Interval = Properties.Settings.Default.RefreshInterval;
            threadP.Enabled = true;

            if (Properties.Settings.Default.UseToolhelpModules)
                UpdateModuleInfoToolhelp();
            else
                UpdateModuleInfo();

            UpdateMemoryInfo();
            UpdateMiscInfo();

            this.Cursor = Cursors.Default;
        }

        #endregion

        private void formViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.AlwaysOnTop = this.TopMost;

            if (threadP != null)
                threadP.Kill();

            processP.Kill();

            SaveSettings();

            // kill, just in case we are forming an operation we don't want random .net errors about disposed objects.
            Process.GetCurrentProcess().Kill();
        }

        public HackerWindow()
        {
            InitializeComponent();

            if (!System.IO.File.Exists(Application.StartupPath + "\\Injector.exe"))
                injectorMenuItem.Visible = false;

            this.TopMost = Properties.Settings.Default.AlwaysOnTop;
            HighlightedListViewItem.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNewProcesses;
            HighlightedListViewItem.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;

            Misc.SetDoubleBuffered(listMemory, typeof(ListView), true);
            Misc.SetDoubleBuffered(listModules, typeof(ListView), true);
            Misc.SetDoubleBuffered(treeMisc, typeof(TreeView), true);

            if (Win32.WriteTokenPrivilege("SeDebugPrivilege", Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED) == 0)
                MessageBox.Show("Debug privilege could not be acquired!" +
                    " This will result in reduced functionality.", "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            InitMiscInfo();

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        private void serviceP_Updated()
        {
            listServices.List.EndUpdate();
            serviceP.Updated -= new ProviderUpdateOnce(serviceP_Updated);
        }

        private void processP_Updated()
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                statusText.Text = "";
                statusMessages.Clear();
                log.Clear();
                timerMessages.Enabled = true;
                HighlightedListViewItem.StateHighlighting = true;
                processP.Updated -= new ProviderUpdateOnce(processP_Updated);
            }));
        }

        private void HackerWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindows();

            timerFire.Interval = RefreshInterval;
            timerFire.Enabled = true;
            timerFire_Tick(null, null);

            listProcesses_SelectedIndexChanged(null, null);

            newResultsWindowMenuItem.Click += new EventHandler(PerformSearch);
            literalSearchMenuItem.Click += new EventHandler(PerformSearch);
            regexSearchMenuItem.Click += new EventHandler(PerformSearch);
            stringScanMenuItem.Click += new EventHandler(PerformSearch);
            heapScanMenuItem.Click += new EventHandler(PerformSearch);

            listViews.Add(listProcesses);
            listViews.Add(listThreads);
            listViews.Add(listModules);
            listViews.Add(listMemory);
            listViews.Add(listServices);

            ListViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, listProcesses.List, null);
            ListViewMenu.AddMenuItems(copyThreadMenuItem.MenuItems, listThreads.List, null);
            ListViewMenu.AddMenuItems(copyModuleMenuItem.MenuItems, listModules, null);
            ListViewMenu.AddMenuItems(copyMemoryMenuItem.MenuItems, listMemory, null);
            ListViewMenu.AddMenuItems(copyServiceMenuItem.MenuItems, listServices.List, null);

            listProcesses.ContextMenu = menuProcess;
            listThreads.ContextMenu = menuThread;
            listModules.ContextMenu = menuModule;
            listMemory.ContextMenu = menuMemory;
            listServices.ContextMenu = menuService;

            HighlightedListViewItem.StateHighlighting = false;
            HighlightedListViewItem.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            processP.Interval = RefreshInterval;
            listProcesses.Provider = processP;
            processP.DictionaryAdded += new ProviderDictionaryAdded(processP_DictionaryAdded);
            processP.DictionaryRemoved += new ProviderDictionaryRemoved(processP_DictionaryRemoved);
            processP.Updated += new ProviderUpdateOnce(processP_Updated);
            processP.Enabled = true;

            listServices.List.BeginUpdate();
            serviceP.Interval = RefreshInterval;
            listServices.Provider = serviceP;
            serviceP.Updated += new ProviderUpdateOnce(serviceP_Updated);
            serviceP.DictionaryAdded += new ProviderDictionaryAdded(serviceP_DictionaryAdded);
            serviceP.DictionaryModified += new ProviderDictionaryModified(serviceP_DictionaryModified);
            serviceP.DictionaryRemoved += new ProviderDictionaryRemoved(serviceP_DictionaryRemoved);
            serviceP.Enabled = true;

            statusText.Text = "Waiting...";

            // huge hack
            listModules.Items.Add(new ListViewItem("b"));
            listModules.Items.Add(new ListViewItem("a"));
            listModules.Sorting = SortOrder.Ascending;
            listModules.Sorting = SortOrder.None;
            listModules.Items.Add(new ListViewItem("c"));
            tabControl.SelectedTab = tabProcess;
            tabControl.SelectedTab = tabThreads;
            tabControl.SelectedTab = tabModules;
            tabControl.SelectedTab = tabMemory;

            listModules.Items.Clear();

            LoadSettings();

            // load symbols on a separate thread
            Thread t = new Thread(new ThreadStart(delegate
            {
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                        {
                            statusIcon.Icon = null;
                            statusText.Text = "Loading symbols for " + module.ModuleName + "...";
                        }));

                    try
                    {
                        if (!module.FileName.ToLower().EndsWith(".exe"))
                            Symbols.LoadSymbolsFromLibrary(module.FileName, module.BaseAddress.ToInt32());
                    }
                    catch (Exception ex)
                    {
                        QueueMessage("Could not load symbols for " + module.ModuleName + ": " + ex.Message, null);
                    }
                }

                this.BeginInvoke(new MethodInvoker(delegate
                {
                    statusIcon.Icon = null;
                    statusText.Text = "";
                }));
            }));

            t.Priority = ThreadPriority.Lowest;
            t.Start(); 
        }
    }
}
