using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MinusText : MonoBehaviour
{
    public TextMeshProUGUI Text => _text;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void Init(int clearAmount, Vector3 pos,int comboStage=0)
    {
        StartCoroutine(InitCor());

        IEnumerator InitCor()
        {
            int comboMultiplier = comboStage;
            if (comboMultiplier > 1)
            {
                _text.SetText($"-{clearAmount}x{comboMultiplier}");
            }
            else
            {
              _text.SetText($"-{clearAmount}");
            }
            
            transform.position = pos;
            _text.DOFade(1, 0f);

            transform
                .DOMoveY(1, .6f)
                .SetRelative()
                .SetEase(Ease.Flash);
            yield return new WaitForSeconds(0.3f);
            _text.DOFade(0, .3f);
            yield return new WaitForSeconds(0.3f);
            gameObject.SetActive(false);
        }
    }
}