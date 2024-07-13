using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private List<Transform> listOfSnekss;
    private List<NewAISnek> newAISnekScripts;
    private List<TMP_Text> textList = new();
    List<GameObject> AIManager;
    private int[] snakeObjects;

    // Start is called before the first frame update
    void Start()
    {
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

        //foreach (var AI in AIManager) {
        //    Debug.Log(AI.GetComponent<NewAISnek>().GetMass());
        //}

        snakeObjects = new int[AIManager.Count];

    }

    /*private GameObject[] snakeObjects;*/// = new GameObject[AIManager.Count];

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < snakeObjects.Length; i++) {
            snakeObjects[i] = AIManager[i].GetComponent<NewAISnek>().GetMass();
        }

        Array.Sort(snakeObjects); // sorts list in accending order

        Array.Reverse(snakeObjects); // reverses the sort so its in decending order

        //Debug.Log(snakeObjects[1]);

        for (int i = 0; i < textList.Count; i++) {
            textList[i].SetText(snakeObjects[i].ToString());

        }
    }
}
