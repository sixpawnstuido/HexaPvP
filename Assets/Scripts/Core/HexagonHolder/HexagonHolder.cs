using System;
using DG.Tweening;
using Lofelt.NiceVibrations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro.SpriteAssetUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[SelectionBase]
public class HexagonHolder : MonoBehaviour
{
    [HideInInspector] public Collider hexagonCollider;

    [ReadOnly] public List<HexagonElement> hexagonElements = new();

    [ReadOnly] public GridHolder gridHolder;

    [ReadOnly] public bool isJumpStopped;
    public bool isClearHappening;

    [SerializeField] private Vector3 _jumpOffet;

    private HexagonSlot _hexagonSlot;

    private HexagonMovement _hexagonMovement;

    private LevelHolder _levelHolder;

    public PlayerType playerType;

    public LayerMask SecondCamLayerMask => secondCamLayerMask;
    private LayerMask secondCamLayerMask=1<<10;
    
    public LayerMask HexagonHolderLayerMask => hexagonHolderLayerMask;
    private LayerMask hexagonHolderLayerMask=1<<7;

    public static PlayerType PlayerTypeGlobal;

    private LevelInfo _levelInfo;

    private void Awake()
    {
        hexagonCollider = GetComponent<Collider>();
        _hexagonMovement = GetComponent<HexagonMovement>();
        _levelHolder = FindObjectOfType<LevelHolder>();
    }

    private void OnEnable()
    {
        EventManager.CoreEvents.HexagonHolderColliderState += CanTouchHexagonHolder;
    }

    private void OnDisable()
    {
        EventManager.CoreEvents.HexagonHolderColliderState -= CanTouchHexagonHolder;
    }

    private void Start()
    {
        _levelInfo = ResourceSystem.ReturnLevelInfo();
    }

    public void Init(HexagonSlot hexagonSlot)
    {
        hexagonSlot.hexagonHolder = this;
        _hexagonSlot = hexagonSlot;
        SlideAnim(hexagonSlot);
        SortElementsListByTransformY();
        _hexagonMovement.ChangeFirstTarget(hexagonSlot.transform);
    }

    public void InitCreative()
    {
        SortElementsListByTransformY();
        _hexagonMovement.ChangeFirstTarget(transform);
    }

    public void CanTouchHexagonHolder(bool colliderState)
    {
        if (!colliderState)
        {
            if (gridHolder is not null) hexagonCollider.enabled = colliderState;
        }
        else
        {
            if (hexagonElements.Count <= 0) return;
            hexagonCollider.enabled = colliderState;
        }
    }

    public void HexagonPlacedState(GridHolder gridHolder, bool isChangeSlotHint = false,bool isOpponent=false)
    {
        this.gridHolder = gridHolder;
        if (_hexagonSlot && _hexagonSlot.hexagonHolder) _hexagonSlot.hexagonHolder = null;
        CanTouchHexagonHolder(false);
        gridHolder.hexagonHolder = this;
        gridHolder.ColliderState(false);
        gridHolder.ScanNeighborGrids();
        if (PvPController.Instance.orderIndex%2!=0 && PvPController.Instance.playerType==PlayerType.PLAYER)
        {
            LevelManager.Instance.GameOverCheck();    
        }
        if (!isChangeSlotHint && isOpponent) LevelManager.Instance.MoveCount++;
        PvPController.Instance.OrderChecker();
        AudioManager.Instance.PlayHaptic(HapticPatterns.PresetType.LightImpact);
        PlayerTypeGlobal = PvPController.Instance.playerType;
    }

    public void BounceHexagonsToAnotherHolder(HexagonHolder targetHolder)
    {
        StartCoroutine(JumpHexagonsToAnotherHolder(targetHolder));

        IEnumerator JumpHexagonsToAnotherHolder(HexagonHolder targetHolder)
        {
            //To find hexagons at the top
            var hexagonType = ReturnLastHexagonsType();
            List<HexagonElement> upperHexagonElementList = new();
            for (int i = hexagonElements.Count - 1; i >= 0; i--)
            {
                if (hexagonElements[i].hexagonType != hexagonType) break;
                upperHexagonElementList.Add(hexagonElements[i]);
            }

            if (targetHolder.hexagonElements.Count < 1)
            {
                Debug.LogError("Sequence has no element");
            }

            Transform jumpPos = targetHolder.hexagonElements.Last().transform;

            upperHexagonElementList = upperHexagonElementList.OrderBy(g => g.transform.position.y).ToList();
            upperHexagonElementList.Reverse();

            // Update lists
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                hexagonElements.Remove(upperHexagonElementList[i]);
                targetHolder.hexagonElements.Add(upperHexagonElementList[i]);
                upperHexagonElementList[i].transform.SetParent(targetHolder.transform);
            }

            //Bounce hexagons
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                var hexagonElement = upperHexagonElementList[i];
               // hexagonElement.StimulatorAmount(1);
                //hexagonElement.RotationAmount(.5f);
               // hexagonElement.MovementMuscleAmount(.13f);
                hexagonElement.transform.DOLocalJump(jumpPos.localPosition + new Vector3(0, _jumpOffet.y * (i + 1), 0),
                        .75f, 1, .4f)
                    .OnComplete(() => { });
                hexagonElement.transform.DOLocalRotate(
                    new Vector3(0, 0, IsObjectOnRight(jumpPos.position, hexagonElement.transform) ? -180 : 180), .4f,
                    RotateMode.LocalAxisAdd);
                if (i % 2 == 0) AudioManager.Instance.Play(AudioManager.AudioEnums.HexagonBounce, .4f);
                yield return new WaitForSeconds(.05f);
            }

