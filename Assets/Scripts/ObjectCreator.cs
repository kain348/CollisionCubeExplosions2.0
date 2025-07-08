using System.Collections.Generic;
using UnityEngine;

internal class ObjectCreator : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField, Min(2)] private int _minCount = 2;
    [SerializeField, Min(2)] private int _maxCount = 4;
    [SerializeField, Min(1)] private float _sizeReduction = 2f;
    [SerializeField, Min(0.1f)] private float _spawnRadius = 1f;
    [SerializeField] private int _splitChanceDivider = 2;

    [Header("Resources")]
    [SerializeField] private ClickableObject _objectPrefab;
    [SerializeField] private List<Material> _materials = new List<Material>();

    public List<ClickableObject> CreateFragments(ClickableObject original)
    {
        if (!_objectPrefab)
        {
            Debug.LogError("Prefab not assigned!", this);

            return new List<ClickableObject>();
        }
               
        return CreateNewFragments(original);        
    }

    private List<ClickableObject> CreateNewFragments(ClickableObject original)
    {
        List<ClickableObject> fragments = new List<ClickableObject>();

        int count = Random.Range(_minCount, _maxCount + 1);
        Vector3 newSize = original.Size / _sizeReduction;
        int newSplitChance = original.SplitChance / _splitChanceDivider;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = original.transform.position + Random.insideUnitSphere * _spawnRadius;

            var fragment = CreateFragment(spawnPosition, newSize, original.Material, newSplitChance);

            if (fragment.TryGetComponent<ClickableObject>(out var newObject))
            {
                fragments.Add(newObject);
            }
        }

        return fragments;
    }

    private ClickableObject CreateFragment(Vector3 position, Vector3 size, Material originalMaterial, int splitChance)
    {
        ClickableObject newObject = Instantiate(
            _objectPrefab,
            position,
            Quaternion.identity
        );

        newObject.Initialize(
            position: position,
            size: size,
            material: GetUniqueMaterial(originalMaterial),
            splitChance: splitChance
        );

        return newObject;
    }

    private Material GetUniqueMaterial(Material original)
    {
        if (_materials.Count == 0)
            return CreateRandomMaterial();

        var available = _materials.FindAll(material => !MaterialsEqual(material, original));

        return available.Count > 0
            ? available[Random.Range(0, available.Count)]
            : _materials[Random.Range(0, _materials.Count)];
    }

    private bool MaterialsEqual(Material newMaterial, Material originalMaterial)
    {
        return newMaterial && originalMaterial && newMaterial.color == originalMaterial.color && newMaterial.mainTexture == originalMaterial.mainTexture;
    }

    private Material CreateRandomMaterial()
    {
        return new Material(Shader.Find("Standard")) { color = Random.ColorHSV() };
    }
}