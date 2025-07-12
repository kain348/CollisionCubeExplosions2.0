using UnityEngine;

internal class RaycastClickHandler : MonoBehaviour
{
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private Camera mainCamera;

    const int CommandMouseLeftKey = 0;

    public event System.Action<ClickableCube> ClickableCubeClicked;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(CommandMouseLeftKey))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayer))
            {
                if (hit.collider.TryGetComponent(out ClickableCube clickable))
                {
                    ClickableCubeClicked?.Invoke(clickable);
                }
            }
        }
    }
}