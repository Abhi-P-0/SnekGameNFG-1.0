using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameObject snakeObject;

    [SerializeField] private List<GameObject> listOfAIs;

    [SerializeField] private int numAI;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numAI; i++) {
            GameObject temp = Instantiate(snakeObject);

            temp.transform.GetChild(0).GetComponent<LongThinLine>().enabled = false;

            listOfAIs.Add(temp);

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (GameObject snek in listOfAIs) {
            if (!snek.activeInHierarchy) {
                //Vector3 spawnPoint = new Vector3(Random.Range(-50, 50), Random.Range(5, 50), Random.Range(-50, 50));

                //snek.GetComponent<NewAISnek>().SetHeadPosition(spawnPoint);

                snek.SetActive(true);

            }
        }
    }

    private void EnableSnek() {

    }
}
