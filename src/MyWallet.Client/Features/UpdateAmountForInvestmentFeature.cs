using Microsoft.AspNetCore.Mvc;
using MyWallet.Domain.Contracts.Grains;
using MyWallet.Domain.Contracts.Types;

namespace MyWallet.Client.Features;

public static class UpdateAmountForInvestmentFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapPut("users/{userId}/wallets/{walletId}/investments/{cryptoType}", Handle)
            .WithTags("UpdateAmountForInvestment");

    private static async Task<IResult> Handle([AsParameters] ChangeAmountForInvestmentRequest request, [FromServices] IClusterClient client)
    {
        var walletGrain = client.GetGrain<IWalletGrain>(request.WalletId);
        await walletGrain.UpdateAmount(request.CryptoType, request.Body.Amount);
        return Results.Ok();
    }
    
    public record ChangeAmountForInvestmentRequest(
        [FromRoute] string UserId,
        [FromRoute] string WalletId,
        [FromRoute] CryptoType CryptoType,
        [FromBody] ChangeAmountForInvestmentRequest.Investment Body)
    {
        public record Investment(decimal Amount);
    }
}