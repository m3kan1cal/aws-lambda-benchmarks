namespace AwsDotnetFsharp

open System.Collections.Generic
open System.Net
open Amazon.Lambda.Core
open Amazon.Lambda.APIGatewayEvents
open Amazon.DynamoDBv2
open Amazon.DynamoDBv2.Model
open Newtonsoft.Json

[<assembly:LambdaSerializer(typeof<Amazon.Lambda.Serialization.Json.JsonSerializer>)>]
do ()

module Handler =
    open System
    open System.IO
    open System.Text

    let hello(request : APIGatewayProxyRequest, context : ILambdaContext): APIGatewayProxyResponse =

        let clientConfig = new AmazonDynamoDBConfig()
        clientConfig.RegionEndpoint <- Amazon.RegionEndpoint.USWest2

        let client = new AmazonDynamoDBClient(clientConfig)
        let query = new QueryRequest()
        query.TableName <- System.Environment.GetEnvironmentVariable("DYNAMODB_TABLE")
        query.IndexName <- System.Environment.GetEnvironmentVariable("DYNAMODB_GSI_USERID_NOTEID")
        query.KeyConditionExpression <- "userId = :userId"
        query.ProjectionExpression <- "userId, noteId, notebook, #note_text"
        query.ScanIndexForward <- true

        let names = Dictionary<string, string>(dict [ ("#note_text", "text") ])
        query.ExpressionAttributeNames <- names

        let values = Dictionary<string, AttributeValue>(dict [ (":userId", new AttributeValue(S =  "arbiter")) ])
        query.ExpressionAttributeValues <- values

        let cleaned = new Dictionary<string, AttributeValue>()
        let mutable results = new QueryResponse()
        results <- client.QueryAsync(query)
        |> Async.AwaitTask
        |> Async.RunSynchronously

        for item in results.Items do
            item.Keys |> Seq.iter (fun k -> cleaned.[k] <- item.[k])

        let headers = new Dictionary<string, string>()
        headers.Add("Content-Type", "application/json")

        let response = new APIGatewayProxyResponse()
        response.StatusCode <- int HttpStatusCode.OK
        response.Headers <- headers

        let body = {| results=cleaned |}
        response.Body <- JsonConvert.SerializeObject(body)

        response
