using Microsoft.AspNetCore.Mvc;
using MyWallet.Domain.Contracts.Grains;
using MyWallet.Domain.Contracts.Types;

namespace MyWallet.Client.Features;

public static class AddInvestmentFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapPost("users/{userId}/wallets/{walletId}/investments", Handle)
            .WithTags("AddInvestment");

    private static async Task<IResult> Handle([AsParameters] AddInvestmentRequest request, [FromServices] IClusterClient clusterClient)
    {
        var walletGrain = clusterClient.GetGrain<IWalletGrain>(request.WalletId);
        await walletGrain.AddInvestment(request.Body.CryptoType, request.Body.Amount);
        return Results.Ok();
    }

    public record AddInvestmentRequest(
        [FromRoute] string UserId,
        [FromRoute] string WalletId,
        [FromBody] AddInvestmentRequest.Investment Body)
    {
        public record Investment(CryptoType CryptoType, decimal Amount);
    }
}