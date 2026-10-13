using System;
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


        public AzureTableStorageConfigurationProvider(string connection, string appName, string environment, string version)
        {
            _connection = connection;
            _environment = environment;
            _version = version;
            _appName = appName;
        }

        public override void Load()
        {
            if (_environment.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var configItem = GetOperation(_appName, _environment, _version);

            var jsonObject = JObject.Parse(configItem.Data);

            foreach (var child in jsonObject.Children())
            {
                foreach (var jToken in child.Children().Children())
                {
                    var child1 = (JProperty)jToken;
                    Data.Add($"{child.Path}:{child1.Name}", child1.Value.ToString());
                }
            }
        }

        private TableClient GetTable()
        {
            var tableClient = new TableServiceClient(_connection);
            return tableClient.GetTableClient("Configuration");
        }

        private ConfigurationItem GetOperation(
            string serviceName,
            string environmentName,
            string version)
        {
            return GetTable().GetEntity<ConfigurationItem>(
                environmentName,
                $"{serviceName}_{version}").Value;
        }
    }
}
