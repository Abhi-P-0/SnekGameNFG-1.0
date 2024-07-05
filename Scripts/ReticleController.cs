using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class ReticleController : MonoBehaviour {
    public Image reticle;
    public Transform player;
    public CinemachineFreeLook cinemachineFreeLook;
    public float reticleDistance = 10f;
    

    private Camera mainCamera;

    void Start() {
        // Get the camera from the CinemachineFreeLook component
        mainCamera = cinemachineFreeLook.VirtualCameraGameObject.GetComponent<Camera>();

        
        
    }

    void Update() {
        // Calculate the forward position of the player
        Vector3 playerForward = player.position + player.forward * reticleDistance;

        // Project the player's forward position onto the screen
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(playerForward);

        // Set the reticle position to the projected screen point
        reticle.transform.position = screenPoint;

    }
}
