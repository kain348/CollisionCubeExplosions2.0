using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]

public class ClickableObject : MonoBehaviour
{
    [Header("Prefab Defaults (For Editor Only)")]
    [SerializeField] private Vector3 _prefabDefaultPosition = Vector3.zero;
    [SerializeField] private Vector3 _prefabDefaultSize = Vector3.one;
    [SerializeField] private Material _prefabDefaultMaterial;
    [SerializeField, Range(0, 100)] private int _prefabSplitChance = 100;

    private Renderer _cachedRenderer;
    private Rigidbody _cachedRigidbody;
    private Collider _cachedCollider;

    public Vector3 Position
    {
        get => transform.position;
        private set => transform.position = value;
    }

    public Vector3 Size
    {
        get => transform.localScale;
        private set => transform.localScale = value;
    }

    public Material Material
    {
        get => _cachedRenderer.material;
        private set => _cachedRenderer.material = value;
    }

    public Collider ObjectCollider
    {
        get => _cachedCollider;
        private set => _cachedCollider = value;
    }

    public Rigidbody ObjectRigidbody
    {
        get => _cachedRigidbody;
        private set => _cachedRigidbody = value;
    }

    public int SplitChance { get; private set; }

    public event System.Action<ClickableObject> OnObjectClicked;

    private void Awake()
    {
        _cachedRenderer = GetComponent<Renderer>();
        _cachedRigidbody = GetComponent<Rigidbody>();
        _cachedCollider = GetComponent<Collider>();

        SplitChance = _prefabSplitChance;
    }

    public void Initialize(Vector3 position, Vector3 size, Material material, int splitChance)
    {
        Position = position;
        Size = size;
        Material = material;
        SplitChance = splitChance;
    }
            
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {            
            transform.position = _prefabDefaultPosition;
            transform.localScale = _prefabDefaultSize;

            if (_prefabDefaultMaterial != null)
            {
                GetComponent<Renderer>().material = _prefabDefaultMaterial;
            }
        }
    }
}