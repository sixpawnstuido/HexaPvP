using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringBase
{
    public struct SaveKeys
    {
        public const string DROPPABLE_SPAWNER_SPAWNINDEX = "DroppableSpawnIndex";
        public const string MAP_BUTTON_ELEMENT = "MapButtonElement";
        public const string TUTORIAL_STAGE = "TutorialStage";
        public const string MAGNIFIER_COUNT= "MagnifierCount";
        public const string COMPASS_COUNT= "CompassCount";
    }

    public struct ResourceDatas
    {
        public const string VISUAL_DATA= "AllData/VisualData";
        public const string LEVEL_DATA= "AllData/LevelData";
        public const string LEVEL_INFO= "AllData/LevelInfo";
        public const string GLOBAL_VARIABLES= "AllData/GlobalVariables";
    }

    public struct GlobalVariables
    {
        public const float CAMERA_MIN_ZOOM = 10F;
        public const float CAMERA_MAX_ZOOM = 22F;
    }

}
