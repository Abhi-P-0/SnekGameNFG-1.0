using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpawner : MonoBehaviour
{
    [SerializeField] private GameObject massPrefab;

    [SerializeField] private Transform parentTransform;

    [SerializeField] private float spawnRadius = 50f;

    [SerializeField] private int initialSpawnCount = 10;

    [SerializeField] private List<GameObject> massPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < initialSpawnCount; i++) {
            CreateAndAddMass();

        }

    }

    private void FixedUpdate() {
        for (int i = 0; i < massPool.Count; i++) {
            if (!massPool[i].activeInHierarchy) {
                EnableMass(i);


            }

        }

    }

    private void EnableMass(int massIndex) {
        massPool[massIndex].SetActive(true);
        massPool[massIndex].transform.position = GetRandomPosition();

    }

    void CreateAndAddMass() {
        Vector3 spawnPosition = GetRandomPosition();
        GameObject newMass = Instantiate(massPrefab, spawnPosition, Quaternion.identity);

        newMass.transform.SetParent(parentTransform, true);

        newMass.SetActive(true);
        //newMass.GetComponent<MassBehaviour>().SetSpawner(this);
        
        massPool.Add(newMass);

    }

    private Vector3 GetRandomPosition() {
        return new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            Random.Range(-2, -25),
            Random.Range(-spawnRadius, spawnRadius)
        );

    }


}
