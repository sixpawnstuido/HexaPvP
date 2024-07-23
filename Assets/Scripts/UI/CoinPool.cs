using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    private Vector3[] _initialPos;
    private Quaternion[] _initialRotation;
    private int coinsAmount;

    [Header("Coins")]
    [SerializeField] private List<GameObject> _coins;

    [Header("AnimValues")]
    [SerializeField] private Transform _target;
    [SerializeField] private ParticleSystem _starVFX;

    private Canvas _canvas;
    private void Awake()
    {
        _canvas=GetComponentInParent<Canvas>();
        
        _initialPos = new Vector3[_coins.Count];
        _initialRotation = new Quaternion[_coins.Count];

        for (int i = 0; i < _coins.Count; i++)
        {
            _initialPos[i] = _coins[i].transform.localPosition;
            _initialRotation[i] = _coins[i].transform.rotation;
        }
    }

    private void ResetCoins()
    {
        for (int i = 0; i < _coins.Count; i++)
        {
            _coins[i].transform.localPosition = _initialPos[i];
            _coins[i].transform.rotation= _initialRotation[i];
            _coins[i].transform.localScale = Vector3.zero;
        }
    }
    public void CoinAnim(int coinAmount)
    {
       ResetCoins();
        var delay = 0f;

        for (int i = 0; i < coinAmount; i++)
        {
            var coin = _coins[i];
            coin.SetActive(true);
            coin.transform.DOScale(1f, 0.3f)
               .SetDelay(delay)
               .SetEase(Ease.OutBack);

            DOVirtual.DelayedCall(delay, () => AudioManager.Instance.Play(AudioManager.AudioEnums.CoinPop,1));
            
            coin.transform.DOMove(_target.position, 0.8f)
                .SetDelay(delay + 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(()=>WhenCoinArrived(coin));

            // coin.transform.DOLocalRotate(new Vector3(_canvas.transform.localRotation.x, 0, 180), 0.2f, RotateMode.LocalAxisAdd)
            //     .SetDelay(delay + 0.5f)
            //     .SetEase(Ease.Flash)
            //     .SetLoops(4,LoopType.Restart);

            coin.transform.DOScale(0f, 0.3f)
                .SetDelay(delay + 1.5f)
                .SetEase(Ease.OutBack);

            delay += 0.1f;
        }

        DOVirtual.DelayedCall(3,()=>gameObject.SetActive(false));
    }
    
    private void WhenCoinArrived(GameObject coin)
    {
        //coin.GetComponent<TrailRenderer>().enabled = false;
        CurrencyManager.Instance.AddGold(1);
        AudioManager.Instance.Play(AudioManager.AudioEnums.CoinArrived, 1);
        if (!DOTween.IsTweening(_target.transform.GetHashCode()))
        {
            _target
                .DOPunchScale(new Vector3(-.15f, 0.15f, 0), .5f, 10)
                .SetId(_target.transform.GetHashCode());

            if (!_starVFX.isPlaying)
            {
                _starVFX.Stop();
                _starVFX.Play();
            }
        }
        DOVirtual.DelayedCall(0.2f, () => coin.SetActive(false));
        //int goldAmount = CurrencyManager.Instance.clearSliceAmount;
    }
}
