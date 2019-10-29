#[macro_use]
extern crate log;

extern crate rusoto_core;
extern crate rusoto_dynamodb;

use std::collections::HashMap;
use std::default::Default;
use std::env;

use http::StatusCode;
use lambda_http::{lambda, Body, IntoResponse, Request, Response};
use lambda_runtime::{error::HandlerError, Context};
use serde_derive::{Serialize, Deserialize};
use serde_json::json;

use rusoto_core::Region;
use rusoto_dynamodb::{
    DynamoDb, DynamoDbClient, QueryInput, AttributeValue
};

#[derive(Deserialize, Serialize)]
struct Note {
    note_id: String,
    user_id: String,
    notebook: String,
    text: String,
}

impl From<&HashMap<String, AttributeValue>> for Note {
    fn from(attr_map: &HashMap<String, AttributeValue>) -> Self {
        let note_id = attr_map
            .get("noteId")
            .and_then(|v| v.s.clone())
            .unwrap_or_default();
        let user_id = attr_map
            .get("userId")
            .and_then(|v| v.s.clone())
            .unwrap_or_default();
        let notebook = attr_map
            .get("notebook")
            .and_then(|v| v.s.clone())
            .unwrap_or_default();
        let text = attr_map
            .get("text")
            .and_then(|v| v.s.clone())
            .unwrap_or_default();

        Note {
            note_id,
            user_id,
            notebook,
            text,
        }
    }
}

fn main() {
    simple_logger::init_with_level(log::Level::Info).unwrap();
    lambda!(handler)
}

fn handler(_: Request, _: Context) -> Result<impl IntoResponse, HandlerError> {

    // Query DynamoDB and respond with results.
    let client = DynamoDbClient::new(Region::UsWest2);
    let mut names = HashMap::new();
    names.insert(String::from("#note_text"), String::from("text"));

    let mut values = HashMap::new();
    let mut attr_value: AttributeValue = Default::default();
    attr_value.s = Some(String::from("arbiter"));
    values.insert(String::from(":user_id"), attr_value);

    let mut query_input: QueryInput = Default::default();
    query_input.table_name = env::var("DYNAMODB_TABLE").unwrap_or_default();
    query_input.index_name = Some(env::var("DYNAMODB_GSI_USERID_NOTEID").unwrap_or_default());
    query_input.expression_attribute_names = Some(names);
    query_input.expression_attribute_values = Some(values);
    query_input.key_condition_expression = Some(String::from("userId = :user_id"));
    query_input.projection_expression = Some(String::from("userId, noteId, notebook, #note_text"));
    query_input.scan_index_forward = Some(true);

    match client.query(query_input).sync() {
        Ok(results) => {
            let notes: Vec<Note> = results
                .items
                .unwrap_or_default()
                .iter()
                .map(|n| n.into()) // HashMap -> Note
                .collect();

            Ok(json!(notes).into_response())
        }
        Err(e) => {
            error!("Internal {}", e);
            Ok(build_resp(
                "internal error".to_owned(),
                StatusCode::NOT_FOUND,
            ))
        }
    }
}

// Simple response builder that uses a str msessage.
fn build_resp(msg: String, status_code: StatusCode) -> Response<Body> {
    Response::builder()
        .status(status_code)
        .body(msg.into())
        .expect("err creating response")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn handler_handles() {
        let request = Request::default();
        let expected = json!({
            "message": "Go Serverless v1.54! Your Rust function executed successfully!"
        })
        .into_response();
        let response = handler(request, Context::default())
            .expect("expected Ok(_) value")
            .into_response();
        assert_eq!(response.body(), expected.body())
    }
}