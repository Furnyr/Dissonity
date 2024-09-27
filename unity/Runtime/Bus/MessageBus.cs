
using System.Collections.Generic;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class MessageBus
    {
        #nullable enable

        internal Dictionary<string, HashSet<MessageBusReader>> ReaderSetDictionary = new();

        private object ReaderLock = new object();

        public MessageBusReader AddReader(string eventString, MessageBusReader reader)
        {
            lock (ReaderLock)
            {
                //? Existing reader set
                if (ReaderSetDictionary.ContainsKey(eventString))
                {
                    var readerSet = ReaderSetDictionary[eventString];

                    readerSet.Add(reader);
                }

                //? New reader set
                else
                {
                    var readerSet = new HashSet<MessageBusReader>();

                    ReaderSetDictionary.Add(eventString, readerSet);

                    readerSet.Add(reader);
                }
            }

            return reader;
        }

        public bool RemoveReader(string eventString, MessageBusReader reader)
        {
            bool removedSet = false;

            lock (ReaderLock)
            {
                if (!ReaderSetDictionary.ContainsKey(eventString)) return false;

                var readerSet = ReaderSetDictionary[eventString];

                readerSet.RemoveWhere(r => r == reader);

                //? Empty reader set
                if (readerSet.Count == 0)
                {
                    removedSet = true;
                    ReaderSetDictionary.Remove(eventString);
                }
            }

            return removedSet;
        }

        public void RemoveAllReaders(string eventString)
        {
            lock (ReaderLock)
            {
                if (!ReaderSetDictionary.ContainsKey(eventString)) return;

                ReaderSetDictionary.Remove(eventString);
            }
        }

        public void DispatchEvent(DiscordEvent discordEvent)
        {
            string eventString = discordEvent.Event!;

            lock (ReaderLock)
            {
                if (!ReaderSetDictionary.ContainsKey(eventString)) return;

                var readerSet = ReaderSetDictionary[eventString];

                foreach (var reader in readerSet)
                {
                    reader.ReadEvent(discordEvent);
                }

                readerSet.RemoveWhere(reader => reader.Done);
            }
        }
    }
}