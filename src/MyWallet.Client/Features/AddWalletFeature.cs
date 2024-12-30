using Microsoft.AspNetCore.Mvc;
using MyWallet.Domain.Contracts.Grains;
using MyWallet.Domain.Contracts.Types;

namespace MyWallet.Client.Features;

public static class RegisterWalletFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapPost("users/{userId}/wallets", Handle)
            .WithTags("AddWallet");

    private static async Task<IResult> Handle([AsParameters] AddWalletRequest request, [FromServices] IClusterClient clusterClient)
    {
        var userGrain = clusterClient.GetGrain<IUserGrain>(request.UserId);
        var userWalletId = new UserWalletId(request.UserId, Guid.NewGuid().ToString());
        await userGrain.AddWallet(userWalletId.Id, request.Body.Name);
        return Results.Ok();
    }
    
    public record AddWalletRequest(
        [FromRoute] string UserId,
        [FromBody] AddWalletRequest.NewWallet Body)
    {
        public record NewWallet(string Name);
    }
}