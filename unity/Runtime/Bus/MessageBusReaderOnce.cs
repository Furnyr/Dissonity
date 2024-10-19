using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBusReaderOnce : MessageBusReader
    {
        internal override Action<DiscordEvent> Listener { get; }

        private bool _Done;
        public override bool Done => _Done;

        public MessageBusReaderOnce(object userListener, Action<DiscordEvent> listener) : base(userListener)
        {
            Listener = discordEvent =>
            {
                _Done = true;
                listener.Invoke(discordEvent);
            };
        }
    }
}