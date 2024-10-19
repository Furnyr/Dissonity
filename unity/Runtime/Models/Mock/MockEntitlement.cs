using System;
using UnityEngine;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockEntitlement : Entitlement
    {
        #nullable enable annotations

        new public long Id = 123456789;

        new public long SkuId = 123456789;

        new public long ApplicationId = 123456789;

        new public int GiftCodeFlags = 0;

        new public EntitlementType Type = EntitlementType.Purchase;

        // UserId isn't exposed
 
        new public bool? Consumed = false;

        new public bool? Deleted = false;

        new public string EndsAt = "20xx-04-15T15:50+00Z";

        new public string? GiftCodeBatchId = null;

        new public long? GifterUserId = null;

        new public string StartsAt = "20xx-04-15T15:50+00Z";

        // ParentId isn't exposed

        // Branches isn't exposed

        public string __mock__name; // funky name so users know it's not normally there, just in case

        public Entitlement ToEntitlement()
        {
            return new Entitlement()
            {
                ApplicationId = ApplicationId,
                GiftCodeFlags = GiftCodeFlags,
                Id = Id,
                SkuId = SkuId,
                Type = Type,
                Consumed = Consumed,
                Deleted = Deleted,
                EndsAt = EndsAt,
                GiftCodeBatchId = GiftCodeBatchId,
                GifterUserId = GifterUserId,
                StartsAt = StartsAt
            };
        }
    }
}