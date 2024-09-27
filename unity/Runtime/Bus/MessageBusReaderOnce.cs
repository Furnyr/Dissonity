using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBusReaderOnce : MessageBusReader
    {
        protected override Action<DiscordEvent> Listener { get; }

        private bool _Done;
        public override bool Done => _Done;

        public MessageBusReaderOnce(Action<DiscordEvent> listener)
        {
            Listener = discordEvent =>
            {
                listener.Invoke(discordEvent);
                _Done = true;
            };
        }
    }
}