/*
 * Process Hacker - 
 *   dump viewer (process)
 * 
 * Copyright (C) 2009 wj32
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
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Mfs; 
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class DumpProcessWindow : Form
    {
        private DumpHackerWindow _hw;
        private ProcessItem _item;
        private MemoryObject _processMo;

        public DumpProcessWindow(DumpHackerWindow hw, ProcessItem item, MemoryObject processMo)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            _hw = hw;
            _item = item;
            _processMo = processMo;

            this.Text = _item.Name + " (PID " + _item.Pid.ToString() + ")";
            this.Icon = _item.Icon;
        }

        private void DumpProcessWindow_Load(object sender, EventArgs e)
        {
            this.LoadProperties();

            // Modules
            if (_item.FileName != null)
                listModules.DumpSetMainModule(_item.FileName);
            listModules.List.SetTheme("explorer");
            listModules.List.AddShortcuts();
            listModules.List.ContextMenu = listModules.List.GetCopyMenu();

            this.LoadModules();
            listModules.UpdateItems();

            // Environment
            listEnvironment.SetTheme("explorer");
            listEnvironment.AddShortcuts();
            listEnvironment.ContextMenu = listEnvironment.GetCopyMenu();

            this.LoadEnvironment();

            // Handles
            listHandles.DumpDisableEvents();
            listHandles.List.SetTheme("explorer");
            listHandles.List.AddShortcuts();
            listHandles.List.ContextMenu = listHandles.List.GetCopyMenu();

            this.LoadHandles();
            listHandles.UpdateItems();
        }

        private void DumpProcessWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pictureIcon.Image != null)
                pictureIcon.Image.Dispose();

            _processMo.Dispose();
        }

        private void LoadProperties()
        {
            var names = _processMo.GetChildNames();

            if (names.Contains("LargeIcon"))
            {
                using (var largeIcon = _processMo.GetChild("LargeIcon"))
                    pictureIcon.Image = Dump.GetIcon(largeIcon).ToBitmap();
            }
            else
            {
                pictureIcon.Image = Properties.Resources.Process.ToBitmap();
            }

            if (_item.VersionInfo != null)
            {
                textFileDescription.Text = _item.VersionInfo.FileDescription;
                textFileCompany.Text = _item.VersionInfo.CompanyName;
                textFileVersion.Text = _item.VersionInfo.FileVersion;
            }

            textFileName.Text = _item.FileName;

            if (_item.VerifyResult == VerifyResult.Trusted)
            {
                if (!string.IsNullOrEmpty(_item.VerifySignerName))
                    textFileCompany.Text = _item.VerifySignerName + " (verified)";
                else
                    textFileCompany.Text += " (verified)";
            }

            textStartTime.Text = _item.CreateTime.ToString();
            textCmdLine.Text = _item.CmdLine;

            if (_item.HasParent)
            {
                if (_hw.Processes.ContainsKey(_item.ParentPid))
                {
                    textParent.Text =
                        _hw.Processes[_item.ParentPid].Name + " (" + _item.ParentPid.ToString() + ")";
                }
                else
                {
                    textParent.Text = "Non-existent Process (" + _item.ParentPid.ToString() + ")";
                    buttonInspectParent.Enabled = false;
                }
            }
            else if (_item.ParentPid == -1)
            {
                textParent.Text = "No Parent Process";
                buttonInspectParent.Enabled = false;
            }
            else
            {
                textParent.Text = "Non-existent Process (" + _item.ParentPid.ToString() + ")";
                buttonInspectParent.Enabled = false;
            }

            using (var general = _processMo.GetChild("General"))
            {
                var dict = Dump.GetDictionary(general);

                if (dict.ContainsKey("CurrentDirectory"))
                    textCurrentDirectory.Text = dict["CurrentDirectory"];

                if (dict.ContainsKey("DepStatus"))
                {
                    DepStatus status = (DepStatus)Dump.ParseInt32(dict["DepStatus"]);

                    if ((status & DepStatus.Enabled) != 0)
                        textDEP.Text = "Enabled";
                    else
                        textDEP.Text = "Disabled";

                    if ((status & DepStatus.Permanent) != 0)
                        textDEP.Text += ", Permanent";
                    if ((status & DepStatus.AtlThunkEmulationDisabled) != 0)
                        textDEP.Text += ", DEP-ATL thunk emulation disabled";
                }

                if (_hw.Architecture == OSArch.Amd64)
                {                          
                    labelProcessType.Visible = true;
                    labelProcessTypeValue.Visible = true;
                    labelProcessTypeValue.Text = _item.IsWow64 ? "32-bit" : "64-bit";
                }
                else
                {
                    labelProcessType.Visible = false;
                    labelProcessTypeValue.Visible = false;
                }
            }
        }

        private void LoadModules()
        {
            MemoryObject modulesMo;

            modulesMo = _processMo.GetChild("Modules");

            if (modulesMo == null)
                return;

            modulesMo.EnumChildren((childMo) =>
            {
                using (childMo)
                    this.LoadModule(childMo);

                return true;
            });

            modulesMo.Dispose();
        }

        private void LoadModule(MemoryObject mo)
        {
            var dict = Dump.GetDictionary(mo);
            ModuleItem item = new ModuleItem();

            item.BaseAddress = Dump.ParseInt64(dict["BaseAddress"]);
            item.Size = Dump.ParseInt32(dict["Size"]);
            item.Flags = (LdrpDataTableEntryFlags)Dump.ParseInt32(dict["Flags"]);
            item.Name = dict["Name"];
            item.FileName = dict["FileName"];

            if (dict.ContainsKey("FileDescription"))
            {
                item.FileDescription = dict["FileDescription"];
                item.FileVersion = dict["FileVersion"];
            }

            listModules.AddItem(item);
        }

        private void LoadEnvironment()
        {
            var env = _processMo.GetChild("Environment");
            var dict = Dump.GetDictionary(env);

            foreach (var kvp in dict)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                    listEnvironment.Items.Add(new ListViewItem(new string[] { kvp.Key, kvp.Value }));
            }

            env.Dispose();
        }

        private void LoadHandles()
        {
            MemoryObject handlesMo;

            handlesMo = _processMo.GetChild("Handles");

            if (handlesMo == null)
                return;

            handlesMo.EnumChildren((childMo) =>
            {
                using (childMo)
                    this.LoadHandles(childMo);

                return true;
            });

            handlesMo.Dispose();
        }

        private void LoadHandles(MemoryObject mo)
        {
            var dict = Dump.GetDictionary(mo);
            HandleItem item = new HandleItem();

            if (!dict.ContainsKey("Handle"))
                return;

            item.Handle.ProcessId = _item.Pid;
            item.Handle.Flags = (HandleFlags)Dump.ParseInt32(dict["Flags"]);
            item.Handle.Handle = (short)Dump.ParseInt32(dict["Handle"]);
            // Not really needed, just fill it in ignoring 32-bit vs 64-bit 
            // differences.
            item.Handle.Object = (Dump.ParseInt64(dict["Object"]) & 0xffffffff).ToIntPtr();
            item.Handle.GrantedAccess = Dump.ParseInt32(dict["GrantedAccess"]);

            if (dict.ContainsKey("TypeName"))
            {
                item.ObjectInfo.TypeName = dict["TypeName"];
                item.ObjectInfo.BestName = dict["ObjectName"];
            }

            if (Settings.Instance.HideHandlesWithNoName)
            {
                if (string.IsNullOrEmpty(item.ObjectInfo.BestName))
                    return;
            }

            listHandles.AddItem(item);
        }

        private void buttonInspectParent_Click(object sender, EventArgs e)
        {
            _hw.ShowProperties(_hw.Processes[_item.ParentPid]);
        }
    }
}
