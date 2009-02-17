using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace wyDay.Controls
{
    public partial class VistaMenu
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        ContainerControl ownerForm;

        //conditionally draw the little lines under menu items with keyboard accelators on Win 2000+
        private bool isUsingKeyboardAccel;


        public VistaMenu(ContainerControl parentControl)
            : this()
        {
            ownerForm = parentControl;
        }
        public ContainerControl ContainerControl
        {
            get { return ownerForm; }
            set { ownerForm = value; }
        }
        public override ISite Site
        {
            set
            {
                // Runs at design time, ensures designer initializes ContainerControl
                base.Site = value;
                if (value == null) return;
                IDesignerHost service = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (service == null) return;
                IComponent rootComponent = service.RootComponent;
                ContainerControl = rootComponent as ContainerControl;
            }
        }


        void PreVistaMenuItem_Popup(object sender, EventArgs e)
        {
            if (ownerForm == null)
            {
                isUsingKeyboardAccel = true;
                return;
            }


            //#define WM_QUERYUISTATE                 0x0129
            //int ret = SendMessage(new HandleRef(((Menu)sender).GetMainMenu().GetForm(), ((Menu)sender).GetMainMenu().GetForm().Handle), 0x0129, IntPtr.Zero, IntPtr.Zero);
            int ret = SendMessage(new HandleRef(ownerForm, ownerForm.Handle), 0x0129, IntPtr.Zero, IntPtr.Zero);


            /*
             The return value is NULL if the focus indicators and the keyboard accelerators are visible.
             Otherwise, the return value can be one or more of the following values:
             
                UISF_HIDEFOCUS	Focus indicators are hidden.
                UISF_HIDEACCEL	Keyboard accelerators are hidden.
                UISF_ACTIVE	Windows XP: A control should be drawn in the style used for active controls.
             */

            //#define UISF_HIDEACCEL                  0x2

            isUsingKeyboardAccel = (ret & 0x2) == 0;
        }


        const int SEPARATOR_HEIGHT = 9;
        const int BORDER_VERTICAL = 4;
        const int LEFT_MARGIN = 4;
        const int RIGHT_MARGIN = 6;
        const int SHORTCUT_MARGIN = 20;
        const int ARROW_MARGIN = 12;
        const int ICON_SIZE = 16;


        static void MenuItem_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            Font font = ((MenuItem)sender).DefaultItem
                            ? new Font(SystemFonts.MenuFont, FontStyle.Bold)
                            : SystemFonts.MenuFont;

            if (((MenuItem)sender).Text == "-")
                e.ItemHeight = SEPARATOR_HEIGHT;
            else
            {
                e.ItemHeight = ((SystemFonts.MenuFont.Height > ICON_SIZE) ? SystemFonts.MenuFont.Height : ICON_SIZE)
                                + BORDER_VERTICAL;

                e.ItemWidth = LEFT_MARGIN + ICON_SIZE + RIGHT_MARGIN

                    //item text width
                    + TextRenderer.MeasureText(((MenuItem)sender).Text, font, new Size(0, 0), TextFormatFlags.SingleLine).Width
                    + SHORTCUT_MARGIN

                    //shortcut text width
                    + TextRenderer.MeasureText(ShortcutToString(((MenuItem)sender).Shortcut), font, new Size(0, 0), TextFormatFlags.SingleLine).Width

                    //arrow width
                    + ((((MenuItem)sender).IsParent) ? ARROW_MARGIN : 0);
            }
        }

        void MenuItem_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.InterpolationMode = InterpolationMode.Low;

            bool menuSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            if (menuSelected)
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            else
                e.Graphics.FillRectangle(SystemBrushes.Menu, e.Bounds);

            if (((MenuItem)sender).Text == "-")
            {
                //draw the separator
                int yCenter = e.Bounds.Top + (e.Bounds.Height / 2) - 1;

                e.Graphics.DrawLine(SystemPens.ControlDark, e.Bounds.Left + 1, yCenter, (e.Bounds.Left + e.Bounds.Width - 2), yCenter);
                e.Graphics.DrawLine(SystemPens.ControlLightLight, e.Bounds.Left + 1, yCenter + 1, (e.Bounds.Left + e.Bounds.Width - 2), yCenter + 1);
            }
            else //regular menu items
            {
                //draw the item text
                DrawText(sender, e, menuSelected);

                if (((MenuItem)sender).Checked)
                {
                    if (((MenuItem)sender).RadioCheck)
                    {
                        //draw the bullet
                        ControlPaint.DrawMenuGlyph(e.Graphics,
                            e.Bounds.Left + (LEFT_MARGIN + ICON_SIZE + RIGHT_MARGIN - SystemInformation.MenuCheckSize.Width) / 2,
                            e.Bounds.Top + (e.Bounds.Height - SystemInformation.MenuCheckSize.Height) / 2 + 1,
                            SystemInformation.MenuCheckSize.Width,
                            SystemInformation.MenuCheckSize.Height,
                            MenuGlyph.Bullet,
                            menuSelected ? SystemColors.HighlightText : SystemColors.MenuText,
                            menuSelected ? SystemColors.Highlight : SystemColors.Menu);
                    }
                    else
                    {
                        //draw the check mark
                        ControlPaint.DrawMenuGlyph(e.Graphics,
                            e.Bounds.Left + (LEFT_MARGIN + ICON_SIZE + RIGHT_MARGIN - SystemInformation.MenuCheckSize.Width) / 2,
                            e.Bounds.Top + (e.Bounds.Height - SystemInformation.MenuCheckSize.Height) / 2 + 1,
                            SystemInformation.MenuCheckSize.Width,
                            SystemInformation.MenuCheckSize.Height,
                            MenuGlyph.Checkmark,
                            menuSelected ? SystemColors.HighlightText : SystemColors.MenuText,
                            menuSelected ? SystemColors.Highlight : SystemColors.Menu);
                    }
                }
                else
                {
                    Image drawImg = GetImage(((MenuItem)sender));

                    if (drawImg != null)
                    {
                        //draw the image
                        if (((MenuItem)sender).Enabled)
                            e.Graphics.DrawImage(drawImg, e.Bounds.Left + LEFT_MARGIN,
                                e.Bounds.Top + ((e.Bounds.Height - ICON_SIZE) / 2),
                                ICON_SIZE, ICON_SIZE);
                        else
                            ControlPaint.DrawImageDisabled(e.Graphics, drawImg,
                                e.Bounds.Left + LEFT_MARGIN,
                                e.Bounds.Top + ((e.Bounds.Height - ICON_SIZE) / 2),
                                SystemColors.Menu);
                    }
                }
            }
        }


        private static string ShortcutToString(Shortcut shortcut)
        {
            if (shortcut != Shortcut.None)
            {
                Keys keys = (Keys)shortcut;
                return TypeDescriptor.GetConverter(keys.GetType()).ConvertToString(keys);
            }

            return null;
        }

        private void DrawText(object sender, DrawItemEventArgs e, bool isSelected)
        {
            string shortcutText = ShortcutToString(((MenuItem)sender).Shortcut);

            int yPos = e.Bounds.Top + (e.Bounds.Height - SystemFonts.MenuFont.Height) / 2;

            Size textSize = TextRenderer.MeasureText(((MenuItem)sender).Text, 
                ((MenuItem)sender).DefaultItem ? 
                new Font(e.Font, FontStyle.Bold) : 
                e.Font, 
                new Size(0, 0), TextFormatFlags.SingleLine);

            Rectangle textRect = new Rectangle(e.Bounds.Left + LEFT_MARGIN + ICON_SIZE + RIGHT_MARGIN, yPos,
                                   textSize.Width, textSize.Height);

            if (!((MenuItem)sender).Enabled && !isSelected) // disabled and not selected
            {
                textRect.Offset(1, 1);

                TextRenderer.DrawText(e.Graphics, ((MenuItem)sender).Text, e.Font,
                    textRect,
                    SystemColors.ControlLightLight,
                    TextFormatFlags.SingleLine | (isUsingKeyboardAccel ? 0 : TextFormatFlags.HidePrefix));

                textRect.Offset(-1, -1);
            }

            //Draw the menu item text
            TextRenderer.DrawText(e.Graphics, ((MenuItem)sender).Text, e.Font,
                textRect,
                ((MenuItem)sender).Enabled ? (isSelected ? SystemColors.HighlightText : SystemColors.MenuText) : SystemColors.GrayText,
                TextFormatFlags.SingleLine | (isUsingKeyboardAccel ? 0 : TextFormatFlags.HidePrefix));



            //Draw the shortcut text
            if (shortcutText != null)
            {
                textSize = TextRenderer.MeasureText(shortcutText, e.Font, Size.Empty, TextFormatFlags.SingleLine);
                textRect = new Rectangle(e.Bounds.Width - textSize.Width - ARROW_MARGIN, yPos, textSize.Width,
                                         textSize.Height);

                if (!((MenuItem)sender).Enabled && !isSelected) // disabled and not selected
                {
                    textRect.Offset(1, 1);

                    TextRenderer.DrawText(e.Graphics, shortcutText, e.Font,
                        textRect,
                        SystemColors.ControlLightLight,
                        TextFormatFlags.SingleLine | (isUsingKeyboardAccel ? 0 : TextFormatFlags.HidePrefix));

                    textRect.Offset(-1, -1);
                }

                TextRenderer.DrawText(e.Graphics, shortcutText, e.Font,
                    textRect,
                    ((MenuItem)sender).Enabled ? (isSelected ? SystemColors.HighlightText : SystemColors.MenuText) : SystemColors.GrayText,
                    TextFormatFlags.SingleLine);
            }
        }
    }
}
