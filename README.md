# Service Bus Throughput

Experiments in Azure Service Bus throughput.

## Getting started

```powershell
dotnet run -c Release -- "(Service Bus connection string)" throughput2 100 10
```

Where:

* **1st argument**: Service Connection string (in quotes).
* **2nd argument**: Topic name.
* **3rd argument**: (Optional) Number of messages to send. Defaults to 100.
* **4th argument**: (Optional) Number of messages per batch. Defaults to 10.

The tool currently only supports Service Bus Topics. The tool will run two tests. The first test runs without batching (sending each message individually). The second test sends messages in batches. The tool will produce output like this:

```
100 messages sent in 23273 ms equals throughput of 4.2966819346394765 per second

100 messages in batches of 10 sent in 4002 ms equals throughput of 24.982216409249073 per second
```
