using System.Runtime.InteropServices;
using System;
using ProcessHacker.Native.Api;

namespace TaskbarLib
{
    public class TaskbarNative
    {
        [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterfaceAttribute(ClassInterfaceType.None)]
        [ComImportAttribute()]
        public class TaskbarList
        { }

        /// <summary>
        /// ITaskbarList COM Interface
        /// </summary>
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C43DC798-95D1-4BEA-9030-BB99E2983A1A")]
        public interface ITaskbarList
        {
            #region "ITaskbarList"

            /// <summary>
            /// Initializes the taskbar list object. 
            /// This method must be called before any other ITaskbarList methods can be called. 
            /// </summary>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error HrInit();

            /// <summary>
            /// Adds an item to the taskbar. 
            /// </summary>
            /// <param name="hwnd">A handle to the window to be added to the taskbar.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error AddTab(IntPtr hwnd);

            /// <summary>
            /// Deletes an item from the taskbar.
            /// </summary>
            /// <param name="hwnd">A handle to the window to be deleted from the taskbar.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error DeleteTab(IntPtr hwnd);

            /// <summary>
            /// Activates an item on the taskbar. The window is not actually activated;
            /// the window's item on the taskbar is merely displayed as active. 
            /// </summary>
            /// <param name="hwnd">A handle to the window on the taskbar to be displayed as active.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error ActivateTab(IntPtr hwnd);

            /// <summary>
            /// Marks a taskbar item as active but does not visually activate it.
            /// </summary>
            /// <remarks>
            /// SetActiveAlt marks the item associated with hwnd as the currently active 
            /// item for the window's process without changing the pressed state of any item. 
            /// Any user action that would activate a different tab in that process will 
            /// activate the tab associated with hwnd instead. The active state of the 
            /// window's item is not guaranteed to be preserved when the process associated 
            /// with hwnd is not active. To ensure that a given tab is always active, 
            /// call SetActiveAlt whenever any of your windows are activated.
            /// Calling SetActiveAlt with a NULL hwnd clears this state. 
            /// </remarks>
            /// <param name="hwnd">A handle to the window to be marked as active.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetActiveAlt(IntPtr hwnd);

            #endregion

            #region "ITaskbarList2"

            /// <summary>
            /// Marks a window as full-screen.
            /// </summary>
            /// <remarks>
            /// Setting the value of fFullscreen to TRUE, the Shell treats this window as a full-screen window,
            /// and the taskbar is moved to the bottom of the z-order when this window is active. 
            /// Setting the value of fFullscreen to FALSE removes the full-screen marking, 
            /// but does not cause the Shell to treat the window as though it were definitely not full-screen. 
            /// With a FALSEfFullscreen value, the Shell depends on its automatic detection facility to specify 
            /// how the window should be treated, possibly still flagging the window as full-screen.
            /// </remarks>
            /// <param name="hwnd">The handle of the window to be marked.</param>
            /// <param name="fFullscreen">A Boolean value marking the desired full-screen status of the window.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            #endregion

            #region "ITaskbarList3"

            /// <summary>
            /// Displays or updates a progress bar hosted in a taskbar button to show the specific percentage completed of the full operation.
            /// </summary>
            /// <param name="hwnd">The handle of the window whose associated taskbar button is being used as a progress indicator.</param>
            /// <param name="completed">An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.</param>
            /// <param name="total">An application-defined value that specifies the value ullCompleted  will have when the operation is complete</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetProgressValue(IntPtr hwnd, UInt64 completed, UInt64 total);

            /// <summary>
            /// Sets the type and state of the progress indicator displayed on a taskbar button.
            /// </summary>
            /// <param name="hwnd">The handle of the window in which the progress of an operation is being shown.
            /// This window's associated taskbar button will display the progress bar.</param>
            /// <param name="tbpFlags">Flags that control the current state of the progress button.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetProgressState(IntPtr hwnd, TBPFlag tbpFlags);

            /// <summary>
            /// Informs the taskbar that a new tab or document thumbnail has been provided for display in an application's taskbar group flyout.
            /// </summary>
            /// <param name="hwndTab">Handle of the tab or document window. This value is required and cannot be NULL.</param>
            /// <param name="hwndMDI">Handle of the application's main window. This value tells the taskbar which application's preview group to attach the new thumbnail to. This value is required and cannot be NULL.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);

