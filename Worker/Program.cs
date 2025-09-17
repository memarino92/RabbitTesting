using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Worker;
using Contracts;
using Saga;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // Add the state machine saga with explicit endpoint naming
    x.AddSagaStateMachine<WorkflowStateMachine, WorkflowState>()
        .InMemoryRepository()
        .Endpoint(e => e.Name = "workflow-saga");

    // Add the StartWorkflowConsumer with explicit endpoint naming
    x.AddConsumer<StartWorkflowConsumer>()
        .Endpoint(e => e.Name = "start-workflow");

    // Add ONLY the direct step consumers - remove all others
    x.AddConsumer<DirectStep1Consumer>()
        .Endpoint(e => e.Name = "step1-activity");

    x.AddConsumer<DirectStep2Consumer>()
        .Endpoint(e => e.Name = "step2-activity");

    x.AddConsumer<DirectStep3Consumer>()
        .Endpoint(e => e.Name = "step3-activity");

    x.AddConsumer<DirectStep4Consumer>()
        .Endpoint(e => e.Name = "step4-activity");

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/");

        // Enable publishing directly to queue
        cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;

        // Configure the endpoints
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

Console.WriteLine("Starting worker...");
await host.RunAsync();
