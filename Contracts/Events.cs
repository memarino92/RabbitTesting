namespace Contracts;

public record WorkflowStartedEvent(Guid WorkflowId);
public record Step1CompletedEvent(Guid WorkflowId);
public record Step2CompletedEvent(Guid WorkflowId);
public record Step3CompletedEvent(Guid WorkflowId);
public record Step4CompletedEvent(Guid WorkflowId);
public record WorkflowCompletedEvent(Guid WorkflowId);
