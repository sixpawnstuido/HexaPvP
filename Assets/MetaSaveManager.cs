using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaSaveManager : MonoBehaviour
{
    public static MetaSaveManager Instance;

   [ShowInInspector,ReadOnly] private static List<HexagonTypes> fruits = new List<HexagonTypes>();

    private void Awake()
    {
        Instance = this;
    }

    public static void SaveFruit(HexagonTypes fruitType)
    {
        fruits.Add(fruitType);

        ES3.Save("MetaSave",fruits);
    }

    public static void RemoveFruit(HexagonTypes fruitType)
    {
        if (fruits.Contains(fruitType))
        {
            fruits.Remove(fruitType);
        }


        ES3.Save("MetaSave", fruits);
    }

    public static List<HexagonTypes> SavedFruitList()
    {
        fruits = ES3.Load<List<HexagonTypes>>("MetaSave");
        return fruits;
    }


}
