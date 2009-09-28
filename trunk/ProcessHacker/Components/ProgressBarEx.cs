using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ProcessHacker.Native;

namespace ProgressBarEx
{
    [ToolboxBitmap(typeof(ProgressBar))]
    public class ProgressBarEx : ProgressBar
    {
        private ProgressState FState; // = ProgressState.Normal;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_USER = 0x400;
        private const int PBM_SETSTATE = WM_USER + 16;
        private const int PBST_NORMAL = 0x0001;
        private const int PBST_ERROR = 0x0002;
        private const int PBST_PAUSED = 0x0003;

        public ProgressBarEx() : base()
        {
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (OSVersion.HasUac)
            {
                SetVistaState(FState);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if ((m.Msg == 15) && OSVersion.HasUac && (FState != ProgressState.Normal))
            {
                SetVistaState(FState);
            }
            base.WndProc(ref m);
        }

        private void SetVistaState(ProgressState state)
        {
            switch (state)
            {
                case ProgressState.Pause:
                    {
                        SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_PAUSED, IntPtr.Zero);
                        break;
                    }
                case ProgressState.Error:
                    {
                        SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_ERROR, IntPtr.Zero);
                        break;
                    }
                case ProgressState.Normal:
                default:
                    {
                        SendMessage(Handle, PBM_SETSTATE, (IntPtr)PBST_NORMAL, IntPtr.Zero);
                        break;
                    }
            }
        }

        [DefaultValue(typeof(ProgressState), "Normal")]
        [Category("Behavior")]
        public ProgressState State
        {
            get { return FState; }
            set
            {
                if (Style != ProgressBarStyle.Blocks && !OSVersion.HasUac)
                {
                    value = ProgressState.Normal;
                }
         
                if (FState == value)
                {
                    return;
                }

                FState = value;

                if (!Enum.IsDefined(typeof(ProgressState), value))
                {
                    throw new InvalidEnumArgumentException();
                }
            }
        }
    }

    public enum ProgressState
    { 
        Normal, 
        Pause, 
        Error 
    }
}
