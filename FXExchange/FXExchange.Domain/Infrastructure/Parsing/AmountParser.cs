using FXExchange.Domain.Infrastructure.Globalization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FXExchange.Domain.Infrastructure.Parsing
{
	public static partial class AmountParser
	{
		[GeneratedRegex(@"^[+-]?\d+([.,]\d+)?$")]
		private static partial Regex ValidFormat();

		public static bool TryParse(string input, out decimal value)
		{
			value = 0;

			if (!ValidFormat().IsMatch(input))
				return false;

			return decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value)
				|| decimal.TryParse(input, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, AppCultures.Danish, out value);
		}
	}
}
