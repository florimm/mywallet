using Binance.Net.Interfaces.Clients;
using MyWallet.Domain.Contracts.Grains;
using Orleans.Streams;

namespace MyWallet.Domain;

[ImplicitStreamSubscription("PAIRDATA")]
public class PairKlineConsumerGrain : Grain, IPairKlineConsumerGrain
{
    private readonly IBinanceRestClient binanceRestClient;
    private string? _pair;
    public PairKlineConsumerGrain(IBinanceRestClient binanceRestClient)
    {
        this.binanceRestClient = binanceRestClient;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _pair = this.GetPrimaryKeyString();
        Console.WriteLine($"Consumer grain activated for pair: {_pair}");
        var streamProvider = this.GetStreamProvider("BinanceMemoryStream");
        var streamId = StreamId.Create("PAIRDATA", _pair);
        var stream = streamProvider.GetStream<KlineData>(streamId);
        await stream.SubscribeAsync(OnNextAsync);
    }

    public Task OnNextAsync(KlineData klineData, StreamSequenceToken? token = null)
    {
        Console.WriteLine($"Received update for {_pair}: Open: {klineData.OpenPrice}, Close: {klineData.ClosePrice}");
        return Task.CompletedTask;
    }
}