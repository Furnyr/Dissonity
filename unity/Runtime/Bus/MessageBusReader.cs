using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal abstract class MessageBusReader
    {
        #nullable enable

        internal abstract Action<DiscordEvent> Listener { get; }

        internal object UserListener { get; } // (Action) For unsubscription methods

        public abstract bool Done { get; }

        public MessageBusReader(object userListener)
        {
            UserListener = userListener;
        }

        internal void ReadEvent(DiscordEvent discordEvent)
        {
            Listener?.Invoke(discordEvent);
        }
    }
}