using FXExchange.Domain.Infrastructure.Parsing;

namespace FXExchange.Tests.Infrastructure
{
	public class AmountParserTests
	{
		[Theory]
		[InlineData("1", 1)]
		[InlineData("1.5", 1.5)]
		[InlineData("1,5", 1.5)]
		[InlineData("123.45", 123.45)]
		[InlineData("123,45", 123.45)]
		public void Parses_Valid_Forms(string input, decimal expected)
		{
			var ok = AmountParser.TryParse(input, out var value);
			Assert.True(ok);
			Assert.Equal(expected, value);
		}

		[Theory]
		[InlineData("1e3")]
		[InlineData("1@#!")]
		[InlineData("pi")]
		[InlineData("tau")]
		[InlineData("1+")]
		[InlineData("abc")]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData("1.000,5")] 
		[InlineData("1,2.3")]  
		[InlineData(".5")]  
		public void Rejects_Invalid_Forms(string input)
		{
			var ok = AmountParser.TryParse(input, out _);
			Assert.False(ok);
		}
	}
}
