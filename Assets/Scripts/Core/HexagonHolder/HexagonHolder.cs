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

    public void Init(HexagonSlot hexagonSlot)
    {
        hexagonSlot.hexagonHolder = this;
        _hexagonSlot = hexagonSlot;
        SlideAnim(hexagonSlot);
        SortElementsListByTransformY();
        _hexagonMovement.ChangeFirstTarget(hexagonSlot.transform);
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
        LevelManager.Instance.GameOverCheck();
      //  if (!isChangeSlotHint) LevelManager.Instance.HexagonHolderSpawnCheck();
        if (!isChangeSlotHint && isOpponent) LevelManager.Instance.MoveCount++;
        PvPController.Instance.OrderChecker();
        AudioManager.Instance.PlayHaptic(HapticPatterns.PresetType.LightImpact);
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

            var playerTypeAtStart = PvPController.Instance.playerType;
            
            //To find hexagons at the top
            var hexagonType = ReturnLastHexagonsType();
            List<HexagonElement> upperHexagonElementList = new();
            for (int i = hexagonElements.Count - 1; i >= 0; i--)
            {
                if (hexagonElements[i].hexagonType != hexagonType) break;
                upperHexagonElementList.Add(hexagonElements[i]);
            }

            if (upperHexagonElementList.Count < 5) yield break;

            // RemoveFromList
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                hexagonElements.Remove(upperHexagonElementList[i]);
            }

            //Check if hexagon holder has hexagon element still
            if (hexagonElements.Count == 0 && gridHolder) gridHolder.CheckIfGridHolderEmpty();

            int hexaCount = 0;
            isClearHappening = true;
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
                PvPController.Instance.DecreaseHealth(playerTypeAtStart);
                tempHexagonElement.ActivateTrail();
                yield return new WaitForSeconds(.05f);
            }
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

    [Button]
    private bool IsObjectOnRight(Vector3 target, Transform main)
    {
        return target.x > main.transform.position.x;
    }

    private void SlideAnim(HexagonSlot hexagonSlot)
    {
        StartCoroutine(SlideAnimAtStartCor(hexagonSlot));

        IEnumerator SlideAnimAtStartCor(HexagonSlot hexagonSlot)
        {
            transform.DOMove(hexagonSlot.transform.position, .75f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => CanTouchHexagonHolder(true));
            transform.DORotate(new Vector3(0, 360, 0), .75f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(.65f);
        }
    }
}