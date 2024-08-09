using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class FailedPanel : MonoBehaviour
{
    public static FailedPanel Instance;

    [SerializeField] private Button tryAgainButton;

    [SerializeField] private GameObject disabledPlayOn;

    [SerializeField] private Button _rwButton;
    [SerializeField] private Button playOnButton;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private ClearSlotHint hint;

    public int revivePrice;

    private bool _isTimerFail;

    [SerializeField] private GameObject _extraTimeInfo;

    [SerializeField] private float _extraTime;


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (CurrencyManager.Instance.GoldAmount >= revivePrice)
        {
            disabledPlayOn.SetActive(false);
        }
        else
        {
            disabledPlayOn.SetActive(true);
        }
    }

    private void Start()
    {
        tryAgainButton.onClick.AddListener(LevelManager.Instance.RestartLevel);
        playOnButton.onClick.AddListener(OnPlayOnClicked);
        _rwButton.onClick.AddListener(OnPlayOnClickedRw);
    }

    public void OnPlayOnClicked()
    {
        CurrencyManager.Instance.TakeGold(revivePrice);
        LevelManager.Instance.isGameOverPanelOpened = false;
        gameObject.SetActive(false);
        if(!_isTimerFail) hint.FailedClick();
        //EventHandler.Instance.LogEconomyEvent(revivePrice, 0, "Revive");
        if (_isTimerFail)
        {
            Timer.Instance.AddTime(_extraTime);
            AudioManager.Instance.Play(AudioManager.AudioEnums.ExtraTime);
        }
    }

    public void OnPlayOnClickedRw()
    {
        LevelManager.Instance.isGameOverPanelOpened = false;
        gameObject.SetActive(false);
        if(!_isTimerFail) hint.FailedClick();
    //    EventHandler.Instance.LogEconomyEvent(revivePrice, 0, "Revive");
        if (_isTimerFail)
        {
            Timer.Instance.AddTime(_extraTime);
            AudioManager.Instance.Play(AudioManager.AudioEnums.ExtraTime);
        }
    }

    public void NormalFailOrTimerFail(bool isTimer = false)
    {
        if (!isTimer)
        {
            _isTimerFail = false;
            _extraTimeInfo.SetActive(false);
        }
        else
        {
            _isTimerFail = true;
            _extraTimeInfo.SetActive(true);
        }
    }


//    Öncelikle oyundaki bir mantýksal bir durum üzerine düþümemiz lazým.  2 tane boþ blender var ve hedef limon suyu yapmak.Bir tane limon sortladým bir blendera gitti diðer blender hala boþ.Tekrar limon sortlarsam bu limon diðer boþ blendera gitmesi lazým.Daha sonra limon sortlarsam önce baþtakini sonra diðerini doldurmasý lazým.Þöyle bi durum var ben hedef olan limonu blendera göderdim. Diðer blenderda baþka birþey var.Bu durumda 3.blender para karþýlýðý açarsam param boþa gidiyor çünkü hedef olan blender zaten devrede.En kötü 3. blender para karþýlýðý açýlýrsak ona ilk sorlanan meyveyi koþulsuz göndermemiz lazým.Mesele limon blender içinde devre.Ben 3. Blender açtým, eðer limon sortlarsam bunun o blendera gitmesi lazým. Bunu konuþalým.

//Kullanýcý fail olduðunda level fail ekranýnda hint kullanmak istermisin seçeneði koyulabilir.Hexadaki gibi aþaðý basýnca alpha kýsýp boarda göz atabilir.


//--Blocker slotlarý kaldýralým.Delikli slotlarda olmasýn.Boardda ortasý boþ olan slotlarý kasdediyorum. Çünkü bunlar oyunu çok zorlaþtýrýyor.
//--Yerde level baþlangýcýnda hiç meyve koymayalým. Ya oyunu çok kolaylaþtýrýyor ya da çok zorlaþtýrýyor. Bunu kullancaðýmýz yeri yazýcam. Stratejþi yapmaný engelliyor.
//--Slotlarýn sayýsýný yüzde 25 arttýralým.

//Level 1 Tutorial
//Level 2 - 2 Blender açýk -  2 Kýrmýzý Meyve var - Bitiþte metaya gidiyor.
//Level 3 - 1 Blender açýk - Hedef 3 Limon
//Level 4 - 2 Blender açýk  - Avacado ve Brocoli hedef - Burda hint açýlsýn
//Level 5 - 2 Blender açýk - Ýlk kez 3 lü hedef var
//Level 6 - 1 Blender açýk - Tek hedef var 5 tane Su istiyoruz - Ýlk kez sort yaptýkça slot açýlmasýný burda devreye sokalým.Burdan sonraki bütün levellerde olsun.
//Level 7 -  2 Blender açýk - Bu level 3lü hedef ve renk olarak bir ayrým yok - Diðer hint burda açýlsýn
//Level 8 - Burdan sonrasý Normal Level - Hedefler düzenlend
}