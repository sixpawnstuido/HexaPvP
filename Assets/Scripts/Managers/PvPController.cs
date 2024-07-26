using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

    [Button]
    public void SelectFirstPlayer()
    {
        StartCoroutine(SelectFirstPlayerCor());

        IEnumerator SelectFirstPlayerCor()
        {
            HexagonMovement.PvPBlock = true;
            _arrowRotator.ActivateArrow();
            yield return new WaitUntil(() => _arrowRotator.isRotating);
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

    private void OpponentState()
    {
        StartCoroutine(OpponentStateCor());
    }

    IEnumerator OpponentStateCor()
    {
        if (isLevelEnd) yield break;
        playerType = PlayerType.OPPONENT;
        HexagonMovement.PvPBlock = true;

        yield return new WaitForSeconds(0.2f);
        // avatarDict[PlayerType.PLAYER].SetColor(true);
        // avatarDict[PlayerType.OPPONENT].SetColor(false);

        // START POS TO HEXAGON
        var hexagonSpawner = LevelManager.Instance.ReturnHexagonSpawner();
        var hexagonSlotList = hexagonSpawner.hexagonSlotListOpponent;
        handHolder.StartPosToHexagonHolder(hexagonSlotList[0]);

        yield return new WaitForSeconds(handHolder.startPosToHexagonDuration + .2f);

        // HEXAGON TO GRID

        if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
        bool isAllGridsOccupied = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
        if (isAllGridsOccupied)
        {
            var gridController = LevelManager.Instance.ReturnGridHolderController();
            yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
            bool isAllGridsOccupiedStill = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
            if (isAllGridsOccupiedStill)
            {
                LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
            }
        }

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

        //GRID TO HEXAGON
        handHolder.GridToHexagonHolder(hexagonSlotList[1].hexagonHolder);

        yield return new WaitForSeconds(handHolder.gridToHexagonDuration);

        // HEXAGON TO GRID

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
        StopCoroutine(OpponentStateCor());
        handHolder.GoBackToStartPos();
    }

    private void PlayerState()
    {
        playerType = PlayerType.PLAYER;
        // avatarDict[PlayerType.PLAYER].SetColor(false);
        // avatarDict[PlayerType.OPPONENT].SetColor(true);
        HexagonMovement.PvPBlock = false;
    }

    public void OrderChecker()
    {
        StartCoroutine(OrderCheckerCor());

        IEnumerator OrderCheckerCor()
        {
            orderIndex++;
            LevelManager.Instance.SpawnCount++;
            if (orderIndex % 2 == 0)
            {
                yield return new WaitForSeconds(0.1f);
                var gridController = LevelManager.Instance.ReturnGridHolderController();
                var hexagonHolderController = LevelManager.Instance.ReturnHexagonSpawnerHexagonHolderController();
                yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
                yield return new WaitForSeconds(0.1f);
                for (int i = 0; i < 5; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    yield return new WaitUntil(() => !hexagonHolderController.CheckHexagonClearState());
                    yield return new WaitUntil(() => !gridController.AreThereAnyHexagonBouncing());
                }
                LevelManager.Instance.HexagonHolderSpawnCheck();
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
        avatarDict.ForEach(avatar => avatar.Value.SetTargetAmount(targetAmount));
        avatarDict.ForEach(avatar => avatar.Value.SetFillAmount());
        avatarDict.ForEach(avatar => avatar.Value.SetHealthText());
    }

    public void DecreaseHealth(PlayerType playerType)
    {
        if (isLevelEnd) return;
        var playerTypeTemp = playerType == PlayerType.PLAYER ? PlayerType.OPPONENT : PlayerType.PLAYER;
        avatarDict[playerTypeTemp].DecreaseHealth();
    }


    public void LevelCompleted()
    {
        if (isLevelEnd) return;
        isLevelEnd = true;
        if (playerType == PlayerType.PLAYER)
        {
            SuccessState();
            StopOpponentStateCor();
        }
        else
        {
            FailState();
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


    public AvatarElement ReturnTarget()
    {
        var avatarElement = playerType == PlayerType.PLAYER
            ? avatarDict[PlayerType.OPPONENT]
            : avatarDict[PlayerType.PLAYER];
        return avatarElement;
    }
}