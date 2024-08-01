using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Level1Tut : MonoBehaviour
{
    [SerializeField] private GameObject holder;
    
   [SerializeField] private GameObject hand1;
   [SerializeField] private GameObject hand2;


   [SerializeField] private List<GridHolder> allGrids;
   [SerializeField] private GridHolder tut1Grid;
   [SerializeField] private GridHolder tut2Grid;
   
   
   [ShowInInspector, ReadOnly]
   public static int Level1TutCount
   {
      get => PlayerPrefs.GetInt("Level1Tut", 0);
      set => PlayerPrefs.SetInt("Level1Tut", value);
   }

   private void Awake()
   {
       if (Level1TutCount>0)
       {
           holder.SetActive(false);
       }
   }

   public void Tut1()
   { 
       hand1.SetActive(true);
       allGrids.ForEach(grid=>grid.GridCollider.enabled=false);
       tut1Grid.GridCollider.enabled = true;
       Level1TutCount = 0;
   }

   public void Tut1Completed()
   {
       Level1TutCount = 1;
       hand1.SetActive(false);
   }

   public void StartTut2()
   {
       hand2.SetActive(true);
       allGrids.ForEach(grid=>grid.GridCollider.enabled=false);
       tut2Grid.GridCollider.enabled = true;
   }

   public void Tut2()
   {
       Level1TutCount = 2;
       hand2.SetActive(false);
   }

   public void Tut2Completed()
   {
       hand2.SetActive(false);
       allGrids.ForEach(grid=>grid.GridCollider.enabled=true);
       Level1TutCount = 3;
   }
}
