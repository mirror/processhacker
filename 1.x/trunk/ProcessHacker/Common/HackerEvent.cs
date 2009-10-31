using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcessHacker.Components;
using ProcessHacker.Native;
using System.Windows.Forms;
using ProcessHacker.Common;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Aga.Controls.Tree.NodeControls;
using System.Globalization;

namespace ProcessHacker
{
    public delegate void HackerLogUpdatedHandler();

    [Flags]
    public enum EventType : int
    {
        Information = 0,
        Warning = 1,
        Error = 2,
        Exception = 3,
        Debug
    }

    /// <summary>
    ///Process Hacker event logging/TaskDialog management.
    /// </summary>
    public class HackerEvent : IEnumerable
    {
        /// <summary>
        /// The PH internal log.
        /// </summary>
        private List<KeyValuePair<EventType, KeyValuePair<DateTime, String>>> _events = 
            new List<KeyValuePair<EventType, KeyValuePair<DateTime, String>>>();

        /// <summary>
        /// Log updated event notification for new item.
        /// Allows plugins/window querying event records.
        /// </summary>
        public static event HackerLogUpdatedHandler HackerLogUpdated;

        private static HackerEvent log = null;
        public static HackerEvent Log
        {
            get 
            {
                if (log == null)
                    log = new HackerEvent();

                return log; 
            }
            set
            {
                if (log == null)
                    log = new HackerEvent();

                log = value;

                if (HackerLogUpdated != null)
                    HackerLogUpdated();
            }
        }

        public KeyValuePair<EventType, KeyValuePair<DateTime, String>> this[Int32 index]
        {
            get 
            { 
                return _events[index]; 
            }
            set
            {
                _events[index] = value;

                if (HackerLogUpdated != null)
                    HackerLogUpdated();
            }
        }

        public void Debug(String operation)
        {
            LogBase(EventType.Debug, operation, null);
        }

