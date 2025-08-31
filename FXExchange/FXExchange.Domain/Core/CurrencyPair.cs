namespace FXExchange.Domain.Core
{
	public record CurrencyPair(string Base, string Quote)
	{
		public static bool TryParse(string input, out CurrencyPair pair)
		{
			pair = null!;
			if (string.IsNullOrWhiteSpace(input))
				return false;

			var parts = input.Split('/', StringSplitOptions.TrimEntries);
			if (parts.Length != 2) return false;

			var baseCurrency = parts[0].ToUpperInvariant();
			var quoteCurrency = parts[1].ToUpperInvariant();
			if (baseCurrency.Length != 3 || quoteCurrency.Length != 3) 
				return false;

			pair = new CurrencyPair(baseCurrency, quoteCurrency);
			return true;
		}
	}
}
