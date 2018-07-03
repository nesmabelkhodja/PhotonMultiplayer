﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Photon.PunBehaviour
{
    #region Public Variables
    ///<summary>
    ///The PuN log
    /// </summary>
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>   
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    public byte MaxPlayersPerRoom = 4;
    #endregion

    #region Private Variables
    /// <summary>
    /// This client's version number. Users are separated from each other by game version (whicha llows you to make breaking changes)
    /// </summary>

    string _gameVersion = "1";
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;
    #endregion
    #region MonoBehaviour CallBacks
    /// <summary>
    /// MonoBehaviour method called on GAmeObject by Unity during early initialiation phase.
    /// </summary>
    private void Awake()
    {
        // #Critical
        //we don;t join the lobby. there is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients i nthe same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        PhotonNetwork.logLevel = Loglevel;
    }
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        //Connect();
        //Debug.Log("yay");
    }
    #endregion

    #region Public Methods
    ///<summary>
    ///Start the connection process.
    ///= If already connected, we attempt joining a random room
    ///- if not connected, connect this app isntance to Photon Cloud Network
    ///
    ///</summary>

    public void Connect()
    {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = true;
        //Debug.Log("yay");


        // we check fi we are aconnected or not, we join if we are, esle we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            //#Critical we need at thos point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() 
            //and we'll create a room
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //#Critical we must first and foremost connect to Photon Online Server
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }
    #endregion

    #region Photon.PunBehaviour CallBacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("DemoAnimator/Launcher: OnConnectedtoMaster() was called by PUN");

        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromphoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() ws called by PUN. no random room available, so we create one. \nCalling:" +
            "Photon.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);

    }

    public override void OnJoinedRoom()
    {
        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");


            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }
    #endregion
}


