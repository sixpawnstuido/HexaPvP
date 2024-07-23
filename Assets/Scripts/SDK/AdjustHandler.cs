using com.adjust.sdk;
using UnityEngine;

public class AdjustHandler : MonoBehaviourSingletonPersistent<AdjustHandler>
{
    public override void Awake()
    {
        base.Awake();
    }

    public void Init()
    {
        // import this package into the project:
        // https://github.com/adjust/unity_sdk/releases
#if UNITY_IOS
        /* Mandatory - set your iOS app token here */
        InitAdjust("55kquc8a5ips");
#elif UNITY_ANDROID
        /* Mandatory - set your Android app token here */
        InitAdjust("orgisoeigg74");
#endif
    }


    private void InitAdjust(string adjustAppToken)
    {
        var adjustConfig = new AdjustConfig(
            adjustAppToken,
            AdjustEnvironment.Production,
            true
        );
        adjustConfig.setLogLevel(AdjustLogLevel.Info);
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>();
        Adjust.start(adjustConfig);
    }


}
