﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skymey_crypto_prices_binance.Actions.Prices;

namespace Skymey_crypto_prices_binance
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IHostedService, MySpecialService>();
                });
            await builder.RunConsoleAsync();
        }
    }
    public class MySpecialService : BackgroundService
    {
        GetPrices gp = new GetPrices();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    gp.GetPricesFromBinance();
                    await Task.Delay(TimeSpan.FromSeconds(0));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
