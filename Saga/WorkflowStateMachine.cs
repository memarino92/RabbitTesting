using Contracts;
using MassTransit;

namespace Saga;

public class WorkflowStateMachine : MassTransitStateMachine<WorkflowState>
{
    public WorkflowStateMachine()
    {
        // Map the CurrentState property to track state
        InstanceState(x => x.CurrentState);

        // Configure event correlation based on WorkflowId
        Event(() => WorkflowStarted, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step1Completed, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step2Completed, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step3Completed, x => x.CorrelateById(context => context.Message.WorkflowId));
        Event(() => Step4Completed, x => x.CorrelateById(context => context.Message.WorkflowId));

        // Define the workflow sequence - happy path only
        Initially(
            When(WorkflowStarted)
                .Then(context =>
                {
                    // Initialize state
                    context.Saga.WorkflowId = context.Message.WorkflowId;
                    context.Saga.CorrelationId = context.Message.WorkflowId;
                    context.Saga.StartTime = DateTime.UtcNow;
                })
                .ThenAsync(context => context.Publish(new Step1Command(context.Message.WorkflowId)))
                .TransitionTo(Step1Pending)
        );

        During(Step1Pending,
            When(Step1Completed)
                .Then(context => context.Saga.Step1CompletionTime = DateTime.UtcNow)
                .ThenAsync(context => context.Publish(new Step2Command(context.Message.WorkflowId)))
                .TransitionTo(Step2Pending),

            // Handle duplicate WorkflowStarted events gracefully
            When(WorkflowStarted)
                .Then(context => Console.WriteLine($"Ignoring duplicate WorkflowStartedEvent for WorkflowId: {context.Message.WorkflowId} in Step1Pending state"))
        );

        During(Step2Pending,
            When(Step2Completed)
                .Then(context => context.Saga.Step2CompletionTime = DateTime.UtcNow)
                .ThenAsync(context => context.Publish(new Step3Command(context.Message.WorkflowId)))
                .TransitionTo(Step3Pending)
        );

        During(Step3Pending,
            When(Step3Completed)
                .Then(context => context.Saga.Step3CompletionTime = DateTime.UtcNow)
                .ThenAsync(context => context.Publish(new Step4Command(context.Message.WorkflowId)))
                .TransitionTo(Step4Pending)
        );

        During(Step4Pending,
            When(Step4Completed)
                .Then(context =>
                {
                    context.Saga.Step4CompletionTime = DateTime.UtcNow;
                    context.Saga.CompletionTime = DateTime.UtcNow;
                })
                .ThenAsync(context => context.Publish(new WorkflowCompletedEvent(context.Message.WorkflowId)))
                .Finalize()
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
