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

    public void StartAnimCor(float offset)
    {
        StartCoroutine(AnimCorr(offset));
    }

    private IEnumerator AnimCorr(float offset)
    {
      //  LoadingTextAnim();

        holder.SetActive(true);

        DOVirtual.Float(0, 1, offset, (v) => slicedImage.fillAmount = v);

        yield return new WaitForSeconds(offset);

        holder.SetActive(false);
       // InGameLoading.Instance.OpenHolder();
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
