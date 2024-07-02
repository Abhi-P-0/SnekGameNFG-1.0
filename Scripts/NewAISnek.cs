using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAISnek : MonoBehaviour
{
    [Header("Prefabs")]
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
    [SerializeField] private bool balanceSnakeState = false;
    [SerializeField] private float changeDirectionInterval = 3f; // Time between direction changes
    [SerializeField] private float checkFoodRadius = 1;
    [SerializeField] private float randomDirection = 5f;

    [Header("MASS")]
    [SerializeField] private float MASS = 0f;

    private float dis;
    private float scaleThreshold = 100f, newBodyThreshold = 40f;
    private float directionChangeTimer;

    private Vector3 moveDirection;
    private Transform currBodyPart;
    private Transform prevBodyPart;

    private Coroutine LookCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(10f, 50f), Random.Range(-spawnRadius, spawnRadius));

        Transform head = (Instantiate(headPrefab, spawnPoint, Quaternion.identity)).transform;

        head.SetParent(transform);

        bodyParts.Add(head);

        head.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int i = 0; i < initialBodySize; i++) {
            AddBody();

            for (int j = 0; j < Random.Range(1, 10); j++) {
                IncreaseMass(10);
            }

        }
        speed = Random.Range(1f, 3f);
    }

    private void Update() {
        var massAround = GetMassAround();

        if (massAround != null) {
            if (massAround.transform.position.y >= 5f && massAround.transform.position.y < 100f) {
                Debug.Log("Found Mass, Looking at");

                bodyParts[0].LookAt(massAround.transform);
            }
                                   
        }

        Move();

        if (IsOutOfBounds()) {
            DecreaseMass(0.02f);

        }

        //directionChangeTimer -= Time.deltaTime;

        //if (directionChangeTimer <= 0f) {
        //    var massAround = GetMassAround();
            
        //    if (massAround != null) {
        //        moveDirection = massAround.transform.position;

        //    } 
        //    //else {
        //    //    moveDirection = GetRandomDirection();

        //    //}

        //    directionChangeTimer = changeDirectionInterval;

        //}

    }

    private void Move() {
        float currSpeed = speed;

        Transform snakeHead = bodyParts[0];

        //var massAround = GetMassAround();

        //if (massAround != null) {
        //    snakeHead.LookAt(massAround.transform);

        //} else {
        //    snakeHead.LookAt(GetRandomDirection());

        //}
        //snakeHead.LookAt(moveDirection);
        
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
            bodyParts[bodyParts.Count - 1].gameObject.SetActive(false);

        }

        if (MASS % scaleThreshold == 0) {
            bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            //minimumDistanceBetweenParts += 0.17f;
            minimumDistanceBetweenParts -= bodyParts[1].localScale.z / 4;

        }

        if (MASS <= 0) {
            this.gameObject.SetActive(false);

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

        newPart.gameObject.GetComponentInChildren<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

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
    
    private Vector3 GetRandomDirection() {
        return new Vector3(Random.Range(-randomDirection, randomDirection), Random.Range(-randomDirection, randomDirection), Random.Range(-randomDirection, randomDirection)).normalized;
    }

    private bool IsOutOfBounds() {
        if (bodyParts[0].position.y <= 0 || bodyParts[0].position.y >= 100) {
            return true;

        } 
        
        if (bodyParts[0].position.x <= 100 || bodyParts[0].position.x >= 100) {
            return true;

        }
        
        if (bodyParts[0].position.z <= 100 || bodyParts[0].position.z >= 100) {
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
