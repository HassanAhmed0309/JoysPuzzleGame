using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region Maps, Ground, LevelObjects, Spawn Points and Baked NavMesh Prefabs
    public List<GameObject> Maps = new List<GameObject>(3);
    public List<GameObject> Grounds = new List<GameObject>(3);
    public List<GameObject> LevelObjects = new List<GameObject>(3);
    public List<GameObject> BakedNavMeshes = new List<GameObject>(3);
    public GameObject[] SpawnPoints = new GameObject[3];
    #endregion

    #region Maps, Ground, LevelObjects and Baked NavMesh Clones
    public List<GameObject> ClonedMaps;
    public List<GameObject> ClonedGrounds;
    public List<GameObject> ClonedLevelObjects;
    public List<GameObject> ClonedBakedNavMeshes;
    public List<GameObject> ParentGameObjects;
    public List<GameObject> ClonedSpawnPoints;
    #endregion

    #region Level to turn on (Lvl "1", "2", "3", etc)
    public int LevelToSetActive;
    #endregion

    #region Network Manager Reference
    [SerializeField]
    GameObject NetworkManager;
    #endregion

    #region Input Manager Reference
    [SerializeField]
    GameObject InputManager;
    #endregion

    #region UI Manager Reference
    [SerializeField]
    GameObject UIManager;
    #endregion

    #region Essential Scene Objects
    [SerializeField]
    GameObject MainCamera;
    [SerializeField]
    GameObject DirectionalLight;
    #endregion

    #region OnScreen Debug Console Prefab
    [SerializeField]
    GameObject ScreenConsole;
    #endregion

    [SyncVar]public bool Spawned = false;

    NetworkManagerGame network;

    public CinemachineFreeLook localCamera;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        GameObject clonedPrefab = null;

        //Cloning OnScreen Console
        clonedPrefab = Instantiate(ScreenConsole);
        clonedPrefab.name = ScreenConsole.name;

        //Turning on the UIManager to spawn UI
        clonedPrefab = Instantiate(UIManager);
        clonedPrefab.name = UIManager.name;
        clonedPrefab.SetActive(true);

        //Turning on the NetworkManager to spawn network objects
        clonedPrefab = Instantiate(NetworkManager);
        clonedPrefab.name = NetworkManager.name;
        clonedPrefab.SetActive(true);
        network = clonedPrefab.GetComponent<NetworkManagerGame>();

        //Turning on the InputManager to work on Inputs
        clonedPrefab = Instantiate(InputManager);
        clonedPrefab.name = InputManager.name;

        //Cloning Maps, Grounds, LevelObjects, Navmesh Surfaces and saving them
        for (int i = 0; i < Maps.Count; i++)
        {
            GameObject game = new GameObject("Level " + i + 1);
            ParentGameObjects.Add(game);

            ClonedMaps.Add(Instantiate(Maps[i], game.transform));
            ClonedMaps[i].name = Maps[i].name;

            ClonedGrounds.Add(Instantiate(Grounds[i], game.transform));
            ClonedGrounds[i].name = Grounds[i].name;

            //ClonedLevelObjects.Add(Instantiate(LevelObjects[i], game.transform));
            //ClonedLevelObjects[i].name = LevelObjects[i].name;

            ClonedBakedNavMeshes.Add(Instantiate(BakedNavMeshes[i], game.transform));
            ClonedBakedNavMeshes[i].name = BakedNavMeshes[i].name;

            //Cloning Spawn points
            //ClonedSpawnPoints.Add(Instantiate(SpawnPoints[i], game.transform));
            //ClonedSpawnPoints[i].name = SpawnPoints[i].name;

            if (i == LevelToSetActive - 1)
            {
                ClonedMaps[i].SetActive(true);
                ClonedGrounds[i].SetActive(true);
                //ClonedLevelObjects[i].SetActive(true);
                ClonedBakedNavMeshes[i].SetActive(true);
                //ClonedSpawnPoints[i].SetActive(true);
            }
            else
            {
                ClonedMaps[i].SetActive(false);
                ClonedGrounds[i].SetActive(false);
                //ClonedLevelObjects[i].SetActive(false);
                ClonedBakedNavMeshes[i].SetActive(false);
                //ClonedSpawnPoints[i].SetActive(false);
            }
        }

        //Cloning Main Camera
        clonedPrefab = Instantiate(MainCamera);
        clonedPrefab.name = MainCamera.name;

        //Cloning Directional Light
        clonedPrefab = Instantiate(DirectionalLight);
        clonedPrefab.name = DirectionalLight.name;
    }

    #endregion

    [Server]
    public void SpawnObjects()
    {
        GameObject reference;
        foreach (GameObject prefab in network.spawnPrefabs)
        {
            reference = Instantiate(prefab, GameManager.Instance.ParentGameObjects[0].transform);
            reference.name = prefab.name;
            reference.SetActive(true);
            NetworkServer.Spawn(reference);
        }
    }

    #region Level Handling Functions
    //Input the level number (not the array number) e.g., Level "1", "2", "3", etc
    void ChangeLevel(int CurrLevel)
    {
        //Turning off Current Map & Ground
        ClonedMaps[CurrLevel - 1].SetActive(false);
        ClonedGrounds[CurrLevel - 1].SetActive(false);

        if(CurrLevel == Maps.Count)
        {
            CurrLevel = -1;
        }
        
        ClonedMaps[CurrLevel + 1].SetActive(true);
        ClonedGrounds[CurrLevel + 1].SetActive(true);
    }
    #endregion
}
