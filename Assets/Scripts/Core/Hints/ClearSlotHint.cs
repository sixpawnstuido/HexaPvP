using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearSlotHint : HintBase
{
    [SerializeField] private GameObject focusCloseButton;

    private void Awake()
    {

        focusCanvasGroup.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            Unfocus();
            AudioManager.Instance.Play(AudioManager.AudioEnums.ButtonTap);
        });
    }

    protected override void Start()
    {
        base.Start();
        hintButton.onClick.AddListener(() => { OnButtonClick(); });
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            tutorial.SetActive(true);
        }
    }

    public void OnButtonClick()
    {
        if (HintActivated <= 0) return;
        GridHolderController gridHolderController = LevelManager.Instance.ReturnGridHolderController();
        if (gridHolderController.HowManyGridsOccupied().Item2 <= 0) return;
        if (HintCount > 0)
        {
            Focus();

            AudioManager.Instance.Play(AudioManager.AudioEnums.HintTap);
        }
        else
        {
            AudioManager.Instance.Play(AudioManager.AudioEnums.Boxy);
            buyPanel.SetActive(true);
        }
    }

    public void OnButtonClickCustom()
    {

        GridHolderController gridHolderController = LevelManager.Instance.ReturnGridHolderController();
        if (gridHolderController.HowManyGridsOccupied().Item2 <= 0) return;

        Focus(true);


    }

    public void FailedClick()
    {
         OnButtonClickCustom();
    }



    public void OpenTutorial()
    {
        if (HintActivated == 1) return;
        tutorial.SetActive(true);
    }


    public void Focus(bool fromRevive=false)
    {
        var inGameCanvasGroup = UIManager.Instance.allCanvases[CanvasTypes.InGameCanvas].GetComponent<CanvasGroup>();

        secondCam.SetActive(true);

        if (fromRevive) focusCloseButton.SetActive(false);
        else focusCloseButton.SetActive(true);

        List<GridHolder> grids = LevelManager.Instance.ReturnGridHolderController().ReturnFullGrids();

        for (int i = 0; i < grids.Count; i++)
        {
            grids[i].gameObject.layer = 10;

            SetGameLayerRecursive(grids[i].hexagonHolder.gameObject, 10);
        }

        focusCanvasGroup.blocksRaycasts = true;
        hintParentCanvasGroup.interactable = false;
        DOVirtual.Float(1, 0, .2f, (v) => inGameCanvasGroup.alpha = v);
        DOVirtual.Float(1, 0, .2f, (v) => hintParentCanvasGroup.alpha = v);
        DOVirtual.DelayedCall(0f, () => DOVirtual.Float(0, 1, .2f, (v) => focusCanvasGroup.alpha = v));
        isHintActive = true;
        EventManager.CoreEvents.GridHolderColliderState(true, false);
    }

    public void Unfocus()
    {

        var inGameCanvasGroup = UIManager.Instance.allCanvases[CanvasTypes.InGameCanvas].GetComponent<CanvasGroup>();

        focusCanvasGroup.blocksRaycasts = false;
        hintParentCanvasGroup.interactable = true;
        DOVirtual.Float(1, 0, .2f, (v) => focusCanvasGroup.alpha = v);
        DOVirtual.DelayedCall(0f, () =>
        {
            DOVirtual.Float(0, 1, .2f, (v) => hintParentCanvasGroup.alpha = v);
            DOVirtual.Float(0, 1, .2f, (v) => inGameCanvasGroup.alpha = v);
        });

        secondCam.SetActive(false);

        List<GridHolder> grids = LevelManager.Instance.ReturnGridHolderController().ReturnAllGrids();

        for (int i = 0; i < grids.Count; i++)
        {
            grids[i].gameObject.layer = 6;

            if (grids[i].hexagonHolder != null)
            {
                SetGameLayerRecursive(grids[i].hexagonHolder.gameObject, 6);
            }
        }

        isHintActive = false;
    }
}