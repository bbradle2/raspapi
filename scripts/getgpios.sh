#!/bin/bash

websocket_url="ws://localhost:5000/GetGpios"

# Function to send a message and receive response
send_and_receive() 
{
    request=$(wscat -c "$websocket_url" -w 1 -x  '[]' --header 'AUTHORIZED_USER':'bbrad')
  

    response=$(echo "$request" | grep -v "Connected (press CTRL+C to quit)" | sed 's/> //')

    # Handle empty responses
    if [[ -z "$response" ]]; then
        echo "No response received."
    else
         printf '%s' $response | jq .
    fi

}

send_and_receive