        public void Info(Boolean show, Boolean log, String operation)
        {
            // __________________________________________________
            // | Process Hacker                                x |
            // |                                                 |
            // |   !     Info Message                            |
            // |                                                 |
            // |                                      | Close |  | 
            // |_________________________________________________|

            if (log)
                LogBase(EventType.Information, operation, null);

            // If HackerWindowHandle.IsGreaterThanZero, TaskDialog COM server for this process has been loaded and
            // its safe to call TaskDialog. this maintains WinMainInit and unsupported OS compatibility handling.

            if (show)
            {   
                if (OSVersion.HasTaskDialogs && Program.HackerWindowHandle.IsGreaterThanZero())
                {
                    TaskDialog td = new TaskDialog();
                    td.AllowDialogCancellation = true;
                    td.PositionRelativeToWindow = true;
                    td.MainIcon = TaskDialogIcon.SecurityInformation;
                    td.CommonButtons = TaskDialogCommonButtons.Close;

                    td.WindowTitle = "Process Hacker";
                    td.Content = operation;

                    td.Show(PhUtils.GetFWindow());
                }
                else
                {
                    MessageBox.Show(PhUtils.GetFWindow(), operation, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void Warn(Boolean show, Boolean log, String operation)
        {
            // __________________________________________________
            // | Process Hacker                                x |
            // |                                                 |
            // |  (!!)   Warning Message                         |
            // |                                                 |
            // |                                      | Close |  | 
            // |_________________________________________________|

            if (log)
                LogBase(EventType.Warning, operation, null);

            // If HackerWindowHandle.IsGreaterThanZero, TaskDialog COM server for this process has been loaded and
            // its safe to call TaskDialog. this maintains WinMainInit and unsupported OS compatibility handling.

            if (show)
            {
                if (OSVersion.HasTaskDialogs && Program.HackerWindowHandle.IsGreaterThanZero())
                {
                    TaskDialog td = new TaskDialog();
                    td.AllowDialogCancellation = true;
                    td.PositionRelativeToWindow = true;
                    td.MainIcon = TaskDialogIcon.Warning;
                    td.CommonButtons = TaskDialogCommonButtons.Close;

                    td.WindowTitle = "Process Hacker";
                    td.Content = operation;

                    td.Show(PhUtils.GetFWindow());
                }
                else
                {
                    MessageBox.Show(PhUtils.GetFWindow(), operation, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void Error(Boolean show, Boolean log, String operation)
        {
            // __________________________________________________
            // | Process Hacker                                x |
            // |                                                 |
            // |   X   Error Message                             |
            // |                                                 |
            // |                                      | Close |  | 
            // |_________________________________________________|

            if (log)
                LogBase(EventType.Error, operation, null);

            // If HackerWindowHandle.IsGreaterThanZero, TaskDialog COM server for this process has been loaded and
            // its safe to call TaskDialog. this maintains WinMainInit and unsupported OS compatibility handling.

            if (show)
            {
                if (OSVersion.HasTaskDialogs && Program.HackerWindowHandle.IsGreaterThanZero())
                {
                    TaskDialog td = new TaskDialog();
                    td.AllowDialogCancellation = true;
                    td.PositionRelativeToWindow = true;
                    td.MainIcon = TaskDialogIcon.Error;
                    td.CommonButtons = TaskDialogCommonButtons.Close;

                    td.WindowTitle = string.Format("{0}", "Process Hacker");
                    td.Content = string.Format("{0}", operation);

                    td.Show(PhUtils.GetFWindow());
                }
                else
                {
                    MessageBox.Show(PhUtils.GetFWindow(), string.Format("{0}", operation), "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Ex(Boolean show, Boolean log, String operation, Exception ex)
        {
            // __________________________________________________
            // | Process Hacker                                x |
            // |                                                 |
            // |    /\     Exception Message                     |
            // |   /||\                                          |
            // |  /_||_\   Exception failed                      |
            // |           0x0000000:FAIL!  :P                   |
            // |                                                 |
            // |                                      |  Ok  |   | 
            // |_________________________________________________|

            if(log)
                LogBase(EventType.Exception, operation, ex);

            // If HackerWindowHandle.IsGreaterThanZero, TaskDialog COM server for this process has been loaded and
            // its safe to call TaskDialog. this maintains WinMainInit and unsupported OS compatibility handling.

            if (show)
            {
                if (OSVersion.HasTaskDialogs && Program.HackerWindowHandle.IsGreaterThanZero())
                {
                    ProcessHacker.Components.TaskDialog td = new ProcessHacker.Components.TaskDialog();
                    td.AllowDialogCancellation = true;
                    td.PositionRelativeToWindow = true;
                    td.MainIcon = TaskDialogIcon.ExIcon;
                    td.CommonButtons = TaskDialogCommonButtons.Ok;

                    td.WindowTitle = "Process Hacker";
                    td.MainInstruction = operation;
                    td.Content = ex.Message;

                    td.Show(PhUtils.GetFWindow());
                }
                else
                {
                    MessageBox.Show(
                        PhUtils.GetFWindow(), 
                        PhUtils.FormatException(operation, ex), 
                        "Process Hacker",
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation
                        );
                }
            }
        }

        private void LogBase(EventType msgType, String operation, Exception ex)
        {
            try
            {
                //this is where we filter events based on user preference,
                //only update log if MessageType is lower than LogLevel mode
                if (msgType <= Settings.Instance.AppLogLevel)
                {
                    DateTime baseTime = DateTime.Now;
                    //we use Append/AppendLine and \r\n for splitting the string later
                    StringBuilder msg = new StringBuilder();

                    msg.AppendLine(operation);

                    // debug mode, append exception and stacktrace data 
                    if (Settings.Instance.AppLogLevel == EventType.Debug)
                    {
                        if (ex != null)
                        {
                            msg.Append("\r\nException Data: " + ex.Message);
                            msg.AppendLine("\r\n" + ex.StackTrace);

                            if (ex.InnerException != null)
                            {
                                msg.Append("\r\nInnerException Data: " + ex.InnerException.Message);
                                msg.AppendLine("\r\n" + ex.InnerException.StackTrace);
                            }
                        }

                        //OutputDebugString(msg.ToString());
                    }

                    _events.Add(new KeyValuePair<EventType, KeyValuePair<DateTime, String>>
                        (msgType, new KeyValuePair<DateTime, String>(baseTime, msg.ToString()))
                        );

                    if (HackerLogUpdated != null)
                        HackerLogUpdated();
                }
            }
            catch (Exception)
            {
                //OutputDebugString(exx.ToString());
            }
        }


        private void Add(EventType msgType, String operation)
        {
            _events.Add(new KeyValuePair<EventType, KeyValuePair<DateTime, String>>
                (msgType, new KeyValuePair<DateTime, String>(DateTime.Now, operation))
                );

            if (HackerLogUpdated != null)
                HackerLogUpdated();
        }

        public int Capacity
        {
            get { return _events.Capacity; }
        }

        public void Clear()
        {
            _events.Clear();

            if (HackerLogUpdated != null)
                HackerLogUpdated();
        }

        public int Count
        {
            get { return _events.Count; }
        }

        public void Refresh()
        {
        }

        public void Remove(Int32 index)
        {
            _events.RemoveAt(index);

            if (HackerLogUpdated != null)
                HackerLogUpdated();
        }

        #region Enumerable Members

        public Array ToArray()
        {
            return _events.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        #endregion
    }
}