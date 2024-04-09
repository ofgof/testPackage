using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallerIdSDKController : MonoBehaviour
{
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            CallerIdSdkBridge.LaunchCallerIdSetupFlow();
        }
    }
    void OnDestroy()
    {
        Debug.Log("GameController::OnDestroy");
        if (Application.platform == RuntimePlatform.Android)
        {
            CallerIdSdkBridge.ShutDown();
        }
    }
}
