using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    public string nextSceneName;
    public float minimumLoadTime;
    public Image blackTexture;
    public float fadeDuration;

    private AsyncOperation nextLevel;
    private float waitTime;
    private bool fadedOnce = false;
    private bool readyToPlay = false;

	void Start ()
    {
        // Finding out which scene is supposed to be loaded
        if (StaticInfo.NextGameMode == StaticInfo.GameModes.KingOfTheHill)
        {
            nextSceneName = "King";
        }
        else if (StaticInfo.NextGameMode == StaticInfo.GameModes.Spleef)
        {
            nextSceneName = "Spleef";
        }
        else if (StaticInfo.NextGameMode == StaticInfo.GameModes.Ultracar)
        {
            nextSceneName = "Ultracar";
        }

        // Fading in the scene
        StartCoroutine(FadeScene(StaticInfo.FadeType.In));

        // Loading the next scene
        nextLevel = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        nextLevel.allowSceneActivation = false;
        waitTime = Time.time + minimumLoadTime;
	}
	
	void Update ()
    {
        if (nextLevel.progress == 0.9f && Time.time > waitTime)
        {
            // Fading out the scene
            StartCoroutine(FadeScene(StaticInfo.FadeType.Out));
            fadedOnce = true;

            // Checking if the fading is done
            if (readyToPlay)
            {
                nextLevel.allowSceneActivation = true;
            }
        }
	}

    IEnumerator FadeScene(StaticInfo.FadeType fadeType)
    {
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

                // Checking if the texture is completely gone
                if (blackTexture.color.a <= 0)
                {
                    blackTexture.color = new Color(0, 0, 0, 0);
                    break;
                }
            }

        }
        else if (fadeType == StaticInfo.FadeType.Out)
        {
            if (!fadedOnce)
            {
                // Making the texture transparent
                blackTexture.color = new Color(0, 0, 0, 0);
                
                // Fading out the scene
                for (int f = 0; f < 100; f++)
                {
                    blackTexture.color = new Color(0, 0, 0, blackTexture.color.a + (1 / (fadeDuration * 60f)));
                    yield return new WaitForSeconds(fadeDuration / (fadeDuration * 60f));
                
                    // Checking if the texture is completely black
                    if (blackTexture.color.a >= 1f)
                    {
                        blackTexture.color = Color.black;
                        readyToPlay = true;
                        break;
                    }
                }
            }
        }
    }
}
