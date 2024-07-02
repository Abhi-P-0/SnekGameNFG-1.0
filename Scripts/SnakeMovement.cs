using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class SnakeMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

    [SerializeField] private CinemachineFreeLook playerFreeLookCamera;
    //[SerializeField] private Transform bodySpawnLocation;
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private GameObject bodyPrefab;
    //[SerializeField] private List<GameObject> bodyList = new List<GameObject>();
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();

    //[SerializeField] private int initialBodySize = 5;
    [SerializeField] private float minimumDistanceBetweenParts = 0.5f;
    [SerializeField] private float speed = 1f;
    [SerializeField] float rotationSpeed = 50f;

    [SerializeField] float MASS = 0f;


    private float dis;
    private float scaleThreshold = 100f, newBodyThreshold = 40f;

    private Transform currBodyPart;
    private Transform prevBodyPart;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            TestMove();
        }
    }

    [ServerRpc]
    void SubmitPositionServerRPC(ServerRpcParams rpcParams = default) {
        Position.Value = new Vector3(Random.Range(-10, 10), 1f, Random.Range(-10, 10));

    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Transform head = (Instantiate(headPrefab, new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10)), Quaternion.identity)).transform;

        head.SetParent(transform);

        bodyParts.Add(head);

        //playerFreeLookCamera = Instantiate(playerFreeLookCamera);

        //playerFreeLookCamera.transform.SetParent(transform);

        //GameObject followTarget = transform.GetChild(0).GetChild(3).gameObject;

        //playerFreeLookCamera.Follow = followTarget.transform;
        //playerFreeLookCamera.LookAt = followTarget.transform;

        //for (int i = 0; i < initialBodySize; i++) {
        //    AddBody();
        //}

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Position.Value;

        //Move();

        //if (Input.GetKeyUp(KeyCode.T)) {
        //    AddBody();
        //}

        //if (Input.GetKeyUp(KeyCode.C)) {
        //    bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f);

        //    minimumDistanceBetweenParts += 0.17f;
        //}

        //if (Input.GetKeyUp(KeyCode.Z)) {
        //    bodyParts[0].localScale -= new Vector3(0.1f, 0.1f, 0.1f);

        //    minimumDistanceBetweenParts -= 0.17f;
        //}

    }

    public void TestMove() {
        if (NetworkManager.Singleton.IsServer) {
            var randomPosition = new Vector3(Random.Range(-10, 10), 1f, Random.Range(-10, 10));
            
            transform.position = randomPosition;

            Position.Value = randomPosition;

        } else {
            SubmitPositionServerRPC();

        }
    }

    private void Move() {
        float currSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) && MASS > 0) {
            currSpeed *= 2;

            MASS -= 0.02f;
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


    private void AddBody() {
        Transform newPart = (Instantiate(bodyPrefab, bodyParts[bodyParts.Count - 1].position, bodyParts[bodyParts.Count - 1].rotation) as GameObject).transform;

        newPart.SetParent(transform);

        bodyParts.Add(newPart);

    }

    public void IncreaseMass() {
        MASS += 10f;

        if (MASS % newBodyThreshold == 0) {
            AddBody();

        }

        if (MASS % scaleThreshold == 0) {
            bodyParts[0].localScale += new Vector3(0.1f, 0.1f, 0.1f);

            minimumDistanceBetweenParts += 0.17f;

        }

    }

}
