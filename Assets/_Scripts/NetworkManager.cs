using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    private const string typeName = "UniqueGameName";
    private const string gameName = "RoomName";
    
    private void StartServer()
    {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
        Debug.Log("started server");
    }

    
    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
    }

    public GameObject playerPrefab;

    void OnJoinedRoom()
    {
        // Spawn player
        //PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        Debug.Log("onjoinedroom");
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    private const string roomName = "RoomName";
    private RoomInfo[] roomsList;

    void OnGUI()
    {
        //Debug.Log("cmon");
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null)
        {
            // Create Room
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                PhotonNetwork.CreateRoom(roomName + Guid.NewGuid().ToString("N"));

            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
            {
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }
    }

    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }


    /*public const string version = "1.0";
    // Use this for initialization
    [SerializeField] TextAlignment connectionText;
    [SerializeField] Camera sceneCamera;

    GameObject player;

	void Start () {
        PhotonNetwork.ConnectUsingSettings(version);

	}
	
	// Update is called once per frame
	void Update () {
        connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();

    }

    void OnJoinedLobby()
    {
        RoomOptions ro = new RoomOptions() { isVisible = true, maxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom("Mike", ro, TypedLobby.Default);
    }
    */
}
