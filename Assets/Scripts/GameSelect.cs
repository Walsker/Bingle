using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSelect : MonoBehaviour
{
    public string[] modeNames;
    public string[] modeDescriptions;
    public Button UltracarButton;
    public Button KingButton;
    public Button SpleefButton;
    public Text modeName;
    public Text modeDescription;

    public void ChangeText(int modeIndex)
    {
        modeName.text = modeNames[modeIndex];
        modeDescription.text = modeDescriptions[modeIndex];
    }

    public void LoadUltracar()
    {
        StaticInfo.NextGameMode = StaticInfo.GameModes.Ultracar;
        SceneManager.LoadScene("PlayerSelect");
    }

    public void LoadKingOfTheHill()
    {
        StaticInfo.NextGameMode = StaticInfo.GameModes.KingOfTheHill;
        SceneManager.LoadScene("PlayerSelect");
    }

    public void LoadSpleef()
    {
        StaticInfo.NextGameMode = StaticInfo.GameModes.Spleef;
        SceneManager.LoadScene("PlayerSelect");
    }

    public void BackToStart()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
