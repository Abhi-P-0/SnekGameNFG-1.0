using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour {
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

                currAttack = Instantiate(attackPrefab, spawnPoint.position, spawnPoint.rotation);

                currAttack.transform.forward = transform.forward;

                playerSnakeMovement.DecreaseMass(massCost);
            }

        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerAttackScript : MonoBehaviour {
//    [SerializeField] private GameObject attackPrefab;
//    [SerializeField] private GameObject currAttack;
//    [SerializeField] private int massCost = 5; // Exposed in the inspector for tuning

//    private PlayerSnakeMovement playerSnakeMovement;

//    void Start() {
//        playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();
//    }

//    void Update() {
//        if (Input.GetMouseButtonUp(0) && currAttack == null) {
//            TryAttack();
//        }
//    }

//    private void TryAttack() {
//        if (playerSnakeMovement.GetMass() >= massCost) {
//            Vector3 spawnPosition = transform.position + transform.forward;

//            currAttack = Instantiate(attackPrefab, spawnPosition, transform.rotation);
//            currAttack.transform.forward = transform.forward;

//            playerSnakeMovement.DecreaseMass(massCost);

//            // Start a coroutine to check when the attack is destroyed
//            StartCoroutine(WaitForAttackDestruction());
//        }
//    }

//    private IEnumerator WaitForAttackDestruction() {
//        while (currAttack != null) {
//            yield return null;
//        }
//        // Once currAttack is null (destroyed), the coroutine ends and the player can attack again
//    }
//}
