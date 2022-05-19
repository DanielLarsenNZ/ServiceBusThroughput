using Azure.Messaging.ServiceBus;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        // connection string to your Service Bus namespace
        int numOfMessages = 100;
        int batchSize = 10;

        if (args.Length == 0) throw new ArgumentException("First argument must be Service Bus connection string.\nUsage: ./ServiceBusThroughput.exe (Service Bus connection string) (topic name)");
        if (args.Length == 1) throw new ArgumentException("Second argument must be topic name.\nUsage: ./ServiceBusThroughput.exe (Service Bus connection string) (topic name)");

        string connectionString = args[0];
        string topicName = args[1];
        if (args.Length > 2) int.TryParse(args[2], out numOfMessages);
        if (args.Length > 3) int.TryParse(args[3], out batchSize);

        var client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(topicName);

        await NoBatch(sender, numOfMessages);
        await Batch(sender, numOfMessages, batchSize);

        await sender.DisposeAsync();
        await client.DisposeAsync();
    }

    static private string DataBytes(int length)
    {
        // Construct a random string of data
        string data = "";
        var random = new Random();
        for (int i = 0; i < length; i++)
        {
            data += (char)random.Next(65, 90);
        }

        return data;
    }

    static async Task NoBatch(ServiceBusSender sender, int numOfMessages)
    {
        string data = DataBytes(4096);
        var stopwatch = new Stopwatch();
        int count = 0;

        try
        {
            stopwatch.Start();
            for (int i = 1; i <= numOfMessages; i++)
            {
                var message = new ServiceBusMessage(data);
                await sender.SendMessageAsync(message);
                count++;
                Console.Write($"{count} ");
            }
        }
        finally
        {
            Console.WriteLine();
            stopwatch.Stop();

        }

        Console.WriteLine($"{count} messages sent in {stopwatch.ElapsedMilliseconds} ms equals throughput of {count / stopwatch.Elapsed.TotalSeconds} per second");
    }

    static private async Task Batch(ServiceBusSender sender, int numOfMessages, int batchSize)
    {
        // create a batch 
        ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

        string data = DataBytes(4096);
        var stopwatch = new Stopwatch();
        int count = 0;
        int batchCount = 0;

        try
        {
            stopwatch.Start();

            for (int i = 0; i < numOfMessages; i++)
            {
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(data)))
                {
                    throw new Exception("Batch full.");
                }

                batchCount++;
                count++;

                if (batchCount >= batchSize)
                {
                    await sender.SendMessagesAsync(messageBatch);
                    messageBatch.Dispose();
                    messageBatch = await sender.CreateMessageBatchAsync();
                    batchCount = 0;
                    Console.Write($"{count} ");
                }
            }

            if (batchCount > 0)
            {
                await sender.SendMessagesAsync(messageBatch);
                messageBatch.Dispose();
            }
        }
        finally
        {
            Console.WriteLine();
            stopwatch.Stop();
        }

        Console.WriteLine($"{count} messages in batches of {batchSize} sent in {stopwatch.ElapsedMilliseconds} ms equals throughput of {count / stopwatch.Elapsed.TotalSeconds} per second");
    }
}

