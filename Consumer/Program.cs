using Application;
using Consumer;
using Contracts;
using MassTransit;

var builder = Host.CreateDefaultBuilder(args);
var host = builder
    .ConfigureServices((hostContext, services) =>
    {
        var kafkaConfiguration = hostContext.Configuration.GetSection(nameof(Kafka)).Get<Kafka>();
        services.AddMassTransit(busConfig =>
        {
            busConfig.UsingInMemory((busContext, busFactoryConfig) =>
            {
                busFactoryConfig.ConfigureEndpoints(busContext);
            });
            busConfig.AddRider(riderConfig =>
            {
                riderConfig.AddConsumer<MessageConsumer>(consumerConfig =>
                {
                });
                riderConfig.UsingKafka((riderContext, kafkaConfig) =>
                {
                    kafkaConfig.Host(kafkaConfiguration.Host);
                    kafkaConfig.TopicEndpoint<Message>(nameof(Message), "Consumer", endpointConfig =>
                    {
                        endpointConfig.CreateIfMissing(topicOptions =>
                        {
                            topicOptions.NumPartitions = 2;
                            topicOptions.ReplicationFactor = 1;
                        });
                        endpointConfig.ConfigureConsumers(riderContext);
                    });
                });
            });
        });
    })
    .Build();

await host.RunAsync();
