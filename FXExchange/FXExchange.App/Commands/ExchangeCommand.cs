using FXExchange.Domain.Infrastructure.Exceptions;
using FXExchange.Domain.Infrastructure.Globalization;
using FXExchange.Domain.Infrastructure.Parsing;
using FXExchange.Domain.Core;

namespace FXExchange.App.Commands
{
	public class ExchangeCommand(CurrencyConverter currencyConverter)
	{
		public async Task<int> RunAsync(string[] args)
		{
			if (args.Length != 3 || !args[0].Equals("Exchange", StringComparison.OrdinalIgnoreCase))
			{
				PrintUsage();
				return 1;
			}
			if (!CurrencyPair.TryParse(args[1], out var pair) || !AmountParser.TryParse(args[2], out var amount))
			{
				PrintUsage();
				return 2;
			}

			try
			{
				var value	= await currencyConverter.ConvertAsync(pair, amount);
				var rounded = Math.Round(value, 4, MidpointRounding.AwayFromZero);
				Console.WriteLine(rounded.ToString("0.####", AppCultures.Danish));
				return 0;
			}
			catch (UnknownCurrencyException ex)
			{
				Console.WriteLine(ex.Message);
				return 5; 
			}
			catch (InvalidAmountException ex)
			{
				Console.WriteLine(ex.Message);
				return 6; 
			}
			catch (TimeoutException)
			{
				Console.WriteLine("Rate service timed out.");
				return 7;
			}
			catch (HttpRequestException)
			{
				Console.WriteLine("Could not reach rate service.");
				return 8;
			}
			catch (Exception)
			{
				Console.WriteLine("Unexpected error occurred.");
				return 9;
			}
		}
		static void PrintUsage()
		{
			Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
		}
	}
}
