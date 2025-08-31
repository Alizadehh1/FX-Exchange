namespace FXExchange.Domain.Infrastructure.Exceptions
{
	public class UnknownCurrencyException : Exception
	{
		public UnknownCurrencyException(string iso) : base($"Unknown currency: {iso}.") { }
	}
}
