using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotator : MonoBehaviour
{
    public float rotationSpeed = 360f;

    [ReadOnly] public bool isRotating = false;

    public float rotationDuration = 3f;

    private Animator _animator;

    private PvPController _pvpController;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _pvpController = GetComponentInParent<PvPController>();
    }

    [Button]
    public void ActivateArrow()
    {
        StartCoroutine(RotateArrow());
    }

    private IEnumerator RotateArrow()
    {
        isRotating = false;
        _animator.SetTrigger("ScaleUp");
        yield return new WaitForSeconds(.3f);
        float stopAngle = GetRandomStopAngle();
        stopAngle = (stopAngle + 360) % 360;

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            float currentYRotation = transform.eulerAngles.y;
            currentYRotation = (currentYRotation + 360) % 360;
            if (Mathf.Abs(currentYRotation - stopAngle) < 5f)
            {

                transform.eulerAngles = new Vector3(transform.eulerAngles.x, stopAngle, transform.eulerAngles.z);
                break;
            }
            yield return null;
        }
        isRotating= true;
        _animator.SetTrigger("ScaleDown");
    }

    private float GetRandomStopAngle()
    {
        return Random.value > 0.5f ? 90f : 270f;
    }

    public PlayerType ReturnPlayerType()
    {
        float finalYRotation = transform.eulerAngles.y;

        if (Mathf.Approximately(finalYRotation, 90f))
        {
            return PlayerType.OPPONENT;
        }
        else if (Mathf.Approximately(finalYRotation, 270f))
        {
            return PlayerType.PLAYER;
        }
        else
        {
            return PlayerType.PLAYER;
        }
    }
}

