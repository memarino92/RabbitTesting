using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Worker;
using Contracts;
using Saga;
using StackExchange.Redis;

// Create a unique identifier for this worker instance
var workerInstanceId = Guid.NewGuid().ToString("N").Substring(0, 8);

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

// Configure Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    ConnectionMultiplexer.Connect("localhost"));

// Register the result store
builder.Services.AddSingleton<IResultStore, RedisResultStore>();

builder.Services.AddMassTransit(x =>
{
    // Configure the Saga with a Redis repository for distributed state
    x.AddSagaStateMachine<WorkflowStateMachine, WorkflowState>()
        .RedisRepository(r =>
        {
            r.DatabaseConfiguration("localhost");
            r.KeyPrefix = "workflow";

            // Add optimistic concurrency to prevent conflicts
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
        })
        // The saga must use a single queue name across all instances
        .Endpoint(e =>
        {
            e.Name = "workflow-saga";
            // Only process one message at a time for the saga
            e.PrefetchCount = 1;
            e.ConcurrentMessageLimit = 1;
            // Don't use temporary queues
            e.Temporary = false;
        });

    // Configure activity consumers as competing consumers (multiple instances share same queue)
    // All instances will consume from the same queue, but each message will be processed only once

    // Add the StartWorkflowConsumer
    x.AddConsumer<StartWorkflowConsumer>()
        .Endpoint(e =>
        {
            e.Name = "start-workflow";
            e.PrefetchCount = 16;
            e.Temporary = false;  // Don't use temporary queues
        });

    // Add step consumers with proper concurrency
    x.AddConsumer<DirectStep1Consumer>()
        .Endpoint(e =>
        {
            e.Name = "step1-activity";
            e.PrefetchCount = 16;
            e.Temporary = false;
        });

    x.AddConsumer<DirectStep2Consumer>()
        .Endpoint(e =>
        {
            e.Name = "step2-activity";
            e.PrefetchCount = 16;
            e.Temporary = false;
        });

    x.AddConsumer<DirectStep3Consumer>()
        .Endpoint(e =>
        {
            e.Name = "step3-activity";
            e.PrefetchCount = 16;
            e.Temporary = false;
        });

    x.AddConsumer<DirectStep4Consumer>()
        .Endpoint(e =>
        {
            e.Name = "step4-activity";
            e.PrefetchCount = 16;
            e.Temporary = false;
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Enable publishing directly to queue
        cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;

        // Set retry policy for consumers
        cfg.UseMessageRetry(r => r.Immediate(3));

        // For multiple instances, use a unique discriminator for publisher endpoints
        // but shared consumer endpoints
        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(workerInstanceId, false));

        // Log message consumption
        cfg.UseConsumeFilter(typeof(LoggingConsumeFilter<>), context);
    });
});

var host = builder.Build();

// Get ILogger to log startup
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting worker with instance ID: {WorkerInstanceId}", workerInstanceId);
logger.LogInformation("Starting worker...");

await host.RunAsync();
