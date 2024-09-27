using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal abstract class MessageBusReader
    {
        protected abstract Action<DiscordEvent> Listener { get; }

        public abstract bool Done { get; }

        internal void ReadEvent(DiscordEvent discordEvent)
        {
            Listener?.Invoke(discordEvent);
        }
    }
}