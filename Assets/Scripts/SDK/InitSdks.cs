
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class InitSdks : MonoBehaviourSingletonPersistent<InitSdks>
{
    public bool isSdkOpen;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
      


        //Adjust will be inialized in Applovin Max OnInitComplete
        //ApplovinMaxHandler.Instance.Init();
       // FacebookHandler.Instance.Init();
       // ByteBrewHandler.Instance.Init();
        
    }

    private void BundleCheck()
    {
#if UNITY_EDITOR
        if (EditorUserBuildSettings.buildAppBundle == true)
        {
            isSdkOpen = true;

            GameObject reporter = GameObject.Find("Reporter");
            if (reporter is not null) reporter.SetActive(false);
        }
#endif
    }

}
