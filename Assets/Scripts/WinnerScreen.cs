using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerScreen : MonoBehaviour
{
    public GameObject[] playerColors;
    public List<GameObject> displayPlayers;
    public GameObject[] walls;

    private int winnerID;

    void Start()
    {
        // Changing the physics time back to normal
        Time.timeScale = 1f;

        // Getting the wall colors
        foreach(GameObject wall in walls)
        {
            wall.GetComponent<Renderer>().material.color = StaticInfo.LevelColors[(int)StaticInfo.PreviousGameMode];
        }

        // Finding out the winner
        winnerID = StaticInfo.WinnerID;

        // Finding out how many players were in the last game
        if (StaticInfo.NumberofPlayers == 2)
        {
            // Getting rid of the extra players
            Destroy(displayPlayers[3]); displayPlayers.RemoveAt(3);
            Destroy(displayPlayers[1]); displayPlayers.RemoveAt(1);
        }
        else if (StaticInfo.NumberofPlayers == 3)
        {
            Destroy(displayPlayers[2]); displayPlayers.RemoveAt(2);
        }

        // Checking with the game finished with one winner
        if (winnerID == -1)
        {
            // Moving all the display players to the podium because they're all kinda winners
            displayPlayers[0].GetComponent<VehicleController>().playerID = 1;
            for (int i = 1; i < displayPlayers.Count; i++)
            {
                displayPlayers[i].transform.position = new Vector3(0f, 10f + (5 * i), 0f);
                displayPlayers[i].GetComponent<VehicleController>().playerID = i + 1;
            }
        }
        // There's only one winner
        else if (winnerID >= 1 && winnerID <= 4)
        {
            // Assigning the winner to the car on the podium
            displayPlayers[0].GetComponent<VehicleController>().playerID = winnerID;

            // Finding out which players aren't the winners 
            for (int i = 1; i < StaticInfo.NumberofPlayers + 1; i++)
            {
                if (i != winnerID)
                {
                    displayPlayers[1].GetComponent<VehicleController>().playerID = i;
                    break;
                }
            }
            if (StaticInfo.NumberofPlayers >= 3)
            {
                for (int i = 1; i < StaticInfo.NumberofPlayers + 1; i++)
                {
                    if (i != winnerID && i != displayPlayers[1].GetComponent<VehicleController>().playerID)
                    {
                        print(i);
                        displayPlayers[2].GetComponent<VehicleController>().playerID = i;
                        break;
                    }
                }
            }
            if (StaticInfo.NumberofPlayers == 4)
            {
                for (int i = 1; i < StaticInfo.NumberofPlayers + 1; i++)
                {
                    if (i != winnerID && i != displayPlayers[1].GetComponent<VehicleController>().playerID && i != displayPlayers[2].GetComponent<VehicleController>().playerID)
                    {
                        print(i);
                        displayPlayers[3].GetComponent<VehicleController>().playerID = i;
                        break;
                    }
                }
            }
        }
        // There's two winners
        else if (winnerID > 4)
        {
            string winner1S = winnerID.ToString()[0].ToString();
            string winner2S = winnerID.ToString()[1].ToString();

            int winner1;
            int winner2;
            int.TryParse(winner1S, out winner1);
            int.TryParse(winner2S, out winner2);

            // Moving a second player onto the podium
            displayPlayers[2].transform.position = new Vector3(0f, 15f, 0);

            // Giving out playerIDs to the winners
            displayPlayers[0].GetComponent<VehicleController>().playerID = winner1;
            displayPlayers[2].GetComponent<VehicleController>().playerID = winner2;

            // Giving out the rest
            for (int i = 1; i < StaticInfo.NumberofPlayers + 1; i++)
            {
                // Checking which players didn't win
                if (i != winner1 && i != winner2)
                {
                    displayPlayers[1].GetComponent<VehicleController>().playerID = i;
                    break;
                }

            }
            if (StaticInfo.NumberofPlayers == 4)
            {
                for (int i = 1; i < StaticInfo.NumberofPlayers + 1; i++)
                {
                    if (i != winner1 & i != winner2 && i != displayPlayers[1].GetComponent<VehicleController>().playerID)
                    {
                        displayPlayers[3].GetComponent<VehicleController>().playerID = i;
                        break;
                    }
                }
            }
        }
    }

    public void GoToStart()
    {
        Destroy(GameObject.FindGameObjectWithTag("Music"));
        SceneManager.LoadScene("StartMenu");  
    }

    public void PlayAgain()
    {
        Destroy(GameObject.FindGameObjectWithTag("Music"));
        SceneManager.LoadScene("LoadingScreen");  
    }
}
