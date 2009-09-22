using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace TaskbarLib
{
    /// <summary>
    /// Represents a thumbnail toolbar button.
    /// </summary>
    public abstract class ThumbnailBarButtonBase : IDisposable
    {
        private static Dictionary<IntPtr, uint> idCounters;

        private uint id;
        private bool isDisabled;
        private bool isDismissedOnClick;
        private bool hasBackground;
        private bool isHidden;
        private string tooltip;

        private SafeHandle iconHandle;
        private IntPtr windowHandle;

        static ThumbnailBarButtonBase()
        {
            idCounters = new Dictionary<IntPtr, uint>();
        }

        protected ThumbnailBarButtonBase(string tooltip, bool isHidden, bool isDisabled, bool isDismissedOnClick, bool hasBackground)
        {
            this.isHidden = isHidden;
            this.isDisabled = isDisabled;
            this.isDismissedOnClick = isDismissedOnClick;
            this.hasBackground = hasBackground;
            this.tooltip = tooltip;
        }

        /// <summary>
        /// Finalizer for <see cref="ThumbnailBarButton"/> type.
        /// </summary>
        ~ThumbnailBarButtonBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occurs when button is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Specifies whether button is disabled.
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return this.isDisabled;
            }

            set
            {
                this.isDisabled = value;

                this.Update();
            }
        }

        /// <summary>
        /// Specifies whether button is dismissed after click.
        /// </summary>
        public bool IsDismissedOnClick
        {
            get
            {
                return this.isDismissedOnClick;
            }

            set
            {
                this.isDismissedOnClick = value;

                this.Update();
            }
        }

        /// <summary>
        /// Specifies whether button has background.
        /// </summary>
        public bool HasBackground
        {
            get
            {
                return this.hasBackground;
            }

            set
            {
                this.hasBackground = value;

                this.Update();
            }
        }

        /// <summary>
        /// Specifies whether button is hidden.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return this.isHidden;
            }

            set
            {
                this.isHidden = value;

                this.Update();
            }
        }

        /// <summary>
        /// Tooltip for button.
        /// </summary>
        public string Tooltip
        {
            get
            {
                return this.tooltip;
            }

            set
            {
                this.tooltip = value;

                this.Update();
            }
        }

        internal uint Id
        {
            get
            {
                return this.id;
            }
        }

        internal IntPtr WindowHandle
        {
            get
            {
                return this.windowHandle;
            }
        }

        protected SafeHandle IconHandle
        {
            get
            {
                return this.iconHandle;
            }

            set
            {
                if (this.iconHandle != null && !this.iconHandle.IsInvalid)
                {
                    this.iconHandle.Dispose();
                }

                this.iconHandle = value;
            }
        }

        /// <summary>
        /// Dispose current instance deterministicly.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        internal void Initialize(IntPtr windowHandle)
        {
            if (this.id > 0)
            {
                return;
            }

            this.windowHandle = windowHandle;

            uint id;
            if (idCounters.TryGetValue(windowHandle, out id))
            {
                idCounters[windowHandle]++;
            }
            else
            {
                id = 1;
                idCounters.Add(windowHandle, 2);
            }

            this.id = id;
        }

        internal TaskbarClass.THUMBBUTTON GetUnmanagedButton()
        {
            TaskbarClass.THUMBBUTTON button = new TaskbarClass.THUMBBUTTON();
            button.iId = this.id;
            button.dwMask = TaskbarClass.THBMASK.THB_FLAGS;
            button.dwFlags = TaskbarClass.THBFLAGS.THBF_ENABLED;

            this.BeforeGetUnmanagedButton(ref button);

            if (this.IconHandle != null && !this.IconHandle.IsInvalid)
            {
                button.hIcon = this.iconHandle.DangerousGetHandle();

                button.dwMask |= TaskbarClass.THBMASK.THB_ICON;
            }

            if (!string.IsNullOrEmpty(this.Tooltip))
            {
                button.szTip = this.Tooltip;

                button.dwMask |= TaskbarClass.THBMASK.THB_TOOLTIP;
            }

            if (this.IsDisabled)
            {
                button.dwFlags |= TaskbarClass.THBFLAGS.THBF_DISABLED;
            }

            if (this.IsDismissedOnClick)
            {
                button.dwFlags |= TaskbarClass.THBFLAGS.THBF_DISMISSONCLICK;
            }

            if (!this.HasBackground)
            {
                button.dwFlags |= TaskbarClass.THBFLAGS.THBF_NOBACKGROUND;
            }

            if (this.IsHidden)
            {
                button.dwFlags |= TaskbarClass.THBFLAGS.THBF_HIDDEN;
            }

            return button;
        }

        internal void FireClickEvent()
        {
            var handler = this.Click;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        internal virtual void BeforeGetUnmanagedButton(ref TaskbarClass.THUMBBUTTON button)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            if (this.iconHandle != null && !this.iconHandle.IsInvalid)
            {
                this.iconHandle.Dispose();
            }
        }

        protected abstract void Update();
    }
}