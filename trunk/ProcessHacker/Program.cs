/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace ProcessHacker
{
    static class Program
    {
        /// <summary>
        /// The main Process Hacker window instance
        /// </summary>
        public static HackerWindow HackerWindow;

        /// <summary>
        /// The Results Window ID Generator
        /// </summary>
        public static IdGenerator ResultsIds = new IdGenerator();

        public static Dictionary<string, MemoryEditor> MemoryEditors = new Dictionary<string, MemoryEditor>();
        public static Dictionary<string, Thread> MemoryEditorsThreads = new Dictionary<string, Thread>();
        public static Dictionary<string, ResultsWindow> ResultsWindows = new Dictionary<string, ResultsWindow>();
        public static Dictionary<string, Thread> ResultsThreads = new Dictionary<string, Thread>();
        public static Dictionary<string, ThreadWindow> ThreadWindows = new Dictionary<string, ThreadWindow>();
        public static Dictionary<string, Thread> ThreadThreads = new Dictionary<string, Thread>();

        public delegate void ResultsWindowInvokeAction(ResultsWindow f);
        public delegate void MemoryEditorInvokeAction(MemoryEditor f);
        public delegate void ThreadWindowInvokeAction(ThreadWindow f);
        public delegate void UpdateWindowAction(Form f, List<string> Texts, Dictionary<string, Form> TextToForm);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.Version.Major < 2)
            {
                MessageBox.Show("You must have .NET Framework 2.0 or higher to use Process Hacker.", "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(HackerWindow = new HackerWindow());
        }

        /// <summary>
        /// Creates an instance of the memory editor form.
        /// </summary>
        /// <param name="PID">The PID of the process to edit</param>
        /// <param name="address">The address to start editing at</param>
        /// <param name="length">The length to edit</param>
        /// <returns></returns>
        public static MemoryEditor GetMemoryEditor(int PID, int address, int length)
        {
            return GetMemoryEditor(PID, address, length, new MemoryEditorInvokeAction(delegate {}));
        }

        /// <summary>
        /// Creates an instance of the memory editor form and invokes an action on the memory editor's thread.
        /// </summary>
        /// <param name="PID">The PID of the process to edit</param>
        /// <param name="address">The address to start editing at</param>
        /// <param name="length">The length to edit</param>
        /// <param name="action">The action to be invoked on the memory editor's thread</param>
        /// <returns>Memory editor form</returns>
        public static MemoryEditor GetMemoryEditor(int PID, int address, int length, MemoryEditorInvokeAction action)
        {
            MemoryEditor ed = null;
            string id = PID.ToString() + "-" + address.ToString() + "-" + length.ToString();

            if (MemoryEditors.ContainsKey(id))
            {
                ed = MemoryEditors[id];

                ed.Invoke(new MethodInvoker(delegate { action(ed); }));

                return ed;
            }

            Thread t = new Thread(new ThreadStart(delegate
            {
                ed = new MemoryEditor(PID, address, length);

                id = ed.Id;

                action(ed);

                try
                {
                    Application.Run(ed);
                }
                catch
                { }

                Program.MemoryEditorsThreads.Remove(id);
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (id == "") Thread.Sleep(1);
            Program.MemoryEditorsThreads.Add(id, t);

            return ed;
        }

        /// <summary>
        /// Creates an instance of the results window on a separate thread.
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static ResultsWindow GetResultsWindow(int PID)
        {
            return GetResultsWindow(PID, new ResultsWindowInvokeAction(delegate { }));
        }

        /// <summary>
        /// Creates an instance of the results window on a separate thread and invokes an action on that thread.
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="action">The action to be performed.</param>
        /// <returns></returns>
        public static ResultsWindow GetResultsWindow(int PID, ResultsWindowInvokeAction action)
        {
            ResultsWindow rw = null;
            string id = "";

            Thread t = new Thread(new ThreadStart(delegate
            {
                rw = new ResultsWindow(PID);

                id = rw.Id;

                action(rw);

                try
                {
                    Application.Run(rw);
                }
                catch
                { }

                Program.ResultsThreads.Remove(id);
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (id == "") Thread.Sleep(1);
            Program.ResultsThreads.Add(id, t);

            return rw;
        }

        /// <summary>
        /// Creates an instance of the thread window on a separate thread.
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public static ThreadWindow GetThreadWindow(int PID, int TID)
        {
            return GetThreadWindow(PID, TID, new ThreadWindowInvokeAction(delegate { }));
        }

        /// <summary>
        /// Creates an instance of the thread window on a separate thread and invokes an action on that thread.
        /// </summary>
        /// <param name="PID"></param>
        /// <param name="action">The action to be performed.</param>
        /// <returns></returns>
        public static ThreadWindow GetThreadWindow(int PID, int TID, ThreadWindowInvokeAction action)
        {
            ThreadWindow tw = null;
            string id = "";

            Thread t = new Thread(new ThreadStart(delegate
            {
                tw = new ThreadWindow(PID, TID);

                id = tw.Id;

                action(tw);

                try
                {
                    Application.Run(tw);
                }
                catch
                { }

                Program.ThreadThreads.Remove(id);
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (id == "") Thread.Sleep(1);
            Program.ThreadThreads.Add(id, t);

            return tw;
        }

        public static void FocusWindow(Form f)
        {
            if (f.InvokeRequired)
            {
                f.Invoke(new MethodInvoker(delegate { Program.FocusWindow(f); }));

                return;
            }
            
            f.Activate();
        }

        public static void UpdateWindow(Form f, List<string> Texts, Dictionary<string, Form> TextToForm)
        {
            try
            {
                if (f.InvokeRequired)
                {
                    f.Invoke(new UpdateWindowAction(UpdateWindow), f, Texts, TextToForm);

                    return;
                }

                MenuItem windowMenuItem = (MenuItem)f.GetType().GetProperty("WindowMenuItem").GetValue(f, null);
                wyDay.Controls.VistaMenu vistaMenu =
                    (wyDay.Controls.VistaMenu)f.GetType().GetProperty("VistaMenu").GetValue(f, null);
                MenuItem item;

                lock (windowMenuItem)
                {
                    windowMenuItem.MenuItems.Clear();

                    foreach (string s in Texts)
                    {
                        Bitmap image = new Bitmap(16, 16);

                        item = new MenuItem(s);
                        item.Tag = TextToForm[s];
                        item.Click += new EventHandler(windowItemClicked);

                        if (item.Tag == f)
                            item.DefaultItem = true;

                        windowMenuItem.MenuItems.Add(item);

                        // don't add icon on XP - doesn't work for some reason
                        if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
                        {
                            using (Graphics g = Graphics.FromImage(image))
                            {
                                g.DrawImage(TextToForm[s].Icon.ToBitmap(), 0, 0, 16, 16);

                                vistaMenu.SetImage(item, image);
                            }
                        }
                    }

                    item = new MenuItem("&Close");
                    item.Tag = f;
                    item.Click += new EventHandler(windowCloseItemClicked);
                    windowMenuItem.MenuItems.Add(item);

                    vistaMenu.SetImage(item, global::ProcessHacker.Properties.Resources.application_delete);
                }
            }
            catch
            { }
        }

        public static void UpdateWindows()
        {
            Dictionary<string, Form> TextToForm = new Dictionary<string, Form>();
            List<string> Texts = new List<string>();

            TextToForm.Add("Process Hacker", HackerWindow);
            Texts.Add("Process Hacker");

            foreach (Form f in Program.MemoryEditors.Values)
            {
                TextToForm.Add(f.Text, f);
                Texts.Add(f.Text);
            }

            foreach (Form f in Program.ResultsWindows.Values)
            {
                TextToForm.Add(f.Text, f);
                Texts.Add(f.Text);
            }

            foreach (Form f in Program.ThreadWindows.Values)
            {
                TextToForm.Add(f.Text, f);
                Texts.Add(f.Text);
            }

            Texts.Sort();

            UpdateWindow(HackerWindow, Texts, TextToForm);
                                             
            foreach (Form f in MemoryEditors.Values)
            {
                UpdateWindow(f, Texts, TextToForm);
            }

            foreach (Form f in ResultsWindows.Values)
            {
                 UpdateWindow(f, Texts, TextToForm);
            }

            foreach (Form f in ThreadWindows.Values)
            {
                UpdateWindow(f, Texts, TextToForm);
            }
        }

        private static void windowItemClicked(object sender, EventArgs e)
        {
            Form f = (Form)((MenuItem)sender).Tag;

            Program.FocusWindow(f);
        }

        private static void windowCloseItemClicked(object sender, EventArgs e)
        {
            Form f = (Form)((MenuItem)sender).Tag;

            f.Invoke(new MethodInvoker(delegate { f.Close(); }));
        }
    }
}
