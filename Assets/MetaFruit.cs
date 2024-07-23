using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaFruit : MonoBehaviour
{
    public HexagonTypes type;

    public Rigidbody2D rb;

    public Collider2D coll;

    // Flag to check if the interaction has already occurred
    private bool interactionOccurred = false;

    public MetaFruit nextFruit;

    public bool isCollided;

    public bool isFalled;

    private void OnEnable()
    {
        if (type == HexagonTypes.WATERMELON)
        {
            Meta2DManager.Instance.WatermelonReached();
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with another object
        if (collision.transform.TryGetComponent(out MetaFruit fruit))
        {
            if (fruit.type == type)
            {
                if (isCollided) return;

                fruit.isCollided = true;
                isCollided = true;

                Vector3 spawnPosition = (transform.position + collision.transform.position) / 2;

                MetaFruit tmpFruit = Instantiate(nextFruit, spawnPosition, Quaternion.identity, transform.parent);

                Meta2DManager.Instance.AddItemToUnlockedFruits(tmpFruit.type);

                tmpFruit.coll.enabled = true;
                tmpFruit.rb.bodyType = RigidbodyType2D.Dynamic;

                Vector3 scale = tmpFruit.transform.localScale;

                tmpFruit.transform.localScale = Vector3.zero;

                tmpFruit.transform.DOScale(scale, .5f).SetEase(Ease.OutBack);

                MergeVFX(spawnPosition, type);

                TextEffectsMeta.Instance.PlayEffect(spawnPosition);

                AudioManager.Instance.PlayHaptic(Lofelt.NiceVibrations.HapticPatterns.PresetType.LightImpact);

                Meta2DManager.spawnedFruits.Remove(this);
                Meta2DManager.spawnedFruits.Remove(fruit);


                Destroy(collision.gameObject);

                Destroy(gameObject);

            }
        }
    }

    private void MergeVFX(Vector3 spawnPos, HexagonTypes fruitType)
    {
        MergeVFXController.Instance.ActivateMergeVFX(spawnPos, fruitType);
    }

    public void EnableIsFalled()
    {
        DOVirtual.DelayedCall(1, delegate { isFalled = true; });
    }
}
