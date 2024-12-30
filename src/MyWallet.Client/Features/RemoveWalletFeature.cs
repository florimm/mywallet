using Microsoft.AspNetCore.Mvc;
using MyWallet.Domain.Contracts.Grains;

namespace MyWallet.Client.Features;

public static class RemoveWalletFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapDelete("users/{userId}/wallets/{walletId}", Handle)
            .WithTags("RemoveWallet");

    private static async Task<IResult> Handle([FromRoute] string userId, [FromRoute] string walletId, [FromServices] IClusterClient client)
    {
        var userGrain = client.GetGrain<IUserGrain>(userId);
        await userGrain.RemoveWallet(walletId);
        return Results.Ok();
    }
}