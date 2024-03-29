#!/bin/bash
#assign global variables
port=5000
programuser=$USER
host="localhost"
process_id=$!

#start test
printf "Start Test\n"
cd ..
dotnet run &
printf "Waiting for server to build and start\n\n"
sleep 5


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

#for pid in $(jobs -p); do kill $pid; done

printf "###########\n"
printf "End Test\n"
kill $(jobs -p)

wait $process_id
#echo "Exit status: $?"
#end test
