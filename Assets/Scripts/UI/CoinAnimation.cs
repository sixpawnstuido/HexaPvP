using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private List<CoinPool> _coinPoolList;

    [Button]
    public void StartCoinAnimation(Vector3 startPos, int amount)
    {
        var coinPool = _coinPoolList.FirstOrDefault(g => !g.gameObject.activeInHierarchy);
        if (coinPool)
        {
            CoinPoolInit(startPos, coinPool,amount);
        }
        else
        {
            var coinPoolInstantiated = Instantiate(_coinPoolList[0], transform);
            _coinPoolList.Add(coinPoolInstantiated);
            CoinPoolInit(startPos, coinPoolInstantiated,amount);
        }
    }

    private void CoinPoolInit(Vector3 startPos, CoinPool coinPool,int amount)
    {
        coinPool.transform.position = startPos;
        coinPool.gameObject.SetActive(true);
        coinPool.CoinAnim(amount);
    }
}