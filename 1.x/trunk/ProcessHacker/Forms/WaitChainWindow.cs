using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class WaitChainWindow : Form
    {
        private int processPid;
        private string processName;

        public WaitChainWindow(string procName, int procPid)
        {
            InitializeComponent();
            this.SetPhParent();
            this.AddEscapeToClose();
            this.SetTopMost();

            processPid = procPid;
            processName = procName;
        }

        private void moreInfoLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://go.microsoft.com/fwlink/?LinkID=136333");
        }

        private void WaitChainWindow_Load(object sender, EventArgs e)
        {
            using (WaitChainTraversal wct = new WaitChainTraversal())
            {
                ShowProcessWaitChains(wct, false);    
            }
        }

        private void ShowProcessWaitChains(WaitChainTraversal wct, bool showAllData)
        {
            var threads = Windows.GetProcessThreads(processPid);

            if (threads == null)
            {
                PhUtils.ShowWarning(string.Format("The process ID {0} does not exist", processPid));
                this.Close();
            }

            textDescription.AppendText(string.Format("Process: {0}, PID: {1}", processName, processPid));

            threadTree.Nodes.Add(string.Format("Process: {0}, PID: {1}", processName, processPid));
           
            foreach (var thread in threads)
            {
                //Get the wait chains for this thread.
                int currThreadId = thread.Key;
                
                WaitData data = wct.GetThreadWaitChain(currThreadId);

                if (data != null)
                {
                    DisplayThreadData(data, showAllData);
                }
                else //This happens when running without admin rights.
                {
                    threadTree.Nodes.Add(string.Format("TID:{0} Unable to retrieve wait chains for this thread without Admin rights", currThreadId));
                }
            }
        }

        private void DisplayThreadData(WaitData data, bool allData)
        {
            // Save the process id value for the first item as this is the 
            // process that owns the thread. we'll use this to check for 
            // items used by other threads, from other processes later.
            int startingPID = data.Nodes[0].ProcessId;
            StringBuilder sb = new StringBuilder();

            if (data.IsDeadlock)
            {
                sb.Append("DEADLOCKED: ");
            }

            for (int i = 0; i < data.NodeCount; i++)
            {
                WaitChainNativeMethods.WAITCHAIN_NODE_INFO node = data.Nodes[i];

                if (node.ObjectType == WaitChainNativeMethods.WCT_OBJECT_TYPE.Thread)
                {
                    String procName = Windows.GetProcesses().ContainsKey(node.ProcessId) ? Windows.GetProcesses()[node.ProcessId].Name : "???";

                    sb.Append(string.Format(" PID: {0} {1} TID: {2}", node.ProcessId, procName, node.ThreadId));

                    //Is this a block on a thread from another process?
                    if ((i > 0) && (startingPID != node.ProcessId))
                    {
                        // Yes, so show the PID and name.
                        sb.Append(string.Format(" PID:{0} {1} TID:{2}", node.ProcessId, procName, node.ThreadId));
                    }

                    if (allData)
                    {
                        sb.Append(string.Format(" Status: {0} Wait: {1} CS: {2:N0}", node.ObjectStatus, node.WaitTime, node.ContextSwitches));
                    }

                    if (node.ObjectStatus != WaitChainNativeMethods.WCT_OBJECT_STATUS.Blocked)
                    {
                        sb.Append(string.Format(" Status: {0}", node.ObjectStatus));
                    }
                }
                else
                {
                    sb.Append(string.Format(" {0} Status: {1}", node.ObjectType, node.ObjectStatus));

                    String name = node.ObjectName();
                    if (!String.IsNullOrEmpty(name))
                    {
                        sb.Append(string.Format(" Name: {0}", name));
                    }
                }
                threadTree.Nodes.Add(sb.ToString());
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessWindow pForm = Program.GetProcessWindow(Program.ProcessProvider.Dictionary[processPid],
                    new Program.PWindowInvokeAction(delegate(ProcessWindow f)
                    {
                        Settings.Instance.ProcessWindowSelectedTab = "tabThreads";
                        f.Show();
                        f.Activate();
                    }));
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to inspect the process", ex);
            }
        }
    }

    /// <summary>
    /// Wraps all the native Wait Chain Traversal code.
    /// </summary>
    public static partial class WaitChainNativeMethods
    {
        // Keep the module handle around for the life of the application as the WCT code has pointers into it.
        private static SafeModuleHandle oleModule;

        // Max. number of nodes in the wait chain
        public const int WCT_MAX_NODE_COUNT = 16;
        // Max. length of a named object.
        private const int WCT_OBJNAME_LENGTH = 128;

        public static SafeWaitChainHandle OpenThreadWaitChainSession()
        {
            // Get the COM APIs into WCT. I have no idea why WCT just doesn't do this itself.
            if (oleModule == null)
            {
                oleModule = LoadLibraryW("OLE32.DLL");
                IntPtr coGetCallState = GetProcAddress(oleModule, "CoGetCallState");
                IntPtr coGetActivationState = GetProcAddress(oleModule, "CoGetActivationState");
                // Register these functions with WCT.
                RegisterWaitChainCOMCallback(coGetCallState, coGetActivationState);
            }

            SafeWaitChainHandle wctHandle = RealOpenThreadWaitChainSession(0, IntPtr.Zero);
            if (wctHandle.IsInvalid == true)
            {
                throw new InvalidOperationException("Unable to open the Wait Thread Chain.");
            }
            return (wctHandle);
        }

        public static bool GetThreadWaitChain(SafeWaitChainHandle chainHandle, int threadId, ref int NodeCount, WAITCHAIN_NODE_INFO[] NodeInfoArray, out int IsCycle)
        {
            return RealGetThreadWaitChain(chainHandle, IntPtr.Zero, WCT_FLAGS.All, threadId, ref NodeCount, NodeInfoArray, out IsCycle);
        }

        public static void CloseThreadWaitChainSession(IntPtr handle)
        {
            RealCloseThreadWaitChainSession(handle);
        }

        /// <summary>
        /// The data structure returned indicating blocked threads.
        /// </summary>
        /// <remarks>
        /// Even though the ObjectName field is a character array, it's declared
        /// as a ushort because of a bug in the VS05 CLR marshalling with character
        /// arrays and the fixed keyword. By using the ushort, the structure is
        /// now blittable, meaning the managed and native types are the same.
        /// A char is not blittable because it has multiple representations in 
        /// native code (ANSI and UNICODE). 
        /// Fortunately, to get the actual character array in ObjectName, you 
        /// can cast the ushort pointer to a char pointer passed to the String
        /// constructor.
        /// </remarks>
        [StructLayout(LayoutKind.Explicit, Size = 280)]
        public unsafe struct WAITCHAIN_NODE_INFO
        {
            [FieldOffset(0x0)]
            public WCT_OBJECT_TYPE ObjectType;
            [FieldOffset(0x4)]
            public WCT_OBJECT_STATUS ObjectStatus;

            // The name union.
            [FieldOffset(0x8)]
            private fixed ushort RealObjectName[WCT_OBJNAME_LENGTH];
            [FieldOffset(0x108)]
            public int TimeOutLowPart;
            [FieldOffset(0x10C)]
            public int TimeOutHiPart;
            [FieldOffset(0x110)]
            public int Alertable;

            // The thread union.
            [FieldOffset(0x8)]
            public int ProcessId;
            [FieldOffset(0xC)]
            public int ThreadId;
            [FieldOffset(0x10)]
            public int WaitTime;
            [FieldOffset(0x14)]
            public int ContextSwitches;

            //TODO: fix this... fixes old VS05 bug thats now non-existent
            //Does the work to get the ObjectName field.
            public String ObjectName()
            {
                fixed (WAITCHAIN_NODE_INFO* p = &this)
                {
                    return (p->RealObjectName[0] != '\0') ? new string((char*)p->RealObjectName) : string.Empty;
                }
            }
        }

        [Flags]
        public enum WCT_OBJECT_TYPE
        {
            CriticalSection = 1,
            SendMessage,
            Mutex,
            Alpc,
            COM,
            ThreadWait,
            ProcessWait,
            Thread,
            COMActivation,
            Unknown,
        } ;

        [Flags]
        public enum WCT_OBJECT_STATUS
        {
            NoAccess = 1,            // ACCESS_DENIED for this object
            Running,                 // Thread status
            Blocked,                 // Thread status
            PidOnly,                 // Thread status
            PidOnlyRpcss,            // Thread status
            Owned,                   // Dispatcher object status
            NotOwned,                // Dispatcher object status
            Abandoned,               // Dispatcher object status
            Unknown,                 // All objects
            Error,                   // All objects
        } ;

        [Flags]
        public enum WCT_FLAGS
        {
            Flag = 0x1,
            COM = 0x2,
            Proc = 0x4,
            All = Flag | COM | Proc
        }

        [DllImport("advapi32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode, EntryPoint = "CloseThreadWaitChainSession")]
        private static extern void RealCloseThreadWaitChainSession(IntPtr wctHandle);

        [DllImport("advapi32.dll", EntryPoint = "OpenThreadWaitChainSession")]
        private static extern SafeWaitChainHandle RealOpenThreadWaitChainSession(int flags, IntPtr callback);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern void RegisterWaitChainCOMCallback(IntPtr callStateCallback, IntPtr activationStateCallback);

        [DllImport("advapi32.dll", EntryPoint = "GetThreadWaitChain")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RealGetThreadWaitChain(SafeWaitChainHandle WctHandle, IntPtr Context, WCT_FLAGS Flags, int ThreadId, ref int NodeCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] [In, Out] WAITCHAIN_NODE_INFO[] NodeInfoArray, out int IsCycle);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern SafeModuleHandle LoadLibraryW(String lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetProcAddress(SafeModuleHandle hModule, string lpProcName);
    }

    public class SafeModuleHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeModuleHandle()
            : base(true)
        {
        }

        protected override Boolean ReleaseHandle()
        {
            return (WaitChainNativeMethods.FreeLibrary(this.handle));
        }
    }

    public class SafeWaitChainHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeWaitChainHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            WaitChainNativeMethods.CloseThreadWaitChainSession(this.handle);
            return (true);
        }
    }

    public sealed class WaitData
    {
        private WaitChainNativeMethods.WAITCHAIN_NODE_INFO[] data;
        private bool isDeadlock;
        private int nodeCount;

        public WaitData(WaitChainNativeMethods.WAITCHAIN_NODE_INFO[] data, int nodeCount, bool isDeadlock)
        {
            this.data = data;
            this.nodeCount = nodeCount;
            this.isDeadlock = isDeadlock;
        }

        public WaitChainNativeMethods.WAITCHAIN_NODE_INFO[] Nodes
        {
            get
            {
                return (data);
            }
        }

        public int NodeCount
        {
            get
            {
                return (nodeCount);
            }
        }

        public bool IsDeadlock
        {
            get
            {
                return (isDeadlock);
            }
        }
    }

    public sealed class WaitChainTraversal : IDisposable
    {
        private SafeWaitChainHandle waitChainHandle;

        public WaitChainTraversal()
        {
            waitChainHandle = WaitChainNativeMethods.OpenThreadWaitChainSession();
        }

        public WaitData GetThreadWaitChain(int threadId)
        {
            WaitChainNativeMethods.WAITCHAIN_NODE_INFO[] data = new WaitChainNativeMethods.WAITCHAIN_NODE_INFO[WaitChainNativeMethods.WCT_MAX_NODE_COUNT];
            int isDeadlock = 0;
            int nodeCount = WaitChainNativeMethods.WCT_MAX_NODE_COUNT;

            WaitData retData = null;

            if (WaitChainNativeMethods.GetThreadWaitChain(waitChainHandle, threadId, ref nodeCount, data, out isDeadlock))
            {
                retData = new WaitData(data, (int)nodeCount, isDeadlock == 1);
            }

            return (retData);
        }

        #region IDisposable Members

        public void Dispose()
        {
            waitChainHandle.Dispose();
        }

        #endregion
    }

}
