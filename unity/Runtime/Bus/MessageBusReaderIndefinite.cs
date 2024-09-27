using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBusReaderIndefinite : MessageBusReader
    {
        protected override Action<DiscordEvent> Listener { get; }

        public override bool Done => false;

        public MessageBusReaderIndefinite(Action<DiscordEvent> listener)
        {
            Listener = listener;
        }
    }
}