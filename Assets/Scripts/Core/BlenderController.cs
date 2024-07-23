using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RSG;
using Unity.Collections;
using UnityEngine;

public class BlenderController : Singleton<BlenderController>
{
    [ReadOnly] public List<BlenderElement> blenderElementList;

    [SerializeField] private int _desiredFruitAmount;

    [SerializeField] private List<Transform> _blenderElementPositionsAfterUnlock;
    [SerializeField] private List<Transform> _blenderElementPositionsAtStart;
    public int DesiredFruitAmount => _desiredFruitAmount;

    [SerializeField] private BlenderGhost blenderGhost;

    [ReadOnly] public bool isThirdBlenderOpened;  
    private bool _canOpenThirdBlenderOfferAgain=true;

    [SerializeField] private Transform firstBlender;
    [SerializeField] private Transform secondBlender;
    [SerializeField] private Transform firstBlenderPos;

    private void Start()
    {

        if (LevelManager.Instance.LevelCount == 1)
        {
            firstBlender.transform.position = firstBlenderPos.position;

        }
        else
        {
            SetFirstBlender();
        }
    }

    public void SetFirstBlender()
    {
        secondBlender.gameObject.SetActive(true);
        firstBlender.transform.position = _blenderElementPositionsAtStart[0].position;
    }

    public BlenderElement CheckIfBlenderElementAvailable(HexagonTypes fruitType)
    {
        if (JuiceTargetUIController.Instance.CheckIfTargetCompleted(fruitType)) return null;

        BlenderElement blenderElement = blenderElementList
            .FirstOrDefault(g => g.fruitType == fruitType);
        if (blenderElement)
        {
            if (!blenderElement.isProcessing && blenderElement.BlenderFruitCount <= _desiredFruitAmount)
            {
                return blenderElement;
            }
            else return null;
        }
        else
        {
            BlenderElement blenderElementEmpty = blenderElementList
                .FirstOrDefault(g => g.fruitType == HexagonTypes.NONE);
            if (blenderElementEmpty)
            {
                blenderElementEmpty.SetFruitType(fruitType);
                blenderElementEmpty.SetFillbarColor(fruitType);
                blenderElementEmpty.SetFillbarImage(fruitType, true);
                return blenderElementEmpty;
            }
            else return null;
        }
    }

    public void ResetBlenders()
    {
        for (int i = 0; i < blenderElementList.Count; i++)
        {
            blenderElementList[i].ResetBlender();
        }
        CloseThirdBlender();
        StopCoroutine(OpenPopUpAgainCor());
        _canOpenThirdBlenderOfferAgain = true;
    }

    public void BlenderPopUpCheck()
    {
        if (LevelManager.Instance.LevelCount == 1 || isThirdBlenderOpened || !_canOpenThirdBlenderOfferAgain ) return;
        StartCoroutine(BlenderPopUpCheckCor());
    }
    IEnumerator BlenderPopUpCheckCor()
    {
        var gridHolderController = LevelManager.Instance.ReturnGridHolderController();
        float allGrids =gridHolderController.HowManyGridsOccupied().Item1;
        float occupiedGrids =gridHolderController.HowManyGridsOccupied().Item2;
        if (occupiedGrids/allGrids>=.5f && CurrencyManager.Instance.GoldAmount >= blenderGhost.price)
        {
            _canOpenThirdBlenderOfferAgain = false;
            OpenThirdBlender();
            StartCoroutine(OpenPopUpAgainCor());
            yield return new WaitForSecondsRealtime(20);
            if(isThirdBlenderOpened) yield break;
            CloseThirdBlender();
        }   
    }

    IEnumerator OpenPopUpAgainCor()
    {
        yield return new WaitForSecondsRealtime(120);
        _canOpenThirdBlenderOfferAgain = true;
    }
    
    public void OpenThirdBlender()
    {
        StartCoroutine(AdjustBlenderPositionsCor());
        IEnumerator AdjustBlenderPositionsCor()
        {
            blenderElementList = blenderElementList
                .OrderBy(g => g.transform.localPosition.x)
                .ToList();

            blenderElementList[0].transform
                .DOLocalMove(_blenderElementPositionsAfterUnlock[0].localPosition,.2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
           // AudioManager.Instance.Play(AudioManager.AudioEnums.Swish);
            blenderElementList[1].transform
                .DOLocalMove(_blenderElementPositionsAfterUnlock[1].localPosition, .2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
            AudioManager.Instance.Play(AudioManager.AudioEnums.CoinPop);

            blenderGhost.gameObject.SetActive(true);
            blenderGhost.transform.DOPunchScale(.2f*Vector3.one, .25f, 3);
           // blenderGhost.transform.DOJump(blenderGhost.transform.position, 2, 1, .5f);

            yield break;
        }
    }

    public void CloseThirdBlender()
    {
        StartCoroutine(AdjustBlenderPositionsCor2());
        IEnumerator AdjustBlenderPositionsCor2()
        {
            blenderElementList = blenderElementList
                .OrderBy(g => g.transform.localPosition.x)
                .ToList();

            blenderElementList[0].transform
                .DOLocalMove(_blenderElementPositionsAtStart[0].localPosition, .2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
            AudioManager.Instance.Play(AudioManager.AudioEnums.Swish);

            if (blenderElementList.Count > 1)
            {
                blenderElementList[1].transform
                .DOLocalMove(_blenderElementPositionsAtStart[1].localPosition, .2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
                AudioManager.Instance.Play(AudioManager.AudioEnums.Swish);
            }

            blenderGhost.gameObject.SetActive(false);
            if (blenderElementList.Count < 3) yield break;
            blenderElementList[2].gameObject.SetActive(false);
            blenderElementList[2].UnregisterItselfToBlenderController();
            yield break;
        }
    }
}