            /// <summary>
            /// Removes a thumbnail from an application's preview group when that tab or document is closed in the application.
            /// </summary>
            /// <param name="hwndTab">The handle of the tab window whose thumbnail is being removed. 
            /// This is the same value with which the thumbnail was registered as part the group through ITaskbarList3::RegisterTab. 
            /// This value is required and cannot be NULL.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error UnregisterTab(IntPtr hwndTab);

            /// <summary>
            /// Inserts a new thumbnail into a tabbed-document interface (TDI) or multiple-document interface (MDI) application's group flyout or moves an existing thumbnail to a new position in the application's group.
            /// </summary>
            /// <param name="hwndTab">The handle of the tab window whose thumbnail is being placed. This value is required, must already be registered through ITaskbarList3::RegisterTab, and cannot be NULL.</param>
            /// <param name="hwndInsertBefore">The handle of the tab window whose thumbnail that hwndTab is inserted to the left of. This handle must already be registered through ITaskbarList3::RegisterTab. If this value is NULL, the new thumbnail is added to the end of the list.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

            /// <summary>
            /// Informs the taskbar that a tab or document window has been made the active window.
            /// </summary>
            /// <param name="hwndTab">Handle of the active tab window. This handle must already be registered through ITaskbarList3::RegisterTab. This value can be NULL if no tab is active.</param>
            /// <param name="hwndInsertBefore">Handle of the application's main window. This value tells the taskbar which group the thumbnail is a member of. This value is required and cannot be NULL.</param>
            /// <param name="dwReserved">Reserved</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, TBATFlag dwReserved);

            /// <summary>
            /// Adds a thumbnail toolbar with a specified set of buttons to the thumbnail image of a window in a taskbar button flyout.
            /// </summary>
            /// <param name="hwnd">The handle of the window whose thumbnail representation will receive the toolbar. This handle must belong to the calling process.</param>
            /// <param name="cButtons">The number of buttons defined in the array pointed to by pButton. The maximum number of buttons allowed is 7.</param>
            /// <param name="pButtons"> A pointer to an array of THUMBBUTTON  structures. Each THUMBBUTTON defines an individual button to be added to the toolbar. Buttons cannot be added or deleted later, so this must be the full defined set. Buttons also cannot be reordered, so their order in the array, which is the order in which they are displayed left to right, will be their permanent order.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise, including the following:
            /// E_INVALIDARG - The hwnd parameter does not specify a handle that belongs to the process or does not specify a window that is associated with a taskbar button. This value is also returned if pButton is less than 1 or greater than 7.</returns>
            [PreserveSig]
            Win32Error ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);

            /// <summary>
            /// Shows, enables, disables, or hides buttons in a thumbnail toolbar as required by the window's current state. A thumbnail toolbar is a toolbar embedded in a thumbnail image of a window in a taskbar button flyout.
            /// </summary>
            /// <remarks>Because there is a limited amount of space in which to display thumbnails, 
            /// as well as a constantly changing number of thumbnails to display, applications are not guaranteed a specific toolbar size.
            /// If display space is low, buttons in the toolbar are truncated from right to left as needed. 
            /// Therefore, an application should prioritize the commands associated with its buttons to ensure that those 
            /// of highest priority are to the left and are therefore least likely to be truncated.
            /// Thumbnail toolbars are displayed only when thumbnails are being displayed on the taskbar. 
            /// For instance, if a taskbar button represents a group with more open windows than there is room to display thumbnails for, 
            /// the user interface (UI) reverts to a legacy menu rather than thumbnails.</remarks>
            /// <param name="hwnd">The handle of the window whose thumbnail representation contains the toolbar.</param>
            /// <param name="cButtons">The number of buttons defined in the array pointed to by pButton. The maximum number of buttons allowed is 7. This array contains only structures that represent existing buttons that are being updated.</param>
            /// <param name="pButtons"> A pointer to an array of THUMBBUTTON  structures. Each THUMBBUTTON defines an individual button. If the button already exists (the iId value is already defined), then that existing button is updated with the information provided in the structure.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);

            /// <summary>
            /// Specifies an image list that contains button images for a toolbar embedded in a thumbnail image of a window in a taskbar button flyout.
            /// </summary>
            /// <param name="hwnd">The handle of the window whose thumbnail representation contains the toolbar to be updated. This handle must belong to the calling process.</param>
            /// <param name="himl">The handle of the image list that contains all button images to be used in the toolbar.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);

