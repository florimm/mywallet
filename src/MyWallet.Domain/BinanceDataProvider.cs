using Binance.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Sockets;
using MyWallet.Domain.Contracts.Grains;
using MyWallet.Domain.Contracts.Types;
using Orleans.Streams;

namespace MyWallet.Domain;

public class BinanceWebSocket : Grain, IMarketDataGrain
{
    private readonly IBinanceSocketClient _socketClient;
    private IStreamProvider? _streamProvider;
    private readonly Dictionary<string, UpdateSubscription> _subscriptions = new Dictionary<string, UpdateSubscription>();
    private readonly Dictionary<string, IAsyncStream<KlineData>> _streams = new();

    public BinanceWebSocket(IBinanceSocketClient client)
    {
        _socketClient = client;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _streamProvider = this.GetStreamProvider("BinanceMemoryStream");
        await SubscribeToPair("EURUSDT");
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task Track(CryptoType cryptoType)
    {
        var pair = cryptoType.ToString().ToUpperInvariant()+""+"USDT";
        if (!_streams.ContainsKey(pair))
        {
            await SubscribeToPair(pair);
        }
    }

    private async Task SubscribeToPair(string pair)
    {
        var streamId = StreamId.Create("PAIRDATA", pair);
        _streams[pair] = _streamProvider!.GetStream<KlineData>(streamId);
        var subscription = await _socketClient.SpotApi.ExchangeData.SubscribeToKlineUpdatesAsync(pair, Binance.Net.Enums.KlineInterval.OneMinute, async (data) =>
        {
            await _streams[pair].OnNextAsync(new KlineData(data.Data.Symbol, data.Data.Data.OpenPrice, data.Data.Data.ClosePrice, data.Data.Data.CloseTime));
        });
        subscription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
        subscription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
        _subscriptions.Add(pair, subscription.Data);
    }

    public async Task StopListeningAsync(string pair)
    {
        if (_subscriptions.TryGetValue(pair, out var subscription))
        {
            await _socketClient.UnsubscribeAsync(subscription);
            _subscriptions.Remove(pair);
        }
        if (_streams.ContainsKey(pair))
        {
            _streams.Remove(pair);
        }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await _socketClient.UnsubscribeAllAsync();
        await base.OnDeactivateAsync(reason, cancellationToken);
    }
}

[GenerateSerializer]
public record KlineData(string Symbol, decimal OpenPrice, decimal ClosePrice, DateTime CloseTime);