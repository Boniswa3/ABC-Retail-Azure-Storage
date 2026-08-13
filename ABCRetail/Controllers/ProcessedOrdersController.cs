using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class ProcessedOrdersController : Controller
    {

        
        private readonly ProcessedOrderTableService _processedOrderTableService;


        
        public ProcessedOrdersController(
            ProcessedOrderTableService processedOrderTableService)
        {
            _processedOrderTableService = processedOrderTableService;
        }

        
        public async Task<IActionResult> Index(
     string? customerId,
     string? productId,
     string? status)
        {
            var orders =
                await _processedOrderTableService
                    .GetProcessedOrdersAsync(
                        customerId,
                        productId,
                        status);

            ViewBag.CustomerId = customerId;
            ViewBag.ProductId = productId;
            ViewBag.Status = status;

            return View(orders);
        }

        public async Task<IActionResult> Details(
    string partitionKey,
    string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) ||
                string.IsNullOrWhiteSpace(rowKey))
            {
                return NotFound();
            }

            var order =
                await _processedOrderTableService
                    .GetProcessedOrderAsync(
                        partitionKey,
                        rowKey);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}