            /// <summary>
            /// Applies an overlay to a taskbar button to indicate application status or a notification to the user.
            /// </summary>
            /// <param name="hwnd">The handle of the window whose associated taskbar button receives the overlay. This handle must belong to a calling process associated with the button's application and must be a valid HWND or the call is ignored.</param>
            /// <param name="hIcon"> The handle of an icon to use as the overlay. This should be a small icon, measuring 16x16 pixels at 96 dots per inch (dpi). If an overlay icon is already applied to the taskbar button, that existing overlay is replaced.
            /// This value can be NULL. How a NULL value is handled depends on whether the taskbar button represents a single window or a group of windows.
            /// *If the taskbar button represents a single window, the overlay icon is removed from the display.
            /// *If the taskbar button represents a group of windows and a previous overlay is still available (received earlier than the current overlay, but not yet freed by a NULL value), then that previous overlay is displayed in place of the current overlay.
            /// It is the responsibility of the calling application to free hIcon when it is no longer needed. This can generally be done after you've called SetOverlayIcon because the taskbar makes and uses its own copy of the icon.</param>
            /// <param name="description">A pointer to a string that provides an alt text version of the information conveyed by the overlay, for accessibility purposes.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string description);

            /// <summary>
            /// Specifies or updates the text of the tooltip that is displayed when the mouse pointer rests on an individual preview thumbnail in a taskbar button flyout.
            /// </summary>
            /// <param name="hwnd">The handle to the window whose thumbnail displays the tooltip. This handle must belong to the calling process.</param>
            /// <param name="tip">The pointer to the text to be displayed in the tooltip. This value can be NULL, in which case the title of the window specified by hwnd  is used as the tooltip.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string tip);

            /// <summary>
            /// Selects a portion of a window's client area to display as that window's thumbnail in the taskbar.
            /// </summary>
            /// <param name="hwnd">The handle to a window represented in the taskbar.</param>
            /// <param name="prcClip">A pointer to a RECT  structure that specifies a selection within the window's client area, relative to the upper-left corner of that client area. To clear a clip that is already in place and return to the default display of the thumbnail, set this parameter to NULL.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            [PreserveSig]
            Win32Error SetThumbnailClip(IntPtr hwnd, ref ProcessHacker.Native.Api.Rect prcClip);

            #endregion

            #region "ITaskbarList4"

            /// <summary>
            /// Allows a tab to specify whether the main application frame window or the tab window should be used as a thumbnail or in the peek feature under certain circumstances.
            /// </summary>
            /// <param name="hwndTab">The handle of the tab window that is to have properties set. This handle must already be registered through ITaskbarList3::RegisterTab.</param>
            /// <param name="stpFlags">One or more members of the STPFLAG enumeration that specify the displayed thumbnail and peek image source of the tab thumbnail.</param>
            /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
            Win32Error SetTabProperties(IntPtr hwndTab, STPFlag stpFlags);

            #endregion
        }

        [Flags()]
        public enum TBATFlag
        {
            USEMDITHUMBNAIL = 1,
            USEMDILIVEPREVIEW = 2
        }
        [Flags()]
        public enum TBPFlag
        {
            NOPROGRESS = 0,
            INDETERMINATE = 1,
            NORMAL = 2,
            ERROR = 4,
            PAUSED = 8
        }
        [Flags()]
        public enum STPFlag
        {
            NONE = 0,
            USEAPPTHUMBNAILALWAYS = 1,
            USEAPPTHUMBNAILWHENACTIVE = 2,
            USEAPPPEEKALWAYS = 4,
            USEAPPPEEKWHENACTIVE = 8,
        }
        [Flags()]
        public enum THBMask
        {
            BITMAP = 0x1,
            ICON = 0x2,
            TOOLTIP = 0x4,
            FLAGS = 0x8
        }
        [Flags()]
        public enum THBFlags
        {
            ENABLED = 0x00000000,
            DISABLED = 0x00000001,
            DISMISSONCLICK = 0x00000002,
            NOBACKGROUND = 0x00000004,
            HIDDEN = 0x00000008,
            NONINTERACTIVE = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
        public struct THUMBBUTTON
        {
            //WPARAM value for a THUMBBUTTON being clicked.
            internal const int THBN_CLICKED = 0x1800;

            [MarshalAs(UnmanagedType.U4)]
            internal THBMask dwMask;
            internal uint iId;
            internal uint iBitmap;
            internal IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 259)]
            internal string szTip;
            [MarshalAs(UnmanagedType.U4)]
            internal THBFlags dwFlags;
        }

    }
}