using System;
using System.Diagnostics;

namespace Debugger
{
	[Serializable]
	public class DebuggerEventArgs : EventArgs 
	{
		object debugger;

        public object Debugger
        {
			get {
				return debugger;
			}
		}

        public DebuggerEventArgs(object debugger)
		{
			this.debugger = debugger;
		}
	}

    [Serializable]
    public class ProcessEventArgs : DebuggerEventArgs
    {
        Process process;

        public Process Process
        {
            get
            {
                return process;
            }
        }

        public ProcessEventArgs(Process process)
            : base(null)
        {
            this.process = process;
        }
    }

    [Serializable]
    public class MessageEventArgs : ProcessEventArgs
    {
        int level;
        string message;
        string category;

        public int Level
        {
            get
            {
                return level;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }

        public string Category
        {
            get
            {
                return category;
            }
        }

        public MessageEventArgs(Process process, string message)
            : this(process, 0, message, String.Empty)
        {
            this.message = message;
        }

        public MessageEventArgs(Process process, int level, string message, string category)
            : base(process)
        {
            this.level = level;
            this.message = message;
            this.category = category;
        }
    }
}
