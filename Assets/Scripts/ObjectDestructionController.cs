using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class ObjectDestructionController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Separator _separator;
    [SerializeField] private ObjectCreator _objectCreator;
    [SerializeField] private RaycastClickHandler _clickHandler;
    [SerializeField] private Exploder _exploder;

    private void OnEnable()
    {
        if (_clickHandler != null)
            _clickHandler.ClickableObjectClicked += HandleObjectClick;
    }

    private void OnDisable()
    {
        if (_clickHandler != null)
            _clickHandler.ClickableObjectClicked -= HandleObjectClick;
    }

    private void HandleObjectClick(ClickableObject clickedObject)
    {
        if (clickedObject == null) return;

        if (_separator.ShouldSplit(clickedObject.SplitChance))
        {
            List<ClickableObject> newObject = _objectCreator.CreateFragments(clickedObject);

            List<Rigidbody> rigidbodies = new List<Rigidbody>();
            rigidbodies = TryGetRigidbody(newObject);

            if (rigidbodies != null)
            {
                _exploder.Explode(clickedObject.Position, clickedObject.Size, rigidbodies);
            }
        }
        else
        {
            _exploder.Explode(clickedObject.Position, clickedObject.Size);
        }

        Destroy(clickedObject.gameObject);
    }

    private List<Rigidbody> TryGetRigidbody(List<ClickableObject> clickableObjects)
    {
        return clickableObjects
            .Select(clickable => clickable.ObjectRigidbody)
            .Where(rigidbody => rigidbody != null)
            .ToList();
    }
}