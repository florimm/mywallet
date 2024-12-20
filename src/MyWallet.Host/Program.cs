// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder
        //.AddMemoryStreams("StreamProvider")
        .AddMemoryGrainStorage("wallet");
});

var host = builder.Build();
host.Run();