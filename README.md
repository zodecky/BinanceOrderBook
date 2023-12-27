# Binance Order Book (requires internet connection)

This project connects to the Binance API, retrieves an initial snapshot of the order book for the BNBBTC trading pair, and then continuously updates that order book in real time based on the stream of updates received over the WebSocket connection.

It prints the order book to the console

## Prerequisites

- .NET Core SDK: You can download it from [here](https://dotnet.microsoft.com/download).

#### WebSocket4Net
```bash
dotnet add package WebSocket4Net
```

#### Newtonsoft.Json
```bash
dotnet add package Newtonsoft.Json
```

## Usage

#### Run (make sure you are in the project directory)
```bash
dotnet run
```
