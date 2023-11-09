using Binance.Net.Clients;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models;
using CryptoExchange.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Nancy.Json;
using RestSharp;
using Skymey_crypto_prices_binance.Data;
using Skymey_main_lib.Models.Prices.Binance;
using Skymey_main_lib.Models.Prices.Polygon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skymey_crypto_prices_binance.Actions.Prices
{
    public class GetPrices
    {
        private RestClient _client;
        private RestRequest _request;
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        public GetPrices()
        {
            _client = new RestClient("https://api.binance.com/api/v1/ticker/24hr");
            _request = new RestRequest("https://api.binance.com/api/v1/ticker/24hr", Method.Get);
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }
        public async Task GetPricesFromBinance()
        {
            _request.AddHeader("Content-Type", "application/json");
            var r = _client.Execute(_request).Content;
            List<TickerPricesBinance> ticker = new JavaScriptSerializer().Deserialize<List<TickerPricesBinance>>(r);
            var test = ticker;
            //List<TickerPricesBinance> ticker = [ticker1];
            //var resp = new JavaScriptSerializer().Serialize(spotTickerData);
            foreach (var tickers in ticker)
            {
                Console.WriteLine(tickers.symbol);
                var ticker_find = (from i in _db.Ticker where i.symbol == tickers.symbol select i).FirstOrDefault();

                if (ticker_find == null)
                {
                    tickers._id = ObjectId.GenerateNewId();
                    tickers.Update = DateTime.UtcNow;
                    _db.Ticker.Add(tickers);
                }
                else
                {
                    ticker_find.bidPrice = tickers.bidPrice;
                    ticker_find.bidQty = tickers.bidQty;
                    ticker_find.askPrice = tickers.askPrice;
                    ticker_find.askQty = tickers.askQty;
                    ticker_find.volume = tickers.volume;
                    ticker_find.quoteVolume = tickers.quoteVolume;
                    ticker_find.priceChange = tickers.priceChange;
                    ticker_find.priceChangePercent = tickers.priceChangePercent;
                    ticker_find.weightedAvgPrice = tickers.weightedAvgPrice;
                    ticker_find.lastPrice = tickers.lastPrice;
                    ticker_find.lastQty = tickers.lastQty;
                    ticker_find.openPrice = tickers.openPrice;
                    ticker_find.highPrice = tickers.highPrice;
                    ticker_find.lowPrice = tickers.lowPrice;
                    ticker_find.openTime = tickers.openTime;
                    ticker_find.closeTime = tickers.closeTime;
                    ticker_find.lastId = tickers.lastId;
                    ticker_find.count = tickers.count;
                    ticker_find.Update = DateTime.UtcNow;
                    _db.Ticker.Update(ticker_find);
                }

            }
            _db.SaveChanges();
        }
    }
}
