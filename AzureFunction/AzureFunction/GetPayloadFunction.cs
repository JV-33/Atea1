using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureFunction
{
    public static class GetPayloadFunction
    {
        [FunctionName("GetPayload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string accountName = "jv33";
            string accountKey = "zurohj+fNQRNwEx1IjbmTSEBkM3V1F6gS9pjxj3IppVPdDUPxuopbI4IXJeF8RSGS1bmK1ALmixA+AStWU80+g==";
            string blobAccountUrl = $"https://{accountName}.blob.core.windows.net";
            string blobName = req.Query["blobName"];
            string containerName = "myblobcontainer"; 

            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(accountKey) || string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
            {
                log.LogError("Invalid storage account information provided.");
                return new BadRequestObjectResult("Invalid storage account information provided.");
            }

            var blobServiceClient = new BlobServiceClient(new Uri(blobAccountUrl), new StorageSharedKeyCredential(accountName, accountKey));
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            Response<BlobDownloadInfo> downloadResult;
            try
            {
                downloadResult = await blobClient.DownloadAsync();
            }
            catch (Exception e)
            {
                log.LogError($"Error downloading blob: {e.Message}");
                return new NotFoundObjectResult($"Error downloading blob: {e.Message}");
            }

            using var streamReader = new StreamReader(downloadResult.Value.Content);
            string payload = await streamReader.ReadToEndAsync();

            return new OkObjectResult(payload); // Atgriežam patieso payload, nevis vienkārši izvadu
        }
    }
}