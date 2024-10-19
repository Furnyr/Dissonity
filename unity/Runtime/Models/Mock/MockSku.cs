using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockSku : Sku
    {
        #nullable enable annotations

        new public long Id = 123456789;

        new public string Name = "Mock SKU";

        new public SkuType Type = SkuType.Dlc;

        new public MockSkuPrice Price = new();

        new public long ApplicationId = 123456789;

        new public long Flags = 0;

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