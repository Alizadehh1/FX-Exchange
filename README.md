# FXExchange

FXExchange is a command-line currency converter built in C# using .NET 9.  
Technical task for DanskeBank

## Features

- Converts currency between supported ISO codes (e.g., EUR/USD, USD/DKK)
- Accepts both comma (`,`) and dot (`.`) decimal formats depending on culture
- Switchable between:
  - **Embedded JSON-based provider** (offline/fallback)
  - **Live API-based provider** (real-time, with TTL caching)
- Input validation and meaningful exceptions
- Robust error handling and fallback logic
- Culture-aware rounding and formatting (Danish culture)
- Dependency injection and configuration support
- Full test suite using xUnit and TDD principles

## Requirements

- .NET 9 SDK or later
- Optional: Internet access for API-based rates

## Usage

From the command line, run:

```bash
Exchange <currency pair> <amount to exchange>
