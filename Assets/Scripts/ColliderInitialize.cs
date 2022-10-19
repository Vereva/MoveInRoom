using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInitialize : MonoBehaviour
{
    private void Awake()
    {
        BoxCollider boxcollider = gameObject.AddComponent<BoxCollider>();
        boxcollider.size = GetComponentsInChildren<Transform>()[1].localScale;
    }
}
