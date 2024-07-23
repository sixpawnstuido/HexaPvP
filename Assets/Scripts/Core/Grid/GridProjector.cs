using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridProjector : MonoBehaviour
{
    private LayerMask _gridProjectorLayerMask = 1 << 6;

    [SerializeField] private Vector3 _fingerDistance;

    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private Color32 _defaultColor;
    [SerializeField] private Color32 _mouseOverColor;
    //private void Update()
    //{
    //    if (!HexagonMovement.isMoving) return;
    //    if (Input.GetKey(KeyCode.Mouse0))
    //    {
    //        var gridProjector = RaycastManager.SendRayFromCameraToMousePos<GridProjector>(_gridProjectorLayerMask,true,_fingerDistance);
    //        if (gridProjector)
    //        {
    //            _meshRenderer.material.color= _mouseOverColor;
    //        }
    //        else
    //        {
    //            _meshRenderer.material.color = _defaultColor;
    //        }
    //    }
    //    if (Input.GetKeyUp(KeyCode.Mouse0))
    //    {
    //        _meshRenderer.material.color = _defaultColor;
    //    }
    //}
}
