import random

##--------SET UP GAMES-------------
#Set the gameData to its original state depending on the selected game

def SetupGames(gameID, data, room):

    data = {}

    #Prototype card game
    if gameID == 1:
        data["cardHistory"] = []

    elif gameID == 2:
        #Cycle Management
        data["currentCycleStep"] = "TestVote"   #TestVote --> Testing --> Trial --> Infecting
        data["stepSpecification"] = "Intro"     #Intro --> InProgress --> Conclusion

        #Player management
        data["Players"] = {}
        availableCharacters = ["Peasant"]
        for ID, player in room["Players"].items():
            characterChosen = random.randrange(len(availableCharacters))
            data["Players"][ID] = {
                "score" : 0,
                "role" : "SANE", #SANE/INFECTED
                "infected" : False,
                "immune": False,
                "suspiscious" : False,

                "character" : availableCharacters[characterChosen]
            }

            availableCharacters.pop(characterChosen)


        #Tests Management
        data["currentTestID"] = 0, #0 : None, 1 : ....,, 2 : ........, etc.


    return data


MiniGamesPageSettings = {
    "2": {
        "supportsSubPages" : True,
        "subPagesVariable" : "currentTestID"
    }
}