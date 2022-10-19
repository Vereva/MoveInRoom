using UnityEngine;

public class RayCastMover : MonoBehaviour
{
    public Movable _activeObject;

    private void Start()
    {
        InputEvent.instance.OnTapped += MoveObjectByRayCast;
        Selector.instance.OnSelectObject += SetActiveObject;
        Selector.instance.OnDeselectObject += DeselectActiveObject;
    }
    private void OnDestroy()
    {
        InputEvent.instance.OnTapped -= MoveObjectByRayCast;
        Selector.instance.OnSelectObject -= SetActiveObject;
        Selector.instance.OnDeselectObject -= DeselectActiveObject;
    }
    private void MoveObjectByRayCast()
    {
        if(_activeObject == null)
        {
            return;
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            _activeObject.MoveOnCollider(hit);
        }
    }

    private void SetActiveObject(GameObject selectedObject)
    {
        DeselectActiveObject();
        if (selectedObject.TryGetComponent<Movable>(out Movable movable))
        {
            _activeObject = movable;
            _activeObject.OnSelect();
        }
    }

    private void DeselectActiveObject()
    {
        if(_activeObject == null)
        {
            return;
        }

        _activeObject.OnDeselect();
        _activeObject = null;
    }
}
