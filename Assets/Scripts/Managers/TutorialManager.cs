using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private GameObject _firstHand;
    [SerializeField] private GameObject _secondHand;
    [SerializeField] private GameObject _thirdHand;
    [SerializeField] private GameObject _fourthHand;
    [SerializeField] private GameObject _fifthHand;
    [SerializeField] private GameObject _handHolder;
    [SerializeField] private GameObject _dragAndDropText;
    [SerializeField] private GameObject targetTutorial;
    [SerializeField] private GameObject secondCam;
    [SerializeField] private GameObject blenderTutorial;

    [SerializeField] private Transform firstBlender;

    public GameObject fullFruitTutorial;

    private LayerMask _hexagonHolderLayerMask = 1 << 7;
    private LayerMask _gridHolderLayerMask = 1 << 6;

    private int _tutorialCount;

    public List<BoxCollider> slots;

    [ShowInInspector, ReadOnly] public static bool TargetTutorialCompleted;
    [ShowInInspector, ReadOnly] public static bool FullFruitStart;
    [ShowInInspector, ReadOnly] public static bool FullFruitEnd;

    [ShowInInspector,ReadOnly]
    public int TutorialCount
    {
        get => PlayerPrefs.GetInt("TutorialCount", 0);
        set => PlayerPrefs.SetInt("TutorialCount", value); 
    }

    [ShowInInspector, ReadOnly]
    public static int TutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0);
        set => PlayerPrefs.SetInt("TutorialCompleted", value);
    }

    private void Awake()
    {
        Instance = this;
        if (TutorialCompleted == 0)
        {
            ES3.DeleteFile();
            PlayerPrefs.DeleteAll();
        }

    }


    IEnumerator Start()
    {
        _tutorialCount = TutorialCount;
        yield return new WaitForSeconds(2);
        if (TutorialCompleted==0)
        {
            TutorialCount = 0;
            Tutorial();
            _dragAndDropText.SetActive(true);
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].enabled = true;
            }
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var hexagonHolder = RaycastManager.SendRayFromCameraToMousePos<HexagonHolder>(_hexagonHolderLayerMask);
            if (hexagonHolder)
            {
                _handHolder.SetActive(false);
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var gridHolder = RaycastManager.SendRayFromCameraToMousePos<GridHolder>(_gridHolderLayerMask);
            if (!gridHolder)
            {
                _handHolder.SetActive(true);
            }
            else
            {
                DOVirtual.DelayedCall(.5f, () => _handHolder.SetActive(true));
            }
        }
    }
    public void Tutorial()
    {
        StartCoroutine(FirstStepCor());
        IEnumerator FirstStepCor()
        {

            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => slots!=null);
            slots[0].enabled = true;
            _firstHand.SetActive(true);
            yield return new WaitUntil(() => TutorialCount == 1);
            _firstHand.SetActive(false);
            _secondHand.SetActive(true);
            slots[1].enabled = true;
            yield return new WaitUntil(() => TutorialCount == 2);
            _firstHand.SetActive(false);
            _secondHand.SetActive(false);
            _thirdHand.SetActive(true);
            slots[2].enabled = true;
            yield return new WaitUntil(() => TutorialCount == 3);


            _firstHand.SetActive(false);
            _secondHand.SetActive(false);
            _thirdHand.SetActive(false);
            _dragAndDropText.SetActive(false);

            yield return new WaitForSeconds(.5f);
            targetTutorial.SetActive(true);

            yield return new WaitForSeconds(2);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

            targetTutorial.SetActive(false);

            _fourthHand.SetActive(true);

            slots[3].enabled = true;
            yield return new WaitUntil(() => TutorialCount == 4);

            _fourthHand.SetActive(false);
            _fifthHand.SetActive(true);
            slots[4].enabled = true;
            yield return new WaitUntil(() => TutorialCount == 5);
            _fifthHand.SetActive(false);


            yield return new WaitUntil(() => FullFruitStart);

            fullFruitTutorial.SetActive(true);
            secondCam.SetActive(true);


            yield return new WaitForSeconds(2);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

            secondCam.SetActive(false);
            fullFruitTutorial.SetActive(false);
            FullFruitEnd = true;

            HexagonMovement.HexagonClickBlock = true;

            yield return new WaitForSeconds(2);

            blenderTutorial.SetActive(true);
            secondCam.SetActive(true);

            SetGameLayerRecursive(firstBlender.gameObject,10);

            yield return new WaitForSeconds(2);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));

            SetGameLayerRecursive(firstBlender.gameObject, 0);
            secondCam.SetActive(false);
            blenderTutorial.SetActive(false);

            HexagonMovement.HexagonClickBlock = false;

            for (int i = 0; i < slots.Count; i++)
            {
                if (i == 0 || i == 2) continue;
                slots[i].enabled = true;
            }

            TutorialCompleted = 1;

        }
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }

    public void IncreaseTutorialCount()
    {
        if (TutorialCount < 5) { TutorialCount++; _tutorialCount = TutorialCount; }
    }
}
