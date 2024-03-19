port=5000
programuser=$USER

printf "Start Test\n"


printf "\n"
meminfourl="http://localhost:$port/RaspberryPiInfo/GetMemoryInfo"
printf "Calling $meminfourl\n"
memoryinfomessage=$(curl -s -GET $meminfourl -H "AUTHORIZED_USER: $programuser")

if [[ "$memoryinfomessage" == "" ]]; 
then
    printf 'could not connect to %s.\n' $meminfourl

elif [[ "$memoryinfomessage" == *"status"*  ]] && 
     [[ "$memoryinfomessage" == *"401"*  ]] &&  
     [[ "$memoryinfomessage" == *"title"*  ]] && 
     [[ "$memoryinfomessage" == *"Unauthorized"*  ]]; 
then
    printf "401 Unauthorized\n"

elif [[ "$memoryinfomessage" == *"status"*  ]] && 
     [[ "$memoryinfomessage" == *"400"*  ]] &&  
     [[ "$memoryinfomessage" == *"title"*  ]] &&
     [[ "$memoryinfomessage" == *"Bad Request"*  ]]; 
then
     printf "400 Bad Request\n"

else
    printf 'Success\n'
fi

printf "\n"
cpuinfourl="http://localhost:$port/RaspberryPiInfo/GetCPUInfo"
printf "Calling $cpuinfourl\n"
cpuinfomessage=$(curl -s -GET $cpuinfourl -H "AUTHORIZED_USER: $programuser")

if [[ "$cpuinfomessage" == "" ]]; then
    printf 'could not connect to %s.\n' $cpuinfourl

elif [[ "$cpuinfomessage" == *"status"*  ]] && 
     [[ "$cpuinfomessage" == *"401"*  ]] &&  
     [[ "$cpuinfomessage" == *"title"*  ]] && 
     [[ "$cpuinfomessage" == *"Unauthorized"*  ]];
then
    printf "401 Unauthorized\n"

elif [[ "$cpuinfomessage" == *"status"*  ]] && 
     [[ "$cpuinfomessage" == *"400"*  ]] &&  
     [[ "$cpuinfomessage" == *"title"*  ]] &&
     [[ "$cpuinfomessage" == *"Bad Request"*  ]];
then
    printf "400 Bad Request\n"

else
    printf 'Success\n'
fi

printf "\n"
systeminfourl="http://localhost:$port/RaspberryPiInfo/GetSystemInfo"
printf "Calling $systeminfourl\n"
systeminfomessage=$(curl -s -GET $systeminfourl -H "AUTHORIZED_USER: $programuser")

if [[ "$systeminfomessage" == "" ]]; 
then
    printf 'could not connect to %s.\n' $systeminfourl
    
elif [[ "$systeminfomessage" == *"status"*  ]] && 
     [[ "$systeminfomessage" == *"401"*  ]] &&  
     [[ "$systeminfomessage" == *"title"*  ]] && 
     [[ "$systeminfomessage" == *"Unauthorized"*  ]]; 
then
    printf "401 Unauthorized\n"

elif [[ "$systeminfomessage" == *"status"*  ]] && 
     [[ "$systeminfomessage" == *"400"*  ]] &&  
     [[ "$systeminfomessage" == *"title"*  ]] &&
     [[ "$systeminfomessage" == *"Bad Request"*  ]];
then
    printf "400 Bad Request\n"

else 
    printf 'Success\n'

fi

printf "\n"
printf "End Test\n"
