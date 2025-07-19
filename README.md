# IsolationLab

IsolationLab is a C# console application that demonstrates SQL Server isolation levels using file-based transaction simulation. It allows users to experiment with Read Uncommitted and Read Committed isolation levels, simulating concurrent access and transaction control.

## Features

- Simulates SQL Server isolation levels:  
  - Read Uncommitted (NOLOCK)  
  - Read Committed (default)
- File-based transaction log and data storage
- Supports basic transaction commands: `SELECT`, `COMMIT`, `ROLLBACK`
- Demonstrates locking and rollback behavior

## Project Structure

- `Program.cs` – Entry point, session selection, and orchestration
- `ReadUncommittedSession.cs` – Logic for Read Uncommitted isolation
- `ReadCommittedSession.cs` – Logic for Read Committed isolation, including file-based locking
- `MessageReader.cs` – Reads data and log files
- `MessageWriter.cs` – Writes to data and log files, handles commit and rollback
- `IsolationLevel.cs` – Enum for supported isolation levels
- `data/data.txt` – Simulated database table
- `data/log.txt` – Transaction log

## How to Run

1. **Build the project**  
   Open a terminal in the `IsolationLab` directory and run the following commands:
 
  ```sh
dotnet build
dotnet run --project IsolationLab
```