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

                    UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://github.com/FireGreeks/party-game/raw/master/Party%20Game/Assets/Mobile%20Client/Images/Cards/PNG/" + cardHistory[cardHistory.Count - i] + ".png");
                    yield return www.SendWebRequest();

                    if (www.isNetworkError)
                        Debug.Log("Error: " + webRequest.error);
                    else
                    {
                        Texture2D txt = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        newCard.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(txt, new Rect(0, 0, txt.width, txt.height), new Vector2(0, 0), 100.0f);

                        newCard.transform.DOMove(cardGoal.position, .5f);
                    }
                }
            }
        }
    }
}
