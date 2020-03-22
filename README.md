# Zeus
Telegram bot for Prometheus alerts

## Status
Work in progress. Hope to roll out somethinng usable within a month. 

## Features
* Telegram API update method
  * Polling
* Proxy support
  * Socks5
  * Http
* Notifications routing based on alert labels
* Rich notification tempate customization powered by [Scriban](https://github.com/lunet-io/scriban)
* Persistent bot state storage 
  * Key-value file storage - [LiteDb](https://www.litedb.org/)
* Security 
  * Username based command authorization

### Third-party software
1. [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) - the Bot API is an HTTP-based interface created for developers keen on building bots for Telegram. License: MIT
2. [HttpToSocks5Proxy](https://github.com/MihaZupan/HttpToSocks5Proxy) - the library that allows to connect over Socks5 proxies when using the .NET HttpClient. License: MIT.
3. [MediatR](https://github.com/jbogard/MediatR) - simple mediator implementation in .NET. License: Apache 2.0
4. [LiteDB](https://github.com/mbdavid/litedb) - LiteDB is a small, fast and lightweight .NET NoSQL embedded database. License: MIT.
5. [Scriban](https://github.com/lunet-io/scriban) - Scriban is a fast, powerful, safe and lightweight text templating language and engine for .NET, with a compatibility mode for parsing liquid templates. License: BSD 2.
6. [Nito.AsyncEx](https://github.com/StephenCleary/AsyncEx) - A helper library for async/await. License: MIT. 
