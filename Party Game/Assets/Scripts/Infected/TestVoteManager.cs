using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.Networking;
using SimpleJSON;

public class TestVoteManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image imageTest1;
    [SerializeField] private Image imageTest2;
    [SerializeField] private TextMeshProUGUI voteCounter1;
    [SerializeField] private TextMeshProUGUI voteCounter2;

    [Space(10)]
    [SerializeField] private GameLoopManager gameLoopManager;

    // Start is called before the first frame update
    private void OnEnable()
    {
        //Setup event listeners
        gameLoopManager.AddEventListener((JSONNode GD) => new KeyValuePair<JSONNode, string>(GD["stepSpecification"], "Conclusion"), OnConclusion);


        //When enabled, get the images and tests put to vote and display them
        //TO-DO

        //SOME ANIMATIONS

        //Next Step
        StartCoroutine(ServerInfo.NextStep("TestVote", "Intro"));
    }

    // Update is called once per frame
    void Update()
    {
        if (gameLoopManager.gameData == null)
            return;

        if (gameLoopManager.gameData["stepSpecification"] == "InProgress")
        {
            Dictionary<int, int> voteCounterDict = new Dictionary<int, int>();
            foreach (JSONNode vote in gameLoopManager.gameData["votes"])
                voteCounterDict[vote.AsInt] = voteCounterDict.ContainsKey(vote.AsInt) ? voteCounterDict[vote.AsInt] + 1 : 1;

            if (voteCounterDict.ContainsKey(0))
                voteCounter1.text = voteCounterDict[0].ToString();
            if (voteCounterDict.ContainsKey(1))
                voteCounter2.text = voteCounterDict[1].ToString();
        }
    }

    void OnConclusion()
    {
        JSONNode voteResults = gameLoopManager.gameData["results"];

        //DISPLAY RESULTS

        //Next Step for protype purposes
        StartCoroutine(ServerInfo.NextStep("TestVote", "InProgress"));
    }

}
