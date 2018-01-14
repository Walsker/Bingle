using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject[] spawnLocations;
    public Camera cameraPrefab;

    [HideInInspector]
    public GameObject[] players;
    [HideInInspector]
    public Camera[] playerCameras = new Camera[4];

    void Awake()
    {
        players = new GameObject[4];
    }

    // Creating a function that spawns a player
    public void SpawnPlayer(int playerNumber, int totalPlayers)
    {
        // Creating a player
        players[playerNumber] = (GameObject)Instantiate(playerPrefab);
        players[playerNumber].GetComponent<VehicleController>().playerID = playerNumber + 1;

        // Placing the player in the right position
        players[playerNumber].transform.position = spawnLocations[playerNumber].transform.position;
        players[playerNumber].transform.rotation = spawnLocations[playerNumber].transform.rotation;

        // Creating a camera to follow the player
        playerCameras[playerNumber] = (Camera)Instantiate(cameraPrefab);
        playerCameras[playerNumber].GetComponent<CameraFollow>().target = players[playerNumber];
        playerCameras[playerNumber].GetComponent<CameraFollow>().cameraID = playerNumber + 1;

        // Telling the camera how to divide the screen for splitscreen gameplay depending on the number of players
        if (totalPlayers == 1)
        {
            playerCameras[playerNumber].GetComponent<CameraFollow>().type = ScreenTypes.Single;
        }
        else if (totalPlayers == 2)
        {
            playerCameras[playerNumber].GetComponent<CameraFollow>().type = ScreenTypes.Double;
        }
        else if (totalPlayers == 3 || totalPlayers == 4)
        {
            playerCameras[playerNumber].GetComponent<CameraFollow>().type = ScreenTypes.Quad;

            if (totalPlayers == 3)
            {
                // Creating a camera for the fourth slot that's black
                playerCameras[playerNumber + 1] = (Camera)Instantiate(cameraPrefab);

                GameObject fourthTarget = new GameObject();
                fourthTarget.transform.position = new Vector3(0, -1000f);
                playerCameras[playerNumber + 1].GetComponent<CameraFollow>().target = fourthTarget;
                playerCameras[playerNumber + 1].GetComponent<CameraFollow>().cameraID = 4;
                playerCameras[playerNumber + 1].GetComponent<CameraFollow>().type = ScreenTypes.Quad;
            }
        }
    }

    // Creating a pair of functions that enables and disables all the players from driving
    public void EnableAllPlayers()
    {
        for(int p = 0; p < StaticInfo.NumberofPlayers; p++)
        {
            // Enabling the player's driving functions
            players[p].GetComponent<VehicleController>().Enable();
        }
    }
    public void DisableAllPlayers()
    {
        for(int p = 0; p < StaticInfo.NumberofPlayers; p++)
        {
            // Disabling the player's driving function
            players[p].GetComponent<VehicleController>().Disable();
        }
    }
}
