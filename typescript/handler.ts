'use strict'

import { APIGatewayProxyHandler } from 'aws-lambda';
import { DynamoDB, AWSError } from 'aws-sdk';
import 'source-map-support/register';
import { PromiseResult } from 'aws-sdk/lib/request';

export const hello: APIGatewayProxyHandler = async (_event, _context) => {

  let ddb = new DynamoDB({apiVersion: '2012-08-10', region: 'us-west-2'});
  const params = {
    TableName: process.env.DYNAMODB_TABLE,
    IndexName: process.env.DYNAMODB_GSI_USERID_NOTEID,
    KeyConditionExpression: 'userId = :userId',
    ProjectionExpression: 'userId, noteId, notebook, #note_text',
    ExpressionAttributeNames: {
      '#note_text': 'text'
    },
    ExpressionAttributeValues: {
      ':userId': { 'S': 'arbiter' }
    },
    ScanIndexForward: true
  };

  let response: PromiseResult<DynamoDB.QueryOutput, AWSError> =
    await ddb.query(params, function(err: AWSError, data: DynamoDB.QueryOutput) {
    if (err) {
      return err;
    } else {
      return data.Items;
    }
  }).promise();

  let unmarshalled = response.Items.map(element => {
    return DynamoDB.Converter.unmarshall(element);
  });

  return {
    statusCode: 200,
    body: JSON.stringify({
      results: unmarshalled
    }, null, 2),
  };
}
