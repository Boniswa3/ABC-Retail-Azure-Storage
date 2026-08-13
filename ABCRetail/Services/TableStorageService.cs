using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class TableStorageService
    {
        // Define TableClient instances for Customers and Products
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;

       public TableStorageService(IConfiguration configuration)
{
            // Retrieve the connection string from configuration
            string connectionString = configuration.GetConnectionString("AzureStorage")
                
        ?? throw new InvalidOperationException("Azure Storage connection string not found.");

            // Initialize TableClient instances for Customers and Products
            _customerTable = new TableClient(connectionString, "Customers");
    _productTable = new TableClient(connectionString, "Products");

            // Create the tables if they do not exist
            _customerTable.CreateIfNotExists();
    _productTable.CreateIfNotExists();
}

        // Customer methods
        // Add a new customer
        public async Task AddCustomerAsync(Customer customer)
        {
            customer.RowKey = Guid.NewGuid().ToString();
            // Ensure the RowKey is unique for each customer
            await _customerTable.AddEntityAsync(customer);
        }

        // Retrieve all customers
        public List<Customer> GetCustomers()
        {
            return _customerTable.Query<Customer>().ToList();
        }

        // Product methods

        // Add a new product
        public async Task AddProductAsync(Product product)
        {
            // Ensure the RowKey is unique for each product
            product.RowKey = Guid.NewGuid().ToString();

            await _productTable.AddEntityAsync(product);
        }

        // Retrieve all products
        public List<Product> GetProducts()
        {

            return _productTable.Query<Product>().ToList();
        }
    }
}