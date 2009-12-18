using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    public delegate void ThreadTaskCompletedDelegate(object result);
    public delegate void ThreadTaskRunTaskDelegate(object param, ref object result);

    public sealed class ThreadTask
    {
        public event ThreadTaskCompletedDelegate Completed;
        public event ThreadTaskRunTaskDelegate RunTask;

        private Thread _thread = null;
        private object _result;
        private Exception _exception;
        private bool _cancelled = false;
        private bool _running = false;

        public bool Cancelled
        {
            get { return _cancelled; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public object Result
        {
            get { return _result; }
        }

        public bool Running
        {
            get { return _running; }
        }

        public void Cancel()
        {
            _cancelled = true;
        }

        public void Start()
        {
            this.Start(null);
        }

        public void Start(object param)
        {
            if (_thread != null)
                throw new InvalidOperationException("The task has already been started.");

            _cancelled = false;
            _running = true;

            _thread = new Thread(this.ThreadStart);
            _thread.IsBackground = true;
            _thread.Start(param);
        }

        private void ThreadStart(object param)
        {
            try
            {
                if (this.RunTask != null)
                    this.RunTask(param, ref _result);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            if (!_cancelled && this.Completed != null)
                this.Completed(_result);

            _running = false;
            _thread = null;
        }

        public void Wait()
        {
            _thread.Join();
        }
    }
}
