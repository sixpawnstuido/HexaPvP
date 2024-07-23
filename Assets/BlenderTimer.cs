using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlenderTimer : MonoBehaviour
{
    [SerializeField] private GameObject timerHolder;

    [SerializeField] private TextMeshProUGUI timerText;

    private BlenderController _blenderController;

    private void Awake()
    {
        _blenderController = GetComponentInParent<BlenderController>();
    }

    private void OnEnable()
    {
        OpenTimer();
    }

    public void OpenTimer()
    {
        timerHolder.SetActive(true);
        StartCoroutine(TimerCor());
    }

    private IEnumerator TimerCor()
    {
        float time = 90;
        while (time>0)
        {

            time -= 1;

            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1);
    
        }

        timerHolder.SetActive(false);
        _blenderController.CloseThirdBlender();
    }
}
