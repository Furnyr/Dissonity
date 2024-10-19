using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBusReaderIndefinite : MessageBusReader
    {
        internal override Action<DiscordEvent> Listener { get; }

        public override bool Done => false;

        public MessageBusReaderIndefinite(object userListener, Action<DiscordEvent> listener) : base(userListener)
        {
            Listener = listener;
        }
    }
}