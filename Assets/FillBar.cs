using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] private GameObject holder;

    [SerializeField] private SlicedFilledImage slicedImage;

    private float fillAmount;

    [SerializeField] private Image _fillbarFruitImage;

    public void SetActiveBar(bool v,Color32 color)
    {
        if (v && holder.activeInHierarchy) return;
        holder.SetActive(v);
        slicedImage.color = color;
        fillAmount = 0;
    }

    public void FillBarStep(float stepCount)
    {
        fillAmount = .25f*stepCount;
        fillAmount = Mathf.Clamp01(fillAmount);
        slicedImage.fillAmount = fillAmount;
    }

    public void FillBarColor(Color32 color) => slicedImage.color = color;

    public void SetFillbarFruit(Sprite sprite, bool state)
    {
        if (state)
        {
            _fillbarFruitImage.transform
                .DOScale(Vector3.one, .2f)
                .SetEase(Ease.OutBack);
            _fillbarFruitImage.sprite = sprite;
        }
        else
        {
            _fillbarFruitImage.transform
                .DOScale(Vector3.zero, .2f)
                .SetEase(Ease.OutBack);
        }
    }
}
