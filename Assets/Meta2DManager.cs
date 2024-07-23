using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Extensions;

public class Meta2DManager : SerializedMonoBehaviour
{
    public static Meta2DManager Instance;

    public GameObject holder;

    public Dictionary<HexagonTypes, MetaFruit> fruits;

    public FruitPlace refFruitPlace;

    public static MetaFruit spawnedFruit;

    public static int FruitCount;

    public GameObject FruitFinishedPanel;

    public GameObject failPanel;

    public bool fruitsFinished;

    public TextMeshProUGUI fruitCountText;

    public static List<MetaFruit> spawnedFruits=new List<MetaFruit>();

    public static bool isWatermelonReached;
    public static bool isMetaFailed;

    public GameObject winPanel;

    public Dictionary<HexagonTypes, Sprite> fruitSpritesOutline;

    public GameObject spawner;
   

    private List<HexagonTypes> elements = new List<HexagonTypes>() {
        HexagonTypes.STRAWBERRY,
        HexagonTypes.KIWII, 
        HexagonTypes.LEMON ,
        HexagonTypes.TOMATO,
        HexagonTypes.APPLE,
        HexagonTypes.AVACADO, 
    
    };

    // Probabilities of each element (should sum up to 1)
    public List<float> probabilities = new List<float>() { 0.40f, 0.30f, 0.15f,0.05f,0.05f,0.05f };

    public Image nextFruitImage;

    public HexagonTypes nextFruit;

    public GameObject firstTutorial;
    public GameObject secondTutorial;

    public GameObject circleOfEvolution;

    [SerializeField] private GameObject mainCamera;

    public static bool MetaOpened;

    private Vector3 nextFruitScale;


    public int Meta2DTutorialCompleted
    {
        get => PlayerPrefs.GetInt("Meta2DTutorialCompleted", 0);
        set => PlayerPrefs.SetInt("Meta2DTutorialCompleted", value);
    }

    private void Awake()
    {
        Instance = this;
        nextFruitScale = nextFruitImage.transform.localScale;
    }


    [Button]
    public void OpenMeta()
    {
        if (LevelManager.Instance.LevelCount == 2)
        {
            LevelManager.Instance.NextLevelButton();
        }
        else
        {
       
            AudioManager.Instance.PlayBGMusic();
            if (Meta2DTutorialCompleted == 0) circleOfEvolution.SetActive(false);
            else circleOfEvolution.SetActive(true);
            FruitPlace.TouchBlock = false;
            nextFruit = HexagonTypes.NONE;
            FruitFinishedPanel.SetActive(false);
            winPanel.SetActive(false);
            failPanel.SetActive(false);
            isMetaFailed = false;
            isWatermelonReached = false;
            FruitCount = LevelManager.Instance.SortedFruitCount;
            FruitCount = Mathf.Clamp(FruitCount, 3, 30);
            MetaOpened = true;
            holder.SetActive(true);
            mainCamera.SetActive(false);
            UIManager.Instance.nextLevelPanel.gameObject.SetActive(false);
            fruitsFinished = false;
            nextFruitImage.gameObject.SetActive(true);
            SpawnAFruit(0);
            if(Meta2DTutorialCompleted==0) spawner.SetActive(false);
            EventHandler.Instance.LogMetaEvents(LevelManager.Instance.LevelCount, FruitCount, EventHandler.EventStatus.Start);
        }

    }


    public void SpawnAFruit(float delay)
    {
        if (fruitsFinished) return;

        Debug.Log(delay + "SPAWNN");

        if (FruitCount > 0)
        {
            FruitCount--;
            fruitCountText.text = FruitCount.ToString();

            if (nextFruit == HexagonTypes.NONE)
            {
                StartCoroutine(SpawnCor(delay, SelectNextFruit()));

                nextFruit=SelectNextFruit();

                nextFruitImage.sprite = fruitSpritesOutline[nextFruit];

                nextFruitImage.transform.localScale = Vector3.zero;

                nextFruitImage.transform.DOScale(nextFruitScale, .2f).SetEase(Ease.OutBack);
            }
            else
            {
                StartCoroutine(SpawnCor(delay, nextFruit));

                if (FruitCount > 0)
                {
                    nextFruit = SelectNextFruit();

                    nextFruitImage.transform.localScale = Vector3.zero;

                    nextFruitImage.transform.DOScale(nextFruitScale, .2f).SetEase(Ease.OutBack);

                    nextFruitImage.sprite = fruitSpritesOutline[nextFruit];
                }
                else
                {
                    nextFruitImage.gameObject.SetActive(false);
                }


            }

          
        }
        else
        {
            nextFruitImage.gameObject.SetActive(false);
            fruitsFinished = true;
            FruitPlace.TouchBlock = true;
            DOVirtual.DelayedCall(2, delegate { FruitFinishedPanel.SetActive(true); });
        }


  

    }

