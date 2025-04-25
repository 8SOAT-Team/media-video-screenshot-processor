using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using VideoScreenshot.Application.Configurations;
using VideoScreenshot.Application.Drivers;
using VideoScreenshot.Application.Requests;

namespace VideoScreenshot.MessageConsumer;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly AppConfiguration _settings;
    private readonly ILogger<SqsConsumerService> _logger;
    private readonly ITakeScreenshotService _takeScreenshotService;
    private const string QueueName = "video-start-processing";

    public SqsConsumerService(
        IAmazonSQS sqsClient,
        AppConfiguration settings,
        ILogger<SqsConsumerService> logger,
        ITakeScreenshotService takeScreenshotService)
    {
        _sqsClient = sqsClient;
        _settings = settings;
        _logger = logger;
        _takeScreenshotService = takeScreenshotService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = await _sqsClient.GetQueueUrlAsync(QueueName, stoppingToken);

        if (queueUrl is null)
        {
            _logger.LogWarning("Serviço SQS não iniciado devido à configuração inválida da QueueUrl.");
            return;
        }

        _logger.LogInformation("Serviço SQS iniciado. Aguardando mensagens em {QueueUrl}", queueUrl.QueueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl.QueueUrl,
                    MaxNumberOfMessages = _settings.MaxMessages,
                    WaitTimeSeconds = _settings.LongPollTimeSeconds,
                    // MessageAttributeNames = ["All"],
                    // MessageSystemAttributeNames = ["All"],
                };

                var receiveResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);

                foreach (var message in receiveResponse.Messages)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    var processed = await ProcessMessageAsync(message, stoppingToken);
                    if (!processed) continue;

                    var deleteRequest = new DeleteMessageRequest
                    {
                        QueueUrl = _settings.QueueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    };
                    await _sqsClient.DeleteMessageAsync(deleteRequest, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Operação cancelada. Encerrando o recebimento de mensagens.");
                break;
            }
            catch (AmazonSQSException sqsEx)
            {
                _logger.LogError(sqsEx, "Erro do SQS ao receber/excluir mensagens. Código: {ErrorCode}",
                    sqsEx.ErrorCode);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no loop de processamento SQS.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Serviço SQS encerrado.");
    }

    private async Task<bool> ProcessMessageAsync(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando mensagem ID: {MessageId}", message.MessageId);
        try
        {
            var messageRequest = JsonSerializer.Deserialize<TakeScreenshotRequest>(message.Body);
            await _takeScreenshotService.TakeScreenshot(messageRequest!);

            _logger.LogInformation("Mensagem {MessageId} processada com sucesso.", message.MessageId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao processar mensagem ID: {MessageId}", message.MessageId);
            return false;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitação de parada recebida para o Serviço SQS.");
        await base.StopAsync(cancellationToken);
    }
}