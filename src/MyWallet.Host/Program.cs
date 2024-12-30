using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Serialization;
using Orleans.Streams;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, silo) =>
    {
        silo.UseLocalhostClustering()
        .AddAdoNetGrainStorage("wallet", options =>
        {
            var userStorage = context.Configuration.GetConnectionString("userStorage");
            options.ConnectionString = userStorage;
            options.Invariant = "Npgsql";
        })
        .AddAdoNetGrainStorage("user", options =>
        {
            var walletStorage = context.Configuration.GetConnectionString("walletStorage");
            options.ConnectionString = walletStorage;
            options.Invariant = "Npgsql";
        })
        .AddMemoryStreams("BinanceMemoryStream", options =>
        {
            options.ConfigureStreamPubSub(StreamPubSubType.ExplicitGrainBasedAndImplicit);
        })
        .AddMemoryGrainStorage("PubSubStore")
        .ConfigureLogging(logging => logging.AddConsole())
        .UseLocalhostClustering();
    })
    .UseConsoleLifetime();

builder.ConfigureServices((context, services) =>
{
    services.AddBinance(x =>
    {
        var key = context.Configuration.GetValue<string>("CryptoKeys:Key")!;
        var secrets = context.Configuration.GetValue<string>("CryptoKeys:Secret")!;
        x.ApiCredentials = new ApiCredentials(key, secrets);
    });
    services.AddSerializer(serializerBuilder =>
    {
        serializerBuilder.AddJsonSerializer(
            isSupported: type => type.Namespace.StartsWith("Domain"));
    });
});

using IHost host = builder.Build();

await host.RunAsync();