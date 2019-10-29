#!/bin/bash

rustApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
rustEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

fsharpApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
fsharpEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

csharpApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
csharpEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

kotlinApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
kotlinEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

golangApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
golangEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

pythonApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
pythonEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

typescriptApiKey="<API-KEY-FROM-CLOUDFORMATION-OUTPUTS>"
typescriptEndpoint="https://<API-GATEWAY-ID>.execute-api.<AWS-REGION>.amazonaws.com/<API-STAGE>/<NAME-OF-SERVICE>"

# Round 1: started roughly at 14:45 PM EST on 10/20/2019 (@shouldroforion)
# Round 2: started roughly at 16:17 PM EST on 10/21/2019 (@shouldroforion)
# Round 3: started roughly at 23:42 PM EST on 10/26/2019 (@shouldroforion)
# Round 4: started roughly at 15:36 PM EST on 10/27/2019 (@shouldroforion)

for (( c=1; c<=15000; c++ )); do

    echo -e "\nFiring 10 parallel API cannon requests against APIs: $c x 10 per API so far"

    # Rust API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $rustApiKey" $rustEndpoint &
    done

    # FSharp API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $fsharpApiKey" $fsharpEndpoint &
    done

    # CSharp API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $csharpApiKey" $csharpEndpoint &
    done

    # Golang API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $golangApiKey" $golangEndpoint &
    done

    # Kotlin API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $kotlinApiKey" $kotlinEndpoint &
    done

    # Python API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $pythonApiKey" $pythonEndpoint &
    done

    # TypeScript API monitoring.
    for value in {1..10}; do
        curl -X GET -H "X-Api-Key: $typescriptApiKey" $typescriptEndpoint &
    done

    sleep 1 # Pause 2secs.

done

