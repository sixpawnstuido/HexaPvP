using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlenderLock : MonoBehaviour
{
    private BlenderElement _blenderElement;

    private void Awake()
    {
        _blenderElement = GetComponentInParent<BlenderElement>();
    }

    private void OnMouseDown()
    {
        _blenderElement.OpenBlender();
    }
}
