using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public struct UIEvents
    {
        public static Action<CanvasTypes, bool> CanvasSetter;
    }

    public struct SpawnEvents
    {
        public static Action LoadAllDatas;
        public static Action SpawnHexagonHolder;
        public static Action<GridHolder,List<HexagonTypes>,bool> SpawnHexagonHolderSave;
        public static Func<bool> CheckIfAllGridsOccupied;
    }
    public struct CoreEvents
    {
        public static Action<bool> HexagonHolderColliderState;
        public static Action GridHolderSave;
        public static Action<bool,bool> GridHolderColliderState;
        public static Action CheckIfGridsUnlockable;
    }
}
