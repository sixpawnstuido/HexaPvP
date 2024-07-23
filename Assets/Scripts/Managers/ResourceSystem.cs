using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    private static VisualData _visualData;
    private static LevelData _levelData;
    private static LevelInfo _levelInfo;
    private static GlobalVariables _globalVariables;


    private void OnEnable()
    {
        EventManager.SpawnEvents.LoadAllDatas += LoadAllDatas;
    }
    private void OnDisable()
    {
        EventManager.SpawnEvents.LoadAllDatas -= LoadAllDatas;

    }
    public void LoadAllDatas()
    {
        _visualData = Resources.Load<VisualData>(StringBase.ResourceDatas.VISUAL_DATA);
        _levelData = Resources.Load<LevelData>(StringBase.ResourceDatas.LEVEL_DATA);
        _levelInfo = Resources.Load<LevelInfo>(StringBase.ResourceDatas.LEVEL_INFO);
        _globalVariables = Resources.Load<GlobalVariables>(StringBase.ResourceDatas.GLOBAL_VARIABLES);
    }

    public static VisualData ReturnVisualData()
    {
        return _visualData;
    }
    public static LevelData ReturnLevelData()
    {
        return _levelData;
    }
    public static LevelInfo ReturnLevelInfo()
    {
        return _levelInfo;
    }
    public static GlobalVariables ReturnGlobalVariablesData()
    {
        return _globalVariables;
    }
}
