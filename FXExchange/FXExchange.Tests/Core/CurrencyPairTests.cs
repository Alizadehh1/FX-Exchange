using FXExchange.Domain.Core;

namespace FXExchange.Tests.Core
{
	public class CurrencyPairTests
	{
		[Theory]
		[InlineData("EUR/USD", "EUR", "USD")]
		[InlineData("usd/eur", "USD", "EUR")]
		[InlineData(" dkk / dkk ", "DKK", "DKK")]
		public void TryParse_Valid(string input, string baseCurrency, string quoteCurrency)
		{
			Assert.True(CurrencyPair.TryParse(input, out var pair));
			Assert.Equal(baseCurrency, pair.Base);
			Assert.Equal(quoteCurrency, pair.Quote);
		}

		[Theory]
		[InlineData("")]
		[InlineData("EURUSD")]
		[InlineData("EU/R")]
		[InlineData("EURO/USDOLLAR")]
		[InlineData("EUR/US")]
		public void TryParse_Invalid(string input)
		{
			Assert.False(CurrencyPair.TryParse(input, out _));
		}
	}
}
