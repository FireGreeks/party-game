using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using SimpleJSON;

public class GameLoopManager : MonoBehaviour
{
    [NonSerialized] public JSONNode lastGameData;
    [NonSerialized] public JSONNode gameData;

    [NonSerialized] public int nbOfPlayers = 0;
    [NonSerialized] public int nbOfInfected = 0;
    [NonSerialized] public JSONNode PlayersDict;

    [NonSerialized] public string currentCycleStep;
    [NonSerialized] public string stepSpecification;

    [NonSerialized] public List<Transform> spawnPoints = new List<Transform>();
    [NonSerialized] public Dictionary<string, PlayersManager> getPlayerSpawn = new Dictionary<string, PlayersManager>();


    [Header("Spawn Settings")]
    [Range(5, 15)]
    [SerializeField] private float maxSpawnDistance;
    [SerializeField] private GameObject characterPrefab;

    [Header("Networking Settings")]
    [Range(1, 200)]
    [SerializeField] private int frameBetweenRequests = 200;

    [Header("Components")]
    [SerializeField] private Transform charactersTransform;

    [Header("Cycles")]
    [SerializeField] private GameObject roleDiscoveryUI;
    [SerializeField] private GameObject testVoteUI;
    [SerializeField] private GameObject miniGameManager;
    [SerializeField] private GameObject trialVoteUI;
    [SerializeField] private GameObject infectingUI;



    private Dictionary<Func<JSONNode, KeyValuePair<JSONNode, string>>, Action> eventListeners = new Dictionary<Func<JSONNode, KeyValuePair<JSONNode, string>>, Action>();


    // Start is called before the first frame update
    void Start()
    {
        //Get all the data when the game is first launched
        StartCoroutine(UpdateGameInfo(GetSpawnPoints));
    }

    void GetSpawnPoints()
    {
        lastGameData = gameData.Clone();

        //Determine spawn-points based on the number of players
        float xPos = charactersTransform.position.x - maxSpawnDistance;
        float offset = 2 * maxSpawnDistance / (nbOfPlayers + 1);
        for (int i = 1; i <= nbOfPlayers; i++)
        {
            xPos += offset;
            GameObject spawn = Instantiate(charactersTransform.gameObject);
            spawn.transform.position = charactersTransform.position + new Vector3(xPos, 0, 0);

            spawnPoints.Add(spawn.transform);
        }

        //Spawn a player on each spawn point and set their UI up
        int index = 0;
        foreach (JSONNode ID in gameData["Players"].Keys)
        {
            Transform t = spawnPoints[index];

            t.SetParent(charactersTransform);
            GameObject character = Instantiate(characterPrefab, t);
            PlayersManager playersManager = character.GetComponent<PlayersManager>();

            playersManager.playerID = ID.Value;
            playersManager.nameText.text = "Name: " + gameData["Players"][ID.Value]["name"].Value;

            //Save to which transform the player ID is linked to becuase Dictionnaries are nor ordered
            getPlayerSpawn[ID.Value] = playersManager;

            index++;
        }
    }


    private int f = 0;

    // Update is called once per frame
    void Update()
    {
        f++;
        if (f >= frameBetweenRequests)
        {
            StartCoroutine(UpdateGameInfo(StepManagement));
            f = 0;
        }
    }

    void StepManagement()
    {
        //Depending on all the info that has been retrieved find out what to display and what to do
        roleDiscoveryUI.SetActive(currentCycleStep == "RoleDiscovery");
        testVoteUI.SetActive(currentCycleStep == "TestVote");
        miniGameManager.SetActive(currentCycleStep == "Testing");
        trialVoteUI.SetActive(currentCycleStep == "Trial");
        infectingUI.SetActive(currentCycleStep == "Infecting");
    }

    public IEnumerator UpdateGameInfo(Action nextStep)
    {
        //Get all the info about the current game
        UnityWebRequest webRequest = UnityWebRequest.Get(ServerInfo.RoomURL + "/gameData");

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
            Debug.Log("Error: " + webRequest.error);
        else
        {
            lastGameData = (gameData == null) ? null : gameData.Clone();
            gameData = JSON.Parse(webRequest.downloadHandler.text);

            nbOfPlayers = gameData["nbOfPlayers"].AsInt;
            nbOfInfected = gameData["nbOfInfected"].AsInt;
            PlayersDict = gameData["Players"];
            currentCycleStep = gameData["currentCycleStep"].Value;
            stepSpecification = gameData["stepSpecification"].Value;

            nextStep();

            //Check event listeners
            foreach (KeyValuePair<Func<JSONNode, KeyValuePair<JSONNode, string>>, Action> listener in eventListeners)
            {
                if (listener.Key(lastGameData).Key.Value != listener.Key(gameData).Key.Value && listener.Key(gameData).Key.Value == listener.Key(gameData).Value)
                    listener.Value();
            }
        }
    }



    public void AddEventListener(Func<JSONNode, KeyValuePair<JSONNode, string>> flag, Action action)
    {
        eventListeners.Add(flag, action);
    }
}
