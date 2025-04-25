using System.Text.Json;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var queueUrl = Environment.GetEnvironmentVariable("QueueUrl");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var useLocalStack = config.GetValue<bool>("AWS:UseLocalStack");

    if (useLocalStack)
    {
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
    }

    return new AmazonSQSClient();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/video/start-processing",
    async ([FromBody] StartVideoProcessingMessage request, [FromServices] IAmazonSQS sqsClient) =>
    {
        try
        {
            Console.WriteLine($"Enviando mensagem para a fila: {queueUrl}");

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JsonSerializer.Serialize(request)
            };

            var response = await sqsClient.SendMessageAsync(sendMessageRequest);

            Console.WriteLine("Mensagem enviada com sucesso!");
            Console.WriteLine($"ID da Mensagem: {response.MessageId}");
        }
        catch (AmazonSQSException ex)
        {
            Console.WriteLine($"Erro ao enviar mensagem para o SQS: {ex.Message}");
            Console.WriteLine($"Código de Erro: {ex.ErrorCode}");
            Console.WriteLine($"ID da Requisição AWS: {ex.RequestId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
        }
    });

app.Run();

public record StartVideoProcessingMessage(Guid ProcessId, string VideoName);