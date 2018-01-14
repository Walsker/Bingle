using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingController : MonoBehaviour {

    public GameObject table;
    public float startTableScale;
    public float endTableScale;
    public float scaleDurationInSeconds;

    [HideInInspector]
    public Vector3 newTransformScale;

    private float reduceFactor;
    //private SpawnPlayerScript playerManager;
    private CommonGCMethods commonMethods;
    private bool gameStarted;

    // Use this for initialization
    void Start()
    {
        // Finding the script that manages the players and the script with common methods
        //playerManager = GetComponent<SpawnPlayerScript>();
        commonMethods = GetComponent<CommonGCMethods>();

        // Telling the common methods script the kind of game mode
        commonMethods.gameMode = StaticInfo.GameModes.KingOfTheHill;

        //setting the initial scale of the table
        table.transform.localScale = new Vector3(startTableScale, 1, startTableScale);

        //Get the current scale of the table
        newTransformScale = table.transform.localScale;

        //calculating the amount of reduction the scale needed to get the table scale from start to end in the given time
        reduceFactor = ((table.transform.localScale.x - endTableScale) / scaleDurationInSeconds) / 50;

        // Spawning the players and starting the game
        commonMethods.SpawnPlayers();
        commonMethods.StartCoroutine(commonMethods.StartGameTimer());
    }

    //Called 50 times a second
    void FixedUpdate()
    {
        //if the table is larger than the end scale
        if (newTransformScale.x > endTableScale && gameStarted)
        {
            //reduce the x and z scale of the table by subtracting the existing scale by the reduce factor
            newTransformScale = new Vector3(newTransformScale.x - reduceFactor, newTransformScale.y, newTransformScale.z - reduceFactor);
            table.transform.localScale = newTransformScale;
        }
        // Add a gameover function in an else statement if needed
    }

    // Creating a function that allows the shrinking to be started at a later time
    public void StartShrinking()
    {
        gameStarted = true;
    }
}
