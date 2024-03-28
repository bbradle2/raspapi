port=5000
programuser="bbra"
#start script
printf "Start Test\n"

printf "___________\n\n"

process_test()
{
    url=$1
    echo "$url"
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
}

process_test "http://localhost:$port/RaspberryPiInfo/GetMemoryInfo"
process_test "http://localhost:$port/RaspberryPiInfo/GetCPUInfo"
process_test "http://localhost:$port/RaspberryPiInfo/GetSystemInfo"

printf "___________\n\n"
#end script
printf "End Test\n"
