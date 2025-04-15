using System;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class StartPurchase : FrameCommand
    {
        internal override string Command => DiscordCommandType.StartPurchase;

        [JsonProperty("sku_id")]
        public string SkuId { get; set; }

        public StartPurchase(string skuId)
        {
            SkuId = skuId;
        }
    }
}