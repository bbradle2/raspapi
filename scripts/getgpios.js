import { randomString, randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
import ws from 'k6/ws';
import { check, sleep } from 'k6';

//const sessionDuration = randomIntBetween(1000, 2000); // user session between 1s and 2s

// export const options = {
//   vus: 1,
//   iterations: 1,
// };
const iterations = 100;
var currentIterator = 0;

export default function () {
  const url = `ws://localhost:5000/RaspberryPiGpio/GetGpios`;
  const params = { 
                   headers: {
                    'Content-Type':'application/json',
                    'AUTHORIZED_USER': __ENV.USER
                   } 
                 };
  const user = `user_${__VU}`;

  const res = ws.connect(url, params, function (socket) {
    socket.on('open', function open() {
      console.log(`connected`);

      socket.send(JSON.stringify([]));
     
    });
  
    socket.on('close', function () {
      console.log(`disconnected`);
    });

    socket.on('message', function (message) {
      const gpios = JSON.parse(message);
      for(var i = 0; i < gpios.length; i++) 
        console.log(`received message: ${gpios[i].GpioNumber}:${gpios[i].GpioValue}`);

      if(currentIterator < iterations)
        socket.send(message);

      if(currentIterator >= iterations)
        socket.close();

      //sleep(1);
      currentIterator++;      
    });
   
  });

  check(res, { 'Completed successfully': (r) => r && r.status === 101 });
  sleep(1);
}