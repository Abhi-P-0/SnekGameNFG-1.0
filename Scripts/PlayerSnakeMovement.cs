using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnakeMovement : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private CinemachineFreeLook playerFreeLookCamera;
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();

    [Header("Player Parameters")]
    [SerializeField] private int initialBodySize = 5;
    [SerializeField] private float minimumDistanceBetweenParts = 0.5f;
    [SerializeField] private float minDistanceIncrement;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float balanceSnakeSpeed = 1f;
    [SerializeField] private bool balanceSnakeState = true;

    [Header("MASS")]
    [SerializeField] private int MASS = 0;


    private float dis;
    private float scaleThreshold = 100f, newBodyThreshold = 40f;

    private Transform currBodyPart;
    private Transform prevBodyPart;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Transform head = (Instantiate(headPrefab, new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10)), Quaternion.identity)).transform;

        head.SetParent(transform);

        bodyParts.Add(head);

        playerFreeLookCamera = Instantiate(playerFreeLookCamera);

        playerFreeLookCamera.transform.SetParent(transform);

        GameObject followTarget = transform.GetChild(0).GetChild(3).gameObject;

        playerFreeLookCamera.Follow = followTarget.transform;
        playerFreeLookCamera.LookAt = followTarget.transform;

        for (int i = 0; i < initialBodySize; i++) {
            AddBody();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (Input.GetKeyUp(KeyCode.T)) {
            AddBody();
        }

        if (Input.GetKeyUp(KeyCode.C)) {
            bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f);

            //minimumDistanceBetweenParts += minDistanceIncrement;

            //minDistanceIncrement += minDistanceIncrement * 0.15f;
            
            minimumDistanceBetweenParts += bodyParts[1].localScale.z / 4;
        }

        if (Input.GetKeyUp(KeyCode.Z)) {
            bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            //minimumDistanceBetweenParts -= minDistanceIncrement;

            //minDistanceIncrement -= minDistanceIncrement * 0.15f;

            minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;
        }

        if (Input.GetKeyUp(KeyCode.B)) balanceSnakeState = !balanceSnakeState;

    }

    private void Move() {
        float currSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) && MASS > 0) {
            currSpeed *= 2;

            //MASS -= 0.02f;
            DecreaseMass(1);
        }

        Transform snakeHead = bodyParts[0];

        // MOVES SNAKE FORWARD (For testing only move forward when pressing space)
        if (Input.GetKey(KeyCode.Space)) snakeHead.Translate(snakeHead.forward * currSpeed * Time.deltaTime, Space.World);

        // TURN SNAKE LEFT OR RIGHT 
        snakeHead.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        snakeHead.Rotate(Vector3.right, Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime);

        // ROTATE SNAKE LEFT OR RIGHT
        if (Input.GetKey(KeyCode.Q)) snakeHead.Rotate(Vector3.forward);

        if (Input.GetKey(KeyCode.E)) snakeHead.Rotate(Vector3.forward * -1f);

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

    

    public void IncreaseMass(int increaseAmount) {
        MASS += increaseAmount;

        if (MASS % newBodyThreshold == 0) {
            InitBodyPart();

        }

        if (MASS % scaleThreshold == 0) {
            bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f);

            //minimumDistanceBetweenParts += 0.17f;
            minimumDistanceBetweenParts += bodyParts[1].localScale.z / 4;

        }

    }

    public void DecreaseMass(int decreaseAmount) {
        MASS -= decreaseAmount;

        if (MASS % newBodyThreshold == 0) {
            bodyParts[bodyParts.Count - 1].transform.gameObject.SetActive(false);

        }

        if (MASS % scaleThreshold == 0) {
            bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            //minimumDistanceBetweenParts += 0.17f;
            minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;

        }

    }

    public int GetMass() { return MASS; }

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
