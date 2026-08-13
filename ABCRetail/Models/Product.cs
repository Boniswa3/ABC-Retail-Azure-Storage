using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Product : ITableEntity
    {
        // Required by Azure Table Storage
        public string PartitionKey { get; set; } = "Products";

        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        // Product Information
        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Range(0, 100000)]
        public int StockQuantity { get; set; }

        // Stores the Blob Storage URL
        public string ImageUrl { get; set; } = string.Empty;

        // Required by Azure Table Storage
        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}
