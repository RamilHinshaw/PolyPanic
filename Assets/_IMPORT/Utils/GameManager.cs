using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

//GameManger is a singleton Monobehavior
public class GameManager : MonoBehaviour
{

    #region Instance

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                print("Instance of GameObject does not exist!");

            return instance;
        }
    }

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    #endregion

    public bool overidePlayerCount;
    public int playersPlaying = 2;

    //MANAGERS HERE 
    public GUIManager guiManager;

    public CameraController[] cameraControllers;

    public AudioSource audioSource;
    public Dictionary<int, PlayerController> playerID = new Dictionary<int, PlayerController>(); //FOR CACHING!
    public List<PlayerController> players;

    public EnemySpawner[] enemySpawners;
    public PlayerSpawnPoint[] playerSpawnPoint;

    public List<EnemyController> enemiePrefabs = new List<EnemyController>();
    public List<PlayerController> playerPrefabs = new List<PlayerController>();

    public int waveRound = 0;

    private bool beginWave = false;
    public float spawnStarterTimer = 2.25f;
    public float spawnTimer;

    private float gameLength;
    public float difficultyModifier;


    private void Start()
    {
        if (overidePlayerCount)
            GameSettings.playersPlaying = playersPlaying;

        enemySpawners = GameObject.FindObjectsOfType<EnemySpawner>();
        playerSpawnPoint = GameObject.FindObjectsOfType<PlayerSpawnPoint>();
        SpawnPlayers();

        Camera[] cams = new Camera[cameraControllers.Length];

        for (int i = 0; i < cams.Length; i++)
        {
            cams[i] = cameraControllers[i].cam;
        }

        SetPlayerCameras(cams, GameSettings.playersPlaying);

        SpawnNextWave();

        beginWave = true;

        spawnTimer = spawnStarterTimer;
    }

    public void SetPlayerCameras(Camera[] cam, int playerCount)
    {
        //Set Camera Viewport Partition
        if (playerCount == 2)
        {
            cam[0].rect = new Rect(0, 0.5f, 1, 0.5f);
            cam[1].rect = new Rect(0, 0, 1, 0.5f);
        }

        else if (playerCount == 3 || playerCount == 4)
        {
            cam[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            cam[1].rect = new Rect(0.5f, 0.5f, 1, 0.5f);
            cam[2].rect = new Rect(0, 0, 0.5f, 0.5f);

            if (GameManager.Instance.playersPlaying == 4)
                cam[3].rect = new Rect(0.5f, 0, 1, 0.5f);
        }
    }

    public void CheckIfAllDead()
    {
        //If all players dead go to main menu
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].health > 0)
                return;
        }

        //You Lose!
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        guiManager.UpdatePlayerGUI();

        if (beginWave == true)
        {
            gameLength += Time.deltaTime;
            difficultyModifier = gameLength / 90f; //Double every 2 minutes if 120

            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                SpawnEnemy();

                spawnStarterTimer = spawnStarterTimer - (spawnStarterTimer * 0.00625f);

                if (spawnStarterTimer < 0.75f)
                    spawnStarterTimer = 0.75f;

                spawnTimer = spawnStarterTimer;
            }
        }
    }

    public Vector3 GetPlayerSpawnPoint(int playerID)
    {
        //int spawnPoint = UnityEngine.Random.Range(0, enemySpawners.Length);
        //return enemySpawners[spawnPoint].transform.position;
        return playerSpawnPoint[playerID].transform.position;
    }

    private void SpawnPlayers()
    {


        for (int i = 0; i < GameSettings.playersPlaying; i++)
        {
            //int spawnPoint = UnityEngine.Random.Range(0, enemySpawners.Length);
            //Debug.Log(GameSettings.playersCharacter[i]);

            int rdmCharacter = UnityEngine.Random.Range(0, playerPrefabs.Count);

            PlayerController player = Instantiate(playerPrefabs[rdmCharacter],
                                            playerSpawnPoint[i].transform.position,
                                            playerSpawnPoint[i].transform.rotation);

            //ASSIGN CORRECT CONTROLS TO PLAYER
            player.horizontalInput += (i+1).ToString();
            player.verticalInput += (i + 1).ToString();
            player.attackInput += (i + 1).ToString();
            player.sprintButton += (i + 1).ToString();
            player.switchWeaponInput += (i + 1).ToString();
            player.reloadInput += (i + 1).ToString();
            player.rotateXInput += (i + 1).ToString();
            player.rotateZInput += (i + 1).ToString();

            player.playerID = i;

            playerID.Add(player.gameObject.GetInstanceID(), player);
            players.Add(player);

            cameraControllers[i].gameObject.SetActive(true);
            cameraControllers[i].target = player.gameObject;            
        }

        //Then add to database

        //GameObject[] playerObj = GameObject.FindGameObjectsWithTag("Player");

        //for (int i = 0; i < playerObj.Length; i++)
        //{
        //    var playerScript = playerObj[i].GetComponent<PlayerController>();

        //    playerID.Add(playerObj[i].GetInstanceID(), playerScript);
        //    players.Add(playerScript);
        //}
    }

    private void SpawnEnemy()
    {
        Debug.Log("SPAWNED ENEMY!");

        int enemyType = UnityEngine.Random.Range(0, enemiePrefabs.Count);
        int spawnPoint = UnityEngine.Random.Range(0, enemySpawners.Length);

        EnemyController enemy = Instantiate(enemiePrefabs[enemyType],
                                            enemySpawners[spawnPoint].transform.position,
                                            enemySpawners[spawnPoint].transform.rotation);

        float newSpeed = enemy.movementSpeed + (enemy.movementSpeed * difficultyModifier);
        enemy.SetSpeed(newSpeed);
    }

    public void SpawnNextWave()
    {
        
        waveRound++;
    }

    public void SetDeath(int playerID, Action OnRespawn)
    {
        //int index = players.IndexOf(player);
        int index = playerID;
        guiManager.SetDeath(index, OnRespawn);
    }

    public PlayerController GetPlayer(int id)
    {
        return playerID[id];
    }

 

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayAudio(AudioClip clip, float volume)
    {
        //float prevVol = audioSource.volume;
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
        //audioSource.volume = prevVol;
    }




}



