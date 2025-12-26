using UnityEngine;
using NativeWebSocket;
using System;

[Serializable]
public class Landmark
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Hand
{
    public string handedness;
    public Landmark[] landmarks;
}

[Serializable]
public class HandData
{
    public Hand[] hands;
}

public class HandDataReceiver : MonoBehaviour
{
    private WebSocket websocket;
    public HandData currentHandData;
    public bool isConnected = false;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8765");

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket bağlantısı başarılı!");
            isConnected = true;
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket hatası: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket bağlantısı kapandı");
            isConnected = false;
        };

        websocket.OnMessage += (bytes) =>
        {
            try
            {
                string message = System.Text.Encoding.UTF8.GetString(bytes);
                currentHandData = JsonUtility.FromJson<HandData>(message);
                
                if (currentHandData != null && currentHandData.hands != null && currentHandData.hands.Length > 0)
                {
                    Debug.Log($"El sayısı: {currentHandData.hands.Length}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON parse hatası: {ex.Message}");
            }
        };

        await websocket.Connect();
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            if (websocket != null)
            {
                websocket.DispatchMessageQueue();
            }
        #endif
    }

    async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}