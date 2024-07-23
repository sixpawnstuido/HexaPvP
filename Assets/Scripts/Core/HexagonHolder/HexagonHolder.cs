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

public class HexagonHolder : MonoBehaviour
{
    [HideInInspector] public Collider hexagonCollider;

    [ReadOnly] public List<HexagonElement> hexagonElements = new();

    [ReadOnly] public GridHolder gridHolder;

    [ReadOnly] public bool isJumpStopped;

    [SerializeField] private Vector3 _jumpOffet;

    private HexagonSlot _hexagonSlot;

    private HexagonMovement _hexagonMovement;

    private Dictionary<HexagonTypes, GameObject> _blenderFruits;

    private LevelHolder _levelHolder;

    private List<HexagonTypes> _targetHexagonTypes = new();

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
        _blenderFruits = ResourceSystem.ReturnVisualData().blenderFruits;
        var levelTargetUIList = ResourceSystem.ReturnLevelInfo().levelInfoValues[LevelManager.Instance.LevelCount]
            .targetUITypes;
        for (int i = 0; i < levelTargetUIList.Count; i++)
        {
            _targetHexagonTypes.Add(levelTargetUIList[i].hexagonType);
        }
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

    public void HexagonPlacedState(GridHolder gridHolder, bool isChangeSlotHint = false)
    {
        this.gridHolder = gridHolder;
        if (_hexagonSlot && _hexagonSlot.hexagonHolder) _hexagonSlot.hexagonHolder = null;
        CanTouchHexagonHolder(false);
        gridHolder.hexagonHolder = this;
        gridHolder.ColliderState(false);
        gridHolder.ScanNeighborGrids();
      //  gridHolder.CheckIfGridHolderOccupied = 1;
        LevelManager.Instance.GameOverCheck();
      //  if (!isChangeSlotHint) TutorialManager.Instance.IncreaseTutorialCount();
        if (!isChangeSlotHint) LevelManager.Instance.HexagonHolderSpawnCheck();
        if (!isChangeSlotHint) LevelManager.Instance.MoveCount++;
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
                hexagonElement.StimulatorAmount(1);
                //hexagonElement.RotationAmount(.5f);
                hexagonElement.MovementMuscleAmount(.13f);
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

    // ReSharper disable Unity.PerformanceAnalysis
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
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

            if (upperHexagonElementList.Count < 5) yield break;

            // RemoveFromList
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                hexagonElements.Remove(upperHexagonElementList[i]);
            }

            //Check if hexagon holder has hexagon element still
            if (hexagonElements.Count == 0 && gridHolder) gridHolder.CheckIfGridHolderEmpty();

            int hexaCount = 0;
            //Clear Hexagon Anim
            for (int i = 0; i < upperHexagonElementList.Count; i++)
            {
                LevelManager.Instance.CollectedHexagonCount++;
                //  ProgressBarController.Instance.UpdateProgressBar();
                // if (EventManager.CoreEvents.CheckIfLockUnlocked != null) EventManager.CoreEvents.CheckIfLockUnlocked();
                var tempHexagonElement = upperHexagonElementList[i];
                if (i < upperHexagonElementList.Count - 1)
                {
                    upperHexagonElementList[i + 1].StimulatorAmount(1f);
                    upperHexagonElementList[i + 1].MovementMuscleAmount(0);
                    upperHexagonElementList[i + 1].SqueezAmount(1f);
                }

                tempHexagonElement.fruitSmashVFX.gameObject.SetActive(true);
                tempHexagonElement.fruitSmashVFX.transform.SetParent(null);
                tempHexagonElement.transform.DOScale(Vector3.zero, .2f)
                    .OnComplete(() => Destroy(tempHexagonElement.gameObject));
                if (i % 2 == 0) AudioManager.Instance.Play(AudioManager.AudioEnums.HexagonClear, .5f);

                //hexaCount++;
                //if (hexaCount % 5 == 0)
                //{
                //    if (_targetHexagonTypes.Contains(hexagonType))
                //    {
                //        BlenderElement blenderElement =
                //            BlenderController.Instance.CheckIfBlenderElementAvailable(hexagonType);
                //        if (blenderElement)
                //        {
                //            var fruit = _blenderFruits[hexagonType];
                //            var instantiatedFruit = Instantiate(fruit);

                //            Vector3 offset = new Vector3(0, 2.2f, -1.15f) + transform.position;
                //            ComboManager.Instance.IncreaseComboStage(offset);

                //            if (TutorialManager.TutorialCompleted == 0)
                //            {
                //                instantiatedFruit.gameObject.layer = 10;
                //                instantiatedFruit.transform.GetChild(0).gameObject.layer = 10;
                //            }

                //            instantiatedFruit.transform.position = transform.position;
                //            blenderElement.OnBlendAnimStart(instantiatedFruit.GetComponent<FullFruitElement>());
                //        }
                //    }
                //    else
                //    {
                //        // Vector3 offset = new Vector3(0,2.2f,-1.15f);
                //        // GoldPanel.Instance.ActivateGoldAnim(tempHexagonElement.transform.position+offset);
                //        LevelManager.Instance.SortedFruitCount++;
                //        if (LevelManager.Instance.SortedFruitCount <= 30 && LevelManager.Instance.LevelCount != 1)
                //        {

                //        }
                //    }
                //}

                yield return new WaitForSeconds(.05f);
            }

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
            // transform.DOScale(Vector3.one * 1.2f, .1f).SetEase(Ease.Flash)
            //.OnComplete(() => transform.DOScale(Vector3.one, .1f));
        }
    }

    //private void HexagonElementBoneStimulatorState(bool state)
    //{
    //    for (int i = 0; i < hexagonElements.Count; i++)
    //    {
    //        hexagonElements[i].BoneStimulatorActiveState(state);
    //    }
    //}
}