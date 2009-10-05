// Supports two blog entries by Daniel Moth:
// http://www.danielmoth.com/Blog/2007/01/treeviewvista.html
//  AND
// http://www.danielmoth.com/Blog/2006/12/tvsexautohscroll.html

using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using System;

public class VistaTreeView : System.Windows.Forms.TreeView
{
    public const int TV_FIRST = 0x1100;
    public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
    public const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;
    public const int TVM_SETAUTOSCROLLINFO = TV_FIRST + 59;
    public const int TVS_EX_AUTOHSCROLL = 0x0020;
    public const int TVS_EX_FADEINOUTEXPANDOS = 0x0040;
    public const int GWL_STYLE = -16;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    protected override void OnHandleCreated(System.EventArgs e)
    {
        base.OnHandleCreated(e);

        // get style
        int dw = SendMessage(this.Handle, TVM_GETEXTENDEDSTYLE, 0, 0);

        // Update style
        dw |= TVS_EX_AUTOHSCROLL;       // autoscroll horizontaly
        dw |= TVS_EX_FADEINOUTEXPANDOS; // auto hide the +/- signs

        // set style
        Win32.SendMessage(this.Handle, (WindowMessage)TVM_SETEXTENDEDSTYLE, 0, dw);

        // little black/empty arrows and blue highlight on treenodes
        Win32.SetWindowTheme(this.Handle, "explorer", null);
    }
}