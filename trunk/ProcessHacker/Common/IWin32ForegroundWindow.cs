/*
 * Process Hacker - 
 *   ThreadSafe IWin32Window for obtaining ForegroundWindow Handle
 * 
 * Copyright (C) 2009 dmex
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
  
public class IWin32ForegroundWindow : IWin32Window
{   
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
