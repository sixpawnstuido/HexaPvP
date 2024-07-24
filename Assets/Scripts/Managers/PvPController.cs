using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public enum PlayerType
{
    PLAYER,
    OPPONENT
}
public class PvPController : MonoBehaviour
{
    public static PvPController Instance;

    private ArrowRotator _arrowRotator;

    public PlayerType playerType;

    [SerializeField] private HandHolder handHolder;

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
    
    [Button]
    public void SelectFirstPlayer()
    {
        StartCoroutine(SelectFirstPlayerCor());
        IEnumerator SelectFirstPlayerCor()
        {
            if(EventManager.CoreEvents.HexagonHolderColliderState is not null) EventManager.CoreEvents.HexagonHolderColliderState(false);
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
            LevelManager.Instance.ReturnHexagonSpawner().SpawnOpponentHexagonHolder();
            playerType = PlayerType.OPPONENT;
            if (EventManager.CoreEvents.HexagonHolderColliderState is not null)
                EventManager.CoreEvents.HexagonHolderColliderState(false);
            
            yield return new WaitForSeconds(0.2f);
            
            var hexagonSpawner = LevelManager.Instance.ReturnHexagonSpawner();
            var hexagonSlotList = hexagonSpawner.hexagonSlotListOpponent;
            handHolder.StartPosToHexagonHolder(hexagonSlotList[0]);
            yield return new WaitForSeconds(handHolder.startPosToHexagonDuration+.2f);
            
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
            
            
            
        }
        
    }

    private void PlayerState()
    {
        playerType= PlayerType.PLAYER;
        if(EventManager.CoreEvents.HexagonHolderColliderState is not null) EventManager.CoreEvents.HexagonHolderColliderState(true);
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
}
