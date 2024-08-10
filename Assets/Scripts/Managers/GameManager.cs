using System.Collections;
using System.Collections.Generic;
using Tabtale.TTPlugins;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int _vSyncCount = 0;
    private const int _refreshRate = 60;

    private void Awake()
    {
        TTPCore.Setup();
    }


    private void Start()
    {
        InitializeGame();
        LimitFPS();
    }
    private void InitializeGame()
    {
        StartCoroutine(InitializeGameCor());
        IEnumerator InitializeGameCor()
        {
            StartLoadingScreen.Instance.StartAnimCor(4);
            yield return new WaitUntil(() => EventManager.SpawnEvents.LoadAllDatas != null);
            EventManager.SpawnEvents.LoadAllDatas();
            yield return new WaitForSeconds(1);
            LevelManager.Instance.SpawnLevel();
            PvPController.Instance.ResetAvatars();
            yield return new WaitForSeconds(3);

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


    private void LimitFPS()
    {
        QualitySettings.vSyncCount = _vSyncCount;
        if (Screen.currentResolution.refreshRate >= _refreshRate) Application.targetFrameRate = Screen.currentResolution.refreshRate;
        else Application.targetFrameRate = _refreshRate;
    }
}
