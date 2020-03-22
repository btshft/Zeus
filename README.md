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
