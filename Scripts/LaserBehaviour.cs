using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int dmg;

    private float timePassed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

        timePassed += Time.deltaTime;

        if (timePassed > 10f) {
            Destroy(gameObject);

        }
    }
     
    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.tag);

        if (other.gameObject.CompareTag("AI")) {
            NewAISnek newAISnek = GetComponentInParent<NewAISnek>();

            newAISnek.DecreaseMass(dmg);

            Destroy(this.gameObject);

        }

        //if (other.gameObject.CompareTag("Player")) {
        //    PlayerSnakeMovement playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();

        //    playerSnakeMovement.DecreaseMass(dmg);

        //    Destroy(this.gameObject);

        //}

    }

}
