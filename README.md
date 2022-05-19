# Service Bus Throughput

Experiments in Azure Service Bus throughput.

## Getting started

```powershell
cd ServiceBusThroughput

dotnet run -c Release -- "(Service Bus connection string)" throughput2 100 10
```

Where:

* **1st argument**: Service Connection string (in quotes).
* **2nd argument**: Topic name.
* **3rd argument**: (Optional) Number of messages to send. Defaults to 100.
* **4th argument**: (Optional) Number of messages per batch. Defaults to 10.

The tool currently only supports Service Bus Topics. The tool will run two tests. The first test runs without batching (sending each message individually). The second test sends messages in batches. The tool will produce output like this:

```
1000 messages sent in 18190 ms equals throughput of 54.97281138202825 per second

1000 messages in batches of 10 sent in 1838 ms equals throughput of 543.9089244086799 per second
```
