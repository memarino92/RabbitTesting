using MassTransit;
using Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Starting the NewTask application...");

var builder = Host.CreateApplicationBuilder(args);

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

    var workflowId = Guid.NewGuid();
    Console.WriteLine($"Starting new workflow with WorkflowId: {workflowId}");

    // Send the command to start the workflow
    await publishEndpoint.Publish(new StartWorkflowCommand(workflowId));

    Console.WriteLine("Workflow command published successfully");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
finally
{
    await host.StopAsync();
}