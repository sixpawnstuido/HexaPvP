using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.VisualScripting;

public class GridHolderController : MonoBehaviour
{
    [ShowInInspector, ReadOnly] public Dictionary<Vector2, GridHolder> gridHolderDic = new();

    [SerializeField] private int _randomSpawnAmount;

    private LayerMask _gridHolderLayerMask = 1 << 6;
    private bool _justOnce;
    private Vector3 _fingerDistance;
    private void Awake()
    {
        FillGridHolderNeigbourLists();
    }
    private void OnEnable()
    {
        EventManager.SpawnEvents.CheckIfAllGridsOccupied += CheckIfAllGridHoldersOccupied;
    }
    private void OnDisable()
    {
        EventManager.SpawnEvents.CheckIfAllGridsOccupied -= CheckIfAllGridHoldersOccupied;
    }
    private void Start()
    {
        var globalVariables = ResourceSystem.ReturnGlobalVariablesData();
        _fingerDistance = globalVariables is not null ? globalVariables.hexagonHolderFingerDistance : Vector3.up / 2;
    }
    private void Update()
    {
        GridProjectile();
    }
    private void FillGridHolderNeigbourLists()
    {
        var tempGridHolderList = GetComponentsInChildren<GridHolder>().ToList();
        for (int i = 0; i < tempGridHolderList.Count; i++)
        {
            Vector2 tempVector2 = new Vector2(tempGridHolderList[i].transform.localPosition.x, tempGridHolderList[i].transform.localPosition.z);
            gridHolderDic.Add(tempVector2, tempGridHolderList[i]);
        }
    }

    public bool CheckIfAllGridHoldersOccupied()
    {
        bool allGridHoldersOccupied = gridHolderDic.Values.ToList().All(g => g.hexagonHolder != null || g.isLockActive);
        return allGridHoldersOccupied;
    }

    public Tuple<int, int> HowManyGridsOccupied()
    {
        var allGridHolderList = gridHolderDic.Values.ToList();
        var occupiedGridHolderList = allGridHolderList.Where(g => g.hexagonHolder is not null || g.isLockActive).ToList();
        return Tuple.Create(allGridHolderList.Count, occupiedGridHolderList.Count);
    }

    public List<GridHolder> ReturnFullGrids()
    {
        var allGridHolderList = gridHolderDic.Values.ToList();
        var occupiedGridHolderList = allGridHolderList.Where(g => g.hexagonHolder != null).ToList();
        return occupiedGridHolderList;
    }

    public List<GridHolder> ReturnAllGrids()
    {
        var allGridHolderList = gridHolderDic.Values.ToList();
        return allGridHolderList;
    }

    public HexagonTypes ReturnHexagonType(bool isMax)
    {
        var allGridHolderList = gridHolderDic.Values.ToList();
        var occupiedGrids = allGridHolderList.Where(g => g.hexagonHolder != null && g.hexagonHolder.hexagonElements.Count > 0).ToList();

        if (isMax)
        {
            var tempNumerable = occupiedGrids.
             GroupBy(g => g.hexagonHolder.hexagonElements.Last().hexagonType)
            .OrderByDescending(g => g.Count())
            .First();
            HexagonTypes hexagonType = tempNumerable.Key;
            return hexagonType;
        }
        else
        {
            var tempNumerable = occupiedGrids.
             GroupBy(g => g.hexagonHolder.hexagonElements.Last().hexagonType)
            .OrderBy(g => g.Count())
            .First();
            HexagonTypes hexagonType = tempNumerable.Key;
            return hexagonType;
        }

    }

    public bool AreThereAnyHexagonBouncing()
    {
        bool isAllBouncing = gridHolderDic.Values.Any(g => g.isBouncing);
        return isAllBouncing;
    }

    private void GridProjectile()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var gridHolderList = gridHolderDic.Values.ToList();
            for (int i = 0; i < gridHolderList.Count; i++)
            {
                //gridHolderList[i].gridProjector.SetActive(false);
                gridHolderList[i].DisableHighlight();
            }
        }
        if (!HexagonMovement.isMoving) return;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var gridHolder = RaycastManager.SendRayFromCameraToMousePos<GridHolder>(_gridHolderLayerMask, true, _fingerDistance);
            if (gridHolder)
            {
                if (!gridHolder.gridProjector.activeInHierarchy)
                {
                    _justOnce = true;
                    var gridHolderList = gridHolderDic.Values.ToList();
                    for (int i = 0; i < gridHolderList.Count; i++)
                    {
                        if (gridHolderList[i] != gridHolder) { gridHolderList[i].DisableHighlight(); /*gridHolderList[i].gridProjector.SetActive(false);*/ continue; }
                        //gridHolderList[i].gridProjector.SetActive(true);
                        gridHolderList[i].HighlightSlot();
                    }
                }
            }
            else
            {
                if (_justOnce)
                {
                    var gridHolderList = gridHolderDic.Values.ToList();
                    for (int i = 0; i < gridHolderList.Count; i++)
                    {
                        //gridHolderList[i].gridProjector.SetActive(false);
                        gridHolderList[i].DisableHighlight();
                    }
                    _justOnce = false;
                }
            }
        }
    }


    [Button]
    public void ClearRandomGrids()
    {
        StartCoroutine(ClearRandomGridsCor());
        IEnumerator ClearRandomGridsCor()
        {
            int numberOfGridHoldersToDelete =
                ResourceSystem
                    .ReturnLevelInfo()
                    .levelInfoValues[LevelManager.Instance.LevelCount]
                    .numberOfHexagonHoldersToDelete;

            var gridHolderList = gridHolderDic.Values.ToList();
            var gridsToDelete = gridHolderList
                .Where(g
                    => g.transform.localPosition.x == 0.8f
                       || g.transform.localPosition.x == -0.8f
                       || g.transform.localPosition.x == 0f)
                .Take(numberOfGridHoldersToDelete)
                .ToList();
       
                Debug.Log(gridsToDelete.Count);

            for (int i = 0; i < gridsToDelete.Count; i++)
            {
                Debug.Log(gridsToDelete[i].name);
                if (gridHolderList[i].isLockActive) continue;
                if (gridHolderList[i].hexagonHolder is null) continue;
                var hexagonHolder = gridsToDelete[i].hexagonHolder;
                hexagonHolder.transform
                    .DOScale(0, .2f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        Destroy(hexagonHolder.gameObject);
                    });
                gridHolderList[i].hexagonHolder = null;
                gridHolderList[i].GridCollider.enabled=true;
               yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public GridHolder ReturnAvailableGridHolder()
    {
        var gridHolder = gridHolderDic.Values.FirstOrDefault(grid=>!grid.isLockActive && grid.hexagonHolder is null);
        return gridHolder;
    }
}
