using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AISnek : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();
    public LayerMask layerMask;

    [Header("AI Parameters")]
    //[SerializeField] private int initialBodySize = 5;
    [SerializeField] private float minimumDistanceBetweenParts = 1.1f;
    [SerializeField] private float minDistanceIncrement;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float balanceSnakeSpeed = 1f;
    //[SerializeField] private bool balanceSnakeState = false;
    [SerializeField] private float changeDirectionInterval = 3f; // Time between direction changes
    [SerializeField] private int spawnRadius = 1;
    //[SerializeField] private float checkFoodRadius = 1;

    [Header("MASS")]
    [SerializeField] private float MASS = 0f;
    //[SerializeField] private LayerMask LayerMask;


    private float dis;
    private float scaleThreshold = 100f, newBodyThreshold = 40f;
    private float directionChangeTimer;
    
    private Vector3 moveDirection;
    private Transform currBodyPart;
    private Transform prevBodyPart;

    // Start is called before the first frame update
    void Start()
    {
        //var cubeRenderer = GameObject.Find("Cube").GetComponent<Renderer>();
        //cubeRenderer.material.SetColor("_Color", new Color(0, 204, 102));

        Transform head = (Instantiate(headPrefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), 1, Random.Range(-spawnRadius, spawnRadius)), Quaternion.identity)).transform;

        head.SetParent(transform);

        bodyParts.Add(head);

        head.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                        
                
        //for (int i = 0; i < initialBodySize; i++) {
        //    AddBody();

        //    for (int j = 0; j < Random.Range(1, 10);  j++) {
        //        IncreaseMass(10);
        //    }

        //}
    }

    private void FixedUpdate() {
        //bool closeMass = false;
        //Collider[] colliders;

        //closeMass = Physics.CheckSphere(bodyParts[0].position, 5f, 9);
        //colliders = Physics.OverlapSphere(bodyParts[0].position, 5f, 9);
        //var colliders = Physics.OverlapSphere(transform.position, 10f, layerMask);
        //var colliders = Physics.SphereCastAll(bodyParts[0].position, 10f, layerMask);

        //Move();

        directionChangeTimer -= Time.deltaTime;

        if (directionChangeTimer <= 0f) {
            //var firstMass = Physics.OverlapSphere(bodyParts[0].transform.position, 10f, layerMask);
            var firstMass = GetMassAround();

            //moveDirection = firstMass == null ? GetRandomDirection() : firstMass[0].transform.position;
            //moveDirection = firstMass ?? GetRandomDirection();
            //if (firstMass != null) moveDirection = firstMass; else moveDirection = GetRandomDirection();
            if (firstMass != null) {
                Debug.Log(firstMass);

                //moveDirection = firstMass[0].transform.position;
                //bodyParts[0].transform.rotation = Quaternion.FromToRotation(Vector3.forward, (firstMass[0].transform.position - bodyParts[0].transform.position));
                Vector3 direction_to_model = firstMass.transform.position - bodyParts[0].transform.position;

                //bodyParts[0].transform.rotation = Quaternion.LookRotation(direction_to_model) * Quaternion.Inverse(Quaternion.LookRotation());

            } else {
                moveDirection = GetRandomDirection();
            }

            directionChangeTimer = changeDirectionInterval;

        }

        LevelSnake();

        //colliders[0].gameObject.transform.position;

    }

    //void Update() {
    //    Move();

    //    directionChangeTimer -= Time.deltaTime;
    //    if (directionChangeTimer <= 0) {
    //        moveDirection = GetRandomDirection();
    //        //moveDirection = Physics.CheckSphere(bodyParts[0].transform.position, checkFoodRadius, 6);
    //        directionChangeTimer = changeDirectionInterval;
    //    }

    //    if (balanceSnakeState) LevelSnake();
    //}

    private void Move() {
        float currSpeed = speed;

        Transform snakeHead = bodyParts[0];

        // TURN SNAKE TO THE NEW DIRECTION
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        snakeHead.rotation = Quaternion.Slerp(snakeHead.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        // MOVES SNAKE FORWARD
        snakeHead.Translate(Vector3.forward * currSpeed * Time.deltaTime);

        for (int i = 1; i < bodyParts.Count; i++) {
            currBodyPart = bodyParts[i];
            prevBodyPart = bodyParts[i - 1];

            dis = Vector3.Distance(prevBodyPart.position, currBodyPart.position);

            Vector3 newPos = prevBodyPart.position;

            float T = Time.deltaTime * dis / minimumDistanceBetweenParts * currSpeed;

            if (T > 0.5f) T = 0.5f;

            currBodyPart.position = Vector3.Slerp(currBodyPart.position, newPos, T);
            currBodyPart.rotation = Quaternion.Slerp(currBodyPart.rotation, prevBodyPart.rotation, T);
            currBodyPart.localScale = Vector3.Slerp(currBodyPart.localScale, prevBodyPart.localScale, T);
        }
    }


    // Update is called once per frame
    //void Update()
    //{
    //    MoveAISnake();

    //}

    //private void MoveAISnake() {
    //    float currSpeed = speed;

    //    if (Input.GetKey(KeyCode.LeftShift) && MASS > 0) {
    //        currSpeed *= 2;

    //        //MASS -= 0.02f;
    //        DecreaseMass(0.02f);
    //    }

    //    Transform snakeHead = bodyParts[0];

    //    snakeHead.Translate(snakeHead.forward * currSpeed * Time.deltaTime, Space.World);

    //    float turnLeftChance = Random.Range(1, 5), turnRightChance = Random.Range(1, 5);
    //    int horizontalTurnChance = Random.Range(-1, 1), verticalTurnChance = Random.Range(-1, 1);

    //    //if (turnLeftChance >= 3) snakeHead.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
    //    if (horizontalTurnChance != 0) snakeHead.Rotate(Vector3.up, horizontalTurnChance * rotationSpeed * Time.deltaTime);

    //    if (verticalTurnChance != 0) snakeHead.Rotate(Vector3.right, verticalTurnChance * rotationSpeed * Time.deltaTime);

    //    for (int i = 1; i < bodyParts.Count; i++) {
    //        currBodyPart = bodyParts[i];
    //        prevBodyPart = bodyParts[i - 1];

    //        dis = Vector3.Distance(prevBodyPart.position, currBodyPart.position);

    //        Vector3 newPos = prevBodyPart.position;

    //        //newPos.y = bodyParts[0].position.y;

    //        float T = Time.deltaTime * dis / minimumDistanceBetweenParts * currSpeed;

    //        if (T > 0.5f) T = 0.5f;

    //        currBodyPart.position = Vector3.Slerp(currBodyPart.position, newPos, T);
    //        currBodyPart.rotation = Quaternion.Slerp(currBodyPart.rotation, prevBodyPart.rotation, T);
    //        currBodyPart.localScale = Vector3.Slerp(currBodyPart.localScale, prevBodyPart.localScale, T);

    //    }


    //}

    public void IncreaseMass(float increaseAmount) {
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

    public void DecreaseMass(float decreaseAmount) {
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

    private Vector3 GetRandomDirection() {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

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

    private Collider GetMassAround() {
        //var temp;// = Physics.OverlapSphere(bodyParts[0].transform.position, 10f, layerMask);
        Collider temp;

        if (Physics.OverlapSphere(bodyParts[0].transform.position, 10f, layerMask).Length > 0) {
            temp = Physics.OverlapSphere(bodyParts[0].transform.position, 10f, layerMask)[0];
        
        } else {
            temp = null;

        }
        
        return temp;
    }

}
