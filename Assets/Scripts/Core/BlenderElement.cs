using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LiquidVolumeFX;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlenderElement : SerializedMonoBehaviour
{
    private BlenderController _blenderController;

    [ReadOnly] public HexagonTypes fruitType;

    public float juiceAmount = .1f;

    public int juiceLevel = 0;

    public bool isProcessing;

    [SerializeField] private GameObject _blenderModel;
    [SerializeField] private GameObject _lock;

    public Dictionary<HexagonTypes, Animator> referenceHexagons;
    public Dictionary<HexagonTypes, Color32> juiceColors;

    [ShowInInspector, ReadOnly]
    private Dictionary<HexagonTypes, Transform> referenceHexagonsTransforms = new Dictionary<HexagonTypes, Transform>();

    [SerializeField] private LiquidVolume juice;

    [SerializeField] private FillBar _fillBar;

    [SerializeField] private ParticleSystem juiceVFX;

    [SerializeField] private Transform refParent;

    private bool isFinished = true;

    [SerializeField] private Animator blenderAnim;
    [SerializeField] private Animator mainScaleAnim;

    [SerializeField] private List<ParticleSystem> dropVFX;
    [SerializeField] private List<ParticleSystem> dropVFX2;

    [SerializeField] private GlassHolder glassPrefab;

    [SerializeField] private Transform glassParent;

    [SerializeField] private Animator modelAnim;
    [ReadOnly] public Animator ModelAnim => modelAnim;

    private BlenderReset _blenderReset;

    public int BlenderFruitCount
    {
        get => PlayerPrefs.GetInt("BlenderFruitCount" + LevelManager.Instance.LevelCount + transform.GetSiblingIndex(),
            0);
        set => PlayerPrefs.SetInt("BlenderFruitCount" + LevelManager.Instance.LevelCount + transform.GetSiblingIndex(),
            value);
    }

    public bool IsBlenderOpened
    {
        get => Convert.ToBoolean(
            PlayerPrefs.GetInt("IsBlenderOpened" + LevelManager.Instance.LevelCount + transform.GetSiblingIndex(), 0));
        set => PlayerPrefs.SetInt("IsBlenderOpened" + LevelManager.Instance.LevelCount + transform.GetSiblingIndex(),
            Convert.ToInt32(value));
    }

    public int FruitType
    {
        get => PlayerPrefs.GetInt("FruitType" + LevelManager.Instance.LevelCount + transform.GetSiblingIndex(), 0);
        set => PlayerPrefs.SetInt("FruitType" + LevelManager.Instance.LevelCount + transform.GetSiblingIndex(), value);
    }

    public bool firstBlender;

    private void Awake()
    {
        _blenderController = GetComponentInParent<BlenderController>();
        if (_blenderModel.activeInHierarchy)
        {
            IsBlenderOpened = true;
        }

        _blenderReset = GetComponent<BlenderReset>();
    }

    private void OnEnable()
    {
        if (GetComponent<BlenderTimer>() is not null)
        {
            ResetBlender();
        }
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => ResourceSystem.ReturnVisualData() != null);

        if (IsBlenderOpened)
        {
            OpenBlender();
            RegisterItselfToBlenderController();
        }

        if (BlenderFruitCount > 0)
        {
            BlenderElementSave();
        }

        foreach (var item in referenceHexagons)
        {
            referenceHexagonsTransforms.Add(item.Key, item.Value.transform);
        }

        if (LevelManager.Instance.LevelCount == 1 && TutorialManager.TutorialCompleted == 0 && firstBlender)
        {
            BlenderFirstCustom();

        }
        if(_blenderReset) _blenderReset.ButtonScaleState();
    }

    public void UpdateBlender(HexagonTypes Type)
    {
        juice.liquidColor1 = juiceColors[Type];

        for (int i = 0; i < dropVFX.Count; i++)
        {
            var main = dropVFX[i].main;
            main.startColor = (Color)juiceColors[Type];
        }

        for (int i = 0; i < dropVFX2.Count; i++)
        {
            var main = dropVFX2[i].main;
            main.startColor = (Color)juiceColors[Type];
        }

        var main2 = juiceVFX.main;

        main2.startColor = (Color)juiceColors[Type];
    }

    public void RegisterItselfToBlenderController()
    {
        if(!_blenderController.blenderElementList.Contains(this)) _blenderController.blenderElementList.Add(this);
    }
    public void UnregisterItselfToBlenderController()
    {
        if(_blenderController.blenderElementList.Contains(this)) _blenderController.blenderElementList.Remove(this);
    }
    public void OnBlendAnimStart(FullFruitElement fruit)
    {
        StartCoroutine(OnBlendAnimStartCor());

        IEnumerator OnBlendAnimStartCor()
        {
            isProcessing = true;
            BlenderFruitCount++;
            _fillBar.FillBarStep(BlenderFruitCount);

            BlenderFoundedAnim(fruit);

            AudioManager.Instance.Play(AudioManager.AudioEnums.Boxy,1);

            yield return new WaitForSeconds(1);
            isProcessing = false;
        }
    }

    public void OpenBlender()
    {
        _blenderModel.SetActive(true);
        _lock.SetActive(false);
        IsBlenderOpened = true;
        RegisterItselfToBlenderController();
    }

    private void BlenderFoundedAnim(FullFruitElement fullFruit)
    {
        UpdateBlender(fullFruit.Type);
        StartCoroutine(BlenderAnimCor(fullFruit));
    }

    private IEnumerator BlenderAnimCor(FullFruitElement fullFruit)
    {
        Transform targetPos = referenceHexagonsTransforms[fullFruit.Type].transform;
        GameObject targetPos2 = referenceHexagons[fullFruit.Type].gameObject;

        juice.gameObject.SetActive(true);

        TutorialManager.FullFruitStart = true;

        if (TutorialManager.TutorialCompleted == 0)
        {
            yield return new WaitUntil(() => TutorialManager.FullFruitEnd);
            fullFruit.gameObject.layer = 0;
        }
   

  
        fullFruit.transform.GetChild(0).gameObject.layer = 0;
        fullFruit.transform.DOJump(targetPos.position, 4, 1, .7f).SetEase(Ease.Flash);
        fullFruit.transform.DOScale(targetPos.localScale, .7f);

        juiceLevel++;
        if(_blenderReset) _blenderReset.ButtonScaleState();
        yield return new WaitForSeconds(.6f);

        if (juiceLevel == 2)
        {
            AudioManager.Instance.Play(AudioManager.AudioEnums.Aquatic);
        }

        if (juiceLevel == 3)
        {
            dropVFX[1].Play();
            AudioManager.Instance.Play(AudioManager.AudioEnums.Aquatic);
        }

        if (juiceLevel == 4)
        {
            dropVFX[2].Play();
            AudioManager.Instance.Play(AudioManager.AudioEnums.Aquatic);
        }


        yield return new WaitForSeconds(.1f);

        _fillBar.gameObject.SetActive(true);

        yield return new WaitUntil(() => isFinished);

        isFinished = false;

        juiceVFX.Play();


        blenderAnim.SetTrigger("Blend");

        fullFruit.gameObject.SetActive(false);

        GameObject tmpFruit = Instantiate(targetPos2, targetPos2.transform.position, targetPos2.transform.rotation,
            refParent);

        tmpFruit.transform.localScale = targetPos2.transform.localScale;

        tmpFruit.SetActive(true);

        mainScaleAnim.ResetTrigger("Scale");
        mainScaleAnim.Update(0);
        mainScaleAnim.Rebind();
        mainScaleAnim.SetTrigger("Scale");

        AudioManager.Instance.Play(AudioManager.AudioEnums.Rattle, 1);

        if (juiceLevel < 3) yield return new WaitForSeconds(.6f);
        else yield return new WaitForSeconds(0);

        float targetAmount = juiceAmount + .2f;

        DOVirtual.Float(juiceAmount, targetAmount, .7f, (v) => juice.level = v);

        juiceAmount += .2f;

        yield return new WaitForSeconds(.2f);


        juiceVFX.Stop();

        blenderAnim.SetTrigger("Stop");

        yield return new WaitForSeconds(.5f);

        Destroy(tmpFruit.gameObject);

        isFinished = true;
        if (juiceAmount >= .85f)
        {
            //yield return new WaitForSeconds(.1f);

            isProcessing = true;
            _fillBar.gameObject.SetActive(false);

            modelAnim.SetTrigger("Push");
            AudioManager.Instance.Play(AudioManager.AudioEnums.BlenderPush,1);
            yield return new WaitForSeconds(.2f);

            var tmpType = fruitType;

            bool isUIOk = BlenderFruitCount == _blenderController.DesiredFruitAmount;

            ResetBlender();

            GlassHolder tmpGlass = Instantiate(glassPrefab, glassParent);

            tmpGlass.InitGlass(tmpType);

            yield return new WaitForSeconds(1.75f);

            isProcessing = false;

            //_fillBar.gameObject.SetActive(true);

            if (isUIOk)
            {
                JuiceBoxState(tmpType);
            }
        }
    }

    private void DecreaseTargetCount(HexagonTypes type = HexagonTypes.NONE)
    {
        var juiceTargetUIElement = JuiceTargetUIController.Instance.ReturnJuiceTargetUIElement(type);
        if (juiceTargetUIElement) juiceTargetUIElement.DecreaseJuiceCount();
    }

    private void JuiceBoxState(HexagonTypes type = HexagonTypes.NONE)
    {
        DecreaseTargetCount(type);
    }


    private void BlenderElementSave()
    {
        fruitType = (HexagonTypes)FruitType;
        _fillBar.gameObject.SetActive(true);
   
        _fillBar.FillBarStep(BlenderFruitCount);
        juiceLevel = BlenderFruitCount;
        juiceAmount = .1f + (BlenderFruitCount * .2f);
        juice.level = BlenderFruitCount * .2f;
        juice.liquidColor1 = juiceColors[(HexagonTypes)FruitType];
        SetFillbarColor(fruitType);
        SetFillbarImage(fruitType, true);
       DOVirtual.DelayedCall(.5f,()=> juice.Redraw());
    }

    public void BlenderFirstCustom()
    {
        FruitType = (int)HexagonTypes.AVACADO;

        BlenderFruitCount = 2;

        fruitType = (HexagonTypes)FruitType;
        _fillBar.gameObject.SetActive(true);

        _fillBar.FillBarStep(BlenderFruitCount);
        juiceLevel = BlenderFruitCount;
        juiceAmount = .1f + (BlenderFruitCount * .2f);
        juice.level = BlenderFruitCount * .2f;
        juice.liquidColor1 = juiceColors[(HexagonTypes)FruitType];
        SetFillbarColor(fruitType);
        SetFillbarImage(fruitType, true);
        DOVirtual.DelayedCall(.5f, () => juice.Redraw());
    }

    public void ResetBlender(float juiceLevelDuration=0)
    {
        SetFruitType(HexagonTypes.NONE);
        SetFillbarImage(fruitType, false);
        _fillBar.FillBarStep(0);
        _fillBar.FillBarColor(Color.white);
        _fillBar.gameObject.SetActive(false);
        BlenderFruitCount = 0;
        juiceAmount = .1f;
        juiceLevel = 0;
        DOVirtual.Float(juice.level, 0, juiceLevelDuration, (v) => juice.level = v);
        DOVirtual.DelayedCall(juiceLevelDuration,()=>juice.gameObject.SetActive(false));
        
       if(_blenderReset) _blenderReset.ButtonScaleState();
    }

    public void SetFruitType(HexagonTypes fruitType)
    {
        this.fruitType = fruitType;
        FruitType = (int)fruitType;
    }

    public void SetFillbarColor(HexagonTypes fruitType)
    {
        var juiceSpriteDict = ResourceSystem.ReturnVisualData().GlassInfos;
        Color32 color = fruitType == HexagonTypes.NONE
            ? new Color32(255, 255, 255, 255)
            : juiceSpriteDict[fruitType].color;
        _fillBar.FillBarColor(color);
    }

    public void SetFillbarImage(HexagonTypes type, bool state)
    {
        if (state)
        {
            var juiceSpriteDict = ResourceSystem.ReturnVisualData().GlassInfos;
            Sprite sprite = juiceSpriteDict[type].fruitSprite;
            _fillBar.SetFillbarFruit(sprite, state);
        }
        else
        {
            _fillBar.SetFillbarFruit(null, state);
        }
    }
    
    public void BlenderOpenFirstPieceSFX() => AudioManager.Instance.Play(AudioManager.AudioEnums.BlenderFall);
}