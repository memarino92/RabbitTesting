using Contracts;
using MassTransit;
using System.Collections.Concurrent;

namespace Frontend.Services;

public interface IWorkflowService
{
    Task StartWorkflow();
    Task<Guid> StartWorkflowAndGetId();
    ConcurrentDictionary<Guid, WorkflowStatus> WorkflowStatuses { get; }
    event Action<Guid, WorkflowStatus>? WorkflowStatusChanged;
    void UpdateWorkflowStatus(Guid workflowId, Action<WorkflowStatus> updateAction);
}

public class WorkflowStatus
{
    public bool IsStarted { get; set; }
    public bool IsStep1Completed { get; set; }
    public bool IsStep2Completed { get; set; }
    public bool IsStep3Completed { get; set; }
    public bool IsStep4Completed { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
}

public class WorkflowService : IWorkflowService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly WorkflowTracker _workflowTracker;

    public WorkflowService(IPublishEndpoint publishEndpoint, WorkflowTracker workflowTracker)
    {
        _publishEndpoint = publishEndpoint;
        _workflowTracker = workflowTracker;
        
        // Subscribe to the tracker's status change events
        _workflowTracker.WorkflowStatusChanged += (id, status) => WorkflowStatusChanged?.Invoke(id, status);
    }

    public ConcurrentDictionary<Guid, WorkflowStatus> WorkflowStatuses => _workflowTracker.WorkflowStatuses;
    
    public event Action<Guid, WorkflowStatus>? WorkflowStatusChanged;

    public async Task StartWorkflow()
    {
        await StartWorkflowAndGetId();
    }

    public async Task<Guid> StartWorkflowAndGetId()
    {
        // Create a new workflow ID
        var workflowId = NewId.NextGuid();

        // Initialize status tracking for this workflow
        var status = new WorkflowStatus 
        { 
            StartTime = DateTime.UtcNow
        };
        _workflowTracker.WorkflowStatuses[workflowId] = status;

        // Create and publish the StartWorkflowCommand
        var command = new StartWorkflowCommand(workflowId);
        await _publishEndpoint.Publish(command);

        return workflowId;
    }

    public void UpdateWorkflowStatus(Guid workflowId, Action<WorkflowStatus> updateAction)
    {
        _workflowTracker.UpdateWorkflowStatus(workflowId, updateAction);
    }
}
