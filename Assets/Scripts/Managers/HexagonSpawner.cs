using DG.Tweening.Core.Easing;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonSpawner : MonoBehaviour
{
    private GridHolderController _gridHolderController;
    private HexagonHolderController _hexagonHolderController;

    [SerializeField] private Transform _hexagonHolderSpawnPos;


    private int _maxHexagonCountIndividual;
    private int _maxHexagonTypeCountIndividual;
    private int _newTypeMinIndex;

    private float _hexagonElementFirstLocalPosY;
    private float _hexagonElementYOffset;

    public List<int> _hexagonTypeStageCount;
    private List<HexagonTypes> _hexagonTypesAtTheBeginning;
    private List<HexagonTypes> _hexagonTypesInLine;
    private List<HexagonSlot> _hexagonSlotList;
    private List<LevelInfo.LevelInfoValues.TargetUITypes> _targetUITypes;

    private int _spawnCount;


    private void Awake()
    {
        _gridHolderController = FindObjectOfType<GridHolderController>();
        _hexagonHolderController = FindObjectOfType<HexagonHolderController>();
        _hexagonSlotList = GetComponentsInChildren<HexagonSlot>(true).ToList();
    }

    void Start()
    {
        // yield return new WaitUntil(() => EventManager.SpawnEvents.LoadAllDatas != null);
        AssingReferences();
        SpawnHexagonHolder();
    }

    private void OnEnable()
    {
        EventManager.SpawnEvents.SpawnHexagonHolder += SpawnHexagonHolder;
        EventManager.SpawnEvents.SpawnHexagonHolderSave += SpawnHexagonSave;
    }

    private void OnDisable()
    {
        EventManager.SpawnEvents.SpawnHexagonHolder -= SpawnHexagonHolder;
        EventManager.SpawnEvents.SpawnHexagonHolderSave -= SpawnHexagonSave;
    }

    [Button]
    private void SpawnHexagonHolder()
    {
        StartCoroutine(SpawnHexagonHolderCor());

        IEnumerator SpawnHexagonHolderCor()
        {
            var hexagonHolder = ResourceSystem.ReturnVisualData().prefabData[VisualData.PrefabType.HexagonHolder];
            int spawnAmount = 3;
            for (int i = 0; i < spawnAmount; i++)
            {
                var hexagonHolderInstantiated =
                    Instantiate(hexagonHolder, _hexagonHolderSpawnPos.position, Quaternion.identity)
                        .GetComponent<HexagonHolder>();
                hexagonHolderInstantiated.transform.SetParent(_hexagonHolderController.transform);
                SpawnHexagonElements(hexagonHolderInstantiated);
                hexagonHolderInstantiated.Init(AvailableSlot());
                _spawnCount++;
                yield return new WaitForSeconds(.1f);
            }
        }
    }

    private void SpawnHexagonElements(HexagonHolder hexagonHolder)
    {
        float totalGridCount = _gridHolderController.HowManyGridsOccupied().Item1;
        float occupiedGridCount = _gridHolderController.HowManyGridsOccupied().Item2;

        int hexagonTypeCount = 0;
        if (LevelManager.Instance.LevelCount == 1 && _spawnCount <= 5 && TutorialManager.TutorialCompleted == 0)
        {
            hexagonTypeCount = 2;
        }
        else
        {
            hexagonTypeCount = Random.Range(1, _maxHexagonTypeCountIndividual + 1);
        }


        for (int i = 0; i < _hexagonTypeStageCount.Count; i++)
        {
            if (LevelManager.Instance.CollectedHexagonCount > _hexagonTypeStageCount[i])
            {
                var tempHexagonType = _hexagonTypesInLine[i];
                if (!_hexagonTypesAtTheBeginning.Contains(tempHexagonType))
                {
                    _hexagonTypesAtTheBeginning.Add(tempHexagonType);
                    NewFruitCheck.Instance.UpdateFruitTypeList(tempHexagonType);
                }
            }
        }
           
        List<HexagonElement> tempHexagons = new();
        for (int i = 0; i < hexagonTypeCount; i++)
        {
            HexagonTypes hexagonType;
            int maxHexagonCountIndividual = 1;

            if (LevelManager.Instance.LevelCount == 1 && _spawnCount <= 5 && TutorialManager.TutorialCompleted == 0)
            {
                switch (_spawnCount)
                {
                    case 0:
                        hexagonType = HexagonTypes.ORANGE;
                        break;
                    case 1:
                        hexagonType = HexagonTypes.AVACADO;
                        break;
                    case 2:
                        hexagonType = HexagonTypes.LEMON;
                        break;
                    case 3:
                        hexagonType = HexagonTypes.AVACADO;
                        break;
                    case 4:
                        hexagonType = HexagonTypes.AVACADO;
                        break;
                    case 5:
                        hexagonType = HexagonTypes.ORANGE;
                        break;
                    default:
                        hexagonType = HexagonTypes.LEMON;
                        break;
                }
            }
            else
            {
                if (occupiedGridCount / totalGridCount >= .3f)
                {
                    var maxHexagonType = _gridHolderController.ReturnHexagonType(true);
                    if (_hexagonTypesAtTheBeginning.Contains(maxHexagonType))
                        _hexagonTypesAtTheBeginning.Remove(maxHexagonType);
                    hexagonType = _hexagonTypesAtTheBeginning[Random.Range(0, _hexagonTypesAtTheBeginning.Count)];
                    maxHexagonCountIndividual = Random.Range(2, _maxHexagonCountIndividual - 1);
                    _hexagonTypesAtTheBeginning.Add(maxHexagonType);
                } 
                if (occupiedGridCount / totalGridCount > .75f)
                {
                    if (i == hexagonTypeCount - 1)
                    {
                        var t = Random.Range(0, 10);
                        if (t >= 4)
                        {
                            hexagonType = _targetUITypes[Random.Range(0,_targetUITypes.Count)].hexagonType;
                            maxHexagonCountIndividual = Random.Range(2, _maxHexagonCountIndividual + 2);   
                        }
                        else
                        {
                            hexagonType = _hexagonTypesAtTheBeginning[Random.Range(0, _hexagonTypesAtTheBeginning.Count)];
                            maxHexagonCountIndividual = Random.Range(2, _maxHexagonCountIndividual + 1);
                        }
                        
                    }
                    else
                    {
                        hexagonType = i == hexagonTypeCount - 1
                            ? _gridHolderController.ReturnHexagonType(true)
                            : _hexagonTypesAtTheBeginning[Random.Range(0, _hexagonTypesAtTheBeginning.Count)];
                    }
                }
                else
                {
                    hexagonType = _hexagonTypesAtTheBeginning[Random.Range(0, _hexagonTypesAtTheBeginning.Count)];
                    maxHexagonCountIndividual = Random.Range(2, _maxHexagonCountIndividual + 1);
                }
            }

            NewFruitCheck.Instance.UpdateFruitTypeList(hexagonType);


            for (int j = 0; j < maxHexagonCountIndividual; j++)
            {
                var hexagon = ResourceSystem.ReturnVisualData().hexagons[hexagonType];
                var hexagonElement = Instantiate(hexagon, hexagonHolder.transform);
                hexagonHolder.hexagonElements.Add(hexagonElement);
                tempHexagons.Add(hexagonElement);
                hexagonElement.transform.localPosition =
                    new Vector3(
                        0,
                        j == 0 && i == 0
                            ? _hexagonElementFirstLocalPosY
                            : tempHexagons[tempHexagons.Count - 2].transform.localPosition.y + _hexagonElementYOffset,
                        0
                    );
            }
        }
    }

    public void SpawnHexagonSave(GridHolder gridHolder, List<HexagonTypes> hexagonTypes, bool isFirstSpawn)
    {
        var hexagonHolder = ResourceSystem.ReturnVisualData().prefabData[VisualData.PrefabType.HexagonHolder];

        Vector3 gridOffset = new Vector3(0, .05f, 0);
        var hexagonHolderInstantiated =
            Instantiate(hexagonHolder, gridHolder.transform.position + gridOffset, Quaternion.identity)
                .GetComponent<HexagonHolder>();
        gridHolder.hexagonHolder = hexagonHolderInstantiated;
        gridHolder.ColliderState(false);
        hexagonHolderInstantiated.gridHolder = gridHolder;
        hexagonHolderInstantiated.transform.SetParent(_hexagonHolderController.transform);
        hexagonHolderInstantiated.GetComponent<Collider>().enabled = false;
        int loopCount = 2;
        for (int j = 0; j < loopCount; j++)
        {
            var hexagon = ResourceSystem.ReturnVisualData()
                .hexagons[isFirstSpawn ? _hexagonTypesAtTheBeginning[Random.Range(0, loopCount)] : hexagonTypes[j]];
            var hexagonElement = Instantiate(hexagon);
            hexagonElement.transform.SetParent(hexagonHolderInstantiated.transform);
            hexagonHolderInstantiated.hexagonElements.Add(hexagonElement);
            hexagonElement.transform.localPosition =
                new Vector3(
                    0,
                    j == 0 ? _hexagonElementFirstLocalPosY : j * _hexagonElementYOffset,
                    0
                );
        }
    }

    private HexagonSlot AvailableSlot()
    {
        var availableHexagonSlot = _hexagonSlotList
            .Where(g => g.hexagonHolder == null)
            .OrderBy(g => g.transform.position.x).FirstOrDefault();

        return availableHexagonSlot;
    }

    public void RemoveFromHexagonTypesList(HexagonTypes hexagonType)
    {
        if (_hexagonTypesAtTheBeginning.Contains(hexagonType))
        {
            _hexagonTypesAtTheBeginning.Remove(hexagonType);
        }
    }

    private void AssingReferences()
    {
        var levelInfo = ResourceSystem.ReturnLevelInfo().levelInfoValues[LevelManager.Instance.LevelCount];
        _maxHexagonCountIndividual = levelInfo.maxHexagonCountIndividual;
        _maxHexagonTypeCountIndividual = levelInfo.maxHexagonTypeCountIndividual;
        _newTypeMinIndex = levelInfo.newTypeMinIndex;
        _hexagonElementFirstLocalPosY = levelInfo.hexagonElementFirstLocalPosY;
        _hexagonElementYOffset = levelInfo.hexagonElementYOffset;
        _hexagonTypeStageCount = levelInfo.hexagonTypeStageCount;
        _hexagonTypesAtTheBeginning = new List<HexagonTypes>(levelInfo.hexagonTypesAtTheBeginning);
        _hexagonTypesInLine = new List<HexagonTypes>(levelInfo.hexagonTypesInLine);
        _targetUITypes=new List<LevelInfo.LevelInfoValues.TargetUITypes>(levelInfo.targetUITypes);
    }
}