using System.Collections.Generic;
using UnityEngine;

public class FailDetector : MonoBehaviour
{
    public float timeThreshold = 3f; // Süre eşiği 3 saniye olarak ayarlandı
    private float timeInside = 0f; // Collider içinde geçirilen süre
    private bool isInside = false; // Mesh Collider'ın içinde olup olmadığını kontrol et
    public GameObject alertObject;
    public static bool isFailed = false;
    private List<GameObject> objectsInTrigger = new List<GameObject>();

    void Update()
    {
        if (objectsInTrigger.Count != 0)
        {
            objectsInTrigger.RemoveAll(item => item == null);
        }

        if (isInside && !isFailed)
        {
            // Eğer collider içindeyse süreyi arttır
            timeInside += Time.deltaTime;

            // Eğer belirlenen süre aşıldıysa ve Trigger içinde hiç obje yoksa fonksiyonu çağır
            if (timeInside >= timeThreshold && objectsInTrigger.Count != 0)
            {
                TriggerFunction();
                timeInside = 0f; // Süreyi sıfırla
                alertObject.SetActive(false);
            }
            else if (timeInside >= 1.5f)
            {
                alertObject.SetActive(true);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFailed) return;

        if (other.CompareTag("Player")) // "Player" etiketine sahip nesneleri kontrol et
        {
            objectsInTrigger.Add(other.gameObject);
            isInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isFailed) return;

        if (other.CompareTag("Player"))
        {
            objectsInTrigger.Remove(other.gameObject);
            // Eğer Trigger içinde başka obje kalmadıysa isInside'ı güncelle
            if (objectsInTrigger.Count == 0)
            {
                isInside = false;
                timeInside = 0f; // Süreyi sıfırla
                alertObject.SetActive(false);
            }
        }
    }

    public void ResetInTime()
    {
        timeInside = 0f;
        objectsInTrigger.Clear();
        isInside = false;
    }

    void TriggerFunction()
    {
        isFailed = true;
        MetaWrapper.Instance.ClearMeta();
        FindObjectOfType<SpawnerMovement>(true).OnFailed?.Invoke();
        timeInside = 0;
        EventHandler.Instance.LogMetaEvents(LevelManager.Instance.LevelCount, LevelManager.Instance.SortedFruitCount, EventHandler.EventStatus.Fail);
    }
}
