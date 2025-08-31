using FXExchange.Domain.Infrastructure.Exceptions;
using FXExchange.Domain.Core;
using System.Reflection;
using System.Text.Json;

namespace FXExchange.Domain.Providers
{
	public class JsonEmbeddedExchangeRateProvider : IExchangeRateProvider
	{
		private readonly Dictionary<string, decimal> _rate;
		private const string DefaultResourceName = "FXExchange.Domain.Data.rates.json";

		public JsonEmbeddedExchangeRateProvider(string? resourceName = null)
		{
			var asm = Assembly.GetExecutingAssembly();
			var resName = resourceName ?? DefaultResourceName;

			using var stream = asm.GetManifestResourceStream(resName)
				?? throw new InvalidOperationException(
					$"Embedded resource not found: {resName}\n" +
					$"Hint: ensure <EmbeddedResource Include=\"Data\rates.json\" /> and default namespace match."
				);

			using var doc = JsonDocument.Parse(stream);
			var root = doc.RootElement;

			if (!root.TryGetProperty("dkkPerUnit", out var dkkMap))
				throw new InvalidOperationException("rates.json must contain 'dkkPerUnit' object.");

			var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
			foreach (var p in dkkMap.EnumerateObject())
				dict[p.Name] = p.Value.GetDecimal();

			dict["DKK"] = 1m;

			_rate = dict;
		}

		public Task<decimal> GetRateAsync(string iso, CancellationToken cancellationToken = default)
		{
			if (!_rate.TryGetValue(iso, out var value))
				throw new UnknownCurrencyException(iso);

			return Task.FromResult(value);
		}
	}
}
