using FXExchange.Domain.Infrastructure.Exceptions;

namespace FXExchange.Domain.Core
{
	public class CurrencyConverter(IExchangeRateProvider exchangeRateProvider)
	{
		public async Task<decimal> ConvertAsync(CurrencyPair pair, decimal amount, CancellationToken cancellationToken = default)
		{
			if (amount <= 0)
				throw new InvalidAmountException();

			if (pair.Base == pair.Quote)
				return amount;

			var dkkPerBase	= await exchangeRateProvider.GetRateAsync(pair.Base, cancellationToken);
			var dkkPerQuote = await exchangeRateProvider.GetRateAsync(pair.Quote, cancellationToken);

			return amount * (dkkPerBase / dkkPerQuote);
		}
	}
}
