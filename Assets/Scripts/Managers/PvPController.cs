using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum PlayerType
{
    PLAYER,
    OPPONENT
}

public enum PlayOrderState
{

}
public class PvPController : MonoBehaviour
{
    public static PvPController Instance;

    private ArrowRotator _arrowRotator;

    public PlayerType playerType;

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
    public void SelectFirstPlayer()
    {
        StartCoroutine(SelectFirstPlayerCor());
        IEnumerator SelectFirstPlayerCor()
        {
            EventManager.CoreEvents.HexagonHolderColliderState(false);
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
        playerType = PlayerType.OPPONENT;
        EventManager.CoreEvents.HexagonHolderColliderState(false);
    }

    private void PlayerState()
    {
        playerType= PlayerType.PLAYER;
        EventManager.CoreEvents.HexagonHolderColliderState(true);
    }
}
