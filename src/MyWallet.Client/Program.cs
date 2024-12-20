var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/{userId}/wallets", (string userId) =>
        Results.Ok(new List<string>())).WithName("GetWalletsForUser");

app.MapGet("/{userId}/wallets/{walletId}", (string userId, string walletId) =>
        Results.Ok("")).WithName("GetSpecificWalletForUser");

app.MapPost("/{userId}/wallets/{walletId}", (string userId, string walletId) =>
    Results.Ok("")).WithName("AddNewPairToTrack");

app.MapDelete("/{userId}/wallets/{walletId}/{pair}", (string userId, string walletId, string pair) =>
    Results.Ok("")).WithName("RemovePair");

app.Run();
