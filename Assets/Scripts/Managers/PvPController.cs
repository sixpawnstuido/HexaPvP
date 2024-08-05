using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;


public enum PlayerType
{
    PLAYER,
    OPPONENT
}

public class PvPController : SerializedMonoBehaviour
{
    public static PvPController Instance;

    private ArrowRotator _arrowRotator;

    public PlayerType playerType;

    [SerializeField] private HandHolder handHolder;

    [SerializeField] private Dictionary<PlayerType, AvatarElement> avatarDict;

    [ReadOnly] public int orderIndex;
    [ReadOnly] public int targetAmount;


    [ReadOnly] public bool isLevelEnd;

    private IEnumerator _opponentStateCor;

    [ReadOnly] public bool isExtraMove;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _arrowRotator = GetComponentInChildren<ArrowRotator>();
    }

    private void Start()
    {
        _opponentStateCor = OpponentStateCor();
    }

    [Button]
    public void SelectFirstPlayer()
    {
        StartCoroutine(SelectFirstPlayerCor());

        IEnumerator SelectFirstPlayerCor()
        {
            handHolder.gameObject.SetActive(true);
            HexagonMovement.PvPBlock = true;
            _arrowRotator.ActivateArrow();
            yield return new WaitUntil(() => _arrowRotator.isRotating);
            EventManager.SpawnEvents.SpawnHexagonHolder();
            if (_arrowRotator.ReturnPlayerType() == PlayerType.PLAYER)
            {
                PlayerState();
            }
            else
            {
                OpponentState();
            }
        }
    }

    public void OpponentState()
    {
        StartCoroutine(OpponentStateCor());
    }

    IEnumerator OpponentStateCor()
    {
        if (isLevelEnd) yield break;
        var playerTypeBefore = playerType;
        playerType = PlayerType.OPPONENT;
        HexagonMovement.PvPBlock = true;
       if(playerTypeBefore==PlayerType.PLAYER) OrderOfPlayPanel.Instance.PanelState(PlayerType.OPPONENT);
       var hexagonSpawner = LevelManager.Instance.ReturnHexagonSpawner();
       avatarDict[PlayerType.OPPONENT].Focus();
       avatarDict[PlayerType.PLAYER].Unfocus();
        yield return new WaitForSeconds(0.2f);

        // START POS TO HEXAGON

        var hexagonSlotList = hexagonSpawner.hexagonSlotListOpponent;
        if (hexagonSlotList[0].hexagonHolder is null)
        {
            hexagonSpawner.SpawnOpponentHexagonHolder(2);
            yield return new WaitUntil(()=>hexagonSlotList[0].hexagonHolder);
        }
        handHolder.StartPosToHexagonHolder(hexagonSlotList[0]);

        yield return new WaitForSeconds(handHolder.startPosToHexagonDuration + .2f);

        // HEXAGON TO GRID
        
        if (isLevelEnd) yield break;
        
        var gridHolder = LevelManager.Instance.ReturnGridHolderController().ReturnAvailableGridHolder();
        if (gridHolder)
        {
            handHolder.HexagonToGridHolder(gridHolder, hexagonSlotList[0].hexagonHolder);
        }
        else
        {
            Debug.LogError("No available grid");
            LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
            var gridHolderAvailable = LevelManager.Instance.ReturnGridHolderController().ReturnAvailableGridHolder();
            if (gridHolderAvailable)
            {
                handHolder.HexagonToGridHolder(gridHolderAvailable);
            }
        }

        yield return new WaitForSeconds(handHolder.hexagonToGridDuration + handHolder.hexagonHolderJumpDuration);

        
        if (isLevelEnd) yield break;
        
        //GRID TO HEXAGON
        handHolder.GridToHexagonHolder(hexagonSlotList[1].hexagonHolder);

        yield return new WaitForSeconds(handHolder.gridToHexagonDuration);

        // HEXAGON TO GRID
        if (isLevelEnd) yield break;
        
        if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
        bool isAllGridsOccupied2 = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
        if (isAllGridsOccupied2)
        {
            var gridController = LevelManager.Instance.ReturnGridHolderController();
            yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
            bool isAllGridsOccupiedStill = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
            if (isAllGridsOccupiedStill)
            {
                LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
            }
        }

        var gridHolder2 = LevelManager.Instance.ReturnGridHolderController().ReturnAvailableGridHolder();
        if (gridHolder2)
        {
            handHolder.HexagonToGridHolder(gridHolder2, hexagonSlotList[1].hexagonHolder);
        }
        else
        {
            Debug.LogError("No available grid");
            LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
            var gridHolderAvailable = LevelManager.Instance.ReturnGridHolderController().ReturnAvailableGridHolder();
            if (gridHolderAvailable)
            {
                handHolder.HexagonToGridHolder(gridHolderAvailable);
            }
        }

        yield return new WaitForSeconds(handHolder.hexagonToGridDuration + handHolder.hexagonHolderJumpDuration);
        // GRID TO STARTPOS
        handHolder.GoBackToStartPos();
    }

    public void StopOpponentStateCor()
    {
        StopCoroutine(_opponentStateCor);
        handHolder.DOKill();
        handHolder.GoBackToStartPos();
        handHolder.gameObject.SetActive(false);
        var hexaHolder = handHolder.transform.GetComponentInChildren<HexagonHolder>();
        if (hexaHolder is not null)
        {
            Destroy(hexaHolder.gameObject);
        }
    }

    private void PlayerState()
    {
        OrderOfPlayPanel.Instance.PanelState(PlayerType.PLAYER);
        playerType = PlayerType.PLAYER;
        HexagonMovement.PvPBlock = false;
        avatarDict[PlayerType.PLAYER].Focus();
        avatarDict[PlayerType.OPPONENT].Unfocus();
    }

    public void OrderChecker()
    {
        StartCoroutine(OrderCheckerCor());

        IEnumerator OrderCheckerCor()
        {

            var hexagonSpawner = LevelManager.Instance.ReturnHexagonSpawner();
            var orderCheck= playerType==PlayerType.PLAYER ? hexagonSpawner.ArePlayerSlotsEmpty() : hexagonSpawner.AreOpponentSlotsEmpty();
            if (orderCheck)
            {
                yield return new WaitForSeconds(0.1f);
                var gridController = LevelManager.Instance.ReturnGridHolderController();
                var hexagonHolderController = LevelManager.Instance.ReturnHexagonSpawnerHexagonHolderController();

                yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
                yield return new WaitUntil(() => !hexagonHolderController.CheckHexagonClearState());

                yield return new WaitForSeconds(0.1f);
                for (int i = 0; i < 5; i++)
                {
                    yield return new WaitForSeconds(0.05f);
                    yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
                    yield return new WaitForSeconds(0.05f);
                    yield return new WaitUntil(() => !hexagonHolderController.CheckHexagonClearState());
                }
              
                //If all grids occupied check
                if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
                bool isAllGridsOccupied = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
                if (isAllGridsOccupied)
                {
                    yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
                    bool isAllGridsOccupiedStill = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
                    if (isAllGridsOccupiedStill)
                    {
                        LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
                        yield return new WaitForSeconds(1);
                    }
                }
                Debug.Log("ExtraMoveUp");
                if (isExtraMove)
                {
                    isExtraMove=false;
                    SpawnHexagonsIfThereAreNotAny();
                    yield break;
                }
                LevelManager.Instance.HexagonHolderSpawnCheck();
                ComboManager.Instance.ResetComboStage();
                Debug.Log("ExtraMoveDown");
                if (playerType == PlayerType.PLAYER)
                {
                    OpponentState();
                }
                else
                {
                    PlayerState();
                }
            }
        }
    }

    public void SpawnHexagonsIfThereAreNotAny()
    {
        StartCoroutine(SpawnHexagonsIfThereAreNotAnyCor());
        IEnumerator SpawnHexagonsIfThereAreNotAnyCor()
        {
            yield return new WaitForSeconds(2);
            var hexagonSpawner = LevelManager.Instance.ReturnHexagonSpawner();
            bool playerSlotsState = hexagonSpawner.ArePlayerSlotsEmpty();
            bool opponentSlotsState = hexagonSpawner.AreOpponentSlotsEmpty();
            
            if (playerType==PlayerType.PLAYER && playerSlotsState)
            {
                Debug.LogError("Player state ended");
                if (opponentSlotsState)
                {
                    LevelManager.Instance.HexagonHolderSpawnCheck();
                }
                ComboManager.Instance.ResetComboStage();
                if (playerType == PlayerType.PLAYER)
                {
                    OpponentState();
                }
                else
                {
                    PlayerState();
                }
            }
        }
    }


    public void ResetAvatars()
    {
        var levelInfo = ResourceSystem.ReturnLevelInfo();
        targetAmount = levelInfo.levelInfoValues[LevelManager.Instance.LevelCount].desiredHexagonAmount;
        avatarDict.ForEach(avatar => avatar.Value.ResetAvatarElement(targetAmount));
    }

    public void DecreaseHealth(PlayerType playerType,int hexagonElementAmount,int comboStage=1)
    {
        if (isLevelEnd) return;
        var playerTypeTemp = playerType == PlayerType.PLAYER ? PlayerType.OPPONENT : PlayerType.PLAYER;
        avatarDict[playerTypeTemp].DecreaseHealth(hexagonElementAmount,comboStage);
    }


    public void LevelCompleted(PlayerType playerTypeCurrent)
    {
        if (isLevelEnd) return;
        isLevelEnd = true;
        StopOpponentStateCor();
        if (playerTypeCurrent == PlayerType.PLAYER)
        {
            FailState();
        }
        else
        {
            SuccessState();
        }
    }

    public void SuccessState()
    {
        LevelManager.Instance.OpenNextLevelPanel();
    }

    public void FailState()
    {
        LevelManager.Instance.OpenFailedPanel();
    }


    public AvatarElement ReturnAvatarElement(PlayerType player)
    {
        var avatarElement = player == PlayerType.PLAYER
            ? avatarDict[PlayerType.OPPONENT]
            : avatarDict[PlayerType.PLAYER];
        return avatarElement;
    }
}