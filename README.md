# RabbitTesting

A simple proof of concept for a message-driven workflow application using MassTransit and RabbitMQ.

## Overview

This project demonstrates a basic workflow orchestrated using a Saga pattern with MassTransit. The workflow consists of multiple steps, each executed as a separate activity.

## Getting Started

### Prerequisites

- .NET 10 SDK (Release Candidate)
- RabbitMQ

### Running the Application

1.  **Start RabbitMQ:** Ensure your RabbitMQ instance is running.
2.  **Run the Worker:**
    ```powershell
    dotnet run --project Worker/Worker.csproj
    ```
3.  **Run the NewTask:**
    ```powershell
    dotnet run --project NewTask/NewTask.csproj
    ```

This will start the workflow, and you will see logs in the console of the `Worker` application as it processes each step of the workflow.
