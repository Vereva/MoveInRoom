using System;
using UnityEngine;

public class InputEvent : MonoBehaviour
{
    public static InputEvent instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnStartTap?.Invoke();
        }
        else if (Input.GetMouseButton(0))
        {
            OnTapped?.Invoke();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnEndTap?.Invoke();
        }
    }

    public event Action OnStartTap;
    public event Action OnTapped;
    public event Action OnEndTap;
}
