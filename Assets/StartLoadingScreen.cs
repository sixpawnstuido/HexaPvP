using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartLoadingScreen : MonoBehaviour
{
    public static bool justOnce;

    public static StartLoadingScreen Instance;

    public GameObject holder;

    public SlicedFilledImage slicedImage;

    [SerializeField] private TextMeshProUGUI _loadingText;

    private void Awake()
    {
        Instance = this;


    }

    private void Start()
    {
        if (!justOnce)
        {
            justOnce = true;

            StartCoroutine(AnimCorr());
        }
        LoadingTextAnim();
    }

    private IEnumerator AnimCorr()
    {
        holder.SetActive(true);

        DOVirtual.Float(0, 1, 4, (v) => slicedImage.fillAmount = v);

        yield return new WaitForSeconds(4);

       // JuiceTargetUIController.Instance.TargetUILevelStartAnimation();

        holder.SetActive(false);
    }

    public void LoadingTextAnim()
    {
        var animator = new DOTweenTMPAnimator(_loadingText);
        for (var i = 0; i < animator.textInfo.characterCount; i++)
        {
            if (!animator.textInfo.characterInfo[i].isVisible) continue;
            var currCharOffset = animator.GetCharOffset(i);
            animator
                .DOOffsetChar(i, currCharOffset + new Vector3(0, 10, 0), .8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(i * 0.1f);
        }
    }


}
