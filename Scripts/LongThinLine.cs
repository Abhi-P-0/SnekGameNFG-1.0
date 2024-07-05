using UnityEngine;

public class LongThinLine : MonoBehaviour {
    public float maxDistance = 1000f;
    public float lineWidth = 0.01f;
    public LayerMask hitLayers;
    public float lineElevation = 0.1f; // Slight elevation above surfaces

    private LineRenderer lineRenderer;

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        // Create a new material with a shader that ignores depth testing
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = Color.red; // Set your desired color
        lineMaterial.renderQueue = 3000; // Set to render after most transparent objects
        lineRenderer.material = lineMaterial;

        // Disable shadow casting and receiving
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;

        // Set to world space
        lineRenderer.useWorldSpace = true;
    }

    void Update() {
        Vector3 startPoint = transform.position + (Vector3.up * lineElevation);
        Vector3 direction = transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction, out hit, maxDistance, hitLayers)) {
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, hit.point + (Vector3.up * lineElevation));
        } else {
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, startPoint + direction * maxDistance);
        }
    }
}