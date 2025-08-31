using FXExchange.Domain.Infrastructure.Exceptions;
using FXExchange.Domain.Providers;

namespace FXExchange.Tests.Providers
{
	public class JsonEmbeddedExchangeRateProviderTests
	{
		private readonly JsonEmbeddedExchangeRateProvider provider = new();

		[Theory]
		[InlineData("DKK", 1.0)]
		[InlineData("EUR", 7.4394)]
		[InlineData("USD", 6.6311)]
		[InlineData("JPY", 0.05974)]
		public async Task Loads_Known_Rates(string currency, decimal expectedRate)
		{
			var actual = await provider.GetRateAsync(currency, default);

			Assert.Equal(expectedRate, actual);
		}

		[Theory]
		[InlineData("XXX")]
		[InlineData("ABC")]
		[InlineData("EURO")]
		[InlineData("USDOLLAR")]
		[InlineData("E")]   
		[InlineData("123")]
		public async Task Unknown_Currency_Throws(string iso)
		{
			await Assert.ThrowsAsync<UnknownCurrencyException>(async () => await provider.GetRateAsync(iso, default));
		}
	}
}
