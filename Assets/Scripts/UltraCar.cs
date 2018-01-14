using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltraCar : MonoBehaviour
{
    public float ultraCarMagnitude;
    public bool debugMode;

    private SpawnPlayerScript playerManager;
    private CommonGCMethods commonMethods;
    private int ultraCarID;
    private GameObject ultracar;
    private float endOfRound;

    void Start()
    {
        // Finding the script that manages the players and the script with common methods
        playerManager = GetComponent<SpawnPlayerScript>();
        commonMethods = GetComponent<CommonGCMethods>();

        // Telling the common methods script the kind of game mode
        commonMethods.gameMode = StaticInfo.GameModes.Ultracar;

        // Telling the game which player is the ultracar
        ultraCarID = StaticInfo.UltracarRounds + 1;

        // Spawning the players
        commonMethods.SpawnPlayers();

        // Checking if it's debug mode
        if (!debugMode)
        {
            CreateUltracar();
        }

        // Starting the game
        commonMethods.StartCoroutine(commonMethods.StartGameTimer());
    }

    // Creating a function that enlarges one of the cars
    void CreateUltracar()
    {
        // Turning the chosen car into the ultracar
        ultracar = playerManager.players[ultraCarID - 1];
        ultracar.GetComponent<VehicleController>().isUltraCar = true;
        playerManager.playerCameras[ultraCarID - 1].GetComponent<CameraFollow>().cameraHeight += 6;
        ultracar.GetComponent<VehicleController>().UltraMagnify(ultraCarMagnitude);
    }
}
