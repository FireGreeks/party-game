using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MiniGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(NextStep());
    }

    IEnumerator NextStep()
    {
        //For prototype purposes, go to next major step, Trial
        var webRequest = new UnityWebRequest(ServerInfo.RoomURL + "/AlterGameData");
        webRequest.method = "POST";

        var data = "{";
        data += "\"currentCycleStep\" : [\"SET\", \"Trial\"]";
        data += "}";

        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(data));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
            Debug.Log("Error: " + webRequest.error);
    }
}
