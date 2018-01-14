using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticInfo 
{
    private static Color[] playerColors = new Color[] {new Color(143f/255f, 18f/255f, 18f/255f), new Color(0f/255f, 61f/255f, 200f/255f), new Color(191f/255f, 157f/255f, 0f/255f), new Color(6f/255f, 69f/255f, 6f/255f)};
    private static Color[] levelColors = new Color[] {new Color(141f/255f, 9f/255f, 9f/255f), new Color(15f/255f, 135f/255f, 100f/255f), new Color(82f/255f, 65f/255f, 176f/255f)};
    private static int winnerID;
    private static int ultracarRounds = 0;
    private static float[] remainingTimes = new float[4];
    private static int[] playerPoints = new int[4];
    private static GameModes previousGameMode;
    private static GameModes nextGameMode;
    private static int numberOfPlayers;
    private static bool levelIsReady;
    private static bool gameStarted = false;

    public static Color[] PlayerColors
    {
        get{ return playerColors; }
        set{ playerColors = value; }
    }
    public static Color[] LevelColors
    {
        get{ return levelColors; }
        set{ levelColors = value; }
    }

    public static int WinnerID
    {
        get{ return winnerID; }
        set{ winnerID = value; }
    }

    public static int UltracarRounds
    {
        get{ return ultracarRounds; }
        set{ ultracarRounds = value; }
    }

    public static float[] RemainingTimes
    {
        get{ return remainingTimes; }
        set{ remainingTimes = value; }
    }

    public static int[] PlayerPoints
    {
        get{ return playerPoints; }
        set{ playerPoints = value; }
    }

    public static GameModes PreviousGameMode
    {
        get{ return previousGameMode; }
        set{ previousGameMode = value; }
    }

    public static GameModes NextGameMode
    {
        get{ return nextGameMode; }
        set{ nextGameMode = value; }
    }

    public static int NumberofPlayers
    {
        get{ return numberOfPlayers; }
        set{ numberOfPlayers = value; }
    }

    public static bool LevelIsReady
    {
        get{ return levelIsReady; }
        set{ levelIsReady = value; }
    }

    public static bool GameStarted
    {
        get{ return gameStarted; }
        set{ gameStarted = value; }
    }

    public enum GameModes
    {
        KingOfTheHill,
        Spleef,
        Ultracar
    }
    public enum FadeType
    {
        In,
        Out
    }
}
