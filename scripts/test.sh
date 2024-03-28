#assign global variables
port=5000
programuser=$USER
host="localhost"


#start test
printf "Start Test\n"

printf "___________\n\n"

process_test()
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
}

process_test "http://$host:$port/RaspberryPiInfo/GetMemoryInfo"
process_test "http://$host:$port/RaspberryPiInfo/GetCPUInfo"
process_test "http://$host:$port/RaspberryPiInfo/GetSystemInfo"

printf "___________\n\n"
printf "End Test\n"
#end test
