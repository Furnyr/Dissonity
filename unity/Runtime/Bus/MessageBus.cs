
using System.Collections.Generic;
using System.Linq;
using Dissonity.Events;
using UnityEngine;

namespace Dissonity.Bus
{
    internal class MessageBus
    {
        internal Dictionary<string, HashSet<MessageBusReader>> ReaderSetDictionary = new();
        internal Dictionary<string, HashSet<MessageBusReader>> InternalReaderSetDictionary = new();

        private object ReaderLock = new object();

        public MessageBusReader AddReader(string eventString, MessageBusReader reader, bool isInternal = false)
        {
            lock (ReaderLock)
            {
                Dictionary<string, HashSet<MessageBusReader>> dictionary = isInternal
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
                    var readerSet = new HashSet<MessageBusReader>();

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

        public bool RemoveReader(string eventString, MessageBusReader reader, bool isInternal = false)
        {
            bool setIsGone = false;

            lock (ReaderLock)
            {
                Dictionary<string, HashSet<MessageBusReader>> dictionary = isInternal
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
                Dictionary<string, HashSet<MessageBusReader>> dictionary = isInternal
                    ? InternalReaderSetDictionary
                    : ReaderSetDictionary;
                
                if (!dictionary.ContainsKey(eventString)) return;

                dictionary.Remove(eventString);
            }
        }

        public void DispatchEvent(DiscordEvent discordEvent)
        {
            string eventString = discordEvent.Event!;

            lock (ReaderLock)
            {
                //\ Dispatch in both dictionaries
                var dictionaries = new Dictionary<string, HashSet<MessageBusReader>>[] { ReaderSetDictionary, InternalReaderSetDictionary };
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