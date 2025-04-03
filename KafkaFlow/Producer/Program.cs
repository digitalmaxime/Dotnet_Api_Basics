using Confluent.SchemaRegistry;
using KafkaFlow;
using Microsoft.Extensions.DependencyInjection;
using KafkaFlow.Producers;
using Producer;
// using KafkaFlow.SchemaRegistry;
// using KafkaFlow.SchemaRegistry.Json;
using System.Text.Json;
using Confluent.SchemaRegistry.Serdes;

var services = new ServiceCollection();

const string topicName = "sample-topic";
const string producerName = "say-hello";

// var config = new SchemaRegistryConfig()
// {
//     Url = "http://localhost:8081",
// };
//
// var schemaRegistryClient = new CachedSchemaRegistryClient(config);
//
// services.AddSingleton<ISchemaRegistryClient>(schemaRegistryClient);

services.AddSingleton(new JsonSerializerOptions
{
    WriteIndented = true
});

services.AddKafka(
    kafka => kafka
        .UseConsoleLog()
        .AddCluster(
            cluster => cluster
                .WithBrokers(new[] { "localhost:9092" })
                .WithSchemaRegistry(config => config.Url = "localhost:8081")
                .CreateTopicIfNotExists(topicName, 1, 1)
                .AddProducer(
                    producerName,
                    producer => producer
                        .DefaultTopic(topicName)
                        .AddMiddlewares(m =>
                        {
                            m.AddSchemaRegistryJsonSerializer<HelloMessage>(
                                new JsonSerializerConfig
                                {
                                    SubjectNameStrategy = SubjectNameStrategy.TopicRecord,
                                    
                                });
                        })
                )
        )
);

var serviceProvider = services.BuildServiceProvider();

var producer = serviceProvider
    .GetRequiredService<IProducerAccessor>()
    .GetProducer(producerName);

await producer.ProduceAsync(
    topicName,
    Guid.NewGuid().ToString(),
    new HelloMessage { Text = "Hello from Json Schema!" });


Console.WriteLine("Message sent!");