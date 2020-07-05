using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class ServerInfo
{
    //public static string ServerURL = "http://127.0.0.1:8000/SERVER";
    public static string ServerURL = "https://party-game-mobile.herokuapp.com/SERVER";
    public static string RoomURL;
    public static bool hasRoom = false;

    public static string CDN_URL = "https://cdn.jsdelivr.net/gh/FireGreeks/party-game";

    public static IEnumerator NextStep(string step, string spec)
    {
        var webRequest = new UnityWebRequest(RoomURL + "/CallGameFunction");
        webRequest.method = "POST";

        var data = "{";
        data += "\"functionName\" : \"NextStep\",";
        data += "\"arguments\" : [\"" + step + "\", \"" + spec + "\", true]";
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
