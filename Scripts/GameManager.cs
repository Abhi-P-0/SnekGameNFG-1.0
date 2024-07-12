using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.SetTweensCapacity(1000, 1000);

        playerPrefab = Instantiate(playerPrefab);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerPrefab.activeInHierarchy) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
