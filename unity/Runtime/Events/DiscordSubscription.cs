using Dissonity.Bus;

namespace Dissonity.Events
{
    public class DiscordSubscription 
    {
        internal MessageBusReader<DiscordEvent> Reader { get; set; }
        internal string EventString { get; set; }

        internal void SaveSubscriptionData(MessageBusReader<DiscordEvent> reader, string eventString)
        {
            Reader = reader;
            EventString = eventString;
        }
    }
}