using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupGenerator : MonoBehaviour
{
    public GameObject pickupPrefab;
    public int pickupCap;

    [HideInInspector]
    public int pickupCount;

    private float placeHeight;
    private float areaHalfSize;
    private cityGenerator cityGen;
    private int randomNumber;
    private List<GameObject> pickups = new List<GameObject>();

    void Start()
    {
        cityGen = GameObject.FindWithTag("GameController").GetComponent<cityGenerator>();
        placeHeight = cityGen.CityHeight;
        areaHalfSize = cityGen.CityHalfSize;
    }

    void Update()
    {
        // Checking if there are less than the maximum amount of pickups
        if (pickupCount < pickupCap)
        {
            // Choosing a random pickup
            randomNumber = Random.Range(0, 3);
            pickupCount++;
            CreatePickup((PickupType)randomNumber);

        }

        // Checking if the gamemode is king of the hill
        if (GetComponent<CommonGCMethods>().gameMode == StaticInfo.GameModes.KingOfTheHill)
        {
            //print((GetComponent<KingController>().newTransformScale.x - GetComponent<KingController>().endTableScale) / (GetComponent<KingController>().startTableScale - GetComponent<KingController>().endTableScale));
            pickupCap = (int)Mathf.Lerp(1, 10, (GetComponent<KingController>().newTransformScale.x - GetComponent<KingController>().endTableScale) / (GetComponent<KingController>().startTableScale - GetComponent<KingController>().endTableScale));

            // Checking if there are too many pickups
            if (pickupCount > pickupCap)
            {
                pickups.RemoveAt(0);
            }
        }
    }

    // Creating a function that cretes a pickup at a random location
    void CreatePickup(PickupType type)
    {
        Vector3 spawnLocation;
        // Checking if the game mode is king of the hill
        if (GetComponent<CommonGCMethods>().gameMode == StaticInfo.GameModes.KingOfTheHill)
        {
            spawnLocation = new Vector3(Random.Range(-GetComponent<KingController>().newTransformScale.x / 2, GetComponent<KingController>().newTransformScale.x / 2 + 1), placeHeight, Random.Range(-areaHalfSize, areaHalfSize + 1));
        }
        else
        {
            spawnLocation = new Vector3(Random.Range(-areaHalfSize, areaHalfSize + 1), placeHeight, Random.Range(-areaHalfSize, areaHalfSize + 1));
        }
        bool spawnOkay = false;
        //float safeDistance = areaHalfSize / pickupCap;
        float safeDistance = 1f;
        float afterGameStarted = Time.time + GameObject.FindWithTag("GameController").GetComponent<CommonGCMethods>().preGameTime;
       
        // Finding a suitable area to spawn the pickup
        while (!spawnOkay)
        {
            // Checking if the game has just started
            if (Time.timeSinceLevelLoad > afterGameStarted)
            {
                // Using the player as the basis for where the pickup can't spawn
                foreach (GameObject player in GetComponent<SpawnPlayerScript>().players)
                {
                    // Checking if the selected location is close to the player and if there is ground beneath where we want to spawn the pickup
                    if (Vector3.Distance(player.transform.position, spawnLocation) > safeDistance && Physics.Raycast(spawnLocation, Vector3.down, 1))
                    {
                        spawnOkay = true;
                    }
                    else
                    {
                        spawnOkay = false;
                        spawnLocation = new Vector3(Random.Range(-areaHalfSize, areaHalfSize + 1), placeHeight, Random.Range(-areaHalfSize, areaHalfSize + 1));
                        break;
                    }
                }
            }
            else
            {
                // Using the player spawn as the basis for where the pickup can't spawn
                foreach (GameObject playerSpawn in GetComponent<SpawnPlayerScript>().spawnLocations)
                {
                    // Checking if the selected location is close to the player and if there is ground beneath where we want to spawn the pickup
                    if (Vector3.Distance(playerSpawn.transform.position, spawnLocation) > safeDistance && Physics.Raycast(spawnLocation, Vector3.down, 1))
                    {
                        spawnOkay = true;
                    }
                    else
                    {
                        spawnOkay = false;
                        spawnLocation = new Vector3(Random.Range(-areaHalfSize, areaHalfSize + 1), placeHeight, Random.Range(-areaHalfSize, areaHalfSize + 1));
                        break;
                    }
                }
            }
        }

        GameObject pickup = (GameObject)Instantiate(pickupPrefab, spawnLocation, Quaternion.identity);
        pickup.GetComponent<PickupScript>().powerUp = type;
        pickups.Add(pickup);
    }
}
