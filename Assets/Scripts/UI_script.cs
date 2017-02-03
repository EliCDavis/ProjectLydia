using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_script : MonoBehaviour {

    GameObject[] pauseObjects;
    GameObject[] clickObjects;
    GameObject[] clicknotObjects;
  

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePaused();
        clickObjects = GameObject.FindGameObjectsWithTag("ShowOnClick");
        clicknotObjects = GameObject.FindGameObjectsWithTag("HideOnClick");
        hideClicked();
    }

    // Update is called once per frame
    void Update()
    {

        //pauses the game using timeScale + button input
        //in this case: P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                showPaused();
            }
            else if (Time.timeScale == 0)
            {
                Debug.Log("high");
                Time.timeScale = 1;
                hidePaused();
            }
        }
       

        
    }

    

    //Restarts level
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
    }

    //Controls the pausing
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }



    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }
    
    public void showClicked()
    {
        foreach (GameObject g in clickObjects)
        {
            g.SetActive(true);
        }
        foreach (GameObject g in clicknotObjects)
        {
            g.SetActive(false);
        }
    }
    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void hideClicked()
    {
        foreach (GameObject g in clickObjects)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in clicknotObjects)
        {
            g.SetActive(true);
        }
    }
    public void Credits(string Credits)
    {
        SceneManager.LoadScene(Credits);
    }

    

    //exits to Main Menu
    public void LoadLevel(string MainMenu)
    {
        //Application.Quit();
        // SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene(MainMenu);
    }

    public void quit()
    {
        Application.Quit();
    }
}

