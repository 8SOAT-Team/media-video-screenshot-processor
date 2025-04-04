using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.LocalStack;
using Wrapr;

namespace VideoScreenshot.Tests.Integration.Applications;

public sealed class ConsumerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // private readonly int _daprHttpPort = 3023;
    // private readonly int _daprGrpcPort = 3024;
    // private const int AppPort = 5148;

    private readonly LocalStackContainer _localStackContainer = new LocalStackBuilder()
        .WithEnvironment("SERVICES", "s3,sqs,sns,sts")
        .WithExposedPort(4566)
        .Build();

    // private readonly Sidecar _sidecar = new();

    public async Task InitializeAsync()
    {
        await _localStackContainer.StartAsync();
        // await StartSidecar();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        builder.UseConfiguration(configuration);

        base.ConfigureWebHost(builder);
    }

    public new async Task DisposeAsync()
    {
        // await _sidecar.Stop();
        await _localStackContainer.DisposeAsync();
    }

    // private async Task StartSidecar()
    // {
    //     await _sidecar.Start(with => with
    //         .ResourcesPath(Path.Combine(Directory.GetCurrentDirectory(), "Integration/Components"))
    //         .AppPort(AppPort)
    //         .DaprHttpPort(_daprHttpPort)
    //         .DaprGrpcPort(_daprGrpcPort)
    //         .Args("--log-level", "debug"));
    // }
}