            yield return new WaitForSeconds(.4f);
            isJumpStopped = true;
            if (hexagonElements.Count == 0)
            {
                hexagonCollider.enabled = false;
            }
        }
    }

    public void ClearHexagons()
    {
        StartCoroutine(ClearHexagonsCor());

        IEnumerator ClearHexagonsCor()
        {
            //To find hexagons at the top
            var hexagonType = ReturnLastHexagonsType();
            List<HexagonElement> upperHexagonElementList = new();
            for (int i = hexagonElements.Count - 1; i >= 0; i--)
            {
                if (hexagonElements[i].hexagonType != hexagonType) break;
                upperHexagonElementList.Add(hexagonElements[i]);
            }

            if (upperHexagonElementList.Count < _levelInfo.hexagonClearAmount) yield break;

            // RemoveFromList
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                hexagonElements.Remove(upperHexagonElementList[i]);
            }

            //Check if hexagon holder has hexagon element still
            if (hexagonElements.Count == 0 && gridHolder) gridHolder.CheckIfGridHolderEmpty();

            isClearHappening = true;
            var playerTypeAtStart = PvPController.Instance.playerType;
            //Clear Hexagon Anim
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                LevelManager.Instance.CollectedHexagonCount++;
                var tempHexagonElement = upperHexagonElementList[i];
                if (i < upperHexagonElementList.Count - 1)
                {
                    upperHexagonElementList[i + 1].StimulatorAmount(1f);
                    upperHexagonElementList[i + 1].MovementMuscleAmount(0);
                    upperHexagonElementList[i + 1].SqueezAmount(1f);
                }

                tempHexagonElement.fruitSmashVFX.gameObject.SetActive(true);
                tempHexagonElement.fruitSmashVFX.transform.SetParent(null);
                tempHexagonElement.transform
                    .DOScale(Vector3.zero, .2f)
                    .OnComplete(() => Destroy(tempHexagonElement.gameObject));
                if (i % 2 == 0) AudioManager.Instance.Play(AudioManager.AudioEnums.HexagonClear, .5f);
            //    PvPController.Instance.DecreaseHealth(playerTypeAtStart);
                if (i == upperHexagonElementList.Count - 1)
                {
                    int hexagonElementAmount = upperHexagonElementList.Count;
                    Vector3 trailPos = tempHexagonElement.transform.position;
                    var trail = TrailVFXPool.Instance.GetParticle();
                    
                    Vector3 offset = new Vector3(0, 2.2f, -1.15f) + trailPos;
                    ComboManager.Instance.IncreaseComboStage(offset);
                    int comboStage = ComboManager.Instance.comboStage;
                    ActivateTrail(PlayerTypeGlobal, trailPos,trail,hexagonElementAmount,comboStage);
                }
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.05f);
            isClearHappening = false;
            if (hexagonElements.Count > 0)
            {
                gridHolder.ScanNeighborGrids();
            }
            else
            {
                hexagonCollider.enabled = false;
            }
        }
    }

    public HexagonTypes ReturnLastHexagonsType()
    {
        if (hexagonElements.Count > 0) return hexagonElements.Last().hexagonType;
        else return HexagonTypes.NONE;
    }

    public void SortElementsListByTransformY()
    {
        hexagonElements = hexagonElements.OrderBy(g => g.transform.position.y).ToList();
    }

    private bool IsObjectOnRight(Vector3 target, Transform main)
    {
        return target.x > main.transform.position.x;
    }

    private void SlideAnim(HexagonSlot hexagonSlot)
    {
        StartCoroutine(SlideAnimAtStartCor(hexagonSlot));

        IEnumerator SlideAnimAtStartCor(HexagonSlot hexagonSlot)
        {
            if(transform is null) yield break;
            transform.DOMove(hexagonSlot.transform.position, .75f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => CanTouchHexagonHolder(true));
            transform.DORotate(new Vector3(0, 360, 0), .75f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(.65f);
        }
    }
    
    
    public void ActivateTrail(PlayerType playerTyp,Vector3 trailPos,TrailVFX trailVFX,int hexagonElementAmount,int comboStage=1)
    {
        trailVFX.gameObject.SetActive(true);
        trailVFX.transform.position=trailPos;
        trailVFX.TrailMotion(playerTyp,hexagonElementAmount,comboStage);
    }
    
    public void SetLayerRecursively(GameObject obj, LayerMask layerMask)
    {
        obj.layer = Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerMask);
        }
    }
}