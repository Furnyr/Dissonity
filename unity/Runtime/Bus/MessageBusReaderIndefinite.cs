using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBusReaderIndefinite<T> : MessageBusReader<T>
    {
        internal override Action<T> Listener { get; }

        public override bool Done => false;

        public MessageBusReaderIndefinite(object userListener, Action<T> listener) : base(userListener)
        {
            Listener = listener;
        }
    }
}