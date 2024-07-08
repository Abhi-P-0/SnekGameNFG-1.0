using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int dmg;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("AI")) {
            NewAISnek newAISnek = GetComponentInParent<NewAISnek>();

            newAISnek.DecreaseMass(dmg);

            Destroy(this.gameObject);

        }

        if (other.gameObject.CompareTag("Player")) {
            PlayerSnakeMovement playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();

            playerSnakeMovement.DecreaseMass(dmg);

            Destroy(this.gameObject);

        }

    }

}
