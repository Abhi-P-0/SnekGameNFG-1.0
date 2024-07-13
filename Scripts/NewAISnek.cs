using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewAISnek : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<GameObject> headPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> bodyPrefabs = new List<GameObject>();
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();
    [SerializeField] private LayerMask layerMask;

    [Header("AI Parameters")]
    [SerializeField] private int spawnRadius = 1;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private Transform massToLookAt;
    [SerializeField] private int initialBodySize = 5;
    [SerializeField] private float minimumDistanceBetweenParts = 1.1f;
    [SerializeField] private float minDistanceIncrement;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float balanceSnakeSpeed = 1f;
    //[SerializeField] private bool balanceSnakeState = false;
    //[SerializeField] private float changeDirectionInterval = 3f; // Time between direction changes
    //[SerializeField] private float checkFoodRadius = 1;
    [SerializeField] private float randomDirection = 5f;
    [SerializeField] private LayerMask aiLayer;
    //[SerializeField] private string snakeName;

    [Header("MASS")]
    [SerializeField] private int MASS = 0;

    private float dis;
    private float scaleThreshold = 100f, newBodyThreshold = 40f;
    private float directionChangeTimer;

    private float timePassed = 0f;

    private Vector3 moveDirection;
    private Transform currBodyPart;
    private Transform prevBodyPart;

    private Color headColour, bodyColour;

    private Coroutine LookCoroutine;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderInterval = 3f;

    private Vector3 targetPosition;
    private float wanderTimer;
    private Lexic.NameGenerator nameGenerator;

    //void Start()
    //{
    //    Vector3 spawnPoint = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(10f, 50f), Random.Range(-spawnRadius, spawnRadius));

    //    Transform head = (Instantiate(headPrefab, spawnPoint, Quaternion.identity)).transform;

    //    head.SetParent(transform);

    //    bodyParts.Add(head);

    //    head.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

    //    for (int i = 0; i < initialBodySize; i++) {
    //        AddBody();

    //        for (int j = 0; j < Random.Range(1, 10); j++) {
    //            IncreaseMass(10);
    //        }

    //    }
    //    speed = Random.Range(1f, 3f);
    //}

    private void OnEnable() {
        //snakeName = nameGenerator.GetNextRandomName();

        Vector3 spawnPoint = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(5, spawnRadius), Random.Range(-spawnRadius, spawnRadius));

        headPrefab = headPrefabs[Random.Range(0, headPrefabs.Count)]; // randomly selects a head prefab from the available options

        bodyPrefab = bodyPrefabs[Random.Range(0, bodyPrefabs.Count)];

        Transform head = (Instantiate(headPrefab, spawnPoint, Quaternion.identity)).transform;

        head.gameObject.tag = "AI";

        //head.gameObject.layer = aiLayer;

        head.SetParent(transform);

        bodyParts.Add(head);

        headColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        bodyColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        head.gameObject.GetComponent<MeshRenderer>().material.color = headColour;

        for (int i = 0; i < initialBodySize; i++) {
            for (int j = 0; j < Random.Range(1, 10); j++) {
                IncreaseMass(10);

            }

        }

        speed = Random.Range(1f, 3f);

        SetNewWanderTarget();

    }

    private void OnDisable() {
        bodyParts.Clear();

        //Debug.Log(transform.childCount);

        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        //while (transform.childCount > 0) { // this shit broke somehow, used to work perfectly, then just broke, no idea
        //    //DestroyImmediate(transform.GetChild(0).gameObject);
        //    Destroy(transform.GetChild(0).gameObject); // safer, waits till the end of frame so keep index safe for whatever reason (just incase)
        //}

    }
        
    private void Update() {
        var massAround = GetMassAround();

        if (massAround != null) {
            if (massAround.transform.position.y >= 5f && massAround.transform.position.y < 100f) {
                bodyParts[0].LookAt(massAround.transform);
                wanderTimer = 0; // Reset wander timer when food is found
            }
        } else {
            Wander();
        }

        LevelSnake();
        Move();
        HandleOutOfBounds();
    }

    //private void Update() {
    //    var massAround = GetMassAround();

    //    if (massAround != null) {
    //        if (massAround.transform.position.y >= 5f && massAround.transform.position.y < 100f) {
    //            //Debug.Log("Found Mass, Looking at");

    //            bodyParts[0].LookAt(massAround.transform);
    //        }

    //        randomLookTimer = 0;

    //    } else {
    //        randomLookTimer += Time.deltaTime;

    //        if (randomLookTimer > 2f) {
    //            int selectedDir = Random.Range(0, directions.Length - 1);

    //            bodyParts[0].LookAt(directions[selectedDir]);

    //            randomLookTimer = 0;

    //        }

    //        //    int horizontalTurnChance = Random.Range(-1, 1), verticalTurnChance = Random.Range(-1, 1);

    //        //    if (horizontalTurnChance != 0) snakeHead.Rotate(Vector3.up, horizontalTurnChance * rotationSpeed * Time.deltaTime);

    //        //    if (verticalTurnChance != 0) snakeHead.Rotate(Vector3.right, verticalTurnChance * rotationSpeed * Time.deltaTime);
    //    }

    //    LevelSnake();

    //    Move();

    //    if (IsOutOfBounds()) {
    //        timePassed += Time.deltaTime;

    //        if (timePassed > 1f) {
    //            DecreaseMass(1);

    //            timePassed = 0f;
    //        }

    //    } else if (!IsOutOfBounds()) {
    //        timePassed = 0f;
    //    }

    //}

    private void Move() {
        float currSpeed = speed;

        Transform snakeHead = bodyParts[0];
               
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

    int[] bodyThresholdArr = { 40, 80, 120, 160, 200, 240, 280, 320, 360, 400, 440, 480, 520, 560, 600, 640, 680, 720, 760, 800, 840, 880, 920, 960, 1000, 1040, 1080, 1120, 1160, 1200, 1240, 1280, 1320, 1360, 1400, 1440, 1480, 1520, 1560, 1600, 1640, 1680, 1720, 1760, 1800, 1840, 1880, 1920, 1960 };
    int[] scaleThresholdArr = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3100, 3200, 3300, 3400, 3500, 3600, 3700, 3800, 3900, 4000, 4100, 4200, 4300, 4400, 4500, 4600, 4700, 4800, 4900 };


    public void IncreaseMass(int increaseAmount) {
        MASS += increaseAmount;

        //if (((MASS / 10) * 10) % newBodyThreshold == 0) {
        if (bodyThresholdArr.Contains(MASS)) { 
            InitBodyPart();

        }

        //if (((MASS / 10) * 10) % scaleThreshold == 0) {
        if (scaleThresholdArr.Contains(MASS)) {
            bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f);

            minimumDistanceBetweenParts += bodyParts[1].localScale.z / 4;

        }

    }

    public void DecreaseMass(int decreaseAmount) {
        MASS -= decreaseAmount;
        
        if (MASS <= 0) {
            this.gameObject.SetActive(false);

        }

        //if (((MASS / 10) * 10) % newBodyThreshold == 0) {
        if (bodyThresholdArr.Contains(MASS)) {
            // start end of list, first active bodypart should be set to deactive
            for (int i = bodyParts.Count - 1; i >= 0; i--) {
                if (bodyParts[i].gameObject.activeSelf) {
                    bodyParts[i].gameObject.SetActive(false);

                    break;
                }
            }

        }

        //if (((MASS / 10) * 10) % scaleThreshold == 0) {
        if (scaleThresholdArr.Contains(MASS)) {
            bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;

        }


                
    }

    public int GetMass() { return MASS; }

    //public string GetSnakeName() { return snakeName; }

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
        bodyParts[0].transform.transform.rotation = Quaternion.Euler(currentEulerAngles); // -------------------------- FIX THIS LINE LATER ---------------------------------
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

        //newPart.gameObject.GetComponentInChildren<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        newPart.gameObject.GetComponentInChildren<MeshRenderer>().material.color = bodyColour;

        newPart.gameObject.tag = "AI";

        //newPart.gameObject.layer = aiLayer;

        newPart.SetParent(transform);

        bodyParts.Add(newPart);

    }

    private Collider GetMassAround() {
        Collider temp;

        if (Physics.OverlapSphere(bodyParts[0].transform.position, 10f, layerMask).Length > 0) {
            temp = Physics.OverlapSphere(bodyParts[0].transform.position, 10f, layerMask)[0];

        } else {
            temp = null;

        }

        return temp;
    }

    private void HandleOutOfBounds() {
        if (IsOutOfBounds()) {
            timePassed += Time.deltaTime;
            if (timePassed > 1f) {
                DecreaseMass(1);
                timePassed = 0f;

                // Move towards center when out of bounds
                Vector3 centerDirection = -bodyParts[0].position.normalized;
                bodyParts[0].rotation = Quaternion.Slerp(bodyParts[0].rotation, Quaternion.LookRotation(centerDirection), rotationSpeed * Time.deltaTime);
            }
        } else {
            timePassed = 0f;
        }
    }

    private void Wander() {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval) {
            SetNewWanderTarget();
            wanderTimer = 0;
        }

        Vector3 direction = (targetPosition - bodyParts[0].position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        bodyParts[0].rotation = Quaternion.Slerp(bodyParts[0].rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void SetNewWanderTarget() {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += bodyParts[0].position;
        randomDirection.y = Mathf.Clamp(randomDirection.y, 5f, 95f); // Keep within vertical bounds
        targetPosition = randomDirection;
    }


    private Vector3 GetRandomDirection() {
        return new Vector3(Random.Range(-randomDirection, randomDirection), Random.Range(-randomDirection, randomDirection), Random.Range(-randomDirection, randomDirection)).normalized;
    }

    private bool IsOutOfBounds() {
        if (bodyParts[0].position.y <= 0 || bodyParts[0].position.y >= 100) {
            return true;

        } 
        
        if (bodyParts[0].position.x <= -100 || bodyParts[0].position.x >= 100) {
            return true;

        }
        
        if (bodyParts[0].position.z <= -100 || bodyParts[0].position.z >= 100) {
            return true;

        }

        return false;

    }

    public void SetHeadPosition(Vector3 newPosition) {
        bodyParts[0].position = newPosition;

    }

    public void StartRotating() {
        if (LookCoroutine != null) {
            StopCoroutine(LookCoroutine);

        }

        LookCoroutine = StartCoroutine(LookAt());

    }

    private IEnumerator LookAt() {
        Quaternion lookRotation = Quaternion.LookRotation(massToLookAt.position - bodyParts[0].position);

        float time = 0;

        while (time < 1) {
            bodyParts[0].rotation = Quaternion.Slerp(bodyParts[0].rotation, lookRotation, time);

            time += Time.deltaTime * rotateSpeed;

            yield return null;

        }
    }

}
