using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockEntitlement : Entitlement
    {
        #nullable enable annotations

        new public string Id = "012345678";

        new public string SkuId = "mock-sku-id";

        new public string ApplicationId = "121221123456";

        new public int GiftCodeFlags = 0;

        new public EntitlementType Type = EntitlementType.Purchase;

        new public string UserId = "9123456780";

        // Branches isn't exposed
 
        new public bool? Consumed = false;

        new public bool? Deleted = false;

        new public string EndsAt = "20xx-04-15T15:50+00Z";

        new public string? GiftCodeBatchId = null;

        new public string? GifterUserId = null;

        // ParentId isn't exposed

        new public string StartsAt = "20xx-04-15T15:50+00Z";

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
                UserId = UserId,
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