import json
import os

import boto3
from boto3.dynamodb.conditions import Key


def hello(event, context):
    dynamodb = boto3.resource("dynamodb", region_name=os.environ["AWS_REGION"])
    table = dynamodb.Table(os.environ["DYNAMODB_TABLE"])

    # Fetch all items from the database by index.
    items = table.query(
        IndexName=os.environ["DYNAMODB_GSI_USERID_NOTEID"],
        ExpressionAttributeNames={
            "#note_text": "text"
        },
        ProjectionExpression="userId, noteId, notebook, #note_text",
        KeyConditionExpression=Key("userId").eq("arbiter"),
        ScanIndexForward=True
    )

    results = items["Items"] if "Items" in items else {}

    return {
        "statusCode": 200,
        "body": json.dumps({"results": results})
    }
