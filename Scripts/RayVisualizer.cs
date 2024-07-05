using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayVisualizer : MonoBehaviour
{
    public LineRenderer lineRenderer; // Reference to the Line Renderer component
    public float rayDistance = 10f; // Maximum distance for the ray
    public LayerMask layerMask; // Layer mask to specify which layers the ray should interact with

    void Start() {
        // Initialize the line renderer positions
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
    }

    void Update() {
        // Start point of the ray is the player's position
        Vector3 startPoint = transform.position;

        // Direction of the ray is the player's forward direction
        Vector3 direction = transform.forward;

        // End point of the ray, initially set to the maximum distance
        Vector3 endPoint = startPoint + direction * rayDistance;

        // Perform a raycast to check for collisions
        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction, out hit, rayDistance, layerMask)) {
            // If the ray hits an object, set the end point to the hit point
            endPoint = hit.point;
        }

        // Set the positions of the line renderer
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
