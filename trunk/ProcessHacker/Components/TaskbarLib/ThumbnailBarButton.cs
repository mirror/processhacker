using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
namespace TaskbarLib
{
    /// <summary>
    /// Represents a thumbnail toolbar button.
    /// </summary>
    public class ThumbnailBarButton : ThumbnailBarButtonBase
    {
        private Bitmap icon;

        private uint imageIndex;

        /// <summary>
        /// Initializes a <see cref="ThumbnailBarButton"/> with a Bitmap icon.
        /// </summary>
        /// <param name="icon">Icon of the button.</param>
        /// <param name="tooltip">Tooltip of the button.</param>
        /// <param name="isHidden">Specifies whether button is hidden initially.</param>
        /// <param name="isDisabled">Specifies whether button is disabled initially.</param>
        /// <param name="isDismissedOnClick">Specifies whether button is dismissed on click.</param>
        /// <param name="hasBackground">Specifies whether button has background.</param>
        public ThumbnailBarButton(Bitmap icon, string tooltip, bool isHidden, bool isDisabled, bool isDismissedOnClick, bool hasBackground)
            : base(tooltip, isHidden, isDisabled, isDismissedOnClick, hasBackground)
        {
            this.icon = icon;
            if (icon != null)
            {
                this.IconHandle = new IconHandle(icon.GetHicon());
            }
        }

        /// <summary>
        /// Initializes a <see cref="ThumbnailBarButton"/> with an icon resides in an <see cref="ImageList"/>.
        /// </summary>
        /// <param name="imageIndex">Index of the image from associated <see cref="System.Windows.Forms.ImageList"/>.</param>
        /// <param name="tooltip">Tooltip of the button.</param>
        /// <param name="isHidden">Specifies whether button is hidden initially.</param>
        /// <param name="isDisabled">Specifies whether button is disabled initially.</param>
        /// <param name="isDismissedOnClick">Specifies whether button is dismissed on click.</param>
        /// <param name="hasBackground">Specifies whether button has background.</param>
        public ThumbnailBarButton(uint imageIndex, string tooltip, bool isHidden, bool isDisabled, bool isDismissedOnClick, bool hasBackground)
            : base(tooltip, isHidden, isDisabled, isDismissedOnClick, hasBackground)
        {
            this.imageIndex = imageIndex;
        }

        /// <summary>
        /// A <see cref="Bitmap"/> icon for button.
        /// </summary>
        public Bitmap Icon
        {
            get
            {
                return this.icon;
            }

            set
            {
                this.icon = value;

                if (value != null)
                {
                    this.IconHandle = new IconHandle(value.GetHicon());
                }

                this.Update();
            }
        }

        /// <summary>
        /// <see cref="System.Windows.Forms.ImageList"/> index of the the image that will be used as button icon.
        /// </summary>
        public uint ImageIndex
        {
            get
            {
                return this.imageIndex;
            }

            set
            {
                this.imageIndex = value;

                this.Update();
            }
        }

        internal override void BeforeGetUnmanagedButton(ref TaskbarClass.THUMBBUTTON button)
        {
            button.iBitmap = this.imageIndex;

            button.dwMask |= TaskbarClass.THBMASK.THB_BITMAP;
        }

        protected override void Update()
        {  }
    }
}