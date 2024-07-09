using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int dmg;
    [SerializeField] private float maxLifeTime;

    private float timePassed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

        timePassed += Time.deltaTime;

        if (timePassed > maxLifeTime) {
            Destroy(gameObject);

        }
    }
     
    private void OnTriggerEnter(Collider other) {        
        if (other.gameObject.CompareTag("AI")) {
            //var script = other.GetComponentInParent<NewAISnek>();

            //if (script != null) {                
            //    NewAISnek newAISnek = other.GetComponentInParent<NewAISnek>();
                              
            //    newAISnek.DecreaseMass(dmg);

            //    Destroy(this.gameObject);
            //}
            if (other.transform.parent.TryGetComponent<NewAISnek>(out NewAISnek AIScript)) {
                AIScript.DecreaseMass(dmg);

                Destroy(gameObject);

            }            

        }

        //if (other.gameObject.CompareTag("Player")) {
        //    PlayerSnakeMovement playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();

        //    playerSnakeMovement.DecreaseMass(dmg);

        //    Destroy(this.gameObject);

        //}

    }

}
