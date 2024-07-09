using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour {
    public PlayerSnakeMovement playerMovement;
    private Label massLabel;

    private void OnEnable() {
        Debug.Log(GameObject.FindGameObjectsWithTag("Player").Length);
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        massLabel = root.Q<Label>("MassLabel");

        //playerMovement = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    private void Update() {
        if (playerMovement != null && massLabel != null) {
            massLabel.text = $"MASS: {playerMovement.GetMass()}";
        }
    }
}