using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.SsLogging;

namespace SysCallHacker
{
    public class LogEvent
    {
        private SsEvent _event;
        private SsData[] _arguments;

        public LogEvent(SsEvent even)
        {
            _event = even;
            _arguments = new SsData[even.Arguments.Length];
        }

        public SsData[] Arguments
        {
            get { return _arguments; }
        }

        public SsEvent Event
        {
            get { return _event; }
        }
    }
}
