using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttackScript : MonoBehaviour {
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject currAttack;
    [SerializeField] private int massCost;

    private PlayerSnakeMovement playerSnakeMovement;

    // Start is called before the first frame update
    void Start() {
        playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonUp(0) && currAttack == null) {
            if (playerSnakeMovement.GetMass() >= massCost) {
                Vector3 spawnPosition = transform.position + transform.forward;

                currAttack = Instantiate(attackPrefab, spawnPosition, spawnPoint.rotation);

                currAttack.transform.forward = transform.forward;

                playerSnakeMovement.DecreaseMass(massCost);
            }

        }
    }
}

