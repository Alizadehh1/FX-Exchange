namespace FXExchange.Domain
{
	public interface IExchangeRateProvider
	{
		decimal GetDkkPerUnit(string iso);
	}
}
