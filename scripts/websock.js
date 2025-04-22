import { randomString, randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
import ws from 'k6/ws';
import { check, sleep } from 'k6';

const sessionDuration = randomIntBetween(1000, 2000); // user session between 3s and 6s

export const options = {
  vus: 1,
  iterations: 1,
};

export default function () {
  const url = `ws://localhost:5000/GetGpios`;
  const params = { tags: { 
                     my_tag: 'my ws session'
                   },
                   headers: {
                    'Content-Type':'application/json',
                    'AUTHORIZED_USER': 'bbrad' 
                   } 
                 };
  const user = `user_${__VU}`;

  const res = ws.connect(url, params, function (socket) {
    socket.on('open', function open() {
      console.log(`VU ${__VU}: connected`);

      socket.send(JSON.stringify([]));

      socket.setInterval(function timeout() {
        socket.send(
          JSON.stringify([])
        );
      }, randomIntBetween(1000, 2000)); // say something every 1-2 seconds
    });

    socket.on('ping', function () {
      console.log('PING!');
    });

    socket.on('pong', function () {
      console.log('PONG!');
    });

    socket.on('close', function () {
      console.log(`VU ${__VU}: disconnected`);
    });

    socket.on('message', function (message) {
      
      const gpios = JSON.parse(message);
      for(var i = 0; i < gpios.length; i++)
        console.log(`VU ${__VU} received message: ${gpios[i].GpioNumber}:${gpios[i].GpioValue}`);

    });

    socket.setTimeout(function () {
      console.log(`VU ${__VU}: ${sessionDuration}ms passed, leaving the website`);
      socket.send(JSON.stringify({ msg: 'Goodbye!', user: user }));
      socket.close();
    }, sessionDuration);

    // socket.setTimeout(function () {
    //   console.log(`Closing the socket forcefully 3s after graceful LEAVE`);
    //   socket.close();
    // }, sessionDuration + 3000);
  });

  check(res, { 'Connected successfully': (r) => r && r.status === 101 });
  sleep(1);
}