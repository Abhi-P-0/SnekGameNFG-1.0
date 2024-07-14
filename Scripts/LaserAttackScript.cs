using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaserAttackScript : MonoBehaviour {

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private GameObject currAttack;
    [SerializeField] private int massCost;

    private PlayerSnakeMovement playerSnakeMovement;
    private TMP_Text timerTxt;
    private float maxLifeTimer;
    private float currLifeTimer;

    private NewPlayerInput newPlayerInput;

    // Start is called before the first frame update
    void Start() {
        newPlayerInput = new NewPlayerInput();
        newPlayerInput.Enable();

        playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();

        timerTxt = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<TMP_Text>();

        maxLifeTimer = attackPrefab.GetComponent<LaserBehaviour>().GetMaxLifeTime();
    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetMouseButtonUp(0) && currAttack == null) {
        if (newPlayerInput.PlayerTouch.Attack.triggered && currAttack == null) {
            if (playerSnakeMovement.GetMass() >= massCost) {
                Vector3 spawnPosition = transform.position + transform.forward;

                currAttack = Instantiate(attackPrefab, spawnPosition, spawnPoint.rotation);

                currAttack.transform.forward = transform.forward;

                //playerSnakeMovement.DecreaseMass(massCost);
                for (int i = 0; i < massCost; i++) {
                    playerSnakeMovement.DecreaseMass(1);
                }
            }

        }

        if (currAttack != null) {
            currLifeTimer -= Time.deltaTime;

            timerTxt.SetText((Mathf.Round(currLifeTimer * 10f) * 0.1f).ToString());

        } else {
            currLifeTimer = maxLifeTimer;

            timerTxt.SetText("Ready");
        }
    }
}

