using System;

namespace Dissonity.Models.Mock
{
    [Serializable]
    public class MockSkuPrice : SkuPrice
    {
        new public double Amount = 199;

        new public string Currency = CurrencyCode.Usd;

        public SkuPrice ToSkuPrice()
        {
            return new SkuPrice()
            {
                Amount = Amount,
                Currency = Currency
            };
        }
    }
}