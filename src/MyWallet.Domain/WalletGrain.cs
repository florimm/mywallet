using MyWallet.Domain.Contracts.Grains;
using MyWallet.Domain.Contracts.Types;

namespace MyWallet.Domain;

public class WalletGrain : Grain, IWalletGrain
{
    private readonly IPersistentState<WalletState> _walletState;

    public WalletGrain([PersistentState("mywallet", "wallet")] IPersistentState<WalletState> walletState)
    {
        _walletState = walletState;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var cryptos = _walletState.State.Investments.Select(x => x.Crypto).Distinct();
        var dataProviderGrain = this.GrainFactory.GetGrain<IMarketDataGrain>("0");
        foreach (var crypto in cryptos)
        {
            dataProviderGrain.Track(crypto);
        }
        return base.OnActivateAsync(cancellationToken);
    }

    public Task<decimal> GetBalance()
    {
        return Task.FromResult(0m);
    }

    public Task AddInvestment(CryptoType cryptoType, decimal amount)
    {
        _walletState.State = _walletState.State with { Investments = [.._walletState.State.Investments, new WalletState.Investment(cryptoType, amount)] };
        return Task.CompletedTask;
    }
    
    public Task UpdateAmount(CryptoType cryptoType, decimal amount)
    {
        var updatedInvestments = new List<WalletState.Investment>();
        foreach (var investment in _walletState.State.Investments)
        {
            if (investment.Crypto == cryptoType)
            {
                updatedInvestments.Add(investment with { Amount = amount });
            }
            else
            {
                updatedInvestments.Add(new WalletState.Investment(investment.Crypto, investment.Amount));
            }
        }
        _walletState.State = _walletState.State with { Investments = updatedInvestments };
        return Task.CompletedTask;
    }
    
    public Task RemoveInvestment(CryptoType cryptoType)
    {
        _walletState.State = _walletState.State with { Investments = [.._walletState.State.Investments.Where(x => x.Crypto != cryptoType)] };
        return Task.CompletedTask;
    }

    public Task<WalletState> GetState()
    {
        return Task.FromResult(this._walletState.State);
    }

    public Task Initialize(string walletName)
    {
        _walletState.State = new WalletState(walletName, []);
        return Task.CompletedTask;
    }
}