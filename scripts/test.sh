printf "Start Test\n"

printf "Calling http://localhost:5000/RaspberryPiInfo/GetMemoryInfo\n"
curl -s -X "GET" "http://localhost:5000/RaspberryPiInfo/GetMemoryInfo" -H "accept: plain/text" | jq > Memoryinfo.json

printf "Calling http://localhost:5000/RaspberryPiInfo/GetCPUInfo\n"
curl -s -X "GET" "http://localhost:5000/RaspberryPiInfo/GetCPUInfo" -H "accept: plain/text" | jq > CPUInfo.json

printf "Calling http://localhost:5000/RaspberryPiInfo/GetSystemInfo\n"
curl -s -X "GET" "http://localhost:5000/RaspberryPiInfo/GetSystemInfo" -H "accept: plain/text" | jq > SystemInfo.json

printf "End Test\n"
