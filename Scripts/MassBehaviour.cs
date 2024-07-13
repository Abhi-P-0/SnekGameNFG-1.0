using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassBehaviour : MonoBehaviour
{
    [SerializeField] MeshRenderer m_Renderer;

    //[SerializeField] private float growthAmount = 0.01f;
    //[SerializeField] private float massValue = 1.0f;
    [SerializeField] private float risingSpeedMin, risingSpeedMax, risingSpeed;
    [SerializeField] private float massCurrLifetime = 0, minMassLifetime, maxMassLifetime, setMassLifetime;
    [SerializeField] private int massValue;

    private MassSpawner spawner;
    
    private void Start() {
        transform.DOScale(transform.localScale * 1.5f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        
    }

    private void OnEnable() {
        setMassLifetime = Random.Range(minMassLifetime, maxMassLifetime);

        risingSpeed = Random.Range(risingSpeedMin, risingSpeedMax);

    }

    private void FixedUpdate() {
        Rise();

        massCurrLifetime += Time.deltaTime;

        if (massCurrLifetime >= setMassLifetime) {
            transform.DOScale(transform.localScale * 0.25f, 1f).SetEase(Ease.InOutSine);

            gameObject.SetActive(false);
            massCurrLifetime = 0; 
        }

    }


    private void Rise() {
        transform.Translate(transform.up * risingSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
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
