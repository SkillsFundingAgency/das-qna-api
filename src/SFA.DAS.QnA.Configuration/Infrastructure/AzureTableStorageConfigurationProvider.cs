using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.QnA.Configuration.Infrastructure
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        private readonly string _connection;
        private readonly string _environment;
        private readonly string _version;
        private readonly string _appName;


        public AzureTableStorageConfigurationProvider(string connection,string appName, string environment, string version)
        {
            _connection = connection;
            _environment = environment;
            _version = version;
            _appName = appName;
        }

        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        private async Task LoadAsync()
        {
            if (_environment.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var tableClient = GetTableClient(); ;
            var entity = await GetEntityAsync(tableClient, _appName, _environment, _version);

            var jsonObject = JObject.Parse(entity.GetString("Data"));

            foreach (var child in jsonObject.Children())
            {
                foreach (var jToken in child.Children().Children())
                {
                    var child1 = (JProperty)jToken;
                    Data.Add($"{child.Path}:{child1.Name}", child1.Value.ToString());
                }
            }
        }

        private TableClient GetTableClient()
        {
            return new TableClient(_connection, "Configuration");
        }

        private async Task<TableEntity> GetEntityAsync(TableClient tableClient, string serviceName, string environmentName, string version)
        {
            var entity = await tableClient.GetEntityAsync<TableEntity>(environmentName, $"{serviceName}_{version}");
            return entity.Value;
        }
    }
}
