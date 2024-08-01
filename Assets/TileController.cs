using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TileController : SerializedMonoBehaviour
{
   public static TileController Instance;
   
   [SerializeField] private List<Tile> tiles;

   [SerializeField] private Button playButton;
   
   [SerializeField] private GameObject holder;

   public Dictionary<int, List<Sprite>> tileSprites;


   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      playButton.onClick.AddListener(() =>
      {
         holder.SetActive(false);
         AudioManager.Instance.Play(AudioManager.AudioEnums.ButtonTap);
         PvPController.Instance.SelectFirstPlayer();
      });
      
      var tile = tiles.FirstOrDefault(tile => tile.TileState == 1);
      if (tile is null)
      {
         tiles[0].TileState=1;
         SelectTile(tiles[0]);
      }
   }

   public void SelectTile(Tile tile)
   {
      for (int i = 0; i < tiles.Count; i++)
      {
         if (tiles[i]==tile)
         {
            tiles[i].ChangeColor(true);
            tiles[i].SelectText.SetText($"Selected!");
            tiles[i].TileState=1;
            tiles[i].shineVFX.gameObject.SetActive(true);
         }
         else
         {
            tiles[i].ChangeColor(false);
            tiles[i].SelectText.SetText($"Select");
            tiles[i].TileState=0;
            tiles[i].shineVFX.gameObject.SetActive(false);
         }
      }
   }

   [Button]
   public void OpenHolder()
   {
      StartCoroutine(OpenHolderCor());
      IEnumerator OpenHolderCor()
      {
         holder.SetActive(true);
         playButton.transform.localScale=Vector3.zero;

         for (int i = 0; i < tiles.Count; i++)
         {
            if (tiles[i].TileState==1)
            {
               tiles[i].ChangeColor(true);
               tiles[i].SelectText.SetText($"Selected!");
               tiles[i].TileState=1;
               tiles[i].shineVFX.gameObject.SetActive(true);
            }
            else
            {
               tiles[i].ChangeColor(false);
               tiles[i].SelectText.SetText($"Select");
               tiles[i].TileState=0;
               tiles[i].shineVFX.gameObject.SetActive(false);
            }
         }
         
         for (int i = 0; i < tiles.Count; i++)
         {
            tiles[i].transform.localScale = Vector3.zero;
         }

         for (int i = 0; i < tiles.Count; i++)
         {
            tiles[i].transform
               .DOScale(2, .2f)
               .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.05f);
         }

         playButton.transform
            .DOScale(1, .2f)
            .SetEase(Ease.OutBack);
      }
   }


   public int ReturnTileIndex()
   {
      var tile = tiles.FirstOrDefault(tile => tile.TileState == 1);
      var tileIndex = 0;
      
      if (tile is not null) tileIndex = tiles.IndexOf(tile);
      else tileIndex = 0;
      
      return tileIndex;
   }
}
