using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private List<TMP_Text> textList = new();
    private AIManager manager;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.SetTweensCapacity(1000, 1000);

        playerPrefab = Instantiate(playerPrefab);

        // start at 7th child
        var temp = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<TMP_Text>();

        for (int i = 7; i < 17; i++) {
            textList.Add(GameObject.Find("Canvas").transform.GetChild(i).GetComponent<TMP_Text>());

        }

        manager = GetComponent<AIManager>();

    }

    private void FixedUpdate() {
        //var allMass = manager.GetAllMassSorted();

        //for (int i = 0; i < textList.Count; i++) {
        //    textList[i].SetText(allMass[i].ToString());

        //}

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerPrefab.activeInHierarchy) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
