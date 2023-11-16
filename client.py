import sys,json

fileName = sys.argv[1]


with open(fileName,"r") as f:
    cs1 = json.load(f)

endpoints = cs1["end-points"]
switches = cs1["switches"]
links = cs1["links"]
circuits = cs1["possible-circuits"]
duration = cs1["simulation"]["duration"]
demands = cs1["simulation"]["demands"]

def routeExists(endU, endV):
    allRoutesBetween2Endpoints = []
    for route in circuits:
        if (route[0] == endU and route[len(route)-1] == endV):
         
                allRoutesBetween2Endpoints.append(route)
            
    if(len(allRoutesBetween2Endpoints)==0):
         return False
    else:
         return allRoutesBetween2Endpoints
    
edges = []
caps = []
conns = {}

for i in links:
    edge = ''.join(i["points"])
    edges.append(edge)
    revedge = ''.join(i["points"][::-1])

    edges.append(revedge)
    
for i in links:
     cap = i["capacity"]
     caps.append(cap)
     caps.append(cap)

for key in edges:
     for value in caps:
          conns[key] = value

def makeEdgeRoute(route):
    edgeRoute = []
    endU = route[0] 
    endV = route[1]
    for node in route:
        if(node != route[0]):
            endV = node
        if(endV != endU):    
            edge = endU+";"+endV
            edgeRoute.append(edge)
        endU = endV 
    return edgeRoute

linkStatus = {}
for link in links:
    edge = ';'.join(link["points"])
    linkStatus[edge] = False

def routeIsFree(route):
    for link in route:
        if(linkStatus[link] == True):
            return False
          
    return True
     
def findFreeRoute(endU,endV):
    possibleRoutes = routeExists(endU, endV)


    for i in possibleRoutes:
        
        if(routeIsFree(makeEdgeRoute(i))):
               return i
         
    return False




def reserveRoute(route):
    for i in route:
        linkStatus[i] = True

def freeRoute(route):
    for i in route:
        linkStatus[i] = False
    

reservedRoutes = {}

actionCount = 0;
for i in range(duration):
    currentTime = i+1
    for j in demands:
        
        if(j['start-time'] == currentTime):
            actionCount+= 1

           
            route = findFreeRoute(j['end-points'][0],j['end-points'][1])
            if(route):
                reserveRoute(makeEdgeRoute(route))
                key = ';'.join(j["end-points"])
                reservedRoutes[key] = route
                print("%d. igény foglalás: " % actionCount + j['end-points'][0]+"<->"+j['end-points'][1]+" st:%d"%currentTime+" - sikeres")
               
              
            else:
                print("%d. igény foglalás: " % actionCount + j['end-points'][0]+"<->"+j['end-points'][1]+" st:%d"%currentTime+" - sikertelen")
               
        if(j['end-time'] == currentTime):
            actionCount+= 1
            if(not all(value == False for value in linkStatus.values())):
                print("%d. igény felszabadítás: " % actionCount + j['end-points'][0]+"<->"+j['end-points'][1]+" st:%d"%currentTime)
            
                
                key = ';'.join(j["end-points"])
                if(reservedRoutes.get(key) != None):
                    toFree = reservedRoutes[key]    
                    freeRoute(makeEdgeRoute(toFree))
                    del(reservedRoutes[key])
                
               
        



    



    

        

     