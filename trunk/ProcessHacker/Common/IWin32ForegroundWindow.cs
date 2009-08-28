using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
  
public class IWin32ForegroundWindow : IWin32Window
{   
    //ThreadSafe IWin32Window
    private static IWin32ForegroundWindow _window = new IWin32ForegroundWindow();  
    private IWin32ForegroundWindow() { }
   
    public static IWin32Window Instance   
    {    
        get { return _window; }  
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    IntPtr IWin32Window.Handle   
    {  
        get   
        {      
            return GetForegroundWindow();  
        }   
    } 
}
