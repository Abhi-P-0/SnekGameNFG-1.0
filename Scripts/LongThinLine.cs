using UnityEngine;

public class LongThinLine : MonoBehaviour {
    public float maxDistance = 1000f;
    public float lineWidth = 0.01f;
    public LayerMask hitLayers;
    public float lineElevation = 0.1f;

    private LineRenderer lineRenderer;
    private Color aiColor = Color.red;
    private Color defaultColor = Color.white;
    private MaterialPropertyBlock propertyBlock;
    private int layerMask;

    private NewPlayerInput newPlayerInput;

    void Start() {
        newPlayerInput = new NewPlayerInput();
        newPlayerInput.Enable();

        layerMask = hitLayers.value & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        propertyBlock = new MaterialPropertyBlock();

        SetupLineRenderer();

        // Initially disable the line renderer
        lineRenderer.enabled = false;
    }

    private void OnDisable() {
        newPlayerInput?.Disable();
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
        //if (Input.GetMouseButton(1))  // 0 is left, 1 is right, 2 is middle
        if (newPlayerInput.PlayerTouch.Line.IsPressed())
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

        // Define the layer mask to ignore the Ignore Raycast layer
        //int layerMask = hitLayers.value & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        lineRenderer.GetPropertyBlock(propertyBlock);

        if (Physics.Raycast(startPoint, direction, out hit, maxDistance)) {
            //Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("AI")) {
                propertyBlock.SetColor("_Color", aiColor);

            } else {
                propertyBlock.SetColor("_Color", defaultColor);

            }

            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, hit.point + (Vector3.up * lineElevation));

        } else {
            propertyBlock.SetColor("_Color", defaultColor);

            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, startPoint + direction * maxDistance);

        }
        
        lineRenderer.SetPropertyBlock(propertyBlock);

    }
}