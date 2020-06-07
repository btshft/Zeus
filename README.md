# About
Telegram bot for [Prometheus](https://prometheus.io/) alerts. Bot provides a flexible and convenient way to send alerts to Telegram.

## Status
Work in progress. 

## Features
1. Channels & Subscriptions. Publish alerts to different channels using signle app, manage channel subscriptions from Telegram using commands.
2. Docker ready. Easy to start using app with Docker.
3. Localization (only russian now).
4. Message template customization. Rich and simple template language, customized templates for different locales and channels.
5. Transparent API. Debug and test alerts even without Alertmanager via Swagger.

## Quickstart
See [examples]() directory to get started with Zeus.

## Commands
1. `/echo` - displays information about incoming request. Use this command to get Telegram metadata like chat or user identifier.
2. `/subscibe {channel}` - subscribe current chat to receive notifications sent to the appropriate channel.
3. `/unsubscribe {channel}` - unsubscribe current chat from receiving notifications.

## Third-party software
1. [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) - the Bot API is an HTTP-based interface created for developers keen on building bots for Telegram. License: MIT
2. [HttpToSocks5Proxy](https://github.com/MihaZupan/HttpToSocks5Proxy) - the library that allows to connect over Socks5 proxies when using the .NET HttpClient. License: MIT.
3. [MediatR](https://github.com/jbogard/MediatR) - simple mediator implementation in .NET. License: Apache 2.0
5. [Scriban](https://github.com/lunet-io/scriban) - Scriban is a fast, powerful, safe and lightweight text templating language and engine for .NET, with a compatibility mode for parsing liquid templates. License: BSD 2.
