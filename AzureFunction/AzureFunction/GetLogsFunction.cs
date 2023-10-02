using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AzureFunction
{
    public static class GetLogsFunction
    {
        [FunctionName("GetLogs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=jv33;AccountKey=your_key;EndpointSuffix=core.windows.net";

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
