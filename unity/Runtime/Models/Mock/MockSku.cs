using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockSku : Sku
    {
        #nullable enable annotations

        new public string Id = "543212345";

        new public string Name = "Mock SKU";

        new public SkuType Type = SkuType.Dlc;

        new public MockSkuPrice Price = new();

        new public string ApplicationId = "121221123456";

        new public int Flags = 0;

        new public string? ReleaseDate = "20xx-04-15T15:50+00Z";

        public Sku ToSku()
        {
            return new Sku()
            {
                Id = Id,
                Name = Name,
                Type = Type,
                Price = Price.ToSkuPrice(),
                ApplicationId = ApplicationId,
                Flags = Flags,
                ReleaseDate = ReleaseDate
            };
        }
    }
}