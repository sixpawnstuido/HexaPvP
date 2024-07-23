using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class RaycastManager
{
    public static T SendRayFromCameraToMousePos<T>(LayerMask layerMask, bool isOverMouse = false,Vector3 offset=(default))
    {
        if (Meta2DManager.MetaOpened) return default(T);

        Camera camera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offsetVector = camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mousePosition.z + 10)) + offset;
        Ray ray = camera.ScreenPointToRay(isOverMouse ? camera.WorldToScreenPoint(offsetVector) : mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.TryGetComponent(out T type))
            {
                return type;
            }
            else return default(T);
        }
        else return default(T);
    }
    public static T SendRayToGivenPos<T>(Vector3 startPos, Vector3 direction, float rayRange, LayerMask layerMask)
    {
        Ray ray = new Ray(startPos, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange, layerMask))
        {
            if (hit.collider.TryGetComponent(out T type))
            {
                return type;
            }
            else return default(T);
        }
        else return default(T);
    }

}


