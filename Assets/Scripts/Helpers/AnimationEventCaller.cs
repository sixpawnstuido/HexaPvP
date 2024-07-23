using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AnimationEventCaller : MonoBehaviour
{
    public List<UnityEvent> events = new List<UnityEvent>();

    public void InvokeEvents(int eventNumber)
    {
        events[eventNumber].Invoke();
    }
}
