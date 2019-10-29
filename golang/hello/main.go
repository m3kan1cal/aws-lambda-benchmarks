package main

import (
	"bytes"
	"context"
	"encoding/json"
	"os"

	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/dynamodb"
	"github.com/aws/aws-sdk-go/service/dynamodb/dynamodbattribute"
)

// Response is of type APIGatewayProxyResponse since we're leveraging the
// AWS Lambda Proxy Request functionality (default behavior)
//
// https://serverless.com/framework/docs/providers/aws/events/apigateway/#lambda-proxy-integration
type Response events.APIGatewayProxyResponse

// Note is of type struct and is used for storing friendly results from DynamoDB.
type Note struct {
	NoteID   string `json:"noteId"`
	UserID   string `json:"userId"`
	Notebook string `json:"notebook"`
	Text     string `json:"text"`
}

// Handler is our lambda handler invoked by the `lambda.Start` function call
func Handler(ctx context.Context) (Response, error) {
	var buf bytes.Buffer
	desc := true

	svc := dynamodb.New(session.New(&aws.Config{
		Region: aws.String(os.Getenv("AWS_REGION")),
	}))

	var (
		notes                    []Note
		expressionAttributeNames = map[string]*string{
			"#note_text": aws.String("text"),
		}
		expressionAttributeValues = map[string]*dynamodb.AttributeValue{
			":user_id": {
				S: aws.String("arbiter"),
			},
		}
	)

	input := &dynamodb.QueryInput{
		TableName:                 aws.String(os.Getenv("DYNAMODB_TABLE")),
		IndexName:                 aws.String(os.Getenv("DYNAMODB_GSI_USERID_NOTEID")),
		ExpressionAttributeNames:  expressionAttributeNames,
		ExpressionAttributeValues: expressionAttributeValues,
		KeyConditionExpression:    aws.String("userId = :user_id"),
		ProjectionExpression:      aws.String("userId, noteId, notebook, #note_text"),
		ScanIndexForward:          &desc,
	}

	results, err := svc.Query(input)
	if err != nil {
		return Response{StatusCode: 404}, err
	}

	var tmpNotes []Note
	maperr := dynamodbattribute.UnmarshalListOfMaps(results.Items, &tmpNotes)
	if maperr != nil {
		return Response{StatusCode: 404}, maperr
	}
	notes = append(notes, tmpNotes...)

	body, err := json.Marshal(map[string]interface{}{
		"results": notes,
	})
	if err != nil {
		return Response{StatusCode: 404}, err
	}
	json.HTMLEscape(&buf, body)

	resp := Response{
		StatusCode:      200,
		IsBase64Encoded: false,
		Body:            buf.String(),
		Headers: map[string]string{
			"Content-Type": "application/json",
		},
	}

	return resp, nil
}

func main() {
	lambda.Start(Handler)
}
