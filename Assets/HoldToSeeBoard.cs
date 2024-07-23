using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldToSeeBoard : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private Tween alphaTween;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (alphaTween != null) alphaTween.Kill();

        float startVal = _canvasGroup.alpha;

        alphaTween =  DOVirtual.Float(startVal, 0, .5f, (v) => _canvasGroup.alpha = v);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (alphaTween != null) alphaTween.Kill();

        float startVal = _canvasGroup.alpha;

        alphaTween = DOVirtual.Float(startVal, 1, .5f, (v) => _canvasGroup.alpha = v);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
