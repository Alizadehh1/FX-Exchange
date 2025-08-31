using FXExchange.Domain.Infrastructure.Exceptions;
using FXExchange.Domain.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FXExchange.Domain.Providers
{
	public sealed class ApiExchangeRateProvider : IExchangeRateProvider
	{
		private readonly HttpClient _http;
		private readonly string _endpoint;
		private readonly TimeSpan _ttl;

		private Dictionary<string, decimal>? _eurPerCurrency;
		private decimal? _dkkPerEur;
		private DateTimeOffset _loadedAt;

		private static readonly JsonSerializerOptions JsonOpts = new()
		{
			PropertyNameCaseInsensitive = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString
		};

		public ApiExchangeRateProvider(HttpClient httpClient, string endpoint, TimeSpan? ttl = null)
		{
			_http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_endpoint = string.IsNullOrWhiteSpace(endpoint)
				? throw new ArgumentNullException(nameof(endpoint))
				: endpoint;
			_ttl = ttl ?? TimeSpan.FromMinutes(15);
		}

		public async Task<decimal> GetRateAsync(string iso, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(iso))
				throw new UnknownCurrencyException(iso ?? string.Empty);

			await EnsureRatesLoadedAsync(cancellationToken);

			if (iso.Equals("DKK", StringComparison.OrdinalIgnoreCase))
				return 1m;

			var key = iso.ToLowerInvariant();
			if (!_eurPerCurrency!.TryGetValue(key, out var perEur) || perEur <= 0m)
				throw new UnknownCurrencyException(iso);

			if (_dkkPerEur <= 0m)
				throw new InvalidOperationException("Invalid DKK per EUR rate loaded from API.");

			return _dkkPerEur!.Value / perEur;
		}

		private async Task EnsureRatesLoadedAsync(CancellationToken cancellationToken)
		{
			if (_eurPerCurrency is not null &&
				_dkkPerEur is not null &&
				(DateTimeOffset.UtcNow - _loadedAt) < _ttl)
			{
				return;
			}

			using var response = await _http.GetAsync(_endpoint, cancellationToken);
			response.EnsureSuccessStatusCode();

			using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			var payload = await JsonSerializer.DeserializeAsync<ApiPayload>(stream, JsonOpts, cancellationToken)
						  ?? throw new InvalidOperationException("API returned empty or invalid response.");

			if (payload.Eur is null || payload.Eur.Count == 0)
				throw new InvalidOperationException("API payload missing 'eur' section.");

			if (!payload.Eur.TryGetValue("dkk", out var dkkPerEur) || dkkPerEur <= 0m)
				throw new InvalidOperationException("API payload missing or invalid 'dkk' rate.");

			_eurPerCurrency = payload.Eur;
			_dkkPerEur = dkkPerEur;
			_loadedAt = DateTimeOffset.UtcNow;
		}

		private sealed class ApiPayload
		{
			[JsonPropertyName("date")]
			public string? Date { get; set; }

			[JsonPropertyName("eur")]
			public Dictionary<string, decimal>? Eur { get; set; }
		}
	}
}
