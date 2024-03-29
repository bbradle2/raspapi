#assign global variables
port=5000
programuser=$USER
host="localhost"


#start test
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

printf "###########\n"
printf "End Test\n"
#end test
