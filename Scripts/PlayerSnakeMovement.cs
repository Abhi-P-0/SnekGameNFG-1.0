using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerSnakeMovement : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private CinemachineFreeLook playerFreeLookCamera;
    [SerializeField] private CinemachineFreeLook rdPersonCam;
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();
    //[SerializeField] private MonoScript lineRendererScript;

    [Header("Player Character Parameters")]
    [SerializeField] private int initialBodySize = 5;
    [SerializeField] private float minimumDistanceBetweenParts = 0.5f;
    [SerializeField] private float minDistanceIncrement;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float balanceSnakeSpeed = 1f;
    [SerializeField] private bool balanceSnakeState = true;
        
    [Header("MASS")]
    [SerializeField] private int MASS = 0;
    public TMP_Text massText, heightText;

    [Header("Player Attacks and Abilities Prefabs")]
    //[SerializeField] private MonoScript attackPrefabs;
    //[SerializeField] private MonoScript abilitiesPrefabs;
    [SerializeField] private LayerMask abilityHitLayer;
    
    private float dis;
    //private readonly int scaleThreshold = 100, newBodyThreshold = 40;

    private Transform currBodyPart;
    private Transform prevBodyPart;

    //private PlayerInput playerInput;
    private NewPlayerInput newPlayerInput;

    // Start is called before the first frame update
    void Start()
    {
        newPlayerInput = new NewPlayerInput();

        newPlayerInput.Enable();

        MASS = 0;

        var temp = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<TMP_Text>();

        massText = temp;
        heightText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<TMP_Text>();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Transform head = (Instantiate(headPrefab, new Vector3(UnityEngine.Random.Range(-10, 10), 1, UnityEngine.Random.Range(-10, 10)), Quaternion.identity)).transform;

        head.SetParent(transform);

        // Add linerenderer script, player attack and ability script --------------- Not implemented (D0 later) ------------------------------

        bodyParts.Add(head);

        playerFreeLookCamera = Instantiate(playerFreeLookCamera);
        rdPersonCam = Instantiate(rdPersonCam);

        playerFreeLookCamera.transform.SetParent(transform);
        rdPersonCam.transform.SetParent(transform);

        GameObject followTarget = transform.GetChild(0).GetChild(3).gameObject;

        playerFreeLookCamera.Follow = followTarget.transform;
        playerFreeLookCamera.LookAt = followTarget.transform;

        rdPersonCam.Follow = followTarget.transform;
        rdPersonCam.LookAt = followTarget.transform;

        //rdPersonCam.gameObject.SetActive(false);

        for (int i = 0; i < initialBodySize; i++) {
            AddBody();
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        massText.SetText("MASS: " + MASS.ToString());
        heightText.SetText("Y: " + Mathf.Round(bodyParts[0].position.y * 10f) * 0.1f);

        if (bodyParts[0].position.y < 0) {
            DecreaseMass(1);
        }

        Move();

        //if (Input.GetKeyUp(KeyCode.T)) {
        //    AddBody();
        //}

        //if (Input.GetKeyUp(KeyCode.F)) {
        //    IncreaseMass(5);
        //}

        //if (Input.GetKeyUp(KeyCode.C)) {
        //    bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f);

        //    //minimumDistanceBetweenParts += minDistanceIncrement;

        //    //minDistanceIncrement += minDistanceIncrement * 0.15f;
            
        //    minimumDistanceBetweenParts += bodyParts[1].localScale.z / 4;
        //}

        //if (Input.GetKeyUp(KeyCode.Z)) {
        //    bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

        //    //minimumDistanceBetweenParts -= minDistanceIncrement;

        //    //minDistanceIncrement -= minDistanceIncrement * 0.15f;

        //    minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;
        //}

        //if (Input.GetKeyUp(KeyCode.B)) balanceSnakeState = !balanceSnakeState;

        
                
    }

    private float massDecreaseTimer = 0f;

    private void Move() {
        float currSpeed = speed;

        //Debug.Log(newPlayerInput.PlayerTouch.Jump.triggered);
        //if (Input.GetKey(KeyCode.LeftShift) && MASS > 0) {
        if (newPlayerInput.PlayerTouch.Speed.IsPressed() && MASS > 0) { 
            currSpeed *= 2;

            //MASS -= 0.02f;
            //DecreaseMass(1);
            massDecreaseTimer += Time.deltaTime;

            if (massDecreaseTimer >= 1f) {
                DecreaseMass(1);

                massDecreaseTimer = 0f;

            }

        } else {
            massDecreaseTimer = 0;

        }

        Transform snakeHead = bodyParts[0];

        // MOVES SNAKE FORWARD (For testing only move forward when pressing space)
        /*if (Input.GetKey(KeyCode.Space)) */snakeHead.Translate(snakeHead.forward * currSpeed * Time.deltaTime, Space.World);

        // TURN SNAKE LEFT OR RIGHT 
        //snakeHead.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        //snakeHead.Rotate(Vector3.right, Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime);
        Vector2 moveInput = newPlayerInput.PlayerTouch.Move.ReadValue<Vector2>();
        snakeHead.Rotate(Vector3.up, moveInput.x * rotationSpeed * Time.deltaTime);
        snakeHead.Rotate(Vector3.right, -moveInput.y * rotationSpeed * Time.deltaTime);

        // ROTATE SNAKE LEFT OR RIGHT
        //if (Input.GetKey(KeyCode.Q)) snakeHead.Rotate(Vector3.forward);

        //if (Input.GetKey(KeyCode.E)) snakeHead.Rotate(Vector3.forward * -1f);

        if (balanceSnakeState) LevelSnake();


        for (int i = 1; i < bodyParts.Count; i++) {
            currBodyPart = bodyParts[i];
            prevBodyPart = bodyParts[i - 1];

            dis = Vector3.Distance(prevBodyPart.position, currBodyPart.position);

            Vector3 newPos = prevBodyPart.position;

            //newPos.y = bodyParts[0].position.y;

            float T = Time.deltaTime * dis / minimumDistanceBetweenParts * currSpeed;

            if (T > 0.5f) T = 0.5f;

            currBodyPart.position = Vector3.Slerp(currBodyPart.position, newPos, T);
            currBodyPart.rotation = Quaternion.Slerp(currBodyPart.rotation, prevBodyPart.rotation, T);
            currBodyPart.localScale = Vector3.Slerp(currBodyPart.localScale, prevBodyPart.localScale, T);

        }

    }

    private void LevelSnake() {
        // Gradually level the z rotation back to 0
        Vector3 currentEulerAngles = bodyParts[0].transform.rotation.eulerAngles;

        // Convert from 0-360 to -180 to 180 for easier handling
        float zRotation = currentEulerAngles.z;
        if (zRotation > 180) {
            zRotation -= 360;
        }

        // Apply gradual correction
        if (zRotation > 0) {
            zRotation -= balanceSnakeSpeed * Time.deltaTime;
            if (zRotation < 0) {
                zRotation = 0;
            }
        } else if (zRotation < 0) {
            zRotation += balanceSnakeSpeed * Time.deltaTime;
            if (zRotation > 0) {
                zRotation = 0;
            }
        }

        // Apply the corrected z rotation back
        currentEulerAngles.z = zRotation;
        bodyParts[0].transform.transform.rotation = Quaternion.Euler(currentEulerAngles);
    }

    // can just fill array/list at run time, but for testing setup using this
    int[] bodyThresholdArr = { 40, 80, 120,160,200,240,280,320,360,400,440,480,520,560,600,640,680,720,760,800,840,880,920,960,1000,1040,1080,1120,1160,1200,1240,1280,1320,1360,1400,1440,1480,1520,1560,1600,1640,1680,1720,1760,1800,1840,1880,1920,1960 };
    int[] scaleThresholdArr = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3100, 3200, 3300, 3400, 3500, 3600, 3700, 3800, 3900, 4000, 4100, 4200, 4300, 4400, 4500, 4600, 4700, 4800, 4900 };

    public void IncreaseMass(int increaseAmount) {
        MASS += increaseAmount;
        // mass will only increase by 1, (checks included prior to this function call)

        //if (((MASS / 10) * 10) % newBodyThreshold == 0 && MASS > 10) {
        if (bodyThresholdArr.Contains(MASS)) {
            InitBodyPart();

        }

        //if (((MASS / 10) * 10) % scaleThreshold == 0 && MASS > 10) {
        if (scaleThresholdArr.Contains(MASS)) { 
            bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f); // increases head scale, Move() will auto update the other body parts to the same scale

            //minimumDistanceBetweenParts += 0.17f;
            minimumDistanceBetweenParts += bodyParts[1].localScale.z / 4;

        }

    }

    //public void IncreaseMass(int increaseAmount) {
    //    int previousMass = MASS;
    //    //MASS = Mathf.Min(MASS + increaseAmount, maxMass); // Assuming you define a maxMass
    //    MASS += increaseAmount;

    //    // Check if we've crossed a threshold for adding a body part
    //    int previousBodyParts = previousMass / newBodyThreshold;
    //    int currentBodyParts = MASS / newBodyThreshold;

    //    if (currentBodyParts > previousBodyParts) {
    //        for (int i = 0; i < currentBodyParts - previousBodyParts; i++) {
    //            InitBodyPart();
    //        }
    //    }

    //    // Check if we've crossed a threshold for scale increase
    //    int previousScaleLevel = previousMass / scaleThreshold;
    //    int currentScaleLevel = MASS / scaleThreshold;

    //    if (currentScaleLevel > previousScaleLevel) {
    //        float scaleIncrease = 0.1f * (currentScaleLevel - previousScaleLevel);
    //        Vector3 newScale = bodyParts[0].localScale + new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);

    //        // Limit the maximum scale
    //        float maxScale = 5f; // Adjust this value as needed
    //        bodyParts[0].localScale = new Vector3(
    //            Mathf.Min(newScale.x, maxScale),
    //            Mathf.Min(newScale.y, maxScale),
    //            Mathf.Min(newScale.z, maxScale)
    //        );

    //        minimumDistanceBetweenParts += bodyParts[1].localScale.z / 4 * (currentScaleLevel - previousScaleLevel);

    //    }

    //}

    //public void DecreaseMass(int decreaseAmount) {
    //    MASS = Mathf.Max(0, MASS - decreaseAmount); // Prevent negative mass

    //    // Check if we've crossed a threshold for body part removal
    //    if (MASS / newBodyThreshold < bodyParts.Count - 1) { // Keep at least one body part
    //        for (int i = bodyParts.Count - 1; i > 0; i--) { // Start from end, don't deactivate head
    //            if (bodyParts[i].gameObject.activeSelf) {
    //                bodyParts[i].gameObject.SetActive(false);
    //                break;
    //            }
    //        }
    //    }

    //    // Check if we've crossed a threshold for scale reduction
    //    if (MASS / scaleThreshold < bodyParts[0].localScale.x * 10) { // Assuming initial scale is 1
    //        Vector3 newScale = bodyParts[0].localScale - new Vector3(0.1f, 0.1f, 0.1f);
    //        bodyParts[0].localScale = new Vector3(
    //            Mathf.Max(0.1f, newScale.x),
    //            Mathf.Max(0.1f, newScale.y),
    //            Mathf.Max(0.1f, newScale.z)
    //        );

    //        minimumDistanceBetweenParts = Mathf.Max(0.1f, minimumDistanceBetweenParts - bodyParts[1].localScale.z / 4);
    //    }
    //}

    //public void DecreaseMass(int decreaseAmount) {
    //    MASS -= decreaseAmount;
    //    // mass will only decrease by 1, (checks included prior to this functions run)

    //    // Ensure MASS is non-negative to avoid unexpected behavior
    //    if (MASS < 0) {
    //        MASS = 0;
    //        gameObject.SetActive(false);
    //        return;
    //    }

    //    // Use Mathf.Floor to handle the nearest lower multiple of 10 if needed
    //    int truncatedMass = Mathf.FloorToInt(MASS / 10f) * 10;

    //    if (truncatedMass % newBodyThreshold == 0 && truncatedMass > 10) {
    //        //bodyParts[bodyParts.Count - 1].transform.gameObject.SetActive(false);
    //        for (int i = bodyParts.Count - 1; i >= 0; i--) {
    //            if (bodyParts[i].gameObject.activeSelf) {
    //                bodyParts[i].gameObject.SetActive(false);
    //                break;
    //            }
    //        }
    //    }

    //    if (truncatedMass % scaleThreshold == 0 && truncatedMass > 10) {
    //        // Ensure there are enough body parts to scale and adjust
    //        if (bodyParts.Count > 1) {
    //            bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    //            //minimumDistanceBetweenParts += 0.17f;
    //            minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;
    //        }
    //    }
    //}


    public void DecreaseMass(int decreaseAmount) {
        MASS -= decreaseAmount;
        // mass will only decrease by 1, (checks included prior to this function call)

        if (MASS < 0) {
            gameObject.SetActive(false);

        }

        //if (((MASS / 10) * 10) % newBodyThreshold == 0 && MASS > 10) {
        if (bodyThresholdArr.Contains(MASS)) { 
            //bodyParts[bodyParts.Count - 1].transform.gameObject.SetActive(false);
            for (int i = bodyParts.Count - 1; i >= 0; i--) {
                if (bodyParts[i].gameObject.activeSelf) {
                    bodyParts[i].gameObject.SetActive(false);

                    break;
                }
            }

        }

        //if (((MASS / 10) * 10) % scaleThreshold == 0 && MASS > 10) {
        if (scaleThresholdArr.Contains(MASS)) {
            bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            //minimumDistanceBetweenParts += 0.17f;
            minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;

        }



    }

    public int GetMass() { return MASS; }

    public float GetHeight() { return bodyParts[0].position.y; }

    private void InitBodyPart() {
        bool activatedPart = true;

        foreach (var part in bodyParts) {
            if (!part.gameObject.activeSelf) {
                part.gameObject.SetActive(true);

                activatedPart = false;

                break;

            }

        }

        if (activatedPart) AddBody();

    }

    private void AddBody() {
        Transform newPart = (Instantiate(bodyPrefab, bodyParts[bodyParts.Count - 1].position, bodyParts[bodyParts.Count - 1].rotation)).transform;

        newPart.SetParent(transform);

        bodyParts.Add(newPart);

    }


    //private void AddBody() {
    //    Transform newPart = (Instantiate(bodyPrefab, bodyParts[bodyParts.Count - 1].position, bodyParts[bodyParts.Count - 1].rotation)).transform;

    //    newPart.SetParent(transform);

    //    bodyParts.Add(newPart);

    //}
}
