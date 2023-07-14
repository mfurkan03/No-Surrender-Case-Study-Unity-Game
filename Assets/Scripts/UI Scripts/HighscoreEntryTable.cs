using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreEntryTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform;
        entryTemplate = transform.Find("HighscoreEntryTemplate");
        entryTemplate.gameObject.SetActive(false);
        
    }


    /// <summary>
    /// sorts the playerdata and overwrites the updated information
    /// </summary>
    /// <param name="playerData"></param>
    public void UpdateTable(Hashtable playerData)
    {
        float templateHeight = 40f;
        int i= 0;

        List<DictionaryEntry> entries = new List<DictionaryEntry>();
        foreach (DictionaryEntry entry in playerData)
        {
            entries.Add(entry);
        }

        // Step 2: Sort the list based on keys
        entries.Sort((x, y) => ((IComparable)x.Value).CompareTo(y.Value));
        Debug.Log(entries.Count);
        for(i = 0; i < entryContainer.childCount; i++)
        {
            Transform child = entryContainer.GetChild(i);
            if (child != entryTemplate)
            {
                Destroy(child.gameObject);
            }
        }
        for(i=0; i<entries.Count; i++)
        {
        
            Transform entryTransform = Instantiate(entryTemplate,entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -40 - templateHeight * i);
            entryTransform.gameObject.SetActive(true);
            entryTransform.SetParent(entryContainer);

            int rank = i + 1;
            string rankString;
            switch (rank)
            {
                default:
                    rankString = rank + "TH"; break;
                case 1:
                    rankString = rank + "ST";
                    break;
                case 2:
                    rankString = rank + "ND";
                    break;
                case 3:
                    rankString = rank + "RD";

                    break;

            }
            entryTransform.Find("PositionText").GetComponent<Text>().text = rankString;
            int score = (int)entries[i].Value;
            entryTransform.Find("ScoreText").GetComponent<Text>().text = score.ToString();
            string name = ((Transform)entries[i].Key).name;
            entryTransform.Find("NameText").GetComponent<Text>().text = name;
        }
    }


}
