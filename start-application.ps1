### Start-Application.ps1
### This script starts the entire application with multiple worker instances

# First ensure services are running
. .\start-services.ps1

# Define log files
$worker1Log = "worker1.log"
$worker2Log = "worker2.log"
$newTaskLog = "newtask.log"

Write-Host "`nStarting application components..." -ForegroundColor Cyan

# Function to start a process in a new window
function Start-ProcessInNewWindow {
    param (
        [string]$workingDirectory,
        [string]$command,
        [string]$windowTitle
    )

    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$workingDirectory'; $command" -WindowStyle Normal
}

# Start Worker 1
Write-Host "Starting Worker instance 1..." -ForegroundColor Yellow
Start-ProcessInNewWindow -workingDirectory "C:\Users\Michael\projects\RabbitTesting\Worker" -command "dotnet run | Tee-Object -FilePath '..\..\$worker1Log'" -windowTitle "Worker 1"

# Start Worker 2
Write-Host "Starting Worker instance 2..." -ForegroundColor Yellow
Start-ProcessInNewWindow -workingDirectory "C:\Users\Michael\projects\RabbitTesting\Worker" -command "dotnet run | Tee-Object -FilePath '..\..\$worker2Log'" -windowTitle "Worker 2"

# Wait a moment for workers to initialize
Write-Host "Waiting for worker instances to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Start NewTask client
Write-Host "Starting NewTask client..." -ForegroundColor Yellow
Start-ProcessInNewWindow -workingDirectory "C:\Users\Michael\projects\RabbitTesting\NewTask" -command "dotnet run | Tee-Object -FilePath '..\..\$newTaskLog'" -windowTitle "NewTask Client"

Write-Host "`nApplication started successfully!" -ForegroundColor Green
Write-Host "- Worker instance 1 is running in a separate window"
Write-Host "- Worker instance 2 is running in a separate window"
Write-Host "- NewTask client is running in a separate window"
Write-Host "`nLogs are being saved to:"
Write-Host "- $worker1Log"
Write-Host "- $worker2Log"
Write-Host "- $newTaskLog"
