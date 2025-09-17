using Contracts;
using MassTransit;

namespace Frontend.Services;

public interface IWorkflowService
{
    Task StartWorkflow();
}

public class WorkflowService : IWorkflowService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public WorkflowService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task StartWorkflow()
    {
        // Create a new workflow ID
        var workflowId = NewId.NextGuid();

        // Create and publish the StartWorkflowCommand
        var command = new StartWorkflowCommand(workflowId);
        await _publishEndpoint.Publish(command);

        return;
    }
}
