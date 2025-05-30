import http from 'k6/http';

export const options = {
  iterations: 1,
};

export default function () {
 
 
  const payload = JSON.stringify(
    [
      {
       gpioNumber: 23,
       gpioValue: null
      },
      {
       gpioNumber: 24,
       gpioValue: null
      },
      {
       gpioNumber: 25,
       gpioValue: null
      },
      {
       gpioNumber: 26,
       gpioValue: null
      },
      {
        gpioNumber: 27,
        gpioValue: null
       }
    ]
  );
  

  const params = {
    headers: {
      'Content-Type': 'application/json',
      'AUTHORIZED_USER': __ENV.USER
      },
    };
  

  //const response = http.put('http://localhost:5000/RaspberryPiGpio/SetGpiosHigh', payload, params);
  const response = http.put('http://localhost:5000/RaspberryPiGpio/SetGpiosLow', payload, params);   
  
  
  //const result = JSON.stringify(response.body, payload, params);
  const gpios = JSON.parse(response.body);
      for(var i = 0; i < gpios.length; i++)
        console.log(`VU ${__VU} received message: ${gpios[i].GpioNumber}:${gpios[i].GpioValue}`);


  // const cpuInfoObject = JSON.parse(response.body);

  // console.log(`VU ${__VU}: ${cpuInfoObject.productName}`);
  // console.log(`VU ${__VU}: ${cpuInfoObject.description}`);
  // console.log(`VU ${__VU}: ${cpuInfoObject.cpuObjects[1].id}`);
  // console.log(`VU ${__VU}: ${cpuInfoObject.cpuObjects[1].businfo}`);
}