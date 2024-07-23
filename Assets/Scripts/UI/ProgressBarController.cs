using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LevelInfo;

public class ProgressBarController : Singleton<ProgressBarController>
{
    private SlicedFilledImage _slicedFilledImage;

    private Dictionary<int, LevelInfoValues> _levelInfoValues;

    [SerializeField] private TextMeshProUGUI _progressText;

    private NextLevelPanel _nextLevelPanel;


    private void Awake()
    {
        _slicedFilledImage= GetComponent<SlicedFilledImage>();
        _nextLevelPanel=FindObjectOfType<NextLevelPanel>(true);
    }   
    IEnumerator Start()
    {
        yield return new WaitUntil(()=>EventManager.SpawnEvents.LoadAllDatas!=null);
        _levelInfoValues = ResourceSystem.ReturnLevelInfo().levelInfoValues;
        UpdateProgressBar();
    }
    
    public void UpdateProgressBar()
    {
        LevelManager levelManager = LevelManager.Instance;
        int desiredHexagonAmount = _levelInfoValues[levelManager.LevelCount].desiredHexagonAmount;

        if (_nextLevelPanel.gameObject.activeInHierarchy) return;

        int collectedHexagonCount = levelManager.CollectedHexagonCount;
        collectedHexagonCount = Mathf.Clamp(collectedHexagonCount,0, desiredHexagonAmount);
        _progressText.SetText($"{levelManager.CollectedHexagonCount}/{desiredHexagonAmount}");
        _slicedFilledImage.fillAmount = Mathf.InverseLerp(0, desiredHexagonAmount, collectedHexagonCount);
    }
    public void ResetProgressBar()
    {
        int desiredHexagonAmount = _levelInfoValues[LevelManager.Instance.LevelCount].desiredHexagonAmount;
        _slicedFilledImage.fillAmount = 0;
        _progressText.SetText($"{0}/{desiredHexagonAmount}");
    }
    public void ProgressTextState(bool setActive)
    {
        _progressText.gameObject.SetActive(setActive);
    }
}
