using FXExchange.Domain.Infrastructure.Exceptions;
using FXExchange.Domain.Core;
using FXExchange.Domain.Providers;

namespace FXExchange.Tests.Core
{
	public class CurrencyConverterTests
	{
		private readonly CurrencyConverter _sut = new(new JsonEmbeddedExchangeRateProvider());

		[Theory]
		[InlineData("USD", "USD", 10, 10)]
		[InlineData("EUR", "EUR", 1, 1)]
		[InlineData("DKK", "DKK", 123.456, 123.456)]
		public async Task SameCurrency_ReturnsAmount(string baseCurrency, string quoteCurrency, decimal amount, decimal expected)
		{
			var pair = new CurrencyPair(baseCurrency, quoteCurrency);
			var convertedAmount = await _sut.ConvertAsync(pair, amount);
			Assert.Equal(expected, convertedAmount);
		}

		[Theory]
		[InlineData("EUR", "DKK", 1, 7.4394)]
		[InlineData("USD", "EUR", 1, 0.8913)]
		[InlineData("DKK", "USD", 100, 15.0805)]
		[InlineData("GBP", "CHF", 1, 1.2476)]
		[InlineData("JPY", "DKK", 1000, 59.74)]
		public async Task Converts_Correctly(string baseCurrency, string quoteCurrency, decimal amount, decimal expected)
		{
			var pair = new CurrencyPair(baseCurrency, quoteCurrency);
			var convertedAmount = await _sut.ConvertAsync(pair, amount);
			Assert.Equal(expected, Math.Round(convertedAmount, 4, MidpointRounding.AwayFromZero));
		}

		[Fact]
		public async Task ZeroOrNegative_Amount_Throws()
		{
			var pair = new CurrencyPair("EUR", "USD");
			await Assert.ThrowsAsync<InvalidAmountException>(() => _sut.ConvertAsync(pair, 0));
		}

		[Fact]
		public async Task UnknownCurrency_Throws()
		{
			var pair = new CurrencyPair("XYZ", "USD");
			await Assert.ThrowsAsync<UnknownCurrencyException>(() => _sut.ConvertAsync(pair, 1m));
		}
	}
}
