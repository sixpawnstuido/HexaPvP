using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewFruitCheck : MonoBehaviour
{
   public static NewFruitCheck Instance;
   
   public List<HexagonTypes> fruitTypeList=new();


   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      fruitTypeList = ES3.Load("FruitTypeList", fruitTypeList);
   }

   public void UpdateFruitTypeList(HexagonTypes fruitType)
   {
      //
      // if (fruitType == HexagonTypes.AVACADO || fruitType == HexagonTypes.ORANGE ||
      //     fruitType == HexagonTypes.LEMON) return;
      //
      // if (!fruitTypeList.Contains(fruitType))
      // {
      //    fruitTypeList.Add(fruitType);
      //    ES3.Save("FruitTypeList",fruitTypeList);  
      // }
   }
}
