using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrderController : Controller
    {
        private readonly QueueStorageService _queueStorageService;

        public OrderController(QueueStorageService queueStorageService)
        {
            // Initialize the QueueStorageService
            _queueStorageService = queueStorageService;
        }

        // Display the order form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Send the order to Azure Queue Storage
        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // Assign a unique OrderId and set the order date and status
            order.OrderId = Guid.NewGuid().ToString();
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

            // Send the order to Azure Queue Storage for processing
            await _queueStorageService.SendOrderAsync(order);

            // Provide feedback to the user
            TempData["Success"] = "Order submitted successfully and added to the processing queue.";

            return RedirectToAction(nameof(Create));
        }
    }
}