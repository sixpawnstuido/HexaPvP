using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelInfo;

public class LevelManager : Singleton<LevelManager>
{
    private Dictionary<int, LevelInfoValues> _levelInfoValues;

    private LevelHolder _currentLevel;

    public bool isGameOverPanelOpened;

    public int SpawnCount;

    public ParticleSystem levelEndVFX;

    public static bool IsLevelCompleted;
    
    #region Prefs

    [ShowInInspector]
    public int LevelCount
    {
        get => PlayerPrefs.GetInt("LevelCount", 1);
        set => PlayerPrefs.SetInt("LevelCount", value);
    }

    public int TotalLevelCount
    {
        get => PlayerPrefs.GetInt("TotalLevelCount", 1);
        set => PlayerPrefs.SetInt("TotalLevelCount", value);
    }

    [ShowInInspector, ReadOnly]
    public int CollectedHexagonCount
    {
        get => PlayerPrefs.GetInt("CollectedHexagonCount" + LevelCount, 0);
        set => PlayerPrefs.SetInt("CollectedHexagonCount" + LevelCount, value);
    }

    [ShowInInspector, ReadOnly]
    public int MoveCount
    {
        get => PlayerPrefs.GetInt("MoveCount" + LevelCount, 0);
        set => PlayerPrefs.SetInt("MoveCount" + LevelCount, value);
    }
    [ShowInInspector, ReadOnly]
    public int SortedFruitCount
    {
        get => PlayerPrefs.GetInt("SortedFruitCount" + SortedFruitLevel, 0);
        set => PlayerPrefs.SetInt("SortedFruitCount" + SortedFruitLevel, value);
    }
    public int SortedFruitLevel
    {
        get => PlayerPrefs.GetInt("SortedFruitLevel", 0);
        set => PlayerPrefs.SetInt("SortedFruitLevel", value);
    }

    #endregion


    IEnumerator Start()
    {
        yield return new WaitUntil(() => EventManager.SpawnEvents.LoadAllDatas != null);
        _levelInfoValues = ResourceSystem.ReturnLevelInfo().levelInfoValues;
    }

    public void SpawnLevel()
    {
        var levelHolder = ResourceSystem.ReturnLevelData().allLevels[LevelCount];
        _currentLevel = Instantiate(levelHolder);
    }

    private void DestroyLevel()
    {
        Destroy(_currentLevel.gameObject);
        _currentLevel = null;
    }

    public void OpenNextLevelPanel()
    {
        if (UIManager.Instance.failedPanel.gameObject.activeInHierarchy) return;
        EventHandler.Instance.LevelComplete(LevelCount, MoveCount);
        LevelCount++;
        TotalLevelCount++;
        if (LevelCount > 15)
        {
            ClearAllSave();
          //  TutorialManager.Instance.TutorialCount = 3;
            LevelCount = 2;
            TotalLevelCount = 16;
        }
        StopAllCoroutines();

        StartCoroutine(LevelEndPanelCor());
    }

    private IEnumerator LevelEndPanelCor()
    {
      


        EventManager.CoreEvents.HexagonHolderColliderState(false);
        AudioManager.Instance.Play(AudioManager.AudioEnums.LevelEnd2, .6f);

       

        levelEndVFX.Play();

        AudioManager.LevelEndSoundCheck = true;
        AudioManager.Instance.StopBGMusic();

        yield return new WaitForSeconds(1f);

        MainSceneCamera.Instance.CameraLevelEnd();


        yield return new WaitForSeconds(.9f);

        JuiceTargetUIController.Instance.ClearTargets();

        UIManager.Instance.nextLevelPanel.gameObject.SetActive(true);
      CurrencyManager.Instance.AddGold(10);

    }

    public void OpenFailedPanel(bool isTimer=false)
    {
        if (UIManager.Instance.nextLevelPanel.gameObject.activeInHierarchy) return;
        UIManager.Instance.failedPanel.gameObject.SetActive(true);
        UIManager.Instance.failedPanel.NormalFailOrTimerFail(isTimer);
        AudioManager.Instance.Play(AudioManager.AudioEnums.LevelFail, .6f);
        isGameOverPanelOpened = true;
    }

