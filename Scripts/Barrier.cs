using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    private void OnTriggerStay(Collider other) {
        if (other.transform.parent.gameObject.name == "Player Snek(Clone)" && other.gameObject.name != "Body(Clone)") {

            PlayerSnakeMovement betterSnakeMovement = other.GetComponentInParent<PlayerSnakeMovement>();

            betterSnakeMovement.DecreaseMass(1);

        }

        if (other.transform.parent.gameObject.name == "AI Snek(Clone)" && other.gameObject.name != "Body(Clone)") {

            NewAISnek newAISnek = other.GetComponentInParent<NewAISnek>();

            newAISnek.DecreaseMass(1);

        }

    }

}
