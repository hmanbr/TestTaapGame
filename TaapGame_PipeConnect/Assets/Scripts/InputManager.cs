using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera camera;

    [SerializeField] private GridManager gridManager;

    void Awake()
    {
        camera = Camera.main;
    }

    void Update()
    {
        //Unity 6 Input Manager
        if (Pointer.current == null) return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();

            Ray ray = camera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // Check for PipeTile component
                PipeTile pipe = hit.collider.GetComponent<PipeTile>();

                if (pipe != null)
                {
                    gridManager.TryRotatePipe(pipe);
                    //pipe.DebugOpenDirections();
                }
            }
        }
    }
}
