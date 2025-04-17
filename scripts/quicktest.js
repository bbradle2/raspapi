import http from 'k6/http';

export const options = {
  iterations: 1,
};

export default function () {
  const response = http.get('http://localhost:5000/RaspberryPiInfo/GetCPUInfo');
  console.log(`VU ${__VU}: leaving the website`);
  const result = JSON.stringify(response.body);

  const cpuInfoObject = JSON.parse(response.body);

  console.log(`VU ${__VU}: ${cpuInfoObject.productName}`);
  console.log(`VU ${__VU}: ${cpuInfoObject.description}`);
  console.log(`VU ${__VU}: ${cpuInfoObject.cpuObjects[1].id}`);
  console.log(`VU ${__VU}: ${cpuInfoObject.cpuObjects[1].businfo}`);
}