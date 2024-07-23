using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutYourself : MonoBehaviour
{
    public float delay = 1f;

    private void OnEnable()
    {
        StartCoroutine(ShutYourselfCor());
    }

    private IEnumerator ShutYourselfCor()
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }
}
