using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonHolderController : MonoBehaviour
{
    public List<HexagonHolder> hexagonHolders;


    public bool CheckHexagonClearState()
    {
        var clearState = hexagonHolders.Any(hexagon=> hexagon.gridHolder is not null && hexagon.isClearHappening );
        return clearState;
    }
}
