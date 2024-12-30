using MyWallet.Client.Common;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.UseOrleansClient(client =>
{
    client.UseLocalhostClustering();
});
var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.LoadFeatures();

app.Run();
