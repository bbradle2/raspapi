#!/bin/bash

websocket_url="ws://localhost:5000/GetGpios"

# Function to send a message and receive response
send_and_receive() 
{
    response=$(wscat -c "$websocket_url" -w 1 -x  '[]' --header 'AUTHORIZED_USER':"$USER")
  

    returnVal=$(echo "$response" | grep -v "Connected (press CTRL+C to quit)" | sed 's/> //')

    # Handle empty returnVal
    if [[ -z "$returnVal" ]]; then
        echo "No returnVal received."
    else
         printf '%s' $returnVal | jq .
    fi

}

send_and_receive
