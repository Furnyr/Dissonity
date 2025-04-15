using Dissonity.Bus;

namespace Dissonity.Models.Interop
{
    public class HiRpcSubscription 
    {
        internal MessageBusReader<HiRpcMessage> Reader { get; set; }
        internal string EventString { get; set; }

        internal void SaveSubscriptionData(MessageBusReader<HiRpcMessage> reader, string eventString)
        {
            Reader = reader;
            EventString = eventString;
        }
    }
}