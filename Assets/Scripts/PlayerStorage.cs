using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerStorage : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform playerTransform1;
    [SerializeField] Transform playerTransform2;
    [SerializeField] Transform playerTransform3;
    Transform[] transforms;
    Hashtable playerData;
    [SerializeField] Transform scoreboard;
    public void Start()
    {
        playerData = new Hashtable();
        transforms = new Transform[]{ playerTransform, playerTransform1, playerTransform2, playerTransform3 };
        foreach (Transform t in transforms)
        {
            playerData.Add(t,0);
        }
        bool success =scoreboard.TryGetComponent<HighscoreEntryTable>(out HighscoreEntryTable highscoreEntryTable);
        highscoreEntryTable.UpdateTable(playerData);
    }
    public Transform[] GetPlayersTransforms()
    {
        return transforms;
    }

    public void AddScore(Transform transform,int score)
    {
        playerData[transform] = (int)playerData[transform] + score;
        bool success = scoreboard.TryGetComponent<HighscoreEntryTable>(out HighscoreEntryTable highscoreEntryTable);
        highscoreEntryTable.UpdateTable(playerData);

    }

}
