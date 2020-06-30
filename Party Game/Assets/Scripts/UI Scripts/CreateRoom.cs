using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

using SimpleJSON;

public class CreateRoom : MonoBehaviour
{
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


    [SerializeField] private int frameCycle = 20;
    [SerializeField] private TextMeshProUGUI playerList;

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
                JSONNode result = JSON.Parse(webRequest.downloadHandler.text);
                JSONNode players = result["Players"];

                playerList.text = "";
                foreach (JSONNode player in players)
                {
                    //Display room changes
                    playerList.text += "- " + player["name"].ToString() + (player["isPartyLeader"] ? "    VIP" : "") + "\n" ;

                }
                    
            }
        }
    }


    //The following code is only used as a protoype
    public void SelectGame()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        var webRequest = new UnityWebRequest("https://party-game-mobile.herokuapp.com/SERVER/6S6L/SelectGame");
        webRequest.method = "POST";
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes("{\"gameID\":1}"));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
            Debug.Log("Error: " + webRequest.error);
        else
        {
            Debug.Log("Game Started");
        }
        
    }
}
