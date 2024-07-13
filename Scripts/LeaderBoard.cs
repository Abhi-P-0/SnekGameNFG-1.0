using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private List<Transform> listOfSnekss;
    private List<NewAISnek> newAISnekScripts;
    private List<TMP_Text> textList = new();

    List<GameObject> AIManager;

    private int[] snakeMass;
    private string[] snakeNames;

    //private List<TMP_Text> textList = new List<TMP_Text>();
    //private List<GameObject> AIManager;
    //private List<SnakeData> snakeDataList = new List<SnakeData>();

    //public struct SnakeData {
    //    public int Mass { get; private set; }
    //    public string Name { get; private set; }

    //    public SnakeData(int mass, string name) {
    //        Mass = mass;
    //        Name = name;
    //    }
    //}

    //void Start() {
    //    for (int i = 7; i < 17; i++) {
    //        textList.Add(GameObject.Find("Canvas").transform.GetChild(i).GetComponent<TMP_Text>());
    //    }
    //    AIManager = GetComponent<AIManager>().GetSnakes();
    //}

    //void FixedUpdate() {
    //    UpdateSnakeData();
    //    SortSnakeData();
    //    UpdateLeaderboard();
    //}

    //void UpdateSnakeData() {
    //    snakeDataList.Clear();
    //    foreach (var snake in AIManager) {
    //        var aiSnek = snake.GetComponent<NewAISnek>();
    //        snakeDataList.Add(new SnakeData(aiSnek.GetMass(), aiSnek.GetSnakeName()));
    //    }
    //}

    //void SortSnakeData() {
    //    snakeDataList = snakeDataList.OrderByDescending(s => s.Mass).ToList();
    //}

    //void UpdateLeaderboard() {
    //    for (int i = 0; i < Mathf.Min(textList.Count, snakeDataList.Count); i++) {
    //        textList[i].SetText($"{snakeDataList[i].Name}: {snakeDataList[i].Mass}");
    //    }
    //}

    // Start is called before the first frame update
    void Start() {
        //var listOfSneks = GameObject.FindGameObjectsWithTag("AI");

        //Debug.Log(listOfSneks.Length);

        //List<int> uniqueAIs = new();

        //foreach (var sne in listOfSneks) {
        //    if (sne.transform.parent != null) {
        //        if (!uniqueAIs.Contains(sne.transform.parent.GetInstanceID())) {

        //            uniqueAIs.Add(sne.transform.parent.GetInstanceID());

        //            listOfSnekss.Add(sne.transform.parent);

        //            //newAISnekScripts.Add(sne.transform.parent.gameObject.GetComponent<NewAISnek>());

        //        }

        //    }

        //}

        //Debug.Log(listOfSnekss);

        ////foreach (var sneScript in newAISnekScripts) {
        ////    Debug.Log(sneScript);
        ////}

        for (int i = 7; i < 17; i++) {
            textList.Add(GameObject.Find("Canvas").transform.GetChild(i).GetComponent<TMP_Text>());

        }

        AIManager = GetComponent<AIManager>().GetSnakes();

        snakeMass = new int[AIManager.Count];
        snakeNames = new string[AIManager.Count];

    }

    void FixedUpdate() {
        for (int i = 0; i < snakeMass.Length; i++) {
            snakeMass[i] = AIManager[i].GetComponent<NewAISnek>().GetMass();
            //snakeNames[i] = AIManager[i].GetComponent<NewAISnek>().GetSnakeName();
        }

        Array.Sort(snakeMass); // sorts list in accending order
        // cant sort strings

        Array.Reverse(snakeMass); // reverses the sort so its in decending order

        for (int i = 0; i < textList.Count; i++) {
            textList[i].SetText(snakeMass[i].ToString()); // only posting mass right now, cant sort names, maybe use list

        }
    }
}
