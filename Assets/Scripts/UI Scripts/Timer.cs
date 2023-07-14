using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float timeDuration = 60f;
    private float timer;
    void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer);
        }
        else
        {
            Flash();
        }
    }

    private void ResetTimer()
    {
        timer = timeDuration;
    }

    private void UpdateTimerDisplay(float time)
    {
        float seconds = Mathf.FloorToInt(time);
        string currentTime = seconds.ToString();
        Text timerText = transform.Find("TimerText").GetComponent<Text>();
        timerText.text = currentTime;
    }

    private void Flash()
    {
        //restart the game when the timer ends
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
