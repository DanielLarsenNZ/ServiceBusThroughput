using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

class Program
{
    static async Task Main(string[] args)
    {
        // connection string to your Service Bus namespace
        string connectionString = null;
        string topicName = "throughput2";
        int numOfMessages = 100;

        if (args.Length == 0) throw new ArgumentException("First argument must be Service Bus connection string");

        connectionString = args[0];
        if (args.Length > 1) topicName = args[1];
        if (args.Length > 2) int.TryParse(args[2], out numOfMessages);

        var client = new ServiceBusClient(connectionString);
        var sender = client.CreateSender(topicName);

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

            await sender.DisposeAsync();
            await client.DisposeAsync();
        }

        Console.WriteLine($"{count} messages sent in {stopwatch.ElapsedMilliseconds} ms equals throughput of {count / stopwatch.Elapsed.TotalSeconds} per second");

        Console.WriteLine("Press any key to end the application");
        Console.ReadKey();
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

    //static private async Task Batch(int numOfMessages)
    //{
    //    // create a batch 
    //    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

    //    for (int i = 1; i <= numOfMessages; i++)
    //    {
    //        // try adding a message to the batch
    //        if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    //        {
    //            // if it is too large for the batch
    //            throw new Exception($"The message {i} is too large to fit in the batch.");
    //        }
    //    }

    //    try
    //    {
    //        // Use the producer client to send the batch of messages to the Service Bus topic
    //        await sender.SendMessagesAsync(messageBatch);
    //        Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");
    //    }
    //    finally
    //    {
    //        // Calling DisposeAsync on client types is required to ensure that network
    //        // resources and other unmanaged objects are properly cleaned up.
    //        await sender.DisposeAsync();
    //        await client.DisposeAsync();
    //    }

    //}

}