    public void FailedEvent()
    {
        StopAllCoroutines();
        EventManager.CoreEvents.HexagonHolderColliderState(false);
    }

    public void NextLevelButton()
    {
        StartCoroutine(NextLevelButtonCor());

        IEnumerator NextLevelButtonCor()
        {
            HexagonMovement.HexagonClickBlock = true;

            //EventManager.UIEvents.CanvasSetter(CanvasTypes.LoadingCanvas, true);
            InGameLoading.Instance.OpenHolder();

            UIManager.Instance.nextLevelPanel.gameObject.SetActive(false);
            DestroyLevel();
            SpawnCount = 0;
            AudioManager.Instance.Play(AudioManager.AudioEnums.Button, .6f);
            //ProgressBarController.Instance.ResetProgressBar();
           // yield return new WaitForSeconds(1);
            SpawnLevel();
            MainSceneCamera.Instance.ResetCam();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1.5f);
            JuiceTargetUIController.Instance.TargetUILevelStartAnimation();
            yield return new WaitForEndOfFrame();
            InGameLoading.Instance.CloseHolder();
            HintsEnable.Instance.OpenHints();
            UIManager.Instance.levelProgressUIController.ChangeLevelProgressText(TotalLevelCount);
           // EventManager.UIEvents.CanvasSetter(CanvasTypes.LoadingCanvas, false);
            isGameOverPanelOpened = false;
            MoveCount = 0;
            SortedFruitLevel++;
            if(LevelCount>1) RestartButton.Instance.HolderSetactiveState(true);
            Timer.Instance.ResetTimer();
            MetaProgress.Instance.ResetProgress();
            AudioManager.LevelEndSoundCheck = false;
            AudioManager.Instance.PlayBGMusic();
        }
    }

    public void RestartLevel()
    {

        FailedEvent();

        StartCoroutine(NextLevelButtonCor());

        IEnumerator NextLevelButtonCor()
        {
            Timer.Instance.ResetTimer();
            InGameLoading.Instance.OpenHolder();
            UIManager.Instance.failedPanel.gameObject.SetActive(false);
            DestroyLevel();
            SpawnCount = 0;
            CollectedHexagonCount = 0;
            JuiceTargetUIController.Instance.ResetTargets();
            AudioManager.Instance.Play(AudioManager.AudioEnums.Button, .6f);
            yield return new WaitForSeconds(2);
            SpawnLevel();
            yield return new WaitForEndOfFrame();
            InGameLoading.Instance.CloseHolder();
            //MetaProgress.Instance.ResetProgress();
            isGameOverPanelOpened = false;
            MoveCount = 0;
        }
    }



    public void GameOverCheck()
    {
        if (isGameOverPanelOpened) return;
        StartCoroutine(GameOverCheckCor());

        IEnumerator GameOverCheckCor()
        {
            yield return new WaitForSeconds(1.5f);
            if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
            bool isAllGridsOccupied = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
            if (isAllGridsOccupied)
            {
                var gridController =
                    _currentLevel ? _currentLevel.gridController : FindObjectOfType<GridHolderController>();
                if (gridController.IsThereAnyGridBouncing())
                {
                    yield return new WaitForSeconds(2);
                    GameOverCheck();
                }
                else
                {
                    if (isGameOverPanelOpened) yield break;
                    OpenFailedPanel();
                    Timer.Instance.canUpdate = false;
                }
            }
        }
    }

    public void HexagonHolderSpawnCheck()
    {
        SpawnCount++;
        if (SpawnCount % 3 == 0)
        {
            EventManager.SpawnEvents.SpawnHexagonHolder();
        }
    }

    public GridHolderController ReturnGridHolderController()
    {
        var gridHolderController = _currentLevel.gridController;
        return gridHolderController;
    }

    public HexagonSpawner ReturnHexagonSpawner()
    {
        var hexagonSpawner = _currentLevel.hexagonSpawner;
        return hexagonSpawner;
    }

    [Button]
    private void ClearAllSave()
    {
        ES3.DeleteFile();
        PlayerPrefs.DeleteAll();
    }
}