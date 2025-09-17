using System.Collections.Concurrent;

namespace Frontend.Services;

public class WorkflowTracker
{
    public ConcurrentDictionary<Guid, WorkflowStatus> WorkflowStatuses { get; } = new();

    public event Action<Guid, WorkflowStatus>? WorkflowStatusChanged;

    public void UpdateWorkflowStatus(Guid workflowId, Action<WorkflowStatus> updateAction)
    {
        if (WorkflowStatuses.TryGetValue(workflowId, out var status))
        {
            updateAction(status);
            WorkflowStatusChanged?.Invoke(workflowId, status);
        }
    }
}
