using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage;
using System.IO;
using System.Text;

namespace AzureFunction
{
    public class AzureFunction
    {
        [FunctionName("FetchAndStoreDataFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string accountName = Environment.GetEnvironmentVariable("AccountName");
            string accountKey = Environment.GetEnvironmentVariable("AccountKey");
            string tableAccountUrl = Environment.GetEnvironmentVariable("TableAccountUrl");
            string blobAccountUrl = Environment.GetEnvironmentVariable("BlobAccountUrl");
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(accountKey) || string.IsNullOrWhiteSpace(connectionString))
            {
                log.LogError("Invalid storage account information provided.");
                return;
            }


            var apiData = await FetchDataFromAPI();

            if (apiData != null)
            {
                await LogDataToTableStorage(tableAccountUrl, accountName, accountKey, apiData, log);
                await StoreDataToBlobStorage(blobAccountUrl, accountName, accountKey, apiData, log);
            }
            else
            {
                log.LogError("Could not fetch data from API.");
            }
        }

        private static async Task<string> FetchDataFromAPI()
        {
            using HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://api.publicapis.org/random?auth=null");
            return response;
        }

        private static async Task LogDataToTableStorage(string tableAccountUrl, string accountName, string accountKey, string apiData, ILogger log)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=jv33;AccountKey=zurohj+fNQRNwEx1IjbmTSEBkM3V1F6gS9pjxj3IppVPdDUPxuopbI4IXJeF8RSGS1bmK1ALmixA+AStWU80+g==;EndpointSuffix=core.windows.net";


            var serviceClient = new TableServiceClient(connectionString);


            var client = serviceClient.GetTableClient("AteaHomework01");

            await client.CreateIfNotExistsAsync();

            var entity = new TableEntity("somePartition", DateTime.UtcNow.Ticks.ToString())
            {
                { "Data", apiData }
            };

            await client.UpsertEntityAsync(entity, TableUpdateMode.Replace);

            log.LogInformation($"Logged data: {apiData}");
        }

        private static async Task StoreDataToBlobStorage(string blobAccountUrl, string accountName, string accountKey, string apiData, ILogger log)
        {
            var blobServiceClient = new BlobServiceClient(new Uri(blobAccountUrl), new StorageSharedKeyCredential(accountName, accountKey));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("myblobcontainer");

            await containerClient.CreateIfNotExistsAsync();

            string blobName = $"myblob-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";


            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(apiData)))
            {
                await blobClient.UploadAsync(stream, true);
            }

            log.LogInformation($"Stored data to blob: {blobName}");
        }
    }
}