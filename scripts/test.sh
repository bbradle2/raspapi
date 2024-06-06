#!/bin/bash
#assign global variables
port=5000
programuser=$USER
host="localhost"
process_id=$!
startserver=$1

#start test


if [[ "$startserver" == "1" ]];
then

    cd ..
    dotnet clean
    #dotnet build
    dotnet run &
    printf "Waiting for server to build and start\n\n"
    sleep 15
    printf "\nServer Started\n\n"

fi

printf "Start Test\n"
printf "###########\n\n"

getheaders()
{
    url=$1
    mkfifo headers
    curl -si -d --request -GET $url -H "AUTHORIZED_USER: $programuser" > headers &

    {
  # This line is guaranteed to be first, before any headers.
  # Read it separately.
read -r GitSemVer
while IFS=':' read -r key value; do
    # trim whitespace in "value"
    read -r value <<EOF
$value
EOF

      case $key in
        GitSemVer) GitSemVer="$value"
                 ;;

      esac
  done
} < headers

    rm headers
    printf 'GET Headers\n'
    printf '###########\n'
    printf 'GitSemVer:%s\n' $GitSemVer
    printf '\n'

}

runtest()
{
    url=$1
    printf "$url\n"
   
    json=$(curl -s -GET $url -H "AUTHORIZED_USER: $programuser" | jq .)

    if [[ "$json" == "" ]]; 
    then
        printf 'Could not connect to %s.\n' $url

    elif [[ "$json" == *"statusCode"*  ]] && 
         [[ "$json" == *"401"*  ]]; 
    then
        printf '%s\n' $json

    elif [[ "$json" == *"statusCode"*  ]] && 
         [[ "$json" == *"400"*  ]];
    then
        printf '%s\n' $json

    else
        printf 'Success\n'
    fi
     printf '\n'
}


printf 'Run Tests\n'
printf '#########\n'
printf '\n'

getheaders "http://$host:$port"


runtest "http://$host:$port/RaspberryPiInfo/GetMemoryInfo"
runtest "http://$host:$port/RaspberryPiInfo/GetCPUInfo"
runtest "http://$host:$port/RaspberryPiInfo/GetSystemInfo"

if [[ "$startserver" == "1" ]];
then
    while true; do
        sleep 1
        jobs_running=($(jobs -l | grep Running | awk '{print $2}'))
        if [ ${#jobs_running[@]} -eq 0 ]; then
            break
        fi
        echo "Stopping job(s): ${jobs_running[@]}"
        kill -SIGTERM $jobs_running
    done

    wait $process_id
    printf "\nServer Stopped\n"
fi

printf "\n"
printf "###########\n"
printf "End Test\n\n"

