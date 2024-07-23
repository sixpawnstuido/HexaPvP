using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCamera : MonoBehaviour
{
    public static MainSceneCamera Instance;

    private Camera cam;

    public float levelEndOrtho;

    public float normalOrtho;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void CameraLevelEnd()
    {
        cam.DOOrthoSize(levelEndOrtho, 1f);
    }

    public void ResetCam()
    {
        cam.orthographicSize = normalOrtho;
    }


}
