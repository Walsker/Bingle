using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    public GameObject blocks;
    public GameObject carToThrow;
    public GameObject eventSystem;
    public Canvas Title;
    public Canvas About;

    private Vector3 throwCarFrom = new Vector3(100, 1, 0);

	// Use this for initialization
	void Start ()
    {
        // Checking if the title screen has been showed before
        if (StaticInfo.GameStarted)
        {
            blocks.SetActive(false);
            ShowTitle();
        }
        else
        {
            // Waiting 3 seconds before throwing a car across the screen
            Title.enabled = false;
            About.enabled = false;
            eventSystem.SetActive(false);
            blocks.SetActive(true);
            Invoke("ThrowCar", 3.5f);
            Invoke("ShowTitle", 7f);
            StaticInfo.GameStarted = true;
        }
	}

    void ThrowCar()
    {
        GameObject tempCar = Instantiate(carToThrow, throwCarFrom, Quaternion.identity);
        tempCar.transform.rotation = Quaternion.Euler(0, -90, 0);
        tempCar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        tempCar.GetComponent<Rigidbody>().velocity = new Vector3(-100, 0, 0);
    }

    void ShowTitle()
    {
        Title.enabled = true;
        About.enabled = false;
        eventSystem.SetActive(true);
        GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().Play();
    }

    public void Playgame()
    {
        // Going to game mode selection
        SceneManager.LoadScene("GameSelect");
    }

    public void AboutScreen()
    {
        Title.enabled = false;
        About.enabled = true;
    }

    public void TitleScreen()
    {
        Title.enabled = true;
        About.enabled = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
