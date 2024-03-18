printf "Start Test\n"
port=5000

meminfourl="http://localhost:$port/RaspberryPiInfo/GetMemoryInfo"
printf "Calling $meminfourl\n"
memoryinfocount=$(curl -X GET $meminfourl | jq | wc -l)

if [ $memoryinfocount -gt 0 ]; 
then
    printf 'Success.\n'
else
    printf 'Fail.\n'
fi

cpuinfourl="http://localhost:$port/RaspberryPiInfo/GetCPUInfo"
printf "Calling $cpuinfourl\n"
cpuinfocount=$(curl -X GET $cpuinfourl | jq | wc -l) 

if [ $cpuinfocount -gt 0 ]; 
then
    printf 'Success.\n'
else
    printf 'Fail.\n'
fi


systeminfourl="http://localhost:$port/RaspberryPiInfo/GetSystemInfo"
printf "Calling $systeminfourl\n"
systeminfocount=$(curl -X GET $systeminfourl | jq | wc -l)

if [ $systeminfocount -gt 0 ]; 
then
    printf 'Success.\n'
else
    printf 'Fail.\n'
fi

printf "End Test\n"
