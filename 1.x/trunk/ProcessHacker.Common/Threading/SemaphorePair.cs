using System;
using System.Threading;

namespace ProcessHacker.Common.Threading
{
    public class SemaphorePair : IDisposable
    {
        private int _count;
        private Semaphore _readSemaphore;
        private Semaphore _writeSemaphore;

        public SemaphorePair(int count)
        {
            _count = count;
            _readSemaphore = new Semaphore(0, count);
            _writeSemaphore = new Semaphore(count, count);
        }

        public int Count
        {
            get { return _count; }
        }

        public void Dispose()
        {
            _readSemaphore.Close();
            _writeSemaphore.Close();
        }

        public void ReleaseRead()
        {
            _readSemaphore.Release();
        }

        public void ReleaseWrite()
        {
            _writeSemaphore.Release();
        }

        public void WaitRead()
        {
            _readSemaphore.WaitOne();
        }

        public bool WaitRead(int timeout)
        {
            return _readSemaphore.WaitOne(timeout, false);
        }

        public void WaitWrite()
        {
            _writeSemaphore.WaitOne();
        }

        public bool WaitWrite(int timeout)
        {
            return _writeSemaphore.WaitOne(timeout, false);
        }
    }
}
