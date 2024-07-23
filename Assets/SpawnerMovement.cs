using UnityEngine;
using UnityEngine.Events;

public class SpawnerMovement : MonoBehaviour
{
    public float leftBound = -5f;
    public float rightBound = 5f;
    public float cooldownTime = 2f; // Cooldown time in seconds
    public UnityEvent OnFingerRelease; // Unity Event for finger release
    public UnityEvent OnWatermelonReached;
    public UnityEvent OnFailed; 

    public GameObject AimObject;

    private float cooldownTimer = 0f;

    public GameObject failPanel;
    public GameObject winPanel;

    public static bool canClick;

    private void Awake()
    {
        OnFingerRelease.AddListener(OnRelease);
        OnWatermelonReached.AddListener(OnWatermelonReachedFunc);
        OnFailed.AddListener(OnFailedFunc);
    }



    void Update()
    {
        // Update the cooldown timer

            cooldownTimer -= Time.deltaTime;


        if (MetaWrapper.TouchLock == true) return;
        if (canClick == true) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle touch move or stationary
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                float clampedX = Mathf.Clamp(touchPosition.x, leftBound, rightBound);
                transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                AimObject.SetActive(true);
            }

            // Handle touch end
            if (touch.phase == TouchPhase.Ended)
            {
                AimObject?.SetActive(false);

                if (cooldownTimer <= 0)
                {
                    // Invoke the Unity Event
                    OnFingerRelease.Invoke();
                    cooldownTimer = cooldownTime; // Reset cooldown timer
                }
            }
        }
    }

    public void OnRelease()
    {
        AimObject.SetActive(false);
    }

    public void OnWatermelonReachedFunc()
    {
        Debug.LogWarning("WatermelonReached");
        MetaWrapper.TouchLock = true;
        FruitSpawner.numberOfFruitsToSpawn = 0;
        winPanel.SetActive(true);
        AudioManager.Instance.Play(AudioManager.AudioEnums.LevelEnd, .6f);
        EventHandler.Instance.LogMetaEvents(LevelManager.Instance.LevelCount, LevelManager.Instance.SortedFruitCount, EventHandler.EventStatus.Complete);

    }


    public void OnFailedFunc()
    {
        Debug.LogWarning("OnFailed");
        FailDetector.isFailed = false;

        MetaWrapper.TouchLock = true;
        FruitSpawner.numberOfFruitsToSpawn = 0;
        failPanel.SetActive(true);
        AudioManager.Instance.Play(AudioManager.AudioEnums.LevelFail, .6f);

    }

  
 
}
