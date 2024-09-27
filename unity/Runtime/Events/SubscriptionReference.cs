using Dissonity.Bus;

namespace Dissonity.Events
{
    public class SubscriptionReference 
    {
        internal MessageBusReader Reader { get; set; }
        internal string EventString { get; set; }

        internal void SaveSubscriptionData(MessageBusReader reader, string eventString)
        {
            Reader = reader;
            EventString = eventString;
        }
    }
}