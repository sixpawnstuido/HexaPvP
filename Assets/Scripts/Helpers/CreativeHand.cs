using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CreativeHand : MonoBehaviour
{
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-Camera.main.transform.position.z));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            transform.DOScale(1.3f, .1f).SetEase(Ease.OutBack);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            transform.DOScale(1.5f, .1f).SetEase(Ease.OutBack);
        }
    }
}
