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

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {  
        Vector3 syncPosition = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = GetComponent<Rigidbody>().position;
            stream.Serialize(ref syncPosition);
        }
        else
        {
            stream.Serialize(ref syncPosition);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = GetComponent<Rigidbody>().position;
            syncEndPosition = syncPosition;
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        SpawnPlayer();
    }

    private HostData[] hostList;

    private void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    private void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
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
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                RefreshHostList();

            if (hostList != null)
            {
                for (int i = 0; i < hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                        JoinServer(hostList[i]);
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
