using FXExchange.Domain.Core;

namespace FXExchange.Domain.Providers
{
	public class FallbackExchangeRateProvider : IExchangeRateProvider
	{
		private readonly IExchangeRateProvider _primary;
		private readonly IExchangeRateProvider _fallback;

		public FallbackExchangeRateProvider(IExchangeRateProvider primary, IExchangeRateProvider fallback)
		{
			_primary = primary ?? throw new ArgumentNullException(nameof(primary));
			_fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
		}
		public async Task<decimal> GetRateAsync(string iso, CancellationToken cancellationToken = default)
		{
			try
			{
				return await _primary.GetRateAsync(iso, cancellationToken);
			}
			catch
			{
				return await _fallback.GetRateAsync(iso, cancellationToken);
			}
		}
	}
}
