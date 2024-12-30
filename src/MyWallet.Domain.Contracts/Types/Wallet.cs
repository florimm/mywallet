namespace MyWallet.Domain.Contracts.Types;

[GenerateSerializer]
public record WalletState(string Name, List<WalletState.Investment> Investments)
{
    [GenerateSerializer]
    public record Investment(CryptoType Crypto, decimal Amount);
}

[GenerateSerializer]
public enum CryptoType
{
    BTC,
    ETH,
    LINK,
    ADA,
    SUI,
    USDT,
    TSDT,
        
}

[GenerateSerializer]
public class UserState
{
    public List<string>? Wallets { get; set; }
}

[GenerateSerializer]
public record UserWalletId(string UserId, string WalletId)
{
    public string Id => UserId + "-" + WalletId;
}

[GenerateSerializer]
public record Pair();