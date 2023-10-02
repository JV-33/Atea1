using Azure;
using Azure.Data.Tables;
using System;

namespace AzureFunction
{
    public class YourEntity : ITableEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
