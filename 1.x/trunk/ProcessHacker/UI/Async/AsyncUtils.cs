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
using System.Threading;
using System.ComponentModel;

namespace ProcessHacker.FormHelper
{
    /// <summary>
    /// Exception thrown when an operation is already in progress.
    /// </summary>
    public class AlreadyRunningException : ApplicationException
    {
        public AlreadyRunningException() : base("Operation already running")
        { }
    }

    public abstract class AsyncOperation
    {
        private Thread _asyncThread;
        private readonly object _asyncLock = new object();

        protected AsyncOperation(ISynchronizeInvoke target)
        {
            this.isiTarget = target;
            this.isRunning = false;
        }

        public void Start()
        {
            lock (this._asyncLock)
            {
                if (this.isRunning)
                {
                    throw new AlreadyRunningException();
                }
                this.isRunning = true;
            }

            this._asyncThread = new Thread(this.InternalStart, Common.Utils.SixteenthStackSize);
            this._asyncThread.Start();
        }

        public void Cancel()
        {
            lock (this._asyncLock)
            {
                this.cancelledFlag = true;
            }
        }

        public bool CancelAndWait()
        {
            lock (this._asyncLock)
            {
                this.cancelledFlag = true;

                while (!this.IsDone)
                {
                    Monitor.Wait(this._asyncLock, 1000);
                }
            }

            return !this.HasCompleted;
        }

        public bool WaitUntilDone()
        {
            lock (this._asyncLock)
            {
                // Wait for either completion or cancellation.  As with
                // CancelAndWait, we don't sleep forever - to reduce the
                // chances of deadlock in obscure race conditions, we wake
                // up every second to check we didn't miss a Pulse.
                while (!this.IsDone)
                {
                    Monitor.Wait(this._asyncLock, 1000);
                }
            }

            return this.HasCompleted;
        }

        public bool IsDone
        {
            get
            {
                lock (this._asyncLock)
                {
                    return this.completeFlag || this.cancelAcknowledgedFlag || this.failedFlag;
                }
            }
        }

        public event EventHandler Completed;              
        public event EventHandler Cancelled;       
        public event ThreadExceptionEventHandler Failed;   

        private readonly ISynchronizeInvoke isiTarget;
        protected ISynchronizeInvoke Target
        {
            get { return this.isiTarget; }
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
                lock (this._asyncLock) { return this.cancelledFlag; }
            }
        }

        private bool completeFlag;
        protected bool HasCompleted
        {
            get
            {
                lock (this._asyncLock) { return this.completeFlag; }
            }
        }

        protected void AcknowledgeCancel()
        {
            lock (this._asyncLock)
            {
                this.cancelAcknowledgedFlag = true;
                this.isRunning = false;

                Monitor.Pulse(this._asyncLock);

                if (this.Cancelled != null)
                    this.Cancelled(this, EventArgs.Empty);
            }
        }

        private bool cancelAcknowledgedFlag;
        // if the operation fails with an exception, set to true
        private bool failedFlag;
        // if the operation is running, set to true
        private bool isRunning;

        private void InternalStart()
        {
            this.cancelledFlag = false;
            this.completeFlag = false;
            this.cancelAcknowledgedFlag = false;
            this.failedFlag = false; 
          
            try
            {
                this.DoWork();
            }
            catch (Exception e)
            {                
                try
                {
                    this.FailOperation(e);
                }
                catch
                { }  
            
                if (e is SystemException)
                {
                    throw;
                }
            }

            lock (this._asyncLock)
            {
                // raise the Completion event 
                if (!this.cancelAcknowledgedFlag && !this.failedFlag)
                {
                    this.CompleteOperation();
                }
            }
        } 

        private void CompleteOperation()
        {
            lock (this._asyncLock)
            {
                this.completeFlag = true;
                this.isRunning = false;

                Monitor.Pulse(this._asyncLock);

                if (this.Completed != null)
                    this.Completed(this, EventArgs.Empty);
            }
        }

        private void FailOperation(Exception e)
        {
            lock (this._asyncLock)
            {
                this.failedFlag = true;
                this.isRunning = false;

                Monitor.Pulse(this._asyncLock);

                if (this.Failed != null)
                    this.Failed(this, new ThreadExceptionEventArgs(e));
            }
        }
    }
}


