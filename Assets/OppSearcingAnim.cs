using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class OppSearcingAnim : MonoBehaviour
{
    [SerializeField] private GameObject magnifier;
    
    [SerializeField] private GameObject avatar;
    [SerializeField] private GameObject name;
    [SerializeField] private GameObject flag;
    [SerializeField] private GameObject player;

    [SerializeField] private Animator oppAnimator;
    [SerializeField] private Animator holderAnimator;

    [SerializeField] private TextMeshProUGUI searchingForAnOppText;
    [SerializeField] private TextMeshProUGUI vs;


    [SerializeField] private Image bgImage;
    [SerializeField] private Color32 greyColor;
    [SerializeField] private Color32 originalColor;

    private void OnEnable()
    {
        Reset();
        StartSearchAnim();
    }

    public void StartSearchAnim()
    {
        StartCoroutine(StartSearchAnimCor());
        IEnumerator StartSearchAnimCor()
        {
            oppAnimator.SetTrigger("Search");
            yield return new WaitForSeconds(2);
            OpenAvatar();
        }
    }

    public void OpenAvatar()
    {
        StartCoroutine(OpenAvatarCor());
        IEnumerator OpenAvatarCor()
        {
            AudioManager.Instance.Play(AudioManager.AudioEnums.OppFound);
            bgImage.color = originalColor;
            avatar.SetActive(true);
            avatar.transform.DOPunchScale(new Vector3(-.15f, 0.15f, 0), .3f, 10) .SetId(transform.GetHashCode());
            magnifier.SetActive(false);
            searchingForAnOppText.SetText("READY");
            yield return new WaitForSeconds(1);
            searchingForAnOppText.SetText("3");
            AudioManager.Instance.Play(AudioManager.AudioEnums.Three);
            yield return new WaitForSeconds(0.5f);
            searchingForAnOppText.SetText("2");
            AudioManager.Instance.Play(AudioManager.AudioEnums.Two);
            yield return new WaitForSeconds(0.5f);
            searchingForAnOppText.SetText("1");
            AudioManager.Instance.Play(AudioManager.AudioEnums.One);
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.Play(AudioManager.AudioEnums.End);
            player.transform.DOScale(0,.2f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.05f);
            transform.DOScale(0,.2f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.05f);
            searchingForAnOppText.transform.DOScale(0, .2f).SetEase(Ease.InBack);
            vs.transform.DOScale(0, .2f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.2f);
            holderAnimator.SetTrigger("OppFound");
            AudioManager.Instance.Play(AudioManager.AudioEnums.Swish,1);
            yield return new WaitForSeconds(1f);
            holderAnimator.gameObject.SetActive(false);
            
            if (LevelManager.Instance.LevelCount % 2 != 0)
            {
                TileController.Instance.OpenHolder();
            }
            else
            {
                PvPController.Instance.SelectFirstPlayer();
            }
        }
    }
    
    public void Reset()
    {
        holderAnimator.SetTrigger("Idle");
        player.transform.DOScale(1,0f);
        transform.DOScale(1,0f);
        searchingForAnOppText.transform.DOScale(1, 0f);
        vs.transform.DOScale(1, 0f);
        searchingForAnOppText.SetText("Looking for an opponent...");
        bgImage.color = greyColor;
        magnifier.SetActive(true);
        avatar.SetActive(false);
    }
}
