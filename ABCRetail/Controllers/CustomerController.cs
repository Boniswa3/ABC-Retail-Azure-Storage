using Microsoft.AspNetCore.Mvc;
using ABCRetail.Models;
using ABCRetail.Services;

namespace ABCRetail.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableStorageService;

        public CustomerController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        // Display all customers
        public IActionResult Index()
        {
            var customers = _tableStorageService.GetCustomers();
            return View(customers);
        }

        // Display Create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save customer
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _tableStorageService.AddCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }
    }
}
