using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Components
{
    public partial class MessageLabel : UserControl
    {
        private MessageLabelIcon _icon = MessageLabelIcon.None;

        public MessageLabel()
        {
            InitializeComponent();
        }

        public bool AutoEllipsis
        {
            get { return labelText.AutoEllipsis; }
            set { labelText.AutoEllipsis = value; }
        }

        public MessageLabelIcon Icon
        {
            get { return _icon; }
            set
            {
                Image oldImage;

                _icon = value;

                oldImage = pictureIcon.Image;

                pictureIcon.Image = this.GetBitmap(_icon);

                if (oldImage != null)
                    oldImage.Dispose();
            }
        }

        [Browsable(true)]
        public override string Text
        {
            get { return labelText.Text; }
            set { labelText.Text = value; }
        }

        private Bitmap GetBitmap(MessageLabelIcon icon)
        {
            if (icon == MessageLabelIcon.None)
                return null;

            IntPtr iconHandle;

            Win32.LoadIconMetric(IntPtr.Zero, new IntPtr((int)icon), 0, out iconHandle);

            try
            {
                return Utils.ToBitmap(iconHandle, 16, 16);
            }
            finally
            {
                Win32.DestroyIcon(iconHandle);
            }
        }
    }

    public enum MessageLabelIcon
    {
        None = 0,
        Information = 32516,
        Warning = 32515,
        Error = 32513
    }
}
