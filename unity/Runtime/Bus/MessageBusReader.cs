using System;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal abstract class MessageBusReader<T>
    {
        #nullable enable

        internal abstract Action<T> Listener { get; }

        internal object UserListener { get; } // (Action) For unsubscription methods

        public abstract bool Done { get; }

        public MessageBusReader(object userListener)
        {
            UserListener = userListener;
        }

        internal void ReadEvent(T discordEvent)
        {
            Listener?.Invoke(discordEvent);
        }
    }
}