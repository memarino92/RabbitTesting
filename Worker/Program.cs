using MassTransit;
using Microsoft.Extensions.Hosting;
using Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StartWorkflowConsumer>();
    x.AddConsumer<Step1Consumer>();
    x.AddConsumer<Step2Consumer>();
    x.AddConsumer<Step3Consumer>();
    x.AddConsumer<Step4Consumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/");

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

await host.RunAsync();