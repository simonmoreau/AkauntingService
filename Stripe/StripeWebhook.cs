using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Table;
using Stripe;

namespace AkauntingService
{
    public static class StripeWebhook
    {
        [FunctionName("StripeWebhook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("stripe-webhooks")] CloudTable webhookCloudTable,
            [Table("stripe-webhooks")] IAsyncCollector<PaymentIntent> webhookTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Event @event = JsonSerializer.Deserialize<Event>(requestBody);

                if (@event.Type == "payment_intent.succeeded")
                {
                    EventData data = @event.Data;
                    PaymentIntent paymentIntent = data.Object as PaymentIntent;
                    // paymentIntent.Charges.Data[0].BillingDetails.Address

        //                             TableQuery<PaymentIntent> rangeQuery = new TableQuery<PaymentIntent>().Where(
        // TableQuery.CombineFilters(
        //     TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, body.PartitionKey),
        //     TableOperators.And,
        //     TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, body.RowKey)));

        //         TableQuerySegment<PaymentIntent> querySegment = await webhookCloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null);

        //         if (querySegment.Results.Count == 0)
        //         {
        //             await webhookTable.AddAsync(body);
        //         }
                }

                string responseMessage = "Ok";

            return new OkObjectResult(responseMessage);
        }
    }
}
