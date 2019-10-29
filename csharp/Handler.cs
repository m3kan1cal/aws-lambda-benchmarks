using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using System.Net;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace AwsDotnetCsharp
{
    public class Handler
    {
        private const bool desc = true;

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> Hello(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var clientConfig = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USWest2
            };

            var client = new AmazonDynamoDBClient(clientConfig);
            var query = new QueryRequest
            {
                TableName = System.Environment.GetEnvironmentVariable("DYNAMODB_TABLE"),
                IndexName = System.Environment.GetEnvironmentVariable("DYNAMODB_GSI_USERID_NOTEID"),
                KeyConditionExpression = "userId = :userId",
                ProjectionExpression="userId, noteId, notebook, #note_text",
                ExpressionAttributeNames = new Dictionary<string, string> {
                    {"#note_text", "text"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":userId", new AttributeValue { S =  "arbiter" }}
                },
                ScanIndexForward = desc
            };

            var cleaned = new Dictionary<string, AttributeValue>();

            QueryResponse results = null;
            results = await client.QueryAsync(query);
            foreach (var item in results.Items)
                item.ToList().ForEach(x => cleaned[x.Key] = x.Value);

            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            return await Task.Run(() => new APIGatewayProxyResponse()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = headers,
                Body = JsonConvert.SerializeObject(new { results = cleaned })
            });
        }
    }
}
