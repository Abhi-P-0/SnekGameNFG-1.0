using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MassDrainAbility : MonoBehaviour
{
    [SerializeField] private int massCost;
    [SerializeField] private int maxCoolDownTimer;

    private PlayerSnakeMovement playerSnakeMovement;
    private float timePassed;
    private TMP_Text timerTxt;

    List<int> uniqueAIs = new();
    List<Collider> filteredAround = new();

    // Start is called before the first frame update
    void Start()
    {
        playerSnakeMovement = GetComponentInParent<PlayerSnakeMovement>();

        timerTxt = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<TMP_Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (timePassed >= 0) {
            timePassed -= Time.deltaTime;

            timerTxt.SetText((Mathf.Round(timePassed * 10f) * 0.1f).ToString());

        } else { // prevent it from increasing infinitly 
            timerTxt.SetText("Ready");

        }

        if (Input.GetKeyUp(KeyCode.R) && timePassed <= 0 && playerSnakeMovement.GetMass() >= massCost) { // only allow player to shoot if the cooldown timer is reached
            timePassed = maxCoolDownTimer;

            var sneksAround = Physics.OverlapSphere(transform.position, 15f);

            

            // check every thing around if it has AI tag which means an AI snake is close to the player
            foreach (Collider c in sneksAround) {
                if (c.gameObject.CompareTag("AI")) {
                    filteredAround.Add(c);
                }
            }

            // filter each head/bodypart into the parent's unique ID since all individual snake part is nested in an overarching parent object
            if (filteredAround.Count > 0) {
                
                //List<GameObject> uniqueAIObjects = new();

                foreach (Collider c in filteredAround) {
                    if (!uniqueAIs.Contains(c.gameObject.transform.parent.GetInstanceID())) {

                        uniqueAIs.Add(c.gameObject.transform.parent.GetInstanceID());

                        //uniqueAIObjects.Add(c.gameObject.transform.parent.gameObject);
                        NewAISnek tempSnekScript = c.gameObject.transform.parent.gameObject.GetComponent<NewAISnek>();

                        //tempSnekScript.DecreaseMass(5);
                        for (int i = 0; i < 5;  i++) {
                            tempSnekScript.DecreaseMass(1);
                        }

                        //playerSnakeMovement.IncreaseMass(5);
                        for (int i = 0;i < 5; i++) {
                            playerSnakeMovement.IncreaseMass(1);
                        }

                    }
                }

                // now drain the mass of any snake around
                //foreach (GameObject c in uniqueAIObjects) {
                //    NewAISnek tempSnekScript = c.GetComponent<NewAISnek>();

                //    tempSnekScript.DecreaseMass(5);

                //    playerSnakeMovement.IncreaseMass(5);
                //}

            }

            filteredAround.Clear();
            uniqueAIs.Clear();

        }
    }
}
