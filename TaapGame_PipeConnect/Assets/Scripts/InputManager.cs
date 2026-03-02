using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera camera;

    void Awake()
    {
        camera = Camera.main;
    }

    void Update()
    {
        // Left mouse click (Unity 6 Input Manager)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                GameObject clicked = hit.collider.gameObject;

                // Check for PipeTile component
                PipeTile pipe = clicked.GetComponent<PipeTile>();

                if (pipe != null)
                {
                    Debug.Log("Clicked PipeTile: " + clicked.name);

                    pipe.Rotate();
                    pipe.DebugOpenDirections();
                }
                else
                {
                    Debug.Log("Clicked non-pipe object: " + clicked.name);
                }
            }
        }
    }
}
