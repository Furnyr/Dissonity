using System.Collections.Generic;
using System.Linq;
using Dissonity.Events;

namespace Dissonity.Bus
{
    internal class DiscordMessageBus
    {
        internal Dictionary<string, HashSet<MessageBusReader<DiscordEvent>>> ReaderSetDictionary = new();
        internal Dictionary<string, HashSet<MessageBusReader<DiscordEvent>>> InternalReaderSetDictionary = new();

        private object ReaderLock = new object();

        public MessageBusReader<DiscordEvent> AddReader(string eventString, MessageBusReader<DiscordEvent> reader, bool isInternal = false)
        {
            lock (ReaderLock)
            {
                Dictionary<string, HashSet<MessageBusReader<DiscordEvent>>> dictionary = isInternal
                    ? InternalReaderSetDictionary
                    : ReaderSetDictionary;

                //? Existing reader set
                if (dictionary.ContainsKey(eventString))
                {
                    var readerSet = dictionary[eventString];

                    readerSet.Add(reader);
                }

                //? New reader set
                else
                {
                    var readerSet = new HashSet<MessageBusReader<DiscordEvent>>();

                    dictionary.Add(eventString, readerSet);

                    readerSet.Add(reader);
                }
            }

            return reader;
        }

        public bool ReaderSetExists(string eventString)
        {
            return ReaderSetDictionary.ContainsKey(eventString) || InternalReaderSetDictionary.ContainsKey(eventString);
        }

        public bool RemoveReader(string eventString, MessageBusReader<DiscordEvent> reader, bool isInternal = false)
        {
            bool setIsGone = false;

            lock (ReaderLock)
            {
                Dictionary<string, HashSet<MessageBusReader<DiscordEvent>>> dictionary = isInternal
                    ? InternalReaderSetDictionary
                    : ReaderSetDictionary;

                if (!dictionary.ContainsKey(eventString)) return false;

                var readerSet = dictionary[eventString];

                readerSet.RemoveWhere(r => r == reader);

                //? Empty reader set
                if (readerSet.Count == 0)
                {
                    dictionary.Remove(eventString);
                    setIsGone = ReaderSetExists(eventString);
                }
            }

            return setIsGone;
        }

        public void RemoveAllReaders(string eventString, bool isInternal = false)
        {
            lock (ReaderLock)
            {
                Dictionary<string, HashSet<MessageBusReader<DiscordEvent>>> dictionary = isInternal
                    ? InternalReaderSetDictionary
                    : ReaderSetDictionary;
                
                if (!dictionary.ContainsKey(eventString)) return;

                dictionary.Remove(eventString);
            }
        }

        public virtual void DispatchEvent(DiscordEvent discordEvent)
        {
            string eventString = discordEvent.Event!;

            lock (ReaderLock)
            {
                //\ Dispatch in both dictionaries
                var dictionaries = new Dictionary<string, HashSet<MessageBusReader<DiscordEvent>>>[] { ReaderSetDictionary, InternalReaderSetDictionary };
                foreach (var dictionary in dictionaries)
                {
                    if (!dictionary.ContainsKey(eventString)) continue;

                    var readerSet = dictionary[eventString];

                    readerSet.RemoveWhere(reader => reader.Done);

                    foreach (var reader in readerSet.ToList())
                    {
                        reader.ReadEvent(discordEvent);
                    }
                }
            }
        }
    }
}