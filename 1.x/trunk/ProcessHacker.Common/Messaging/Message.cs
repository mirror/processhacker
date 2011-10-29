using System;

namespace ProcessHacker.Common.Messaging
{
    public class Message
    {
        public object Tag { get; set; }
    }

    public class ActionMessage : Message
    {
        private readonly Action _action;

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
