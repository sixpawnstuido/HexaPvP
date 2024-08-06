using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelInfo;

public class LevelManager : Singleton<LevelManager>
{
    private LevelInfo _levelInfo;
    private Dictionary<int, LevelInfoValues> _levelInfoValues;

    public LevelHolder CurrentLevel => _currentLevel;
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
        _levelInfo = ResourceSystem.ReturnLevelInfo();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            OpenNextLevelPanel();
        }
    }

    public void OpenNextLevelPanel()
    {
        if (UIManager.Instance.failedPanel.gameObject.activeInHierarchy) return;
        LevelCount++;
        TotalLevelCount++;
        if (LevelCount > 25)
        {
            LevelCount = 15;
            TotalLevelCount = 16;
        }
        StopAllCoroutines();
        StartCoroutine(LevelEndPanelCor());
    }

    private IEnumerator LevelEndPanelCor()
    {
        yield return new WaitForSeconds(2f);

        EventManager.CoreEvents.HexagonHolderColliderState(false);
        AudioManager.Instance.Play(AudioManager.AudioEnums.LevelEnd2, .6f);

        levelEndVFX.Play();

        AudioManager.LevelEndSoundCheck = true;
        AudioManager.Instance.StopBGMusic();

        yield return new WaitForSeconds(1f);

        MainSceneCamera.Instance.CameraLevelEnd();

        yield return new WaitForSeconds(.9f);

        UIManager.Instance.nextLevelPanel.gameObject.SetActive(true);

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
    }

    public void NextLevelButton()
    {
        StartCoroutine(NextLevelButtonCor());

        IEnumerator NextLevelButtonCor()
        {
            HexagonMovement.HexagonClickBlock = true;

            InGameLoading.Instance.OpenHolder();

            UIManager.Instance.nextLevelPanel.gameObject.SetActive(false);
            DestroyLevel();
            SpawnCount = 0;
            AudioManager.Instance.Play(AudioManager.AudioEnums.Button, .6f);
            SpawnLevel();
            MainSceneCamera.Instance.ResetCam();
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1.5f);
            yield return new WaitForEndOfFrame();
            UIManager.Instance.levelProgressUIController.ChangeLevelProgressText(TotalLevelCount);;
            isGameOverPanelOpened = false;
            MoveCount = 0;
            SortedFruitLevel++;
            AudioManager.LevelEndSoundCheck = false;
           if(AudioManager.Instance.MusicOnOffCheck==0) AudioManager.Instance.PlayBGMusic();
            PvPController.Instance.ResetAvatars();
            PvPController.Instance.isLevelEnd = false;
            PvPController.Instance.orderIndex =0;
            HexagonMovement.HexagonClickBlock = false;
          //  PvPController.Instance.SelectFirstPlayer();
          //  TileController.Instance.OpenHolder();
           // InGameLoading.Instance.CloseHolder();
        }
    }

    public void RestartLevel()
    {
        FailedEvent();
        StartCoroutine(NextLevelButtonCor());
        IEnumerator NextLevelButtonCor()
        {
            InGameLoading.Instance.OpenHolder();
            UIManager.Instance.failedPanel.gameObject.SetActive(false);
            DestroyLevel();
            SpawnCount = 0;
            CollectedHexagonCount = 0;
            AudioManager.Instance.Play(AudioManager.AudioEnums.Button, .6f);
            yield return new WaitForSeconds(2);
            SpawnLevel();
            PvPController.Instance.ResetAvatars();
            PvPController.Instance.isLevelEnd = false;
           // PvPController.Instance.SelectFirstPlayer();
          //  TileController.Instance.OpenHolder();
            yield return new WaitForEndOfFrame();
          //  InGameLoading.Instance.CloseHolder();
            isGameOverPanelOpened = false;
            PvPController.Instance.orderIndex =0;
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
                var gridController = _currentLevel ? _currentLevel.gridController : FindObjectOfType<GridHolderController>();
                var hexagonHolderController = _currentLevel ? _currentLevel.hexagonHolderController : FindObjectOfType<HexagonHolderController>();
                if (gridController.AreThereAnyHexagonBouncing() || hexagonHolderController.CheckHexagonClearState())
                {
                    yield return new WaitForSeconds(2);
                    GameOverCheck();
                }
                else
                {
                    if (isGameOverPanelOpened) yield break;
                   ClearBoard();
                }
            }
        }
    }

    public void HexagonHolderSpawnCheck()
    {
        var hexagonSpawner=ReturnHexagonSpawner();
        if (hexagonSpawner.AllSlotsEmptyCheck())
        {
           if(  EventManager.SpawnEvents.SpawnHexagonHolder is not null) EventManager.SpawnEvents.SpawnHexagonHolder();
           else Debug.LogError("Spawn event is null");
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
    
    public HexagonHolderController ReturnHexagonSpawnerHexagonHolderController()
    {
        var hexagonHolderController = _currentLevel.hexagonHolderController;
        return hexagonHolderController;
    }

    public void ClearBoard()
    {
        _currentLevel.gridController.ClearRandomGrids();
    }
}