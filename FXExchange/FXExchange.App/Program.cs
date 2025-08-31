using FXExchange.App.Commands;
using FXExchange.Domain.Core;
using FXExchange.Domain.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var		builder		= Host.CreateApplicationBuilder(args);
var		cfg			= builder.Configuration.GetSection("Rates");
bool	useApi		= cfg.GetValue("UseApi", true);
string	endpoint	= cfg.GetValue<string>("Endpoint")!;
int		ttl			= cfg.GetValue("TtlMinutes", 15);

builder.Services.AddHttpClient<ApiExchangeRateProvider>(c =>
{
	c.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddSingleton<JsonEmbeddedExchangeRateProvider>();
builder.Services.AddSingleton<IExchangeRateProvider>(sp =>
{
	var json = sp.GetRequiredService<JsonEmbeddedExchangeRateProvider>();
	if (!useApi || string.IsNullOrWhiteSpace(endpoint))
		return json;

	var api = ActivatorUtilities.CreateInstance<ApiExchangeRateProvider>(
		sp, sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(ApiExchangeRateProvider)), endpoint, TimeSpan.FromMinutes(ttl));

	return new FallbackExchangeRateProvider(api, json);
});
builder.Services.AddSingleton<CurrencyConverter>();
builder.Services.AddSingleton<ExchangeCommand>();

var app = builder.Build();

var command = app.Services.GetRequiredService<ExchangeCommand>();
int code	= await command.RunAsync(args);
Environment.Exit(code);
