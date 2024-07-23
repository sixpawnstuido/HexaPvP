
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extensions
{
    public static void Logger(object message, LogTypes logType=LogTypes.LOG)
    {
        switch (logType)
        {
            case LogTypes.LOG:
                Debug.Log(message);
                break;
            case LogTypes.WARNING:
                Debug.LogWarning(message);
                break;
            case LogTypes.ERROR:
                Debug.LogError(message);
                break;
            default:
                break;
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult r in results)
            if (r.gameObject.GetComponent<RectTransform>() != null)
                return true;
        return false;
    }

    public static void AnimationStateChanger(Animator animator,bool isAnimationStateBool,string animationState,bool animationBoolState=true)
    {
        if (isAnimationStateBool) animator.SetBool(animationState, animationBoolState);
        else animator.SetTrigger(animationState);
    }

    public static void TouchBlock(bool blockTouch)
    {
        //if(blockTouch)
        //{
        //    EventManager.UIEvents.CanvasSetter(CanvasTypes.TouchBlockerCanvas,true);
        //    EventManager.UIEvents.CanvasSetter(CanvasTypes.JoystickCanvas,false);
        //}
        //else
        //{
        //    EventManager.UIEvents.CanvasSetter(CanvasTypes.TouchBlockerCanvas, false);
        //    EventManager.UIEvents.CanvasSetter(CanvasTypes.JoystickCanvas, true);
        //}
    }

    public static void ChangeCameraPos(Transform cameraTarget,Vector3 destination,float desiredStandbyTime,float desiredArrivelTime)
    {
        Vector3 tempPos= cameraTarget.position;
        TouchBlock(true);
        Sequence myS = DOTween.Sequence();
        myS.AppendInterval(desiredStandbyTime / 2);
        myS.Append(cameraTarget.transform.DOMove(destination, desiredArrivelTime));
        myS.AppendInterval(desiredStandbyTime);
        myS.Append(cameraTarget.transform.DOMove(tempPos, desiredArrivelTime));
        myS.AppendCallback(() => TouchBlock(false));
    }

    public class WeightedRandomSelector<T>
    {
        private List<T> elements;
        private List<float> weights;

        public WeightedRandomSelector(List<T> elements)
        {
            this.elements = elements;

            // Calculate weights inversely proportional to the indexes
            weights = new List<float>();
            for (int i = 0; i < elements.Count; i++)
            {
                weights.Add(1f / (i + 1));
            }
        }

        public T Select()
        {
            float totalWeight = 0f;

            // Calculate the total weight
            foreach (float weight in weights)
            {
                totalWeight += weight;
            }

            // Generate a random value between 0 and the total weight
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);

            // Find the selected element based on the random value and weights
            float cumulativeWeight = 0f;
            for (int i = 0; i < elements.Count; i++)
            {
                cumulativeWeight += weights[i];
                if (randomValue <= cumulativeWeight)
                {
                    return elements[i];
                }
            }

            // This should not happen, but just in case
            return elements[elements.Count - 1];
        }
    }


}
