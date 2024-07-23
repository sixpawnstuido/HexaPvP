
using ByteBrewSDK;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviourSingletonPersistent<EventHandler>
{
    public enum EventStatus
    {
        Start,
        Complete,
        Fail
    }


    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameAnalyticsSDK.GameAnalytics.Initialize();
    }

    public void LevelStart(int levelNumber)
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Start, levelNumber.ToString());
        ByteBrew.NewCustomEvent("LevelStart", "Level=" + levelNumber + ";"
                                           );
    }

    public void LevelComplete(int levelNumber, int moveCount)
    {

        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Complete, levelNumber.ToString());

        ByteBrew.NewCustomEvent("LevelComplete", "Level=" + levelNumber + ";" +
                                                 "MoveCount=" + moveCount + ";"
                                             );

        Debug.Log("LevelComplete" + "Level=" + levelNumber + ";" +
                                                  "MoveCount=" + moveCount + ";"
                                              );

    }

    public void LevelFailed(int levelNumber, int moveCount)
    {
        GameAnalyticsSDK.GameAnalytics.NewProgressionEvent(GameAnalyticsSDK.GAProgressionStatus.Fail, levelNumber.ToString());

        ByteBrew.NewCustomEvent("LevelFail", "Level=" + levelNumber + ";" +
                                             "MoveCount=" + moveCount + ";"
                                           );

        Debug.Log("LevelFail" + "Level=" + levelNumber + ";" +
                                             "MoveCount=" + moveCount + ";"
                                           );


    }

    public void LogMetaEvents(int levelNumber, int sortedFruits, EventStatus eventStatus)
    {
        levelNumber--;

        switch (eventStatus)
        {
            case EventStatus.Start:
               // LionAnalytics.MissionStarted(false, "Meta", "FruitMeta", levelNumber.ToString(), sortedFruits, null);

                break;

            case EventStatus.Complete:
                //LionAnalytics.MissionCompleted(false, "Meta", "FruitMeta", levelNumber.ToString(), sortedFruits, null);

                break;

            case EventStatus.Fail:
               // LionAnalytics.MissionFailed(false, "Meta", "FruitMeta", levelNumber.ToString(), sortedFruits, null);

                break;

            default:

                Debug.LogWarning($"No Event Status Passed");

                break;
        }

        Debug.Log($"Meta{eventStatus}___{levelNumber}___{sortedFruits}");

    }


    public void LogEconomyEvent(int spendAmount, int receiveAmount, string purchaseName)
    {
     

      //  LionAnalytics.EconomyEvent(purchaseName, productSpent, productReceived, "General", null, null, null, LionStudios.Suite.Analytics.Events.ReceiptStatus.Unknown);


        Debug.LogWarning($"{purchaseName} {spendAmount} {receiveAmount}");


    }





}
