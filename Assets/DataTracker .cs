using System;
using System.Collections;
using System.Collections.Generic;
using ByteBrewSDK;
using UnityEngine;

public class DataTracker : MonoBehaviour
{
    public static DataTracker Instance;

    private void Awake()
    {
        Instance = this;

    }
    
    public void LevelStart(int level)
    {
       // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,level.ToString());
        var eventParameters = new Dictionary<string, string>()
        {
            { "Level", level.ToString() },
        };
        ByteBrew.NewCustomEvent("LevelStart", eventParameters);


        Debug.Log("LevelStart" + level.ToString());
    }

    public void LevelComplete(int level)
    {
       // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete,level.ToString());
        var eventParameters = new Dictionary<string, string>()
        {
            { "Level", level.ToString() },
        };
        ByteBrew.NewCustomEvent("LevelComplete", eventParameters);

        Debug.Log("LevelComplete" + level.ToString());
    }

    public void LevelFail(int level)
    {
       // GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail,level.ToString());
        var eventParameters = new Dictionary<string, string>()
        {
            { "Level", level.ToString() },
        };
        ByteBrew.NewCustomEvent("LevelFail", eventParameters);
        Debug.Log("LevelFail" + level.ToString());
    }
}