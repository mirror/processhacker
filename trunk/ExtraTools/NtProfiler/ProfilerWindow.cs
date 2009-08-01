using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Symbols;

namespace NtProfiler
{
    public partial class ProfilerWindow : Form
    {
        private readonly IntPtr _userModeBase;
        private readonly IntPtr _userModeLimit;
        private readonly IntPtr _kernelModeBase;
        private readonly IntPtr _kernelModeLimit;

        private ProfileHandle _profileHandle;
        private Dictionary<IntPtr, KernelModule> _kernelModules;
        private SymbolProvider _kernelSymbols;
        private IntPtr _profileBase;
        private uint _profileSize;
        private int _bucketSizeLog;
        private uint _bucketSize;

        public ProfilerWindow()
        {
            InitializeComponent();

            unchecked
            {
                _userModeBase = new IntPtr(0x00000000);
                _userModeLimit = new IntPtr(0x7fffffff);
                _kernelModeBase = new IntPtr((int)0x80000000);
                _kernelModeLimit = new IntPtr((int)0xffffffff);
            }

            try { KProcessHacker.Instance = new KProcessHacker(); }
            catch { }

            try
            {
                using (var thandle = ProcessHandle.GetCurrent().GetToken(TokenAccess.Query | TokenAccess.AdjustPrivileges))
                    thandle.SetPrivilege("SeSystemProfilePrivilege", SePrivilegeAttributes.Enabled);
            }
            catch
            { }

            Win32.LoadLibrary("C:\\Program Files\\Debugging Tools for Windows (x86)\\dbghelp.dll");
            SymbolProvider.Options |= SymbolOptions.DeferredLoads;

            listModules.ListViewItemSorter = new SortedListViewComparer(listModules)
            {
                SortColumn = 1,
                SortOrder = SortOrder.Descending
            };
            listFunctions.ListViewItemSorter = new SortedListViewComparer(listFunctions)
            {
                SortColumn = 1,
                SortOrder = SortOrder.Descending
            };
        }

        private uint GetKernelModeCodeRange(out IntPtr baseAddress)
        {
            IntPtr minAddress = _kernelModeLimit;
            IntPtr maxAddress = _kernelModeBase;

            foreach (var module in Windows.GetKernelModules())
            {
                if (module.BaseAddress.CompareTo(_kernelModeBase) == -1)
                    continue;

                if (module.BaseAddress.CompareTo(minAddress) == -1)
                    minAddress = module.BaseAddress;
                if (module.BaseAddress.CompareTo(maxAddress) == 1)
                    maxAddress = module.BaseAddress;
            }

            baseAddress = minAddress;

            return maxAddress.Decrement(minAddress).ToUInt32();
        }

        private IntPtr GetAddress(int bufferIndex)
        {
            return _profileBase.Increment(_bucketSize * bufferIndex);
        }

        private void LoadKernelSymbols()
        {
            _kernelSymbols = new SymbolProvider(new ProcessHandle(4, ProcessAccess.QueryInformation));
            _kernelSymbols.PreloadModules = true;

            foreach (var module in Windows.GetKernelModules())
            {
                try
                {
                    _kernelSymbols.LoadModule(module.FileName, module.BaseAddress);
                }
                catch
                { }
            }
        }

        private void LoadProfileModules()
        {
            int[] counters = _profileHandle.Collect();
            Dictionary<IntPtr, int> modules = new Dictionary<IntPtr, int>();

            for (int i = 0; i < counters.Length; i++)
            {
                if (counters[i] != 0)
                {
                    IntPtr realAddress = this.GetAddress(i);
                    IntPtr baseAddress;

                    _kernelSymbols.GetModuleFromAddress(realAddress, out baseAddress);

                    if (!modules.ContainsKey(baseAddress))
                        modules.Add(baseAddress, 0);

                    modules[baseAddress]++;
                }
            }

            listModules.Items.Clear();

            foreach (var moduleBase in modules.Keys)
            {
                listModules.Items.Add(new ListViewItem(
                    new string[]
                    {
                        _kernelModules[moduleBase].BaseName,
                        modules[moduleBase].ToString("N0"),
                        _kernelModules[moduleBase].FileName
                    })
                    {
                        Tag = moduleBase
                    }
                    );
            }
        }

        private void LoadProfileFunctions(IntPtr moduleBase)
        {
            int[] counters = _profileHandle.Collect();
            Dictionary<string, int> functions = new Dictionary<string, int>();

            for (int i = 0; i < counters.Length; i++)
            {
                if (counters[i] != 0)
                {
                    IntPtr realAddress = this.GetAddress(i);
                    IntPtr baseAddress;

                    _kernelSymbols.GetModuleFromAddress(realAddress, out baseAddress);

                    if (baseAddress != moduleBase)
                        continue;

                    string fileName;
                    string symbolName;
                    ulong displacement;

                    symbolName = _kernelSymbols.GetSymbolFromAddress(realAddress.ToUInt64(), out fileName, out displacement);

                    if (symbolName != null)
                    {
                        if (!functions.ContainsKey(symbolName))
                            functions.Add(symbolName, 0);

                        functions[symbolName]++;
                    }
                }
            }

            listFunctions.Items.Clear();

            foreach (var function in functions.Keys)
            {
                listFunctions.Items.Add(new ListViewItem(
                    new string[]
                    {
                        function,
                        functions[function].ToString("N0")
                    }));
            }
        }

        #region Menu Items

        #region Profiler

        private void profileProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void profileKernelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IntPtr baseAddress;
            uint size = this.GetKernelModeCodeRange(out baseAddress);

            _kernelModules = new Dictionary<IntPtr,KernelModule>();

            foreach (var module in Windows.GetKernelModules())
                _kernelModules.Add(module.BaseAddress, module);

            _profileBase = baseAddress;
            _profileSize = size;
            _bucketSizeLog = 6; // 64 byte bucket size
            _bucketSize = (uint)(2 << (_bucketSizeLog - 1));
            _profileHandle = ProfileHandle.Create(
                null,
                baseAddress,
                size,
                _bucketSizeLog,
                KProfileSource.ProfileTime,
                IntPtr.Zero
                );
            ProfileHandle.SetInterval(KProfileSource.ProfileTime, 1); // 100 nanoseconds

            this.LoadKernelSymbols();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

        #region Toolbar

        #region Profile Control

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            _profileHandle.Start();
            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = true;
        }

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            _profileHandle.Stop();
            toolStripButtonStart.Enabled = true;
            toolStripButtonStop.Enabled = false;
            this.LoadProfileModules();
        }

        #endregion

        #endregion

        private void listModules_DoubleClick(object sender, EventArgs e)
        {
            this.LoadProfileFunctions((IntPtr)listModules.SelectedItems[0].Tag);
            tabControl.SelectedTab = tabFunctions;
        }
    }
}
