using Contracts;
using MassTransit;

namespace Consumer;
public class MessageConsumer : IConsumer<Message>
{
    private readonly ILogger<MessageConsumer> _logger;

    public MessageConsumer(ILogger<MessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Message> context)
    {
        _logger.LogInformation("Received at {} with {}", DateTime.UtcNow, context.Message.Body);
        return Task.CompletedTask;
    }
}