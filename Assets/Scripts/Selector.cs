using System;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public static Selector instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        InputEvent.instance.OnStartTap += SelectObjectByRayCast;
        InputEvent.instance.OnEndTap += DeselectObject;
    }
    private void OnDestroy()
    {
        InputEvent.instance.OnStartTap -= SelectObjectByRayCast;
        InputEvent.instance.OnEndTap -= DeselectObject;
    }
    private void SelectObjectByRayCast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            SelectObject(hit.collider.gameObject);
        }
    }

    public event Action<GameObject> OnSelectObject;

    private void SelectObject(GameObject value)
    {
        OnSelectObject?.Invoke(value);
    }
    public event Action OnDeselectObject;

    private void DeselectObject()
    {
        OnDeselectObject?.Invoke();
    }
}
