using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Messaging
{
    public class Message
    {
        private object _tag;

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }

    public class ActionMessage : Message
    {
        private Action _action;

        public ActionMessage(Action action)
        {
            _action = action;
        }

        public Action Action
        {
            get { return _action; }
        }
    }
}
