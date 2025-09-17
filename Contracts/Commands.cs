using MassTransit;

namespace Contracts;

// Add CorrelatedBy<Guid> interface to all commands for proper correlation
public record StartWorkflowCommand(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step1Command(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step2Command(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step3Command(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}

public record Step4Command(Guid WorkflowId) : CorrelatedBy<Guid>
{
    public Guid CorrelationId => WorkflowId;
}
