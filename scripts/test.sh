printf "Start Test\n"
port=5000

printf "Calling http://localhost:$port/RaspberryPiInfo/GetMemoryInfo\n"
memoryinfocount=$(curl -X GET http://localhost:$port/RaspberryPiInfo/GetMemoryInfo | jq | wc -l) # Memoryinfo.json

if [ $memoryinfocount -gt 0 ]; 
then
    printf 'Success.\n'
else
    printf 'Fail.\n'
fi
#printf "Calling http://localhost:5000/RaspberryPiInfo/GetCPUInfo\n"
#curl -X GET http://localhost:5000/RaspberryPiInfo/GetCPUInfo | jq > CPUInfo.json

#printf "Calling http://localhost:5000/RaspberryPiInfo/GetSystemInfo\n"
#curl -X GET http://localhost:5000/RaspberryPiInfo/GetSystemInfo | jq > SystemInfo.json

printf "End Test\n"
