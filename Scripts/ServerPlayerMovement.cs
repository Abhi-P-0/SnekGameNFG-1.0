using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject headPrefab;
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();


    public CharacterController characterController;

    private NewPlayerInput playerInput; 

    void Start()
    {
        playerInput = new();

        playerInput.Enable();

        Transform head = (Instantiate(headPrefab, new Vector3(0, 0, 0), Quaternion.identity)).transform;

        head.SetParent(transform);

        playerTransform = transform.GetChild(0);

        bodyParts.Add(head);

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = playerInput.Player.Movement.ReadValue<Vector2>();

        if (IsServer && IsLocalPlayer) {
            Move(moveInput);

        } else if (IsClient && IsLocalPlayer) {
            MoveServerRPC(moveInput);

        }

    }

    private void Move(Vector2 _input) {
        Vector3 clacMove = _input.x * playerTransform.right + _input.y * playerTransform.forward;

        characterController.Move(clacMove * playerSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void MoveServerRPC(Vector2 _input) {
        Move(_input);

    }

}
