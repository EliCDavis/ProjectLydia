using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_script : MonoBehaviour {

    GameObject[] pauseObjects;
    GameObject[] clickObjects;
    GameObject[] clicknotObjects;
    GameObject[] howtoplay;
    GameObject[] hownottoplay;
	GameObject[] credits;
    GameObject[] deathObjects;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        hidePaused();
        howtoplay = GameObject.FindGameObjectsWithTag("howtoplay");
        hownottoplay = GameObject.FindGameObjectsWithTag("hownottoplay");
        hidehowtoplay();
		credits = GameObject.FindGameObjectsWithTag("credits");
		hideCredits ();
        deathObjects = GameObject.FindGameObjectsWithTag("death");
        hideDeath();
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

    public void showhowtoplay()
    {
        foreach (GameObject g in howtoplay)
        {
            g.SetActive(true);
        }
        foreach (GameObject g in hownottoplay)
        {
            g.SetActive(false);
        }
    }
    
    public void hidehowtoplay()
    {
        foreach (GameObject g in howtoplay)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in hownottoplay)
        {
            g.SetActive(true);
        }
 
    }
	public void hideCredits()
	{
		foreach (GameObject g in credits) {
			g.SetActive (false);
		}
	}

	public void showCredits()
	{
		Debug.Log ("Toggling....");

		if (credits.Length == 0) {
			return;
		}
		if (credits [0].activeSelf) {
			foreach (GameObject g in credits) {
				g.SetActive (false);
			}
		} else {
			foreach (GameObject g in credits) {
				g.SetActive (true);
			}
		}


	}
    
    public void showDeath()
    {
        foreach (GameObject g in deathObjects)
        {
            g.SetActive(true);
        }
    }

    public void hideDeath()
    {
        foreach (GameObject g in deathObjects)
        {
            g.SetActive(false);
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
    public void LoadLevel(string levelToLoad)
    {
		SceneManager.LoadScene(levelToLoad);
    }

    public void quit()
    {
        Application.Quit();
    }
}

