from http.server import HTTPServer, BaseHTTPRequestHandler
from io import BytesIO
import json
import re
import os
import random


SERVER = {
    #Server information (Host and Port)
    #"HOST": '127.0.0.1',
    "HOST": "0.0.0.0",
    "PORT": os.environ.get("PORT", 80),
    #"PORT": 8000,

    "Rooms": {}
}

class SimpleHTTPRequestHandler(BaseHTTPRequestHandler):


    def do_OPTIONS(self):
        self.send_response(200, "ok")
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'POST, GET, OPTIONS')
        self.send_header("Access-Control-Allow-Headers", "X-Requested-With")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        self.end_headers()


    def do_GET(self):

        repnumber = 200

        response = BytesIO() #Create a Byte Buffer that will store the bytes to return

        ##--------HTML RETURNED TO HEROKU---------
        #If a GET request is sent by the browser, under www.HOST:PORT/
        #Send back the HTML code to display

        decodedPath = self.path.split("/")

        if self.path == "/play":
            html = open("Mobile Client/joinRoom.html")
            response.write(html.read().encode())

        #Return Room state
        elif decodedPath[1] == "play":
            roomID = decodedPath[2]

            #Check if room has been created
            if roomID in SERVER["Rooms"].keys():

                #Return the current game played in this room
                currentGameID = SERVER["Rooms"][roomID]["currentGameID"]

                html = open("Mobile Client/Mini-Games/" + str(currentGameID) + ".html")
                response.write(html.read().encode())

            else:
                repnumber = 404
                print("404: Room not found, sending back to joinRoom.html ({{server}}/play)") #TO-DO


        ##--------SPECIAL GET REQUESTS HANDLERS----------
        #Special GET Requests are request that cannot be handled by the automatic GET handler
        #This can be the case if the wanted variable is before SERVER/Rooms
        #Or if a complex response is awaited

        elif self.path == "/SERVER/GetRooms":
            response.write(json.dumps(list(SERVER["Rooms"].keys())).encode())


        ##--------GET REQUESTS HANDLER----------
        #A GET request should send back the data asked for, provided with HOST:PORT/SERVER/XXX
        #If the path is empty (/SERVER/), all the server data needs to sent
        #Otherwise, if a path is provided, the asked variable should be returned

        else:
            paths = self.path.split("/")[2:] #Get requested variable path (SERVER/___GameID___/XXX)

            currentVar = SERVER["Rooms"]

            for path in paths:
                if (isinstance(currentVar, dict) and path in currentVar.keys()) or (isinstance(currentVar, list) and currentVar.length > path):
                    currentVar = currentVar[path]
                else:
                    repnumber = 404
                    print("404: Variable not found")

            if repnumber == 200:
                response.write(json.dumps(currentVar).encode())


        self.send_response(repnumber) #Add Response header --> 200:OK
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Headers", "*")
        self.send_header("Access-Control-Allow-Methods", "OPTIONS")

        self.end_headers() #Close Headers --> No more headers to add

        self.wfile.write(response.getvalue()) #Write the buffer bytes to the returned socketWriter

    def do_POST(self):

        ##-------POST REQUEST SETUP---------
        #POST Request setup code to accept all POST request sent to server
        #It reads the POST's body data
        #And sends back new data depending on the POST request (HOST:PORT/XXX)

        response = BytesIO()

        repnumber = 200 #Number used to send server status back

        if "Content-Length" in self.headers.keys() or "content-length" in self.headers.keys():
            content_length = int(self.headers['Content-Length']) #Get ContentLength header (length of json)
            body = self.rfile.read(content_length) #Read bytes from buffer upto "content-length" nb bytes
        else:
            #Read the file character by character
            body = "";
            currentCharacter = 1;
            while not "*" in body:
                print(currentCharacter)
                body = self.rfile.read(currentCharacter)
                currentCharacter += 1
            print(body)


        parsed_json = json.loads(body)

        ##-------CREATE GAME ROOM (POST by Unity)--------
        #Takes as parameters an "id" that corresponds to the ID for players to enter
        #This ID has been checked to be unique by the Unity client
        #Creating a room will add an element to the SERVER.Rooms array
        #A Room Dictionnary contains information about the server, players, and games, etc.

        if self.path == "/SERVER/CreateRoom":
            roomID = parsed_json["id"]
            SERVER["Rooms"][roomID] = {
                "id": roomID,
                "nbPlayers": 0,
                "Players": {},
                "currentGameID":0,
                "gameData": {}
            }

            response.write(json.dumps(SERVER["Rooms"][roomID]).encode())




        #The following requests require the room ID
        else:

            roomID = self.path.split("/")[2]
            if not roomID in SERVER["Rooms"].keys(): #Check if specified room ID exists
                print("404: Room not found")
                repnumber = 404 #Not found

            else:

                path = self.path[12:] #Remove /SERVER/XXXX from the path

                ##-------JOIN GAME ROOM (Post by HTML Client)---------
                #This POST Request takes as argument the name of the player.
                #When sent, a Player Dictionnay is added to the Player list in the selected Game
                #The ID of the Room is specified in the URL

                if path == "/JoinRoom":
                    Room = SERVER["Rooms"][roomID]

                    playerName = parsed_json["name"]
                    playerID = random.randint(0, 1000)
                    while playerID in Room["Players"].keys():
                        playerID = random.randint(0, 1000)

                    Room["nbPlayers"] += 1
                    Room["Players"][str(playerID)] = {
                        "name": playerName,
                        "id": str(playerID),
                        "isPartyLeader": (Room["nbPlayers"] == 1)
                    }

                    response.write(json.dumps(Room["Players"][str(playerID)]).encode())


                ##-------SELECT MINI-GAME (Post by Unity Client)---------
                #This POST Request takes as argument the ID (int) of the wished mini-game
                #When sent, the SERVER/Room/currentGameID is set to sent ID

                elif path == "/SelectGame":
                    Room = SERVER["Rooms"][roomID]

                    gameID = parsed_json["gameID"]
                    Room["currentGameID"] = gameID





        self.send_response(repnumber) #Add Response header --> 200:OK
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Headers", "*")
        self.send_header("Access-Control-Allow-Methods", "*")

        self.end_headers() #Close Headers --> No more headers to add

        self.wfile.write(response.getvalue()) #Write the buffer bytes to the returned socketWriter

httpd = HTTPServer((SERVER["HOST"], int(SERVER["PORT"])), SimpleHTTPRequestHandler)
print("SERVER STARTED at ", SERVER["HOST"], ":", SERVER["PORT"], sep="")
httpd.serve_forever()
