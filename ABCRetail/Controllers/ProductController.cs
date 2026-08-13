using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class ProductController : Controller
    {
        
        private readonly TableStorageService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;

        public ProductController(
            TableStorageService tableStorageService,
            BlobStorageService blobStorageService)
        {
            // Initialize services
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
        }

        // Display all products
        public IActionResult Index()
        {
            // Retrieve products from Table Storage
            var products = _tableStorageService.GetProducts();
            return View(products);
        }

        // Display the Create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save the product
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Upload image if one was selected
            if (imageFile != null && imageFile.Length > 0)
            {
                // Upload the image to Blob Storage and get the URL
                product.ImageUrl = await _blobStorageService.UploadImageAsync(imageFile);
            }
            // Add the product to Table Storage
            await _tableStorageService.AddProductAsync(product);

            return RedirectToAction(nameof(Index));
        }
    }
}
