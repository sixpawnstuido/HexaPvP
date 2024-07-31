using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
   [SerializeField] private Button selectButton;
   
   
   [SerializeField] private Image icon;
   
   private TileController _tileController;

   public Color32 blueColor;
   public Color32 greenColor;
   public TextMeshProUGUI SelectText=>selectText;
   [SerializeField] private TextMeshProUGUI selectText;

   public ParticleSystem shineVFX;

   [ShowInInspector]
   public int TileState
   {
      get => PlayerPrefs.GetInt(gameObject.name, 0);
      set => PlayerPrefs.SetInt(gameObject.name, value);
   }

   private void Awake()
   {
      _tileController = GetComponentInParent<TileController>();
   }

   private void Start()
   {
      selectButton.onClick.AddListener(() =>
      {
         _tileController.SelectTile(this);
         AudioManager.Instance.Play(AudioManager.AudioEnums.ButtonTap2);
         if (!DOTween.IsTweening(transform.GetHashCode()))
         {
           icon.transform
             .DOPunchScale(new Vector3(-.025f, 0.025f, 0), .2f, 5)
             .SetId(transform.GetHashCode());
         }
      });
   }

   public void ChangeColor(bool isGreen)
   {
      Color32 buttonColor = isGreen ? greenColor : blueColor;
      selectButton.GetComponent<Image>().color = buttonColor;
   }
}
