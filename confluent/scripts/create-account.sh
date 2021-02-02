if [ "$#" -le 5 ]
  then
    echo "Usage: $0 CONFLUENT-ENVIRONMENT-NAME CONFLUENT-CLUSTER-NAME SERVICE-ACCOUNT-NAME SERVICE-ACCOUNT-DESCRIPTION ESW-ENVIRONMENT ESW-DOMAIN RECREATE-KEYS" >&2
    exit 1
fi

ConfluentEnvironmentName=$1
ConfluentClusterName=$2
ServiceAccountName=$3
ServiceAccountDescription=$4
EswEnvironment=$5
EswDomain=$6
RecreateKeys=$7 || false

if [ -z "$ConfluentEnvironmentName" ]
  then
    echo "No Confluent environment name provided"
    exit 1
fi

if [ -z "$ConfluentClusterName" ]
  then
    echo "No Confluent cluster name provided"
    exit 1
fi

if [ -z "$ServiceAccountName" ]
  then
    echo "No service account name provided"
    exit 1
fi

if [ -z "$ServiceAccountDescription" ]
  then
    echo "No service account description provided"
    exit 1
fi

if [ -z "$EswEnvironment" ]
  then
    echo "No ESW environment provided"
    exit 1
fi

if [ -z "$EswDomain" ]
  then
    echo "No ESW domain provided"
    exit 1
fi

ccloud environment list > /dev/null || { echo "Error occurred during connection test" ; exit 1; }

EnvironmentId=$(ccloud environment list -o json | jq -r --arg ENVIRONMENTNAME "$ConfluentEnvironmentName" '.[] | select(.name==$ENVIRONMENTNAME) | .id')
if [ -z "$EnvironmentId" ]
    then
        echo "Environment with name $1 is not found" >&2
        echo "Valid environments:"
        echo $(ccloud environment list -o json | jq '.[] | .name')
        exit 1
fi
(ccloud environment use $EnvironmentId) > /dev/null 2>&1 || { echo "Error occurred using environment $ConfluentEnvironmentName" ; exit 1; }

ClusterId=$(ccloud kafka cluster list -o json | jq -r --arg CLUSTERNAME "$ConfluentClusterName" '.[] | select(.name==$CLUSTERNAME) | .id')
if [ -z "$ClusterId" ]
    then
        echo "Cluster with name $2 is not found" >&2
        echo "Valid clusters:"
        echo $(ccloud kafka cluster list -o json | jq '.[] | .name')
        exit 1
fi
(ccloud kafka cluster use $ClusterId) > /dev/null 2>&1 || ( echo "Error occurred using cluster $ClusterId" ; exit 1; )

ServiceAccountId=$(ccloud service-account list -o json | jq -r --arg NAME "$ServiceAccountName" '.[] | select(.name==$NAME) | .id')
if [ -z "$ServiceAccountId" ]
    then
        ServiceAccountId=$(ccloud service-account create $ServiceAccountName --description "$ServiceAccountDescription" -o json | jq -r .id)
fi

ccloud kafka acl create --allow --service-account $ServiceAccountId --operation CREATE --prefix --topic $EswEnvironment-$EswDomain- > /dev/null
ccloud kafka acl create --allow --service-account $ServiceAccountId --operation WRITE --prefix --topic $EswEnvironment-$EswDomain- > /dev/null
ccloud kafka acl create --allow --service-account $ServiceAccountId --operation READ --prefix --topic $EswEnvironment- > /dev/null

ccloud kafka acl create --allow --service-account $ServiceAccountId --operation DESCRIBE --prefix --consumer-group $EswEnvironment-$EswDomain- > /dev/null
ccloud kafka acl create --allow --service-account $ServiceAccountId --operation READ --prefix --consumer-group $EswEnvironment-$EswDomain- > /dev/null

ClusterApiKey=$(ccloud api-key list -o json | jq -r --arg ACCOUNTID "$ServiceAccountId" --arg RESOURCEID "$ClusterId" '.[] | select(.owner==$ACCOUNTID and .resource_id==$RESOURCEID) | .key')
if [ -z "$ClusterApiKey" ]
    then
        ClusterApi=$(ccloud api-key create --service-account $ServiceAccountId --resource $ClusterId -o json | jq .)
    else
        if [ $RecreateKeys ]
            then
                (ccloud api-key delete $ClusterApiKey) > /dev/null 2>&1
                ClusterApi=$(ccloud api-key create --service-account $ServiceAccountId --resource $ClusterId -o json | jq .)
            else
                ClusterApi=$(jq -n -r --arg ApiKey "$ClusterApiKey" '{key: $ApiKey, secret:""}')
        fi
fi

SchemaRegistryId=$(ccloud schema-registry cluster describe -o json | jq -r .cluster_id)
SchemaRegistryApiKey=$(ccloud api-key list -o json | jq -r --arg ACCOUNTID "$ServiceAccountId" --arg RESOURCEID "$SchemaRegistryId" '.[] | select(.owner==$ACCOUNTID and .resource_id==$RESOURCEID) | .key')
if [ -z "$SchemaRegistryApiKey" ]
    then
        SchemaRegistryApi=$(ccloud api-key create --service-account $ServiceAccountId --resource $SchemaRegistryId -o json | jq .)
    else
        if [ $RecreateKeys ]
            then
                (ccloud api-key delete $SchemaRegistryApiKey) > /dev/null 2>&1
                SchemaRegistryApi=$(ccloud api-key create --service-account $ServiceAccountId --resource $SchemaRegistryId -o json | jq .)
            else
                SchemaRegistryApi=$(jq -n -r --arg ApiKey "$SchemaRegistryApiKey" '{key: $ApiKey, secret:""}') 
        fi
fi
jq -n --argjson CLUSTERAPI "$ClusterApi" --argjson SCHEMAREGISTRYAPI "$SchemaRegistryApi" '{Kafka: $CLUSTERAPI, SchemaRegistry: $SCHEMAREGISTRYAPI}' | jq .