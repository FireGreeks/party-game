using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using SimpleJSON;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    [SerializeField] int getFrequency = 50;
    [SerializeField] float offsetYPerCard = .2f;
    [SerializeField] GameObject cardGB;
    [SerializeField] Transform cardSpawn;
    [SerializeField] Transform cardGoal;

    private List<string> playedCards = new List<string>();
    private float currentYOffset = 0;

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

                    string path = "file://" + Application.dataPath + "/Mobile Client/Images/Cards/PNG/" + cardHistory[cardHistory.Count - i] + ".png";
                    Debug.Log(path);
                    UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
                    yield return www.SendWebRequest();

                    if (www.isNetworkError)
                        Debug.Log("Error: " + webRequest.error);
                    else
                    {
                        Texture2D txt = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        newCard.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(txt, new Rect(0, 0, txt.width, txt.height), new Vector2(0, 0), 100.0f);

                        newCard.transform.DOMove(cardGoal.position + new Vector3(0, currentYOffset, 0), .5f);
                        currentYOffset += offsetYPerCard;
                    }
                }
            }
        }
    }
}
