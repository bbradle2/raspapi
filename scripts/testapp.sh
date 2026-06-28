#!/bin/bash
#assign global variables
port=8080
programuser=$USER
host="raspberrypi51"
process_id=$!
startservice=$1

BLACK=$(tput setaf 0)
RED=$(tput setaf 1)
GREEN=$(tput setaf 2)
YELLOW=$(tput setaf 3)
LIME_YELLOW=$(tput setaf 190)
POWDER_BLUE=$(tput setaf 153)
BLUE=$(tput setaf 4)
MAGENTA=$(tput setaf 5)
CYAN=$(tput setaf 6)
WHITE=$(tput setaf 7)
BRIGHT=$(tput bold)
NORMAL=$(tput sgr0)
BLINK=$(tput blink)
REVERSE=$(tput smso)
UNDERLINE=$(tput smul)

#start test
shopt -s extglob

getheader()
{

url=$1
while IFS=':' read key value; do
    # trim whitespace in "value"
    value=${value##+([[:space:]])}; value=${value%%+([[:space:]])}

    case "$key" in
        GitSimVer) GitSimVer="$value"
                ;;
        Content-Type) CT="$value"
                ;;
        HTTP*) read PROTO STATUS MSG <<< "$key{$value:+:$value}"
                ;;
     esac
done < <(curl -sI $url)
} 

runtest()
{
    verb=$1
    url=$2
    
    printf "${GREEN}Calling $verb $url\n"
    
    if [[ "$verb" == "GET" ]];
    then
        resp=$(curl -s -X  $verb $url -H 'Content-Type: application/json; charset=UTF-8' -H "AUTHORIZED_USER: $programuser" -d $gpioObject)
    else
        resp=$(curl -s -X  $verb $url -H 'Content-Type: application/json; charset=UTF-8' -H "AUTHORIZED_USER: $programuser" -d $gpioObject )
    fi

    if [[ "$resp" == "" ]]; 
    then
        printf ''${RED}'Status: Fail\n'
        printf ''${RED}'Could not connect to %s.\n' $url

    elif [[ "$resp" == *"statusCode"*  ]] && 
         [[ "$resp" == *"401"*  ]]; 
    then
        printf ''${RED}'Status: Fail\n' 
        printf '%s\n' $resp | jq .

    elif [[ "$resp" == *"statusCode"*  ]] && 
         [[ "$resp" == *"403"*  ]]; 
    then
        printf ''${RED}'Status: Fail\n' 
        printf '%s\n' $resp | jq .

    elif [[ "$resp" == *"statusCode"*  ]] && 
         [[ "$resp" == *"400"*  ]];
    then
        printf ''${RED}'Status: Fail\n'
        printf '%s' $resp | jq .

    elif [[ "$resp" == *"statusCode"*  ]] && 
         [[ "$resp" == *"520"*  ]];
    then
        printf ''${RED}'Status: Fail\n'
        printf '%s' $resp | jq .

    elif [[ "$resp" == *"422"*  ]]; 
    then
        printf ''${YELLOW}'Status: Warning\n'
        printf '%s' $resp 

    else
        printf ''${GREEN}'Status: Success\n'
        printf '%s' "$resp" | jq .

    fi

    printf '\n\n'

}

getheader "http://$host:$port"

printf ''${CYAN}'GET Headers\n'
printf ''${CYAN}'###########\n'
printf ''${CYAN}'GitSemVer:%s\n' $GitSimVer
printf ''${CYAN}'CT:%s\n' $CT
printf ''${CYAN}'STATUS:%s\n' $STATUS
printf ''${NORMAL}'\n'

gpioObject='{"gpioNumber":24,"gpioValue":0}'
runtest PUT "http://$host:$port/putgpiobynumber"

gpioObject='{"gpioNumber":24}'
runtest GET "http://$host:$port/getgpiobynumber"

gpioObject='{}'
runtest GET "http://$host:$port/getinfo"



