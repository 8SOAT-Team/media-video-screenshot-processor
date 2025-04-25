using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Infrastructure.Configurations;
using VideoScreenshot.MessageConsumer;

var builder = WebApplication.CreateBuilder(args);

using var factory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
});

var earlyLogger = factory.CreateLogger<Program>();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var environment = builder.Environment.EnvironmentName;
earlyLogger.LogInformation("[Startup] ASPNETCORE_ENVIRONMENT: {environment}", environment);

var appConfiguration = builder.Configuration.GetSection(AppConfiguration.ConfigurationSectionName)
    .Get<AppConfiguration>();

builder.Logging.AddConsole();

builder.Services.AddSingleton(appConfiguration!);

builder.Services
    .AddSingleton<IValidateOptions<AppConfiguration>, ValidateSettingsOptions>();

builder.Services.AddApplicationPorts(appConfiguration!.FfmpegPath);
builder.Services.AddInfrastructurePorts(appConfiguration);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var useLocalStack = config.GetValue<bool>("AWS:UseLocalStack");

    if (!useLocalStack) return new AmazonSQSClient();
    
    var serviceUrl = config["AWS:ServiceURL"];
    var accessKey = config["AWS:AccessKey"];
    var secretKey = config["AWS:SecretKey"];

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    var sqsConfig = new AmazonSQSConfig
    {
        ServiceURL = serviceUrl,
        UseHttp = true
    };

    return new AmazonSQSClient(credentials, sqsConfig);
});

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<SqsConsumerService>();

var app = builder.Build();

if (AppConstants.TempFilePath.Exists is false)
{
    AppConstants.TempFilePath.Create();
}

app.MapHealthChecks("/health");

earlyLogger.LogInformation("[Startup] App iniciado: {environment}", environment);

app.Run();

public partial class Program { }