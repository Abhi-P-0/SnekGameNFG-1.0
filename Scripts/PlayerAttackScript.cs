using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject currAttack;
    [SerializeField] private int massCost;

    private PlayerSnakeMovement playerSnakeMovement;

    // Start is called before the first frame update
    void Start()
    {
        playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && currAttack == null) {
            if (playerSnakeMovement.GetMass() > massCost) {
                currAttack = Instantiate(attackPrefab, transform.forward, transform.rotation, transform);
            }

        }
    }
}
