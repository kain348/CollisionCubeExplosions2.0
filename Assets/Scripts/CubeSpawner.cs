using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class CubeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField, Min(2)] private int _minCount = 2;
    [SerializeField, Min(2)] private int _maxCount = 4;
    [SerializeField, Min(1)] private float _sizeReduction = 2f;
    [SerializeField] private int _splitChanceDivider = 2;

    [Header("Resources")]
    [SerializeField] private RaycastClickHandler _clickHandler;
    [SerializeField] private Exploder _exploder;
    [SerializeField] private ClickableCube _cubePrefab;
    [SerializeField] private List<Material> _materials = new List<Material>();

    private float _spawnRadius = 1f;

    private void OnEnable()
    {
        _clickHandler.ClickableCubeClicked += HandleObjectClick;
    }

    private void OnDisable()
    {
        _clickHandler.ClickableCubeClicked -= HandleObjectClick;
    }

    private void HandleObjectClick(ClickableCube clickedCube)
    {
        if (clickedCube == null) 
            return;

        if (ShouldSplit(clickedCube.SplitChance))
        {
            List<ClickableCube> clickableCube = CreateNewFragments(clickedCube);

            List<Rigidbody> rigidbodies = TryGetRigidbodies(clickableCube);

            if(rigidbodies != null)
            {
                _exploder.Explode(clickedCube.Position, clickedCube.Size, rigidbodies);
            }
        }
        else
        {
            _exploder.Explode(clickedCube.Position, clickedCube.Size);
        }

        Destroy(clickedCube.gameObject);
    }

    private List<Rigidbody> TryGetRigidbodies(List<ClickableCube> clickableCube)
    {
        return clickableCube
            .Select(clickable => clickable.ObjectRigidbody)
            .Where(rigidbody => rigidbody != null)
            .ToList();
    }

    private List<ClickableCube> CreateNewFragments(ClickableCube original)
    {
        List<ClickableCube> fragments = new List<ClickableCube>();

        int count = Random.Range(_minCount, _maxCount + 1);
        Vector3 newSize = original.Size / _sizeReduction;
        int newSplitChance = original.SplitChance / _splitChanceDivider;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = original.transform.position + Random.insideUnitSphere * _spawnRadius;

            var fragment = CreateFragment(spawnPosition, newSize, original.Material, newSplitChance);

            if (fragment.TryGetComponent<ClickableCube>(out var newObject))
            {
                fragments.Add(newObject);
            }
        }

        return fragments;
    }

    private ClickableCube CreateFragment(Vector3 position, Vector3 size, Material originalMaterial, int splitChance)
    {
        ClickableCube newObject = Instantiate(
            _cubePrefab,
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

    private bool ShouldSplit(int chanceToSplit)
    {
        int minRandom = 1;
        int maxRandom = 100;

        Debug.Log("split chance - " + chanceToSplit);
        return chanceToSplit >= Random.Range(minRandom, maxRandom + 1);
    }
}