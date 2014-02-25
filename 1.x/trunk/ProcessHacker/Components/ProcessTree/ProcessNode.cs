/*
 * Process Hacker - 
 *   Node implementation for the process tree
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.Collections.Generic;
using System.Drawing;
using Aga.Controls.Tree;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class ProcessNode : Node, IDisposable
    {
        [ThreadStatic]
        private static ProcessNode[] _processNodeTreePathBuffer;
        private const int _processNodeTreePathMaxDepth = 512;

        private ProcessNode _parent = null;
        private List<ProcessNode> _children = new List<ProcessNode>();
        private TreePath _treePath = null;

        private ProcessItem _pitem;
        private bool _wasNoIcon = false;
        private Bitmap _icon;

        private string _tooltipText;
        private int _lastTooltipTickCount = 0;

        public ProcessNode(ProcessItem pitem)
        {
            _pitem = pitem;
            this.Tag = pitem.Pid;

            if (_pitem.Icon == null)
            {
                _wasNoIcon = true;
                _icon = global::ProcessHacker.Properties.Resources.Process_small.ToBitmap();
            }
            else
            {
                try
                {
                    _icon = _pitem.Icon.ToBitmap();
                }
                catch
                {
                    _wasNoIcon = true;
                    _icon = global::ProcessHacker.Properties.Resources.Process_small.ToBitmap();
                }
            }
        }

        ~ProcessNode()
        {
            if (_icon != null)
                this.Dispose();
        }

        public void Dispose()
        {
            if (_icon != null)
            {
                _icon.Dispose();
                _icon = null;
            }
        }

        public ProcessItem ProcessItem
        {
            get { return _pitem; }
            set
            {
                _pitem = value;

                if (_wasNoIcon && _pitem.Icon != null)
                {
                    if (_icon != null)
                        _icon.Dispose();

                    _icon = new Bitmap(16, 16);

                    try
                    {
                        using (Graphics g = Graphics.FromImage(_icon))
                            g.DrawIcon(_pitem.Icon, new Rectangle(0, 0, 16, 16));

                        _wasNoIcon = false;
                    }
                    catch
                    {
                        _icon.Dispose();
                        _icon = null;
                    }
                }
            }
        }

        public new ProcessNode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<ProcessNode> Children
        {
            get { return _children; }
        }

        public TreePath TreePath
        {
            get { return _treePath; }
        }

        public string GetTooltipText(ProcessToolTipProvider provider)
        {
            int tickCount = Environment.TickCount;

            if (tickCount - _lastTooltipTickCount >= Settings.Instance.RefreshInterval)
            {
                _tooltipText = provider.GetToolTip(this);
                _lastTooltipTickCount = tickCount;
            }

            return _tooltipText;
        }

        public TreePath RefreshTreePath()
        {
            ProcessNode currentNode = this;
            int i = _processNodeTreePathMaxDepth;

            if (_processNodeTreePathBuffer == null)
                _processNodeTreePathBuffer = new ProcessNode[_processNodeTreePathMaxDepth];

            while (i > 0 && currentNode != null)
            {
                _processNodeTreePathBuffer[--i] = currentNode;
                currentNode = currentNode.Parent;
            }

            ProcessNode[] path;

            path = new ProcessNode[_processNodeTreePathMaxDepth - i];
            Array.Copy(_processNodeTreePathBuffer, i, path, 0, _processNodeTreePathMaxDepth - i);

            _treePath = new TreePath(path);

            return _treePath;
        }

        public void RefreshTreePathRecursive()
        {
            foreach (var child in _children)
                child.RefreshTreePathRecursive();

            this.RefreshTreePath();
        }

        public ProcessHacker.Components.NodePlotter.PlotterInfo CpuHistory
        {
            get
            {
                return new ProcessHacker.Components.NodePlotter.PlotterInfo()
                {
                    UseSecondLine = true,
                    OverlaySecondLine = false,
                    UseLongData = false,
                    Data1 = _pitem.CpuKernelHistory,
                    Data2 = _pitem.CpuUserHistory,
                    LineColor1 = Settings.Instance.PlotterCPUKernelColor,
                    LineColor2 = Settings.Instance.PlotterCPUUserColor
                };
            }
        }

        public ProcessHacker.Components.NodePlotter.PlotterInfo IoHistory
        {
            get
            {
                return new ProcessHacker.Components.NodePlotter.PlotterInfo()
                {
                    UseSecondLine = true,
                    OverlaySecondLine = true,
                    UseLongData = true,
                    LongData1 = _pitem.IoReadOtherHistory,
                    LongData2 = _pitem.IoWriteHistory,
                    LineColor1 = Settings.Instance.PlotterIOROColor,
                    LineColor2 = Settings.Instance.PlotterIOWColor
                };
            }
        }

        public string Name
        {
            get { return _pitem.Name ?? ""; }
        }

        public string DisplayPid
        {
            get
            {
                if (_pitem.Pid >= 0)
                    return _pitem.Pid.ToString();
                else
                    return "";
            }
        }

        public int Pid
        {
            get { return _pitem.Pid; }
        }

        public int PPid
        {
            get { if (_pitem.Pid == _pitem.ParentPid) return -1; else return _pitem.ParentPid; }
        }

        public string PvtMemory
        {
            get { return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.PrivatePageCount); }
        }

        public string WorkingSet
        {
            get
            {
                return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.WorkingSetSize);
            }
        }

        public string PeakWorkingSet
        {
            get { return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.PeakWorkingSetSize); }
        }

        private int GetWorkingSetNumber(NProcessHacker.WsInformationClass WsInformationClass)
        {
            NtStatus status;
            int wsInfo;
            int retLen;

            try
            {
                using (var phandle = new ProcessHandle(_pitem.Pid, 
                    ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                {
                    if ((status = NProcessHacker.PhQueryProcessWs(phandle, WsInformationClass, out wsInfo,
                        4, out retLen)) < NtStatus.Error)
                        return wsInfo * Program.ProcessProvider.System.PageSize;
                }
            }
            catch
            { }

            return 0;
        }

        public int WorkingSetNumber
        {
            get { return this.GetWorkingSetNumber(NProcessHacker.WsInformationClass.WsCount); }
        }

        public int PrivateWorkingSetNumber
        {
            get { return this.GetWorkingSetNumber(NProcessHacker.WsInformationClass.WsPrivateCount); }
        }

        public string PrivateWorkingSet
        {
            get { return Utils.FormatSize(this.PrivateWorkingSetNumber); }
        }

        public int SharedWorkingSetNumber
        {
            get { return this.GetWorkingSetNumber(NProcessHacker.WsInformationClass.WsSharedCount); }
        }

        public string SharedWorkingSet
        {
            get { return Utils.FormatSize(this.SharedWorkingSetNumber); }
        }

        public int ShareableWorkingSetNumber
        {
            get { return this.GetWorkingSetNumber(NProcessHacker.WsInformationClass.WsShareableCount); }
        }

        public string ShareableWorkingSet
        {
            get { return Utils.FormatSize(this.ShareableWorkingSetNumber); }
        }

        public string VirtualSize
        {
            get { return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.VirtualSize); }
        }

        public string PeakVirtualSize
        {
            get { return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.PeakVirtualSize); }
        }

        public string PagefileUsage
        {
            get { return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.PagefileUsage); }
        }

        public string PeakPagefileUsage
        {
            get { return Utils.FormatSize(_pitem.Process.VirtualMemoryCounters.PeakPagefileUsage); }
        }

        public string PageFaults
        {
            get { return _pitem.Process.VirtualMemoryCounters.PageFaultCount.ToString("N0"); }
        }

        public string Cpu
        {
            get
            {
                if (_pitem.CpuUsage == 0)
                    return "";
                else
                    return _pitem.CpuUsage.ToString("F2");
            }
        }

        public string Username
        {
            get { return PhUtils.GetBestUserName(_pitem.Username, Settings.Instance.ShowAccountDomains); }
        }

        public string SessionId
        {
            get
            {
                if (Pid < 4)
                    return "";
                else
                    return _pitem.SessionId.ToString();
            }
        }

        public string PriorityClass
        {
            get
            {
                try
                {
                    using (var phandle = new ProcessHandle(Pid, Program.MinProcessQueryRights))
                        return PhUtils.FormatPriorityClass(phandle.GetPriorityClass());
                }
                catch
                {
                    return "";
                }
            }
        }

        public string BasePriority
        {
            get
            {
                if (Pid < 4)
                    return "";
                else
                    return _pitem.Process.BasePriority.ToString();
            }
        }

        public string Description
        {
            get
            {
                if (Pid == 0)
                    return "System Idle Process";
                else if (Pid == -2)
                    return "Deferred Procedure Calls";
                else if (Pid == -3)
                    return "Interrupts";
                else if (_pitem.VersionInfo != null && _pitem.VersionInfo.FileDescription != null)
                    return _pitem.VersionInfo.FileDescription;
                else
                    return "";
            }
        }

        public string Company
        {
            get
            {
                if (_pitem.VersionInfo != null && _pitem.VersionInfo.CompanyName != null)
                    return _pitem.VersionInfo.CompanyName;
                else
                    return "";
            }
        }

        public string FileName
        {
            get
            {
                if (_pitem.FileName == null)
                    return "";
                else
                    return _pitem.FileName;
            }
        }

        public string CommandLine
        {
            get
            {
                if (_pitem.CmdLine == null)
                    return "";
                else
                    return _pitem.CmdLine.Replace("\0", "");
            }
        }

        public string Threads
        {
            get
            {
                if (Pid < 4)
                    return "";
                else
                    return _pitem.Process.NumberOfThreads.ToString();
            }
        }

        public string Handles
        {
            get
            {
                if (Pid < 4)
                    return "";
                else
                    return _pitem.Process.HandleCount.ToString();
            }
        }

        public int GdiHandlesNumber
        {
            get
            {
                try
                {
                    using (var phandle = new ProcessHandle(Pid, ProcessAccess.QueryInformation))
                        return phandle.GetGuiResources(false);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public string GdiHandles
        {
            get
            {
                if (Pid < 4)
                    return "";
                else
                {
                    int number = this.GdiHandlesNumber;

                    if (number == 0)
                        return "";
                    else
                        return number.ToString();
                }
            }
        }

        public int UserHandlesNumber
        {
            get
            {
                try
                {
                    using (var phandle = new ProcessHandle(Pid, ProcessAccess.QueryInformation))
                        return phandle.GetGuiResources(true);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public string UserHandles
        {
            get
            {
                if (Pid < 4)
                    return "";
                else
                {
                    int number = this.UserHandlesNumber;

                    if (number == 0)
                        return "";
                    else
                        return number.ToString();
                }
            }
        }

        public long IoTotalNumber
        {
            get
            {
                return (_pitem.IoReadDelta.Delta + _pitem.IoWriteDelta.Delta +
                    _pitem.IoOtherDelta.Delta) * 1000 / Settings.Instance.RefreshInterval;
            }
        }

        public string IoTotal
        {
            get
            {
                if (this.IoTotalNumber == 0)
                    return "";
                else
                    return Utils.FormatSize(this.IoTotalNumber) + "/s";
            }
        }

        public long IoReadOtherNumber
        {
            get
            {
                return (_pitem.IoReadDelta.Delta + _pitem.IoOtherDelta.Delta) * 1000 /
                    Settings.Instance.RefreshInterval;
            }
        }

        public string IoReadOther
        {
            get
            {
                if (this.IoReadOtherNumber == 0)
                    return "";
                else
                    return Utils.FormatSize(this.IoReadOtherNumber) + "/s";
            }
        }

        public long IoWriteNumber
        {
            get
            {
                return _pitem.IoWriteDelta.Delta * 1000 /
                    Settings.Instance.RefreshInterval;
            }
        }

        public string IoWrite
        {
            get
            {
                if (this.IoWriteNumber == 0)
                    return "";
                else
                    return Utils.FormatSize(this.IoWriteNumber) + "/s";
            }
        }

        public string Integrity
        {
            get { return _pitem.Integrity; }
        }

        public int IntegrityLevel
        {
            get { return _pitem.IntegrityLevel; }
        }

        public int IoPriority
        {
            get
            {
                try
                {
                    return _pitem.ProcessQueryHandle.GetIoPriority();
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int PagePriority
        {
            get
            {
                try
                {
                    return _pitem.ProcessQueryHandle.GetPagePriority();
                }
                catch
                {
                    return 0;
                }
            }
        }

        public Bitmap Icon
        {
            get { return _icon; }
        }

        public string StartTime
        {
            get
            {
                if (Pid < 4 || _pitem.CreateTime.Year == 1)
                    return "";
                else
                    return _pitem.CreateTime.ToString();
            }
        }

        public string RelativeStartTime
        {
            get
            {
                if (Pid < 4 || _pitem.CreateTime.Year == 1)
                    return "";
                else
                    return Utils.FormatRelativeDateTime(_pitem.CreateTime);
            }
        }

        public string TotalCpuTime
        {
            get { return Utils.FormatTimeSpan(new TimeSpan(_pitem.Process.KernelTime + _pitem.Process.UserTime)); }
        }

        public string KernelCpuTime
        {
            get { return Utils.FormatTimeSpan(new TimeSpan(_pitem.Process.KernelTime)); }
        }

        public string UserCpuTime
        {
            get { return Utils.FormatTimeSpan(new TimeSpan(_pitem.Process.UserTime)); }
        }

        public string VerificationStatus
        {
            get { return _pitem.VerifyResult == VerifyResult.Trusted ? "Verified" : ""; }
        }

        public string VerifiedSigner
        {
            get { return _pitem.VerifyResult == VerifyResult.Trusted ? _pitem.VerifySignerName : ""; }
        }
    }
}
