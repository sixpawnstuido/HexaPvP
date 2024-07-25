using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sirenix.OdinInspector;
using UnityEditor;
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

    public int orderIndex;

    private LevelInfo _levelInfo;

    public int targetAmount;

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

        _arrowRotator= GetComponentInChildren<ArrowRotator>();
    }

    private void Start()
    {
        _levelInfo = ResourceSystem.ReturnLevelInfo();
    }

    [Button]
    public void SelectFirstPlayer()
    {
        StartCoroutine(SelectFirstPlayerCor());
        IEnumerator SelectFirstPlayerCor()
        {
            HexagonMovement.PvPBlock = true;
            _arrowRotator.ActivateArrow();
            yield return new WaitUntil(()=> _arrowRotator.isRotating);
            if (_arrowRotator.ReturnPlayerType()==PlayerType.PLAYER)
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
        IEnumerator OpponentStateCor()
        {
            playerType = PlayerType.OPPONENT;
            HexagonMovement.PvPBlock = true;
            avatarDict[PlayerType.OPPONENT].AvatarImageColor(Color.green);
            avatarDict[PlayerType.PLAYER].AvatarImageColor(Color.white);
            yield return new WaitForSeconds(0.2f);
            
            // START POS TO HEXAGON
            var hexagonSpawner = LevelManager.Instance.ReturnHexagonSpawner();
            var hexagonSlotList = hexagonSpawner.hexagonSlotListOpponent;
            handHolder.StartPosToHexagonHolder(hexagonSlotList[0]);
            
            yield return new WaitForSeconds(handHolder.startPosToHexagonDuration+.2f);
            
            // HEXAGON TO GRID

            if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
            bool isAllGridsOccupied = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
            if (isAllGridsOccupied)
            {
                var gridController = LevelManager.Instance.ReturnGridHolderController();
                yield return new WaitUntil(()=>!gridController.IsThereAnyGridBouncing());
                bool isAllGridsOccupiedStill = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
                if (isAllGridsOccupiedStill)
                {
                    LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
                }
            }

            var gridHolder = LevelManager.Instance.ReturnGridHolderController().ReturnAvailableGridHolder();
            if (gridHolder)
            {
                handHolder.HexagonToGridHolder(gridHolder,hexagonSlotList[0].hexagonHolder);
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

            yield return new WaitForSeconds(handHolder.hexagonToGridDuration+handHolder.hexagonHolderJumpDuration);
            
            //GRID TO HEXAGON
            handHolder.GridToHexagonHolder(hexagonSlotList[1].hexagonHolder);
            
            yield return new WaitForSeconds(handHolder.gridToHexagonDuration);
            
            // HEXAGON TO GRID

            if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
            bool isAllGridsOccupied2 = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
            if (isAllGridsOccupied2)
            {
                var gridController = LevelManager.Instance.ReturnGridHolderController();
                yield return new WaitUntil(()=>!gridController.IsThereAnyGridBouncing());
                bool isAllGridsOccupiedStill = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
                if (isAllGridsOccupiedStill)
                {
                    LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
                }
            }

            var gridHolder2 = LevelManager.Instance.ReturnGridHolderController().ReturnAvailableGridHolder();
            if (gridHolder2)
            {
                handHolder.HexagonToGridHolder(gridHolder2,hexagonSlotList[1].hexagonHolder);
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
            
            yield return new WaitForSeconds(handHolder.hexagonToGridDuration+handHolder.hexagonHolderJumpDuration);
            // GRID TO STARTPOS
            handHolder.GoBackToStartPos();
            
        }
        
    }

    private void PlayerState()
    {
        playerType= PlayerType.PLAYER;
        avatarDict[PlayerType.PLAYER].AvatarImageColor(Color.green);
        avatarDict[PlayerType.OPPONENT].AvatarImageColor(Color.white);
        HexagonMovement.PvPBlock = false;
    }
    
    
    public void GameOverCheck()
    {
        StartCoroutine(GameOverCheckCor());
        IEnumerator GameOverCheckCor()
        {
            if (EventManager.SpawnEvents.CheckIfAllGridsOccupied is null) yield break;
            bool isAllGridsOccupied = EventManager.SpawnEvents.CheckIfAllGridsOccupied();
            if (isAllGridsOccupied)
            {
                var gridController = LevelManager.Instance.ReturnGridHolderController();
                if (gridController.IsThereAnyGridBouncing())
                {
                    yield return new WaitForSeconds(2);
                    GameOverCheck();
                }
                else
                {
                  LevelManager.Instance.ReturnGridHolderController().ClearRandomGrids();
                }
            }
        }
    }


    public void OrderChecker()
    {
        StartCoroutine(OrderCheckerCor());
        IEnumerator OrderCheckerCor()
        { 
            orderIndex++;
            LevelManager.Instance.SpawnCount++;
            if (orderIndex%2==0)
            {
                var gridController = LevelManager.Instance.ReturnGridHolderController();
                yield return new WaitUntil(()=>!gridController.IsThereAnyGridBouncing());
                LevelManager.Instance.HexagonHolderSpawnCheck();
                yield return new WaitForSeconds(0.5f);
                if (playerType==PlayerType.PLAYER)
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


    public void SetTargetAmount(int targetAmount)
    {
        this.targetAmount = targetAmount;
    }
}
