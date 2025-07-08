using System.Collections.Generic;
using UnityEngine;

internal class Exploder : MonoBehaviour
{
    [Header("Force Settings")]
    [SerializeField, Min(0f)] private float _baseExplosionRadius = 5f;
    [SerializeField, Min(0f)] private float _baseExplosionForce = 10f;
    [SerializeField, Min(0f)] private float _upwardsModifier = 0f;

    [Header("Size Scaling")]
    [SerializeField] private bool _scaleForceWithSize = false;
    [SerializeField, Min(0.1f)] private float _minSizeMultiplier = 0.5f;
    [SerializeField, Min(1f)] private float _maxSizeMultiplier = 2f;
    [SerializeField] private float _noScalingMultiplier = 1f;

    [Header("Inverse Scaling (for Explode without fragments)")]
    [SerializeField] private bool _useInverseScaling = true;
    [SerializeField, Min(0f)] private float _inverseBaseExplosionRadius = 6f; 
    [SerializeField, Min(0.1f)] private float _inverseScalingFactor = 1f;

    [Header("Gizmos Settings")]
    [SerializeField] private bool _showBaseRadius = true;
    [SerializeField] private Color _baseRadiusColor = new Color(1f, 0.3f, 0f, 0.5f);
    [SerializeField] private bool _showInverseRadius = true;
    [SerializeField] private Color _inverseRadiusColor = new Color(0.3f, 0.8f, 1f, 0.5f);

    private float _inverseBaseValue = 1f;
    private float _minObjectSize = 0.001f;

    public void Explode(Vector3 explosionPosition, Vector3 objectSize, List<Rigidbody> fragmentsRigidbodies)
    {
        if (fragmentsRigidbodies == null || fragmentsRigidbodies.Count == 0)
            return;

        float sizeMultiplier = GetFragmentsSizeMultiplier(objectSize);

        ApplyExplosionForce(explosionPosition, sizeMultiplier, fragmentsRigidbodies);
    }

    public void Explode(Vector3 explosionPosition, Vector3 objectSize)
    {
        float sizeMultiplier = GetSizeMultiplierForEnvironment(objectSize);

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, _inverseBaseExplosionRadius * sizeMultiplier);
        List<Rigidbody> rigidbodies = ExtractRigidbodiesFromColliders(colliders);

        ApplyExplosionForce(explosionPosition, sizeMultiplier, rigidbodies);
    }

    private float GetFragmentsSizeMultiplier(Vector3 objectSize)
    {
        if (_scaleForceWithSize == false)
            return _noScalingMultiplier;

        return Mathf.Clamp(objectSize.magnitude, _minSizeMultiplier, _maxSizeMultiplier);
    }

    private float GetSizeMultiplierForEnvironment(Vector3 objectSize)
    {
        if (_useInverseScaling == false)
            return _noScalingMultiplier;

        float clampedSize = Mathf.Max(objectSize.magnitude, _minObjectSize);
        float inverRatio = _inverseBaseValue / clampedSize;

        return Mathf.Clamp(inverRatio * _inverseScalingFactor, _minSizeMultiplier, _maxSizeMultiplier);
    }

    private List<Rigidbody> ExtractRigidbodiesFromColliders(Collider[] colliders)
    {
        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        foreach (var collider in colliders)
        {
            if (collider.attachedRigidbody != null)
            {
                rigidbodies.Add(collider.attachedRigidbody);
            }
        }

        return rigidbodies;
    }

    private void ApplyExplosionForce(Vector3 explosionPosition, float sizeMultiplier, List<Rigidbody> rigidbodies)
    {
        float actualForce = _baseExplosionForce * sizeMultiplier;
        float actualRadius = _baseExplosionRadius * sizeMultiplier;

        foreach (var rigidbody in rigidbodies)
        {
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(
                    actualForce,
                    explosionPosition,
                    actualRadius,
                    _upwardsModifier,
                    ForceMode.Impulse
                    );
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_showBaseRadius)
        {
            Gizmos.color = _baseRadiusColor;
            Gizmos.DrawSphere(transform.position, _baseExplosionRadius);
        }

        if (_showInverseRadius)
        {
            Gizmos.color = _inverseRadiusColor;
            Gizmos.DrawWireSphere(transform.position, _inverseBaseExplosionRadius);
        }
    }
}