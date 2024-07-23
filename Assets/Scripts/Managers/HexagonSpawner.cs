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
    [SerializeField] private Transform _hexagonHolderSpawnPosOpponent;

    [SerializeField] private List<HexagonSlot> _hexagonSlotList;
    [SerializeField] private List<HexagonSlot> _hexagonSlotListOpponent;

    private int _maxHexagonCountIndividual;
    private int _maxHexagonTypeCountIndividual;
    private int _newTypeMinIndex;

    private float _hexagonElementFirstLocalPosY;
    private float _hexagonElementYOffset;

    public List<int> _hexagonTypeStageCount;
    private List<HexagonTypes> _hexagonTypesAtTheBeginning;


    private int _spawnCount;


    private void Awake()
    {
        _gridHolderController = FindObjectOfType<GridHolderController>();
        _hexagonHolderController = FindObjectOfType<HexagonHolderController>();
    }

    void Start()
    {
        AssingReferences();
        SpawnPlayersHexagonHolder();
    }

    private void OnEnable()
    {
        EventManager.SpawnEvents.SpawnHexagonHolder += SpawnPlayersHexagonHolder;
    }

    private void OnDisable()
    {
        EventManager.SpawnEvents.SpawnHexagonHolder -= SpawnPlayersHexagonHolder;
    }

    private void SpawnPlayersHexagonHolder()
    {
        StartCoroutine(SpawnHexagonHolderCor());

        IEnumerator SpawnHexagonHolderCor()
        {
            var hexagonHolder = ResourceSystem.ReturnVisualData().prefabData[VisualData.PrefabType.HexagonHolder];
            int spawnAmount = 3;
            for (int i = 0; i < spawnAmount; i++)
            {
                var hexagonHolderInstantiated = Instantiate(hexagonHolder, _hexagonHolderSpawnPos.position, Quaternion.identity).GetComponent<HexagonHolder>();
                hexagonHolderInstantiated.transform.SetParent(_hexagonHolderController.transform);
                SpawnHexagonElements(hexagonHolderInstantiated);
                hexagonHolderInstantiated.Init(AvailableSlot());
                _spawnCount++;
                yield return new WaitForSeconds(.1f);
            }
        }
    }

    public void SpawnOpponentHexagonHolder()
    {
        StartCoroutine(SpawnHexagonHolderCor());

        IEnumerator SpawnHexagonHolderCor()
        {
            var hexagonHolder = ResourceSystem.ReturnVisualData().prefabData[VisualData.PrefabType.HexagonHolder];
            int spawnAmount = 3;
            for (int i = 0; i < spawnAmount; i++)
            {
                var hexagonHolderInstantiated = Instantiate(hexagonHolder, _hexagonHolderSpawnPos.position, Quaternion.identity).GetComponent<HexagonHolder>();
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
        int hexagonTypeCount = 0;
        if (LevelManager.Instance.LevelCount == 1 && _spawnCount <= 5 && TutorialManager.TutorialCompleted == 0)
        {
            hexagonTypeCount = 2;
        }
        else
        {
            hexagonTypeCount = Random.Range(1, _maxHexagonTypeCountIndividual + 1);
        }

        List<HexagonElement> tempHexagons = new();
        for (int i = 0; i < hexagonTypeCount; i++)
        {
            HexagonTypes hexagonType;
            int maxHexagonCountIndividual = 1;

            if (LevelManager.Instance.LevelCount == 1 && _spawnCount <= 5 && TutorialManager.TutorialCompleted == 0) //Level 1 Tut
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
                hexagonType = _hexagonTypesAtTheBeginning[Random.Range(0, _hexagonTypesAtTheBeginning.Count)];
                maxHexagonCountIndividual = Random.Range(2, _maxHexagonCountIndividual + 1);
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
        _hexagonTypesAtTheBeginning = new List<HexagonTypes>(levelInfo.hexagonTypes);
    }
}