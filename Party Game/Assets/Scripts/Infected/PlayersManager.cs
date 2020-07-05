using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SimpleJSON;
using TMPro;

public class PlayersManager : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    public Transform trialVotesParent;


    private GameLoopManager gameLoopManager;

    [NonSerialized] public string playerID;

    // Start is called before the first frame update
    void Start()
    {
        gameLoopManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<GameLoopManager>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score:" + gameLoopManager.PlayersDict[playerID]["score"];

        //Effects (Immunity, Suspected, etc.)
        //TO-DO
    }
}
