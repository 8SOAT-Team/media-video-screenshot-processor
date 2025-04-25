using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Driven.MessageBrokers;
using VideoScreenshot.Domain.Results;

namespace VideoScreenshot.Infrastructure.Driven.MessageBrokers;

public sealed class EventBusService(IAmazonSQS amazonSqs, AppConfiguration appConfig) : IEventBusService
{
    public async Task<OperationResult> Publish<T>(T message, CancellationToken cancellation = default)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = appConfig.PublishEventQueueUrl,
            MessageBody = JsonSerializer.Serialize(message)
        };

        var response = await amazonSqs.SendMessageAsync(sendMessageRequest, cancellation);

        return ((int)response.HttpStatusCode) >= 200 && ((int)response.HttpStatusCode) < 300
            ? OperationResult.Success("Mensagem publicada com sucesso")
            : OperationResult.Fail("Falha ao publicar mensagem");
    }
}