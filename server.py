from socket import socket,AF_INET, SOCK_STREAM, timeout, SOL_SOCKET, SO_REUSEADDR
import struct
import sys
import random

from select import select

serverarg = sys.argv[1]
portarg = int(sys.argv[2])
server_addr = (serverarg, portarg)
packer = struct.Struct('ci')  

guessEnd = False
theNumber = random.randint(1,100)


with socket(AF_INET, SOCK_STREAM) as server:
    server.bind(server_addr)
    server.listen(10)
    server.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
    
    socketek = [ server ]
    try:
        while True:
            r,w,e = select(socketek,[],[],1) 
            
            if not (r or w or e):
                continue
            
            for s in r:
                if s is server:
                    client, client_addr = s.accept()
                    socketek.append(client)
                    print("Csatlakozott", client_addr)
                else:
                    data = s.recv(packer.size)
                    
                    if not data:
                        socketek.remove(s)
                        s.close()
                        print("Kilepett")
                    else:

                        op, num = packer.unpack(data)
                        op = op.decode()
                        print(op,num,'?')
                        if(guessEnd):
                            packed_data = packer.pack('V'.encode(),4)
                            s.sendall(packed_data)
                        elif(op == '=' and num == theNumber):
                            guessEnd = True
                            packed_data = packer.pack('Y'.encode(),4)
                            s.sendall(packed_data)
                        elif(op == '>'):
                            if(num < theNumber):
                                packed_data = packer.pack('I'.encode(),4)
                                s.sendall(packed_data)
                            else: 
                                packed_data = packer.pack('N'.encode(),4)
                                s.sendall(packed_data)

                        elif(op =='<'):
                            if( num > theNumber):
                                packed_data = packer.pack('I'.encode(),4)
                                s.sendall(packed_data)
                            else:
                                packed_data = packer.pack('N'.encode(),4)
                                s.sendall(packed_data)
                                
                        else:
                            packed_data = packer.pack('K'.encode(),4)
                            s.sendall(packed_data)
    except KeyboardInterrupt:
        print("SZERVER LEALL")                    