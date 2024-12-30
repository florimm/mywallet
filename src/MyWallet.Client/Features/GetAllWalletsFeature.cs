using Microsoft.AspNetCore.Mvc;
using MyWallet.Domain.Contracts.Grains;

namespace MyWallet.Client.Features;

public static class GetAllWalletsFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapGet("users/{userId}/wallets", Handle)
            .WithTags("GetWallets");

    private static async Task<IResult> Handle([FromRoute] string userId, [FromServices] IClusterClient clusterClient)
    {
        var userGrain = clusterClient.GetGrain<IUserGrain>(userId);
        var wallets = await userGrain.GetAllWallets();
        return Results.Ok(wallets);
    }
}