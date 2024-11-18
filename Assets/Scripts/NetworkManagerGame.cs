using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class NetworkManagerGame : NetworkManager
{
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        Debug.Log("Starting client...");
        List<GameObject> spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        Debug.Log("Spawnable Prefab count: " + spawnablePrefabs.Count());

        foreach (GameObject prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
            Debug.Log("Registering prefab: " + prefab);
        }
        //NetworkServer.connections.Count;
    }

    public override void OnClientConnect()
    {
        if (!GameManager.Instance.Spawned)
        {
            GameManager.Instance.Spawned = true;
            GameManager.Instance.SpawnObjects();
        }
    }
}
