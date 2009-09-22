using System;
using System.Collections.Generic;
using System.Text;
using TaskbarLib;
using System.Drawing;
using ProcessHacker;

class TaskbarClass
{
    private static Object syncLock = new Object();

    private static ITaskbarList taskbar = default(ITaskbarList);
    private static ITaskbarList Taskbar
    {
        get  
        {
            if (taskbar == null)
            {  
                lock (syncLock)   
                {   
                    if (taskbar == null)  
                    {  
                        taskbar = (ITaskbarList)new TaskbarList();  
                        taskbar.HrInit();
                    }
                }
            }
            return taskbar;
        }
    }

    private static bool IsSupported = default(bool);
    private static bool IsSupportedOS()
        {
            if (IsSupported != default(bool))
            {
                return IsSupported; // return cached result
            }
            else
            {
                if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1)
                {
                    IsSupported = true;
                    return IsSupported; //cache result
                }
                else
                {
                    IsSupported = false;
                    return IsSupported; //cache result
                }
            }
        }


    public static void AddTab(IntPtr hwnd)
    {   
        if (TaskbarClass.IsSupportedOS())  
        {  
            Taskbar.AddTab(hwnd);
        }
    }

    public static void DeleteTab(IntPtr hwnd)
    {
        if (TaskbarClass.IsSupportedOS())
        {
            Taskbar.DeleteTab(hwnd);
        }
    }
   
    public static void ActivateTab(IntPtr hwnd)
    {
        if (TaskbarClass.IsSupportedOS())
        {  
            Taskbar.ActivateTab(hwnd);
        }
    }

    public static void SetActivateAlt(IntPtr hwnd)
    {
        if (TaskbarClass.IsSupportedOS()) 
        {
            Taskbar.SetActiveAlt(hwnd);
        }
    }
    
    public static void MarkFullscreenWindow(IntPtr hwnd, bool fullscreen)
    { 
        if (TaskbarClass.IsSupportedOS()) 
        {
            Taskbar.MarkFullscreenWindow(hwnd, fullscreen);
        }
    }
 
    public static void SetProgressValue(ulong completed, ulong total)
    {
        if (TaskbarClass.IsSupportedOS())
        {
            Taskbar.SetProgressValue(Program.HackerWindowHandle, completed, total);
        }   
    }
 
    public static void SetProgressState(TBPFlag flags)
    {  
        if (TaskbarClass.IsSupportedOS())
        {
            Taskbar.SetProgressState(Program.HackerWindowHandle, flags);
        }
    }

    public static void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI)
    {   
        if (TaskbarClass.IsSupportedOS())
        {
            Taskbar.RegisterTab(hwndTab, hwndMDI);
        }
    }
 
    public static void UnregisterTab(IntPtr hwndTab)
    { 
        if (TaskbarClass.IsSupportedOS())
        {
            Taskbar.UnregisterTab(hwndTab);
        }
    }
      
    public static void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore)
    { 
        if (TaskbarClass.IsSupportedOS()) 
        {  
            Taskbar.SetTabOrder(hwndTab, hwndInsertBefore);
        }
    }
  
    public static void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, TBATFLAG tbatFlags)
    {
        if (TaskbarClass.IsSupportedOS())
        { 
            Taskbar.SetTabActive(hwndTab, hwndInsertBefore, tbatFlags); 
        }
    }

    public static HRESULT ThumbBarAddButtons(IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButton)
    {
        if (TaskbarClass.IsSupportedOS())
        {
            return Taskbar.ThumbBarAddButtons(hwnd, cButtons, pButton);
        }
        else
        {
            return HRESULT.S_OK;
        }
    }

    public static HRESULT ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButton)
    { 
        if (TaskbarClass.IsSupportedOS())  
        {
            return Taskbar.ThumbBarUpdateButtons(hwnd, cButtons, pButton); 
        }
        else
        {
            return HRESULT.S_OK;
        }
    }
  
    public static void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl)
    {  
        if (TaskbarClass.IsSupportedOS()) 
        {
            Taskbar.ThumbBarSetImageList(hwnd, himl);
        }
    }

    public static void SetOverlayIcon(Icon icon, string pszDescription) 
    { 
        if (TaskbarClass.IsSupportedOS())  
        {
            Taskbar.SetOverlayIcon(Program.HackerWindowHandle, icon.Handle, pszDescription);            
        }
    }
  
    public static void SetThumbnailTooltip(IntPtr hwnd, string pszTip)
    { 
        if (TaskbarClass.IsSupportedOS()) 
        {
            Taskbar.SetThumbnailTooltip(hwnd, pszTip);
        }
    }
 
    public static void SetThumbnailClip(IntPtr hwnd, ref RECT prcClip)
    {   
        if (TaskbarClass.IsSupportedOS())   
        {  
            Taskbar.SetThumbnailClip(hwnd, ref prcClip);
        }
    }

    /// <summary>
    /// HRESULT - Succeeded
    /// </summary>
    /// <param name="hresult">The error code.</param>
    /// <returns>True if the error code indicates success.</returns>
    public static bool Succeeded(HRESULT hresult)
    {
        return ((int)hresult >= 0);
    }

    /// <summary>
    /// HRESULT - Failed
    /// </summary>
    /// <param name="hResult">The error code.</param>
    /// <returns>True if the error code indicates failure.</returns>
    public static bool Failed(HRESULT hResult)
    {
        return ((int)hResult < 0);
    }
}

