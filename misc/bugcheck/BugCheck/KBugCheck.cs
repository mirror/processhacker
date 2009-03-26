using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using ProcessHacker;

namespace BugCheck
{
    public class KBugCheck
    {
        private enum Control : uint
        {
            BugCheckEx = 0
        }

        private string _deviceName;
        private Win32.FileHandle _fileHandle;
        private uint _baseControlNumber;
        private Win32.ServiceHandle _service;

        /// <summary>
        /// Creates a connection to KProcessHacker.
        /// </summary>
        /// <param name="deviceName">The device to connect to.</param>
        public KBugCheck(string deviceName)
        {
            _deviceName = deviceName;

            bool started = false;

            // delete the service if it exists
            try
            {
                using (var shandle = new Win32.ServiceHandle(deviceName))
                {
                    started = shandle.GetStatus().CurrentState == Win32.SERVICE_STATE.Running;

                    if (!started)
                        shandle.Delete();
                }
            }
            catch
            { }

            try
            {
                Win32.ServiceManagerHandle scm =
                    new Win32.ServiceManagerHandle(Win32.SC_MANAGER_RIGHTS.SC_MANAGER_CREATE_SERVICE);

                _service = scm.CreateService(deviceName, deviceName, Win32.SERVICE_TYPE.KernelDriver,
                    Application.StartupPath + "\\kbugcheck.sys");
                _service.Start();
            }
            catch
            { }

            _fileHandle = new Win32.FileHandle("\\\\.\\" + deviceName,
                Win32.FILE_RIGHTS.FILE_GENERIC_READ | Win32.FILE_RIGHTS.FILE_GENERIC_WRITE);

            try
            {
                if (!started)
                    _service.Delete(); // the service will automatically get deleted once it stops
            }
            catch
            { }

            _baseControlNumber = Misc.BytesToUInt(_fileHandle.Read(4), Misc.Endianness.Little);
        }

        public string DeviceName
        {
            get { return _deviceName; }
        }

        private uint CtlCode(Control ctl)
        {
            return _baseControlNumber + ((uint)ctl * 4);
        }

        /// <summary>
        /// Closes the connection to KProcessHacker.
        /// </summary>
        public void Close()
        {
            _fileHandle.Dispose();
        }

        public void BugCheckEx(uint code, uint param1, uint param2, uint param3, uint param4)
        {
            byte[] data = new byte[5 * 4];

            Array.Copy(Misc.UIntToBytes(code, Misc.Endianness.Little), 0, data, 0, 4);
            Array.Copy(Misc.UIntToBytes(param1, Misc.Endianness.Little), 0, data, 4, 4);
            Array.Copy(Misc.UIntToBytes(param2, Misc.Endianness.Little), 0, data, 8, 4);
            Array.Copy(Misc.UIntToBytes(param3, Misc.Endianness.Little), 0, data, 12, 4);
            Array.Copy(Misc.UIntToBytes(param4, Misc.Endianness.Little), 0, data, 16, 4);

            _fileHandle.IoControl(CtlCode(Control.BugCheckEx), data, null);
        }
    }
}
