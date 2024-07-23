using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintTutorialAnimation : MonoBehaviour
{
    [SerializeField] private GameObject realHint;
    [SerializeField] private GameObject dummyHint;

    [SerializeField] private Button claimButton;

    [SerializeField] private Transform targetPos;

    [SerializeField] private Transform popup;

    [SerializeField] private Image blackBG;

    [SerializeField] private HintBase hint;

    void Start()
    {
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    private void OnClaimClicked()
    {
        StartCoroutine(ClaimCor());
    }

    private IEnumerator ClaimCor()
    {
        realHint.SetActive(false);

        dummyHint.SetActive(true);

        popup.transform.DOScale(Vector3.zero, .2f);

        blackBG.DOFade(0, .4f);

        yield return new WaitForSeconds(.4f);

        dummyHint.transform.DOMove(targetPos.position, .5f).SetEase(Ease.InBack);
        dummyHint.transform.DOScale(1.84f*Vector3.one, .5f);

        yield return new WaitForSeconds(.5f);

        hint.HintScalePunch();

        hint.UnlockHint();

        gameObject.SetActive(false);


    }
}
