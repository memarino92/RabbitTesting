using MassTransit;
using Contracts;

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
});

await busControl.StartAsync();

try
{
    var workflowId = Guid.NewGuid();
    Console.WriteLine($"Starting new workflow with WorkflowId: {workflowId}");

    await busControl.Publish(new StartWorkflowCommand(workflowId));

    Console.WriteLine("Workflow command published successfully");

    // Wait a moment for the message to be sent
    await Task.Delay(2000);
}
finally
{
    await busControl.StopAsync();
}