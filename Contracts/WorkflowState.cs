using MassTransit;

namespace Contracts;

public class WorkflowState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;

    // The WorkflowId to correlate events
    public Guid WorkflowId { get; set; }

    // Timestamps for tracking progress and duration
    public DateTime? StartTime { get; set; }
    public DateTime? Step1CompletionTime { get; set; }
    public DateTime? Step2CompletionTime { get; set; }
    public DateTime? Step3CompletionTime { get; set; }
    public DateTime? Step4CompletionTime { get; set; }
    public DateTime? CompletionTime { get; set; }
}
