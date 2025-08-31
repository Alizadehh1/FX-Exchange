namespace FXExchange.Domain.Core
{
	public interface IExchangeRateProvider
	{
		Task<decimal> GetRateAsync(string iso, CancellationToken cancellationToken = default);
	}
}
