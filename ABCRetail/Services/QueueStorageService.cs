using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;

        public QueueStorageService(IConfiguration configuration)
        {
            // Retrieve the connection string from configuration
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string not found.");

            // Initialize the QueueClient for the "orders" queue
            _queueClient = new QueueClient(
                connectionString,
                "orders");

            _queueClient.CreateIfNotExists();
        }

        // Send an order to the Azure Queue Storage
        public async Task SendOrderAsync(Order order)
        {
            string message = JsonSerializer.Serialize(order);

            string encodedMessage =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(message));

            await _queueClient.SendMessageAsync(encodedMessage);
        }

        // Receive an order from the Azure Queue Storage
        public async Task<Order?> ReceiveOrderAsync()
        {
            // Receive a message from the queue
            QueueMessage? message =
                (await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 1)).Value.FirstOrDefault();

            // If no message is available, return null
            if (message == null)
            {
                return null;
            }

            byte[] bytes =
                Convert.FromBase64String(message.MessageText);

            string json =
                Encoding.UTF8.GetString(bytes);

            Order? order =
                JsonSerializer.Deserialize<Order>(json);

            // If the order was successfully deserialized, delete the message from the queue
            if (order != null)
            {
                await _queueClient.DeleteMessageAsync(
                    message.MessageId,
                    message.PopReceipt);
            }

            return order;
        }
    }
}