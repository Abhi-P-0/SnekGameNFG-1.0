using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private float maxLineLength = 100f;
    [SerializeField] private LayerMask hitLayers;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        player = transform.GetChild(0);

        freeLookCamera = transform.GetChild(1).GetComponent<CinemachineFreeLook>();

        lineRenderer.positionCount = 2;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 caneraForward = freeLookCamera.State.FinalOrientation * Vector3.forward;
        Ray ray = new Ray(player.position, caneraForward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxLineLength, hitLayers)) {
            lineRenderer.SetPosition(0, player.position);
            lineRenderer.SetPosition(1, hit.point);

        } else {
            lineRenderer.SetPosition(0, player.position);
            lineRenderer.SetPosition(1, player.position + caneraForward * maxLineLength);

        }
        
    }
}
