using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;


    public void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;

    }
    public void Awake()
    {
        pausePanel.SetActive(false );
        Time.timeScale = 1;

    }


    public void _Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        Debug.Log("continue");
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("The game is restarting...");
    }
    
}
