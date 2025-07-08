using UnityEngine;

internal class RaycastClickHandler : MonoBehaviour
{
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private Camera mainCamera;

    public event System.Action<ClickableObject> ClickableObjectClicked;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayer))
            {
                if (hit.collider.TryGetComponent<ClickableObject>(out var clickable))
                {
                    ClickableObjectClicked?.Invoke(clickable);
                }
            }
        }
    }
}