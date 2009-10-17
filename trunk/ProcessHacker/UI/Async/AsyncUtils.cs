/*
 * Process Hacker - 
 *   wrapper for running tasks asynchronously
 * 
 * Copyright (C) 2009 wj32
 * Copyright (C) 2008 Dean
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
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace ProcessHacker.FormHelper
{
    /// <summary>
    /// Exception thrown when an operation is already in progress.
    /// </summary>
    public class AlreadyRunningException : System.ApplicationException
    {
        public AlreadyRunningException() : base("Operation already running")
        { }
    }

    public abstract class AsyncOperation
    {
        private Thread _asyncThread;
        private object _asyncLock = new object();

        public AsyncOperation(ISynchronizeInvoke target)
        {
            isiTarget = target;
            isRunning = false;
        }

        public void Start()
        {
            lock (_asyncLock)
            {
                if (isRunning)
                {
                    throw new AlreadyRunningException();
                }
                isRunning = true;
            }

            _asyncThread = new Thread(InternalStart, ProcessHacker.Common.Utils.SixteenthStackSize);
            _asyncThread.Start();
        }

        public void Cancel()
        {
            lock (_asyncLock)
            {
                cancelledFlag = true;
            }
        }

        public bool CancelAndWait()
        {
            lock (_asyncLock)
            {               
                cancelledFlag = true;

                while (!IsDone)
                {
                    Monitor.Wait(_asyncLock, 1000);
                }
            }

            return !HasCompleted;
        }

        public bool WaitUntilDone()
        {
            lock (_asyncLock)
            {
                // Wait for either completion or cancellation.  As with
                // CancelAndWait, we don't sleep forever - to reduce the
                // chances of deadlock in obscure race conditions, we wake
                // up every second to check we didn't miss a Pulse.
                while (!IsDone)
                {
                    Monitor.Wait(_asyncLock, 1000);
                }
            }

            return HasCompleted;
        }

        public bool IsDone
        {
            get
            {
                lock (_asyncLock)
                {
                    return completeFlag || cancelAcknowledgedFlag || failedFlag;
                }
            }
        }

        public event EventHandler Completed;              
        public event EventHandler Cancelled;       
        public event System.Threading.ThreadExceptionEventHandler Failed;   

        private ISynchronizeInvoke isiTarget;
        protected ISynchronizeInvoke Target
        {
            get { return isiTarget; }
        }

        /// <summary>
        /// To be overridden by the deriving class
        /// </summary>
        protected abstract void DoWork();
                            
        private bool cancelledFlag;
        protected bool CancelRequested
        {
            get
            {
                lock (_asyncLock) { return cancelledFlag; }
            }
        }

        private bool completeFlag;
        protected bool HasCompleted
        {
            get
            {
                lock (_asyncLock) { return completeFlag; }
            }
        }

        protected void AcknowledgeCancel()
        {
            lock (_asyncLock)
            {
                cancelAcknowledgedFlag = true;
                isRunning = false;
                Monitor.Pulse(_asyncLock);
                FireAsync(Cancelled, this, EventArgs.Empty);
            }
        }

        private bool cancelAcknowledgedFlag;
        // if the operation fails with an exception, set to true
        private bool failedFlag;
        // if the operation is running, set to true
        private bool isRunning;

        private void InternalStart()
        {            
            cancelledFlag = false;
            completeFlag = false;
            cancelAcknowledgedFlag = false;
            failedFlag = false; 
          
            try
            {
                DoWork();
            }
            catch (Exception e)
            {                
                try
                {
                    FailOperation(e);
                }
                catch
                { }  
            
                if (e is SystemException)
                {
                    throw;
                }
            }

            lock (_asyncLock)
            {
                // raise the Completion event 
                if (!cancelAcknowledgedFlag && !failedFlag)
                {
                    CompleteOperation();
                }
            }
        } 

        private void CompleteOperation()
        {
            lock (_asyncLock)
            {
                completeFlag = true;
                isRunning = false;
                Monitor.Pulse(_asyncLock);                
                FireAsync(Completed, this, EventArgs.Empty);
            }
        }

        private void FailOperation(Exception e)
        {
            lock (_asyncLock)
            {
                failedFlag = true;
                isRunning = false;
                Monitor.Pulse(_asyncLock);
                FireAsync(Failed, this, new ThreadExceptionEventArgs(e));
            }
        }

        protected void FireAsync(Delegate dlg, params object[] pList)
        {
            if (dlg != null)
            {
                Target.BeginInvoke(dlg, pList);
            }
        }
    }
}


