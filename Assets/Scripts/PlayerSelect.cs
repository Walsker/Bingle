using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelect : MonoBehaviour
{
    [Range(0,4)] public int playerCount;
    public GameObject[] displayPlayers;
    public GameObject[] keys;
    public GameObject addPlayerTexts;
    public Text[] readyTexts;
    public Camera sceneCamera;
    public float playerZ;
    public float playerYOffset;
    public float keyYOffset;

    private float yPositionTop;
    private float yPositionBot;
    private float xPositionLeft;
    private float xPositionRight;

    private int previousPlayerCount;
    private bool[] isPlayerReady = new bool[4];
    private bool allPlayersReady;

    void Start()
    {
        yPositionTop = (sceneCamera.pixelHeight / 4) * 3;
        yPositionBot = sceneCamera.pixelHeight / 4;
        xPositionLeft = sceneCamera.pixelWidth / 4;
        xPositionRight = (sceneCamera.pixelWidth / 4) * 3;
    }

    void FixedUpdate()
    {
        // Spinning the cars on their local y axes
        foreach(GameObject player in displayPlayers)
        {
            player.transform.Rotate(Vector3.up, Space.Self);
        }
    }

    void Update()
    {
        // Adjusting all the text based on the screen's size
        displayPlayers[0].transform.position = sceneCamera.ScreenToWorldPoint(new Vector3(xPositionLeft, yPositionTop + playerYOffset, playerZ));
        displayPlayers[1].transform.position = sceneCamera.ScreenToWorldPoint(new Vector3(xPositionRight, yPositionTop + playerYOffset, playerZ));
        displayPlayers[2].transform.position = sceneCamera.ScreenToWorldPoint(new Vector3(xPositionLeft, yPositionBot + playerYOffset, playerZ));
        displayPlayers[3].transform.position = sceneCamera.ScreenToWorldPoint(new Vector3(xPositionRight, yPositionBot + playerYOffset, playerZ));

        keys[0].transform.position = new Vector3(xPositionLeft, yPositionTop + keyYOffset, playerZ);
        keys[1].transform.position = new Vector3(xPositionRight, yPositionTop + keyYOffset, playerZ);
        keys[2].transform.position = new Vector3(xPositionLeft, yPositionBot + keyYOffset, playerZ);
        keys[3].transform.position = new Vector3(xPositionRight, yPositionBot + keyYOffset, playerZ);

        // Showing a text that tells the user how to add players
        if (playerCount == 0)
        {
            // Placing the text in the top left slot
            addPlayerTexts.SetActive(true);
            addPlayerTexts.transform.position = new Vector3(xPositionLeft, yPositionTop, playerZ);
        }
        else if (playerCount == 1)
        {
            // Placing the text in the top right slot
            addPlayerTexts.transform.position = new Vector3(xPositionRight, yPositionTop, playerZ);
        }
        else if (playerCount == 2)
        {
            // Placing the text in the bottom left slot
            addPlayerTexts.transform.position = new Vector3(xPositionLeft, yPositionBot, playerZ);
        }
        else if (playerCount == 3)
        {
            // Placing the text in the bottom right slot
            addPlayerTexts.transform.position = new Vector3(xPositionRight, yPositionBot, playerZ);
        }
        else if (playerCount == 4)
        {
            // Hiding the text
            addPlayerTexts.SetActive(false);
        }

        // Checking if the space bar was pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerCount == 4)
            {
                playerCount = 0;
            }
            else
            {
                playerCount++;

                // Showing keyboard controls
                foreach(Transform child in keys[playerCount - 1].transform)
                {
                    if (child.name == "Keyboard")
                    {
                        child.gameObject.SetActive(true);
                    }
                    if (child.name == "Controller")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            print(playerCount);
        }

        // Checking if the playercount has changed
        if (previousPlayerCount != playerCount)
        {
            // Looping through all the instances of the car
            for (int i = 0; i < displayPlayers.Length; i++)
            {
                // Checking if the car we're dealing with is supposed to be used
                if (i < playerCount)
                {
                    displayPlayers[i].SetActive(true);
                    keys[i].SetActive(true);
                }
                else
                {
                    displayPlayers[i].SetActive(false);
                    keys[i].SetActive(false);
                }
            }
            previousPlayerCount = playerCount;
        }

        // Checking if the ability keys are pressed
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            // Checking if the player is ready or not
            if (!isPlayerReady[0])
            {
                readyTexts[0].text = "Ready!";
                isPlayerReady[0] = true;
            }
            else
            {
                readyTexts[0].text = "Press your ability key to ready up";
                isPlayerReady[0] = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Checking if the player is ready or not
            if (!isPlayerReady[1])
            {
                readyTexts[1].text = "Ready!";
                isPlayerReady[1] = true;
            }
            else
            {
                readyTexts[1].text = "Press your ability key to ready up";
                isPlayerReady[1] = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.U) && playerCount > 2)
        {
            // Checking if the player is ready or not
            if (!isPlayerReady[2])
            {
                readyTexts[2].text = "Ready!";
                isPlayerReady[2] = true;
            }
            else
            {
                readyTexts[2].text = "Press your ability key to ready up";
                isPlayerReady[2] = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && playerCount > 3)
        {
            // Checking if the player is ready or not
            if (!isPlayerReady[3])
            {
                readyTexts[3].text = "Ready!";
                isPlayerReady[3] = true;
            }
            else
            {
                readyTexts[3].text = "Press your ability key to ready up";
                isPlayerReady[3] = false;
            }
        }

        // Checking if all the players are ready
        for (int i = 0; i < playerCount; i++)
        {
            if (isPlayerReady[i])
            {
                allPlayersReady = true;
            }
            else
            {
                allPlayersReady = false;
                break;
            }
        }

        // Moving to the loading scene if all the players are ready
        if (allPlayersReady && playerCount > 1)
        {
            StaticInfo.NumberofPlayers = playerCount;
            Destroy(GameObject.FindGameObjectWithTag("Music"));
            SceneManager.LoadScene("LoadingScreen");
        }
    }
}
