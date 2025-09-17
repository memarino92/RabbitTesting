using MassTransit;

namespace Contracts;

// Add CorrelatedBy<Guid> interface to all events for proper correlation
public record WorkflowStartedEvent(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step1CompletedEvent(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step2CompletedEvent(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step3CompletedEvent(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step4CompletedEvent(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record WorkflowCompletedEvent(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}
