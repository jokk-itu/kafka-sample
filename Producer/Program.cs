using Application;
using Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices((context, services) =>
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    var kafkaConfiguration = builder.Configuration.GetSection(nameof(Kafka)).Get<Kafka>();
    services.AddMassTransit(busConfig =>
    {
        busConfig.UsingInMemory((busContext, busFactoryConfig) =>
        {
            busFactoryConfig.ConfigureEndpoints(busContext);
        });
        busConfig.AddRider(riderConfig =>
        {
            riderConfig.AddProducer<Message>(nameof(Message));
            riderConfig.UsingKafka((_, kafkaConfig) =>
            {
                kafkaConfig.Host(kafkaConfiguration.Host);
            });
        });
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/message", async (ITopicProducer<Message> topicProducer, CancellationToken cancellationToken) =>
    {
        await topicProducer.Produce(new Message
        {
            Body = "Hello World"
        }, cancellationToken: cancellationToken);
        return Results.Ok();
    })
    .WithName("Enter");

app.Run();