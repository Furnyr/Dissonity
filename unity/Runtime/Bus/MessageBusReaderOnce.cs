using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBusReaderOnce<T> : MessageBusReader<T>
    {
        internal override Action<T> Listener { get; }

        private bool _Done;
        public override bool Done => _Done;

        public MessageBusReaderOnce(object userListener, Action<T> listener) : base(userListener)
        {
            Listener = messageEvent =>
            {
                _Done = true;
                listener.Invoke(messageEvent);
            };
        }
    }
}