# aws-lambda-benchmarks
Repository of Serverless projects targeting AWS Lambda and API Gateway for benchmarking.

# Hitting endpoints

Test the deployment with `curl` once deployed for each stack.

```bash
curl -X POST -H 'X-Api-Key: <API-KEY-FROM-CLOUDFORMATION>' \
    'https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>'
```

# Rust deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./rust
profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```

# FSharp deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./fsharp
chmod +x ./build.sh
./build.sh

profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```

# CSharp deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./csharp
chmod +x ./build.sh
./build.sh

profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```

# Golang deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./golang
make clean build

profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```

# Kotlin deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./kotlin
./gradlew shadowJar

profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```

# Python deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./python

profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```

# TypeScript/Node.js deployment

Deploy to AWS Lambda and API Gateway.

```bash
cd ./typescript

profile="<AWS-PROFILE>"
region="<AWS-REGION>"

# Build S3 utilities bucket.
sls deploy --aws-profile $profile \
    --region $region
```
