#!/bin/bash
#assign global variables
port=5000
programuser=$USER
host="localhost"
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

cleanup()
{
    if [[ "$startservice" == "1" ]];
    then
        while true; do
            sleep 1
            jobs_running=($(jobs -l | grep Running | awk '{print $2}'))
            if [ ${#jobs_running[@]} -eq 0 ]; then
                break
            fi
            printf "${GREEN}Stopping job(s):%s\n\n" ${jobs_running[@]}
            kill -SIGTERM $jobs_running
        done

        wait $process_id
        printf "${GREEN}\nService Stopped\n"
    fi
}

getheader()
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
    printf ''${YELLOW}'GET Headers\n'
    printf ''${YELLOW}'###########\n'
    printf ''${YELLOW}'GitSemVer:%s\n' $GitSemVer
    printf ''${NORMAL}'\n'

}

runtest()
{
    verb=$1
    url=$2
    printf "${GREEN}Calling $verb $url\n"

    resp=$(curl -s -X $verb $url -H "AUTHORIZED_USER: $programuser")
    #jobs_running=($(jobs -l | grep Running | awk '{print $2}'))

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

    else
        printf ''${GREEN}'Status: Success\n'
        printf '%s' "$resp" | jq .

    fi

    printf '\n\n'

}

startservice()
{
    if [[ "$startservice" == "1" ]];
    then
        cd ..
        dotnet clean
        #dotnet build
        dotnet run &
        printf "${GREEN}Waiting for Service to build and start\n\n"
        sleep 20
        printf "${GREEN}\nService Started\n\n"
    fi
}

startservice

printf "${GREEN}Start Test\n"
printf "${GREEN}###########\n\n"
printf 'Run Tests\n'
printf '#########\n'
printf '\n'

getheader "http://$host:$port"

runtest GET "http://$host:$port/RaspberryPiInfo/GetCPUInfo"
runtest GET "http://$host:$port/RaspberryPiInfo/GetSystemInfo"
runtest GET "http://$host:$port/RaspberryPiInfo/GetMemoryInfo"
runtest GET "http://$host:$port/RaspberryPiInfo/GetTemperatureInfo"

runtest PUT "http://$host:$port/RaspberryPiGpio/SetLedOn" 
sleep 1

runtest GET "http://$host:$port/RaspberryPiGpio/GetLedStatus" 
sleep 1

runtest PUT "http://$host:$port/RaspberryPiGpio/SetLedOff"
sleep 1


runtest PUT "http://$host:$port/RaspberryPiGpio/BlinkLed"
sleep 1

#This call will test semaphore code. should not be able to set any gpios without semaphore release. 
#should blink 2 times and take around 8 seconds.
#START=$(date "+%s")
#runtest PUT "http://$host:$port/RaspberryPiGpio/BlinkLed" &
#END=$(date "+%s")
#echo Blinking led 2 times took $((END-START)) seconds.


runtest PUT "http://$host:$port/RaspberryPiGpio/SetLedOn"
sleep 1

runtest PUT "http://$host:$port/RaspberryPiGpio/SetLedOff"
sleep 1

#runtest GET "http://$host:$port/RaspberryPiGpio/GetLedStatus"
#sleep 1

cleanup

printf "\n"
printf "${GREEN}###########\n"
printf "${GREEN}End Test\n\n"

