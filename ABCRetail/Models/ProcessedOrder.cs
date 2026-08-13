using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{

    //Azure Tables SDK that this class can be used to read Azure Table entities
    public class ProcessedOrder : ITableEntity
    {
        public string PartitionKey { get; set; } = "Orders";

        public string RowKey { get; set; } = string.Empty;

        public string OrderId { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public string ProductId { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

       
        public ETag ETag { get; set; }
    }
}