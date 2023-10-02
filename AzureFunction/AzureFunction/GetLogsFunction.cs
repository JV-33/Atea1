using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction
{
    public static class GetLogsFunction
    {
        [FunctionName("GetLogs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string connectionString = Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                log.LogError("Invalid connection string provided.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            string fromTime = req.Query["fromTime"];
            string toTime = req.Query["toTime"];

            TableServiceClient serviceClient = new TableServiceClient(connectionString);
            TableClient tableClient = serviceClient.GetTableClient("AteaHomework01");

            string filter = TableClient.CreateQueryFilter<YourEntity>(e => e.Timestamp >= DateTimeOffset.Parse(fromTime) && e.Timestamp <= DateTimeOffset.Parse(toTime));
            List<YourEntity> logs = tableClient.Query<YourEntity>(filter).ToList();

            return new OkObjectResult(logs);
        }
    }
}
