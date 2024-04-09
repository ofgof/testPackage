using UnityEditor;
using UnityEngine;

public static class CallerIdSdkBridge
{

    public class SetupResultHandler : AndroidJavaProxy
    {
        int CID_SETUP_RESULT_CANCELED = 0;
        int CID_SETUP_RESULT_OK = -1;
        int CID_SETUP_RESULT_ERROR = -2;

        public SetupResultHandler() : base("me.sync.callerid.sdk.unity.CidUnitySetupResultHandler") { }


        public void onSetupResult(int result)
        {
            if(result == CID_SETUP_RESULT_OK)
            {
                ShowToast("Setup done!");
            }
            else if(result == CID_SETUP_RESULT_CANCELED)
            {
                ShowToast("Setup canceled!");
            }
            else 
            {
                ShowToast("Setup error!");
            }
        }
    }

    private static int setupRequestCode = 123;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Initialize()
    {
        if (Application.platform != RuntimePlatform.Android) return;
        
          InitCallerIdSdk();
         #if !UNITY_EDITOR
         new AndroidJavaClass("me.sync.callerid.sdk.unity.CidUnityBridge").GetStatic<AndroidJavaObject>("INSTANCE")
         .Call("registerMessageHandler", new SetupResultHandler());
         #endif
        
    }

    public static void LaunchCallerIdSetupFlow()
    {
        if (Application.platform != RuntimePlatform.Android) return;

        if (IsSetupAlreadyDone())
        {
            return;
        }
        
        AndroidJavaClass setupLauncherClass = new AndroidJavaClass("me.sync.callerid.sdk.CallerIdSdk$SetupLauncher");
        AndroidJavaObject companion = setupLauncherClass.GetStatic<AndroidJavaObject>("Companion");
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        companion.Call("launchSetupForResult", currentActivity, null, setupRequestCode);
    }

    public static bool IsSetupAlreadyDone()
    {
        if (Application.platform != RuntimePlatform.Android) return false;

        return GetCallerIdSdkInstance().Call<bool>("isSetupDone");
    }

    private static AndroidJavaObject GetCallerIdSdkInstance()
    {
        AndroidJavaClass callerIdSdkClass = new AndroidJavaClass("me.sync.callerid.sdk.CallerIdSdk");
        return callerIdSdkClass.GetStatic<AndroidJavaObject>("Companion").Call<AndroidJavaObject>("getInstance");
    }

    public static bool isSdkInitialized()
    {
        if (Application.platform != RuntimePlatform.Android) return false;

        AndroidJavaClass callerIdSdkClass = new AndroidJavaClass("me.sync.callerid.sdk.CallerIdSdk");
        bool isInitialized = callerIdSdkClass.GetStatic<AndroidJavaObject>("Companion").Call<bool>("isInitialized");
        return isInitialized;

    }

    public static void ShutDown()
    {
        if (Application.platform != RuntimePlatform.Android) return;

        CallerIdSdkBridge.ShowToast("OnDestroy!");

        new AndroidJavaClass("me.sync.callerid.sdk.unity.CidUnityBridge")
            .GetStatic<AndroidJavaObject>("INSTANCE")
            .Call("unregisterMessageHandler");
    }


    
    public static void InitCallerIdSdk()
    {

        if(isSdkInitialized())
        {
            return;
        }
        ShowToast("InitCallerIdSdk");
        BuildCallerIdSdk();
        GetCallerIdSdkInstance().Call("init");
        ShowToast("CallerIdSdk: Setup done");
    }
    

    
    public static void BuildCallerIdSdk()
    {
        AndroidJavaClass callerIdSdkClass = new AndroidJavaClass("me.sync.callerid.sdk.CallerIdSdk");
        AndroidJavaObject builder = callerIdSdkClass.GetStatic<AndroidJavaObject>("Companion").Call<AndroidJavaObject>("builder");
        AndroidJavaObject builderWithContext = builder.Call<AndroidJavaObject>("withContext", new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
        //builder.Call("withApplicationType", "Game");
        //builder.Call("withAuthType", "Game", "615107278646-u9tcm5s0j442ubmj1tf1biq0lodi1dbn.apps.googleusercontent.com");
        AndroidJavaObject builerdWithDebugMode = builderWithContext.Call<AndroidJavaObject>("withDebugMode", true);
        AndroidJavaObject buiderWithoutCallLogAndOutgoingCallPermissions = builerdWithDebugMode.Call<AndroidJavaObject>("withoutCallLogAndOutgoingCallPermissions");
        AndroidJavaObject builderwithPrivacyPolicyAndTermsOfServiceAccepted = buiderWithoutCallLogAndOutgoingCallPermissions.Call<AndroidJavaObject>("withPrivacyPolicyAndTermsOfServiceAccepted", false);
        builderwithPrivacyPolicyAndTermsOfServiceAccepted.Call<AndroidJavaObject>("build");
    }
    

    public static void ShowToast(string message)
    {
        if (Application.platform != RuntimePlatform.Android) return;

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject androidToast = new AndroidJavaObject("android.widget.Toast", currentActivity);
        androidToast.CallStatic<AndroidJavaObject>("makeText", currentActivity, message, androidToast.GetStatic<int>("LENGTH_SHORT")).Call("show");
    }
}
