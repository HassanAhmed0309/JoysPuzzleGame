using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menu = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;
    
    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;


    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();


    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        //Stops people from joining if the game is in progress
        if(SceneManager.GetActiveScene().name != "Lobby")
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            //Binds the player to the server
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);

            NotifyPlayersofReadyState();
        }    
        base.OnServerDisconnect(conn);
    }

    public void NotifyPlayersofReadyState()
    {
        foreach(var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
       if(numPlayers < minPlayers)
       {
           return false;
       }

       foreach(var player in RoomPlayers)
       {
            if(!player.IsReady)
            {
                return false;
            }
       }
       
       return true;
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void StartGame()
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("Scene_Map_01");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().name == "Lobby" && newSceneName.StartsWith("Scene_Map"))
        {
            for(int i = RoomPlayers.Count -1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName.StartsWith("Scene_Map"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }
}
