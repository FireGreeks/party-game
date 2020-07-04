using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

using SimpleJSON;
using UnityEngine.SceneManagement;


public class CreateRoom : MonoBehaviour
{

    [Header("Networking Settings")]
    [SerializeField] private int frameCycle = 20;
    [SerializeField] private int sceneIndexDelay = 0;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI playerList;
    [SerializeField] private TextMeshProUGUI roomCode;


    private string codePossibilities = "ABCDEFGHIJKLMNOPQRSTUWXYZ0123456789";
    private string currentCode = "";

    public void CreateRoomButton()
    {
        StartCoroutine(GetUniqueCode());
    }

    private IEnumerator GetUniqueCode()
    {
        Debug.Log("Getting Unique Code...");

        string uri = ServerInfo.ServerURL + "/GetRooms";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
                Debug.Log("Error: " + webRequest.error);
            else
            {
                currentCode = "";
                for (int i = 0; i < 4; i++)
                {
                    currentCode += codePossibilities[Random.Range(0, codePossibilities.Length - 1)];
                }

                bool flag = false;
                JSONNode result = JSON.Parse(webRequest.downloadHandler.text);
                foreach (JSONNode code in result.AsArray)
                {
                    if (currentCode == code.ToString())
                        flag = true;
                }

                if (flag)
                    GetUniqueCode();
                else
                    StartCoroutine(CreateRoomPOST());

            }
        }
    }


    private IEnumerator CreateRoomPOST()
    {

        Debug.Log("The code is : " + currentCode);
        Debug.Log("Creating Room");

        roomCode.text = "Room Code: " + currentCode;

        string uri = ServerInfo.ServerURL + "/CreateRoom";
        string data = "{\"id\": \"" + currentCode + "\"}";

        var webRequest = new UnityWebRequest(uri);
        webRequest.method = "POST";
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(data));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
            Debug.Log("Error: " + webRequest.error);
        else
        {
            ServerInfo.RoomURL = ServerInfo.ServerURL + "/" + currentCode;
            ServerInfo.hasRoom = true;
        }
    }

    private int frame = 0;
    private void Update()
    {
        frame++;
        if (frame >= frameCycle && ServerInfo.hasRoom)
        {
            frame = 0;
            StartCoroutine(GetRoomUpdate());
        }
    }

    private IEnumerator GetRoomUpdate()
    {
        Debug.Log("Getting and Displaying Room updates...");
        Debug.Log("The room URL is now : " + ServerInfo.RoomURL);

        //Get Update on room
        using (UnityWebRequest webRequest = UnityWebRequest.Get(ServerInfo.RoomURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
                Debug.Log("Error: " + webRequest.error);
            else
            {
                //Display connected players
                JSONNode result = JSON.Parse(webRequest.downloadHandler.text);
                JSONNode players = result["Players"];

                playerList.text = "";
                foreach (JSONNode player in players)
                {
                    //Display room changes
                    playerList.text += "- " + player["name"].ToString() + (player["isPartyLeader"] ? "    VIP" : "") + "\n" ;

                }


                //Check if a game has been selected
                int currentGameID = result["currentGameID"].AsInt;
                if (currentGameID != 0)
                    SceneManager.LoadScene(currentGameID + sceneIndexDelay); //If game isn't lobby, load game
                    
            }
        }
    }
}
