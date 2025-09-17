# RabbitTesting

A simple proof of concept for a message-driven workflow application using MassTransit and RabbitMQ.

## Overview

This project demonstrates a basic workflow orchestrated using a Saga pattern with MassTransit. The workflow consists of multiple steps, each executed as a separate activity.

## Getting Started

### Prerequisites

- .NET 10 SDK (Release Candidate)
- Docker (for running RabbitMQ and Redis)

### Running the Application

#### Option 1: Using the Startup Scripts

1. **Start all services and application components:**

   ```powershell
   ./start-application.ps1
   ```

   This script will:

   - Ensure RabbitMQ and Redis are running in Docker
   - Start two Worker instances in separate windows
   - Start the NewTask client in a separate window
   - Log output to separate files

2. **Start just the required services:**
   ```powershell
   ./start-services.ps1
   ```
   This script will only ensure RabbitMQ and Redis are running, allowing you to manually start the application components.

#### Option 2: Manual Startup

1. **Start RabbitMQ and Redis:**

   ```powershell
   # Start Redis
   docker run --name rabbit-redis -p 6379:6379 -d redis

   # Start RabbitMQ
   docker run --name rabbit-mq -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management
   ```

2. **Run the Worker (run in multiple terminals for multiple instances):**

   ```powershell
   dotnet run --project Worker/Worker.csproj
   ```

3. **Run the NewTask:**
   ```powershell
   dotnet run --project NewTask/NewTask.csproj
   ```

## Project Structure

- **Contracts:** Contains message contracts (commands and events) and the workflow state
- **Saga:** Contains the state machine that orchestrates the workflow
- **Worker:** Hosts the saga and the activity consumers
- **NewTask:** Client application that initiates the workflow

## Troubleshooting

If you encounter any issues with events being processed out of order or duplicated:

1. Make sure Redis is running for saga state persistence
2. Ensure only one saga instance is defined in your application
3. Check that each worker has unique consumer IDs when running multiple instances
