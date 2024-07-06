using UnityEngine;

public class LongThinLine : MonoBehaviour {
    public float maxDistance = 1000f;
    public float lineWidth = 0.01f;
    public LayerMask hitLayers;
    public float lineElevation = 0.1f;

    private LineRenderer lineRenderer;

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        SetupLineRenderer();

        // Initially disable the line renderer
        lineRenderer.enabled = false;
    }

    void SetupLineRenderer() {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = Color.white;
        lineMaterial.renderQueue = 3000;
        lineRenderer.material = lineMaterial;

        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.useWorldSpace = true;
    }

    void Update() {
        // Check if right mouse button is being held down
        if (Input.GetMouseButton(1))  // 0 is left, 1 is right, 2 is middle
        {
            lineRenderer.enabled = true;
            UpdateLinePosition();
        } else {
            lineRenderer.enabled = false;
        }
    }

    void UpdateLinePosition() {
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