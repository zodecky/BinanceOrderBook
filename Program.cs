using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using WebSocket4Net;

class Program
{
    private static Dictionary<string, decimal> orderBook = new Dictionary<string, decimal>();
    private static long lastUpdateId = 0;

    static void Main(string[] args)
    {
        GetInitialOrderBook();

        var websocket = new WebSocket("wss://stream.binance.com:9443/ws/bnbbtc@depth"); // api
        websocket.MessageReceived += Websocket_MessageReceived; // attach the event handler
        websocket.Open();
        Console.ReadLine();
    }

    /*
        This method is called by the Main first, so the order book is initialized.
    */
    private static void GetInitialOrderBook()
    {

        using (var client = new HttpClient())
        {
            // Open connection to Binance API and get the order book
            var response = client.GetAsync("https://api.binance.com/api/v3/depth?symbol=BNBBTC&limit=1000").Result; 
            var content = response.Content.ReadAsStringAsync().Result; 
            var json = JObject.Parse(content);
            lastUpdateId = json?.Value<long>("lastUpdateId") ?? 0;


            // Get the bids and asks and add them to the order book (dictionary)
            var bids = json?["bids"];
            if (bids != null)
            {
                foreach (var bid in bids)
                {
                    orderBook[bid[0]?.Value<string>() ?? ""] = bid[1]?.Value<decimal>() ?? 0;
                }
            }

            // Add the asks too
            var asks = json?["asks"];
            if (asks != null)
            {
                foreach (var ask in asks)
                {
                    orderBook[ask[0]?.Value<string>() ?? ""] = ask[1]?.Value<decimal>() ?? 0;
                }
            }
        }
    }

    /*
        **ASYNC**
        This method is called every time a message is received from the websocket.
    */
    private static void Websocket_MessageReceived(object? sender, MessageReceivedEventArgs e)
    {

        if (sender == null) 
        {
            Console.WriteLine("Sender is null");
            return;
        }

        var json = JObject.Parse(e.Message);

        // Console.WriteLine("json: " + json); // <-- Uncomment this to see the json

        var U = json?.Value<long>("U") ?? 0;
        var u = json?.Value<long>("u") ?? 0;

        if (u > lastUpdateId) // BUG: should have also U >= lastUpdateId + 1, but seems to never happen
        {

            lastUpdateId = u;


            // Get bids from JSON and update the order book
            var bids = json?["b"];
            if (bids != null)
            {
                foreach (var bid in bids)
                {
                    var price = bid[0]?.Value<string>();
                    var quantity = bid[1]?.Value<decimal>() ?? 0;
                    if (quantity == 0)
                    {
                        if (price != null)
                        {
                            orderBook.Remove(price);
                        }
                    }
                    else
                    {
                        if (price != null)
                        {
                            orderBook[price] = quantity;
                        }
                    }
                }
            }

            // Get asks from JSON and update the order book
            var asks = json?["a"];
            if (asks != null)
            {
                foreach (var ask in asks)
                {
                    var price = ask[0]?.Value<string>();
                    var quantity = ask[1]?.Value<decimal>() ?? 0;
                    if (quantity == 0)
                    {
                        if (price != null)
                        {
                            orderBook.Remove(price);
                        }
                    }
                    else
                    {
                        if (price != null)
                        {
                            orderBook[price] = quantity;
                        }
                    }
                }
            }

            // Clear, to visualize the order book better
            Console.Clear();

            // Print the entire order book
            foreach (var entry in orderBook)
            {
                Console.WriteLine($"Price: {entry.Key}, Quantity: {entry.Value}");
            }
        }
    }
}