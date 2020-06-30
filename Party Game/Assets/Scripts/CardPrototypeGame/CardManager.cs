using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using SimpleJSON;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    [SerializeField] int getFrequency = 50;
    [SerializeField] GameObject cardGB;
    [SerializeField] Transform cardSpawn;
    [SerializeField] Transform cardGoal;

    private List<string> playedCards = new List<string>();

    private int frame = 0;
    // Update is called once per frame
    void Update()
    {
        frame++;
        if (frame >= getFrequency)
        {
            frame = 0;
            StartCoroutine(GetCards());
        }
    }

    private IEnumerator GetCards()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(ServerInfo.RoomURL + "/gameData"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
                Debug.Log("Error: " + webRequest.error);
            else
            {
                JSONNode result = JSON.Parse(webRequest.downloadHandler.text);
                JSONArray cardHistory = result["cardHistory"].AsArray;

                for (int i = cardHistory.Count - playedCards.Count; i > 0; i--)
                {
                    playedCards.Add(cardHistory[cardHistory.Count - i]);

                    GameObject newCard = Instantiate(cardGB, cardSpawn);
                    newCard.transform.DOMove(cardGoal.position, .5f);
                }
            }
        }
    }
}