    public HexagonTypes SelectNextFruit()
    {

        HexagonTypes selectedItem = SelectRandomElement();

        return selectedItem;
    }

    private IEnumerator SpawnCor(float delay,HexagonTypes selectedType)
    {
        yield return new WaitForSeconds(delay);

        if (Meta2DTutorialCompleted == 0)
        {
            firstTutorial.SetActive(true);

            yield return new WaitUntil(() => Meta2DTutorialCompleted == 1);

            yield return new WaitForSeconds(.5f);

            secondTutorial.SetActive(true);

            spawner.SetActive(true);


        }

        MetaFruit fruit = fruits[selectedType];

        MetaFruit tmpFruit = Instantiate(fruit, refFruitPlace.transform.position, Quaternion.identity, refFruitPlace.transform);

        spawnedFruits.Add(tmpFruit);

        Vector3 scale = tmpFruit.transform.localScale;

        tmpFruit.transform.localScale = Vector3.zero;

        tmpFruit.transform.DOScale(scale, .2f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(.22f);

        spawnedFruit = tmpFruit;
    }

    public void CloseMeta()
    {
        LevelManager.Instance.NextLevelButton();
    }

    public void CloseMetaCustom()
    {
       
        mainCamera.SetActive(true);
        holder.SetActive(false);
        MetaOpened = false;

    }

    public void ResetMeta()
    {
        LevelManager.Instance.NextLevelButton();
        for (int i = 0; i < spawnedFruits.Count; i++)
        {
            if (spawnedFruits[i] == null) continue;

            Destroy(spawnedFruits[i].gameObject);
        }

        spawnedFruits = new List<MetaFruit>();

        FruitCount = 0;
    }

    public void WatermelonReached()
    {
        if (isWatermelonReached) return;

        isWatermelonReached = true;

        FruitPlace.TouchBlock = true;
        FruitCount = 0;

        DOVirtual.DelayedCall(1, delegate
        {
            winPanel.SetActive(true);
            AudioManager.Instance.Play(AudioManager.AudioEnums.LevelEnd, .6f);
            EventHandler.Instance.LogMetaEvents(LevelManager.Instance.LevelCount, LevelManager.Instance.SortedFruitCount, EventHandler.EventStatus.Complete);
        });
    }

    public void ContinueMeta()
    {
        FruitFinishedPanel.SetActive(false);
        LevelManager.Instance.NextLevelButton();
    }

    public void MetaFailed()
    {
        if (isMetaFailed) return;

        isMetaFailed = true;

        FruitPlace.TouchBlock = true;
        Meta2DManager.FruitCount = 0;
        failPanel.SetActive(true);
        AudioManager.Instance.Play(AudioManager.AudioEnums.LevelFail, .6f);
    }


    // Method to select a random element based on the probabilities
    HexagonTypes SelectRandomElement()
    {
        // Generate a random value between 0 and 1
        float randomValue = UnityEngine.Random.value;

        // Initialize cumulative probability
        float cumulativeProbability = 0f;

        // Iterate over elements and check if random value falls within the range
        for (int i = 0; i < elements.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return elements[i];
            }
        }

        // If for some reason the random value is greater than the sum of probabilities, return the last element
        return elements[elements.Count - 1];
    }


    public void AddItemToUnlockedFruits(HexagonTypes type)
    {
        //if (unlockedFruits.Contains(type)) return;

        //if (unlockedFruits.Count >= 4) return;

        //unlockedFruits.Add(type);
    }

}
