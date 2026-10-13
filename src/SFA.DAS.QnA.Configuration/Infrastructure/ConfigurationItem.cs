using System;
using Azure;
using Azure.Data.Tables;

namespace SFA.DAS.QnA.Configuration.Infrastructure
{
    public class ConfigurationItem : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Data { get; set; }
    }
}
