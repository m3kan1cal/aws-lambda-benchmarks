package com.serverless

import com.amazonaws.services.dynamodbv2.AmazonDynamoDBClientBuilder
import com.amazonaws.services.dynamodbv2.document.DynamoDB
import com.amazonaws.services.dynamodbv2.document.Index
import com.amazonaws.services.dynamodbv2.document.Item
import com.amazonaws.services.dynamodbv2.document.QueryOutcome
import com.amazonaws.services.dynamodbv2.document.spec.QuerySpec
import com.amazonaws.client.builder.AwsClientBuilder
import com.amazonaws.services.lambda.runtime.Context
import com.amazonaws.services.lambda.runtime.RequestHandler
import org.apache.logging.log4j.LogManager

class Handler:RequestHandler<Map<String, Any>, ApiGatewayResponse> {
  override fun handleRequest(input:Map<String, Any>, context:Context):ApiGatewayResponse {

    val client = AmazonDynamoDBClientBuilder
          .standard()
          .build()

    val dynamoDB = DynamoDB(client)
    val table = dynamoDB.getTable(System.getenv("DYNAMODB_TABLE"))
    val index = table.getIndex(System.getenv("DYNAMODB_GSI_USERID_NOTEID"))

    val nameMap = mapOf("#note_text" to "text")
    val valueMap = mapOf(":user_id" to "arbiter")
    val spec = QuerySpec()
      .withProjectionExpression("userId, noteId, notebook, #note_text")
      .withKeyConditionExpression("userId = :user_id")
      .withNameMap(nameMap)
      .withValueMap(valueMap)
      .withScanIndexForward(true)

    LOG.info("made it here")

    var results = mutableListOf<Item>()
    val buildResults: (item: Item) -> Unit = {
      results.add(it)
    }

    index.query(spec).forEach(buildResults)

    return ApiGatewayResponse.build {
      statusCode = 200
      objectBody = ApiResponse(results.toString())
      headers = mapOf("Content-Type" to "application/json")
    }
  }

  companion object {
    private val LOG = LogManager.getLogger(Handler::class.java)
  }
}
