namespace Contracts;

public record StartWorkflowCommand(Guid WorkflowId);
public record Step1Command(Guid WorkflowId);
public record Step2Command(Guid WorkflowId);
public record Step3Command(Guid WorkflowId);
public record Step4Command(Guid WorkflowId);
