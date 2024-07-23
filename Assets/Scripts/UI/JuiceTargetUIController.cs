using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuiceTargetUIController : Singleton<JuiceTargetUIController>
{
    private List<JuiceTargetUIElement> _targetUIElementList = new();

    private HorizontalLayoutGroup _horizontalLayoutGroup;

    [SerializeField] private Transform startPos;
    [SerializeField] private Transform InGamePos;

    [SerializeField] private Image blackBG;

    [SerializeField] private TextMeshProUGUI goalsText;

    private void Awake()
    {
        _horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

    public void TargetUILevelStartAnimation()
    {
        if (LevelManager.Instance.LevelCount == 1) return;
        StartCoroutine(AnimCor());
    }

    private IEnumerator AnimCor()
    {
        HexagonMovement.HexagonClickBlock = true;

        //yield return new WaitForSeconds(.5f);

        transform.position = startPos.position;

        transform.localScale = startPos.localScale;

        goalsText.transform.localScale = Vector3.zero;

        blackBG.DOFade(0,0);

        for (int i = 0; i < _targetUIElementList.Count; i++)
        {
            _targetUIElementList[i].transform.localScale = Vector3.zero;
            _targetUIElementList[i].NameHolderScale(0,0,Ease.Linear);
        }

        blackBG.DOFade(0.7176471f, .2f);

        yield return new WaitForSeconds(.2f);

        goalsText.transform.DOScale(Vector3.one, .25f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < _targetUIElementList.Count; i++)
        {
            _targetUIElementList[i].transform.DOScale(Vector3.one*.89f, .25f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(.25f);
            AudioManager.Instance.Play(AudioManager.AudioEnums.Boxy);
        }


        yield return new WaitForSeconds(2);

        blackBG.DOFade(0, .1f);

        goalsText.transform.DOScale(Vector3.zero, .1f);

        transform.DOMove(InGamePos.position, .5f);
        transform.DOScale(InGamePos.localScale, .5f);

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < _targetUIElementList.Count; i++)
        {
            _targetUIElementList[i].NameHolderScale(1,.2f,Ease.OutBack);
        }
        HexagonMovement.HexagonClickBlock = false;

        if (LevelManager.Instance.LevelCount == 2)
        {
            UIManager.Instance.changeSlotHint.OpenTutorial();
        }
        if (LevelManager.Instance.LevelCount == 4)
        {
            UIManager.Instance.clearSlotHint.OpenTutorial();
        }

        yield break;

    }

    public void AddToTargetUIElementList(JuiceTargetUIElement targetUI)
    {
        if (!_targetUIElementList.Contains(targetUI))
        {
            _targetUIElementList.Add(targetUI);
        }


    }

    public void CheckIfAllTargetsCompleted()
    {
        bool isAllCompleted = _targetUIElementList.All(g => g.JuiceBoxCount <= 0);
        if (isAllCompleted)
        {
            AudioManager.Instance.Stop();
            LevelManager.IsLevelCompleted = true;
            Timer.Instance.canUpdate = false;
            DOVirtual.DelayedCall(.5f, () => LevelManager.Instance.OpenNextLevelPanel());
        }
    }

    public bool CheckIfTargetCompleted(HexagonTypes fruitType)
    {
        var targetUIElement = _targetUIElementList.FirstOrDefault(g=>g.hexagonType==fruitType);
        if (targetUIElement is null)
        {
            return false;
        }
        else return targetUIElement.JuiceBoxCount == 0;
    }

    public void ClearTargets()
    {
        if (_targetUIElementList.Count <= 0) return;
        for (int i = 0; i < _targetUIElementList.Count; i++)
        {
            Destroy(_targetUIElementList[i].gameObject);
        }

        _targetUIElementList.Clear();
    }


    public JuiceTargetUIElement ReturnJuiceTargetUIElement(HexagonTypes hexagonType)
    {
        var juiceTargetUIElement = _targetUIElementList.FirstOrDefault(g => g.hexagonType == hexagonType);
        if (juiceTargetUIElement) return juiceTargetUIElement;
        else return null;
    }

    public void ResetTargets()
    {
        for (int i = 0; i < _targetUIElementList.Count; i++)
        {
            _targetUIElementList[i].IsJuiceTargetActivated=false;
        }
    }
}