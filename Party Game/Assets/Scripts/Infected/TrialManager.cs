using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class TrialManager : MonoBehaviour
{

    [Header("UI Settings")]
    [SerializeField] private float offsetUIVotes;
    [SerializeField] private TextMeshProUGUI blankVotesText;

    [Header("Components")]
    [SerializeField] private GameObject fingerPrefab;

    [Space(10)]
    [SerializeField] private GameLoopManager gameLoopManager;

    private void Start()
    {
        //Setup event listeners
        gameLoopManager.AddEventListener((JSONNode GD) => new KeyValuePair<JSONNode, string>(GD["stepSpecification"], "Conclusion"), OnConclusion);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        gameLoopManager.UpdateGameInfo(Update);

        //TO-DO

        //SOME ANIMATIONS

        //Next Step
        StartCoroutine(ServerInfo.NextStep("Trial", "Intro"));
    }

    // Update is called once per frame
    void Update()
    {
        if (gameLoopManager.gameData == null)
            return;

        if (gameLoopManager.gameData["stepSpecification"] == "InProgress")
        {
            Dictionary<string, int> voteCounterDict = new Dictionary<string, int>();
            foreach (KeyValuePair<string, JSONNode> vote in gameLoopManager.gameData["votes"])
            {
                foreach (JSONNode player in vote.Value)
                    voteCounterDict[player.Value] = voteCounterDict.ContainsKey(player.Value) ? voteCounterDict[player.Value] + 1 : 1;
            }

            foreach (KeyValuePair<string, int> vote in voteCounterDict)
            {
                if (vote.Key != "-1")
                {
                    Transform parent = gameLoopManager.getPlayerSpawn[vote.Key].trialVotesParent;

                    foreach (Transform child in parent)
                        Destroy(child.gameObject);

                    for (int i = 0; i < vote.Value; i++)
                    {
                        GameObject gb = Instantiate(fingerPrefab, parent);
                        gb.transform.position += new Vector3(offsetUIVotes * i, 0, 0);

                    }
                }
                else
                    blankVotesText.text = "Blank votes : " + vote.Value;
            }
        }
    }

    private void OnConclusion()
    {
        if (gameObject.activeInHierarchy)
        {
            JSONNode voteResults = gameLoopManager.gameData["results"];

            //DISPLAY RESULTS

            StartCoroutine(ServerInfo.NextStep("Trial", "Conclusion"));
        }
    }
}
