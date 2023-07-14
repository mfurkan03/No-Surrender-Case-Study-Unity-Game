using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathFloor : MonoBehaviour
{
    private int playerCount = 4;
    private void OnTriggerEnter(Collider other)
    {
        bool player = other.TryGetComponent<PlayerScript>(out PlayerScript playerScript);
        other.TryGetComponent<Navmesh>(out Navmesh script);
        if (player)
        {
            playerScript.Dying();
        }
        else
        {
            script.Dying();
        }
        playerCount -= 1;
        if (playerCount <= 1) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            print("game ended, restarting...");
        }
    }
}
