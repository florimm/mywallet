using MyWallet.Domain.Contracts.Types;

namespace MyWallet.Domain.Contracts.Grains;

public interface IMarketDataGrain : IGrainWithStringKey
{
    Task Track(CryptoType cryptoType);
    Task StopListeningAsync(string pair);
}

public interface INotifierGrain : IGrainWithStringKey
{
    Task NotifyAsync(string message);
    Task<List<string>> GetAllNotificationsAsync();
}

public interface IWalletGrain : IGrainWithStringKey
{
    Task<WalletState> GetState();

    Task AddInvestment(CryptoType cryptoType, decimal amount);
    Task RemoveInvestment(CryptoType cryptoType);

    Task UpdateAmount(CryptoType cryptoType, decimal amount);
    Task Initialize(string walletName);
}

public interface IUserGrain : IGrainWithStringKey
{
    Task<List<string>> GetAllWallets();
    Task AddWallet(string id, string walletName);

    Task RemoveWallet(string walletId);

    Task Initiate();
}

public interface IPairKlineConsumerGrain : IGrainWithStringKey
{
}