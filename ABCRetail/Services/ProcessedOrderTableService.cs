using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class ProcessedOrderTableService
    {
        private readonly TableClient _processedOrderTable;

        public ProcessedOrderTableService(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "AzureStorage connection string is not configured.");

            _processedOrderTable = new TableClient(
                connectionString,
                "ProcessedOrders");

            _processedOrderTable.CreateIfNotExists();
        }

        //method can now receive three optional filters
        public async Task<List<ProcessedOrder>> GetProcessedOrdersAsync(
     string? customerId = null,
     string? productId = null,
     string? status = null)
        {
            List<ProcessedOrder> orders = new();

            await foreach (
                TableEntity entity
                in _processedOrderTable.QueryAsync<TableEntity>())
            {
                decimal totalAmount = 0;

                if (entity.ContainsKey("TotalAmount"))
                {
                    object? amountValue = entity["TotalAmount"];

                    if (amountValue is double doubleValue)
                    {
                        totalAmount = Convert.ToDecimal(doubleValue);
                    }
                    else if (amountValue is decimal decimalValue)
                    {
                        totalAmount = decimalValue;
                    }
                    else if (amountValue != null)
                    {
                        decimal.TryParse(
                            amountValue.ToString(),
                            out totalAmount);
                    }
                }

                ProcessedOrder order = new ProcessedOrder
                {
                    PartitionKey = entity.PartitionKey,
                    RowKey = entity.RowKey,

                    OrderId =
                        entity.GetString("OrderId")
                        ?? string.Empty,

                    CustomerId =
                        entity.GetString("CustomerId")
                        ?? string.Empty,

                    ProductId =
                        entity.GetString("ProductId")
                        ?? string.Empty,

                    Quantity =
                        entity.GetInt32("Quantity")
                        ?? 0,

                    TotalAmount = totalAmount,

                    OrderDate =
                        entity.GetDateTime("OrderDate")
                        ?? DateTime.MinValue,

                    Status =
                        entity.GetString("Status")
                        ?? string.Empty,

                    Timestamp = entity.Timestamp,

                    ETag = entity.ETag
                };

                if (!string.IsNullOrWhiteSpace(customerId) &&
                    !order.CustomerId.Equals(
                        customerId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(productId) &&
                    !order.ProductId.Equals(
                        productId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(status) &&
                    !order.Status.Equals(
                        status,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                orders.Add(order);
            }

            return orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }


        public async Task<ProcessedOrder?> GetProcessedOrderAsync(
    string partitionKey,
    string rowKey)
        {
            try
            {
                TableEntity entity =
                    await _processedOrderTable.GetEntityAsync<TableEntity>(
                        partitionKey,
                        rowKey);

                decimal totalAmount = 0;

                if (entity.ContainsKey("TotalAmount"))
                {
                    object? amountValue = entity["TotalAmount"];

                    if (amountValue is double doubleValue)
                    {
                        totalAmount = Convert.ToDecimal(doubleValue);
                    }
                    else if (amountValue is decimal decimalValue)
                    {
                        totalAmount = decimalValue;
                    }
                    else if (amountValue != null)
                    {
                        decimal.TryParse(
                            amountValue.ToString(),
                            out totalAmount);
                    }
                }

                return new ProcessedOrder
                {
                    PartitionKey = entity.PartitionKey,
                    RowKey = entity.RowKey,

                    OrderId =
                        entity.GetString("OrderId")
                        ?? string.Empty,

                    CustomerId =
                        entity.GetString("CustomerId")
                        ?? string.Empty,

                    ProductId =
                        entity.GetString("ProductId")
                        ?? string.Empty,

                    Quantity =
                        entity.GetInt32("Quantity")
                        ?? 0,

                    TotalAmount = totalAmount,

                    OrderDate =
                        entity.GetDateTime("OrderDate")
                        ?? DateTime.MinValue,

                    Status =
                        entity.GetString("Status")
                        ?? string.Empty,

                    Timestamp = entity.Timestamp,

                    ETag = entity.ETag
                };
            }
            catch (Azure.RequestFailedException ex)
                when (ex.Status == 404)
            {
                return null;
            }
        }
    }
}
