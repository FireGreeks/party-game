import random
import collections


MiniGamesPageSettings = {
    "2": {
        "supportsSubPages" : True,
        "subPagesVariable" : "currentTestID",
        "callOnDataChange": True
    }
}


##--------SET UP GAMES-------------
#Set the gameData to its original state depending on the selected game

def SetupGames(gameID, data, room):

    data = {}

    #Prototype card game
    if gameID == 1:
        data["cardHistory"] = []

    elif gameID == 2:
        #Cycle Management
        data["currentCycleStep"] = "RoleDiscovery"   #RoleDicovery --> TestVote --> Testing --> Trial --> Infecting
        data["stepSpecification"] = "Intro"     #Intro --> InProgress --> Conclusion

        #Player management
        data["nbOfPlayers"] = room["nbPlayers"]
        data["Players"] = {}
        data["nbOfInfected"] = 1
        #data["nbOfInfected"] = {4:1, 5:2, 6:2, 7:3, 8:3}[room["nbPlayers"]],

        data["infectedPlayers"] = []
        for i in range(data["nbOfInfected"]):
            index = -1
            while index in data["infectedPlayers"] or index == -1:
                index = random.randrange(len(room["Players"]))
            data["infectedPlayers"].append(list(room["Players"].keys())[index])

        availableCharacters = ["Peasant"]
        for ID, player in room["Players"].items():
            characterChosen = random.randrange(len(availableCharacters))
            data["Players"][ID] = {
                "name": room["Players"][ID]["name"],
                "score" : 0,

                "role" : "INFECTED" if ID in data["infectedPlayers"] else "SANE", #SANE/INFECTED
                "infected" : (ID in data["infectedPlayers"]),
                "immune": False,
                "suspiscious" : False,

                "character" : availableCharacters[characterChosen]
            }

            #availableCharacters.pop(characterChosen)


        #Tests Management
        data["currentTestID"] = 0 #0 : None, 1 : ....,, 2 : ........, etc.
        data["presentedTests"] = ["Test1", "Test2"]
        data["results"] = {}

        #Voting Management
        data["votes"] = {}


    return data


##--------ON GAME DATA CHANGE-------------
#Called each time game data has been chnaged through /AlterGameData
#Needs to be enbale in the miniGamePageSettings

def OnGameDataChange(gameID, data, room):

    #INFECTED
    if gameID == 2:

         #Check, if in voting phase, if all votes have been received
         if data["currentCycleStep"] in ["TestVote", "Trial", "Infecting"] and data["stepSpecification"] == "InProgress":

            #All players have voted
            if len(data["votes"].keys()) == data["nbOfPlayers"]:

                #Reset the results dictionnary
                data["results"] = {}

                #Add every vote (and newly infected players) into one array
                voteCounter = []
                infected = []
                for (playerID, vote) in data["votes"].items():

                    if data["currentCycleStep"] == "TestVote":
                        voteCounter.append(vote)
                    elif data["currentCycleStep"] == "Trial":
                        if data["Players"][playerID]["role"] == "SANE":
                            for a in vote:
                                voteCounter.append(a)
                    else:
                        if data["Players"][playerID]["role"] == "SANE":
                            voteCounter.append(vote)
                        elif not vote == -1:
                            infected.append(vote)

                #Order the array and find out which was the most voted
                orderedVotes = collections.Counter(voteCounter)
                mostVotes = list(orderedVotes.keys())[0]

                print("Results:", mostVotes, " - ", voteCounter, orderedVotes)

                #Setup the results and the next step depending on current step and votes
                if data["currentCycleStep"] == "TestVote":
                    data["results"]["nextTest"] = mostVotes
                    data["currentTestID"] = data["presentedTests"][mostVotes]
                    data["stepSpecification"] = "Conclusion"

                elif data["currentCycleStep"] == "Trial":
                    data["stepSpecification"] = "Conclusion"

                    if -1 in orderedVotes.keys():
                        data["results"]["trialSuccess"] = False
                    elif len(orderedVotes.keys()) == 1:
                        data["results"]["trialSuccess"] = True
                        data["results"]["playerKilled"] = mostVotes
                    else:
                        data["results"]["trialSuccess"] = False

                else:
                    data["stepSpecification"] = "Conclusion"
                    data["results"]["suspisciousPlayer"] = mostVotes

                    #Reset infceted variable
                    for (playerID, player) in data["Players"].items():
                        if player["role"] == "INFECTED":
                            player["infected"] = True
                        else:
                            player["infected"] = False

                    #Chnage infected variable based on the newly received info
                    for a in infected:
                        player = data["Players"][a]
                        if player["role"] == "INFECTED":
                            player["infected"] = False
                        else:
                            player["infected"] = True

                data["votes"] = {}



##--------CUSTOM FUNCTION-------------
#Called when a client POST to /CallGameFunction
#A set of functions depending on the current game played

def CustomFunction(gameID, functionName, arguments, room):
    gameData = room["gameData"]

    #INFECTED
    if gameID == 2:

        #def NextStep(string currentCycle, string specification, bool stepSpecification)
        #Will go to the next step of the night/day cycle
        if functionName == "NextStep":
            flag = False
            if arguments[2]:
                if arguments[1] == "Intro":
                    gameData["stepSpecification"] = "InProgress"
                elif arguments[1] == "InProgress":
                    gameData["stepSpecification"] = "Conclusion"
                else:
                    flag = True

            if not arguments[2] or flag:
                gameData["stepSpecification"] = "Intro"

                if arguments[0] == "TestVote":
                    gameData["currentCycleStep"] = "Testing"
                elif arguments[0] == "Testing":
                    gameData["currentCycleStep"] = "Trial"
                elif arguments[0] == "Trial":
                    gameData["currentCycleStep"] = "Infecting"
                elif arguments[0] == "Infecting":
                    gameData["currentCycleStep"] = "TestVote"








