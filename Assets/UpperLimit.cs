using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperLimit : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.TryGetComponent(out MetaFruit fruit))
        {
            if (!fruit.isFalled) return;

            Meta2DManager.Instance.MetaFailed();
        }
    }

    public void OnRetryClicked()
    {
        Meta2DManager.Instance.ResetMeta();
    }
}
