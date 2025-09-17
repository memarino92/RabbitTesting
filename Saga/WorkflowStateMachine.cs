using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Saga;

public class WorkflowStateMachine : MassTransitStateMachine<WorkflowState>
{
    private readonly ILogger<WorkflowStateMachine> _logger;

    public WorkflowStateMachine(ILogger<WorkflowStateMachine> logger)
    {
        _logger = logger;

        // Map the CurrentState property to track state
        InstanceState(x => x.CurrentState);

        // Configure event correlation based on WorkflowId
        Event(() => WorkflowStarted, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step1Completed, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step2Completed, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step3Completed, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step4Completed, x => x.CorrelateById(context => context.Message.WorkflowId));

        // Log all state transitions and event processing
        this.OnUnhandledEvent(x =>
        {
            _logger.LogWarning("Unhandled event {EventName} received in state {CurrentState} for workflow {WorkflowId}",
                x.Event.Name, x.Saga.CurrentState, x.Saga.WorkflowId);
            return Task.CompletedTask;
        });

        // Define the workflow sequence - happy path only
        Initially(
            When(WorkflowStarted)
                .Then(context =>
                {
                    // Initialize state
                    context.Saga.WorkflowId = context.Message.WorkflowId;
                    context.Saga.CorrelationId = context.Message.WorkflowId;
                    context.Saga.StartTime = DateTime.UtcNow;
                    context.Saga.Version++; // Increment version for optimistic concurrency

                    _logger.LogInformation("Starting workflow for WorkflowId: {WorkflowId}", context.Message.WorkflowId);
                })
                .ThenAsync(context => context.Publish(new Step1Command(context.Message.WorkflowId)))
                .TransitionTo(Step1Pending)
        );

        During(Step1Pending,
            When(Step1Completed)
                .Then(context =>
                {
                    context.Saga.Step1CompletionTime = DateTime.UtcNow;
                    context.Saga.Version++; // Increment version for optimistic concurrency

                    _logger.LogInformation("Step 1 completed for WorkflowId: {WorkflowId}, transitioning to Step2Pending", context.Message.WorkflowId);
                })
                .ThenAsync(context => context.Publish(new Step2Command(context.Message.WorkflowId)))
                .TransitionTo(Step2Pending),

            // Handle duplicate WorkflowStarted events gracefully
            When(WorkflowStarted)
                .Then(context => _logger.LogInformation("Ignoring duplicate WorkflowStartedEvent for WorkflowId: {WorkflowId} in Step1Pending state", context.Message.WorkflowId)),

            // Handle out-of-order completion events
            When(Step2Completed)
                .Then(context => _logger.LogInformation("Received Step2Completed out of order for WorkflowId: {WorkflowId} - ignoring while in Step1Pending", context.Message.WorkflowId)),
            When(Step3Completed)
                .Then(context => _logger.LogInformation("Received Step3Completed out of order for WorkflowId: {WorkflowId} - ignoring while in Step1Pending", context.Message.WorkflowId)),
            When(Step4Completed)
                .Then(context => _logger.LogInformation("Received Step4Completed out of order for WorkflowId: {WorkflowId} - ignoring while in Step1Pending", context.Message.WorkflowId))
        );

        During(Step2Pending,
            When(Step2Completed)
                .Then(context =>
                {
                    context.Saga.Step2CompletionTime = DateTime.UtcNow;
                    context.Saga.Version++; // Increment version for optimistic concurrency

                    _logger.LogInformation("Step 2 completed for WorkflowId: {WorkflowId}, transitioning to Step3Pending", context.Message.WorkflowId);
                })
                .ThenAsync(context => context.Publish(new Step3Command(context.Message.WorkflowId)))
                .TransitionTo(Step3Pending),

            // Handle duplicate or out-of-order events
            When(WorkflowStarted)
                .Then(context => _logger.LogInformation("Ignoring duplicate WorkflowStartedEvent for WorkflowId: {WorkflowId} in Step2Pending state", context.Message.WorkflowId)),
            When(Step1Completed)
                .Then(context => _logger.LogInformation("Ignoring duplicate Step1Completed for WorkflowId: {WorkflowId} in Step2Pending state", context.Message.WorkflowId)),
            When(Step3Completed)
                .Then(context => _logger.LogInformation("Received Step3Completed out of order for WorkflowId: {WorkflowId} - ignoring while in Step2Pending", context.Message.WorkflowId)),
            When(Step4Completed)
                .Then(context => _logger.LogInformation("Received Step4Completed out of order for WorkflowId: {WorkflowId} - ignoring while in Step2Pending", context.Message.WorkflowId))
        );

        During(Step3Pending,
            When(Step3Completed)
                .Then(context =>
                {
                    context.Saga.Step3CompletionTime = DateTime.UtcNow;
                    context.Saga.Version++; // Increment version for optimistic concurrency

                    _logger.LogInformation("Step 3 completed for WorkflowId: {WorkflowId}, transitioning to Step4Pending", context.Message.WorkflowId);
                })
                .ThenAsync(context => context.Publish(new Step4Command(context.Message.WorkflowId)))
                .TransitionTo(Step4Pending),

            // Handle duplicate or out-of-order events
            When(WorkflowStarted)
                .Then(context => _logger.LogInformation("Ignoring duplicate WorkflowStartedEvent for WorkflowId: {WorkflowId} in Step3Pending state", context.Message.WorkflowId)),
            When(Step1Completed)
                .Then(context => _logger.LogInformation("Ignoring duplicate Step1Completed for WorkflowId: {WorkflowId} in Step3Pending state", context.Message.WorkflowId)),
            When(Step2Completed)
                .Then(context => _logger.LogInformation("Ignoring duplicate Step2Completed for WorkflowId: {WorkflowId} in Step3Pending state", context.Message.WorkflowId)),
            When(Step4Completed)
                .Then(context => _logger.LogInformation("Received Step4Completed out of order for WorkflowId: {WorkflowId} - ignoring while in Step3Pending", context.Message.WorkflowId))
        );

        During(Step4Pending,
            When(Step4Completed)
                .Then(context =>
                {
                    context.Saga.Step4CompletionTime = DateTime.UtcNow;
                    context.Saga.CompletionTime = DateTime.UtcNow;
                    context.Saga.Version++; // Increment version for optimistic concurrency

                    _logger.LogInformation("Step 4 completed for WorkflowId: {WorkflowId}, workflow complete!", context.Message.WorkflowId);
                })
                .ThenAsync(context => context.Publish(new WorkflowCompletedEvent(context.Message.WorkflowId)))
                .Finalize(),

            // Handle duplicate events
            When(WorkflowStarted)
                .Then(context => _logger.LogInformation("Ignoring duplicate WorkflowStartedEvent for WorkflowId: {WorkflowId} in Step4Pending state", context.Message.WorkflowId)),
            When(Step1Completed)
                .Then(context => _logger.LogInformation("Ignoring duplicate Step1Completed for WorkflowId: {WorkflowId} in Step4Pending state", context.Message.WorkflowId)),
            When(Step2Completed)
                .Then(context => _logger.LogInformation("Ignoring duplicate Step2Completed for WorkflowId: {WorkflowId} in Step4Pending state", context.Message.WorkflowId)),
            When(Step3Completed)
                .Then(context => _logger.LogInformation("Ignoring duplicate Step3Completed for WorkflowId: {WorkflowId} in Step4Pending state", context.Message.WorkflowId))
        );

        // Mark the state machine as complete when finalized
        SetCompletedWhenFinalized();
    }

    // States
    public State Step1Pending { get; private set; } = default!;
    public State Step2Pending { get; private set; } = default!;
    public State Step3Pending { get; private set; } = default!;
    public State Step4Pending { get; private set; } = default!;

    // Events
    public Event<WorkflowStartedEvent> WorkflowStarted { get; private set; } = default!;
    public Event<Step1CompletedEvent> Step1Completed { get; private set; } = default!;
    public Event<Step2CompletedEvent> Step2Completed { get; private set; } = default!;
    public Event<Step3CompletedEvent> Step3Completed { get; private set; } = default!;
    public Event<Step4CompletedEvent> Step4Completed { get; private set; } = default!;
}
