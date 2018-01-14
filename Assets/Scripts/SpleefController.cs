using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileSize
{
    Five,
    Ten,
    TwentyFive
}

public class SpleefController : MonoBehaviour {

    public GameObject platformPrefab;
    Vector3 spawnLocation;

    public TileSize tileSize;

    [HideInInspector]
    public bool gameStarted;

    int iExtents;
    int iModifier;

    private CommonGCMethods commonMethods;
    private SpawnPlayerScript playerManager;

    // Use this for initialization
	void Start ()
    {
        // Finding the script that manages the players and the script with common methods
        commonMethods = GetComponent<CommonGCMethods>();
        playerManager = GetComponent<SpawnPlayerScript>();

        // Telling the common methods script the kind of game mode
        commonMethods.gameMode = StaticInfo.GameModes.Spleef;

        //instantiating all the planes
        if (tileSize == TileSize.Five)
        {
            platformPrefab.transform.localScale = new Vector3(5, 5, 5);
            iExtents = 50;
            iModifier = 5;
        }
        else if (tileSize == TileSize.Ten)
        {
            platformPrefab.transform.localScale = new Vector3(10, 5, 10);
            iExtents = 25;
            iModifier = 10;
        }
        else
        {
            platformPrefab.transform.localScale = new Vector3(25, 5, 25);
            iExtents = 10;
            iModifier = 25;
        }

        SpawnPlatforms();

        // Spawning the players and starting the game
        commonMethods.SpawnPlayers();
        commonMethods.StartCoroutine(commonMethods.StartGameTimer());
	}

    void SpawnPlatforms()
    {
        for (int i = -iExtents; i < iExtents; i++)
        {
            for (int j = -iExtents; j < iExtents; j++)
            {
                //instantiating a new platform at the specified location
                //this is going to create a grid
                spawnLocation = new Vector3(i * iModifier, 100, j * iModifier);
                GameObject tempPlatform = Instantiate(platformPrefab, spawnLocation, Quaternion.identity);

                //setting the platforms parent to the the gamecontroller for later purposes
                tempPlatform.transform.parent = transform;
            }
        }
    }

    // Creating a function that lets the blocks fall
    public void StartFalling()
    {
        Invoke("ActivateFall", 1f);
    }

    void ActivateFall()
    {
        // Accessing all of the players
        for(int i = 0; i < StaticInfo.NumberofPlayers; i++)
        {
            playerManager.players[i].GetComponent<Fall>().active = true;
        }

    }
}


