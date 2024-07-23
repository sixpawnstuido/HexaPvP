using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using System.Linq;
using Sirenix.OdinInspector;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance;

    public List<GameObject> Fruits = new List<GameObject>();
    public int maxFruitsToChooseFrom = 4;
    public Transform spawnTransform;
   [ ShowInInspector] public static int numberOfFruitsToSpawn;
    public Transform spawnedFruitHoder;
    public Transform afterReleaseHolder;
    public TextMeshProUGUI amountText;

    private GameObject nextFruitToSpawn;
    public GameObject lastSpawnedFruit; // En son spawn edilen meyve
    public GameObject fruitFinishPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (lastSpawnedFruit != null) Destroy(lastSpawnedFruit.gameObject);
        UpdateNextFruitToSpawn();
        SpawnFruits();
        UpdateAmountText();

    
    }

    private void Start()
    {
        UpdateAmountText();
       // SpawnSave();

    }

    private void SpawnSave()
    {
        StartCoroutine(SaveCor());
    }

    private IEnumerator SaveCor()
    {
        List<HexagonTypes> savedFruits = MetaSaveManager.SavedFruitList();

        for (int i = 0; i < savedFruits.Count; i++)
        {
            GameObject fruitToSpawn = Fruits.First((x) => x.GetComponent<FullFruitElement>().Type == savedFruits[i]);

            lastSpawnedFruit = Instantiate(fruitToSpawn, spawnTransform.position, spawnTransform.rotation, spawnedFruitHoder);
            lastSpawnedFruit.transform.localScale = Vector3.zero;

            lastSpawnedFruit.transform.DOScale(fruitToSpawn.transform.localScale, 0.2f).SetEase(Ease.OutBack);
            Rigidbody rb = lastSpawnedFruit.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
            }
            lastSpawnedFruit.GetComponentInChildren<MeshCollider>().enabled = false;

            yield return new WaitForSeconds(.1f);
        }
    }

    public void UpdateAmountText()
    {
        amountText.SetText(numberOfFruitsToSpawn.ToString());

    }

    public void SpawnFruits()
    {
        if (numberOfFruitsToSpawn <= 0)
        {
            Debug.LogWarning("No more fruits left to spawn.");
      
            return;
        }

        if (Fruits.Count < maxFruitsToChooseFrom)
        {
            Debug.LogWarning("Not enough fruits in the list to spawn.");
            return;
        }

        GameObject fruitToSpawn = nextFruitToSpawn;

        lastSpawnedFruit = Instantiate(fruitToSpawn, spawnTransform.position, spawnTransform.rotation, spawnedFruitHoder);
        lastSpawnedFruit.transform.localScale = Vector3.zero;

       // MetaSaveManager.SaveFruit(lastSpawnedFruit.GetComponent<FullFruitElement>().Type);

        lastSpawnedFruit.transform.DOScale(fruitToSpawn.transform.localScale, 0.2f).SetEase(Ease.OutBack);

        SpawnerMovement.canClick = false;
        Rigidbody rb = lastSpawnedFruit.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
        lastSpawnedFruit.GetComponentInChildren<MeshCollider>().enabled = false;

        numberOfFruitsToSpawn--;
        amountText.SetText(numberOfFruitsToSpawn.ToString());

        UpdateNextFruitToSpawn();

        if (numberOfFruitsToSpawn == 0)
        {
            OnAllFruitsSpawned();
        }
    }

    void UpdateNextFruitToSpawn()
    {
        int fruitIndex = Random.Range(0, maxFruitsToChooseFrom);
        nextFruitToSpawn = Fruits[fruitIndex];
        Debug.Log(Fruits[fruitIndex]);
    }

    void OnAllFruitsSpawned()
    {
        Debug.Log("All fruits have been spawned.");
        MetaWrapper.Instance.FruitsFinishInMeta();
        //MetaWrapper.TouchLock = true;
       
    }

    public void GoNext()
    {
        StartCoroutine(nameof(GoNextFruitCor));
    }
    IEnumerator GoNextFruitCor()
    {
        if(lastSpawnedFruit == null) yield break;
        if (numberOfFruitsToSpawn <= 0) MetaWrapper.TouchLock = true;
        lastSpawnedFruit.GetComponentInChildren<Rigidbody>().useGravity = true;
        lastSpawnedFruit.GetComponentInChildren<MeshCollider>().enabled = true;
        lastSpawnedFruit.transform.parent = afterReleaseHolder;
        lastSpawnedFruit = null;
        SpawnerMovement.canClick = true;
        if (numberOfFruitsToSpawn <= 0) DOVirtual.DelayedCall(1, () => fruitFinishPanel.SetActive(true));
        yield return new WaitForSeconds(0.5f);
        SpawnFruits();
 
    }
}
