using MyWallet.Domain.Contracts.Grains;
using MyWallet.Domain.Contracts.Types;

namespace MyWallet.Domain;

public class UserGrain : Grain, IUserGrain
{
    private readonly IPersistentState<UserState> _userState;

    public UserGrain([PersistentState("mywallet", "user")] IPersistentState<UserState> userState)
    {
        _userState = userState;
    }
    
    public async Task AddWallet(string id, string walletName)
    {
        _userState.State = new UserState { Wallets = [.._userState.State.Wallets, id] };
        await _userState.WriteStateAsync();
        var newWallet = GrainFactory.GetGrain<IWalletGrain>(id);
        await newWallet.Initialize(walletName);
    }

    public async Task RemoveWallet(string walletId)
    {
        _userState.State.Wallets.Remove(walletId);
        await _userState.WriteStateAsync();
    }

    public Task Initiate()
    {
        return Task.CompletedTask;
    }

    public async Task<List<string>> GetAllWallets()
    {
        var ls = new List<string>();
        foreach (var wallet in _userState.State.Wallets)
        {
            var walletGrain = GrainFactory.GetGrain<IWalletGrain>(wallet);
            var walletState = await walletGrain.GetState();
        }
        return ls;
    }
}