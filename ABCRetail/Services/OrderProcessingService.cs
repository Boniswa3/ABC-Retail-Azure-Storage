using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class OrderProcessingService
    {
        public async Task ProcessOrderAsync(Order order)
        {
            // Simulate order processing
            await Task.Delay(1000);

            Console.WriteLine(
                $"Order {order.OrderId} processed successfully.");

            Console.WriteLine(
                $"Customer: {order.CustomerId}");

            Console.WriteLine(
                $"Product: {order.ProductId}");

            Console.WriteLine(
                $"Quantity: {order.Quantity}");

            Console.WriteLine(
                $"Total: R{order.TotalAmount:N2}");
        }
    }
}
