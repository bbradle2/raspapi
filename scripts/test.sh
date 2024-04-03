#!/bin/bash
#assign global variables
port=5000
programuser=$USER
host="localhost"
process_id=$!

#start test

cd ..
dotnet clean
dotnet build
dotnet run &
printf "Waiting for server to build and start\n\n"
sleep 5

printf "\nServer Started\n\n"
printf "Start Test\n"
printf "###########\n\n"

runtest()
{
    url=$1
    printf "$url\n"
    message=$(curl -s -GET $url -H "AUTHORIZED_USER: $programuser" | jq .)

    if [[ "$message" == "" ]]; 
    then
        printf 'Could not connect to %s.\n' $url

    elif [[ "$message" == *"statusCode"*  ]] && 
         [[ "$message" == *"401"*  ]]; 
    then
        printf '%s\n' $message

    elif [[ "$message" == *"statusCode"*  ]] && 
         [[ "$message" == *"400"*  ]];
    then
        printf '%s\n' $message

    else
        printf 'Success\n'
    fi
     printf '\n'
}

runtest "http://$host:$port/RaspberryPiInfo/GetMemoryInfo"
runtest "http://$host:$port/RaspberryPiInfo/GetCPUInfo"
runtest "http://$host:$port/RaspberryPiInfo/GetSystemInfo"
#for pid in $(jobs -p); do echo $pid; done

kill -SIGTERM $(jobs -p)

wait $process_id

printf "\nServer Stopped\n"
printf "\n"
printf "###########\n"
printf "End Test\n\n"

#echo "Exit status: $?"
#end test
