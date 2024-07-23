using DG.Tweening;
using Lofelt.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class HexagonMovement : MonoBehaviour
{
    public static bool HexagonClickBlock;

    private float _distance;

    private Camera _camera;

    private Vector3 _fingerDistance;
    private Vector3 _firstPos;

    private LayerMask _gridHolderLayerMask;

    private HexagonHolder _hexagonHolder;

    private ChangeSlotHint _changeSlotHint;
    private ClearSlotHint _clearSlotHint;

    [SerializeField] private float _planeY = 2;
    public bool _canMove;

    public static bool isMoving;
    private void Awake()
    {
        _camera = FindObjectOfType<MainSceneCamera>(true).GetComponent<Camera>();
        _gridHolderLayerMask = LayerMask.GetMask("Grid");
        _firstPos = transform.position;
        _hexagonHolder = GetComponent<HexagonHolder>();
        _changeSlotHint = UIManager.Instance.changeSlotHint;
        _clearSlotHint = UIManager.Instance.clearSlotHint;
    }
    private void Start()
    {
        var globalVariables = ResourceSystem.ReturnGlobalVariablesData();
        _fingerDistance = globalVariables is not null ? globalVariables.hexagonHolderFingerDistance : Vector3.up / 2;
    }

    private void OnMouseDown()
    {
        if (HexagonClickBlock) return;

        if (_changeSlotHint.isHintActive && _hexagonHolder.gridHolder is null || _clearSlotHint.isHintActive)
        {
            return;
        }
        _canMove = true;
        AudioManager.Instance.Play(AudioManager.AudioEnums.Tap, .3f);
        isMoving = true;
        //List<HexagonElement> fruits = _hexagonHolder.hexagonElements;

        ////for (int i = 0; i < fruits.Count; i++)
        ////{
        ////    fruits[i].StimulatorAmount(.5f);
        ////    fruits[i].MovementMuscleAmount(.2f);
        ////    fruits[i].SqueezAmount(0);

        ////}
    }
    private void Update()
    {
        if (!_canMove) return;
        Plane plane = new Plane(Vector3.up, Vector3.up * _planeY);
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        float distance=0;
        Debug.DrawRay(_camera.transform.position, ray.direction * 1000f, Color.red);
        if (plane.Raycast(ray, out distance))
        {
            transform.position = ray.GetPoint(distance) + _fingerDistance;
        }
    }
    private void OnMouseUp()
    {
        if (!_canMove) return;
        OnClickEndState();
        isMoving = false;
    }

    private void OnClickEndState()
    {
        _canMove = false;
        var gridHolder = RaycastManager.SendRayFromCameraToMousePos<GridHolder>(_gridHolderLayerMask, true, _fingerDistance);
        bool isHintActive = _changeSlotHint.isHintActive;
        Vector3 gridOffset = new Vector3(0, .05f, 0);
        if (gridHolder)
        {
            if (isHintActive)
            {
                _hexagonHolder.gridHolder.hexagonHolder = null;
                _hexagonHolder.gridHolder.ColliderState(true);
                _hexagonHolder.gridHolder.SaveHexagonHolder();
                _changeSlotHint.Unfocus();
                _changeSlotHint.DecreaseMoveCount();
            }
            _hexagonHolder.hexagonCollider.enabled=false;
            transform.DOMove(gridHolder.transform.position + gridOffset, .2f)
            .OnComplete(() => _hexagonHolder.HexagonPlacedState(gridHolder, isHintActive));
            AudioManager.Instance.Play(AudioManager.AudioEnums.HexagonHolderPlaced, .4f);
        }
        else
        {
            AudioManager.Instance.Play(AudioManager.AudioEnums.WrongMove, .4f);
            AudioManager.Instance.PlayHaptic(HapticPatterns.PresetType.MediumImpact);
            transform.DOMove(isHintActive ? _hexagonHolder.gridHolder.transform.position + gridOffset : _firstPos, .2f);
        }

    }
    public void ChangeFirstTarget(Transform target)
    {
        _firstPos = target.position;
    }
}
