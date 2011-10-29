using System.Linq;

namespace System
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    public enum ToolbarTheme
    {
        Toolbar,
        Black,
        Blue,
        BrowserTabBar,
        HelpBar
    }

    /// <summary>Renders a toolstrip using the UxTheme API via VisualStyleRenderer using specific styles.</summary>
    /// <remarks>Special thanks to Szotar and Lee.</remarks>
    public class AeroRenderer : ToolStripSystemRenderer
    {
        private VisualStyleRenderer Renderer;
        private readonly ToolbarTheme Theme;

        const int RebarBackground = 6;

        public AeroRenderer(ToolbarTheme theme)
        {
            this.Theme = theme;
        }

        /// <summary>
        /// It shouldn't be necessary to P/Invoke like this, however VisualStyleRenderer.GetMargins misses out a parameter in its own P/Invoke.
        /// </summary>
        internal static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Margins
            {
                public int cxLeftWidth;
                public int cxRightWidth;
                public int cyTopHeight;
                public int cyBottomHeight;
            }

            [DllImport("uxtheme.dll"), Security.SuppressUnmanagedCodeSecurity]
            public extern static int GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, IntPtr rect, out Margins pMargins);
        }

        Padding GetThemeMargins(IDeviceContext dc, MarginTypes marginType)
        {
            NativeMethods.Margins margins;
            try
            {
                IntPtr hDc = dc.GetHdc();

                if (NativeMethods.GetThemeMargins(Renderer.Handle, hDc, Renderer.Part, Renderer.State, (int)marginType, IntPtr.Zero, out margins) == 0)
                {
                    return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
                }

                return new Padding(0);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        // See http://msdn2.microsoft.com/en-us/library/bb773210.aspx - "Parts and States"
        // Only menu-related parts/states are needed here, VisualStyleRenderer handles most of the rest.
        private enum MenuParts
        {
            ItemTMSchema = 1,
            DropDownTMSchema = 2,
            BarItemTMSchema = 3,
            BarDropDownTMSchema = 4,
            ChevronTMSchema = 5,
            SeparatorTMSchema = 6,
            BarBackground = 7,
            BarItem = 8,
            PopupBackground = 9,
            PopupBorders = 10,
            PopupCheck = 11,
            PopupCheckBackground = 12,
            PopupGutter = 13,
            PopupItem = 14,
            PopupSeparator = 15,
            PopupSubmenu = 16,
            SystemClose = 17,
            SystemMaximize = 18,
            SystemMinimize = 19,
            SystemRestore = 20
        }

        private enum MenuBarStates
        {
            Active = 1,
            Inactive = 2
        }

        private enum MenuBarItemStates
        {
            Normal = 1,
            Hover = 2,
            Pushed = 3,
            Disabled = 4,
            DisabledHover = 5,
            DisabledPushed = 6
        }

        private enum MenuPopupItemStates
        {
            Normal = 1,
            Hover = 2,
            Disabled = 3,
            DisabledHover = 4
        }

        private enum MenuPopupCheckStates
        {
            CheckmarkNormal = 1,
            CheckmarkDisabled = 2,
            BulletNormal = 3,
            BulletDisabled = 4
        }

        private enum MenuPopupCheckBackgroundStates
        {
            Disabled = 1,
            Normal = 2,
            Bitmap = 3
        }

        private enum MenuPopupSubMenuStates
        {
            Normal = 1,
            Disabled = 2
        }

        private enum MarginTypes
        {
            Sizing = 3601,
            Content = 3602,
            Caption = 3603
        }

        private static int GetItemState(ToolStripItem item)
        {
            bool hot = item.Selected;

            if (item.IsOnDropDown)
            {
                if (item.Enabled)
                    return hot ? (int)MenuPopupItemStates.Hover : (int)MenuPopupItemStates.Normal;

                return hot ? (int)MenuPopupItemStates.DisabledHover : (int)MenuPopupItemStates.Disabled;
            }

            if (item.Pressed)
                return item.Enabled ? (int)MenuBarItemStates.Pushed : (int)MenuBarItemStates.DisabledPushed;
            
            if (item.Enabled)
                return hot ? (int)MenuBarItemStates.Hover : (int)MenuBarItemStates.Normal;

            return hot ? (int)MenuBarItemStates.DisabledHover : (int)MenuBarItemStates.Disabled;
        }

        private string RebarClass
        {
            get { return this.SubclassPrefix + "Rebar"; }
        }

        private string ToolbarClass
        {
            get { return this.SubclassPrefix + "ToolBar"; }
        }

        private string MenuClass
        {
            get { return this.SubclassPrefix + "Menu"; }
        }

        private string SubclassPrefix
        {
            get
            {
                switch (this.Theme)
                {
                    case ToolbarTheme.Black: 
                        return "Media::";
                    case ToolbarTheme.Blue: 
                        return "Communications::";
                    case ToolbarTheme.BrowserTabBar: 
                        return "BrowserTabBar::";
                    case ToolbarTheme.HelpBar: 
                        return "Help::";
                    default: 
                        return string.Empty;
                }
            }
        }

        private VisualStyleElement Subclass(VisualStyleElement element)
        {
            return VisualStyleElement.CreateElement(SubclassPrefix + element.ClassName, element.Part, element.State);
        }

        private bool EnsureRenderer()
        {
            if (!this.IsSupported)
                return false;

            if (this.Renderer == null)
                this.Renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);

            return true;
        }

        // Gives parented ToolStrips a transparent background.
        protected override void Initialize(ToolStrip toolStrip)
        {
            if (toolStrip.Parent is ToolStripPanel)
                toolStrip.BackColor = Color.Transparent;

            base.Initialize(toolStrip);
        }

        // Using just ToolStripManager.Renderer without setting the Renderer individually per ToolStrip means
        // that the ToolStrip is not passed to the Initialize method. ToolStripPanels, however, are. So we can 
        // simply initialize it here too, and this should guarantee that the ToolStrip is initialized at least 
        // once. Hopefully it isn't any more complicated than this.
        protected override void InitializePanel(ToolStripPanel toolStripPanel)
        {
            foreach (ToolStrip c in toolStripPanel.Controls.OfType<ToolStrip>())
            {
                this.Initialize(c);
            }

            base.InitializePanel(toolStripPanel);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                this.Renderer.SetParameters(this.MenuClass, (int)MenuParts.PopupBorders, 0);

                if (e.ToolStrip.IsDropDown)
                {
                    Region oldClip = e.Graphics.Clip;

                    // Tool strip borders are rendered *after* the content, for some reason.
                    // So we have to exclude the inside of the popup otherwise we'll draw over it.
                    Rectangle insideRect = e.ToolStrip.ClientRectangle;
                    insideRect.Inflate(-1, -1);

                    e.Graphics.ExcludeClip(insideRect);

                    Renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);

                    // Restore the old clip in case the Graphics is used again (does that ever happen?)
                    e.Graphics.Clip = oldClip;
                }
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        Rectangle GetBackgroundRectangle(ToolStripItem item)
        {
            if (!item.IsOnDropDown)
                return new Rectangle(new Point(), item.Bounds.Size);

            // For a drop-down menu item, the background rectangles of the items should be touching vertically.
            // This ensures that's the case.
            Rectangle rect = item.Bounds;

            // The background rectangle should be inset two pixels horizontally (on both sides), but we have 
            // to take into account the border.
            rect.X = item.ContentRectangle.X + 1;
            rect.Width = item.ContentRectangle.Width - 1;

            // Make sure we're using all of the vertical space, so that the edges touch.
            rect.Y = 0;
            return rect;
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                int partId = e.Item.IsOnDropDown ? (int)MenuParts.PopupItem : (int)MenuParts.BarItem;
                this.Renderer.SetParameters(MenuClass, partId, GetItemState(e.Item));

                Rectangle bgRect = this.GetBackgroundRectangle(e.Item);
                this.Renderer.DrawBackground(e.Graphics, bgRect, bgRect);
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                // Draw the background using Rebar & RP_BACKGROUND (or, if that is not available, fall back to Rebar.Band.Normal)
                if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(RebarClass, RebarBackground, 0)))
                {
                    this.Renderer.SetParameters(RebarClass, RebarBackground, 0);
                }
                else
                {
                    this.Renderer.SetParameters(RebarClass, 0, 0);
                }

                if (this.Renderer.IsBackgroundPartiallyTransparent())
                    this.Renderer.DrawParentBackground(e.Graphics, e.ToolStripPanel.ClientRectangle, e.ToolStripPanel);

                this.Renderer.DrawBackground(e.Graphics, e.ToolStripPanel.ClientRectangle);

                e.Handled = true;
            }
            else
            {
                base.OnRenderToolStripPanelBackground(e);
            }
        }

        // Render the background of an actual menu bar, dropdown menu or toolbar.
        protected override void OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                if (e.ToolStrip.IsDropDown)
                {
                    this.Renderer.SetParameters(MenuClass, (int)MenuParts.PopupBackground, 0);
                }
                else
                {
                    // It's a MenuStrip or a ToolStrip. If it's contained inside a larger panel, it should have a
                    // transparent background, showing the panel's background.

                    if (e.ToolStrip.Parent is ToolStripPanel)
                    {
                        // The background should be transparent, because the ToolStripPanel's background will be visible.
                        // Of course, we assume the ToolStripPanel is drawn using the same theme, but it's not my fault
                        // if someone does that.
                        return;
                    }
                    else
                    {
                        // A lone toolbar/menubar should act like it's inside a toolbox, I guess.
                        // Maybe I should use the MenuClass in the case of a MenuStrip, although that would break
                        // the other themes...
                        if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(this.RebarClass, RebarBackground, 0)))
                            this.Renderer.SetParameters(this.RebarClass, RebarBackground, 0);
                        else
                            this.Renderer.SetParameters(this.RebarClass, 0, 0);
                    }
                }

                if (this.Renderer.IsBackgroundPartiallyTransparent())
                    this.Renderer.DrawParentBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.ToolStrip);

                this.Renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);
            }
            else
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        // The only purpose of this override is to change the arrow colour.
        // It's OK to just draw over the default arrow since we also pass down arrow drawing to the system renderer.
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                ToolStripSplitButton sb = e.Item as ToolStripSplitButton;
                base.OnRenderSplitButtonBackground(e);

                // It doesn't matter what colour of arrow we tell it to draw. OnRenderArrow will compute it from the item anyway.
                this.OnRenderArrow(new ToolStripArrowRenderEventArgs(e.Graphics, sb, sb.DropDownButtonBounds, Color.Red, ArrowDirection.Down));
            }
            else
            {
                base.OnRenderSplitButtonBackground(e);
            }
        }

        Color GetItemTextColor(ToolStripItem item)
        {
            int partId = item.IsOnDropDown ? (int)MenuParts.PopupItem : (int)MenuParts.BarItem;

            this.Renderer.SetParameters(MenuClass, partId, GetItemState(item));

            return Renderer.GetColor(ColorProperty.TextColor);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (this.EnsureRenderer())
                e.TextColor = this.GetItemTextColor(e.Item);

            base.OnRenderItemText(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                if (e.ToolStrip.IsDropDown)
                {
                    this.Renderer.SetParameters(this.MenuClass, (int)MenuParts.PopupGutter, 0);
                    // The AffectedBounds is usually too small, way too small to look right. Instead of using that,
                    // use the AffectedBounds but with the right width. Then narrow the rectangle to the correct edge
                    // based on whether or not it's RTL. 
                    //(It doesn't need to be narrowed to an edge in LTR mode, but let's do that anyway.)
                    // Using the DisplayRectangle gets roughly the right size so that the separator is closer to the text.
                   
                    Padding margins = this.GetThemeMargins(e.Graphics, MarginTypes.Sizing);
                    int extraWidth = (e.ToolStrip.Width - e.ToolStrip.DisplayRectangle.Width - margins.Left - margins.Right - 1) - e.AffectedBounds.Width;
                    Rectangle rect = e.AffectedBounds;
                    rect.Y += 2;
                    rect.Height -= 4;
                    int sepWidth = this.Renderer.GetPartSize(e.Graphics, ThemeSizeType.True).Width;
                    if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                    {
                        rect = new Rectangle(rect.X - extraWidth, rect.Y, sepWidth, rect.Height);
                        rect.X += sepWidth;
                    }
                    else
                    {
                        rect = new Rectangle(rect.Width + extraWidth - sepWidth, rect.Y, sepWidth, rect.Height);
                    }
                    this.Renderer.DrawBackground(e.Graphics, rect);
                }
            }
            else
            {
                base.OnRenderImageMargin(e);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (e.ToolStrip.IsDropDown && this.EnsureRenderer())
            {
                this.Renderer.SetParameters(MenuClass, (int)MenuParts.PopupSeparator, 0);
                Rectangle rect = new Rectangle(e.ToolStrip.DisplayRectangle.Left, 0, e.ToolStrip.DisplayRectangle.Width, e.Item.Height);
                this.Renderer.DrawBackground(e.Graphics, rect, rect);
            }
            else
            {
                base.OnRenderSeparator(e);
            }
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                Rectangle bgRect = this.GetBackgroundRectangle(e.Item);
                bgRect.Width = bgRect.Height;

                // Now, mirror its position if the menu item is RTL.
                if (e.Item.RightToLeft == RightToLeft.Yes)
                    bgRect = new Rectangle(e.ToolStrip.ClientSize.Width - bgRect.X - bgRect.Width, bgRect.Y, bgRect.Width, bgRect.Height);

                this.Renderer.SetParameters(this.MenuClass, (int)MenuParts.PopupCheckBackground, e.Item.Enabled ? (int)MenuPopupCheckBackgroundStates.Normal : (int)MenuPopupCheckBackgroundStates.Disabled);
                this.Renderer.DrawBackground(e.Graphics, bgRect);

                Rectangle checkRect = e.ImageRectangle;
                checkRect.X = bgRect.X + bgRect.Width / 2 - checkRect.Width / 2;
                checkRect.Y = bgRect.Y + bgRect.Height / 2 - checkRect.Height / 2;
           
                // I don't think ToolStrip even supports radio box items, so no need to render them.
                var item = e.Item as ToolStripMenuItem;

                if (item.CheckState == CheckState.Indeterminate)
                {
                    this.Renderer.SetParameters(this.MenuClass, (int)MenuParts.PopupCheck, e.Item.Enabled ? (int)MenuPopupCheckStates.BulletNormal : (int)MenuPopupCheckStates.BulletDisabled);
                }
                else
                {
                    this.Renderer.SetParameters(this.MenuClass, (int)MenuParts.PopupCheck, e.Item.Enabled ? (int)MenuPopupCheckStates.CheckmarkNormal : (int)MenuPopupCheckStates.CheckmarkDisabled);
                }

                this.Renderer.DrawBackground(e.Graphics, checkRect);
            }
            else
            {
                base.OnRenderItemCheck(e);
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            // The default renderer will draw an arrow for us (the UXTheme API seems not to have one for all directions),
            // but it will get the colour wrong in many cases. The text colour is probably the best colour to use.
            if (this.EnsureRenderer())
            {
                e.ArrowColor = this.GetItemTextColor(e.Item);
            }

            base.OnRenderArrow(e);
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (this.EnsureRenderer())
            {
                // BrowserTabBar::Rebar draws the chevron using the default background. Odd.
                string rebarClass = this.RebarClass;

                if (this.Theme == ToolbarTheme.BrowserTabBar)
                    rebarClass = "Rebar";

                int state = VisualStyleElement.Rebar.Chevron.Normal.State;

                if (e.Item.Pressed)
                    state = VisualStyleElement.Rebar.Chevron.Pressed.State;
                else if (e.Item.Selected)
                    state = VisualStyleElement.Rebar.Chevron.Hot.State;

                Renderer.SetParameters(rebarClass, VisualStyleElement.Rebar.Chevron.Normal.Part, state);
                Renderer.DrawBackground(e.Graphics, new Rectangle(Point.Empty, e.Item.Size));
            }
            else
            {
                base.OnRenderOverflowButtonBackground(e);
            }
        }

        private bool IsSupported
        {
            get
            {
                // TODO: Needs a more robust check.

                return VisualStyleRenderer.IsSupported && 
                    VisualStyleRenderer.IsElementDefined(
                    VisualStyleElement.CreateElement("Menu", 
                    (int)MenuParts.BarBackground, 
                    (int)MenuBarStates.Active)
                    );
            }
        }
    }
}