using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StationaryMassBehaviour : MonoBehaviour
{
    [SerializeField] private float massCurrLifetime = 0, minMassLifetime, maxMassLifetime, setMassLifetime;
    [SerializeField] private int massValue;

    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(transform.localScale * 1.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

    }
    private void OnEnable() {
        setMassLifetime = UnityEngine.Random.Range(minMassLifetime, maxMassLifetime);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        massCurrLifetime += Time.deltaTime;

        if (massCurrLifetime >= setMassLifetime) {
            transform.DOScale(transform.localScale * 0.25f, 1f).SetEase(Ease.InOutSine);

            gameObject.SetActive(false);
            massCurrLifetime = 0;
        }
    }

    

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Attack")) { Debug.Log("Returning"); return; }

        if (other.transform.parent.gameObject.name == "Player Snek(Clone)" && other.gameObject.name != "Body(Clone)") {

            PlayerSnakeMovement betterSnakeMovement = other.GetComponentInParent<PlayerSnakeMovement>();

            //betterSnakeMovement.IncreaseMass(massValue);
            for (int i = 0; i < massValue; i++) {
                betterSnakeMovement.IncreaseMass(1);
            }

            gameObject.SetActive(false);
        }

        if (other.transform.parent.gameObject.name == "AI Snek(Clone)" && other.gameObject.name != "Body(Clone)") {

            NewAISnek newAISnek = other.GetComponentInParent<NewAISnek>();

            //newAISnek.IncreaseMass(massValue);
            for (int i = 0; i < massValue; i++) {
                newAISnek.IncreaseMass(1);
            }

            gameObject.SetActive(false);
        }

        if (other.transform.gameObject.name == "Celing") {
            gameObject.SetActive(false);

        }

    }

}
