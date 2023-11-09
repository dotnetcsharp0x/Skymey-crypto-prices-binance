using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Skymey_main_lib.Models.Prices.Binance;
using Skymey_main_lib.Models.Prices.Polygon;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Skymey_crypto_prices_binance.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<TickerPricesBinance> Ticker { get; init; }
        public static ApplicationContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<ApplicationContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TickerPricesBinance>().ToCollection("crypto_actual_prices");
        }
    }
}
