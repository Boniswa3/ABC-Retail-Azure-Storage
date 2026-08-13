using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail.Models
{
    public class Customer : ITableEntity
    {
        // Required by Azure Table Storage
        public string PartitionKey { get; set; } = "Customers";

        // Unique identifier
        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        // Customer Information
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        // Required by Azure
        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}
