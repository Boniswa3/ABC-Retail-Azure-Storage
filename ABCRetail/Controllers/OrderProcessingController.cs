using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrderProcessingController : Controller
    {
        // Initialize services
        private readonly QueueStorageService _queueStorageService;
        private readonly OrderProcessingService _orderProcessingService;

        // Constructor to inject services
        public OrderProcessingController(
            QueueStorageService queueStorageService,
            OrderProcessingService orderProcessingService)
        {
            _queueStorageService = queueStorageService;
            _orderProcessingService = orderProcessingService;
        }

        // Process the next order in the queue
        [HttpGet]
        public async Task<IActionResult> Process()
        {
            var order =
                await _queueStorageService.ReceiveOrderAsync();

            if (order == null)
            {
                return Content("No orders are currently waiting in the queue.");
            }

            // Process the order using the OrderProcessingService
            await _orderProcessingService.ProcessOrderAsync(order);

            // Provide feedback to the user
            return Content(
                $"Order {order.OrderId} was processed successfully.");
        }
    }
}