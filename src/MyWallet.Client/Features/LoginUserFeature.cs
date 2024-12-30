using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MyWallet.Domain.Contracts.Grains;

namespace MyWallet.Client.Features;

public static class LoginUserFeature
{
    public static void Register(IEndpointRouteBuilder endpoints) =>
        endpoints
            .MapPost("login", Handle)
            .WithTags("Login");

    private static async Task<IResult> Handle([FromBody] LoginRequest request, [FromServices] IClusterClient client)
    {
        var userClient = client.GetGrain<IUserGrain>(request.Email.HashEmail());
        await userClient.Initiate();
        return Results.Ok(request.Email.HashEmail());
    }
    
    private static string HashEmail(this string email)
    {
        using SHA256 sha256 = SHA256.Create();
        var emailBytes = Encoding.UTF8.GetBytes(email);
        var hashBytes  = sha256.ComputeHash(emailBytes);
        return Convert.ToBase64String(hashBytes);
    }
    
    public record LoginRequest(string Email);
}