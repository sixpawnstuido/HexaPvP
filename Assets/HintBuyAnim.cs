using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBuyAnim : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Transform main;

    [SerializeField] private HintBase hint;

    Vector3 firstPos;


    private void Awake()
    {
        firstPos = main.transform.position;   
    }

    public void PlayAnim()
    {
        AudioManager.Instance.Play(AudioManager.AudioEnums.CoinPop);
        StartCoroutine(HintAnimCor());
    }

    private IEnumerator HintAnimCor()
    {
        main.gameObject.SetActive(true);

        main.transform.localScale = Vector3.zero;

        main.transform.position = firstPos;

        main.transform.DOScale(0.27522f * Vector3.one, .3f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(.3f);

       

        main.transform.DOMove(target.position, .5f).SetEase(Ease.InBack);

        yield return new WaitForSeconds(.2f);

        main.transform.DOScale(.11f * Vector3.one, .3f);

        yield return new WaitForSeconds(.3f);

        AudioManager.Instance.Play(AudioManager.AudioEnums.Pop);

        main.gameObject.SetActive(false);

        hint.HintScalePunch();

        hint.EarnHint();
    }
}
