using MassTransit;
using Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
});

builder.Services.AddMassTransit(x =>
{
    // Client doesn't need to register the saga
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Enable publishing directly to queue
        cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.MaintainHierarchy;

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
await host.StartAsync();

try
{
    var publishEndpoint = host.Services.GetRequiredService<IPublishEndpoint>();
    var logger = host.Services.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Starting the NewTask application...");

    var workflowId = Guid.NewGuid();
    logger.LogInformation("Starting new workflow with WorkflowId: {WorkflowId}", workflowId);

    // Send the command to start the workflow
    await publishEndpoint.Publish(new StartWorkflowCommand(workflowId));

    logger.LogInformation("Workflow command published successfully");
    logger.LogInformation("Press any key to exit...");
    Console.ReadKey();
}
finally
{
    await host.StopAsync();
}