using UnityEngine;
using DG.Tweening; // Dotween kütüphanesini ekleyin

public class MergeController : MonoBehaviour
{
    public GameObject newObjectPrefab; // Yeni obje için prefab
    public LayerMask mergeLayer; // Çarpışmayı kontrol etmek istediğiniz katman
    public bool isSpawned = false;


    private void Start()
    {
        if(gameObject.GetComponentInParent<FullFruitElement>().Type == HexagonTypes.WATERMELON)
        {
            FindObjectOfType<SpawnerMovement>(true).OnWatermelonReached?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Çarpışan objenin katmanı belirlediğiniz katmanla aynı mı?
        if (((1 << collision.gameObject.layer) & mergeLayer) != 0)
        {
            var fullFruitType = collision.gameObject.GetComponentInParent<FullFruitElement>().Type;
            // Çarpışan objenin türü bu objenin türüyle aynı mı?
            var typ2 = gameObject.GetComponentInParent<FullFruitElement>().Type;

            if (fullFruitType == typ2)
            {
                if (isSpawned) return;

                // İki objenin orta noktasını hesapla
                Vector3 spawnPosition = (transform.position + collision.transform.position) / 2;

                // Yeni objeyi oluştur ve başlangıç scale'ini 0 yap
                collision.gameObject.GetComponent<MergeController>().isSpawned = true;
                GameObject spawnedObject = Instantiate(newObjectPrefab, spawnPosition, Quaternion.identity);
                Vector3 finalScale = spawnedObject.transform.localScale; // Gerçek scale değerini al
                spawnedObject.transform.localScale = Vector3.zero;

                spawnedObject.GetComponentInChildren<Rigidbody>().useGravity = false;
                spawnedObject.GetComponentInChildren<Rigidbody>().useGravity = true;
 
                // Dotween ile gerçek scale değerine animasyon yap
                spawnedObject.transform.DOScale(finalScale,
                    0.2f).SetUpdate(true); // 0'dan gerçek scale değerine, 0.2 saniyede animasyon yap

                spawnedObject.transform.parent = GameObject.FindGameObjectWithTag("AfterReleaseHolder").transform;


                MergeVFX(spawnPosition - Vector3.forward,fullFruitType);

                //MetaSaveManager.RemoveFruit(fullFruitType);
                //MetaSaveManager.RemoveFruit(typ2);
                //MetaSaveManager.SaveFruit(spawnedObject.GetComponent<FullFruitElement>().Type);

                Destroy(collision.gameObject); // Çarpışan objeyi yok et
                Destroy(gameObject); // Mevcut objeyi yok et


               
            }
        }
    }

    private void MergeVFX(Vector3 spawnPos,HexagonTypes fruitType)
    {
          MergeVFXController.Instance.ActivateMergeVFX(spawnPos, fruitType);
    }
}