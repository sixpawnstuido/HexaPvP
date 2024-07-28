using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class GridHolder : MonoBehaviour
{
    public static Action OpenCollider;

    public Collider GridCollider => _collider;
    private Collider _collider;

    private GridHolderController _gridController;

    [ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    private List<GridHolder> _neighborGridHolderList = new();

    [SerializeField] private Vector3 _neighborGridHolderOffset;

    public HexagonHolder hexagonHolder;

    [Sirenix.OdinInspector.ReadOnly] public bool isBouncing;
    [Sirenix.OdinInspector.ReadOnly] public bool isScanning;

    public List<GridHolder> gridHoldersInLineList = new();

    public bool isLockActive;

    public GameObject gridProjector;

    private Tween slotScaleTween;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _gridController = GetComponentInParent<GridHolderController>();
    }

    private void OnEnable()
    {
        OpenCollider += OpenColliderr;
        EventManager.CoreEvents.GridHolderColliderState += ColliderState;
    }

    private void OnDisable()
    {
        OpenCollider -= OpenColliderr;
        EventManager.CoreEvents.GridHolderColliderState -= ColliderState;
    }

    public void OpenColliderr()
    {
        _collider.enabled = true;
    }

    public void HighlightSlot()
    {
        gridProjector.SetActive(true);

        transform.localScale = new Vector3(1f, 2.11432958f, 1f);

        if (slotScaleTween != null) slotScaleTween.Kill();

        slotScaleTween = transform.DOPunchScale(Vector3.one*.15f, .25f,3);
    }

    public void DisableHighlight()
    {
        gridProjector.SetActive(false);

        if (slotScaleTween != null) slotScaleTween.Kill();

        transform.localScale = new Vector3(1f, 2.11432958f, 1f);


    }

    private void Start()
    {
        GetNeighborGrids();
    }

    private void OnMouseDown()
    {
        ClearSlotHintState();
    }

    public void ColliderState(bool state, bool isLockOpened = false)
    {
        if (state)
        {
            if (isLockActive && !isLockOpened) return;
            _collider.enabled = state;
        }
        else
        {
            if (hexagonHolder is null) return;
            _collider.enabled = state;
        }
    }

    public void GetNeighborGrids()
    {
        List<Vector2> tempVector2List = new List<Vector2>
        {
            new Vector2(transform.localPosition.x, transform.localPosition.z + _neighborGridHolderOffset.z * 2),
            new Vector2(transform.localPosition.x + _neighborGridHolderOffset.x,
                transform.localPosition.z + _neighborGridHolderOffset.z),
            new Vector2(transform.localPosition.x + _neighborGridHolderOffset.x,
                transform.localPosition.z - _neighborGridHolderOffset.z),
            new Vector2(transform.localPosition.x, transform.localPosition.z - _neighborGridHolderOffset.z * 2),
            new Vector2(transform.localPosition.x - _neighborGridHolderOffset.x,
                transform.localPosition.z - _neighborGridHolderOffset.z),
            new Vector2(transform.localPosition.x - _neighborGridHolderOffset.x,
                transform.localPosition.z + _neighborGridHolderOffset.z)
        };

        for (int i = 0; i < tempVector2List.Count; i++)
        {
            if (_gridController.gridHolderDic.ContainsKey(tempVector2List[i]))
            {
                _neighborGridHolderList.Add(_gridController.gridHolderDic[tempVector2List[i]]);
            }
        }
    }

    public void CheckIfGridHolderEmpty()
    {
        if (hexagonHolder && hexagonHolder.hexagonElements.Count == 0)
        {
            ColliderState(true);
            hexagonHolder.gridHolder = null;
            hexagonHolder = null;
        }
    }

    public void ScanNeighborGrids()
    {
        StartCoroutine(ScanNeighborGridsCor());

        IEnumerator ScanNeighborGridsCor()
        {
            if (!hexagonHolder) yield break;
            if (hexagonHolder.hexagonElements.Count == 0) yield break;
            isScanning = true;
            var hexagonType = hexagonHolder.ReturnLastHexagonsType();
            List<GridHolder> tempGridHolders = new List<GridHolder>();

            for (int i = 0; i < _neighborGridHolderList.Count; i++)
            {
                if (!_neighborGridHolderList[i].hexagonHolder) continue; //if grid does not have a hexagon holder
                if (hexagonType ==
                    _neighborGridHolderList[i].hexagonHolder.ReturnLastHexagonsType()) //Select matching grid holders
                {
                    tempGridHolders.Add(_neighborGridHolderList[i]);
                }
            }

            if (tempGridHolders.Count == 0) // if there is no grid match
            {
                isScanning = false;
                LevelManager.Instance.GameOverCheck();
                yield break;
            }
            else // if grid match
            {
                isBouncing = true;
                for (int i = 0; i < tempGridHolders.Count; i++)
                {
                    if (tempGridHolders[i].isBouncing &&
                        !tempGridHolders[i].gridHoldersInLineList.Contains(tempGridHolders[i]))
                    {
                        tempGridHolders[i].gridHoldersInLineList.Add(tempGridHolders[i]);
                        isBouncing = false;
                        yield break;
                    }
                }

                for (int i = 0; i < tempGridHolders.Count; i++)
                {
                    tempGridHolders[i].isBouncing = true;
                }

                for (int i = 0; i < tempGridHolders.Count; i++)
                {
                    if (i >= tempGridHolders.Count) continue;
                    tempGridHolders[i].hexagonHolder.BounceHexagonsToAnotherHolder(hexagonHolder);
                    var tempHexagonHolder = tempGridHolders[i].hexagonHolder;
                    if (tempHexagonHolder is null) continue;
                    yield return new WaitUntil(() => tempHexagonHolder.isJumpStopped);
                    tempHexagonHolder.isJumpStopped = false;
                    tempGridHolders[i].isBouncing = false;


                    tempGridHolders[i].CheckIfGridHolderEmpty();
                    tempGridHolders[i].ScanNeighborGrids();
                }

                isBouncing = false;
                isScanning = false;

                for (int i = 0; i < gridHoldersInLineList.Count; i++)
                {
                    if (i >= gridHoldersInLineList.Count) continue;
                    var tempGridHolderInLine = gridHoldersInLineList[i];
                    if (tempGridHolderInLine is null) continue;
                    tempGridHolderInLine.ScanNeighborGrids();
                    yield return new WaitUntil(() => !tempGridHolderInLine.isScanning);
                }

                gridHoldersInLineList = new();

                //Check if there are more then 5 hexagon elements 
                if (hexagonHolder) hexagonHolder.ClearHexagons();
                for (int i = 0; i < tempGridHolders.Count; i++)
                {
                    if (tempGridHolders[i].hexagonHolder)
                    {
                        var hexagonHolder = tempGridHolders[i].hexagonHolder;
                        if (hexagonHolder is null) continue;
                        yield return new WaitUntil(() => hexagonHolder.isJumpStopped);
                        if (hexagonHolder) hexagonHolder.ClearHexagons();
                    }
                }

                LevelManager.Instance.GameOverCheck();
            }
        }
    }

    public void ClearSlotHintState()
    {
        ClearSlotHint clearSlotHint = UIManager.Instance.clearSlotHint;
        if (_gridController.AreThereAnyHexagonBouncing()
            || !UIManager.Instance.clearSlotHint.isHintActive
            || hexagonHolder is null) return;

        clearSlotHint.DecreaseMoveCount();

        DOVirtual.DelayedCall(.3f, () => clearSlotHint.Unfocus());

        var knife = ResourceSystem.ReturnVisualData().prefabData[VisualData.PrefabType.Knife];
        Vector3 knifeSpawnPos =
            hexagonHolder.hexagonElements.Last().transform.position - new Vector3(-0.012f, -0.495f, .52f);
        Knife knifeInstantiated = Instantiate(knife, knifeSpawnPos, knife.transform.rotation).GetComponent<Knife>();
        knifeInstantiated.Init(hexagonHolder);

        StartCoroutine(HexagonElementClearCor());
        
        hexagonHolder.hexagonElements.Clear();
        hexagonHolder.CanTouchHexagonHolder(false);
        hexagonHolder.gridHolder = null;
        hexagonHolder = null;
    }

    IEnumerator HexagonElementClearCor()
    {
        List<HexagonElement> tempHexagonElementList = new List<HexagonElement>(hexagonHolder.hexagonElements);
        yield return new WaitForSeconds(0.6f);
        AudioManager.Instance.Play(AudioManager.AudioEnums.Cut);
        tempHexagonElementList = tempHexagonElementList
            .OrderByDescending(g => g.transform.localPosition.y).ToList();
        for (int i = 0; i < tempHexagonElementList.Count; i++)
        {
            tempHexagonElementList[i].ClearSlotHintState();
            yield return new WaitForSeconds(0.05f);
        }
    }
}