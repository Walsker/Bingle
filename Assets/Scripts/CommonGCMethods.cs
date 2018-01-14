using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CommonGCMethods : MonoBehaviour
{
    public float roundDuration;
    public float preGameTime;
    public Text preGameCount;
    public Text gameTimer;
    public Image blackTexture;
    public float fadeDuration;
    public GameObject ingameMusic;

    [HideInInspector]
    public int bingleCount = 0;
    [HideInInspector]
    public StaticInfo.GameModes gameMode;
    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public bool fadingDone;

    private float endOfRound;
    private SpawnPlayerScript playerManager;
    private int numberOfPlayers;

    void Awake()
    {
        // Increasing the round duration by 1 to compensate for the time "GO!" is on screen
        roundDuration++;

        // Finding the script that manages the players
        playerManager = GetComponent<SpawnPlayerScript>();
        numberOfPlayers = StaticInfo.NumberofPlayers;
    }

    public void SpawnPlayers()
    {
        // Spawning the number of players dictated by numberOfPlayers
        for (int p = 0; p < numberOfPlayers; p++)
        {
            playerManager.SpawnPlayer(p, numberOfPlayers);
        }
    }

    // A Coroutine for fading in the scene
    IEnumerator FadeScene(StaticInfo.FadeType fadeType)
    {
        // Telling the game that the fading is still not done
        fadingDone = false;

        // Checking if the game wants to fade in or out
        if (fadeType == StaticInfo.FadeType.In)
        {
            // Setting it's color to black
            blackTexture.color = Color.black;

            // Fading in the scene
            for (int f = 0; f < 100; f++)
            {
                blackTexture.color = new Color(0, 0, 0, blackTexture.color.a - (1 / (fadeDuration * 60f)));
                yield return new WaitForSeconds(fadeDuration / (fadeDuration * 60f));

                // Checking if the 
                // Checking if the texture is completely gone
                if (blackTexture.color.a <= 0)
                {
                    blackTexture.color = new Color(0, 0, 0, 0);
                    fadingDone = true;
                    break;
                }
            }

        }
        else if (fadeType == StaticInfo.FadeType.Out)
        {
            // Making the texture transparent
            blackTexture.color = new Color(0, 0, 0, 0);

            // Fading out the scene
            for (int f = 0; f < 100; f++)
            {
                blackTexture.color = new Color(0, 0, 0, blackTexture.color.a + (1 / (fadeDuration * 60f)));
                yield return new WaitForSeconds(fadeDuration / (fadeDuration * 60f));

                // Checking if the 
                // Checking if the texture is completely black
                if (blackTexture.color.a >= 1f)
                {
                    blackTexture.color = new Color(0, 0, 0, 0);
                    fadingDone = true;
                    break;
                }
            }
        }
    }

    public IEnumerator StartGameTimer()
    {
        // Fading in the scene
        preGameCount.text = "";
        //StartCoroutine(FadeScene(StaticInfo.FadeType.In));

        // Waiting until the fade is done
        //yield return new WaitUntil(() => fadingDone);
        yield return new WaitForSeconds(2f);
        blackTexture.color = new Color(0, 0, 0, 0);

        // Playing the ingame music
        Instantiate(ingameMusic);

        // Checking if the game is in debug mode
        if (numberOfPlayers != 1)
        {
            // Hiding the clock
            gameTimer.text = "";
            
            // Counting down until the start of the game
            for (int i = 0; i < preGameTime; i++)
            {
                preGameCount.text = ((int)preGameTime - i).ToString();
                yield return new WaitForSeconds(1f);
            }
            
            if (gameMode == StaticInfo.GameModes.Ultracar)
            {
                // Finding out when the end of the game is and when it starts
                endOfRound = Time.time + roundDuration;
            }
            
            // Displaying "GO!" and enabling all the players to move
            preGameCount.text = "GO!";
            playerManager.EnableAllPlayers();

            // Checking if the game mode is King of the Hill
            if (gameMode == StaticInfo.GameModes.KingOfTheHill)
            {
                // Start shrinking the floor
                GetComponent<KingController>().StartShrinking();

                // Adding a height death to all the players
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    playerManager.players[i].AddComponent<HeightDeath>();
                }
            }

            // Checking if the game mode is Spleef
            if (gameMode == StaticInfo.GameModes.Spleef)
            {
                // Adding a height death and a fall script to all the players
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    playerManager.players[i].AddComponent<HeightDeath>();
                    playerManager.players[i].AddComponent<Fall>();
                }

                // Allowing the blocks to fall
                GetComponent<SpleefController>().StartFalling();
            }

            // Checking if the game mode is ULTRAcar
            if (gameMode == StaticInfo.GameModes.Ultracar)
            {
                // Starting a countdown timer that makes the game expire
                StartCoroutine(Countdown());
            }
            else
            {
                // Waiting until there is only one car standing
                StartCoroutine(WaitForWinner());
            }
        }
        else
        {
            playerManager.EnableAllPlayers();
        }
    }

    // Creating a function that counts the timer down (Only for the utlracar game mode)
    IEnumerator Countdown()
    {
        string minute;
        string second;

        for (int e = 0; e < roundDuration; e++)
        {
            // Checking if one second has passed
            if (e >= 1)
            {
                // Hiding the big "GO!"
                preGameCount.text = "";

                // Updating the clock
                minute = (Mathf.Floor((endOfRound - Time.time) / 60)).ToString();
                second = ((int)(endOfRound - Time.time) % 60).ToString();

                if (second.Length == 1)
                {
                    second = "0" + second;
                }

                gameTimer.text = minute  + ":" + second;
            }

            // Checking if the round is over
            if (bingleCount == numberOfPlayers - 1)
            {
                // Saving the time remaining for the ultracar to get everyone
                StaticInfo.RemainingTimes[StaticInfo.UltracarRounds] = endOfRound - Time.time;

                // Slowing down the game
                Time.timeScale = 0.5f;
                playerManager.DisableAllPlayers();

                yield return new WaitForSecondsRealtime(2f);

                // Checking if there are more rounds
                if (StaticInfo.UltracarRounds < numberOfPlayers - 1)
                {
                    StaticInfo.UltracarRounds++;
                    EndRound();
                }
                else
                {
                    // Ending the game
                    StaticInfo.UltracarRounds = 0;
                    EndGame(ReturnWinnerID());
                }
                break;
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }

        // Ending the round due to time
        if (Time.time > endOfRound)
        {
            // Saving the time remaining for the ultra to get everyone (should be negative)
            StaticInfo.RemainingTimes[StaticInfo.UltracarRounds] = endOfRound - Time.time;
            
            // Slowing down the game
            Time.timeScale = 0.5f;
            playerManager.DisableAllPlayers();
            
            yield return new WaitForSecondsRealtime(2f);
            
            // Checking if there are more rounds
            if (StaticInfo.UltracarRounds < numberOfPlayers - 1)
            {
                StaticInfo.UltracarRounds++;
                EndRound();
            }
            else
            {
                // Ending the game
                EndGame(ReturnWinnerID());
            }
        }
    }

    // Creating a function that waits until the game is over
    IEnumerator WaitForWinner()
    {
        // Waiting one second before hiding the big "GO!"
        yield return new WaitForSeconds(1f);
        preGameCount.text = "";

        // Waiting until all but one player is left standing
        yield return new WaitUntil(() => bingleCount >= numberOfPlayers - 1);

        // Slowing down the game
        Time.timeScale = 0.5f;
        playerManager.DisableAllPlayers();

        int winnerID = ReturnWinnerID();

        yield return new WaitForSecondsRealtime(2f);

        // Ending the game and sending the number of the winning player
        EndGame(winnerID);
    }

    // Creating a function that looks for the player with the highest score and returns their ID
    int ReturnWinnerID()
    {
        int winnerPlayerID = -1;

        if (gameMode == StaticInfo.GameModes.Ultracar)
        {
            // Finding out who has the highest score
            for (int i = 1; i <= numberOfPlayers - 1; i++)
            {
                //print("Player " + i + "'s points are " + StaticInfo.PlayerPoints[i - 1] + " and Player " + (i + 1) + "'s points are " + StaticInfo.PlayerPoints[i]);
                //print("Player " + i + "'s remaining time is " + StaticInfo.RemainingTimes[i - 1] + " and Player " + (i + 1) + "'s remaining time is " + StaticInfo.RemainingTimes[i]);
            
                // Checking who's points are higher
                if (StaticInfo.PlayerPoints[i - 1] < StaticInfo.PlayerPoints[i])
                {
                    winnerPlayerID = i + 1;
                }
                // Checking if they have the same amount of points
                else if (StaticInfo.PlayerPoints[i - 1] == StaticInfo.PlayerPoints[i])
                {
                    // Checking who took less time
                    if (StaticInfo.RemainingTimes[i - 1] > StaticInfo.RemainingTimes[i])
                    {
                        winnerPlayerID = i;
                    }
                    else
                    {
                        winnerPlayerID = i + 1;
                    }
                }
                // It's somehow in some way a tie which is basically impossible
                else
                {
                    // Putting the two IDs into one number so they can be split and retrieved later (Ex. winners are players 1 and 4, winnerID = 14)
                    string mergedId = i.ToString() + (i + 1).ToString();
                    if (!int.TryParse(mergedId, out winnerPlayerID))
                    {
                        // If it really didn't work after all this just go and declare everyone winner
                        winnerPlayerID = -1;
                    }
                }
            }
        }
        else
        {
            // Checking all the players
            for (int i = 0; i < numberOfPlayers; i++)
            {
                // Checking if the player is bingled
                if (!playerManager.players[i].GetComponent<VehicleController>().bingled)
                {
                    // Saving the ID of the player who isn't bingled
                    winnerPlayerID = playerManager.players[i].GetComponent<VehicleController>().playerID;
                    break;
                }
            }
        }

        return winnerPlayerID;
    }

    // Creating a function that moves the game to the next round
    void EndRound()
    {
        StaticInfo.NextGameMode = StaticInfo.GameModes.Ultracar;
        Destroy(GameObject.FindGameObjectWithTag("Music"));
        SceneManager.LoadScene("LoadingScreen");
    }

    // Creating a function that ends the game
    void EndGame(int winnerID)
    {
        // Saving the winning player for the next scene
        StaticInfo.WinnerID = winnerID;
        StaticInfo.PreviousGameMode = gameMode;
        StaticInfo.NumberofPlayers = numberOfPlayers;

        // Going to winner screen
        print("And the winner is player " + winnerID);
        SceneManager.LoadScene("WinnerScreen");
    }
}