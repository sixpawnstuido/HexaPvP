using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class MetaWrapper : MonoBehaviour
{
    public static MetaWrapper Instance;


    public UnityEvent OnOpenMeta;
    public UnityEvent OnCloseMeta;
    public UnityEvent OnFruitsFinishInMeta;

    public GameObject tutorialFirst;
    public GameObject overlayCanvas;
    public GameObject fruitFinishPanel;
    public GameObject fruitFailPanel;
    public GameObject winPanel;
 

    public Transform spawnedFruitHolder;

    public static bool TouchLock = false;

    private void Awake()
    {
        Instance = this;
    }



    [Button]
    public void OpenMeta()
    {
       

        fruitFailPanel.SetActive(false);
        fruitFinishPanel.SetActive(false);
        winPanel.SetActive(false);

        AudioManager.LevelEndSoundCheck = false;
        AudioManager.Instance.PlayBGMusic();

        InGameLoading.Instance.OpenHolder();

        DOVirtual.DelayedCall(1, delegate { InGameLoading.Instance.CloseHolder(); });


       TouchLock = false;

        SpawnerMovement.canClick = false;

        if (LevelManager.Instance.LevelCount == 2)
        {
            LevelManager.Instance.NextLevelButton();
        }
        else
        {

            if (PlayerPrefs.GetInt("MetaFirst") != 1)
            {
                StartCoroutine(MetaTut());
                PlayerPrefs.SetInt("MetaFirst", 1);
            }

            overlayCanvas.SetActive(true);

            UIManager.Instance.nextLevelPanel.gameObject.SetActive(false);
            int sortedFruitCount = LevelManager.Instance.SortedFruitCount;
            FruitSpawner.numberOfFruitsToSpawn = Mathf.Clamp(sortedFruitCount, 3, 30);
            OnOpenMeta?.Invoke();
            EventHandler.Instance.LogMetaEvents(LevelManager.Instance.LevelCount, FruitSpawner.numberOfFruitsToSpawn, EventHandler.EventStatus.Start);
        }


       


      

    }

    private IEnumerator MetaTut()
    {
        tutorialFirst.SetActive(true);

        yield return new WaitForSeconds(.2f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

        tutorialFirst.SetActive(false);
    }

    [Button]
    public void CloseMeta()
    {
       
        LevelManager.Instance.NextLevelButton();
    }

    [Button]
    public void ClearMeta()
    {
        foreach (Transform child in spawnedFruitHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public void CloseMetaCustom()
    {
        OnCloseMeta?.Invoke();
    }

    public void FruitsFinishInMeta()
    {
        //�ste Yaz


        OnFruitsFinishInMeta?.Invoke();
    }

   

    public void ToggleTouchLock(bool value)
    {
        TouchLock = value;
    }

}
