from socket import socket,AF_INET, SOCK_STREAM, timeout, SOL_SOCKET, SO_REUSEADDR
import struct
import sys
import math
import time
import random

serverarg = sys.argv[1]
portarg = int(sys.argv[2])

mini = 1;
maxi = 100;    


server_addr = (serverarg, portarg)
packer = struct.Struct('ci')

with socket(AF_INET, SOCK_STREAM) as client:
    client.connect(server_addr)
    
    try:
        while(True):
            time.sleep(random.randint(0,2))
            slicepoint = math.floor((maxi+mini)/2)
            
            if((mini+1) == maxi):
                packed_data = packer.pack('='.encode(),maxi)
                client.sendall(packed_data)
            else:
                packed_data = packer.pack('>'.encode(),slicepoint)
                client.sendall(packed_data)
            print(packed_data)
            data = client.recv(packer.size)
            resp, _ = packer.unpack(data)
            resp = resp.decode()
            print(resp);
            if resp == 'Y':
                print("win")
                break
            if(resp == 'K'):
                break
            elif(resp == 'V'):
                break
            elif(resp == 'I'):
                mini = slicepoint
                print(mini)

            
            elif(resp == 'N'):
                maxi = slicepoint
                print(maxi)
    except KeyboardInterrupt:
        print("KLIENS LEALL")